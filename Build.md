# Build

This project uses GitVersion to calculate version numbers. If a major bump has to be made (i. e. for a public release), the file `version.json` should be changed accordingly.
More info from [Nerdbank here](https://github.com/dotnet/Nerdbank.GitVersioning/blob/master/doc/public_vs_stable.md).

To build the project for packaging, run the following command in the root directory:

```powershell
dotnet build src -c Release
```

This will build and pack the assemblies to according output directories:
- `\src\FluentDragDrop\bin\Release` 
- `\src\FluentDragDrop.Effects\bin\Release`
