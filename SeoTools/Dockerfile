FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000


# build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /soruce
COPY . .
RUN dotnet restore "SeoTools/SeoTools.csproj"

WORKDIR "/soruce/SeoTools"

RUN dotnet build "SeoTools.csproj" -c Release -o /app


#server stage
FROM build AS publish
WORKDIR "/soruce/SeoTools"
RUN dotnet publish "SeoTools.csproj" -c Release -o /app



FROM base  AS final
WORKDIR /app
COPY --from=publish /app .

EXPOSE 5000

ENTRYPOINT ["dotnet", "SeoTools.dll"]
