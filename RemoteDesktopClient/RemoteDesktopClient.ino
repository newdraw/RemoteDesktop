#include <Arduino.h>
#include <SSD1306.h>
#include <WiFi.h> 

#define FRAME_WIDTH  128
#define FRAME_HEIGHT  64
#define FRAME_BYTES FRAME_WIDTH * FRAME_HEIGHT / 8

#define DATA_FRAME 0
#define DATA_FRAMEDIFF 1
#define DATA_CONFIG 2

bool config_showinfo = false;
 
int16_t readBytes(WiFiClient* client, int16_t bytes, byte* buff, int timeout = 3000)
{ 

  uint32_t start = millis();
  while(client->available() < bytes)
  {
    if(millis() - start > timeout)
    {
      throw; 
    } 
    delay(1);
  }

  for(int i = 0;i<bytes;i++)
  {
    buff[i] = client->read();
  }   
  return bytes;
}


template <typename T>   
T read(WiFiClient* client)          
{
  int16_t bytes = sizeof(T);
  byte buff[bytes];
  readBytes(client, bytes, buff);
  return *(T*)buff;
} 

void updateDisplay(SSD1306* display, byte* frame, bool update = true)
{
  display->clear(); 
  for(int y=0;y<FRAME_HEIGHT;y++)
  {
    for(int x=0;x<FRAME_WIDTH;x++)
    {
      int bits = y * FRAME_WIDTH + x;
      int index = bits / 8;
      int bit = bits % 8;
      if(frame[index] & (1<<bit))
      {
        display->setPixel(x, y);
      } 
    }
  } 

  if(update)
  {
    display->display(); 
  }
}

int16_t handleFrameData(WiFiClient* client, byte* frame)
{
  return readBytes(client, FRAME_BYTES, frame);
}
 
int16_t handleFrameDiffData(WiFiClient* client, byte* frame)
{
  int16_t len = read<int16_t>(client); 
  if(len > FRAME_BYTES)
  {
      Serial.println("长度错误");
      throw;
  } 

  byte buff[FRAME_BYTES]; 
  readBytes(client, len, buff); 
  
  
  int16_t pBuff = 0; 
  while(pBuff < len)
  {
    int16_t changeIndex = *(int16_t*)(buff+pBuff);
    pBuff+=2;  
    if(changeIndex >= FRAME_BYTES || changeIndex < 0)
    {
        Serial.println("变化索引号错误");
        throw;
    }
    int16_t changeCount = *(int16_t*)(buff+pBuff);
    pBuff+=2;  
    
    if(changeCount >= FRAME_BYTES || changeIndex < 0 || changeIndex+changeCount >= FRAME_BYTES)
    {
        Serial.println("变化字节数错误");
        throw;
    } 
 
    memcpy(frame + changeIndex, buff + pBuff, changeCount);
    pBuff+=changeCount; 
  } 
  return len + 2;
}

void setup() { 

  Serial.begin(115200);

  WiFi.begin("ssid", "password");
  while(!WiFi.isConnected())
  {
    delay(100);
  }    
  
  WiFiServer server(5720);
  server.begin();

  SSD1306 display(0x3c, 23, 22, GEOMETRY_128_64, I2C_ONE, 1024*1024*10);
  display.setContrast(10, 5, 0);
  display.init(); 

  
  const int indicatorWidth = FRAME_WIDTH * 0.98;
  int indicatorX = -indicatorWidth;
  while(true)
  {  
    display.clear();
    display.setFont(ArialMT_Plain_10);
    display.drawString(0, 0, "Device IP");
    display.drawString(0, FRAME_HEIGHT - 10 - 4, "Listening...");
    display.setFont(ArialMT_Plain_16);
    display.drawString(0, 10+4, WiFi.localIP().toString());
   
    display.drawHorizontalLine(indicatorX, FRAME_HEIGHT - 1, indicatorWidth);
    indicatorX++;
    if(indicatorX == FRAME_WIDTH)
    {
      indicatorX = -indicatorWidth;
    }
    display.display();
    delay(10);
    

    WiFiClient client = server.available();
    if(!client)
    {  
      continue;
    }  

    display.setFont(ArialMT_Plain_10);
    uint32_t frames = 0;
    uint32_t start = millis();
    uint64_t bytes = 0;
    byte frame[FRAME_BYTES];
 
    while(true)
    {   
      byte type = read<byte>(&client);
      bytes++;
      
      switch(type) 
      {
        case DATA_FRAME:
          bytes+=handleFrameData(&client, frame);
          break;
        case DATA_FRAMEDIFF:
          bytes+=handleFrameDiffData(&client, frame);
          break;
        case DATA_CONFIG:
          config_showinfo = read<bool>(&client);
          bytes++;
          continue;
      } 
       
      updateDisplay(&display, frame, false);  
      
      frames++;
      
      if(config_showinfo)
      { 
        
        int elapse = millis()-start; 
        char buff[100]; 
        sprintf(
          buff, 
          "%d fps, %d KBps", 
          frames * 1000 / elapse,
          (int)((bytes / 1024) * 1000 / elapse)
        );
        display.setColor(BLACK);
        display.fillRect(0,0, strlen(buff) * (10/2), 11);
        display.setColor(WHITE);
        display.drawString(0, 0, buff); 
      } 
      display.display();
    } 
  }
  
}

void loop() { 
}
