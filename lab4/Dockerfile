FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /app

COPY . ./

RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

WORKDIR /app

COPY --from=build /app/out .
COPY --from=build /app/UserInterface/Static ./Static

RUN apt-get update && apt-get install -y \
    iputils-ping \
    curl \
    docker.io

RUN curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" \
    -o /usr/local/bin/docker-compose && \
    chmod +x /usr/local/bin/docker-compose

COPY ./scripts/report.sh /app/scripts/report.sh

COPY ./docker-compose.yml /app/docker-compose.yml


RUN chmod +x /app/scripts/report.sh


EXPOSE 8080

ENTRYPOINT ["dotnet", "lab4.dll"]
