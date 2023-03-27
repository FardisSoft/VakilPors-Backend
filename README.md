# VakilPors-Backend

## Installation Guide

- You need to install .NET SDK 6.0 and SQL Server
- cd into VakilPors.Web
- run these commands:
```
dotnet tool install --global dotnet-ef
dotnet ef database update --project ../VakilPors.Data
dotnet run
```
- open https://localhost:7032/swagger/index.html