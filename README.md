<p align="center">
  <img src="art/dotnet-document.png"/>
</p>

# dotnet-document
A cross platform tool that auto generates an XML doc starting point for your C# codebase.

Thanks to `Microsoft.CodeAnalysis.CSharp` this tool is able to identify undocumented members and to generate a meaningful XML doc by *humanizing* member names.

## Installation
The tool can be installed globally via Nuget by running 

```sh
dotnet tool install --global dotnet-document --version 0.1.0-alpha
```
> 👉 When installing pre releases the version has to be explicitly specified

## How to run

### Apply doc

To run the tool invoke `dotnet document apply`

```sh
# Documents all *.cs files in the current dir and all sub dirs 
dotnet document apply

# Documents the specified .cs file
dotnet document apply ./src/folder/MyClass.cs

# Documents all *.cs files in the specified dir and all sub dirs 
dotnet document apply ./src/folder/

# Documents all *.cs files in the specified solution
dotnet document apply ./src/solution.sln

# Documents all *.cs files in the specified project
dotnet document apply ./src/folder/project.csproj
```

### Dry run
To test the command without saving changes on disk a dry run option is available.

In case of undocumented members a non-zero exit code is returned so that it is possible to warn about it  during CI.
```sh
dotnet document apply --dry-run
```
## Configuration

The tool can be configured so that document templates match the developer preferred style.

The default configuration can be seen by invoking `dotnet document config --default`.

To apply changes on it simply save it somewhere by invoking

```sh
dotnet document config --default > ~/dotnet-document.yaml
```

Once changed provide it when calling the `apply` command via `-c` argument

```sh
dotnet document apply \
  -c ~/dotnet-document.yaml \
  ./src/folder/
```

## Acknowledgments
* [CommandLine](https://github.com/commandlineparser/commandline) - Used for parsing command line args
* [Humanizer](https://github.com/Humanizr/Humanizer) - Used for humanizing member names
* [Ensure.That](https://github.com/danielwertheim/Ensure.That) - Used as a guard clause
* [FluentAssertions](https://fluentassertions.com/) - Used for wrinting better assertions
* [Moq4](https://github.com/Moq/moq4) - A mocking library for easier testing
* [xUnit](https://github.com/xunit/xunit) - The test framework