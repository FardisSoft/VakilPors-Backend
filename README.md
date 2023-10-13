# VakilPors-Backend

![badge](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/MSPoulaei/20d8309ce4ac8005e22df30d985c6883/raw/code-coverage.json)


## Installation Guide

1. You need to install .NET SDK 6.0 and SQL Server
2. Set Environment variables for:
- DataBase:
    - ConnectionString
- SMS:
    - RAYGAN_SMS_SENDER_NUMBER
    - RAYGAN_SMS_USERNAME
    - RAYGAN_SMS_PASSWORD
- Email:
    - EMAIL_NAME
    - EMAIL_FROM
    - EMAIL_HOST
    - EMAIL_PORT
    - EMAIL_USERNAME
    - EMAIL_PASSWORD
    - EMAIL_USESSL (true/false)
- File:
    - FILE_BUCKET
    - FILE_API_ENDPOINT
    - FILE_ACCESS_KEY
    - FILE_SECRET_KEY

3. cd into VakilPors.Web
4. run these commands:
```
dotnet run
```
5. open https://localhost:7032/swagger/index.html