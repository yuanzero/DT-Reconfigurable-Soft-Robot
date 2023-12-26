#include <WiFi.h>
#include <WiFiUdp.h>

#define RXp2 16
#define TXp2 17

const char* ssid = "SMMG_CAlgroup";
const char* password = "hkustsmmg";

WiFiUDP udp;
IPAddress serverIP;
unsigned int inPort;
unsigned int outPort;

void setup() {
  Serial.begin(115200);
  delay(1000);
  Serial2.begin(9600, SERIAL_8N1, RXp2, TXp2);

  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {
    delay(1000);
    Serial.println("Connecting to WiFi...");
  }

  Serial.println("Connected to WiFi");
  Serial.println("IP address: ");
  Serial.println(WiFi.localIP());

  serverIP = IPAddress(192, 168, 1, 33); // 设置Hololens服务器的IP地址
  inPort = 22222; // 设置接收消息的端口号
  outPort = 22224; // 设置发送消息的端口号

  udp.begin(inPort);
  Serial.print("Listening on UDP port ");
  Serial.println(inPort);
}


void SetMode(char* packetData) {
  int pinToMap = 100; //100 is never reached
  char *arg = strtok(packetData, " ");
  arg = strtok(NULL, " ");
  if (arg != NULL)
  {
    pinToMap = atoi(arg);
  }
  int type;
  arg = strtok(NULL, " ");
  if (arg != NULL)
  {
    type = atoi(arg);
    PinSetMode(pinToMap, type);
  }

  Serial.println( "PinMOde set" );
}

void PinSetMode(int pin, int type) {
  //TODO : vérifier que ça, ça fonctionne
  if (type != 4)
    //DisconnectServo(pin);  //uncommon initial

  switch (type) {
    case 0: // Output
      pinMode(pin, OUTPUT);
      break;
    case 1: // PWM
      pinMode(pin, OUTPUT);
      break;
    case 2: // Analog
      pinMode(pin, INPUT);
      break;
    case 3: // Input_Pullup
      pinMode(pin, INPUT_PULLUP);
      break;
    case 4: // Servo
      //SetupServo(pin);
      break;
  }
}

void WritePinAnalog(char* packetData) {
  int pinToMap = 100;
  char *arg = strtok(packetData, " ");
  arg = strtok(NULL, " ");
  if (arg != NULL)
  {
    pinToMap = atoi(arg);
  }

  int valueToWrite = 0;
  arg = strtok(NULL, " ");
  if (arg != NULL)
  {
    valueToWrite = atoi(arg);
  }

  analogWrite(pinToMap, valueToWrite);
}

void WritePinDigital(char* packetData) {
  int pinToMap = -1;
  char *arg = strtok(packetData, " ");
  arg = strtok(NULL, " ");
  if (arg != NULL)
    pinToMap = atoi(arg);

  int writeValue;
  arg = strtok(NULL, " ");
  if (arg != NULL && pinToMap != -1)
  {
    writeValue = atoi(arg);
    
    if (writeValue == 0)
    {
      digitalWrite(pinToMap, LOW);
      Serial.println( pinToMap );
      Serial.println( "is low");
      Serial.println( "writeValue is" );
      Serial.println( writeValue );
      
     digitalWrite(pinToMap, LOW);
    }
    else
    {
      digitalWrite(pinToMap, HIGH);
      Serial.println( pinToMap );
      Serial.println( "is high");
      Serial.println( "writeValue is" );
      Serial.println( writeValue );
      
      digitalWrite(pinToMap, HIGH);
    }
    
  }
}

void ReadAnalogPin(char* packetData) {
  int pinToRead = -1;
  char *arg = strtok(packetData, " ");
  arg = strtok(NULL, " ");
  if (arg != NULL)
  {
    pinToRead = atoi(arg);
    if (pinToRead != -1)
      analogRead(pinToRead);
  }
  
  String message = "readpin " + String(pinToRead) + " " + analogRead(pinToRead) ;
  udp.beginPacket(serverIP, outPort);
  udp.print(message);
  udp.endPacket();
  Serial.print( "read is: " );
  Serial.println( analogRead(pinToRead) );
}

void ReadDigitalPin(char* packetData) {
  int pinToRead = -1;
  char *arg = strtok(packetData, " ");
  arg = strtok(NULL, " ");
  if (arg != NULL)
  {
    pinToRead = atoi(arg);
  }
  if (pinToRead != -1)
    digitalRead(pinToRead);
}

void BundleReadPin(char* packetData) {
  int pinToRead = -1;
  char *arg = strtok(packetData, " ");
  arg = strtok(NULL, " ");
  if (arg != NULL)
  {
    pinToRead = atoi(arg);
    if (pinToRead != -1)
      analogRead(pinToRead);
  }
}

void loop() {
  int packetSize = udp.parsePacket();
  
//  udp.beginPacket(serverIP, outPort);
//  udp.print(massage_from_arduino);
//  udp.endPacket();
//  Serial.println(massage_from_arduino);
//  delay(1000);
  
  if (packetSize) {
    Serial.print("Received packet of size ");
    Serial.println(packetSize);
    char packetData[255];
    int len = udp.read(packetData, 255);
    if (len > 0) {
      packetData[len] = 0;
    }
    Serial.print("Received message: ");
    Serial.println(packetData);
    
    delay(100);

    // 解析命令并执行相应的操作
  if (packetData[0] == 's') {
    SetMode(packetData);
  } else if (packetData[0] == 'd') {
    WritePinDigital(packetData);
  } else if (packetData[0] == 'a') {
    WritePinAnalog(packetData);
  } else if (packetData[0] == 'rd') {
    ReadDigitalPin(packetData);
  } else if (packetData[0] == 'r') {
    ReadAnalogPin(packetData);
  } else if (packetData[0] == 'br') {
    BundleReadPin(packetData);
  } else if (packetData[0] == 'm') {
    Serial2.println(packetData); // 串口通信
    //Serial.println("send to mega");
  } else {
    // 未知命令处理
    Serial.println("Unknown command");
  }
  
  }
  
  String massage_from_arduino = Serial2.readString(); // 串口通信
  if (!massage_from_arduino.isEmpty()){
      //String message = "Hello from ESP32!";
      udp.beginPacket(serverIP, outPort);
      //udp.print(message);
      //Serial.println("Sent message: " + message);
      udp.print(massage_from_arduino);
      udp.endPacket();
      Serial.println(massage_from_arduino);
    
      delay(100);
    }
}
