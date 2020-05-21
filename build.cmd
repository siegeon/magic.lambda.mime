
set version=%1
set key=%2

cd %~dp0
dotnet build magic.lambda.mime/magic.lambda.mime.csproj --configuration Release --source https://api.nuget.org/v3/index.json
dotnet nuget push magic.lambda.mime/bin/Release/magic.lambda.mime.%version%.nupkg -k %key% -s https://api.nuget.org/v3/index.json
