using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace BIGNUM
{
    /// <summary>
    /// class for doing arithmetics on non-negative large integers
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
        //static uint HALFWORDMAX = (uint)9;
        /// <summary>
        /// the base of our operation
        /// </summary>
        public static ulong BASE = (ulong)HALFWORDMAX + (ulong)1;
        //public static ulong BASE = (ulong)1000000000;
        //public static ulong BASE = (ulong)10;
        #region Properties

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
        /// true or false depending on the number is negative or greater 
        /// than or equal to zero
        /// </summary>
        public bool IsNegative { get; set; }

        #endregion

        #region Constructors

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
            throw new NotImplementedException("Not yet finished");
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
        public BigNumber(List<uint> numList) //HACK temporarily opened up this constructor as public pending constructor that accepts a string.
        {
            if (numList == null)
                throw new ArgumentNullException("Input list is null");
            _number.AddRange(numList);
            Trim();
        }

        #endregion

        #region Properties

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

        #endregion

        /// <summary>
        /// Overrides ToString()
        /// </summary>
        /// <returns>the internal number as a string representation</returns>
        /// <exception cref="FormatException">is thrown by base.ToString(), see documentation</exception>
        /// 
        public override string ToString()
        {
            string res = string.Empty;
            
            for (int i = 0; i < _number.Count; i++)
            {
                // here we pad the number with leading zeros to make it
                // ten digits long, equal to the decimal equivalent of
                // 0xFFFFFFFF, i.e. uint.MaxValue.ToString().Length
                res = _number[i].ToString("D10") + res;
            }

            if (IsNegative)
                res = "-" + res;
            return res;
        }

        /// <summary>
        /// Trims elements that are zero from the BigNumber, making
        /// it as compact as possible. A trimmed object is returned.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// when the argument to remove an element is less than zero or
        /// the argument is greater than or equal to the argument
        /// is greater than or equal to the number of elements in 
        /// the underlying data structure for BigNumber or
        /// there is no data in the underlying data structure at all
        /// </exception>
        void Trim()
        {
            int nSize = _number.Count;
            if (nSize == 0)
                throw new ArgumentOutOfRangeException("Number contains no data");
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

        public static BigNumber operator-(BigNumber lhs, BigNumber rhs)
        {
            if (lhs < rhs)
                return -(rhs - lhs);

            BigNumber res;

            try
            {
                lhs.Trim();
                rhs.Trim();
                // The number having the longest list determines the size
                // of the resulting list
                int newSize = Math.Max(lhs.Size, rhs.Size);
                List<uint> resList = new List<uint>(newSize);
                uint valLhs, valRhs;
                uint borrow = 0;

                for (int i = 0; i < newSize; i++)
                {
                    if (lhs.Size <= i)
                        valLhs = 0;
                    else
                        valLhs = lhs.Number[i];
                    if (rhs.Size <= i)
                        valRhs = 0;
                    else
                        valRhs = rhs.Number[i];

                    long diff = (long)valLhs - (long)valRhs - (long)borrow;

                    if (diff < 0)
                    {
                        borrow = 1;
                        diff += (long)BASE;
                    }
                    else
                    {
                        borrow = 0;
                    }
                    Debug.Assert(diff >= 0 && diff <= uint.MaxValue);
                    resList.Add((uint)diff);
                }
                Debug.Assert(borrow == 0, "Error in implementation of binary - : there should not be any borrow at the end of the loop.");
                res = new BigNumber(resList);
                res.IsNegative = (borrow == 1);
                return res;
            }
            catch (ArgumentOutOfRangeException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Operators

        /// <summary>
        /// Unary minus
        /// </summary>
        public static BigNumber operator-(BigNumber lhs)
        {
            //TODO: is this a proper solution???

            BigNumber res = new BigNumber(lhs);

            res.IsNegative = !lhs.IsNegative;

            return res;
        }

        /// <summary>
        /// greater than operator
        /// </summary>
        public static bool operator>(BigNumber lhs, BigNumber rhs)
        {
            lhs.Trim();
            rhs.Trim();

            int size = Math.Max(lhs.Number.Count, rhs.Number.Count);

            if (lhs.Number.Count != rhs.Number.Count)
                return lhs.Number.Count > rhs.Number.Count;
            // Both are of equal sizes.
            for (int i = lhs.Number.Count - 1; i >= 0;  i--)
            {
                if (lhs.Number[i] > rhs.Number[i])
                    return true;
            }
            return false; // all numbers are equal
        }

        /// <summary>
        /// Implements the operator >= between two operators,
        /// returning true if this is the case, else false
        /// </summary>
        public static bool operator >=(BigNumber lhs, BigNumber rhs)
        {
            //TODO better implementation of this for speed
            if (lhs < rhs)
                return false;
            if (lhs != rhs)
                return false;
            return true;
        }

        /// <summary>
        /// Implements the operator <= between two operators,
        /// returning true if this is the case, else false
        /// </summary>
        public static bool operator <=(BigNumber lhs, BigNumber rhs)
        {
            //TODO better implementation of this for speed
            if (lhs > rhs)
                return false;
            if (lhs != rhs)
                return false;
            return true;
        }

        /// <summary>
        /// less than operator
        /// </summary>
        /// <exception cref="ArgumentException">
        /// thrown when either of the operands have no data
        /// </exception>
        public static bool operator<(BigNumber lhs, BigNumber rhs)
        {
            lhs.Trim();
            rhs.Trim();

            if (lhs.Number.Count == 0)
                throw new ArgumentException("Left hand operand has no data");
            if (rhs.Number.Count == 0)
                throw new ArgumentException("Right hand operand has no data");

            int size = Math.Max(lhs.Number.Count, rhs.Number.Count);

            if (lhs.Number.Count != rhs.Number.Count)
                return lhs.Number.Count < rhs.Number.Count;
            // Both are of equal sizes.
            for (int i = lhs.Number.Count - 1; i >= 0; i--)
            {
                if (lhs.Number[i] < rhs.Number[i])
                    return true;
            }
            return false; // all numbers are equal
        }

        /// <summary>
        /// equality operator
        /// </summary>
        public static bool operator==(BigNumber lhs, BigNumber rhs)
        {
            lhs.Trim();
            rhs.Trim();
            if (lhs.Number.Count != rhs.Number.Count)
                return false;
            for (int i = 0; i < lhs.Number.Count; i++)
            {
                if (lhs.Number[i] != rhs.Number[i])
                    return false;
            }
            return true;
        }

        /// <summary>
        /// inequality operator
        /// </summary>
        public static bool operator!=(BigNumber lhs, BigNumber rhs)
        {
            //TODO optimize this
            return !(lhs == rhs);
        }

        #endregion
    }
}
