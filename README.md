# C# Application Development
## Lesson 1 - Setup
### DEV Container
<Content>

#### Setup the development environment using dev container manually (Reference ONLY)
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





## Lesson 3 - Database Setup
### TBC
### Oracle Database Operations
#### Step 1 : Create User
```bash
sqlplus /nolog
```

```sql
connect sys/password@cad003 as sysdba
create user scott identified by scott DEFAULT TABLESPACE USERS;
grant connect,resource,create view to scott;
GRANT UNLIMITED TABLESPACE TO scott;
```

### Step 2 : Create Tables
```sql
create table scott.dept(deptno number(2) visible not null, dname varchar2(14 byte) visible, loc varchar2(13 byte) visible);
insert into dept values ('50','TEST','WASHINGTON');
alter table dept add constraint "pk_dept" primary key ("DEPTNO");

create table emp(empno number(4) visible not null, ename varchar2(10 byte) visible, job varchar2(9 byte) visible, mgr number(4) visible, hiredate date visible, sql number(7,2) visible, comm number(7,2) visible, deptno number(2) visible);
insert into emp values('7935','SSSS','SALESMAN','7792',TO_DATE('2022-01-18 17:06:37','SYYYY-MM-DD HH24:MI:SS'),'4100',NULL,NULL);
alter table emp add constraint "pk_emp" primary key ("EMPNO");
alter table emp add constraint "fk_deptno" foreign key ("DEPTNO") references dept("DEPTNO") NOT deferrable initially immediate norely validate;

create table bonus(ename varchar2(10 byte) visible, job varchar2(9 byte) visible, sal number visible, comm number visible);
create table salgrade(grade number visible,losal number visible, hisal number visible);
insert into salgrade values('1','700','1200');
```

### Step 3 : Data Types
- +, -, *, /
- NUMBER(+-*/),VARCHAR2,DATE(+-)
### Step 4 : Basic Operations
```sql
SQL> select sysdate from dual;

SYSDATE
---------
21-NOV-23
```

```sql
SQL> select sysdate + 1 from dual;    

SYSDATE+1
---------
22-NOV-23
```

```sql
SQL> select (sysdate-hiredate)/365 from emp;    

(SYSDATE-HIREDATE)/365
----------------------
            1.84010635
```

```sql
SQL> select trunc(months_between(sysdate,hiredate)/12) from emp; 

TRUNC(MONTHS_BETWEEN(SYSDATE,HIREDATE)/12)
------------------------------------------
                                         1
```

```sql
SQL> select trunc(months_between(sysdate,hiredate)/12) as period from emp; 

    PERIOD
----------
         1

SQL> select trunc(months_between(sysdate,hiredate)/12) period from emp;

    PERIOD
----------
         1

SQL> select e.empno,e.ename from emp e;      

     EMPNO ENAME
---------- ----------
      7935 SSSS


SQL> select e.ename || ' (Staff# is '||e.empno||')' staff from emp e;  

STAFF
---------------------------------------------------------------
SSSS (Staff# is 7935)

```

Offset and Limit
```sql
SELECT * FROM emp OFFSET 0 ROWS FETCH NEXT 3 ROWS ONLY;
```

Basic SQL Syntax
```sql
SQL> select * from emp where comm is null;

     EMPNO ENAME      JOB              MGR HIREDATE         SQL       COMM
---------- ---------- --------- ---------- --------- ---------- ----------
    DEPTNO
----------
      7935 SSSS       SALESMAN        7792 18-JAN-22       4100

SQL> select * from emp where comm is not null;        

no rows selected

SQL> select * from emp where upper(ename) = upper('ssss');

     EMPNO ENAME      JOB              MGR HIREDATE         SQL       COMM
---------- ---------- --------- ---------- --------- ---------- ----------
    DEPTNO
----------
      7935 SSSS       SALESMAN        7792 18-JAN-22       4100


SQL> select * from emp where extract(year from hiredate) = '1981';

no rows selected

SQL> select * from emp where extract(year from hiredate) = '2022';

     EMPNO ENAME      JOB              MGR HIREDATE         SQL       COMM
---------- ---------- --------- ---------- --------- ---------- ----------
    DEPTNO
----------
      7935 SSSS       SALESMAN        7792 18-JAN-22       4100

SQL> select * from emp where job like('_A%');      

     EMPNO ENAME      JOB              MGR HIREDATE         SQL       COMM
---------- ---------- --------- ---------- --------- ---------- ----------
    DEPTNO
----------
      7935 SSSS       SALESMAN        7792 18-JAN-22       4100



SQL> select * from emp where job like('%_E%'); 

     EMPNO ENAME      JOB              MGR HIREDATE         SQL       COMM
---------- ---------- --------- ---------- --------- ---------- ----------
    DEPTNO
----------
      7935 SSSS       SALESMAN        7792 18-JAN-22       4100

SQL> select * from emp where job like('_A%');      

     EMPNO ENAME      JOB              MGR HIREDATE         SQL       COMM
---------- ---------- --------- ---------- --------- ---------- ----------
    DEPTNO
----------
      7935 SSSS       SALESMAN        7792 18-JAN-22       4100



SQL> select * from emp where job like('%_E%'); 

     EMPNO ENAME      JOB              MGR HIREDATE         SQL       COMM
---------- ---------- --------- ---------- --------- ---------- ----------
    DEPTNO
----------
      7935 SSSS       SALESMAN        7792 18-JAN-22       4100

SQL> select * from emp where job like('_A%');      

     EMPNO ENAME      JOB              MGR HIREDATE         SQL       COMM
---------- ---------- --------- ---------- --------- ---------- ----------
    DEPTNO
----------
      7935 SSSS       SALESMAN        7792 18-JAN-22       4100



SQL> select * from emp where job like('%_E%'); 

     EMPNO ENAME      JOB              MGR HIREDATE         SQL       COMM
---------- ---------- --------- ---------- --------- ---------- ----------
    DEPTNO
----------
      7935 SSSS       SALESMAN        7792 18-JAN-22       4100

```
_  any ONE character

```sql
SQL> select * from emp where job like '%\%%' escape '\';

no rows selected
```

AND,OR, NOT are similar to other sql server.

SubQuery

Transraction
```sql
SQL> select * from dept;

    DEPTNO DNAME          LOC
---------- -------------- -------------
        50 TEST           WASHINGTON

SQL> delete from dept where deptno=60; 

0 rows deleted.

SQL> delete from dept where deptno=50;

1 row deleted.

SQL> select * from dept;

no rows selected

SQL> rollback;

Rollback complete.

SQL> select * from dept;

    DEPTNO DNAME          LOC
---------- -------------- -------------
        50 TEST           WASHINGTON

```

```sql
SQL> select * from dept;

    DEPTNO DNAME          LOC
---------- -------------- -------------
        50 TEST           WASHINGTON

SQL> delete from dept where deptno=50;

1 row deleted.

SQL> select * from dept;

no rows selected

SQL> insert into dept(deptno,dname,loc) values(63,'BOSS','DONGGUAN');

1 row created.

SQL> select * from dept;

    DEPTNO DNAME          LOC
---------- -------------- -------------
        63 BOSS           DONGGUAN

SQL> commit; 

Commit complete.

SQL> select * from dept;

    DEPTNO DNAME          LOC
---------- -------------- -------------
        63 BOSS           DONGGUAN

SQL> rollback;

Rollback complete.

SQL> select * from dept;

    DEPTNO DNAME          LOC
---------- -------------- -------------
        63 BOSS           DONGGUAN
```

```sql
SQL> delete from dept where deptno = 50; 

1 row deleted.

SQL> savepoint A;

Savepoint created.

SQL> update dept set loc='BEIJIN' where deptno=60;

0 rows updated.

SQL> insert into dept(deptno,dname,loc) values(63,'BOSS','DONGGUAN');

1 row created.

SQL> savepoint B; 

Savepoint created.

SQL> delete from dept where deptno = 63;

1 row deleted.

SQL> select * from dept;

no rows selected

SQL> rollback to savepoint B;

Rollback complete.

SQL> select * from dept; 

    DEPTNO DNAME          LOC
---------- -------------- -------------
        63 BOSS           DONGGUAN

SQL> rollback to savepoint A;

Rollback complete.

SQL> select * from dept;

no rows selected

SQL> rollback;

Rollback complete.

SQL> select * from dept;

    DEPTNO DNAME          LOC
---------- -------------- -------------
        50 TEST           WASHINGTON

```

## Reference
* https://github.com/PacktPublishing/Building-Modern-SaaS-Applications-with-C-and-.NET
* https://blog.csdn.net/langfeiyes/article/details/123597992
