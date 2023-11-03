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

* Step 4 : Create `devcontainer.json` under `.devcontainer` folder
```json
{
	"name": "CAD001 Example",
	"dockerComposeFile": "../docker-compose.yaml",
	"service": "dev-env",
	"workspaceFolder": "/workspace/${localWorkspaceFolderBasename}",
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
	//"remoteUser": "root"
}

```
Reference : https://code.visualstudio.com/docs/editor/variables-reference

* Step 6 : Create `launch.json`,`tasks.json` and `settings.json` under `.vscode` folder:
```json
//settings.json
{
    "thunder-client.saveToWorkspace": true,
    "thunder-client.workspaceRelativePath": ".thunder-client"
}
```

* Step 5 : Testing Container `dev-env`
```bash
docker compose up -d
docker exec -it dev-env /bin/bash
cat /etc/os-release
dotnet --version
dotnet-ef --version

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
* Step 1 : Install VSCode Extension `Dev Containers`
* Step 2 : Cmd + Shift + P and type `>Dev Containers: Add Dev Container Configuration Files`
* Step 3 : Select `From a predefined container configuration definition`
* Step 4 : Select `Show All Definitions`
* Step 5 : Type "C#" and Select `C# (.NET) and MS SQL`
* Step 6 : Select 7.0 and Click OK
* Step 7 : To adopt my development environment setup, please modify the devcontainer.json as following:
```json
{
	"name": "CAD001 Example", //Change own name
	"dockerComposeFile": "../docker-compose.yml",
	"service": "dev-env", //Change own name
	"workspaceFolder": "/workspaces/${localWorkspaceFolderBasename}",
	"customizations": {
		"vscode": {	
			"extensions": [
				"ms-dotnettools.csharp",
				"ms-mssql.mssql"
        //Add Other packages
        "shardulm94.trailing-spaces",
        "mikestead.dotenv",
        "fernandoescolar.vscode-solution-explorer",
        "jmrog.vscode-nuget-package-manager",
        "patcx.vscode-nuget-gallery",
        "pkief.material-icon-theme",
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
	"postCreateCommand": "bash .devcontainer/mssql/postCreateCommand.sh 'P@ssw0rd' './bin/Debug/' './.devcontainer/mssql/'"
}
```
* Step 8 : Move `docker-compose.yml` to Root Folder
* Step 9 : Update `docker-compose.yml` as following:
```yml
version: '3.4' # 3->3.4

services:
  dev-env: #app -> dev-env
    build: 
      context: ./dev-env #. -> ./dev-env
      dockerfile: Dockerfile
    volumes:
      - ..:/workspaces #../..:/workspaces:cached -> ..:/workspaces
    command: sleep infinity
    #network_mode: service:sql_server #db -> sql_server

  sql_server: #db -> sql_server
    image: mcr.microsoft.com/mssql/server:2022-latest #2019 -> 2022
    restart: unless-stopped
    volumes:
      - ./habit-db-volume:/var/lib/mssqlql/data #add this mapping
    environment:
      SA_PASSWORD: P@ssw0rd
      ACCEPT_EULA: Y
```
 * Step 10 : Update `postCreateCommand.sh` under `mssql` folder: replace all `localhost` to `sql_server`

#### Setup the development environment using dev container in VSCode

## Reference
https://github.com/PacktPublishing/Building-Modern-SaaS-Applications-with-C-and-.NET
