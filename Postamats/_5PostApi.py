import requests
from DeliveryAbsctract import DeliveryAbsctract
from Database import Database
from PointData import PointData

class _5Post(DeliveryAbsctract):

    def __init__(self):
        self.TEST_BASE_URL = 'https://api-preprod-omni.x5.ru'
        self.TEST_ID = '***'
        self.TEST_API_KEY = '***'
        self.PROD_BASE_URL = 'https://api-omni.x5.ru'
        self.PROD_ID = '***'
        self.PROD_API_KEY = '***'

        self.VCIDWhereQuery = "WHERE VC_ID > '50000'"
        self.database = Database("msserv", "5Post", "sa", "***", "[Postamats]", "[Postamats-PointsVC]")

    def __generateBearer(self):
        url: str = f"{self.PROD_BASE_URL}/jwt-generate-claims/rs256/1?apikey={self.PROD_API_KEY}"
        headers = {'content-type': 'application/x-www-form-urlencoded'}
        data = {
            "subject": "OpenAPI",
            "audience": "A122019!"
        }
        response = requests.post(url=url, headers=headers, data=data)
        return response.json()['jwt']

    def GetOrGenerateVCID(self, uuid):
        vcid = self.database.TryToFindVC_IDByUUID(uuid)
        if vcid is None:
            prevVcid = self.database.GetLastVC_ID(self.VCIDWhereQuery)
            if prevVcid is None:
                prevVcid = '50000'
            
            vcid = super().GenerateVCNumber(prevVcid)
            print('New postamat VC ID -', vcid)
            self.database.SetVC_IDUUID(vcid,uuid)

        return vcid

    def ProcessApiData(self):
        i = 0
        
        url: str = f"{self.PROD_BASE_URL}/api/v1/pickuppoints/query"
        headers = {'content-type': 'application/json',
                   'authorization': f'Bearer {self.__generateBearer()}'}

        self.database.ClearDataBase()
        attempt = 0
        totalItems = 0
        while True: 
            try:
                data = {
                    "pageSize": 1000,
                    "pageNumber": i
                }
                response = requests.post(url=url, headers=headers, json=data)
                x = response.json()
                for postamat in response.json()['content']:
                    if postamat['extStatus'] != 'ACTIVE': # if point is not active
                        continue

                    uuid = postamat['id']
                    
                    item:PointData = PointData()
                    item.id = self.GetOrGenerateVCID(postamat['id'])
                    item.pvz_id = "5POSTID"
                    item.latitude = postamat['address']['lat']
                    item.longtitude = postamat['address']['lng']
                    item.company = '5PO'
                    item.name = postamat['name']
                    item.metro = postamat['address']['metroStation'] if postamat['address'].get('metroStation') else None
                    item.address = postamat['fullAddress']
                    item.worktime =  self.__processWorkHours(postamat['workHours']) if len(postamat['workHours']) > 0 else None
                    item.phone = self.PhoneFormat(postamat['phone'])
                    item.postamat = True
                    item.returns = postamat['returnAllowed']
                    item.payment = self.__paymentMethod(postamat)
                    item.limit = self.__processLimits(postamat['cellLimits'])

                    self.database.insertData(item)

                    totalItems = totalItems + 1
                    if totalItems % 500 == 0:
                        print("Appended postamats:", totalItems)
                    
                if i>=response.json()['totalPages']:
                    break
                i = i + 1
                attempt = 0
            except Exception as e:
                print(e)
                print('Attempt #', attempt)
                attempt = attempt + 1
                if(attempt > 3):
                    i = i + 1
        print("Total items:", totalItems)
        return

    def __processWorkHours(self, workHours):
        string = ''
        for dayHours in workHours:
            day = ''
            if(dayHours['day'] == 'MON'):
                day = "Пн: "
            if(dayHours['day'] == 'TUE'):
                day = "Вт: "
            if(dayHours['day'] == 'WED'):
                day = "Ср: "
            if(dayHours['day'] == 'THU'):
                day = "Чт: "
            if(dayHours['day'] == 'FRI'):
                day = "Пт: "
            if(dayHours['day'] == 'SAT'):
                day = "Сб: "
            if(dayHours['day'] == 'SUN'):
                day = "Вc: "
            string = string + day + dayHours['opensAt'][:-3] + '-' + dayHours['closesAt'][:-3] + '<br>'
        return string

    def __paymentMethod(self, postamat):
        if(postamat['cashAllowed'] and postamat['cardAllowed']):
            return "Наличные, карта"
        if(postamat['cashAllowed']):
            return "Наличные"
        if(postamat['cardAllowed']):
            return "Карта"

    def __processLimits(self, cellLimits):
        return f"Ширина: {cellLimits['maxCellWidth']}мм <br>Высота: {cellLimits['maxCellHeight']}мм <br>Длина: {cellLimits['maxCellLength']}мм <br>Вес: {cellLimits['maxWeight']/1000000}кг <br>"
