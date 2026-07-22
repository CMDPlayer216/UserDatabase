#!/bin/bash

mkdir -p ./publish/linux-x64
mkdir -p ./publish/windows-x64

dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -p:EnableCompressionInSingleFile=true -o ./publish/linux-x64
chmod +x ./publish/linux-x64/userdb
cp -f ./publish/linux-x64/userdb ~/Escritorio/userdb-linux-x64

dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ./publish/windows-x64
chmod +x ./publish/winddows-x64/userdb.exe
cp -f ./publish/windows-x64/userdb.exe ~/Escritorio/userdb-windows-x64.exe