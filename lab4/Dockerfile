FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /app

COPY . ./

RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

WORKDIR /app

COPY --from=build /app/out .
COPY --from=build /app/UserInterface/Static ./Static

# Установка ping и Docker CLI
RUN apt-get update && apt-get install -y iputils-ping docker.io

# Копируем скрипт report.sh
COPY ./scripts/report.sh /app/scripts/report.sh

# Убедитесь, что скрипт имеет права на выполнение
RUN chmod +x /app/scripts/report.sh

EXPOSE 8080

ENTRYPOINT ["dotnet", "lab4.dll"]
