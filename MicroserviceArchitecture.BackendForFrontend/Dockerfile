#FROM microsoft/dotnet:2.1-runtime AS base
#WORKDIR /app

#FROM microsoft/dotnet:2.1-sdk AS build
#WORKDIR /src
#COPY MicroserviceArchitecture.BackendForFrontend/MicroserviceArchitecture.BackendForFrontend.csproj MicroserviceArchitecture.BackendForFrontend/
#RUN dotnet restore MicroserviceArchitecture.BackendForFrontend/MicroserviceArchitecture.BackendForFrontend.csproj
#COPY . .
#WORKDIR /src/MicroserviceArchitecture.BackendForFrontend
#RUN dotnet build MicroserviceArchitecture.BackendForFrontend.csproj -c Release -o /app

#FROM build AS publish
#RUN dotnet publish MicroserviceArchitecture.BackendForFrontend.csproj -c Release -o /app

#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app .
#ENTRYPOINT ["dotnet", "MicroserviceArchitecture.BackendForFrontend.dll"]

FROM microsoft/aspnetcore:2.0.5
ARG source
WORKDIR /app
EXPOSE 80
COPY ${source:-obj/Docker/publish} .
ENTRYPOINT ["dotnet", "MicroserviceArchitecture.BackendForFrontend.dll"]