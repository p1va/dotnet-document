<p align="center">
  <img src="art/dotnet-document.png"/>
</p>

# dotnet-document
Auto-generate XML documentations for your C# code


## Installation

```sh
dotnet tool install --global dotnet-document --version 0.1.0-alpha
```

## How to run

```sh
dotnet document apply --dry-run
```

```sh
dotnet document apply
```

```sh
dotnet document apply ./src/project/MyClass.cs
dotnet document apply ./src/project/
dotnet document apply ./src/solution.sln
dotnet document apply ./src/project/project.csproj
```

## Configuration


```sh
dotnet document config --default > ~/dotnet-document.yaml
```
```sh
dotnet document apply -c ~/dotnet-document.yaml
```
