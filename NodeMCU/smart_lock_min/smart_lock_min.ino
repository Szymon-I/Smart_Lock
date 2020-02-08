#include <stdio.h>
#include <ESP8266WebServer.h>
#include <ArduinoJson.h>
#include <Hash.h>
#include <Servo.h>
#include <FS.h>
#define HTTP_REST_PORT 8000
#define WIFI_RETRY_DELAY 500
#define MAX_WIFI_INIT_RETRY 10
#define SETTINGS_PATH "/settings.txt"
#define RED_LED_PIN 4
#define GREEN_LED_PIN 2
#define CONTACT_PIN 16

String SECRET = "default";
String wifi_ssid = "default";
String wifi_passwd = "default";
String node_key;
byte lock_action;
ESP8266WebServer http_rest_server(HTTP_REST_PORT);

//******** setup & wifi  ******************//

int init_wifi()
{
    int retries = 0;

    Serial.println("Connecting to WiFi AP..........");

    WiFi.mode(WIFI_STA);
    WiFi.begin(wifi_ssid, wifi_passwd);
    // check the status of WiFi connection to be WL_CONNECTED
    while ((WiFi.status() != WL_CONNECTED) && (retries < MAX_WIFI_INIT_RETRY))
    {
        retries++;
        delay(WIFI_RETRY_DELAY);
        Serial.print("#");
    }
    return WiFi.status(); // return the WiFi connection status
}

//******** server & json  ******************//

void get_lock_status()
{
    StaticJsonBuffer<200> jsonBuffer;
    JsonObject &jsonObj = jsonBuffer.createObject();
    char JSONmessageBuffer[200];

    jsonObj["lock_action"] = lock_action;
    jsonObj.prettyPrintTo(JSONmessageBuffer, sizeof(JSONmessageBuffer));
    http_rest_server.send(200, "application/json", JSONmessageBuffer);
}

int json_to_resource(JsonObject &jsonBody)
{
    lock_action = jsonBody["lock_action"];
    String key = jsonBody["key"].as<String>();

    if (key != node_key)
    {
        return 1;
    }

    if (digitalRead(CONTACT_PIN) != 0)
    {
        return 2;
    }

    if (lock_action == 1)
    {
        digitalWrite(GREEN_LED_PIN,HIGH);
        digitalWrite(RED_LED_PIN,LOW);
    }
    else
    {
        digitalWrite(GREEN_LED_PIN,LOW);
        digitalWrite(RED_LED_PIN,HIGH);
    }

    return 0;
}

void post_lock_action()
{
    StaticJsonBuffer<500> jsonBuffer;

    JsonObject &jsonBody = jsonBuffer.parseObject(http_rest_server.arg("plain"));

    if (!jsonBody.success())
    {
        Serial.println("error in parsin json body");
        http_rest_server.send(400);
    }
    else
    {
        if (http_rest_server.method() == HTTP_POST)
        {
            int status = json_to_resource(jsonBody);
            // status = 0 -> no error
            // status = 1 -> wrong token
            // status = 2 -> reed switch error

            if (status == 0)
            {
                http_rest_server.send(201);
            }
            else if (status == 1)
            {
                http_rest_server.sendHeader("Error", "wrong token");
                http_rest_server.send(401);
            }
            else if (status == 2)
            {
                http_rest_server.sendHeader("Error", "reed switch error");
                http_rest_server.send(401);
            }
        }
    }
}

void config_rest_server_routing()
{
    http_rest_server.on("/lock_action", HTTP_GET, get_lock_status);
    http_rest_server.on("/lock_action", HTTP_POST, post_lock_action);
}

// connect to network with given settings
void connect_to_network()
{
    if (init_wifi() == WL_CONNECTED)
    {
        Serial.print("Connetted to ");
        Serial.print(wifi_ssid);
        Serial.print("--- IP: ");
        Serial.println(WiFi.localIP());
        config_rest_server_routing();

        http_rest_server.begin();
        Serial.println("HTTP REST Server Started");
    }
    else
    {
        Serial.print("Error connecting to: ");
        Serial.println(wifi_ssid);
    }

    Serial.println();
}

void print_network_info()
{
    Serial.print("network name:");
    Serial.println(wifi_ssid);
    Serial.print("network password:");
    Serial.println(wifi_passwd);
    Serial.print("secret:");
    Serial.println(SECRET);
}

void set_network(String command)
{
    command.remove(0, 4);
    String delimiter = "&";
    String token;
    size_t pos = 0;

    pos = command.indexOf(delimiter);
    wifi_ssid = command.substring(0, pos);
    command.remove(0, pos + delimiter.length());

    pos = command.indexOf(delimiter);
    wifi_passwd = command.substring(0, pos);
    command.remove(0, pos + delimiter.length());

    SECRET = command;

    File file = SPIFFS.open(SETTINGS_PATH, "w");
    file.print(wifi_ssid + '\n' + wifi_passwd + '\n' + SECRET + '\n');
    file.close();
}

void read_settings(bool enable_print)
{
    File file = SPIFFS.open(SETTINGS_PATH, "r");
    int i = 0;
    while (file.available())
    {

        String line = file.readStringUntil('\n');
        if (i == 0)
        {
            wifi_ssid = line;
        }
        else if (i == 1)
        {
            wifi_passwd = line;
        }
        else if (i == 2)
        {
            SECRET = line;
        }
        if (enable_print)
        {
            Serial.println(line);
        }
        i++;
    }
    file.close();
    node_key = sha1(SECRET + WiFi.macAddress());
}
void print_help()
{

    Serial.println("Available commands:");
    Serial.println("info - print actual network settings");
    Serial.println("set - set network settings in format:\nset&wifi_ssid&wifi_passwd&secret");
    Serial.println("connect - connect to network with actual settings");
    Serial.println("disconnect - disconnect from current network");
    Serial.println("mac - print MAC adress of microcontroller");
    Serial.println("ip - print local ip of microcontroller");
    Serial.println("status - display status of actual network connection");
    Serial.println("help or h - print help message");
}

void cli_action(String command)
{
    if (command == "info")
    {
        print_network_info();
    }
    else if (command.substring(0, 3) == "set")
    {
        set_network(command);
    }
    else if (command == "connect")
    {
        connect_to_network();
    }
    else if (command == "disconnect")
    {
        Serial.println("Disconnected from:" + wifi_ssid);
        WiFi.disconnect();
    }
    else if (command == "mac")
    {
        Serial.print("Mac adress:");
        Serial.println(WiFi.macAddress());
    }
    else if (command == "ip")
    {
        Serial.print("IP adress:");
        Serial.println(WiFi.localIP());
    }
    else if (command == "status")
    {
        if (WiFi.status() == WL_CONNECTED)
        {

            Serial.print("Connetted to ");
            Serial.print(wifi_ssid);
            Serial.print("--- IP: ");
            Serial.println(WiFi.localIP());
        }
        else
        {
            Serial.println("Not connected to network");
        }
    }
    else if (command == "help" || command == "h")
    {
        print_help();
    }
    else
    {
        Serial.println("Command unknown");
    }
}

void setup()
{
    pinMode(RED_LED_PIN,OUTPUT);
    pinMode(GREEN_LED_PIN,OUTPUT);
    digitalWrite(RED_LED_PIN,LOW);
    digitalWrite(GREEN_LED_PIN,LOW);
    pinMode(CONTACT_PIN, INPUT_PULLUP);
    pinMode(LED_BUILTIN, OUTPUT);
    digitalWrite(LED_BUILTIN, HIGH);
    Serial.begin(9600);
    SPIFFS.begin();
    read_settings(false);
    connect_to_network();
    print_help();
}

void loop()
{
    http_rest_server.handleClient();
    if (Serial.available())
    {
        String command = Serial.readStringUntil('\n');
        cli_action(command);
    }
}
