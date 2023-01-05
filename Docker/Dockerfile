FROM mcr.microsoft.com/dotnet/aspnet:6.0
COPY bin/Debug/net6.0/publish/ App/

EXPOSE 80

WORKDIR /App
ENTRYPOINT ["dotnet", "DockerNetwork.dll"]