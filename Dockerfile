# https://mcr.microsoft.com/v2/dotnet/sdk/tags/list
FROM mcr.microsoft.com/dotnet/sdk:6.0.402-alpine3.16@sha256:cfaf6935ad6ec66ae0be7af332523d21cc810d74120b21d95376ae9581090a09 AS installer-env

COPY . /src/dotnet-function-app

RUN cd /src/dotnet-function-app && \
    mkdir -p /home/site/wwwroot && \
    dotnet publish *.csproj --output /home/site/wwwroot

# https://mcr.microsoft.com/v2/azure-functions/dotnet/tags/list
FROM mcr.microsoft.com/azure-functions/dotnet:4.11.3-slim@sha256:a3f3888209af3c9f21e87527aee35d1b938a72db4f47d0e2defa1e791f58ffb4

RUN rm -rf /FuncExtensionBundles

RUN apt-get update && apt-get install -y curl

ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true

COPY --from=installer-env ["/home/site/wwwroot", "/home/site/wwwroot"]

RUN useradd pagopa-user && \
    mkdir -p /home/pagopa-user && \
    chown -R pagopa-user:pagopa-user /home/pagopa-user

USER pagopa-user

RUN whoami
