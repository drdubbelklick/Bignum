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
        
#if THOMAS
        static uint HALFWORDMAX = (uint)9;
#else
        static uint HALFWORDMAX = (uint)0xFFFFFFFF;
#endif

        /// <summary>
        /// the base of our operation
        /// </summary>
        public static ulong BASE = (ulong)HALFWORDMAX + (ulong)1;
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
        /// <param name="num">consists of [-]0..9, hence base 10</param>
        /// <exception cref="ArgumentException">
        ///     thrown if there are characters other than [-]0..9 
        ///     in the input argument
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// thrown if the parameter is null or the empty string
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// thrown if the index to the Substring call on the supplied string 
        /// is less than zero or greater than the length of the string
        /// </exception>
        /// <remarks>The parameter is assumed to be in base 10</remarks>
        public BigNumber(string num)
        {
            if (string.IsNullOrEmpty(num))
                throw new ArgumentNullException("num is null or the empty string");
            if (!num.IsValidNumber())
                throw new ArgumentException("Parameter must only consist of digits [-]0..9");
            else
            {
                num = num.Trim();
                if (num.Left(1) == "-")
                {
                    num = num.Substring(1);
                    IsNegative = true;
                }
                else
                    IsNegative = false;


                /*
                        public BigNumber(ulong num)
                        {
                            while(num > 0)
                            {
                                uint rem = (uint)(num % BASE);
                                num = num / BASE;
                                _number.Add(rem);
                            }
                            if (_number.Count == 0)
                                _number.Add(0);
                        }
                */
                ulong val = 0;
                uint rem = 0;
                ulong quo = 0;
                ulong remainder = 0;

                while (num != string.Empty)
                {
                    rem = uint.Parse(num.Right(1)); // "rem = num MOD 10"
                    remainder = 10 * remainder + rem;
                    _number.Add(rem);
                    num = num.Left(num.Length - 1); // "num = num DIV 10"
                    val = 10 * val + rem; // always 10, regardless of BASE
                    if (val > HALFWORDMAX)
                    {
                        val = val / BASE;
                    }
                }
                Compress();
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
            //Trim();
        }

        /// <summary>
        /// Convenience constructor for laborating with small numbers
        /// </summary>
        public BigNumber(ulong num)
        {
            //TODO this implementation suffers from the same problem 
            // as constructor BigNumber(string) has
            while(num > 0)
            {
                uint rem = (uint)(num % BASE);
                num = num / BASE;
                _number.Add(rem);
            }
            if (_number.Count == 0)
                _number.Add(0);
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
#if THOMAS
                res = _number[i].ToString("D") + res;
#else
            res = _number[i].ToString("D10") + res;
#endif

                //while we laborate with 10 as base, we don't put any leading zeros in front of each number
                //res = _number[i].ToString("D") + res;
            }

            if (IsNegative)
                res = "-" + res;
            return res;
        }

        /// <summary>
        /// Given a base 10 number that has one digit per slot in
        /// the _number list, we compress it to HALFWORDMAX digits large
        /// numbers
        /// </summary>
        public void Compress() //TODO: change accesibility back to private
        {
            ulong fact = 0;
            uint rem = 0;
            ulong quo = 0;
            List<uint> res = new List<uint>();
            int i;

            //TODO vet inte vad som skulle stå på den här raden
            Trim();
            if (_number.Count == 1 && _number[0] == 0)
                return; // nothing to do - the Trim() has done its job

            i = 0;
            
            while (i < _number.Count)
            {
                fact = 10 * fact + _number[i];
                
                if (fact > HALFWORDMAX)
                {
                    quo = fact / BASE;
                    rem = (uint)(fact % BASE);
                
                    res.Add(rem);
                    tmp = quo;
                    for (int k = 0; k < i; k++)
                        _number.RemoveAt(0);

                    i = -1; //start at the first element (we do i++ in the while loop number.below)
                }
                i++;
            }
            //TODO: något måste göras här på slutet om tmp har ett värde
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
        public void Trim()
        {
            int nSize = _number.Count;
            if (nSize == 0)
                throw new ArgumentOutOfRangeException("Number contains no data");
            while (nSize > 0 && _number[nSize - 1] == 0)
            {
                _number.RemoveAt(nSize - 1);
                nSize--;
            }
            // the number must contain at least one element
            // (lots of functions depend on it)
            if (_number.Count == 0)
                _number.Add(0);
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
            int newSize = Math.Max(lhs.Size, rhs.Size) + 1;
            List<uint> resList = new List<uint>(newSize);
            uint carry = 0;

            for (int i = 0; i < newSize; i++)
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

                val = valLhs + valRhs + carry;                

                if (val > HALFWORDMAX)
                {
                    carry = (uint)(val / BASE);
                    val = val % BASE;
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

        /// <summary>
        /// subtraction operator for two BigNumbers
        /// </summary>
        /// <param name="lhs">left hand side of the operation</param>
        /// <param name="rhs">right hand side of the operation</param>
        /// <returns>the resultant BigNumber</returns>
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
                    Debug.Assert(diff >= 0 && diff <= HALFWORDMAX);
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

        /// <summary>
        /// multiplication operator for two BigNumbers
        /// </summary>
        /// <param name="lhs">left hand side of the operation</param>
        /// <param name="rhs">right hand side of the operation</param>
        /// <returns>the resultant BigNumber</returns>
        public static BigNumber operator*(BigNumber lhs, BigNumber rhs)
        {
            // The number having the longest list determines the size
            // of the resulting list
            lhs.Trim();
            rhs.Trim();
            if (lhs.Size < rhs.Size)
                return rhs * lhs;
            int newSize = lhs.Size + rhs.Size + 2; // add one for each operand's plausible carry
            List<uint> row = new List<uint>(newSize);
            List<uint> sum = new List<uint>(newSize);
            BigNumber rowNumber;
            BigNumber totalSum;
            uint carry = 0;

            // fill up the lists with empty slots (needed for the [] operator below to succeed
            for (int i = 0; i < newSize; i++)
                row.Add(0);

            for (int i = 0; i < newSize; i++)
                sum.Add(0);
            totalSum = new BigNumber(sum);

            for (int iRhs = 0; iRhs < rhs.Size; iRhs++)
            {
                carry = 0;
                // clear the row before each run. 
                // row.Clear() removes the elements, and we
                // want to keep them, just set them to zero
                for (int i = 0; i < newSize; i++)
                    row[i] = 0;
                for (int iLhs = 0; iLhs < lhs.Size; iLhs++)
                {
                    ulong fact = (ulong)lhs.Number[iLhs] * (ulong)rhs.Number[iRhs] + (ulong)carry;
                    uint mod = (uint)(fact % BASE);

                    row[iLhs + iRhs] = mod;

                    if (fact > (ulong)HALFWORDMAX)
                    {
                        carry = (uint)(fact / BASE);
                    }
                    else
                    {
                        carry = 0;
                    }
                }
                if (carry > 0)
                    row[iRhs + lhs.Size] = carry;
                rowNumber = new BigNumber(row);
                totalSum += rowNumber;
            }

            BigNumber res = new BigNumber(totalSum);

            return res;
        }

        /// <summary>
        /// Increment operator
        /// </summary>
        public static BigNumber operator++(BigNumber num)
        {
            BigNumber one = new BigNumber(1);

            num = num + one;
            return num;
        }

        /// <summary>
        /// Decrement operator
        /// </summary>
        public static BigNumber operator --(BigNumber num)
        {
            BigNumber one = new BigNumber(1);

            num = num - one;
            return num;
        }

#endregion

#region Comparison operators

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
                if (lhs.Number[i] > rhs.Number[i])
                    return false;
            }
            return true; // all numbers are equal
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

        /// <summary>
        /// allows for assignment of a string to an already allocated BigNumber
        /// </summary>
        /// <param name="num">the string</param>
        /// <exception cref="ArgumentNullException">thrown when the parameter is null</exception>
        /// <exception cref="ArgumentException">thrown when the argument does contain other than [-]0..9</exception>
        public static implicit operator BigNumber(string num)
        {
            if (string.IsNullOrEmpty(num))
                throw new ArgumentNullException("Attempt to assign the BigNumber to a null value");
            if (!num.IsValidNumber())
                throw new ArgumentException("Parameter must only consist of digits [-]0..9");

            BigNumber res = new BigNumber(num);

            return res;
        }

#endregion
    }
}
