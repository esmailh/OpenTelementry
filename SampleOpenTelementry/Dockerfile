#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SampleOpenTelementry/SampleOpenTelementry.csproj", "SampleOpenTelementry/"]
RUN dotnet restore "./SampleOpenTelementry/./SampleOpenTelementry.csproj"
COPY . .
WORKDIR "/src/SampleOpenTelementry"
RUN dotnet build "./SampleOpenTelementry.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SampleOpenTelementry.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV OTEL_EXPORTER_JAEGER_AGENT_HOST=jaeger
ENV OTEL_EXPORTER_JAEGER_AGENT_PORT=6831
ENV OTEL_DOTNET_AUTO_TRACES_ADDITIONAL_SOURCES=SampleOpenTelementry.ManualInstrumentations.*
ENTRYPOINT ["dotnet", "SampleOpenTelementry.dll"]