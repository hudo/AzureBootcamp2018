FROM microsoft/dotnet:2.0-sdk AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY BootShop.Web.sln .
COPY BootShop.Web.API/*.csproj ./BootShop.Web.API/
COPY BootShop.Common/*.csproj ./BootShop.Common/
RUN dotnet restore

# copy and build everything else
COPY BootShop.Web.API/. ./BootShop.Web.API/
COPY BootShop.Common/. ./BootShop.Common/

RUN dotnet build

FROM build AS publish
WORKDIR /app/BootShop.Web.API
RUN dotnet publish -o out

FROM microsoft/dotnet:2.0-runtime AS runtime
WORKDIR /app
COPY --from=publish /app/BootShop.Web.API/out ./
ENTRYPOINT ["dotnet", "BootShop.Web.API.dll"]