
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src
ARG RID=linux-musl-x64

COPY ["ordering-service/ordering-service.csproj", "ordering-service/"]
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet restore "ordering-service/ordering-service.csproj" \
    -r $RID -p:PublishReadyToRun=true

COPY . .
WORKDIR "/src/ordering-service"
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish "ordering-service.csproj" \
    -c Release -o /app/publish -r $RID --no-restore \
    -p:PublishReadyToRun=true -p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app

# Chỉ copy kết quả đã build xong, bỏ lại 1GB SDK ở stage trước
COPY --from=build /app/publish .

# Bảo mật: Sử dụng user hạn quyền có sẵn
USER $APP_UID
EXPOSE 8080

# Chạy ứng dụng (Sửa lỗi /bin/sh)
ENTRYPOINT ["dotnet", "ordering-service.dll"]