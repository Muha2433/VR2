# المرحلة الأولى: صورة التشغيل الخفيفة
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# المرحلة الثانية: البناء
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# نسخ كل ملفات المشروع
COPY . .

# استرجاع الحزم
RUN dotnet restore "./VR2.csproj"

# نشر المشروع (compile + publish)
RUN dotnet publish "./VR2.csproj" -c Release -o /app/publish

# المرحلة النهائية: تشغيل المشروع
FROM base AS final
WORKDIR /app

# نسخ الملفات المنشورة من مرحلة البناء
COPY --from=build /app/publish .

# تشغيل التطبيق
ENTRYPOINT ["dotnet", "VR2.dll"]
