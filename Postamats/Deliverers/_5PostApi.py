import requests
from DeliveryAbsctract import DeliveryAbsctract
from Database import Database
from PointData import PointData

class _5Post(DeliveryAbsctract):

    def __init__(self):
        # self.BASE_URL = 'https://api-omni.x5.ru'
        # self.API_ID = '***'
        # self.API_KEY = '***'

        # For debug
        self.BASE_URL = 'https://api-preprod-omni.x5.ru'
        self.API_ID = '***'
        self.API_KEY = '***'

        self.pvz_id = "5POSTID"
        self.VCIDWhereQuery = "WHERE VC_ID LIKE '5%'"
        self.database = Database("msserv", "5Post", "sa", "]BVzo;o4I89}KWP`EYrD", "[Postamats]", "[Postamats-PointsVC]")

        self.cachedVCID = None

    def __generateBearer(self):
        url: str = f"{self.BASE_URL}/jwt-generate-claims/rs256/1?apikey={self.API_KEY}"
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
            prevVcid = self.database.GetLastVC_ID(self.VCIDWhereQuery) if self.cachedVCID is None else self.cachedVCID
            if prevVcid is None:
                prevVcid = '50000'
            
            vcid = super().GenerateVCNumber(prevVcid)
            print('New postamat VC ID -', vcid)
            self.cachedVCID = vcid
            self.database.SetVC_IDUUID(vcid,uuid)

        return vcid

    def ProcessApiData(self):
        i = 0
        
        url: str = f"{self.BASE_URL}/api/v1/pickuppoints/query"
        headers = {'content-type': 'application/json',
                   'authorization': f'Bearer {self.__generateBearer()}'}

        self.database.ClearDataBase(self.pvz_id)
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
                    try:
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

                    except Exception as e:
                        print(e)
                    
                if i>=response.json()['totalPages']:
                    break
                i = i + 1
            except Exception as e:
                print(e)

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
