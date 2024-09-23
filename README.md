# Face Recognition Project

This project as part of the course "_Building microservices using asp.net core 5.0 and docker_" from Udemy is based in microservices using face recognition systems from Microsoft.

The main idea for this project is to upload an image and your email address, this will dispatch internal events to:
- Store information in DB.
- Dispatch events using RabbitMQ and Masstransit to perform actions in server side.
- Call Microsoft Congnitive Services Vision Faces to detect people from that image.
- Send an email to the user with the faces found on the image.
- Send push notifications to main view to refresh data.

## Architecture

Docker Swarm Cluster.

![Architecture](/UML/ArchitectureFaces.png)


## Secuence Diagrams
This diagram explains the hole interaction between all components in application.

![Sequence Diagram](/UML/FacesApiSD.png)

## Run Project

### Prerequisites
This project user Face Recognition from Microsoft Cognitive Services Vision Faces.

1. You need to create a _Face API_ service in [Azure Portal](https://portal.azure.com/#home).
2. Once you create the service, you need to copy the _Key_ and _EndPoint_ from api and place them in your docker-compose solution. ![Face Api](/sources/images/MicrosoftFaceApi.png) ![Docker Config](/sources/images/dockerComposeFaceConfiguration.png)

### Run
To run this project you can use `docker-compose up -d`.

## Results

1. Order registration form. ![Form](/sources/images/orderRegistration.png)
2. Order registered. ![Order Registered](/sources/images/orderRegistered.png)
3. Order stored with status Registered ![Order Status Registered](/sources/images/orderStored.png)
4. Order status finally processed and sent email to user. ![Order Sent](/sources/images/orderChangedStatus.png)
5. Email received from service after faces have been proccesed. ![Email](/sources/images/email.png)
