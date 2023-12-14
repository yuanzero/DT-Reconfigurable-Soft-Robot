// this sample code provided by www.programmingboss.com
void setup() {
  Serial.begin(9600);
  Serial.println("Hello Boss");
  delay(1500);
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
//  Serial.println("Hello Boss");
//  delay(1500);
  if (Serial.available()) {
      // 读取接收到的命令
      char ch = Serial.read(); // 读取单个字符 if (ch == 'm') 
      char packetData[255];
      int len = Serial.readBytes(packetData, 255); // 从串口读取数据并返回读取的字节数
      if (len > 0) {
        packetData[len] = 0; // 添加空字符 '\0' 以确保字符数组以空字符结尾
        Serial.print("Received message: ");
        Serial.println(packetData);
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
        } else {
          // 未知命令处理
          Serial.println("Unknown command");
        }
      }
    }
  }
