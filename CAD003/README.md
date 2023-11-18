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
docker build -t <username>/oracle19c_m2 --pull=false  . 
docker push <username>/oracle19c_m2
```
Reference : https://stackoverflow.com/questions/20481225/how-can-i-use-a-local-image-as-the-base-image-with-a-dockerfile

Try run
```bash
docker run --name test_db -p 1521:1521 -p 5500:5500 -e ORACLE_PDB=cad003 -e ORACLE_PWD=123456 -e INIT_SGA_SIZE=3000 -e INIT_PGA_SIZE=1000 -v /opt/oracle/oradata -d <username>/oracle19c_m2
```