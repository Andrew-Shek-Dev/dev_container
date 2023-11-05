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
FROM mcr.microsoft.com/devcontainers/dotnet:1-7.0-bullseye

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

# Install SQL Tools: SQLPackage and sqlcmd
COPY ./mssql/installSQLtools.sh installSQLtools.sh
RUN bash ./installSQLtools.sh \
     && apt-get clean -y && rm -rf /var/lib/apt/lists/* /tmp/library-scripts
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

#### Setup the development environment using dev container in VSCode
* Step 1 : Install VSCode Extension `Dev Containers`
* Step 2 : Cmd + Shift + P and type `>Dev Containers: Add Dev Container Configuration Files`
* Step 3 : Select `From a predefined container configuration definition`
* Step 4 : Select `Show All Definitions`
* Step 5 : Type "C#" and Select `C# (.NET) and MS SQL`
* Step 6 : Select 7.0 and Click OK
* Step 7 : To adopt my development environment setup, please modify the devcontainer.json as following:
```json
{
	"name": "CAD001 Demo", //<- update
	"dockerComposeFile": "../docker-compose.yml",//<- update
	"service": "dev-env", //<- update
	"workspaceFolder": "/workspaces/${localWorkspaceFolderBasename}",

	"customizations": {
		"vscode": {
			"extensions": [
				"ms-dotnettools.csharp",
				"ms-mssql.mssql",
				 //Add Other packages
         //<- update below
				  "shardulm94.trailing-spaces",
				 "mikestead.dotenv",
				 "fernandoescolar.vscode-solution-explorer",
				 "aliasadidev.nugetpackagemanagergui",
				 "pkief.material-icon-theme",
				 "humao.rest-client",
				 "rangav.vscode-thunder-client",
				 "formulahendry.dotnet-test-explorer",
				 "kevin-chatham.aspnetcorerazor-html-css-class-completion",
				 "syncfusioninc.blazor-vscode-extensions",
				 "ms-dotnettools.vscode-dotnet-runtime",
				 "ms-dotnettools.blazorwasm-companion",
				 "streetsidesoftware.code-spell-checker"
			]
		}
	},
	// Uncomment to connect as root instead. More info: https://aka.ms/dev-containers-non-root.
	"remoteUser": "root"
}
```
* Step 8  : Delete `mssql` folder and `Dockerfile` under `.devcontainer`
* Step 9  : Move `docker-compose.yml` to Root Folder
* Step 10 : Update `docker-compose.yml` as following:
```yml
version: '3.4' # 3 => 3.4
services:
  sql_server: # db => sql_server
    image: mcr.microsoft.com/mssql/server:2022-latest #2019=>2022
    restart: unless-stopped
    volumes:
      - ./habit-db-volume:/var/lib/mssqlql/data # add
    environment:
      SA_PASSWORD: P@ssw0rd
      ACCEPT_EULA: Y

  dev-env: #app => dev-env
    build:
      context: ./dev-env # . => ./dev-env
      dockerfile: Dockerfile
    volumes:
      - ..:/workspaces #../..:/workspaces:cached => ..:/workspaces
    command: sleep infinity
    network_mode: service:sql_server # db => sql_server
```


## Foundation of C#
### Hello World Program
TBC

### Database Migration
Step 1: Create .NET library
```bash
dotnet new classlib --name RoomBookingSystem.Database
```

Step 2: Add all essential NuGet Packages of new .NET library
* Open your project workspace in VSCode
* Open the Command Palette (Ctrl+Shift+P)
* Select > NuGet Package Manager GUI
* Click Install New Package
* Search and add following package:
  - Microsoft.EntityFrameworkCore
  - Microsoft.EntityFrameworkCore.Design
  - Microsoft.EntityFrameworkCore.Analyzers
  - Microsoft.EntityFrameworkCore.Relational
  - Microsoft.EntityFrameworkCore.SqlServer
  - Microsoft.EntityFrameworkCore.Tools

Step 3: Create Database Migration File
  - Create `RoomBookingSystemDbContext.cs` and `SeedData.cs`
  - Remove `Class1.cs`

Step 4: Create Entity File (ORM File)
  - Create `Entities` folder
  - Create `Room.cs`

Step 5: Design `Room.cs`
```C#
namespace RoomBookingSystem.Database.Entities;

public class Room{
    public int Id {get;set;}
    //default = use default value
    //! =  The unary postfix ! operator is the null-forgiving, or null-suppression, operator. In an enabled nullable annotation context, you use the null-forgiving operator to suppress all nullable warnings for the preceding expression. 
    public String Name {get;set;} = default!;
    public String Description {get;set;} = default!;
}
```

Step 6: Design `SeedData.cs`
```c#
namespace RoomBookingSystem.Database.Entities; //Java = package..

using Microsoft.EntityFrameworkCore; //Java = import..

public static class SeedData{
    public static void Seed(ModelBuilder modelBuilder){
        
    }
}
```

```c#
namespace RoomBookingSystem.Database.Entities; //Java = package..

using Microsoft.EntityFrameworkCore; //Java = import..

public static class SeedData{
  //Similar to knex seed file
    public static void Seed(ModelBuilder modelBuilder){
        modelBuilder.Entity<Room>().HasData(
            new Room {Id=1,Name="Room#1",Description="1/F"},
            new Room {Id=2,Name="Room#2",Description="2/F"},
            new Room {Id=3,Name="Room#3",Description="3/F"}
        );
    }
}
```

Step 7: Design the migration file
```C#
namespace RoomBookingSystem.Database;

using Microsoft.EntityFrameworkCore;
using RoomBookingSystem.Database.Entities;

public class RoomBookingSystemDbContext : DbContext{
    public DbSet<Room>? Rooms {get;set;}
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)=>
        optionsBuilder.UseSqlServer("Server=sql_server;Database=CAD001_Demo;User Id=sa;Password=P@ssw0rd;Integrated Security=false;TrustServerCertificate=true;");
    protected override void OnModelCreating(ModelBuilder modelBuilder)=> SeedData.Seed(modelBuilder);
}
```

Step 8: Database Migration
Please following commands:
```bash
dotnet-ef migrations add InitialSetup
dotnet-ef database update
```

if something wrong in migration file, run following commands for rollback:
```bash
dotnet-ef database drop
dotnet-ef migrations remove
```
Next, run the migration commands again.

Step 9: Open the database using VSCode Extension, SQL Server (mssql).

## Reference
https://github.com/PacktPublishing/Building-Modern-SaaS-Applications-with-C-and-.NET
