# RPi and Arduino communication sample application 

## Arduino

Application is for sending information requested from RPi. There is simple serial communication to send and recieve data.

## RPi

Raspberry pi works as server sending requests for information and Arduino. Then sends requests to perform some action. This will work as hub and coordinates between various devices.

## Logs

NLog logs are working on both windows and linux machine. 

On windows Application Data folder({specialfolder:folder=ApplicationData}) where logs are created is: ```C:\Users\deore\AppData\Roaming\IOTDevice```

Where as on Linux machine it as at : ```~/.config/IOTDevice```

## References

- [Communication](https://roboticsbackend.com/raspberry-pi-arduino-serial-communication/)
