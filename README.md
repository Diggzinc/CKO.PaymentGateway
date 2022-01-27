# CKO.PaymentGateway

[![Build and Test .NET](https://github.com/Diggzinc/CKO.PaymentGateway/actions/workflows/build-and-test.yaml/badge.svg)](https://github.com/Diggzinc/CKO.PaymentGateway/actions/workflows/build-and-test.yaml)

CKO take-home challenge

## Prerequisites

Software 
- Docker version 20.10.12, build e91ed57 
- dotnet 6.0.200-preview.22055.15
- Visual Studio Community Version 17.1.0 Preview 4.0 (with ASP.NET and web development workload)

The following ports should be available on your local machine, although configurable it's advised to just use the defaults.

## Build Solution (command line)
<p align="center">
<a href="https://asciinema.org/a/464738"><img  src="https://asciinema.org/a/464738.svg"></a>
</p>



## Run tests (command line)

All Tests
<p align="center">
<a href="https://asciinema.org/a/464742"><img  src="https://asciinema.org/a/464742.svg"></a>
</p>

Only Unit Tests
<p align="center">
<a href="https://asciinema.org/a/464743"><img  src="https://asciinema.org/a/464743.svg"></a>
</p>

Performance Tests
<p align="center">
<a href="https://asciinema.org/a/464740"><img  src="https://asciinema.org/a/464740.svg"></a>
</p>


## Run Payment Gateway Application

### Run with `dotnet-cli`

<p align="center">
<a href="https://asciinema.org/a/464741"><img  src="https://asciinema.org/a/464741.svg"></a>
</p>

### Visual Studio (with `docker-compose` project)

<p align="center">
<img  src="./assets/vs-compose.gif">
</p>

### Visual Studio standalone 
#### Setup Infrastructure
<p align="center">
<a href="https://asciinema.org/a/464739"><img  src="https://asciinema.org/a/464739.svg"></a>
</p>






```
docker-compose --profile infrastructure up --no-build --detach
docker ps
```

```
docker-compose down --volumes
```

```
dotnet clean .\CKO.PaymentGateway.Challenge.sln
dotnet restore .\CKO.PaymentGateway.Challenge.sln
dotnet build .\CKO.PaymentGateway.Challenge.sln --no-restore
dotnet run --no-build --project .\src\CKO.PaymentGateway.Host.Api\CKO.PaymentGateway.Host.Api.csproj
dotnet test .\CKO.PaymentGateway.Challenge.sln
```

- pre-requisites
- explain solution (use diagram image)
  - database
  - migrations
  - mockoon
  - service
- explain features
- explain models/API
  - explain authentication
- explain requirements
- explain architecture
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
    - add diagram from a run
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
  - resiliency
  - cloud provider deployments
  - parameter store for configuration
  - database encryption
