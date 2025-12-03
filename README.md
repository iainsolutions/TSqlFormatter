
## T-SQL Formatter

A free, open-source library for formatting T-SQL code, with support for SSMS integration, command-line usage, and various other tools.

> **Based on Poor Man's T-SQL Formatter by Tao Klerks**
> This project is a continuation and modernization of the original Poor Man's T-SQL Formatter, maintaining full attribution to the original author and all contributors while updating the architecture for modern development.

### Project Structure (2025 Refactor)

The project has been restructured into a modern, maintainable architecture:

* **TSqlFormatter.Core** - Core .NET 4.7.2 library containing all formatting logic
  * Tokenization and parsing engine
  * Multiple formatter implementations (Standard, Identity, Obfuscating)
  * Thread-safe node implementation for concurrent usage
  * Configurable formatting options

* **TSqlFormatter.SSMS** - SSMS 21+ extension package (supports SSMS 21, 22, and later)
  * Full Options dialog for configuration
  * Keyboard shortcuts (Ctrl+K, Ctrl+F)
  * Persistent settings via Visual Studio settings store

* **TSqlFormatter.VS2026** - Visual Studio 2022+ extension package
  * Supports Visual Studio 2022, 2026, and later
  * Same formatting options as SSMS extension
  * Works with SQL files in Visual Studio IDE
  * Uses traditional VSSDK model for compatibility

### Current Solution

* **TSqlFormatter.SSMS21.sln** - Main solution file with 3 projects (Core + SSMS + VS2026)

### Building the Solution

#### Prerequisites:
* Visual Studio 2022 or later (Visual Studio 2026 recommended)
* .NET Framework 4.7.2
* VSSDK (Visual Studio SDK) for building VSIX packages

#### Build Commands:
```bash
# Build the solution (creates both SSMS and VS extensions)
msbuild TSqlFormatter.SSMS21.sln /p:Configuration=Release /p:Platform="Any CPU"

# Or restore packages first if needed
msbuild /t:Restore TSqlFormatter.SSMS21.sln
msbuild TSqlFormatter.SSMS21.sln /p:Configuration=Release

# The VSIX packages will be created at:
# TSqlFormatter.SSMS\bin\Release\TSqlFormatter.SSMS.vsix (for SSMS)
# TSqlFormatter.VS2026\bin\Release\TSqlFormatter.VS2026.vsix (for Visual Studio)
```

### Installing the Extensions

#### For SSMS:
1. Close all instances of SSMS
2. Double-click the generated `TSqlFormatter.SSMS.vsix` file
3. Follow the installation wizard
4. Restart SSMS
5. Access the formatter via:
   - **Tools > Format T-SQL Code** (or Ctrl+K, Ctrl+F)
   - **Tools > T-SQL Formatter Options...** for settings

#### For Visual Studio 2022/2026:
1. Close all instances of Visual Studio
2. Double-click the generated `TSqlFormatter.VS2026.vsix` file
3. Follow the installation wizard
4. Restart Visual Studio
5. Access the formatter via:
   - **Tools > Format T-SQL Code** (or Ctrl+K, Ctrl+F)
   - **Tools > T-SQL Formatter Options...** for settings

### Features

* **Core Formatting Engine**
  * Simple XML-style parse tree for SQL structure representation
  * Extensible architecture supporting multiple SQL dialects
  * Thread-safe implementation for concurrent formatting operations
  * Fault-tolerant parsing - handles unknown constructs gracefully

* **Formatting Options**
  * **Indentation**: Tabs or spaces with configurable width
  * **Line Width Control**: Maximum line length (50-999 characters)
  * **List Formatting**: Expand comma lists, IN lists with various styles
  * **Expression Formatting**: Boolean expressions, CASE statements, BETWEEN conditions
  * **JOIN Formatting**: Break at ON clauses
  * **Keyword Formatting**: Uppercase and standardization options
  * **Line Breaks**: Control blank lines between clauses and statements

* **Output Formats**
  * Standard formatted T-SQL
  * Colorized HTML output
  * Obfuscated SQL (for demos/examples)

* **Capabilities**
  * Handles complete T-SQL including procedural code
  * Formats entire batches and multi-batch scripts
  * Preserves comments and special formatting where possible
  * Fast performance - processes large codebases efficiently


### General Limitations

* This is NOT a full SQL-parsing solution: only "coarse" parsing is performed, the 
    minimum necessary for re-formatting.
* The standard formatter does not always maintain the order of comments in the code;
    a comment inside an "INNER JOIN" compound keyword, like "inner/\*test\*/join", would
    get moved out, to "INNER JOIN /\*test\*/". The original data is maintaned in the 
    parse tree, but the standard formatter shuffles comments in cases like this for 
    clarity.
* DDL parsing, in particular, is VERY coarse - the bare minimum to display ordered table 
    column and procedure parameter declarations.
* No effort has been made to support compatibility level 70 (SQL Server 7)
* Where there is ambiguity between different compatibility levels (eg cross apply 
    parens in compatibility level 90 vs table hints without "WITH" keyword in 
    compatibility level 80), no approach has been decided. For now, table hints 
    without WITH are considered to be arguments to a function.
* Settings may not be correctly maintained across major upgrades of SSMS and Visual Studio
 

### Known Issues / Todo

* Handling of DDL Triggers (eg "FOR LOGON")
* Formatting/indenting of ranking functions
* FxCop checking
* And other stuff that is tracked in the GitHub issues list


### Longer-term enhancements / additions

* Compiled mono library + bulk formatting tool download (eg for use on SVN server)
* Documentation of Xml structure and class usage
    * Keeping track of versioning and documentation more carefully: http://semver.org/

### License & Credits

This application and library is released under the GNU Affero GPL v3:
http://www.gnu.org/licenses/agpl.txt

**Original Project**: Poor Man's T-SQL Formatter by Tao Klerks
Original homepage: http://www.architectshack.com/PoorMansTSqlFormatter.ashx

This project uses several external libraries:

* NDesk.Options, for command-line parsing: The NDesk.Options library is licensed under 
    the MIT/X11 license, and its homepage is here: http://www.ndesk.org/Options
* LinqBridge, for convenience, supporting extension methods and Linq-to-Objects 
    despite this being a .Net 2.0 library. LinqBridge is licensed under the BSD 3-clause 
    license, and its homepage is here: http://code.google.com/p/linqbridge/
* NUnit, for automated testing. NUnit is licensed under a custom open-source license
    based on the zlib/libpng license, and its homepage is: http://www.nunit.org/
* UnmanagedExports (DLLExport), for exporting .Net code to Notepad++ plugin environment
* Notepad++ C# plugin template, based on work by Robert Giesecke and UFO, 
    available from the [notepad++ plugin development forum](https://sourceforge.net/projects/notepad-plus/forums/forum/482781).
* ILRepack, by François Valdy, for assembly-merging, available from the [github project page](https://github.com/gluck/il-repack).
* Bridge.Net, by Object.Net, for C#-to-JS transpiling, available from http://bridge.net

#### Original Author
* **Tao Klerks** - Original creator of Poor Man's T-SQL Formatter

#### Contributors
Special thanks to contributors that have given their time to make this library better:

* Timothy Klenke

Also thanks to Adam Pawsey, who maintains the [NuGet package](http://nuget.org/packages/PoorMansTSQLFormatter/).

Many of the features in this project result from feedback by multiple people, including
but not limited to:

* Loren Halvorson
* Recep Guzel
* Lane Duncan
* Gokhan Varol
* Pushpendra Rishi
* Jonathan Fahey
* Tim Costello
* Jörg Burdorf
* William Lin
* Brad Wood
* Richard King
* Jeff Clark
* Jarred Cleem
* Paul Toms
* Tom Holden
* Marvin Eads
* Bill Ruehle
* Farzad Jalali
* Sheldon Hull
* Benjamin Solomon


Translation work on this project was originally facilitated by [Amanuens](http://amanuens.com/), the online translation platform that is now sadly defunct.

---

**Original Author Contact**: Tao Klerks (tao at klerks dot biz)

**Current Repository**: https://github.com/iainsolutions/TSqlFormatter

This project continues to be developed while maintaining full respect for the original author's work and the open-source license under which it was released.

