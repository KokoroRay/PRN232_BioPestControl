FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
ARG RID=linux-x64

COPY ["agri-expert-service/agri-expert-service.csproj", "agri-expert-service/"]
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet restore "agri-expert-service/agri-expert-service.csproj" \
    -r $RID -p:PublishReadyToRun=true

COPY . .
WORKDIR "/src/agri-expert-service"
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish "agri-expert-service.csproj" \
    -c Release -o /app/publish -r $RID --no-restore \
    -p:PublishReadyToRun=true -p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Chỉ copy kết quả đã build xong, bỏ lại 1GB SDK ở stage trước
COPY --from=build /app/publish .

# Bảo mật: Sử dụng user hạn quyền có sẵn
USER $APP_UID
EXPOSE 8080

# Chạy ứng dụng (Sửa lỗi /bin/sh)
ENTRYPOINT ["dotnet", "agri-expert-service.dll"]