import sqlite3
from datetime import datetime, timedelta
import random

class PopulateData:
    def AddData(self):
        connection = sqlite3.connect('../ServerDashboard/bin/Debug/net8.0-windows/iot.db')
        cur = connection.cursor()
        
        data = self.CreateData('RaspberryPi-1')
        data2 = self.CreateData('RaspberryPi-2')
        data.extend(data2)
        
        # device_name, status_time, status
        cur.executemany('INSERT INTO DeviceData(device_name, status_time, status) VALUES(?, ?, ?)', data)
        connection.commit()
        
        connection.close()

    def CreateData(self, name):
        statusTime = datetime(2024, 12, 1, 8)
        delta = timedelta(minutes=1)
        data = list()
        limit = datetime(2024, 12, 1, 18)
        currentStatus = True
        while(statusTime < limit):
            sameStatus = random.randrange(7, 14)
            for x in range(sameStatus):
                    data.append((name, statusTime, currentStatus))
                    statusTime = statusTime + delta
            
            currentStatus = not currentStatus


        return data

print("Application started!")
obj = PopulateData()
obj.AddData()
print("Application finished!")