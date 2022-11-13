#!/bin/bash

echo "Restoring .NET Core tools"
dotnet tool restore --configfile ./nuget.config

echo "Bootstrapping Cake"
dotnet cake build.cake --bootstrap

echo "Running Build"
dotnet cake build.cake "$@"
