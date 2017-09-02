using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIGNUM
{
    /// <summary>
    /// class for doing big number arithmetic
    /// </summary>
    public class BigNumber
    {
        List<int> _number = new List<int>();
        /// <summary>
        /// default constructor, that 
        /// prevents caller instantiation
        /// </summary>
        protected BigNumber()
        {
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="number">consists of 0..9</param>
        /// <exception cref="ArgumentException">
        ///     thrown if there are characters other than 0..9 
        ///     in the input argument
        /// </exception>
        public BigNumber(string number)
        {
            if (!IsValidNumber(number))
                throw new ArgumentException("Parameter must only consist of characters 0..9");
            else
            {
                //TODO: transform string into List<int>
            }
        }

        /// <summary>
        /// Provides outside access to the internal list of integers composing this BigNumber.
        /// This is a must when building the operators, since they must be static :-(
        /// C++ offers greater flexibility here.
        /// </summary>
        public List<int> Number
        {
            get
            {
                return _number;
            }
        }

        /// <summary>
        /// checks if all characters in supplied argument are 0..9
        /// and returns true, else false
        /// </summary>
        /// <param name="number">string to examine</param>
        /// <returns></returns>
        public static bool IsValidNumber(string number)
        {
            foreach (char ch in number)
            {
                if (!char.IsDigit(ch))
                    return false;
            }
            return true;
        }
        /// <summary>
        /// addition operator for two BigNumbers
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static BigNumber operator+(BigNumber lhs, BigNumber rhs)
        {
            //TODO: proper implementation of this. 
            // The number having the longest list determines the size
            // of the resulting list
            return lhs;
        }
    }
}
