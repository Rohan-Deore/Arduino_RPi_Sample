
int LED = 12;
int status = 1;

int res = 0; /* declaring the variable that will store the value of photoresistor*/
int sensor = A0;/* assigning Arduino pin for photoresistor*/
int LM35 = A1;

void setup() {
  Serial.begin(9600);
  pinMode(LED, OUTPUT); /* assigning mode to LED pin */
}

void loop() {
  // put your main code here, to run repeatedly:
res = analogRead(sensor); /* getting the value of photoresistor*/

Serial.print(res); /* displaying the photoresistor value on serial monitor */

    if(res < 700) { /* when the value of sensor is less than 100 */
Serial.println(" : Low intensity ");
digitalWrite(LED,LOW); /* keep the LED off*/

    }
    else { /* otherwise turn the light on */
Serial.println(" : High Intensity ");
digitalWrite(LED,HIGH); /* turn the LED on*/

    }

  float temp_val;
  int tempADCValue = analogRead(LM35); /* Read Temperature */
  temp_val = (tempADCValue * 4.88);	/* Convert adc value to equivalent voltage */
  temp_val = (temp_val/10);	/* LM35 gives output of 10mv/°C */
  Serial.print("Temperature = ");
  Serial.print(temp_val);
  Serial.print(" Degree Celsius\n");

delay(1000);
}