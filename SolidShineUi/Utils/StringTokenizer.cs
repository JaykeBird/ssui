using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SolidShineUi.Utils
{
    // taken from Avalonia's SpanStringTokenizer and StringTokenizer
    // https://github.com/AvaloniaUI/Avalonia/blob/11.3.0/src/Avalonia.Base/Utilities/StringTokenizer.cs
    // https://github.com/AvaloniaUI/Avalonia/blob/main/src/Avalonia.Base/Utilities/SpanStringTokenizer.cs

    /// <summary>
    /// A helper class for tokenizing a string into substrings (or other data types) based on a separator character.
    /// <para/>
    /// This is based on the SpanStringTokenizer used internally in Avalonia.
    /// </summary>
    /// <remarks>
    /// The .NET Framework version of this control is actually based on Avalonia's older StringTokenizer that doese not use spans, with additional
    /// tweaks to remove some additional features or items that .NET Framework doesn't have.
    /// The .NET Core version fully utilizes spans, and so has less string allocations and uses less memory.
    /// </remarks>
#if NETCOREAPP
    public ref struct StringTokenizer
    {
        private const char DefaultSeparatorChar = ',';

        private readonly ReadOnlySpan<char> _s;
        private readonly int _length;
        private readonly char _separator;
        private readonly string? _exceptionMessage;
        private readonly IFormatProvider _formatProvider;
        private int _index;
        private int _tokenIndex;
        private int _tokenLength;
        
        /// <summary>
        /// Create a StringTokenizer from a string.
        /// </summary>
        /// <param name="s">the string to build tokens from</param>
        /// <param name="formatProvider">the format provider to use for parsing numbers and values from the string</param>
        /// <param name="exceptionMessage">the custom exception message to use if an exception occurs while tokenizing this string</param>
        public StringTokenizer(string s, IFormatProvider formatProvider, string? exceptionMessage = null)
            : this(s.AsSpan(), GetSeparatorFromFormatProvider(formatProvider), exceptionMessage)
        {
            _formatProvider = formatProvider;
        }
        
        /// <summary>
        /// Create a StringTokenizer from a string.
        /// </summary>
        /// <param name="s">the string to build tokens from</param>
        /// <param name="separator">the separator character to use for separating tokens in this string; by default, this is a comma ("<c>,</c>")</param>
        /// <param name="exceptionMessage">the custom exception message to use if an exception occurs while tokenizing this string</param>
        public StringTokenizer(string s, char separator = DefaultSeparatorChar, string? exceptionMessage = null)
            : this(s.AsSpan(), separator, exceptionMessage)
        {
        }

        /// <summary>
        /// Create a StringTokenizer from a span.
        /// </summary>
        /// <param name="s">the span to build tokens from</param>
        /// <param name="formatProvider">the format provider to use for parsing numbers and values from the string</param>
        /// <param name="exceptionMessage">the custom exception message to use if an exception occurs while tokenizing this string</param>
        public StringTokenizer(ReadOnlySpan<char> s, IFormatProvider formatProvider, string? exceptionMessage = null)
            : this(s, GetSeparatorFromFormatProvider(formatProvider), exceptionMessage)
        {
            _formatProvider = formatProvider;
        }

        /// <summary>
        /// Create a StringTokenizer from a span.
        /// </summary>
        /// <param name="s">the span to build tokens from</param>
        /// <param name="separator">the separator character to use for separating tokens in this string; by default, this is a comma ("<c>,</c>")</param>
        /// <param name="exceptionMessage">the custom exception message to use if an exception occurs while tokenizing this string</param>
        public StringTokenizer(ReadOnlySpan<char> s, char separator = DefaultSeparatorChar, string? exceptionMessage = null)
        {
            _s = s;
            _length = s.Length;
            _separator = separator;
            _exceptionMessage = exceptionMessage;
            _formatProvider = CultureInfo.InvariantCulture;
            _index = 0;
            _tokenIndex = -1;
            _tokenLength = 0;

            while (_index < _length && char.IsWhiteSpace(_s[_index]))
            {
                _index++;
            }
        }

        /// <summary>
        /// Get the index of the currently read token.
        /// </summary>
        public readonly int CurrentTokenIndex => _tokenIndex;
        
        /// <summary>
        /// The text of the token currently being read, allocated to a string.
        /// </summary>
        public readonly string? CurrentToken => _tokenIndex < 0 ? null : _s.Slice(_tokenIndex, _tokenLength).ToString();
        
        /// <summary>
        /// The text of the token currently being read.
        /// </summary>
        public readonly ReadOnlySpan<char> CurrentTokenSpan => _tokenIndex < 0 ? ReadOnlySpan<char>.Empty : _s.Slice(_tokenIndex, _tokenLength);
        
        /// <summary>
        /// Get if there are any tokens left in this string that haven't yet been parsed and tokenized.
        /// </summary>
        public readonly bool HasTokensLeft()
        {
            // the full string wasn't parsed
            return _index != _length;
        }
        
        /// <summary>
        /// Try to read the current token as an integer.
        /// <para/>
        /// This will read the current token and then position the tokenizer to read the next token.
        /// </summary>
        /// <param name="result">the result of parsing the current token as an integer; if the function returns <c>false</c>, this will return the default int value</param>
        /// <param name="separator">the separator character at the end of this token, if different from the standard separator character</param>
        /// <returns><c>true</c> if this token was an integer; <c>false</c> otherwise</returns>
        public bool TryReadInt32(out int result, char? separator = null)
        {
            if (TryReadSpan(out var stringResult, separator) &&
                int.TryParse(stringResult, NumberStyles.Integer, _formatProvider, out result))
            {
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }
        
        /// <summary>
        /// Read the current token as an integer, or throw an exception if it is not an integer.
        /// <para/>
        /// This will read the current token and then position the tokenizer to read the next token.
        /// </summary>
        /// <param name="separator">the separator character at the end of this token, if different from the standard separator character</param>
        /// <exception cref="FormatException">thrown if the current token couldn't be read as an integer</exception>
        public int ReadInt32(char? separator = null)
        {
            if (!TryReadInt32(out var result, separator))
            {
                throw GetFormatException();
            }

            return result;
        }
        
        /// <summary>
        /// Try to read the current token as a double.
        /// <para/>
        /// This will read the current token and then position the tokenizer to read the next token.
        /// </summary>
        /// <param name="result">the result of parsing the current token as an double; if the function returns <c>false</c>, this will return the default double value</param>
        /// <param name="separator">the separator character at the end of this token, if different from the standard separator character</param>
        /// <returns><c>true</c> if this token was an double; <c>false</c> otherwise</returns>
        public bool TryReadDouble(out double result, char? separator = null)
        {
            if (TryReadSpan(out var stringResult, separator) &&
                double.TryParse(stringResult, NumberStyles.Float, _formatProvider, out result))
            {
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }
        
        /// <summary>
        /// Read the current token as an double, or throw an exception if it is not an double.
        /// <para/>
        /// This will read the current token and then position the tokenizer to read the next token.
        /// </summary>
        /// <param name="separator">the separator character at the end of this token, if different from the standard separator character</param>
        /// <exception cref="FormatException">thrown if the current token couldn't be read as an double</exception>
        public double ReadDouble(char? separator = null)
        {
            if (!TryReadDouble(out var result, separator))
            {
                throw GetFormatException();
            }

            return result;
        }
        
        /// <summary>
        /// Try to read the current token as a string.
        /// <para/>
        /// This will read the current token and then position the tokenizer to read the next token.
        /// </summary>
        /// <param name="result">the result of reading the token as a string; if the function returns <c>false</c>, an empty string is returned</param>
        /// <param name="separator">the separator character at the end of this token, if different from the standard separator character</param>
        /// <returns><c>true</c> if this token had a non-empty value; <c>false</c> otherwise</returns>
        /// <remarks>
        /// The string returned by this will have the whitespace at the beginning and end trimmed from the original token string.
        /// </remarks>
        public bool TryReadString([NotNull] out string result, char? separator = null)
        {
            bool success = TryReadToken(separator ?? _separator);
            result = CurrentTokenSpan.ToString();
            return success;
        }
        
        /// <summary>
        /// Read the current token as a string.
        /// <para/>
        /// This will read the current token and then position the tokenizer to read the next token.
        /// </summary>
        /// <param name="separator">the separator character at the end of this token, if different from the standard separator character</param>
        /// <exception cref="FormatException">thrown if the current token didn't have a non-empty value</exception>
        /// <remarks>
        /// The string returned by this will have the whitespace at the beginning and end trimmed from the original token string.
        /// </remarks>
        public string ReadString(char? separator = null)
        {
            if (!TryReadString(out var result, separator))
            {
                throw GetFormatException();
            }

            return result;
        }
        
        /// <summary>
        /// Try to read the current token as a span.
        /// <para/>
        /// This will read the current token and then position the tokenizer to read the next token.
        /// </summary>
        /// <param name="result">the result of reading the token as a span; if the function returns <c>false</c>, an empty span is returned</param>
        /// <param name="separator">the separator character at the end of this token, if different from the standard separator character</param>
        /// <returns><c>true</c> if this token had a non-empty value; <c>false</c> otherwise</returns>
        /// <remarks>
        /// The span returned by this will have the whitespace at the beginning and end trimmed from the original token string.
        /// </remarks>
        public bool TryReadSpan(out ReadOnlySpan<char> result, char? separator = null)
        {
            bool success = TryReadToken(separator ?? _separator);
            result = CurrentTokenSpan;
            return success;
        }
        
        /// <summary>
        /// Read the current token as a span.
        /// <para/>
        /// This will read the current token and then position the tokenizer to read the next token.
        /// </summary>
        /// <param name="separator">the separator character at the end of this token, if different from the standard separator character</param>
        /// <exception cref="FormatException">thrown if the current token didn't have a non-empty value</exception>
        /// <remarks>
        /// The span returned by this will have the whitespace at the beginning and end trimmed from the original token string.
        /// </remarks>
        public ReadOnlySpan<char> ReadSpan(char? separator = null)
        {
            if (!TryReadSpan(out var result, separator))
            {
                throw GetFormatException();
            }

            return result;
        }
        
        /// <summary>
        /// Try to read the next available token, and position yourself for the following token.
        /// </summary>
        /// <param name="separator">the separator character marking the end of the token</param>
        /// <returns><c>true</c> if there was text to read; <c>false</c> if there was not</returns>
        /// <exception cref="FormatException">
        /// thrown if this token has weird or invalid formatting that puts the index in an invalid spot
        /// </exception>
        private bool TryReadToken(char separator)
        {
            _tokenIndex = -1;

            if (_index >= _length)
            {
                return false;
            }

            char c;

            int index = _index;
            int length = 0;

            while (_index < _length)
            {
                c = _s[_index];

                if (char.IsWhiteSpace(c) || c == separator)
                {
                    break;
                }

                _index++;
                length++;
            }

            SkipToNextToken(separator);

            _tokenIndex = index;
            _tokenLength = length;

            if (_tokenLength < 1)
            {
                throw GetFormatException();
            }

            return true;
        }
        
        /// <summary>
        /// Skip to the beginning of the next token in this string.
        /// This should be called at the end of reading the current token, to prepare for reading the next token.
        /// </summary>
        /// <param name="separator">the separator character marking the end of the token</param>
        /// <exception cref="FormatException">
        /// thrown if this is called while the current index appears to be in the middle of a token,
        /// or the token contains data that puts the index in an invalid spot beyond the end of the string
        /// </exception>
        private void SkipToNextToken(char separator)
        {
            if (_index < _length)
            {
                var c = _s[_index];

                if (c != separator && !char.IsWhiteSpace(c))
                {
                    throw GetFormatException();
                }

                var length = 0;

                while (_index < _length)
                {
                    c = _s[_index];

                    if (c == separator)
                    {
                        length++;
                        _index++;

                        if (length > 1)
                        {
                            throw GetFormatException();
                        }
                    }
                    else
                    {
                        if (!char.IsWhiteSpace(c))
                        {
                            break;
                        }

                        _index++;
                    }
                }

                if (length > 0 && _index >= _length)
                {
                    throw GetFormatException();
                }
            }
        }

        private readonly FormatException GetFormatException() =>
            _exceptionMessage != null ? new FormatException(_exceptionMessage) : new FormatException();

        private static char GetSeparatorFromFormatProvider(IFormatProvider provider)
        {
            var c = DefaultSeparatorChar;

            var formatInfo = NumberFormatInfo.GetInstance(provider);
            if (formatInfo.NumberDecimalSeparator.Length > 0 && c == formatInfo.NumberDecimalSeparator[0])
            {
                c = ';';
            }

            return c;
        }
    }
#else
    public struct StringTokenizer
    {
        private const char DefaultSeparatorChar = ',';

        private readonly string _s;
        private readonly int _length;
        private readonly char _separator;
        private readonly string _exceptionMessage;
        private readonly IFormatProvider _formatProvider;
        private int _index;
        private int _tokenIndex;
        private int _tokenLength;

        /// <summary>
        /// Create a StringTokenizer from a string.
        /// </summary>
        /// <param name="s">the string to build tokens from</param>
        /// <param name="formatProvider">the format provider to use for parsing numbers and values from the string</param>
        /// <param name="exceptionMessage">the custom exception message to use if an exception occurs while tokenizing this string</param>
        public StringTokenizer(string s, IFormatProvider formatProvider, string exceptionMessage = null)
            : this(s, GetSeparatorFromFormatProvider(formatProvider), exceptionMessage)
        {
            _formatProvider = formatProvider;
        }

        /// <summary>
        /// Create a StringTokenizer from a string.
        /// </summary>
        /// <param name="s">the string to build tokens from</param>
        /// <param name="separator">the separator character to use for separating tokens in this string; by default, this is a comma ("<c>,</c>")</param>
        /// <param name="exceptionMessage">the custom exception message to use if an exception occurs while tokenizing this string</param>
        public StringTokenizer(string s, char separator = DefaultSeparatorChar, string exceptionMessage = null)
        {
            _s = s ?? throw new ArgumentNullException(nameof(s));
            _length = s?.Length ?? 0;
            _separator = separator;
            _exceptionMessage = exceptionMessage ?? "The inputted string could not be parsed as requested.";
            _formatProvider = CultureInfo.InvariantCulture;
            _index = 0;
            _tokenIndex = -1;
            _tokenLength = 0;

            while (_index < _length && char.IsWhiteSpace(_s, _index))
            {
                _index++;
            }
        }

        /// <summary>
        /// The text of the token currently being read.
        /// </summary>
        public string CurrentToken => _tokenIndex < 0 ? null : _s.Substring(_tokenIndex, _tokenLength);

        //public ReadOnlySpan<char> CurrentTokenSpan => _tokenIndex < 0 ? ReadOnlySpan<char>.Empty : _s.AsSpan().Slice(_tokenIndex, _tokenLength);

        /// <summary>
        /// Get if there are any tokens left in this string that haven't yet been parsed and tokenized.
        /// </summary>
        public bool HasTokensLeft()
        {
            // the full string wasn't parsed
            return _index != _length;
        }

        /// <summary>
        /// Try to read the current token as an integer.
        /// <para/>
        /// This will read the current token and then position the tokenizer to read the next token.
        /// </summary>
        /// <param name="result">the result of parsing the current token as an integer; if the function returns <c>false</c>, this will return the default int value</param>
        /// <param name="separator">the separator character at the end of this token, if different from the standard separator character</param>
        /// <returns><c>true</c> if this token was an integer; <c>false</c> otherwise</returns>
        public bool TryReadInt32(out int result, char? separator = null)
        {
            if (TryReadString(out var stringResult, separator) &&
                int.TryParse(stringResult, NumberStyles.Integer, _formatProvider, out result))
            {
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }

        /// <summary>
        /// Read the current token as an integer, or throw an exception if it is not an integer.
        /// <para/>
        /// This will read the current token and then position the tokenizer to read the next token.
        /// </summary>
        /// <param name="separator">the separator character at the end of this token, if different from the standard separator character</param>
        /// <exception cref="FormatException">thrown if the current token couldn't be read as an integer</exception>
        public int ReadInt32(char? separator = null)
        {
            if (!TryReadInt32(out var result, separator))
            {
                throw GetFormatException();
            }

            return result;
        }

        /// <summary>
        /// Try to read the current token as a double.
        /// <para/>
        /// This will read the current token and then position the tokenizer to read the next token.
        /// </summary>
        /// <param name="result">the result of parsing the current token as an double; if the function returns <c>false</c>, this will return the default double value</param>
        /// <param name="separator">the separator character at the end of this token, if different from the standard separator character</param>
        /// <returns><c>true</c> if this token was an double; <c>false</c> otherwise</returns>
        public bool TryReadDouble(out double result, char? separator = null)
        {
            if (TryReadString(out var stringResult, separator) &&
                double.TryParse(stringResult, NumberStyles.Float, _formatProvider, out result))
            {
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }

        /// <summary>
        /// Read the current token as an double, or throw an exception if it is not an double.
        /// <para/>
        /// This will read the current token and then position the tokenizer to read the next token.
        /// </summary>
        /// <param name="separator">the separator character at the end of this token, if different from the standard separator character</param>
        /// <exception cref="FormatException">thrown if the current token couldn't be read as an double</exception>
        public double ReadDouble(char? separator = null)
        {
            if (!TryReadDouble(out var result, separator))
            {
                throw GetFormatException();
            }

            return result;
        }

        /// <summary>
        /// Try to read the current token as a string.
        /// <para/>
        /// This will read the current token and then position the tokenizer to read the next token.
        /// </summary>
        /// <param name="result">the result of reading the token as a string; if the function returns <c>false</c>, an empty string is returned</param>
        /// <param name="separator">the separator character at the end of this token, if different from the standard separator character</param>
        /// <returns><c>true</c> if this token had a non-empty value; <c>false</c> otherwise</returns>
        /// <remarks>
        /// The string returned by this will have the whitespace at the beginning and end trimmed from the original token string.
        /// </remarks>
        public bool TryReadString(out string result, char? separator = null)
        {
            bool success = TryReadToken(separator ?? _separator);
            result = CurrentToken.ToString();
            return success;
        }

        /// <summary>
        /// Read the current token as a string.
        /// <para/>
        /// This will read the current token and then position the tokenizer to read the next token.
        /// </summary>
        /// <param name="separator">the separator character at the end of this token, if different from the standard separator character</param>
        /// <exception cref="FormatException">thrown if the current token didn't have a non-empty value</exception>
        /// <remarks>
        /// The string returned by this will have the whitespace at the beginning and end trimmed from the original token string.
        /// </remarks>
        public string ReadString(char? separator = null)
        {
            if (!TryReadString(out var result, separator))
            {
                throw GetFormatException();
            }

            return result;
        }

        /// <summary>
        /// Try to read the next available token, and position yourself for the following token.
        /// </summary>
        /// <param name="separator">the separator character marking the end of the token</param>
        /// <returns><c>true</c> if there was text to read; <c>false</c> if there was not</returns>
        /// <exception cref="FormatException">
        /// thrown if this token has weird or invalid formatting that puts the index in an invalid spot
        /// </exception>
        private bool TryReadToken(char separator)
        {
            _tokenIndex = -1;

            if (_index >= _length) // if the current index is at or beyond the end of the string
            {
                return false;
            }

            char c;

            int index = _index;
            int length = 0;

            while (_index < _length)
            {
                c = _s[_index];

                if (char.IsWhiteSpace(c) || c == separator)
                {
                    break;
                }

                _index++;
                length++;
            }

            SkipToNextToken(separator);

            _tokenIndex = index;
            _tokenLength = length;

            if (_tokenLength < 1)
            {
                throw GetFormatException();
            }

            return true;
        }

        /// <summary>
        /// Skip to the beginning of the next token in this string.
        /// This should be called at the end of reading the current token, to prepare for reading the next token.
        /// </summary>
        /// <param name="separator">the separator character marking the end of the token</param>
        /// <exception cref="FormatException">
        /// thrown if this is called while the current index appears to be in the middle of a token,
        /// or the token contains data that puts the index in an invalid spot beyond the end of the string
        /// </exception>
        private void SkipToNextToken(char separator)
        {
            if (_index < _length) // if the current index is before the end of the string
            {
                var c = _s[_index];

                if (c != separator && !char.IsWhiteSpace(c))
                {
                    throw GetFormatException();
                }

                var length = 0;

                while (_index < _length)
                {
                    c = _s[_index];

                    if (c == separator)
                    {
                        length++;
                        _index++;

                        if (length > 1)
                        {
                            throw GetFormatException();
                        }
                    }
                    else
                    {
                        if (!char.IsWhiteSpace(c))
                        {
                            break;
                        }

                        _index++;
                    }
                }

                if (length > 0 && _index >= _length) // we're at the end of the string, and we're not at the end of this current token
                {
                    throw GetFormatException();
                }
            }
        }

        private FormatException GetFormatException() =>
            _exceptionMessage != null ? new FormatException(_exceptionMessage) : new FormatException();

        private static char GetSeparatorFromFormatProvider(IFormatProvider provider)
        {
            var c = DefaultSeparatorChar;

            var formatInfo = NumberFormatInfo.GetInstance(provider);
            if (formatInfo.NumberDecimalSeparator.Length > 0 && c == formatInfo.NumberDecimalSeparator[0])
            {
                c = ';';
            }

            return c;
        }
    }
#endif
}
