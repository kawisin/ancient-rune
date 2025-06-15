FROM mcr.microsoft.com/dotnet/aspnet:7.0

# ติดตั้ง dependency ที่ต้องใช้
RUN apt-get update && \
    apt-get install -y \
        libglib2.0-0 \
        libsecret-1-0 \
        libmariadb3 \
    && apt-get clean

# ✅ สร้าง user ที่ไม่ใช่ root
RUN useradd -m mmouser

# สร้างโฟลเดอร์ และให้สิทธิ์ user ใหม่
RUN mkdir -p /app/MMORPGServer && chown -R mmouser:mmouser /app

WORKDIR /app

# ✅ ใช้ user ที่ไม่ใช่ root
USER mmouser

# คัดลอกไฟล์
COPY ./publish .

ENV ASPNETCORE_URLS=http://+:5251
EXPOSE 5251

ENTRYPOINT ["dotnet", "Instance-Service.dll"]
