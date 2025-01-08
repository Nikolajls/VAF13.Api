#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging. 

#use asp.net image as the base that will be the final container base
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443


#Use dotnet sdk with tooling as the build container
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .


#Restore nuget packages using the nuget.config from repository
#RUN dotnet restore 

#build 
WORKDIR "/src/VAF13.Klubadmin.API/"
RUN dotnet build "VAF13.Klubadmin.API.csproj" -c Release -o /app/build

#publish
FROM build AS publish
RUN dotnet publish "VAF13.Klubadmin.API.csproj" -c Release -o /app/publish

#copy from the the publishing image onto the base 
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VAF13.Klubadmin.API.dll"]