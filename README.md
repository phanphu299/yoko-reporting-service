# Local Development
## Log into Azure Container Registry
az login -t d9f3dee8-148c-49ea-8e87-dd97cd0cd5de
az account set -s 7a9a0f8c-eb0d-4803-89f5-4e9e32a6333d
az acr login -n dxpprivate

## Use Build Cake to run docker container
```powershell
$env:CAKE_SETTINGS_SKIPPACKAGEVERSIONCHECK="true"
.\build.ps1 --target=Compose
.\build.ps1 --target=Up
.\build.ps1 --target=Down
```

## Run Unit Test
newman run -k -e ./tests/IntegrationTest/AppData/Docker.postman_environment.json ./tests/IntegrationTest/AppData/Test.postman_collection.json

# RabbitMQ
## Windows
$trackingEndpoint = 'https://ahs-test01-ppm-be-sea-wa.azurewebsites.net/fnc/mst/messaging/rabbitmq?code=xKvUgzJgdbwRcBRvPsee3gPbmBMXTpR8pkWhTWQky6RfvW5cxe5kqn94C9D'
Invoke-WebRequest $trackingEndpoint -UseBasicParsing | Set-Content './rabbitmq/rabbitmq-definitions.json'

## Linux
trackingEndpoint='https://ahs-test01-ppm-be-sea-wa.azurewebsites.net/fnc/mst/messaging/rabbitmq?code=xKvUgzJgdbwRcBRvPsee3gPbmBMXTpR8pkWhTWQky6RfvW5cxe5kqn94C9D'
curl -o './rabbitmq/rabbitmq-definitions.json' $trackingEndpoint