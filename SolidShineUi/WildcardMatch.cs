// written by H.A. Sullivan
// source: https://bitbucket.org/hasullivan/fast-wildcard-matching/

using System.Collections.Generic;

namespace SolidShineUi
{
    /// <summary>
    /// A class that can match a string against a given target string with wildcards (i.e. "d*g" can match "dig" or "dog" or "ding"). 
    /// </summary>
    public static class WildcardMatch
    {
        /// <summary>
        /// Match a string against a given target string with wildcards ("*" and "?" are supported). See Remarks for more details.
        /// </summary>
        /// <param name="text">The string to test.</param>
        /// <param name="wildcardString">The string to match against, containing the wildcards.</param>
        /// <returns>True if it can be matched; false if they cannot be.</returns>
        /// <remarks>
        /// This function can match a string against a wildcard string. This is used, for example, in Windows's file dialogs for opening or saving files.
        /// Supported wildcard characters are "*", which matches any number of characters, and "?", which matches just one character.
        /// (For example, "d*g" will match both "dig" and "ding", where as "d?g" will only match "dig" and not "ding".)
        /// Regex (regular expressions) is a lot more powerful than what wildcards alone can provide, but regex strings can also be a lot more complicated to decipher.
        /// <para/>
        /// This class was written by H.A. Sullivan.
        /// </remarks>
        public static bool MatchesWildcard(string text, string wildcardString)
        {
            bool isLike = true;
            byte matchCase = 0;
            char[] filter;
            char[] reversedFilter;
            char[] reversedWord;
            char[] word;
            int currentPatternStartIndex = 0;
            int lastCheckedHeadIndex = 0;
            int lastCheckedTailIndex = 0;
            int reversedWordIndex = 0;
            List<char[]> reversedPatterns = new List<char[]>();

            if (text == null || wildcardString == null)
            {
                return false;
            }

            word = text.ToCharArray();
            filter = wildcardString.ToCharArray();

            //Set which case will be used (0 = no wildcards, 1 = only ?, 2 = only *, 3 = both ? and *
            for (int i = 0; i < filter.Length; i++)
            {
                if (filter[i] == '?')
                {
                    matchCase += 1;
                    break;
                }
            }

            for (int i = 0; i < filter.Length; i++)
            {
                if (filter[i] == '*')
                {
                    matchCase += 2;
                    break;
                }
            }

            if ((matchCase == 0 || matchCase == 1) && word.Length != filter.Length)
            {
                return false;
            }

            switch (matchCase)
            {
                case 0:
                    isLike = text == wildcardString;
                    break;

                case 1:
                    for (int i = 0; i < text.Length; i++)
                    {
                        if ((word[i] != filter[i]) && filter[i] != '?')
                        {
                            isLike = false;
                        }
                    }
                    break;

                case 2:
                    //Search for matches until first *
                    for (int i = 0; i < filter.Length; i++)
                    {
                        if (filter[i] != '*')
                        {
                            if (filter[i] != word[i])
                            {
                                return false;
                            }
                        }
                        else
                        {
                            lastCheckedHeadIndex = i;
                            break;
                        }
                    }
                    //Search Tail for matches until first *
                    for (int i = 0; i < filter.Length; i++)
                    {
                        if (filter[filter.Length - 1 - i] != '*')
                        {
                            if (filter[filter.Length - 1 - i] != word[word.Length - 1 - i])
                            {
                                return false;
                            }

                        }
                        else
                        {
                            lastCheckedTailIndex = i;
                            break;
                        }
                    }


                    //Create a reverse word and filter for searching in reverse. The reversed word and filter do not include already checked chars
                    reversedWord = new char[word.Length - lastCheckedHeadIndex - lastCheckedTailIndex];
                    reversedFilter = new char[filter.Length - lastCheckedHeadIndex - lastCheckedTailIndex];

                    for (int i = 0; i < reversedWord.Length; i++)
                    {
                        reversedWord[i] = word[word.Length - (i + 1) - lastCheckedTailIndex];
                    }
                    for (int i = 0; i < reversedFilter.Length; i++)
                    {
                        reversedFilter[i] = filter[filter.Length - (i + 1) - lastCheckedTailIndex];
                    }

                    //Cut up the filter into seperate patterns, exclude * as they are not longer needed
                    for (int i = 0; i < reversedFilter.Length; i++)
                    {
                        if (reversedFilter[i] == '*')
                        {
                            if (i - currentPatternStartIndex > 0)
                            {
                                char[] pattern = new char[i - currentPatternStartIndex];
                                for (int j = 0; j < pattern.Length; j++)
                                {
                                    pattern[j] = reversedFilter[currentPatternStartIndex + j];
                                }
                                reversedPatterns.Add(pattern);
                            }
                            currentPatternStartIndex = i + 1;
                        }
                    }

                    //Search for the patterns
                    for (int i = 0; i < reversedPatterns.Count; i++)
                    {
                        for (int j = 0; j < reversedPatterns[i].Length; j++)
                        {

                            if ((reversedPatterns[i].Length - 1 - j) > (reversedWord.Length - 1 - reversedWordIndex))
                            {
                                return false;
                            }

                            if (reversedPatterns[i][j] != reversedWord[reversedWordIndex + j])
                            {
                                reversedWordIndex += 1;
                                j = -1;
                            }
                            else
                            {
                                if (j == reversedPatterns[i].Length - 1)
                                {
                                    reversedWordIndex += reversedPatterns[i].Length;
                                }
                            }
                        }
                    }
                    break;

                case 3:
                    //Same as Case 2 except ? is considered a match
                    //Search Head for matches util first *
                    for (int i = 0; i < filter.Length; i++)
                    {
                        if (filter[i] != '*')
                        {
                            if (filter[i] != word[i] && filter[i] != '?')
                            {
                                return false;
                            }
                        }
                        else
                        {
                            lastCheckedHeadIndex = i;
                            break;
                        }
                    }
                    //Search Tail for matches until first *
                    for (int i = 0; i < filter.Length; i++)
                    {
                        if (filter[filter.Length - 1 - i] != '*')
                        {
                            if (filter[filter.Length - 1 - i] != word[word.Length - 1 - i] && filter[filter.Length - 1 - i] != '?')
                            {
                                return false;
                            }

                        }
                        else
                        {
                            lastCheckedTailIndex = i;
                            break;
                        }
                    }
                    // Reverse and trim word and filter
                    reversedWord = new char[word.Length - lastCheckedHeadIndex - lastCheckedTailIndex];
                    reversedFilter = new char[filter.Length - lastCheckedHeadIndex - lastCheckedTailIndex];

                    for (int i = 0; i < reversedWord.Length; i++)
                    {
                        reversedWord[i] = word[word.Length - (i + 1) - lastCheckedTailIndex];
                    }
                    for (int i = 0; i < reversedFilter.Length; i++)
                    {
                        reversedFilter[i] = filter[filter.Length - (i + 1) - lastCheckedTailIndex];
                    }

                    for (int i = 0; i < reversedFilter.Length; i++)
                    {
                        if (reversedFilter[i] == '*')
                        {
                            if (i - currentPatternStartIndex > 0)
                            {
                                char[] pattern = new char[i - currentPatternStartIndex];
                                for (int j = 0; j < pattern.Length; j++)
                                {
                                    pattern[j] = reversedFilter[currentPatternStartIndex + j];
                                }
                                reversedPatterns.Add(pattern);
                            }

                            currentPatternStartIndex = i + 1;
                        }
                    }
                    //Search for the patterns
                    for (int i = 0; i < reversedPatterns.Count; i++)
                    {
                        for (int j = 0; j < reversedPatterns[i].Length; j++)
                        {

                            if ((reversedPatterns[i].Length - 1 - j) > (reversedWord.Length - 1 - reversedWordIndex))
                            {
                                return false;
                            }


                            if (reversedPatterns[i][j] != '?' && reversedPatterns[i][j] != reversedWord[reversedWordIndex + j])
                            {
                                reversedWordIndex += 1;
                                j = -1;
                            }
                            else
                            {
                                if (j == reversedPatterns[i].Length - 1)
                                {
                                    reversedWordIndex += reversedPatterns[i].Length;
                                }
                            }
                        }
                    }
                    break;
            }
            return isLike;
        }

        /// <summary>
        /// Match a string against a given target string with wildcards ("*" and "?" are supported). See Remarks for details.
        /// </summary>
        /// <param name="text">The string to test.</param>
        /// <param name="wildcardString">The string to match against, containing the wildcards.</param>
        /// <param name="ignoreCase">Set if letter casing is ignored while matching.</param>
        /// <returns>True if it can be matched; false if they cannot be.</returns>
        /// <remarks>
        /// This function can match a string against a wildcard string. This is used, for example, in Windows's file dialogs for opening or saving files.
        /// Supported wildcard characters are "*", which matches any number of characters, and "?", which matches just one character.
        /// (For example, "d*g" will match both "dig" and "ding", where as "d?g" will only match "dig" and not "ding".)
        /// Regex (regular expressions) is a lot more powerful than what wildcards alone can provide, but regex strings can also be a lot more complicated to decipher.
        /// <para/>
        /// This class was written by H.A. Sullivan.
        /// </remarks>
        public static bool MatchesWildcard(string text, string wildcardString, bool ignoreCase)
        {
            if (ignoreCase == true)
            {
                return MatchesWildcard(text.ToLowerInvariant(), wildcardString.ToLowerInvariant());
                //return text.ToLower().EqualsWildcard(wildcardString.ToLower());
            }
            else
            {
                return MatchesWildcard(text, wildcardString);
            }
        }
    }
}
