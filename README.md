# VakilPors-Backend

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
3. cd into VakilPors.Web
4. run these commands:
```
dotnet tool install --global dotnet-ef
dotnet ef database update --project ../VakilPors.Data
dotnet run
```
5. open https://localhost:7032/swagger/index.html