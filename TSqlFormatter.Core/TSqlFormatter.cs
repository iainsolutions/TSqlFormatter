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

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using TSqlFormatter.Interfaces;
using TSqlFormatter.Core;

namespace TSqlFormatter
{
    /// <summary>
    /// Main T-SQL formatter implementation with thread safety and comprehensive error handling
    /// </summary>
    public class TSqlFormatter : ITSqlFormatter, IDisposable
    {
        private readonly ILogger<TSqlFormatter> _logger;
        private readonly ITokenizer _tokenizer;
        private readonly IParser _parser;
        private readonly ITreeFormatter _treeFormatter;
        private readonly SemaphoreSlim _semaphore;
        private bool _disposed;

        public TSqlFormatter() : this(NullLogger<TSqlFormatter>.Instance)
        {
        }

        public TSqlFormatter(ILogger<TSqlFormatter> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tokenizer = new TSqlStandardTokenizer();
            _parser = new TSqlStandardParser();
            _treeFormatter = new TSqlStandardFormatter();
            _semaphore = new SemaphoreSlim(Environment.ProcessorCount * 2);
        }

        public TSqlFormatter(
            ILogger<TSqlFormatter> logger,
            ITokenizer tokenizer,
            IParser parser,
            ITreeFormatter treeFormatter)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer));
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));
            _treeFormatter = treeFormatter ?? throw new ArgumentNullException(nameof(treeFormatter));
            _semaphore = new SemaphoreSlim(Environment.ProcessorCount * 2);
        }

        public FormattingResult Format(string sql, FormattingOptions? options = null)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(TSqlFormatter));

            if (string.IsNullOrWhiteSpace(sql))
            {
                _logger.LogDebug("Empty SQL provided for formatting");
                return new FormattingResult
                {
                    FormattedSql = sql ?? string.Empty,
                    Success = true
                };
            }

            options ??= FormattingOptions.Default;
            var stopwatch = Stopwatch.StartNew();

            try
            {
                _logger.LogDebug("Starting SQL formatting. Input length: {Length}", sql.Length);

                // Tokenize
                var tokens = _tokenizer.Tokenize(sql, options);
                _logger.LogDebug("Tokenization complete. Token count: {Count}", tokens.Count);

                // Parse
                var parseTree = _parser.Parse(tokens, options);
                _logger.LogDebug("Parsing complete. Statement count: {Count}", parseTree.StatementCount);

                // Check for errors
                if (parseTree.HasErrors)
                {
                    _logger.LogWarning("Parse errors detected during formatting");
                    return new FormattingResult
                    {
                        FormattedSql = sql,
                        Success = false,
                        ErrorMessage = parseTree.FirstError?.Message,
                        ErrorLine = parseTree.FirstError?.Line,
                        ErrorColumn = parseTree.FirstError?.Column
                    };
                }

                // Format
                var formatted = _treeFormatter.Format(parseTree, options);
                stopwatch.Stop();

                _logger.LogInformation("Formatting completed successfully in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);

                return new FormattingResult
                {
                    FormattedSql = formatted,
                    Success = true,
                    Statistics = new FormattingStatistics
                    {
                        TokenCount = tokens.Count,
                        StatementCount = parseTree.StatementCount,
                        ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                        LinesFormatted = CountLines(formatted)
                    }
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error during SQL formatting");

                return new FormattingResult
                {
                    FormattedSql = sql,
                    Success = false,
                    ErrorMessage = $"Formatting failed: {ex.Message}",
                    Statistics = new FormattingStatistics
                    {
                        ElapsedMilliseconds = stopwatch.ElapsedMilliseconds
                    }
                };
            }
        }

        public async Task<FormattingResult> FormatAsync(
            string sql,
            FormattingOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(TSqlFormatter));

            await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                return await Task.Run(() => Format(sql, options), cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public ValidationResult Validate(string sql)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(TSqlFormatter));

            if (string.IsNullOrWhiteSpace(sql))
            {
                return new ValidationResult { IsValid = true, Errors = Array.Empty<ValidationError>() };
            }

            try
            {
                _logger.LogDebug("Starting SQL validation");

                var tokens = _tokenizer.Tokenize(sql, FormattingOptions.Default);
                var parseTree = _parser.Parse(tokens, FormattingOptions.Default);

                if (parseTree.HasErrors)
                {
                    var errors = parseTree.Errors.Select(e => new ValidationError
                    {
                        Message = e.Message,
                        Line = e.Line,
                        Column = e.Column,
                        Severity = MapSeverity(e.Severity)
                    }).ToArray();

                    _logger.LogInformation("Validation found {Count} errors", errors.Length);

                    return new ValidationResult
                    {
                        IsValid = false,
                        Errors = errors
                    };
                }

                _logger.LogDebug("Validation completed successfully");
                return new ValidationResult { IsValid = true, Errors = Array.Empty<ValidationError>() };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during SQL validation");

                return new ValidationResult
                {
                    IsValid = false,
                    Errors = new[]
                    {
                        new ValidationError
                        {
                            Message = $"Validation failed: {ex.Message}",
                            Line = 0,
                            Column = 0,
                            Severity = ValidationErrorSeverity.Error
                        }
                    }
                };
            }
        }

        public string Obfuscate(string sql)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(TSqlFormatter));

            if (string.IsNullOrWhiteSpace(sql))
                return sql ?? string.Empty;

            try
            {
                _logger.LogDebug("Starting SQL obfuscation");

                var options = new FormattingOptions { ObfuscateMode = true };
                var tokens = _tokenizer.Tokenize(sql, options);
                var parseTree = _parser.Parse(tokens, options);

                var obfuscator = new TSqlObfuscator();
                var result = obfuscator.Obfuscate(parseTree);

                _logger.LogDebug("Obfuscation completed successfully");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during SQL obfuscation");
                return sql; // Return original if obfuscation fails
            }
        }

        private static int CountLines(string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            int count = 1;
            int index = 0;
            while ((index = text.IndexOf('\n', index)) != -1)
            {
                count++;
                index++;
            }
            return count;
        }

        private static ValidationErrorSeverity MapSeverity(ParseErrorSeverity severity)
        {
            return severity switch
            {
                ParseErrorSeverity.Info => ValidationErrorSeverity.Info,
                ParseErrorSeverity.Warning => ValidationErrorSeverity.Warning,
                ParseErrorSeverity.Error => ValidationErrorSeverity.Error,
                _ => ValidationErrorSeverity.Error
            };
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _semaphore?.Dispose();
                    (_tokenizer as IDisposable)?.Dispose();
                    (_parser as IDisposable)?.Dispose();
                    (_treeFormatter as IDisposable)?.Dispose();
                }
                _disposed = true;
            }
        }
    }
}
