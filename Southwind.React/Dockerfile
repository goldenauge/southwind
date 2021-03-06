#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src

## RUN apt-get update
RUN apt-get -y install curl gnupg
RUN curl -sL https://deb.nodesource.com/setup_13.x  | bash -
RUN apt-get -y install nodejs
RUN npm install -g yarn

COPY ["Framework/Signum.Utilities/Signum.Utilities.csproj", "Framework/Signum.Utilities/"]
COPY ["Framework/Signum.Entities/Signum.Entities.csproj", "Framework/Signum.Entities/"]
COPY ["Framework/Signum.Engine/Signum.Engine.csproj", "Framework/Signum.Engine/"]
COPY ["Framework/Signum.React/Signum.React.csproj", "Framework/Signum.React/"]
COPY ["Framework/Signum.React/package.json", "Framework/Signum.React/"]
COPY ["Framework/Signum.React/yarn.lock", "Framework/Signum.React/"]


COPY ["Extensions/Signum.Entities.Extensions/Signum.Entities.Extensions.csproj", "Extensions/Signum.Entities.Extensions/"]
COPY ["Extensions/Signum.Engine.Extensions/Signum.Engine.Extensions.csproj", "Extensions/Signum.Engine.Extensions/"]
COPY ["Extensions/Signum.Engine.MachineLearning.CNTK/Signum.Engine.MachineLearning.CNTK.csproj", "Extensions/Signum.Engine.MachineLearning.CNTK/"]
COPY ["Extensions/Signum.React.Extensions/Signum.React.Extensions.csproj", "Extensions/Signum.React.Extensions/"]
COPY ["Extensions/Signum.React.Extensions/package.json", "Extensions/Signum.React.Extensions/"]
COPY ["Extensions/Signum.React.Extensions/yarn.lock", "Extensions/Signum.React.Extensions/"]

COPY ["Southwind.Entities/Southwind.Entities.csproj", "Southwind.Entities/"]
COPY ["Southwind.Logic/Southwind.Logic.csproj", "Southwind.Logic/"]
COPY ["Southwind.React/Southwind.React.csproj", "Southwind.React/"]
COPY ["Southwind.React/package.json", "Southwind.React/"]
COPY ["Southwind.React/yarn.lock", "Southwind.React/"]

WORKDIR "/src/Southwind.React"
RUN yarn install

WORKDIR /src
RUN dotnet restore "Southwind.React/Southwind.React.csproj"

COPY . .
WORKDIR "/src/Southwind.React"
RUN dotnet build "Southwind.React.csproj" -c Release # -o /app/build
# RUN yarn run build-production
RUN yarn run build-development

FROM build AS publish
RUN dotnet publish "Southwind.React.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Southwind.React.dll"]
