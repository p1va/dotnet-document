#!/usr/bin/env bash


#   -p:VersionPrefix=0.0.1\
#  -p:VersionSuffix=alpha\

dotnet pack \
  -c Release --no-restore \
  --include-symbols --include-source \
  -p:PackageID=dotnet-document \
  #-p:PackageVersion=0.0.2-alpha \
  src/DotnetDocument.Tools/DotnetDocument.Tools.csproj
