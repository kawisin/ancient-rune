FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY ./publish .
ENV ASPNETCORE_URLS=http://+:5252
EXPOSE 5252
ENTRYPOINT ["dotnet", "Login-Service.dll"]
