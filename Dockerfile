FROM mcr.microsoft.com/dotnet/sdk:9.0 as build

WORKDIR /src

COPY ./nuget.config .
COPY ./src ./src
COPY MNX.SubscriptionManagement.sln .

RUN dotnet restore MNX.SubscriptionManagement.sln
RUN dotnet publish MNX.SubscriptionManagement.sln -c Release -o /publish


FROM mcr.microsoft.com/dotnet/aspnet:9.0 as runtime

WORKDIR /publish

COPY --from=build /publish .

EXPOSE 80
ENTRYPOINT ["dotnet", "MNX.SubscriptionManagement.Service.dll"]	