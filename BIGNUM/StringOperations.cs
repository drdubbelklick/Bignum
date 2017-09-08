using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIGNUM
{
    /// <summary>
    /// extension class for string
    /// </summary>
    public static class StringOperations
    {
        /// <summary>
        /// Returns the Min(numLetters, me.Length) characters
        /// as a string from the left of this string instance
        /// </summary>
        public static string Left(this string me, int numLetters)
        {
            numLetters = Math.Min(me.Length, numLetters);

            if (numLetters <= 0)
                return string.Empty;
            else
                return me.Substring(0, numLetters);
        }

        /// <summary>
        /// Returns the Min(numLetters, me.Length) characters
        /// as a string from the right of this string instance
        /// </summary>
        public static string Right(this string me, int numLetters)
        {
            numLetters = Math.Min(me.Length, numLetters);

            if (numLetters <= 0)
                return string.Empty;
            else
                return me.Substring(me.Length - numLetters, numLetters);
        }

        /// <summary>
        /// Reverses the characters in the string
        /// </summary>
        /// <returns>the reversed string</returns>
        public static string Reverse(this string me)
        {
            string res = string.Empty;

            foreach(char ch in me)
            {
                res = ch + res;
            }
            return res;
        }

        /// <summary>
        ///     checks if all characters in supplied argument are 0..9
        ///     and returns true, else false
        /// </summary>
        /// <param name="me">instance to examine</param>
        /// <returns>true if all characters in the string 
        ///     are 0..9 else false
        /// </returns>
        public static bool IsValidNumber(this string me)
        {
            foreach (char ch in me)
            {
                if (!char.IsDigit(ch))
                    return false;
            }
            return true;
        }
    }
}
