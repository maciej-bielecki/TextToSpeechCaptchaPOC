#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.4-bionic AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# without it we will have Unable to load shared library 'libMicrosoft.CognitiveServices.Speech.core.so' or one of its dependencies
# https://github.com/Azure-Samples/cognitive-services-speech-sdk/blob/master/quickstart/cpp/linux/from-microphone/README.md
RUN apt-get update
RUN apt-get -y install build-essential libssl1.0.0 libasound2 wget
RUN apt-get -y install libc6-dev 
RUN apt-get -y install libgdiplus

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-bionic AS build
WORKDIR /src
COPY ["TextToSpeechOnSpeechService/TextToSpeechOnSpeechService.csproj", "TextToSpeechOnSpeechService/"]
RUN dotnet restore "TextToSpeechOnSpeechService/TextToSpeechOnSpeechService.csproj"
COPY . .
WORKDIR "/src/TextToSpeechOnSpeechService"
RUN dotnet build "TextToSpeechOnSpeechService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TextToSpeechOnSpeechService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TextToSpeechOnSpeechService.dll"]