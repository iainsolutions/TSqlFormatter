/*
T-SQL Formatter - A modern, fast, and thread-safe T-SQL formatting library
Copyright (C) 2011-2025 Tao Klerks and Contributors

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.Threading;
using System.Threading.Tasks;

namespace TSqlFormatter.Interfaces
{
    /// <summary>
    /// Main interface for T-SQL formatting operations
    /// </summary>
    public interface ITSqlFormatter
    {
        /// <summary>
        /// Formats the provided T-SQL string synchronously
        /// </summary>
        /// <param name="sql">The SQL string to format</param>
        /// <param name="options">Optional formatting options</param>
        /// <returns>A FormattingResult containing the formatted SQL and any errors</returns>
        FormattingResult Format(string sql, FormattingOptions? options = null);

        /// <summary>
        /// Formats the provided T-SQL string asynchronously
        /// </summary>
        /// <param name="sql">The SQL string to format</param>
        /// <param name="options">Optional formatting options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A FormattingResult containing the formatted SQL and any errors</returns>
        Task<FormattingResult> FormatAsync(string sql, FormattingOptions? options = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Validates T-SQL syntax without formatting
        /// </summary>
        /// <param name="sql">The SQL string to validate</param>
        /// <returns>A ValidationResult containing any syntax errors found</returns>
        ValidationResult Validate(string sql);

        /// <summary>
        /// Obfuscates sensitive data in T-SQL (replaces literals with placeholders)
        /// </summary>
        /// <param name="sql">The SQL string to obfuscate</param>
        /// <returns>The obfuscated SQL string</returns>
        string Obfuscate(string sql);
    }

    /// <summary>
    /// Result of a formatting operation
    /// </summary>
    public class FormattingResult
    {
        public string FormattedSql { get; init; } = string.Empty;
        public bool Success { get; init; }
        public string? ErrorMessage { get; init; }
        public int? ErrorLine { get; init; }
        public int? ErrorColumn { get; init; }
        public FormattingStatistics? Statistics { get; init; }
    }

    /// <summary>
    /// Statistics about the formatting operation
    /// </summary>
    public class FormattingStatistics
    {
        public int TokenCount { get; init; }
        public int StatementCount { get; init; }
        public long ElapsedMilliseconds { get; init; }
        public int LinesFormatted { get; init; }
    }

    /// <summary>
    /// Result of a validation operation
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; init; }
        public ValidationError[] Errors { get; init; } = [];
    }

    /// <summary>
    /// Represents a validation error
    /// </summary>
    public class ValidationError
    {
        public string Message { get; init; } = string.Empty;
        public int Line { get; init; }
        public int Column { get; init; }
        public ValidationErrorSeverity Severity { get; init; }
    }

    /// <summary>
    /// Severity levels for validation errors
    /// </summary>
    public enum ValidationErrorSeverity
    {
        Info,
        Warning,
        Error
    }

    /// <summary>
    /// Options for controlling T-SQL formatting
    /// </summary>
    public class FormattingOptions
    {
        public static FormattingOptions Default { get; } = new FormattingOptions();

        // Indentation
        public int IndentSize { get; init; } = 4;
        public bool UseTabs { get; init; } = false;

        // Line breaks
        public int MaxLineWidth { get; init; } = 999;
        public bool ExpandCommaLists { get; init; } = true;
        public bool ExpandBooleanExpressions { get; init; } = true;
        public bool ExpandCaseStatements { get; init; } = true;
        public bool ExpandBetweenConditions { get; init; } = true;
        public bool ExpandInLists { get; init; } = true;
        public int ExpandInListsAtMoreThanNItems { get; init; } = 3;

        // Spacing
        public bool SpaceAfterComma { get; init; } = true;
        public bool SpaceAroundOperators { get; init; } = true;

        // Keywords
        public KeywordCasing KeywordCasing { get; init; } = KeywordCasing.Upper;
        public KeywordCasing BuiltInFunctionCasing { get; init; } = KeywordCasing.Upper;
        public KeywordCasing DataTypeCasing { get; init; } = KeywordCasing.Upper;

        // Other options
        public bool TrailingCommas { get; init; } = false;
        public bool ObfuscateMode { get; init; } = false;
        public bool ColorizeOutput { get; init; } = false;
        public bool PreserveComments { get; init; } = true;
        public bool AlignColumnDefinitions { get; init; } = false;
    }

    /// <summary>
    /// Casing options for keywords
    /// </summary>
    public enum KeywordCasing
    {
        Lower,
        Upper,
        Capitalize,
        AsIs
    }
}
