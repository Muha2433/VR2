# صورة التشغيل الأساسية (runtime) لـ .NET 8
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# صورة البناء (SDK) لـ .NET 8
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# نسخ كل ملفات المشروع إلى الحاوية
COPY . .

# الذهاب إلى مجلد المشروع
WORKDIR /src/VR2

# استرجاع الحزم
RUN dotnet restore "VR2.csproj"

# بناء المشروع ونشره إلى مجلد مستقل
RUN dotnet publish "VR2.csproj" -c Release -o /app/publish

# صورة التشغيل النهائية
FROM base AS final
WORKDIR /app

# نسخ الملفات المنشورة من مرحلة البناء
COPY --from=build /app/publish .

# تعيين نقطة دخول التطبيق
ENTRYPOINT ["dotnet", "VR2.dll"]
