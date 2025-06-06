# ---------- Stage 1: Build ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers for better caching
COPY VR2/*.csproj ./VR2/
RUN dotnet restore ./VR2/VR2.csproj

# Copy the rest of the application source code
COPY . .

# Build and publish the project
WORKDIR /src/VR2
RUN dotnet publish VR2.csproj -c Release -o /app/publish --no-restore

# ---------- Stage 2: Runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published app from build stage
COPY --from=build /app/publish .

# Expose the port the app listens on
EXPOSE 80

# Run the application
ENTRYPOINT ["dotnet", "VR2.dll"]
