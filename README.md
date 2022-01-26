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
- explain solution (use diagram image)
  - database
  - migrations
  - mockoon
  - service
- explain features
- explain models/API
- explain requirements  
- explain how to run
  - docker-compose project
  - docker-compose standalone infrastructure
    - mockoon outside
  - only service
- explain how to use (requests.http)
- explain validations
- explain tests
  - architecture
  - performance
  - acceptance (BDD)
    - WebFactory
  - validator (fluent validation)
  - mapping
  - unit tests
  - Integration
    - docker-compose ephemeral
- explain logging (fluentbit)
- explain tracing
- explain other considerations
