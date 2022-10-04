import pyodbc
from PointData import PointData
from datetime import datetime

class Database(object):
    def __init__(self, server, name, username, password, tableName, tableVCID):
        self.conn = pyodbc.connect(f"DRIVER={{ODBC Driver 17 for SQL Server}};SERVER={server};DATABASE={name};UID={username};PWD={password};MARS_Connection=yes", autocommit=True)
        self.tableName = tableName
        self.tableVCID = tableVCID

    def insertData(self, point):
        self.writeNewPoint(point)

    def writeNewPoint(self, pointData):
        pointData = self.__prepareForUploadingDB(pointData)
        query = f"""
        INSERT INTO {self.tableName}
           ([id]
           ,[pvz_id]
           ,[latitude]
           ,[longtitude]
           ,[company]
           ,[name]
           ,[station]
           ,[metro]
           ,[address]
           ,[worktime]
           ,[phone]
           ,[payment]
           ,[limit]
           ,[postamat]
           ,[returns]
           ,[record_date])
        VALUES
           ({pointData.id}
           ,{pointData.pvz_id}
           ,{pointData.latitude}
           ,{pointData.longtitude}
           ,{pointData.company}
           ,{pointData.name}
           ,{pointData.station}
           ,{pointData.metro}
           ,{pointData.address}
           ,{pointData.worktime}
           ,{pointData.phone}
           ,{pointData.payment}
           ,{pointData.limit}
           ,{pointData.postamat}
           ,{pointData.returns}
           ,GetDate())"""

        cursor = self.conn.cursor()
        cursor.execute(query)
        return cursor

    def __prepareForUploadingDB(self, obj):
        """
        Modifies fields with types
        None => NULL
        True => 1
        False => 0
        String => 'String'
        """
        for property, value in vars(obj).items():
            if value is None:
                # setattr(obj, property, '\'\'') 
                setattr(obj, property, 'NULL')
            if value is True:
                setattr(obj, property, '1')
            if value is False:
                setattr(obj, property, '0')
            if isinstance(value, str):
                setattr(obj, property, f"'{value}'")


        return obj

    def ClearDataBase(self):
        query = f"TRUNCATE TABLE {self.tableName}"
        cursor = self.conn.cursor()
        cursor.execute(query)
        return cursor

    def GetLastVC_ID(self, whereQuery):
        query = f"SELECT TOP (1) [VC_ID] FROM {self.tableVCID} {whereQuery} ORDER BY VC_ID DESC"
        cursor = self.conn.cursor()
        cursor.execute(query)
        if cursor.rowcount == 0: 
            return None
        return cursor.fetchone()[0]

    def TryToFindVC_IDByUUID(self, uuid):
        query = f"SELECT TOP (1) [VC_ID] FROM {self.tableVCID} WHERE [UUID] = '{uuid}'"
        cursor = self.conn.cursor()
        cursor.execute(query)
        if cursor.rowcount == 0: 
            return None

        result = cursor.fetchone()[0]
        query = f"UPDATE {self.tableVCID} SET [LastTimeUse] = GETDATE() WHERE [UUID] = '{uuid}'"
        cursor.execute(query)

        return result

    def SetVC_IDUUID(self, vcid, uuid):
        query = f"INSERT INTO {self.tableVCID} (VC_ID, UUID, LastTimeUse) VALUES ('{vcid}', '{uuid}', GETDATE())"
        cursor = self.conn.cursor()
        cursor.execute(query)