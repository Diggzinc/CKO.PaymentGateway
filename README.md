# CKO.PaymentGateway
CKO take-home challenge

```
docker-compose --profile infrastructure up --no-build --detach
```

```
docker-compose down --volumes
```

```
dotnet clean
dotnet restore
dotnet build
dotnet run --project .\src\CKO.PaymentGateway.Host.Api\CKO.PaymentGateway.Host.Api.csproj
```
