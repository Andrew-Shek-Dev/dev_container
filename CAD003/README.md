# Database 
Oracle 19c Image will be used.

- Step 1 : Dockerized Oracle 19c from official Oracle 19c version
Please refer below reference:
https://github.com/steveswinsburg/oracle19c-docker

- Step 2 : Create Own Oracle 19c Image (optional)
```Dockerfile
# Dockerfile
FROM oracle/database:19.3.0-ee
```

```bash
docker build -t <username>/oracle19c_m2:full --pull=false  . 
docker push <username>/oracle19c_m2:full
```
Reference : https://stackoverflow.com/questions/20481225/how-can-i-use-a-local-image-as-the-base-image-with-a-dockerfile

Try run
```bash
docker run --name test_db -p 1521:1521 -p 5500:5500 -e ORACLE_PDB=cad003 -e ORACLE_PWD=In4IimWl -e INIT_SGA_SIZE=3000 -e INIT_PGA_SIZE=1000 -v /opt/oracle/oradata -d <username>/oracle19c_m2:full
```

## Data Migration with C# by .NET Core EF
* https://www.google.com/search?q=c%23+multi+tenant+system+oracle&sca_esv=586199351&ei=7-FmZcWID9nh2roPg9qD6AE&oq=C%23+multTenant+System+Oracle&gs_lp=Egxnd3Mtd2l6LXNlcnAiG0MjIG11bHRUZW5hbnQgU3lzdGVtIE9yYWNsZSoCCAAyChAhGKABGMMEGAoyChAhGKABGMMEGAoyChAhGKABGMMEGApIlSlQ1hZYvx1wAXgBkAEAmAFsoAHwAqoBAzIuMrgBA8gBAPgBAcICChAAGEcY1gQYsAPCAggQIRigARjDBOIDBBgAIEGIBgGQBgI&sclient=gws-wiz-serp#ip=1
* https://stackoverflow.com/questions/19458943/multi-tenant-with-code-first-ef6
