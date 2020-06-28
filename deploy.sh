#!/usr/bin/env bash

dotnet build -c Release

dotnet publish -c Release /p:PublishReadyToRun=true /p:PublishTrimmed=true -o publish

vercel --prod publish/wwwroot/
