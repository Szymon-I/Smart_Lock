# Description

## TLDR

Smart Lock is built-in system for remote control of door locks. It allows remote opening and closing of locks to authorized users and downloading logs.

## Server

The server responsible for the operation of the entire system was written using the Django framework in Python. The server is responsible for all events on the client <-> lock path.
The server provides a web client as well as a rest api that has been used through the mobile application.
Users and new locks can be registered in the system, and all events are authorized at several stages to provide the safest and hermetic solution ready for commercial use.
The server establishes communication with the embedded system by sending http packets in the form of json.

Security:
- Authorization of the web client user by means of a session
- Authorizing the user of the mobile application using an authentication token
- User authorization - lock based on their correlation in the database
- Authorization of embedded token, which is produced independently on the server and microcontroller
    using the 'secret password', device's mac address and SHA1 hashing technique

## Web client

The server also provides an html page where basic system functionalities are available, as well as an admin panel to manage the entire system architecture. The technologies used are:
Html, css, javascript, jquery, ajax, bootstrap 4.
The site was written in responsive format, which makes it also transparent for mobile devices.


## Mobile app

The client's mobile application was written in C # in the Xamarin.Android framework for the Android operating system. Thanks to this technology,
it is possible to split back-end and front-end. The back-end can be unchanged regardless of the operating system. When creating applications for the new operating system,
only front-end changes are necessary. The application allows logging into the system, closing and opening locks and displaying usage logs. It offers an encrypted connection,
while the data is transmitted in a lightweight json data exchange format.


## Microcontroller

The microcontroller firmware was written in C ++, based on the rest api server solution. The Nodemcu v2 integrated circuit was used with the esp8266 microprocessor equipped with a WiFi module. This allows the use of WiFi as a global data transmission medium from the user to the end device. The microcontroller works in a loop in which 2 interrupts are serviced. One is responsible for handling all events related to WiFi communication, the other is responsible for UART communication through which the console interface to support the work of the microcontroller has been implemented. By using the Bluetooth HC-06 module, it is possible to control the microcontroller thanks to the BT medium using a telephone with a terminal application. This interface allows you to change the settings of the microcontroller, such as connecting to the network, secret password, network configuration. Servo support mounted to the test lock has been implemented through the use of a microcontroller timer that allows you to generate a PWM signal with a period of 20 ms, and its filling corresponds to the angle of servo control. It was required to match the PWM signal logic from 3.3V to 5V, for which the voltage matching system to the I2C network was used for two reference voltages. In addition, the system has a reed switch connected to the system using the input-pullup technique, which allows you to check whether the door is in the target position before performing any action. The system has also implemented the SPIFFS Flash file system, 1MB in size, which stores the microcontroller settings, and after disconnecting the power supply, the microcontroller reads its previous settings. This eliminates the need for configuration each time the microcontroller is turned on.

# Technologies

- Python
- C#
- C++
- Django
- Android
- Xamarin
- Rest api
- Arduino ide
- Html
- Css
- Javascript
- Ajax
- Bootstrap 4

# Repository content

## Main

* NodeMcu -> Firmware for microcontroller
* server -> Server software
* img -> screenshots
* apk -> mobile app

## Specific

#### Server

* web_client -> responsive web client app
* users -> custom user app
* server -> config folder
* rest_api -> rest api client app
* locks -> db models, secrets and communication protocol

#### Firmware

* smart_lock -> firmware for uC with Servo
* smart_lock_min -> firmware for uC with Leds

# Api example requests

## Get Locks

#### Request

Request: Get

url: http://127.0.0.1:8000/api/get_locks

headers:

``` json
{
    "Authorization": "Token 027be0edfb5aa7b4db8434220e5107811cdb4654"
}
```

body:

``` json
{
}
```

#### Response

body:

``` json
{
    [
    {
        "id": 1,
        "name": "Lock Led 1",
        "location": "second breadboard",
        "open_status": true,
        "hardware": "nodemcu v2, leds, reed",
        "type": "basic",
        "ip_address": "192.168.1.107",
        "mac_adress": "CC:50:E3:4A:54:88",
        "user": [
            1
        ]
    },
    {
        "id": 2,
        "name": "Lock Led 2",
        "location": "second breadboard",
        "open_status": false,
        "hardware": "nodemcu v2, leds, reed",
        "type": "basic",
        "ip_address": "192.168.1.109",
        "mac_adress": "CC:50:E3:4A:4D:F7",
        "user": [
            1,
            2
        ]
    },
    {
        "id": 3,
        "name": "Servo Lock",
        "location": "Main board",
        "open_status": false,
        "hardware": "nodemcu v2, servo,  reed",
        "type": "Advanced",
        "ip_address": "192.168.1.108",
        "mac_adress": "80:7D:3A:3C:34:2A",
        "user": [
            1,
            2
        ]
    }
]
}
```

## Lock action call

#### Request

Request: POST

url: http://127.0.0.1:8000/api/lock_action

headers:

``` json
{
    "Authorization": "Token 6f273c92a218aebf9a701b38162c35e3276c5dd8"
}
```

body:

``` json
{
    "id": "1",
    "action": "open"
}
```

#### Response

body:

``` json
{
    "detail": "Invalid token."
}
```

## Lock logs

#### Request

Request: POST

url: http://127.0.0.1:8000/api/lock_action

headers:

``` json
{
    "Authorization": "Token 027be0edfb5aa7b4db8434220e5107811cdb4654"
}
```

body:

``` json
{
    "id": "1",
    "action": "info"
}
```

#### Response

body:

``` json
{
    [
    {
        "id": 223,
        "action": "open",
        "date": "2020-01-27T15:16:29.851426Z",
        "user": "admin@admin.com"
    },
    {
        "id": 220,
        "action": "close",
        "date": "2020-01-27T15:16:21.453771Z",
        "user": "admin@admin.com"
    },
    {
        "id": 219,
        "action": "open",
        "date": "2020-01-27T15:16:19.099124Z",
        "user": "admin@admin.com"
    },
    {
        "id": 214,
        "action": "close",
        "date": "2020-01-27T15:14:58.300638Z",
        "user": "admin@admin.com"
    },
    {
        "id": 194,
        "action": "open",
        "date": "2020-01-11T22:25:50.818324Z",
        "user": "admin@admin.com"
    },
    {
        "id": 191,
        "action": "close",
        "date": "2020-01-11T22:25:44.071686Z",
        "user": "admin@admin.com"
    },
}
```


# Screenshots

![1](img/1.png "Web starting page")
![2](img/2.png "Login page")
![3](img/3.png "Dashboard")
![4](img/4.png "Admin Panel")
![5](img/5.png "Lock model")
![6](img/6.png "Welcome email")
![7](img/7.jpg "Web description")
![8](img/8.jpg "Mobile dashboard")
![9](img/9.jpg "Lock logs")
![10](img/10.jpg "Mobile error")
![11](img/11.jpg "Mobile login")
![12](img/12.jpg "Mobile web welcome page")
![13](img/13.jpg "Mobile web dashboard")
![14](img/14.jpg "Microcontrollers with led firmware")
![15](img/15.jpg "Microcontroller with servo firmware")
![16](img/16.jpg "Microcontroller with servo firmware")
![17](img/lock.png "Physical lock dimensions")
![18](img/scheme.png "Electronic scheme")