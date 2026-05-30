using System;
using System.Collections.Generic;
using System.Text;

namespace SolidShineUi.Utils
{
    /// <summary>
    /// A helper class for parsing an array of strings into a CSV formatted line of text, or parsing a CSV line back into an array of strings.
    /// </summary>
    public static class CsvLineHelper
    {
        // adapted from https://stackoverflow.com/a/23369687/2987285

#if NETCOREAPP

        /// <summary>
        /// Converts an array of strings into a single string with each value separated by a comma or other delimiter (and using quotes where needed to escape the delimiter).
        /// </summary>
        /// <param name="values">the array of strings to convert</param>
        /// <param name="delimiter">the delimiter character to use to separate values; default is a comma (<c>,</c>)</param>
        public static string ArrayToCsvLine(ReadOnlySpan<string> values, char delimiter = ',')
        {
            if (values.Length == 0) return "";

            var sb = new StringBuilder();
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] != null)
                {
                    bool mustBeQuoted = values[i].Contains("\"") || values[i].Contains(delimiter);
                    if (mustBeQuoted)
                    {
                        sb.Append('"');
                        sb.Append(values[i].Replace("\"", "\"\""));
                        sb.Append('"');
                    }
                    else
                    {
                        sb.Append(values[i]);
                    }
                }

                if (i != values.Length - 1)
                    sb.Append(delimiter);
            }

            return sb.ToString();
        }

#endif


        /// <summary>
        /// Converts an array of strings into a single string with each value separated by a comma or other delimiter (and using quotes where needed to escape the delimiter).
        /// </summary>
        /// <param name="values">the array of strings to convert</param>
        /// <param name="delimiter">the delimiter character to use to separate values; default is a comma (<c>,</c>)</param>
#if NETCOREAPP
        public static string ArrayToCsvLine(string[]? values, char delimiter = ',')
        {
            if (values == null || values.Length == 0) return "";
#else
        public static string ArrayToCsvLine(string[] values, char delimiter = ',')
        {
            if (values == null || values.Length == 0) return "";
#endif

            var sb = new StringBuilder();
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] != null)
                {
                    bool mustBeQuoted = values[i].Contains("\"") || values[i].Contains(delimiter.ToString());
                    if (mustBeQuoted)
                    {
                        sb.Append('"');
                        sb.Append(values[i].Replace("\"", "\"\""));
                        sb.Append('"');
                    }
                    else
                    {
                        sb.Append(values[i]);
                    }
                }

                if (i != values.Length - 1)
                    sb.Append(delimiter);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Parse a line of text in CSV format to convert to an array of strings, using a comma (<c>,</c>) as the delimiter character.
        /// </summary>
        /// <param name="line">the line of values to parse/convert</param>
        /// <param name="fields">the output result of the parsing. If the parsing fails, this is an empty array.</param>
        /// <returns>
        /// Returns <c>true</c> if parsing was successful, <c>false</c> otherwise.
        /// If this returns <c>false</c>, then <paramref name="fields"/> will be an empty array.
        /// </returns>
#if NETCOREAPP
        public static bool CsvLineToArray(string? line, out string[] fields)
#else
        public static bool CsvLineToArray(string line, out string[] fields)
#endif
        {
            return CsvLineToArray(line, ',', out fields);
        }

        /// <summary>
        /// Parse a line of text in CSV format to convert to an array of strings, with the ability to set the delimiter character.
        /// </summary>
        /// <param name="line">the line of values to parse/convert</param>
        /// <param name="delimiter">the delimiter character used to separate values</param>
        /// <param name="fields">the output result of the parsing. If the parsing fails, this is an empty array.</param>
        /// <returns>
        /// Returns <c>true</c> if parsing was successful, <c>false</c> otherwise.
        /// If this returns <c>false</c>, then <paramref name="fields"/> will be an empty array.
        /// </returns>
#if NETCOREAPP
        public static bool CsvLineToArray(string? line, char delimiter, out string[] fields)
#else
        public static bool CsvLineToArray(string line, char delimiter, out string[] fields)
#endif
        {
            fields = Array.Empty<string>();

            if (string.IsNullOrEmpty(line)) return false;

            int index = 0;
            var res = new List<string>();
            while (index != line.Length)
            {
                if (ReadField(line, delimiter, ref index, out string field))
                {
                    res.Add(field);
                }
                else
                {
                    return false;
                }
            }

            if (line[line.Length - 1] == delimiter)
            {
                res.Add(string.Empty);
            }

            fields = res.ToArray();
            return true;
        }

        private static bool ReadField(string line, char delimiter, ref int index, out string field)
        {
            field = "";

            if (index >= line.Length) return false;

            var sb = new StringBuilder();
            int state = 0;
            while (true)
            {
                char c = line[index];
                char? c1 = (index + 1 < line.Length - 1) ? (char?)line[index + 1] : null;
                index++;
                switch (state)
                {
                    case 0: // START
                        if (c == '"') //value start with a quotation mark, so text of value is in quotes
                        {
                            state = 4;
                        }
                        else if (c == delimiter) // empty value (no text)
                        {
                            field = sb.ToString();
                            return true;
                        }
                        else
                        {
                            state = 1;
                            sb.Append(c);
                        }
                        break;
                    case 1: // value not in quotes
                        if (c == '"') // error, cannot contain " in the middle of the field
                        {
                            return false;
                        }
                        else if (c == delimiter)
                        {
                            field = sb.ToString();
                            return true;
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                    case 3: //Escaping quotation mark
                        if (c == '"') //previous quotation mark was escape char for this quotation mark
                        {
                            state = 4;
                            sb.Append(c);
                        }
                        else //error, cannot be any other char
                        {
                            return false;
                        }
                        break;
                    case 4: // Value text in between quotation marks
                        if (c == '"') //closing quoted text or escape char for following qoatation mark - based on which char is following
                        {
                            if (c1 != null && c1.Value == '"') //current quotation mark is escape char for following quotation mark
                            {
                                state = 3;
                            }
                            else
                            {
                                state = 5;
                            }
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                    case 5: //Just after closing quotation mark of quoted text
                        if (c == delimiter) //closing quoted text
                        {
                            field = sb.ToString();
                            return true;
                        }
                        else //error, cannot contain any other char
                        {
                            return false;
                        }
                }

                if (index == line.Length) // we've reached the end of the line
                {
                    if (state == 1 || state == 5)
                    {
                        field = sb.ToString();
                        return true;
                    }

                    return false;
                }
            }
        }
    }
}
