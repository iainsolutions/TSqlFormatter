# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Poor Man's T-SQL Formatter is a .NET 2.0 and JavaScript library for reformatting T-SQL code. It includes multiple components: a core formatting library, WinForms demo app, SSMS/Visual Studio add-ins, command-line utility, Notepad++ plugin, WinMerge plugin, and web service.

The project maintains three main solutions:
- `PoorMansTSqlFormatter.sln` - Main solution with all components
- `PoorMansTSqlFormatterSSMS21.sln` - Legacy SSMS 21 support (4 projects)
- `TSqlFormatter.SSMS21.sln` - New streamlined SSMS 21 solution (2 projects: Core + SSMS)

## Build Commands

### Building the Solutions
```bash
# Build the main solution (requires Visual Studio 2013+ or MSBuild)
msbuild PoorMansTSqlFormatter.sln /p:Configuration=Release /p:Platform="Any CPU"

# Build the streamlined SSMS 21 solution
msbuild TSqlFormatter.SSMS21.sln /p:Configuration=Release /p:Platform="Any CPU"

# Build the legacy SSMS 21 solution
msbuild PoorMansTSqlFormatterSSMS21.sln /p:Configuration=Release /p:Platform="Any CPU"

# Restore NuGet packages before building (if needed)
msbuild /t:Restore PoorMansTSqlFormatter.sln
msbuild /t:Restore TSqlFormatter.SSMS21.sln

# Build specific VS/SSMS packages
msbuild PoorMansTSqlFormatterSSMSPackage2021\PoorMansTSqlFormatterSSMSPackage2021.csproj /p:Configuration=Release
msbuild TSqlFormatter.SSMS\TSqlFormatter.SSMS.csproj /p:Configuration=Release
```

### Running Tests
```bash
# Tests are in PoorMansTSqlFormatterTest project (when it exists)
# Uses NUnit 2.5.10 framework with file-based test data
# Test data location: PoorMansTSqlFormatterTest/Data/
```

### Version Management
```powershell
# Bump version using PowerShell script (from project root)
.\VersionBump.ps1 patch  # Bump patch version
.\VersionBump.ps1 minor  # Bump minor version
.\VersionBump.ps1 major  # Bump major version
.\VersionBump.ps1 1.2.3  # Set specific version

# Version source: appveyor.yml
# Updates: AssemblyInfo.cs, source.extension.vsixmanifest, FormatterPackage.cs
```

## Architecture

### Core Library Structure
- **PoorMansTSqlFormatterLibShared/** - Shared core logic (tokenizing, parsing, formatting)
  - `SqlFormattingManager.cs` - Main entry point for formatting operations
  - `Tokenizers/` - SQL tokenization logic (TSqlStandardTokenizer)
  - `Parsers/` - SQL parsing logic (TSqlStandardParser)
  - `Formatters/` - Various formatters (TSqlStandardFormatter, TSqlObfuscatingFormatter, TSqlIdentityFormatter)
  - `ParseStructure/` - Parse tree node structures

- **PoorMansTSqlFormatterLib/** - .NET 2.0 library wrapper
- **PoorMansTSqlFormatterLib_35/** - .NET 3.5 variant
- **PoorMansTSqlFormatterLib_472/** - .NET 4.7.2 variant
- **TSqlFormatter.Core/** - New .NET 4.7.2 core library for streamlined solution
- **PoorMansTSqlFormatterJSLib/** - JavaScript library (transpiled using Bridge.NET)

### Key Integration Points
- **Command Line**: PoorMansTSqlFormatterCmdLine - uses NDesk.Options for argument parsing
- **SSMS Add-ins**:
  - PoorMansTSqlFormatterSSMSPackage2021 - SSMS 18-21+ (VSIX package, targets `Id="SSMS"`)
  - TSqlFormatter.SSMS - New streamlined SSMS 21 package
- **VS Extensions**:
  - PoorMansTSqlFormatterVSPackage2013 - Visual Studio 2013
  - PoorMansTSqlFormatterVSPackage2019 - Visual Studio 2019+
- **Notepad++ Plugin**: PoorMansTSqlFormatterNppPlugin - uses UnmanagedExports for native integration

### VSIX Manifest Configuration
SSMS extensions must target `Id="SSMS"` (not `Microsoft.VisualStudio.Ssms`) with version range like `[21.0,22.0)` for SSMS 21 compatibility.

### Test Data Organization
When tests are configured, they use file-based comparisons:
- `InputSql/` - Raw SQL input files
- `ParsedSql/` - Expected parse tree outputs
- `StandardFormatSql/` - Expected formatted outputs

File naming convention encodes formatting options in parentheses:
- Example: `Sample(ExpandCommaLists=false,MaxLineWidth=60,SpacesPerTab=8).txt`

## Formatting Pipeline
1. **Tokenization**: SQL string → Token list (ISqlTokenizer)
2. **Parsing**: Token list → Parse tree (ISqlTokenParser)
3. **Formatting**: Parse tree → Formatted SQL string (ISqlTreeFormatter)

The SqlFormattingManager orchestrates this pipeline and handles error states gracefully (fault-tolerant parsing).

## Key Dependencies
- LinqBridge - LINQ support for .NET 2.0
- NDesk.Options - Command-line parsing
- NUnit 2.5.10 - Testing framework
- Bridge.NET 16.0.0-beta4 - C# to JavaScript transpiler
- ILRepack - Assembly merging for single-file distributions
- Microsoft.VSSDK.BuildTools - VS/SSMS extension build support

## Important Notes

- The solution maintains backward compatibility with .NET 2.0 while supporting modern .NET versions
- Multiple project variants exist for different .NET framework versions (_35 suffix for .NET 3.5, _472 for .NET 4.7.2)
- Build-time file replacements use fart.exe (bundled) for ComponentResourceManager modifications
- Post-build merging uses ILRepack to create single-assembly distributions
- The formatter is fault-tolerant - it attempts to format even malformed SQL with best-effort results
- SSMS 21 requires .NET Framework 4.7.2 minimum