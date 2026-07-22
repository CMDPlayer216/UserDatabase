#!/bin/bash

mkdir -p ./bin/release

dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -p:EnableCompressionInSingleFile=true -o ./bin/release
chmod +x ./bin/release/userdb
cp -f ./bin/release/userdb ~/.local/bin/userdb