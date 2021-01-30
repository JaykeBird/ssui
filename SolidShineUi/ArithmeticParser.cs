using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidShineUi
{
    /// <summary>
    /// Contains functionality to parse and evaluate math arithmetic expressions.
    /// </summary>
    public static class ArithmeticParser
    {
        /// <summary>
        /// Evaluate an arithmetic expression and output the result (i.e. "2+5" will output "7"). See remarks for more info on supported functions.
        /// </summary>
        /// <param name="input">The arithmetic expression to evaluate.</param>
        /// <exception cref="FormatException">Thrown if the expression is not valid (contains unrecognized characters or a mismatched number of parantheses).</exception>
        /// <returns>The result of the expression.</returns>
        /// <remarks>Supports addition, subtraction, multiplication, division, and exponents. Also supports parantheses (with implied multiplcation) and negative numbers.<para/>
        /// Uses order of operations: exponents, division, multiplication, subtraction, addition.<para/>
        /// Whitespace and new-line characters are removed prior to evaluation, and the literal characters ×, ⋅, and ÷ are replaced with their representations *, *, and /.</remarks>
        public static double Evaluate(string input)
        {
            // first, remove whitespace/newlines
            input = input.Replace(" ", "");
            input = input.Replace("\t", "");
            input = input.Replace("\n", "");
            input = input.Replace("\r", "");

            // if somehow anyone put in the proper symbols, let's replace it
            input = input.Replace("×", "*");
            input = input.Replace("⋅", "*");
            input = input.Replace("÷", "/");
            // also support someone putting in a backslash
            input = input.Replace("\\", "/");

            // check the string to make sure it's valid
            // outputs as int so that we can give more relevant error messages
            int g = PreCheckString(input);

            if (g == 0)
            {
                return PerformEvaluation(input);
            }
            else if (g == 1)
            {
                throw new FormatException("This expression contains some unrecognized characters. Cannot be evaluated.");
            }
            else if (g == 2)
            {
                throw new FormatException("This expression contains some characters in an invalid combination. Cannot be evaluated.");
            }
            else if (g == 3)
            {
                throw new FormatException("This expression does not contain any numbers. Cannot be evaluated.");
            }
            else if (g == 4)
            {
                throw new FormatException("This expression ends with an operator character, which is invalid. Cannot be evaluated.");
            }
            else if (g == 5)
            {
                throw new FormatException("This expression starts with an operator character, which is invalid. Cannot be evaluated.");
            }
            else
            {
                throw new FormatException("This expression cannot be evaluated for an undetermined reason.");
            }
        }

        static double PerformEvaluation(string input)
        {
            if (input.Contains("(") || input.Contains(")"))
            {
                if (!CheckValidParenthesys(input))
                {
                    //return null;
                    throw new FormatException("This expression has an unmatching number of opening and closing parantheses. Cannot be evaluated.");
                }

                // a listing of substitutions to make
                // as expressions within parantheses are evaluated and resolved
                Dictionary<string, string> Changes = new Dictionary<string, string>();

                bool openPar = false;
                bool justClosed = false;
                int openIndex = 0;
                char prev = ' ';
                for (int i = 0; i < input.Length; i++)
                {
                    if (!openPar)
                    {
                        if (input[i] == '(')
                        {
                            // check for instances of an implied multiplication
                            // i.e. 3(4+2) or (2-3)(4/6)
                            if ("0123456789)".Contains(prev))
                            {
                                input = input.Insert(i, "*");
                                i++;
                            }
                            openPar = true;
                            openIndex = i;
                            justClosed = false;
                        }
                        else if (justClosed)
                        {
                            if ("0123456789".Contains(input[i]))
                            {
                                input = input.Insert(i, "*");
                                i++;
                            }

                            justClosed = false;
                        }
                    }
                    else
                    {
                        if (input[i] == ')')
                        {
                            string subInput = input.Substring(openIndex + 1, i - openIndex - 1);

                            if (!Changes.ContainsKey(subInput))
                            {
                                string evaluatedSubInput = PerformEvaluation(subInput).ToString();
                                Changes.Add(subInput, evaluatedSubInput);
                            }
                            openPar = false;
                            openIndex = 0;
                            justClosed = true;
                        }
                    }
                    prev = input[i];
                }

                foreach (var item in Changes.Keys)
                {
                    input = input.Replace(item, Changes[item]);
                }
                input = input.Replace("(", "");
                input = input.Replace(")", "");
            }

            var ParsedOperations = ParseOperations(input);
            if (ParsedOperations != null)
            {
                try
                {
                    return PerformOperations(ref ParsedOperations);
                }
                catch (ArgumentOutOfRangeException)
                {
                    throw new FormatException("This expression contains an operator without a number to evaluate with it. Cannot be evaluated.");
                }
            }
            else
            {
                // throwing a FormatException would be better than a NullReferenceException
                // since a NullReferenceException just means something somewhere was null, not as helpful lol
                throw new FormatException("This expression contains some unrecognized characters. Cannot be evaluated.");
            }
        }

        // checks to make sure opening parantheses == closing parantheses (i.e. "(())" is good, "(()" is not)
        static bool CheckValidParenthesys(string input)
        {
            int openPar = 0;
            int closePar = 0;
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '(')
                    openPar++;
                else if (input[i] == ')')
                    closePar++;
            }
            return openPar == closePar;
        }

        static double PerformOperations(ref List<string> OperationList)
        {
            while (OperationList.Count > 1)
            {
                int index = 0;
                while (index < OperationList.Count)
                {
                    if (OperationList[index] == "^")
                    {
                        double div1 = double.Parse(OperationList[index - 1]);
                        double div2 = double.Parse(OperationList[index + 1]);
                        OperationList[index - 1] = Math.Pow(div1, div2).ToString();
                        OperationList.RemoveAt(index);
                        OperationList.RemoveAt(index);
                    }
                    index++;
                }
                index = 0;
                while (index < OperationList.Count)
                {
                    if (OperationList[index] == "/")
                    {
                        double div1 = double.Parse(OperationList[index - 1]);
                        double div2 = double.Parse(OperationList[index + 1]);
                        OperationList[index - 1] = (div1 / div2).ToString();
                        OperationList.RemoveAt(index);
                        OperationList.RemoveAt(index);
                    }
                    index++;
                }
                index = 0;
                while (index < OperationList.Count)
                {
                    if (OperationList[index] == "*")
                    {
                        double mult1 = double.Parse(OperationList[index - 1]);
                        double mult2 = double.Parse(OperationList[index + 1]);
                        OperationList[index - 1] = (mult1 * mult2).ToString();
                        OperationList.RemoveAt(index);
                        OperationList.RemoveAt(index);
                    }
                    index++;
                }
                index = 0;
                while (index < OperationList.Count)
                {
                    if (OperationList[index] == "-")
                    {
                        double res1 = double.Parse(OperationList[index - 1]);
                        double res2 = double.Parse(OperationList[index + 1]);
                        OperationList[index - 1] = (res1 - res2).ToString();
                        OperationList.RemoveAt(index);
                        OperationList.RemoveAt(index);
                    }
                    index++;
                }
                index = 0;
                while (index < OperationList.Count)
                {
                    if (OperationList[index] == "+")
                    {
                        double sum1 = double.Parse(OperationList[index - 1]);
                        double sum2 = double.Parse(OperationList[index + 1]);
                        OperationList[index - 1] = (sum1 + sum2).ToString();
                        OperationList.RemoveAt(index);
                        OperationList.RemoveAt(index);
                    }
                    index++;
                }
            }
            return double.Parse(OperationList[0]);
        }

#if NETCOREAPP
        static List<string>? ParseOperations(string input)
#else
        static List<string> ParseOperations(string input)
#endif
        {
            List<string> Operations = new List<string>(); // organized list of operations to do
            int index = 0; // current char looked at
            int prevRes = -2; // store CheckChars result of previous number
            string SubEvaluator = ""; // buffer for numbers
            while (index < input.Length)
            {
                if (CheckChars(input[index]) == 0) // number, digit separator
                {
                    // quick loop to get through all the numbers
                    // return to main while loop once we encounter something that isn't a number
                    for (int i = index; i < input.Length; i++)
                    {
                        if (CheckChars(input[i]) == 0)
                        {
                            SubEvaluator += input[i];
                            if (i == input.Length - 1)
                            {
                                Operations.Add(SubEvaluator);
                                index = input.Length;
                            }
                        }
                        else
                        {
                            Operations.Add(SubEvaluator);
                            SubEvaluator = "";
                            index = i;
                            break;
                        }
                    }
                    prevRes = 0;
                }
                else if (CheckChars(input[index]) == 1) // operation
                {
                    // special handling for negative numbers
                    // check if the previous character was an operation (or start of string)
                    // if so, probably indicating a negative number
                    if ((prevRes == 1 || prevRes == -2) && input[index] == '-')
                    {
                        // put into SubEvaluator
                        SubEvaluator += input[index];
                    }
                    else
                    {
                        Operations.Add(input[index].ToString());
                    }
                    index++;
                    prevRes = 1;
                }
                else // paranthesis, or invalid character (parantheses should not be showing up at this point)
                {
                    return null; // ends up triggering FormatException
                }
            }
            return Operations;
        }

        static int CheckChars(char input)
        {
            return CheckChars(input.ToString());
        }

        static int CheckChars(string input)
        {
            if ("0987654321.,".Contains(input))
                return 0;
            else if ("/*-+^".Contains(input))
                return 1;
            else if ("()".Contains(input))
                return 2;
            else
                return -1;
        }

        static int PreCheckString(string input)
        {

            bool hasnumber = false;
            char prev = '0';

            // iterate through each character to make sure they're all valid
            // if it's invalid, then we can fail fast
            foreach (char c in input)
            {
                if (!"0987654321.,/*-+^()".Contains(c))
                {
                    return 1;
                }
                else if ("0987654321".Contains(c))
                {
                    hasnumber = true;
                }

                if (prev == '0')
                {
                    if (c == '/' || c == '*' || c == '+' || c == '-')
                    {
                        prev = c;
                    }
                }
                else
                {
                    if (c == '/' || c == '*' || c == '+')
                    {
                        return 2;
                    }
                    else if (c == '-')
                    {
                        prev = '-';
                    }
                    else
                    {
                        prev = '0';
                    }
                }
            }

            if (!hasnumber)
            {
                return 3;
            }

            if (input.EndsWith("/") || input.EndsWith("*") || input.EndsWith("+") || input.EndsWith("-"))
            {
                return 4;
            }

            if (input.StartsWith("+") || input.StartsWith("*") || input.StartsWith("/"))
            {
                return 5;
            }

            return 0;
        }

        /// <summary>
        /// Check if a string can be parsed as an expression.
        /// </summary>
        /// <param name="input">The string to parse.</param>
        /// <returns>True if it is a string that can be parsed. False if it contains invalid characters.</returns>
        public static bool IsValidString(string input)
        {
            // first, remove whitespace/newlines
            input = input.Replace(" ", "");
            input = input.Replace("\t", "");
            input = input.Replace("\n", "");
            input = input.Replace("\r", "");

            // if somehow anyone put in the proper symbols, let's replace it
            input = input.Replace("×", "*");
            input = input.Replace("⋅", "*");
            input = input.Replace("÷", "/");
            // also support someone putting in a backslash
            input = input.Replace("\\", "/");

            // check for invalid combinations
            // i.e. 3+*3
            char prev = '0';
            bool hasnumber = false;

            if (input.EndsWith("/") || input.EndsWith("*") || input.EndsWith("+") || input.EndsWith("-"))
            {
                return false;
            }
            
            if (input.StartsWith("+") || input.StartsWith("*") || input.StartsWith("/"))
            {
                return false;
            }

            // iterate through each character to make sure they're all valid
            // if it's invalid, then we can fail fast
            foreach (char c in input)
            {
                if (!"0987654321.,/*-+^()".Contains(c))
                {
                    return false;
                }
                else if ("0987654321".Contains(c))
                {
                    hasnumber = true;
                }

                if (prev == '0')
                {
                    if (c == '/' || c == '*' || c == '+' || c == '-')
                    {
                        prev = c;
                    }
                }
                else
                {
                    if (c == '/' || c == '*' || c == '+')
                    {
                        return false;
                    }
                    else if (c == '-')
                    {
                        prev = '-';
                    }
                    else
                    {
                        prev = '0';
                    }
                }
            }

            if (!hasnumber)
            {
                return false;
            }

            return true;
        }
    }
}
