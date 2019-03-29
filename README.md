# Introduction 
This project contains all our core services for our microservice architecture: processor, redis, backendforfrontend.
For starting processor, redis and backendforfrontend service create a new .env file for all environment variables we are going to use.

# Getting Started

## Running the code

As a developer, you will want to develop and try things out on your own isolated "environment". One part of such environment is your machine,
and the containers that you can deploy on your machine directly using commands such as "docker run" or "docker-compose up". Second part
of your environment will be in Azure, which you have to create for yourself.

To run the application from start to the end you will need:

1. Run Create-Infrastructure.ps1 from the Infrastructure project. As a parameter give an EnvironmentTag of your choice 
(it has to be unique for your environment not to colide with other resources in the subscription!)

2. Create an .env file in the root of the project, and provide the following parameters:

```
EnvironmentTag=<<your environment tag which you gave into Create-Infrastructure.ps1>>
AzureRegionTag=<<your azure tag which you gave into Create-Infrastructure.ps1>>
EventHubSendConnectionString=please_replace
EventHubListenConnectionString=please_replace
EventHubEndpoint=please_replace
EventHubPath=please_replace
StorageAccessKey=please_replace
StorageAccountName=please_replace
LogEventLevel=Debug
```

(make sure to save the file as utf-8 otherwise values might not be parsed by docker-compose!)

3. For running the application, Visual Studio is mandatory at the moment. 
Select the docker-compose project as "Startup project" (right click in visual studio), either in debug mode or not. 

4. Create Network
You need to call the following command: docker network create local_development_network

Extra: if you want to start the project with 'docker-compose up' you need to first build the application.
To build, you have two options:
1. From VS - select "Release" as Visual Studio build configuration & rebuild
2. from the root, do docker-compose -f ... up with files like docker-compose-msa-build.yml

## Running the tests

There is an additional configuration file which tests need to run. 
Create an xunit.runner.json file in MicroserviceArchitecture.Tests folder, and set the contents similar to .env above
{
	"EnvironmentTag": "<< your environment tag which you gave into Create-Infrastructure.ps1>>",
	"AzureRegionTag": "<< if you don't know, set it to 'ne'>>",
	"Test:BackendForFrontendBaseUrl": "http://localhost:8080",
}

As an alternative you can execute the tests via `docker-compose -f docker-compose-tests.yml up` once 
* you have both processor and bff running locally
* you extended your `.env` file with
```
BackendForFrontendBaseUrl=http://backendforfrontend
```
* you added the local network to your `docker-compose-tests.yml` file (but don't commit it)
```
networks:
  default:
    external:
      name: local_development_network
````

## Deploying to k8s cluster

Simply apply the yml files from "deployment" folders which are located in projects of the services. Make sure your cluster has the
required prerequisite configuration resources created.
