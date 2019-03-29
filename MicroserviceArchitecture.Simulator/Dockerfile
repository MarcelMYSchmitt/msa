#FROM microsoft/dotnet:2.1-runtime AS base
#WORKDIR /app

#FROM microsoft/dotnet:2.1-sdk AS build
#WORKDIR /src
#COPY MicroserviceArchitecture.Simulator/MicroserviceArchitecture.Simulator.csproj MicroserviceArchitecture.Simulator/
#RUN dotnet restore MicroserviceArchitecture.Simulator/MicroserviceArchitecture.Simulator.csproj
#COPY . .
#WORKDIR /src/MicroserviceArchitecture.Simulator
#RUN dotnet build MicroserviceArchitecture.Simulator.csproj -c Release -o /app

#FROM build AS publish
#RUN dotnet publish MicroserviceArchitecture.Simulator.csproj -c Release -o /app

#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app .
#ENTRYPOINT ["dotnet", "MicroserviceArchitecture.Simulator.dll"]

FROM microsoft/aspnetcore:2.0.5
ARG source
WORKDIR /app
EXPOSE 80
COPY ${source:-obj/Docker/publish} .
ENTRYPOINT ["dotnet", "MicroserviceArchitecture.Simulator.dll"]