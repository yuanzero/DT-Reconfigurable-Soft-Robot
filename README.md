#  UDP-TCP-for-HoloLens-2

This is a HoloLens2-MRTK-ready template, with TCP and UDP web modules integrated, based on:
 
    ·Unity 2020.3.42f1(LTS)     
    ·OpenXR features    
    ·Windows Mixed Reality Toolkit (MRTK) 2.7.2
    
   
For the users:

    ·Step 1. download it and deploy this project to HoloLens2 directly.
    ·Step 2. In Unity Editor, go to the top and click : Mixed Reality--> Toolkit --> Untilities --> Configure Project for MRTK --> Apply Settings, this step will help your project re-configure for MRTK and avoid errors.
    ·Step 3. In Unity Editor, find the scene "Sucess_sence" in the folder "Scenes", in this scene you can find the gameobject "UDP Communication" as an example
    ·Step 4. in Arduino editor runnning UDP_test.ino, setting your wifi and IP. upload it to ESP32.

One finding is that the TCP connection reads too slowly, I don't know why. I will be very happy if one day you let me know that you create a better TCP connection.

Tips: 

    ·Step 1. The hololens2 and unity have different codes since they are different platforms. It is hard to debug.          
    ·Step 2. The code has some precompiled code, be careful.

Zhongyuan Liao

HKUST

2023.11.28


Plus:

Add serial communication function between Arduino Mega 2560 and ESP32, and the received message can be sent to hololens2.
reference: https://www.programmingboss.com/2021/04/esp32-arduino-serial-communication-with-code.html#gsc.tab=0