FROM mcr.microsoft.com/devcontainers/dotnet:7.0

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
# COPY ./mssql/installSQLtools.sh installSQLtools.sh
# RUN bash ./installSQLtools.sh \
#      && apt-get clean -y && rm -rf /var/lib/apt/lists/* /tmp/library-scripts