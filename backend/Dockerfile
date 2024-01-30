FROM mcr.microsoft.com/dotnet/sdk:8.0

WORKDIR /app
COPY ./backend /app
RUN dotnet dev-certs https --trust
RUN dotnet restore
CMD [ "dotnet", "run" ]