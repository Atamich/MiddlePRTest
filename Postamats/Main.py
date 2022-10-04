#Main script
#_5PostApi.GetKey => GetData => Save to DB

from _5PostApi import _5Post 
from DeliveryAbsctract import DeliveryAbsctract
from typing import List



Deliverys:List[DeliveryAbsctract] = list()
Deliverys.append(_5Post())
#Deliverys.append(NewDelivery())

for delivery in Deliverys:
    delivery.ProcessApiData()