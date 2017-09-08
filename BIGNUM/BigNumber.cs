using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace BIGNUM
{
    /// <summary>
    /// class for doing big number arithmetic
    /// </summary>
    public class BigNumber
    {
        /// <summary>
        /// The element at position zero is the least significant integer
        /// </summary>
        List<uint> _number = new List<uint>();
        /// <summary>
        /// Largest unsigned integer we store in each slot
        /// </summary>
        static uint HALFWORDMAX = (uint)0xFFFFFFFF;
        /// <summary>
        /// true or false depending on the number is <0 or >= 0
        /// </summary>
        public bool IsNegative { get; set; }

        /// <summary>
        /// Returns true if the number is even else false
        /// </summary>
        /// <exception cref="IndexOutOfRangeException">thrown if the BigNumber has no data</exception>
        public bool IsEven
        {
            get
            {
                if (_number.Count == 0)
                    throw new IndexOutOfRangeException("Object contains no data");
                else
                    return (_number[0] & 0x1) == 0x0;
            }
        }

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
        /// <param name="num">consists of 0..9, hence base 10</param>
        /// <exception cref="ArgumentException">
        ///     thrown if there are characters other than 0..9 
        ///     in the input argument
        /// </exception>
        public BigNumber(string num)
        {
            if (string.IsNullOrEmpty(num) || !num.IsValidNumber())
                throw new ArgumentException("Parameter must only consist of digits 0..9");
            else
            {
                num = num.Trim();
                //TODO reverse the string - blir fel på "MSB"
                //num = num.Reverse();
                
                uint digit;
                ulong val = 0;
                uint lastVal = 0;

                while (num != string.Empty)
                {
                    digit = uint.Parse(num.Right(1)); // this amounts to "digit = num MOD 10"
                    num = num.Left(num.Length - 1); // this amounts to "num = num DIV 10"
                    lastVal = (uint)val;
                    val = 10 * val + digit;
                    if (val > HALFWORDMAX)
                    {
                        _number.Add((uint)val - HALFWORDMAX);
                        val = digit;
                    }
                }
                if (val > 0)
                    _number.Add((uint)val);
            }
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        public BigNumber(BigNumber number)
        {
            _number.AddRange(number.Number);
            Trim();
        }

        /// <summary>
        /// private constructor that allows the caller to 
        /// give an already filled list of uint:s as argument
        /// </summary>
        public BigNumber(List<uint> numList) //TODO temporarily open up this constructor. it should be private
        {
            _number.AddRange(numList);
            Trim();
        }

        /// <summary>
        /// Overrides ToString()
        /// </summary>
        /// <returns>the internal number as a string representation</returns>
        /// <exception cref="FormatException">is thrown by base.ToString(), see documentation</exception>
        public override string ToString()
        {
            string res = string.Empty;
            //ulong val = 0;
                      
            for (int i = 0; i < _number.Count; i++)
            {
                res = _number[i].ToString("D") + res;
            }

            return res;
        }

        /// <summary>
        /// Property for outside access to the internal list of integers composing this BigNumber.
        /// This is a must when building the operators, since they must be static :-(
        /// C++ offers greater flexibility here.
        /// </summary>
        public List<uint> Number
        {
            get
            {
                return _number;
            }
            private set
            {
                _number = value;
            }
        }

        /// <summary>
        /// Convenience property, if the user is only interested in
        /// the size of the underlying data structure
        /// </summary>
        public int Size
        {
            get
            {
                return _number.Count;
            }
        }

        /// <summary>
        /// Trims elements that are zero from the BigNumber, making
        /// it as compact as possible. The trimming is done in-place.
        /// </summary>
        void Trim()
        {
            int nSize = _number.Count;

            while (nSize > 0 && _number[nSize - 1] == 0)
            {
                _number.RemoveAt(nSize - 1);
                nSize--;
            }
        }

        #region Arithmetics

        /// <summary>
        /// addition operator for two BigNumbers
        /// </summary>
        /// <param name="lhs">left hand side of the operation</param>
        /// <param name="rhs">right hand side of the operation</param>
        /// <returns>the resultant BigNumber</returns>
        public static BigNumber operator+(BigNumber lhs, BigNumber rhs)
        {
            // The number having the longest list determines the size
            // of the resulting list
            lhs.Trim();
            rhs.Trim();
            int newSize = Math.Max(lhs.Size, rhs.Size);
            List<uint> resList = new List<uint>(newSize + 1); // add one for a possible carry
            uint carry = 0;

            for (int i = 0; i < newSize+1; i++)
            {
                uint valLhs, valRhs;
                ulong val;
                
                if (lhs.Size <= i)
                    valLhs = 0;
                else
                    valLhs = lhs.Number[i];
                if (rhs.Size <= i)
                    valRhs = 0;
                else
                    valRhs = rhs.Number[i];

                val = (ulong)valLhs + (ulong)valRhs + carry;                

                if ((ulong)val > HALFWORDMAX)
                {
                    carry = 1;
                    val -= HALFWORDMAX;
                }
                else
                {
                    carry = 0;
                }
                resList.Add((uint)val);
            }
            BigNumber res = new BigNumber(resList);
            return res;
        }

        #endregion
    }
}
