# Set Working Directory
Split-Path $MyInvocation.MyCommand.Path | Push-Location
[Environment]::CurrentDirectory = $PWD

Remove-Item "$env:RELOADEDIIMODS/Ougon_Trainer/*" -Force -Recurse
dotnet publish "./Ougon_Trainer.csproj" -c Release -o "$env:RELOADEDIIMODS/Ougon_Trainer" /p:OutputPath="./bin/Release" /p:ReloadedILLink="true"

# Restore Working Directory
Pop-Location