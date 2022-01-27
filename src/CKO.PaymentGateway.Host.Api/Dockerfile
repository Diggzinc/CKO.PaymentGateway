#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/CKO.PaymentGateway.Host.Api/CKO.PaymentGateway.Host.Api.csproj", "src/CKO.PaymentGateway.Host.Api/"]
COPY ["src/CKO.PaymentGateway.Services/CKO.PaymentGateway.Services.csproj", "src/CKO.PaymentGateway.Services/"]
COPY ["src/CKO.PaymentGateway.Services.Abstractions/CKO.PaymentGateway.Services.Abstractions.csproj", "src/CKO.PaymentGateway.Services.Abstractions/"]
COPY ["src/CKO.PaymentGateway.Models/CKO.PaymentGateway.Models.csproj", "src/CKO.PaymentGateway.Models/"]
COPY ["src/AcquiringBank.Api.Client/AcquiringBank.Api.Client.csproj", "src/AcquiringBank.Api.Client/"]
COPY ["src/CKO.PaymentGateway.Api.ViewModels/CKO.PaymentGateway.Api.ViewModels.csproj", "src/CKO.PaymentGateway.Api.ViewModels/"]
RUN dotnet restore "src/CKO.PaymentGateway.Host.Api/CKO.PaymentGateway.Host.Api.csproj"
COPY . .
WORKDIR "/src/src/CKO.PaymentGateway.Host.Api"
RUN dotnet build "CKO.PaymentGateway.Host.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CKO.PaymentGateway.Host.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CKO.PaymentGateway.Host.Api.dll"]
