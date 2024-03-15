FROM mcr.microsoft.com/dotnet/sdk:7.0 as build-env
WORKDIR /CommunicationApp


#copy csproject and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore


#copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

#build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /CommunicationApp
COPY --from=build-env /CommunicationApp/out .
ENTRYPOINT [ "dotnet","CommunicationAPI.dll" ]

