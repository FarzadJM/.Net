FROM mcr.microsoft.com/dotnet/aspnet:6.0
COPY bin/Debug/net6.0/publish/ App/

EXPOSE 80

RUN rm -rf /etc/localtime
RUN ln -s /usr/share/zoneinfo/Asia/Tehran /etc/localtime

WORKDIR /App
ENTRYPOINT ["dotnet", "DockerNetwork.dll"]
