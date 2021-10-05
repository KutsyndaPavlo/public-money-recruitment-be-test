#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:2.2 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:2.2 AS build
WORKDIR /src
COPY ["VacationRental.Api/VacationRental.Api.csproj", "VacationRental.Api/"]
COPY ["VacationRental.Services/VacationRental.Services.csproj", "VacationRental.Services/"]
COPY ["VacationRental.Services.Interface/VacationRental.Services.Interface.csproj", "VacationRental.Services.Interface/"]
COPY ["VacationRental.Dal.Interface/VacationRental.Dal.Interface.csproj", "VacationRental.Dal.Interface/"]
COPY ["VacationRental.Dal.InMemory/VacationRental.Dal.InMemory.csproj", "VacationRental.Dal.InMemory/"]
RUN dotnet restore "VacationRental.Api/VacationRental.Api.csproj"
COPY . .
WORKDIR "/src/VacationRental.Api"
RUN dotnet build "VacationRental.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VacationRental.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VacationRental.Api.dll"]