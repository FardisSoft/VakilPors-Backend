# VakilPors-Backend

## Installation Guide

1. You need to install .NET SDK 6.0 and SQL Server
2. Set Environment variables for:
    - RAYGAN_SMS_SENDER_NUMBER
    - RAYGAN_SMS_USERNAME
    - RAYGAN_SMS_PASSWORD
3. cd into VakilPors.Web
4. run these commands:
```
dotnet tool install --global dotnet-ef
dotnet ef database update --project ../VakilPors.Data
dotnet run
```
5. open https://localhost:7032/swagger/index.html