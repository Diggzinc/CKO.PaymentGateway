# CKO.PaymentGateway

[![Build and Test .NET](https://github.com/Diggzinc/CKO.PaymentGateway/actions/workflows/build-and-test.yaml/badge.svg)](https://github.com/Diggzinc/CKO.PaymentGateway/actions/workflows/build-and-test.yaml)

CKO take-home challenge

```
docker-compose --profile infrastructure up --no-build --detach
```

```
docker-compose down --volumes
```

```
dotnet clean .\CKO.PaymentGateway.Challenge.sln
dotnet restore .\CKO.PaymentGateway.Challenge.sln
dotnet build .\CKO.PaymentGateway.Challenge.sln
dotnet run --project .\src\CKO.PaymentGateway.Host.Api\CKO.PaymentGateway.Host.Api.csproj
```
