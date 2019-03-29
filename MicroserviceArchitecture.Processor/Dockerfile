#FROM microsoft/dotnet:2.1-runtime AS base
#WORKDIR /app

#FROM microsoft/dotnet:2.1-sdk AS build
#WORKDIR /src
#COPY MicroserviceArchitecture.Processor/MicroserviceArchitecture.Processor.csproj MicroserviceArchitecture.Processor/
#RUN dotnet restore MicroserviceArchitecture.Processor/MicroserviceArchitecture.Processor.csproj
#COPY . .
#WORKDIR /src/MicroserviceArchitecture.Processor
#RUN dotnet build MicroserviceArchitecture.Processor.csproj -c Release -o /app

#FROM build AS publish
#RUN dotnet publish MicroserviceArchitecture.Processor.csproj -c Release -o /app

#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app .
#ENTRYPOINT ["dotnet", "MicroserviceArchitecture.Processor.dll"]

FROM microsoft/aspnetcore:2.0.5
ARG source
WORKDIR /app
EXPOSE 80
COPY ${source:-obj/Docker/publish} .
ENTRYPOINT ["dotnet", "MicroserviceArchitecture.Processor.dll"]