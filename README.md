# HshiCorp Vault configuration for .NET

## What is it about
This package is a simple way to introduce secret keys from HashiCorp vault into .NET application. The way it works is it adds or replaces configuration key values with values for same keys existing in HashiCorp vault.

## How to use it
It is pretty simple to introduce configuration values from HashiCorp vault using Configuration.Vault.Hashicorp package. 
Prior to using packages vault info and creadentials need to be added to appsettings.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "MySecretsVault": {
    "Url": "http://127.0.0.1:8200",
    "Backend": "app1",
    "Secret": "Development",
    "Username": "app1_user",
    "Password": "thitisnotpassword"
  }
}
```
Section name you use in the appsettings.json is the one you will reference when invoking the extension method to configure vault in your code in Program.cs
> **_NOTE:_**  Currently package only supports username/password authentication for vault

Package exposes extension methods for Microsoft.Extensions.Configuration.IConfiguration which you can just call from the Program.cs when configuraing services
```cs
builder.Configuration.AddVault("MySecretsVault");
```
