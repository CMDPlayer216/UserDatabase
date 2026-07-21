#!/bin/bash

dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -p:EnableCompressionInSingleFile=true -o ./publish
chmod +x ./publish/.userdatabase
cp -f ./publish/.userdatabase ~/Escritorio/userdb