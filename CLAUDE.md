# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Poor Man's T-SQL Formatter is a .NET 2.0 and JavaScript library for reformatting T-SQL code. It includes multiple components: a core formatting library, WinForms demo app, SSMS/Visual Studio add-ins, command-line utility, Notepad++ plugin, WinMerge plugin, and web service.

The project now includes a streamlined SSMS 21 solution (`PoorMansTSqlFormatterSSMS21.sln`) specifically for SQL Server Management Studio 21+ support.

## Build Commands

### Building the Solutions
```bash
# Build the main solution (requires Visual Studio 2013+ or MSBuild)
msbuild PoorMansTSqlFormatter.sln /p:Configuration=Release /p:Platform="Any CPU"

# Build the streamlined SSMS 21 solution
msbuild PoorMansTSqlFormatterSSMS21.sln /p:Configuration=Release /p:Platform="Any CPU"

# Build the .NET Standard version (if exists)
msbuild PoorMansTSqlFormatterNetStandard.sln /p:Configuration=Release

# Build specific VS/SSMS packages
msbuild PoorMansTSqlFormatterSSMSPackage2021\PoorMansTSqlFormatterSSMSPackage2021.csproj /p:Configuration=Release
msbuild PoorMansTSqlFormatterVSPackage2019\PoorMansTSqlFormatterVSPackage2019.csproj /p:Configuration=Release
```

### Running Tests
```bash
# Run tests using the NUnit GUI runner (tests are in PoorMansTSqlFormatterTest project)
PoorMansTSqlFormatterTest\bin\Release\PoorMansTSqlFormatterTests.exe

# Tests use NUnit 2.5.10 framework with bundled GUI runner
# Test data is file-based comparing input, parsing, and formatting results
```

### Version Management
```powershell
# Bump version using PowerShell script (from project root)
.\VersionBump.ps1 patch  # Bump patch version
.\VersionBump.ps1 minor  # Bump minor version
.\VersionBump.ps1 major  # Bump major version
.\VersionBump.ps1 1.2.3  # Set specific version
```

## Architecture

### Core Library Structure
- **PoorMansTSqlFormatterLibShared/** - Shared core logic (tokenizing, parsing, formatting)
  - `SqlFormattingManager.cs` - Main entry point for formatting operations
  - `Tokenizers/` - SQL tokenization logic (TSqlStandardTokenizer)
  - `Parsers/` - SQL parsing logic (TSqlStandardParser)
  - `Formatters/` - Various formatters (TSqlStandardFormatter, TSqlObfuscatingFormatter, TSqlIdentityFormatter)
  - `ParseStructure/` - Parse tree node structures

- **PoorMansTSqlFormatterLib/** - .NET 2.0 library wrapper (also has _35 and _472 variants)
- **PoorMansTSqlFormatterNetStandardLib/** - .NET Standard 1.0 library wrapper
- **PoorMansTSqlFormatterJSLib/** - JavaScript library (transpiled using Bridge.NET)

### Key Integration Points
- **Command Line**: PoorMansTSqlFormatterCmdLine - uses NDesk.Options for argument parsing
- **SSMS Add-ins**:
  - PoorMansTSqlFormatterSSMSAddIn (SSMS 2005-2008R2 via COM)
  - PoorMansTSqlFormatterSSMSPackage (SSMS 2012-17)
  - PoorMansTSqlFormatterSSMSPackage2021 (SSMS 18-21+, now with streamlined solution)
- **VS Extensions**:
  - PoorMansTSqlFormatterVSPackage2013
  - PoorMansTSqlFormatterVSPackage2019
- **Notepad++ Plugin**: PoorMansTSqlFormatterNppPlugin - uses UnmanagedExports for native integration
- **Web Demo**: PoorMansTSqlFormatterWebDemo - includes JS library and web service

### SSMS 21 Streamlined Solution Structure
The `PoorMansTSqlFormatterSSMS21.sln` solution contains:
- **PoorMansTSqlFormatterLib_472** - Core formatting library for .NET 4.7.2
- **PoorMansTSqlFormatterPluginShared_472** - Shared plugin functionality
- **PoorMansTSqlFormatterSSMSLib_472** - SSMS-specific library components
- **PoorMansTSqlFormatterSSMSPackage2021** - VSIX package for SSMS 21+

### Test Data Organization
Tests are located in `PoorMansTSqlFormatterTest/Data/` with three subdirectories:
- `InputSql/` - Raw SQL input files
- `ParsedSql/` - Expected parse tree outputs
- `StandardFormatSql/` - Expected formatted outputs (with optional formatting parameters in filenames)

File naming convention: test files can encode formatting options in parentheses, e.g.:
- `04_MiscProceduralSample_Unstructured(ExpandCommaLists=false,MaxLineWidth=60,SpacesPerTab=8).txt`

## Key Dependencies
- LinqBridge - LINQ support for .NET 2.0
- NDesk.Options - Command-line parsing
- NUnit 2.5.10 - Testing framework
- Bridge.NET 16.0.0-beta4 - C# to JavaScript transpiler
- ILRepack - Assembly merging for single-file distributions
- Microsoft.VSSDK.BuildTools - VS extension build support

## Formatting Pipeline
1. **Tokenization**: SQL string → Token list (ISqlTokenizer)
2. **Parsing**: Token list → Parse tree (ISqlTokenParser)
3. **Formatting**: Parse tree → Formatted SQL string (ISqlTreeFormatter)

The SqlFormattingManager orchestrates this pipeline and handles error states gracefully (fault-tolerant parsing).

## Important Notes

- The solution maintains backward compatibility with .NET 2.0 while also supporting modern .NET Standard
- Multiple project variants exist for different .NET framework versions (_35 suffix for .NET 3.5, _472 for .NET 4.7.2)
- Build-time file replacements use fart.exe (bundled) for ComponentResourceManager modifications
- Post-build merging uses ILRepack to create single-assembly distributions
- The formatter is fault-tolerant - it attempts to format even malformed SQL with best-effort results