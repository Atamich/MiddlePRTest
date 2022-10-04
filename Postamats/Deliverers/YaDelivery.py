import requests
from DeliveryAbsctract import DeliveryAbsctract
from PointData import PointData
from Database import Database


class YaDelivery(DeliveryAbsctract):
    def __init__(self):
        self.TEST_BASE_URL = 'https://b2b.taxi.tst.yandex.net/api/b2b/platform/pickup-points/list'
        self.TEST_TOKEN = '***'
        self.TEST_PLATFORM_STATION_ID = '***'

        self.pvz_id = "YAID"
        self.VCIDWhereQuery = "WHERE VC_ID LIKE 'Y%'"
        self.database = Database("msserv", "5Post", "sa", "$Wv%fd]0G2S&+q(&ns1/", "[Postamats]", "[Postamats-PointsVC]")

        self.cachedVCID = None

    def requestAPI(self):
        """Обращение к АПИ Яндекс Доставки, возвращает json с точками выдачи"""
        headers = {
            'content-type': 'application/json',
            'Authorization': "Bearer %s" % self.TEST_TOKEN # Заменить на токен для продакшена
        }
        data = {}

        response = requests.post(url=self.TEST_BASE_URL, headers=headers, data=data) # Заменить на ссылку для прода
        return response.json()['points']

    def ProcessApiData(self):
        self.database.ClearDataBase(self.pvz_id)
        points = self.requestAPI()
        totalItems = 0
        try:
            for point in points:
                uuid = point['id']
                item: PointData = PointData()
                item.id = self.GetOrGenerateVCID(uuid)
                item.pvz_id = self.pvz_id
                item.company = "YA"
                item.postamat = point['type'] == 'terminal'
                item.longtitude = point['position']['longitude']
                item.latitude = point['position']['latitude']
                item.phone = point['contact']['phone'] if point.get('contact') else None
                item.name = point['name']
                item.address = point['address']['full_address']
                item.payment = self.__paymentMethod(point)
                item.worktime = self.__processWorkHours(point['schedule'])
                self.database.insertData(item)

                totalItems = totalItems + 1
                if totalItems % 500 == 0:
                    print("Appended postamats:", totalItems)

        except Exception as e:
            print(e)
        
        print("Total items:", totalItems)
        return

    def GetOrGenerateVCID(self, uuid):
        vcid = self.database.TryToFindVC_IDByUUID(uuid)
        if vcid is None:
            prevVcid = self.database.GetLastVC_ID(self.VCIDWhereQuery) if self.cachedVCID is None else self.cachedVCID
            if prevVcid is None:
                prevVcid = 'Y0000'

            vcid = super().GenerateVCNumber(prevVcid)
            print('New postamat VC ID -', vcid)
            self.cachedVCID = vcid
            self.database.SetVC_IDUUID(vcid, uuid)
        return vcid

    def __paymentMethod(self, point):
        if ('card_on_receipt' in point['payment_methods'] and 'cash_on_receipt' in point['payment_methods']):
            return "Наличные, карта"
        if 'card_on_receipt' in point['payment_methods']:
            return "Карта"
        if 'cash_on_receipt' in point['payment_methods']:
            return "Наличные"
        else:
            return None

    def __processWorkHours(self, workHours):
        string = ''
        for dayHours in workHours['restrictions']:
            day = ''
            if dayHours['days'] == [1]:
               day = 'Пн: '
            if dayHours['days'] == [2]:
               day = "Вт: "
            if dayHours['days'] == [3]:
               day = "Ср: "
            if dayHours['days'] == [4]:
               day = "Чт: "
            if dayHours['days'] == [5]:
               day = "Пт: "
            if dayHours['days'] == [6]:
               day = "Сб: "
            if dayHours['days'] == [7]:
                day = "Вc: "
            if dayHours['days'] == [1,2,3,4,5,6,7]:
                day = "Все дни: "
            string = string + day + self.__normalizeHour(str(dayHours['time_from']['hours'])) + '-' + \
                    self.__normalizeHour(str(dayHours['time_to']['hours'])) + '<br>'
        return string

    def __normalizeHour(self, hour):
        if len(str(hour)) < 2:
            hour = '0' + str(hour)
        hour = str(hour) + ":00"
        return hour
