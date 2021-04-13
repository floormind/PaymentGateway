FROM mcr.microsoft.com/dotnet/sdk:5.0 as build
LABEL maintainer="Ife Ayelabola"
WORKDIR /src
COPY PaymentGateway.Service/*.csproj /src/PaymentGateway.Service/
COPY PaymentGateway.Repository/*.csproj /src/PaymentGateway.Repository/
RUN dotnet restore /src/PaymentGateway.Service/
COPY PaymentGateway.Service/. /src/PaymentGateway.Service/
COPY PaymentGateway.Repository/. /src/PaymentGateway.Repository/

RUN dotnet publish PaymentGateway.Service/PaymentGateway.Service.csproj -c Release -o out
RUN mkdir ./out/data

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /src
COPY --from=build /src/out .
EXPOSE 5000
ENV ASPNETCORE_URLS http://*:5000
ENTRYPOINT ["dotnet", "PaymentGateway.Service.dll"]

