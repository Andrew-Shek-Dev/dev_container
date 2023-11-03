# C# Application Development
## Lesson 1 - Setup
### DEV Container
<Content>

#### Setup the development environment using dev container manually
* Step 1 : Create following project folder structure
```
project folder
|- dev-env
|---Dockerfile
|
|- sqlserver
|-- sqlserver.env
|
|- docker-compose.yml
```

* Step 2 : Create the `Dockerfile` under `dev-env` folder(container)
```yml
# [Choice] .NET version: 7.0, 6.0, 5.0, 3.1, 6.0-bullseye, 5.0-bullseye, 3.1-bullseye, 6.0-focal, 5.0-focal, 3.1-focal
ARG VARIANT="7.0"
ARG NODE_VERSION="none"

FROM mcr.microsoft.com/vscode/devcontainers/dotnet:0-${VARIANT}
# [Optional] Uncomment this section to install additional OS packages.
RUN apt-get update && \
    export DEBIAN_FRONTEND=noninteractive && \
    apt-get -qy full-upgrade && \
    apt-get install -qy curl && \
    apt-get -y install --no-install-recommends vim && \
    curl -sSL https://get.docker.com/ | sh

RUN dotnet tool install -g dotnet-ef
ENV PATH $PATH:/root/.dotnet/tools

# configure for https
RUN dotnet dev-certs https
```

* Step 3 : Create Docker Compose File
```yml
version: '3.4'
services:
  sql_server:
   container_name: sqlserver
   image: mcr.microsoft.com/mssql/server:2022-latest
   ports:
    - "9876:1433" #MS SQL Server use 1433 port
   volumes:
     - ./habit-db-volume:/var/lib/mssqlql/data #host directory:container directory
   env_file:
     - sqlserver/sqlserver.env
  dev-env:
    container_name: dev-env
    build:
      context: ./dev-env
    volumes:
      - ..:/workspace  #host directory:container directory, Dev Container has set workspace directory is "workspace"
    # Ref : https://www.baeldung.com/ops/docker-compose-interactive-shell
    stdin_open: true # docker run -i
    tty: true # docker run -t
# volumes: comment because the physical folder (network drive) is used here. It can be maintained easily (even expand volumes and backup)
```
NOTE: Macbook M1/2/3 user MUST enable Rosetta at Docker Desktop. Please refer the [link](https://stackoverflow.com/questions/66662820/m1-docker-preview-and-keycloak-images-platform-linux-amd64-does-not-match-th)

* Step 4 : Create `devcontainer.json`
```json
{
    "name":"CAD001 Example",
    "dockerComposeFile":["../docker-compose.yaml"],
    "service":"dev-env",
    "workspaceFolder": "/workspace/${workspaceFolderBasename}",
    "customizations": {
        "vscode": {
            "extensions": [
                "ms-dotnettools.csharp",
                "shardulm94.trailing-spaces",
                "mikestead.dotenv",
                "fernandoescolar.vscode-solution-explorer",
                "jmrog.vscode-nuget-package-manager",
                "patcx.vscode-nuget-gallery",
                "pkief.material-icon-theme",
                "ms-mssql.mssql",
                "humao.rest-client",
                "rangav.vscode-thunder-client",
                "formulahendry.dotnet-test-explorer",
                "kevin-chatham.aspnetcorerazor-html-css-class-completion",
                "syncfusioninc.blazor-vscode-extensions",
                "ms-dotnettools.vscode-dotnet-runtime",
                "ms-dotnettools.blazorwasm-companion"
                ]
          }
    },
    "remoteUser": "root"
}
```
Reference : https://code.visualstudio.com/docs/editor/variables-reference

* Step 5 : Testing Container `dev-env`
```bash
docker compose up -d
docker exec -it dev-env /bin/bash
docker exec -it sqlserver /bin/bash
```

* Step 6 : Testing Container `sqlserver`
```bash
docker exec -it sqlserver /bin/bash
```

* Step 7 : Connect MS SQL Server Console in `sqlserver` container
```bash
/opt/mssql-tools/bin/sqlcmd -S localhost -U SA
```

* Step 8 : Check MS SQL Server Version under Console
```shell
1> SELECT @@VERSION
2> GO
```

The good news is that VSCode extension can streamline above all of steps!
 
#### Setup the development environment using dev container in VSCode

## Reference
https://github.com/PacktPublishing/Building-Modern-SaaS-Applications-with-C-and-.NET
