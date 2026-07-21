#!/bin/bash

dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ./publish
chmod +x ./publish/.userdatabase
cp -f ./publish/.userdatabase.exe ~/Escritorio/userdb.exe