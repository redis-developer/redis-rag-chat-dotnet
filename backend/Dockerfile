FROM mcr.microsoft.com/dotnet/sdk:8.0

WORKDIR /app
COPY . /app
RUN dotnet dev-certs https --trust
RUN dotnet restore
EXPOSE 8000
CMD [ "dotnet", "run" ]