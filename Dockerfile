# Base image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# Build image with SDK
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy all files from your repo into the container
COPY . .

# Change directory to your project folder
WORKDIR /src/VR2

# Restore dependencies
RUN dotnet restore "VR2.csproj"

# Build and publish to a folder
RUN dotnet publish "VR2.csproj" -c Release -o /app/publish

# Final image to run the app
FROM base AS final
WORKDIR /app

# Copy published files from the build image
COPY --from=build /app/publish .

# Run the application
ENTRYPOINT ["dotnet", "VR2.dll"]
