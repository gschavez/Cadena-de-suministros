# ---------------------------------------------------------
# Etapa 1: Compilación (Build)
# ---------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar el archivo del proyecto y restaurar dependencias
COPY ["CadenaSuministro.csproj", "./"]
RUN dotnet restore "CadenaSuministro.csproj"

# Copiar el resto del código y compilar en modo Release
COPY . .
RUN dotnet publish "CadenaSuministro.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ---------------------------------------------------------
# Etapa 2: Ejecución (Final)
# ---------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080

# Configurar ASP.NET Core para escuchar en el puerto 8080
ENV ASPNETCORE_URLS=http://+:8080

# Copiar solo los archivos compilados de la etapa anterior
COPY --from=build /app/publish .

# Comando para iniciar la aplicación
ENTRYPOINT ["dotnet", "CadenaSuministro.dll"]