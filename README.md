# CKO Payment Gateway

[![Build and Test .NET](https://github.com/Diggzinc/CKO.PaymentGateway/actions/workflows/build-and-test.yaml/badge.svg)](https://github.com/Diggzinc/CKO.PaymentGateway/actions/workflows/build-and-test.yaml)

**Table of Contents**

- [CKO Payment Gateway](#cko-payment-gateway)
  - [Prerequisites](#prerequisites)
  - [Architecture](#architecture)
  - [Domain and Assumptions](#domain-and-assumptions)
    - [General Assumptions](#general-assumptions)
    - [Data Models](#data-models)
      - [JSON Representations](#json-representations)
  - [The Payment Gateway API](#the-payment-gateway-api)
    - [`POST /api/v1/payments`](#post-apiv1payments)
    - [`GET /api/v1/payments/{payment-id}`](#get-apiv1paymentspayment-id)
    - [`GET /health`](#get-health)
  - [Setup](#setup)
    - [Setup with `dotnet-cli`](#setup-with-dotnet-cli)
      - [Build](#build)
      - [Run Tests](#run-tests)
        - [All Tests](#all-tests)
        - [Specific Category Tests](#specific-category-tests)
        - [Performance Tests](#performance-tests)
      - [Run API](#run-api)
      - [Environment Clean Up](#environment-clean-up)
    - [Setup with `Visual Studio 2022 Community`](#setup-with-visual-studio-2022-community)
      - [Run with `docker-compose` project](#run-with-docker-compose-project)
      - [Run with standalone `CKO.PaymentGateway.Host.Api` project](#run-with-standalone-ckopaymentgatewayhostapi-project)
  - [How to use Payment Gateway API](#how-to-use-payment-gateway-api)
    - [Make requests](#make-requests)
    - [Introduce Payment Failures](#introduce-payment-failures)
  - [Relevant Technical Comments/Considerations](#relevant-technical-commentsconsiderations)
    - [Comments on the Tests](#comments-on-the-tests)
      - [Unit Tests](#unit-tests)
      - [Integration Tests](#integration-tests)
      - [Acceptance Tests](#acceptance-tests)
      - [Architecture Tests](#architecture-tests)
      - [Performance Tests](#performance-tests-1)
    - [Comments on Observability](#comments-on-observability)
      - [Logging](#logging)
      - [Tracing](#tracing)
    - [Comments on Responsibilities](#comments-on-responsibilities)
    - [Comments on Code Style / Architecture](#comments-on-code-style--architecture)
  - [Areas for Improvement](#areas-for-improvement)

## Prerequisites

The following section describes the prerequisites in order to build and run this challenge solution.

**Software Requirements** 

The following list of software is based on the versions I've used to build this challenge, therefore, I would recommend to at least stick with the major version of each one of these.
- `Windows 10`
- `Docker version 20.10.12, build e91ed57` 
- `dotnet 6.0.200-preview.22055.15`
- `Visual Studio Community Version 17.1.0 Preview 4.0` (with `ASP.NET` and web development workload)

**System requirements**

The following `ports` should be available on your local machine, although configurable it's advised to just use the defaults provided.

| Port | Usage                        |
| ---- | ---------------------------- |
| 5342 | PostgreSQL database          |
| 3000 | Acquiring Bank API (Mockoon) |
| 5000 | Payment Gateway API          |

## Architecture

The **Payment Gateway API** solution was made up to run inside a `docker` environment as presented under the following diagram.

<p align="center">
<img  src="./assets/architecture.png">
</p>

The solution consists of the following 4 containers:

**database** - container that leverages [PostgreSQL](https://www.postgresql.org/) to hold the stored data for payments. 

**migrations** - container that runs on docker-compose stack startup using [Flyway](https://flywaydb.org/) waiting for the database to be ready then applies the migrations for the necessary schemas (*depends on the database container*). 

**acquiring bank api** - a container that's running [Mockoon CLI](https://mockoon.com/cli/) as a service with pre-baked responses for the required uses cases to the Acquiring Bank.

**payment gateway api** - The Payment Gateway API application built with .NET6 that fulfill's the use cases for the challenge at hand (*depends on the migrations container*).

## Domain and Assumptions

### General Assumptions
After some research by payment cards in [general](https://baymard.com/checkout-usability/credit-card-patterns) and payment gateway [responsibilities](https://en.wikipedia.org/wiki/Payment_gateway), the following assumptions were made:
- Although a payment gateway has many back and forward iterations with the merchant and the acquiring bank, this demo will simulate all those steps in one go on the processing request.
  - Those steps can be seen as records entries for the payment when retrieved;
  - The available record types are `Issued`, `Verifying`, `Verified`, `Authorizing`, `Authorized`, `Processing`, `Processed`, `Failed`.
- The payment gateway solution will **NOT** store the whole payment card information:
  - Only the last 4 digits of the card, **NOT** the whole number ⚠️;
  - The length of the card number;
  - expiry date;
  - holder name;
  - Security code (cvc/cvv) will **NOT** be stored ⚠️.
- In order to interact with the API authentication is required.
  - We assume that the merchant uses a `JWT` token with a claim `merchantId:<guid>`.
  - Authentication is usually used through an authority such as an Identity Service Provider that issues these tokens, but, for demonstration purposes the application will use a self-signed token with symmetric key material `your-256-bit-secret`, therefore, these tokens can be easily created on the [JWT.io](https://jwt.io/) website.
- While the domain model is thoroughly tested, the other tests are mostly for demonstration purposes in order to show evidence of the way to test certain things on an application, offering different viewpoints.
  - A following section will go over the tests and their intended purpose.
  - Given this though, I firmly believe testing is paramount, my father is a carpenter and he always says `measure twice, cut once`. As a rule of thumb I carry this over to testing as well.

### Data Models

The following examples refer to the API models (view models) that are exposed to the user and not the domain models used by the application, which are translated from these view models.

If you want to see an in-depth explanation of the domain models, please refer to the documentation under the `CKO.PaymentGateway.Models` project.

 In that project you can also find the domain constraints such as the allowed length for the number of a card, supported currencies among other things.

#### JSON Representations

<details>
  <summary><b>ProcessPaymentJsonRequest.json</b> (click to <i>expand/collapse</i>)</summary>
  
```json
{
  "card": {
    "number": "1111 1111 1111 1235",
    "securityCode": "111",                
    "expiryDate": "01/27",
    "holder": "John Doe"
  },
  "charge": {
    "currency": "EUR",
    "amount": 0.01
  },
  "description": "AMZN Purchase"
}
```
</details>

<details>
  <summary><b>PaymentJsonResponse.json</b> (click to <i>expand/collapse</i>)</summary>
  
```json
{
  "id": "606077e8-8f06-4f79-a6a1-12e48f3e33a1",
  "card": {
    "number": "**** **** **** 1234",
    "expiryDate": "12/22",
    "holder": "John Doe"
  },
  "charge": {
    "currency": "EUR",
    "amount": 10.20
  },
  "description": "AMZN PURCHASE",
  "records": [
    {
      "id": "4b5ae15e-ff80-44e4-934d-bd59800c7d53",
      "timestamp": "2022-01-24T20:55:21.0000000+00:00",
      "operation": "Issued",
      "metaData": {
        "transactionId": "020caaa5-fbf1-4923-b2f1-957440de9edd"
      }
    },
    {
      "id": "3062c9f7-6db9-4fbb-a8b2-94c2aab55d9c",
      "timestamp": "2022-01-24T20:55:22.0000000+00:00",
      "operation": "Verifying",
      "metaData": {
        "transactionId": "020caaa5-fbf1-4923-b2f1-957440de9edd"
      }
    },
    {
      "id": "e5a92704-d805-4c92-8c5a-308e040cf963",
      "timestamp": "2022-01-24T20:55:23.0000000+00:00",
      "operation": "Verified",
      "metaData": {
        "transactionId": "020caaa5-fbf1-4923-b2f1-957440de9edd"
      }
    },
    {
      "id": "e84c6ebe-7e15-4160-96ad-70de4d1d74c4",
      "timestamp": "2022-01-24T20:55:24.0000000+00:00",
      "operation": "Authorizing",
      "metaData": {
        "transactionId": "020caaa5-fbf1-4923-b2f1-957440de9edd"
      }
    },
    {
      "id": "9d4accc4-7f2a-4886-8db7-fececedc2a8c",
      "timestamp": "2022-01-24T20:55:25.0000000+00:00",
      "operation": "Authorized",
      "metaData": {
        "transactionId": "020caaa5-fbf1-4923-b2f1-957440de9edd"
      }
    },
    {
      "id": "bcbdb2bc-387e-41b7-879c-427c1bc4cd99",
      "timestamp": "2022-01-24T20:55:26.0000000+00:00",
      "operation": "Processing",
      "metaData": {
        "transactionId": "020caaa5-fbf1-4923-b2f1-957440de9edd"
      }
    },
    {
      "id": "94b98612-1fc7-4974-beb9-dc5c56ec0aed",
      "timestamp": "2022-01-24T20:55:27.0000000+00:00",
      "operation": "Processed",
      "metaData": {
        "transactionId": "020caaa5-fbf1-4923-b2f1-957440de9edd"
      }
    }
  ]
}
```
</details>



## The Payment Gateway API

The payment gateway API application consists in 3 different endpoints. 

Two of those to fulfill the challenge presented:
- `POST /api/v1/payments` - to process a payment.
- `GET /api/v1/payments/{{payment-id}}` - to retrieve the payment details.

And one additional endpoint to check the application health:
- `GET /health`

### `POST /api/v1/payments`

The `body` of the request is the same format as the one presented on [JSON Representations](#json-representations) section for the **ProcessPaymentJsonRequest.json**.

Other possible responses can be:
<details>
  <summary><b>404BadRequest.json</b> (click to <i>expand/collapse</i>)</summary>
  
```http
HTTP/1.1 400 Bad Request
Connection: close
Content-Type: application/problem+json; charset=utf-8
Date: Thu, 27 Jan 2022 17:26:46 GMT
Server: Kestrel
Transfer-Encoding: chunked

{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "traceId": "00-4af2c458ee7cfb27d3897607482e1677-50b436574bd173e0-01",
  "errors": {
    "description": [
      "'description' must not be empty."
    ],
    "card.holder": [
      "'holder' must not be empty."
    ],
    "card.number": [
      "'number' (without whitespaces) must be between 14 and 19 characters. You entered 22 characters.",
      "'number' is not in the correct format."
    ],
    "card.expiryDate": [
      "'expiryDate' provided does conform with the allowed format 'mm/yy' or has expired."
    ],
    "card.securityCode": [
      "'securityCode' must be between 3 and 4 characters. You entered 1 characters.",
      "'securityCode' is not in the correct format."
    ],
    "charge.amount": [
      "'amount' is not within the allowed precision for the given 'currency'."
    ],
    "charge.currency": [
      "'currency' provided is not supported."
    ]
  }
}
```
</details>

<details>
  <summary><b>401Unauthorized.json</b> (click to <i>expand/collapse</i>)</summary>
  
```http
HTTP/1.1 401 Unauthorized
Content-Length: 0
Connection: close
Date: Thu, 27 Jan 2022 17:25:04 GMT
Server: Kestrel
WWW-Authenticate: Bearer error="invalid_token"
```
</details>

### `GET /api/v1/payments/{payment-id}`

The usual result of the request is the same format as the one presented on [JSON Representations](#json-representations) section for the **PaymentJsonResponse.json**.

Other possible responses can be:
<details>
  <summary><b>404NotFound.json</b> (click to <i>expand/collapse</i>)</summary>
  
```http
HTTP/1.1 404 Not Found
Connection: close
Content-Type: application/problem+json; charset=utf-8
Date: Thu, 27 Jan 2022 17:24:45 GMT
Server: Kestrel
Transfer-Encoding: chunked
api-supported-versions: 1.0

{
  "type": "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
  "title": "Payment not found.",
  "status": 404,
  "traceId": "00-54784c50413fb6dbe0278c69ddf18243-5148675ad513f3db-01"
}
```
</details>

<details>
  <summary><b>401Unauthorized.json</b> (click to <i>expand/collapse</i>)</summary>
  
```http
HTTP/1.1 401 Unauthorized
Content-Length: 0
Connection: close
Date: Thu, 27 Jan 2022 17:25:04 GMT
Server: Kestrel
WWW-Authenticate: Bearer error="invalid_token"
```
</details>

### `GET /health`

Example Response:
<details>
  <summary><b>HealthCheck.json</b> (click to <i>expand/collapse</i>)</summary>
  
```json
// 20220127172131
// http://localhost:5000/health

{
  "status": "pass",
  "notes": [
    "TotalDuration: 00:00:00.0593761"
  ],
  "checks": {
    "data-storage": [
      {
        "exception": null,
        "output": "Data storage instance is operational.",
        "status": "pass",
        "duration": "00:00:00.0574901"
      }
    ],
    "acquiring-bank-services": [
      {
        "exception": null,
        "output": "Acquiring bank services are operational.",
        "status": "pass",
        "duration": "00:00:00.0493475"
      }
    ]
  }
}
```
</details>

## Setup

This section describes how can the application be ran in three different ways for commodity purposes.

The ways to use the application are the following:
- Using the `dotnet-cli` through the terminal;
- Using `Visual Studio 2022`:
  - Through the `docker-compose` project;
  - Through the standalone `CKO.PaymentGateway.Host.Api` project.
### Setup with `dotnet-cli`

The lightest form of running the application is through the usage of the `dotnet-cli` command line tool. In order to achieve this please follow the steps below.

#### Build

Build the solution by executing the following commands on your terminal.

```PowerShell
dotnet clean .\CKO.PaymentGateway.Challenge.sln
dotnet restore .\CKO.PaymentGateway.Challenge.sln
dotnet build .\CKO.PaymentGateway.Challenge.sln --no-restore
```

Click on the image to view the **demo**

<p align="center">
<a href="https://asciinema.org/a/464738"><img  src="https://asciinema.org/a/464738.svg"></a>
</p>

#### Run Tests

There are multiple ways that the tests can be ran for the application. Select the flavour you want to use.

##### All Tests

If you want to run all the tests, just execute the following command on your terminal.

```powershell
dotnet test .\CKO.PaymentGateway.Challenge.sln
```
> ⚠️ beware that this won't execute the performance tests.

Click on the image to view the **demo**
<p align="center">
<a href="https://asciinema.org/a/464742"><img  src="https://asciinema.org/a/464742.svg"></a>
</p>

##### Specific Category Tests

In case you want to run just a subset of tests execute the following command for the test category you want to run.

 ```powershell
 dotnet test --no-build --verbosity normal --filter FullyQualifiedName~.UnitTests CKO.PaymentGateway.Challenge.sln
 ```

 The previous example just executed the `UnitTests` as seen by the `--filter` parameter. The categories supported for this method of test execution are:
 - `UnitTests`
 - `IntegrationTests`
 - `AcceptanceTests`
 - `ArchitectureTests`

Click on the image to view the **demo**
<p align="center">
<a href="https://asciinema.org/a/464743"><img  src="https://asciinema.org/a/464743.svg"></a>
</p>

##### Performance Tests

Performance tests are a category on their own since they do not execute with the dotnet test runner but instead are a dotnet application on it's own.

In order to execute them issue the following command on your terminal.

 ```powershell
dotnet run --project ./test/CKO.PaymentGateway.Api.ViewModels.PerformanceTests -c Release
 ```

Click on the image to view the **demo**
<p align="center">
<a href="https://asciinema.org/a/464740"><img  src="https://asciinema.org/a/464740.svg"></a>
</p>

#### Run API

In order to run the API, first, the infrastructure needs to be setup. 

This can be done by executing the following command on your terminal.
 ```powershell
docker-compose --profile infrastructure up --no-build --detach
 ```

 After that you can check if the `database` container and the `acquiring-bank-api` container are running with the following command
 ```powershell
docker ps
 ```

 After you've verified that the containers are now running issue the following command to start the application

 ```powershell
 dotnet run --no-build --project .\src\CKO.PaymentGateway.Host.Api\CKO.PaymentGateway.Host.Api.csproj
 ```

Click on the image to view the **demo**
<p align="center">
<a href="https://asciinema.org/a/464741"><img  src="https://asciinema.org/a/464741.svg"></a>
</p>

#### Environment Clean Up
To clean up the environment please issue the following command after you've stopped the application.
```powershell
docker-compose down --volumes
```

This will ensure that the `docker-compose` stack is removed along with the volumes associated with it.
### Setup with `Visual Studio 2022 Community`
Before running the solution on visual studio make sure that you've destroyed the `docker-compose` stack if you ran the previous setup method otherwise the solution won't work since the ports will already be assigned.
#### Run with `docker-compose` project

To run the solution on visual studio just select the project `docker-compose` as startup project, launch it and wait for the magic to happen 🧙.

This will automatically launch the `/health` endpoint on your browser. After that appears the application is good to go.

You can then inspect the logs under the `Container Tools Window` in visual studio.

See the image **demo**
<p align="center">
<img  src="./assets/vs-compose.gif">
</p>

#### Run with standalone `CKO.PaymentGateway.Host.Api` project

To launch the `CKO.PaymentGateway.Host.Api` as standalone, which is better/faster for development purposes, you can select it has the startup project. 

Before launching it just make sure you do the infrastructure setup described above on [Setup with `dotnet-cli`-> Run API](#run-api).

Also make sure to clean up after you're done by following the [Environment Clean Up](#environment-clean-up) step.

Click on the image to view the **demo**
<p align="center">
<a href="https://asciinema.org/a/464741"><img  src="https://asciinema.org/a/464741.svg"></a>
</p>

## How to use Payment Gateway API


### Make requests

In order to use the API Gateway after it's running you can look at the [requests.http](./requests.http) file for reference.

If you want to actually use those requests make sure to use Visual Studio Code with the [REST Client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) extension.

The file already contains two valid token's to interact with the API in which one user already has data.

If you want to generate your own tokens use the [JWT.io](https://jwt.io/) website with the following payload, change the values accordingly, and use the following secret `your-256-bit-secret` (which is actually the default one the website 😉) 

```json
{   
    "name": "Adidas",
    "merchantId": "ed962527-eb35-44a9-9c09-b75af6c84ac2"
}
```

### Introduce Payment Failures

In order to introduce payment failures you want to change the Acquiring Bank API mocks. 

For that, you can use the [Mockoon](https://mockoon.com/) Client in order to change the [acquiring-bank-mock-api.json](./acquiring-bank-mock-api.json) file.

Just change the response types from the `2xx` range to another range, for ex.: `4xx` range for a given endpoint and then try to issue a payment.

## Relevant Technical Comments/Considerations

This section shows a collection of some thoughts I would like to convey but were not adequate to be included in the other sections.
### Comments on the Tests

#### Unit Tests
#### Integration Tests
#### Acceptance Tests
#### Architecture Tests
#### Performance Tests

### Comments on Observability
#### Logging
#### Tracing
### Comments on Responsibilities

### Comments on Code Style / Architecture

granular packages, microsoft approach

## Areas for Improvement

  - resiliency
  - cloud provider deployments
  - parameter store for configuration
  - database encryption
