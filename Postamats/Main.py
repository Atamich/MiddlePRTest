import sys
from Deliverers._5PostApi import _5Post 
from Deliverers.YaDelivery import YaDelivery 
from DeliveryAbsctract import DeliveryAbsctract
from typing import List

arg = None
if(len(sys.argv)>1):
    arg = sys.argv[1]

if arg == None:
    print("Proccesing all deliverys...")
    Deliverys:List[DeliveryAbsctract] = list()

    Deliverys.append(_5Post())
    Deliverys.append(YaDelivery())

    for delivery in Deliverys:
        delivery.ProcessApiData()
else:
    delivery = None
    if arg == '5POST':
        print("Proccesing 5POST...")
        delivery = _5Post()
    if arg == 'YADELIVER':
        print("Proccesing YADELIVER...")
        delivery = YaDelivery()
    if delivery != None:
        delivery.ProcessApiData()
    else:
        print("Delivery " + arg + " not found.")
