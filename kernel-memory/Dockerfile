FROM mcr.microsoft.com/dotnet/sdk:8.0

WORKDIR /app

RUN git clone https://github.com/slorello89/kernel-memory
WORKDIR /app/kernel-memory
RUN git checkout add-service

COPY appsettings.json /app/kernel-memory/service/Service
COPY entrypoint.sh /app/kernel-memory/service/Service

WORKDIR /app/kernel-memory/service/Service
RUN dotnet dev-certs https --trust
RUN dotnet restore

CMD [ "sh", "entrypoint.sh" ]