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
        static public uint NBITSINWORD
        {
            get
            {
#if THOMAS
                return 4;
#else
                return 32;
#endif
            }
        }

        /// <summary>
        /// returns the maximum value that can be stored in each slot
        /// </summary>
        static public uint WORDMAX
        {
            get
            {
#if THOMAS
                return 9;
#else
                return 0xFFFFFFFF;
#endif
            }
        }


        /// <summary>
        /// the base of our operation
        /// </summary>
        static ulong BASE = (ulong)WORDMAX + (ulong)1;

        /// <summary>
        /// multiplicative neutral element
        /// </summary>
        static BigNumber ONEVAL = new BigNumber((uint)1);

        /// <summary>
        /// additive neutral element
        /// </summary>
        static BigNumber ZEROVAL = new BigNumber((uint)0);

        /// <summary>
        /// Truth table for operation NAND(x,y) = NANDVEC[x,y]
        /// </summary>
        static uint[,] NANDVEC = new uint[,] { { 1, 1 }, { 1, 0 } };
        /// <summary>
        /// Truth table for operation XAND(x,y) = XANDVEC[x,y]
        /// </summary>
        static uint[,] XANDVEC = new uint[,] { { 0, 0 }, { 1, 0 } };
        /// <summary>
        /// Truth table for operation XNOR(x,y) = XNORVEC[x,y]
        /// </summary>
        static uint[,] XNORVEC = new uint[,] { { 1, 0 }, { 0, 1 } };
        /// <summary>
        /// Truth table for operation NOR(x,y) = NORVEC[x,y]
        /// </summary>
        static uint[,] NORVEC = new uint[,] { { 1, 0 }, { 0, 0 } };
        /// <summary>
        /// Truth table for operation IMPLY(x,y) = IMPLYVEC[x,y]
        /// </summary>
        static uint[,] IMPLYVEC = new uint[,] { { 1, 1 }, { 0, 1 } };

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
        ///  Additive neutral element
        /// </summary>
        public static BigNumber ZERO
        {
            get
            {
                return ZEROVAL;
            }
        } 

        /// <summary>
        /// Multiplicative neutral element
        /// </summary>
         public static BigNumber ONE
        {
            get
            {
                return ONEVAL;
            }
        }
#endregion

#region Constructors

        /// <summary>
        /// default constructor
        /// </summary>
        public BigNumber()
        {
            _number.Add(0);
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
                throw new ArgumentNullException("Parameter is null or the empty string");
            if (!num.IsValidNumber())
                throw new ArgumentException("Parameter should only consist of digits [-]0..9");
            else
            {
                BigNumber res = new BigNumber();

                num = num.Trim();
                if (num.Left(1) == "-")
                {
                    num = num.Substring(1);
                    this.IsNegative = true;
                }
                else
                {
                    this.IsNegative = false;
                }

                // since the algoritm picks out the numbers in the wrong order
                // we must reverse the string
                num = num.Reverse();

                uint rem = 0;
                BigNumber base10 = new BigNumber(10);
                while (num != string.Empty)
                {
                    rem = uint.Parse(num.Right(1)); // "rem = num MOD 10"
                    num = num.Left(num.Length - 1); // "num = num DIV 10"
                    res = res * base10 + new BigNumber(rem);

#if NOTHING
while(*pch)
   {
      iNum=*pch-'0';
      
      //validate character at hand
      MPASSERT(iNum>=0 && iNum<=9);
      if (iNum<0 || iNum>9)
         return FALSE;
      
      //new = old * base + next digit
      mp_setdigit(&digit,0,(HALFWORD)iNum);      
      mp_mul(pNum,&base,&temp);
      mp_add(&temp,&digit,pNum);

      pch++;
   }
#endif
                }
                _number = res.Number;
                //TODO: the vector _number needs reversing
                
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
        /// <exception cref="ArgumentNullException">thrown if the supplied list is null</exception>
        /// <exception cref="ArgumentException">thrown if the supplied list contains no data</exception>
        public BigNumber(List<uint> numList) //HACK temporarily opened up this constructor as public pending constructor that accepts a string.
        {
            if (numList == null)
                throw new ArgumentNullException("Input list is null");
            if (numList.Count == 0)
                throw new ArgumentException("Input list contains no data");

            _number.AddRange(numList);
            //Trim();
        }

        /// <summary>
        /// convenience constructor for dealing with smaller numbers
        /// </summary>
        public BigNumber(uint num)
        {
            _number.Add(num);
        }

#endregion

        /// <summary>
        /// Overrides ToString()
        /// </summary>
        /// <returns>the internal number as a base 10 string representation</returns>
        public override string ToString()
        {
            string res = string.Empty;
            
            for (int i = 0; i < _number.Count; i++)
            {
                // here we pad the number with leading zeros to make it
                // ten digits long, equal to the decimal equivalent of
                // 0xFFFFFFFF, i.e. uint.MaxValue.ToString().Length
#if THOMAS
                res = _number[i].ToString("D") + " " + res;
#else
                res = _number[i].ToString("D10") + " " + res;
#endif
            }

            if (IsNegative)
                res = "-" + res;
            return res.Trim();
        }

        /// <summary>
        /// Given a base 10 number that has one digit per slot in
        /// the _number list, we compress it to WORDMAX digits large
        /// numbers
        /// </summary>
        public void Compress() //HACK give "public" temporarily acccess to the function
        {
            Trim();
            if (_number.Count == 1 && _number[0] == 0)
                return; // nothing to do - the Trim() has done its job

            ulong fact = 0; // cumulative factor that builds up the number, one digit at a time
            uint rem = 0; // remainder with division by BASE
            int i = 0; // iteration variable
            List<uint> res = new List<uint>();
            List<uint> rev = new List<uint>(_number);

            // the algorithm works by picking out the least
            // significant uint each iteration, so for this to work as planned, 
            // we have to reverse the data
            rev.Reverse();

            while(i < rev.Count)
            {
                fact = fact * BigNumber.BASE + rev[i];
                rem = (uint)(fact % BASE);

                if (fact > BASE)
                {
                    res.Add(BigNumber.ReverseUInt(rem));
                    fact = fact / BASE;
                }
                i++;
            }
            if (rem > 0)
                res.Add(BigNumber.ReverseUInt(rem));
            _number = res;
            Reverse();
        }

        /// <summary>
        /// resizes the _number storage to newSize elements and fills them with zeros
        /// </summary>
        /// <exception cref="ArgumentException">thrown when the new size is smaller than the current size</exception>
        void Resize(int newSize)
        {
            if (_number.Count == newSize)
                return; //nothing to do
            if (_number.Count > newSize)
                throw new ArgumentException("the new size must be greater than the current size", "newSize");
            int itemsToAdd = newSize - _number.Count;

            for (int i = 0; i < itemsToAdd; i++)
                _number.Add(0); //important that the zero is added as the last element
            //it is counter-productive to call Trim() after this :-)
        }

        /// <summary>
        /// Trims elements that are zero from the BigNumber, making
        /// it as compact as possible.
        /// </summary>
        /// <exception cref="ArithmeticException">
        ///     thrown when there is no data in the underlying 
        ///     data structure at all
        /// </exception>
        void Trim()
        {
            int nSize = _number.Count;

            if (nSize == 0)
                throw new ArithmeticException("Number contains no data");

            // while possible, remove the topmost zero element
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
        
        /// <summary>
        /// Reverses the order of bits in a 32 bit uint
        /// </summary>
        static uint ReverseUInt(uint x)
        {
            x = ((x >> 1) & 0x55555555u) | ((x & 0x55555555u) << 1);
            x = ((x >> 2) & 0x33333333u) | ((x & 0x33333333u) << 2);
            x = ((x >> 4) & 0x0f0f0f0fu) | ((x & 0x0f0f0f0fu) << 4);
            x = ((x >> 8) & 0x00ff00ffu) | ((x & 0x00ff00ffu) << 8);
            x = ((x >> 16) & 0xffffu) | ((x & 0xffffu) << 16);
            return x;
        }

        /// <summary>
        /// retrieves the bit at position bitNo
        /// </summary>
        /// <param name="bitNo">the zero based index to retrieve</param>
        /// <returns>the desired bit</returns>
        /// <exception cref="ArgumentOutOfRangeException">thrown when bitNo is larger than the number of words</exception>
        public byte GetBit(int bitNo)
        {
            throw new NotImplementedException("Snart klar");
            /*
            long idxWord = ((long)bitNo / (long)BigNumber.BASE);
            int idxBit = (int)((long)bitNo % (long)BigNumber.BASE);

            if (idxWord > _number.Count)
            {
                throw new ArgumentOutOfRangeException("Out of range", "bitNo");
            }

            // the if statement above has taken care of that there is no overflow
            // when accessing _number[]
            uint w = _number[(int)idxWord];
            uint b = (w >> (BigNumber.NBITSINWORD - idxBit)) & 0x1;
            Debug.Assert(b == 0 || b == 1, "b > 1");
            return (byte)b;
            */
        }

        /// <summary>
        /// Reverses the internal data structure. This occurs when
        /// we have called the constructor with a base 10 string, for instance.
        /// The operation is selfmutating.
        /// </summary>
        void Reverse()
        {
            if (_number.Count == 0)
                throw new ArithmeticException("Number contains no data");
            _number.Reverse();
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

            BigNumber res;

            try
            {

                lhs.Resize(newSize);
                rhs.Resize(newSize);

                for (int i = 0; i < newSize; i++)
                {
                    uint valLhs, valRhs;
                    ulong val;

                    valLhs = lhs.Number[i];
                    valRhs = rhs.Number[i];

                    val = valLhs + valRhs + carry;

                    if (val > WORDMAX)
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

                res = new BigNumber(resList);
                return res;
            }
            catch (ArgumentOutOfRangeException)
            {
                throw;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
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

                lhs.Resize(newSize);
                rhs.Resize(newSize);

                for (int i = 0; i < newSize; i++)
                {
                    valLhs = lhs.Number[i];
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
                    Debug.Assert(diff >= 0 && diff <= WORDMAX);
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
            catch(ArgumentException)
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
            try
            {
                // The number having the longest list determines the size
                // of the resulting list
                lhs.Trim();
                rhs.Trim();

                int newSize = lhs.Size + rhs.Size + 2; // add one for each operand's plausible carry
                List<uint> row = new List<uint>(newSize);
                uint carry = 0;

                BigNumber rowNumber;
                BigNumber totalSum;

                // fill up the lists with empty slots (needed for the [] operator below to succeed
                for (int i = 0; i < newSize; i++)
                    row.Add(0);

                totalSum = new BigNumber();
                totalSum.Resize(newSize);

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

                        if (fact > (ulong)WORDMAX)
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
            catch(ArgumentOutOfRangeException)
            {
                throw;
            }
            catch(Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Increment operator
        /// </summary>
        public static BigNumber operator++(BigNumber num)
        {
            num = num + ONE;
            return num;
        }

        /// <summary>
        /// Decrement operator
        /// </summary>
        public static BigNumber operator --(BigNumber num)
        {
            num = num - ONE;
            return num;
        }

#endregion

#region Bitwise operators

        /// <summary>
        /// bitwise OR operator
        /// </summary>
        public static BigNumber operator |(BigNumber lhs, BigNumber rhs)
        {
            return lhs.OR(rhs);
        }

        /// <summary>
        /// bitwise AND operator
        /// </summary>
        public static BigNumber operator &(BigNumber lhs, BigNumber rhs)
        {
            return lhs.AND(rhs);
        }

        /// <summary>
        /// unary bitwise NOT operator
        /// </summary>
        public static BigNumber operator ~(BigNumber num)
        {
            return num.NOT();
        }

        /// <summary>
        /// bitwise operator XOR
        /// </summary>
        public static BigNumber operator ^(BigNumber lhs, BigNumber rhs)
        {
            return lhs.XOR(rhs);
        }
        
        /// <summary>
        /// performs a bitwise left shift of a BigNumber
        /// </summary>
        /// <exception cref="ArgumentException">thrown when supplied number has no digits</exception>
        public static BigNumber operator <<(BigNumber num, int noOfBitsToShift)
        {
            if (num.Number.Count == 0)
                throw new ArgumentException("Parameter contains no data");

            return num.LSHn(noOfBitsToShift);
        }

        /// <summary>
        /// performs a bitwise right shift of a BigNumber
        /// </summary>
        /// <exception cref="ArgumentException">thrown when supplied number has no digits</exception>
        public static BigNumber operator >>(BigNumber num, int noOfBitsToShift)
        {
            if (num.Number.Count == 0)
                throw new ArgumentException("Parameter contains no data");

            return num.RSHn(noOfBitsToShift);
        }

        /// <summary>
        /// returns bitwise OR of the number and the supplied operand
        /// </summary>
        public BigNumber OR(BigNumber num)
        {
            // The number having the longest list determines the size
            // of the resulting list
            int newSize = Math.Max(this.Size, num.Size);
            BigNumber res = new BigNumber(this._number);

            res.Resize(newSize);
            num.Resize(newSize);
            
            for (int i = 0; i < newSize; i++)
            {
                uint valLhs, valRhs;

                valLhs = res.Number[i];
                valRhs = num.Number[i];

                res.Number[i] = valLhs | valRhs;
            }

            return res;
        }

        /// <summary>
        /// returns bitwise AND of the number and the supplied operand
        /// </summary>
        public BigNumber AND(BigNumber num)
        {
            // The number having the longest list determines the size
            // of the resulting list
            int newSize = Math.Max(this.Size, num.Size);
            BigNumber res = new BigNumber(this._number);

            res.Resize(newSize);
            num.Resize(newSize);

            for (int i = 0; i < newSize; i++)
            {
                uint valLhs, valRhs;

                valLhs = res.Number[i];
                valRhs = num.Number[i];

                res.Number[i] = valLhs & valRhs;
            }

            return res;
        }

        /// <summary>
        /// unary bitwise NOT
        /// </summary>
        public BigNumber NOT()
        {
            BigNumber res = new BigNumber(this._number);

            res.Trim();

            for (int i = 0; i < res.Number.Count; i++)
                res.Number[i] = ~(res.Number[i]);

            return res;
        }

        /// <summary>
        /// returns bitwise XOR of the number and the supplied operand
        /// </summary>
        public BigNumber XOR(BigNumber num)
        {
            // The number having the longest list determines the size
            // of the resulting list
            int newSize = Math.Max(this.Size, num.Size);
            BigNumber res = new BigNumber(this._number);

            res.Resize(newSize);
            num.Resize(newSize);

            for (int i = 0; i < newSize; i++)
            {
                uint valLhs, valRhs;

                valLhs = res.Number[i];
                valRhs = num.Number[i];

                res.Number[i] = valLhs ^ valRhs;
            }

            return res;
        }

        /// <summary>
        /// returns the absolute value of this instance
        /// </summary>
        public BigNumber ABS()
        {
            BigNumber res = new BigNumber(this);

            res.Trim();
            res.IsNegative = false;

            return res;
        }

        /// <summary>
        /// Performs the Left Shift function with the specified number of bits
        /// </summary>
        public BigNumber LSHn(int noOfBitsToShift)
        {
            throw new NotImplementedException("På gång");
/*
            long noOfWordShifts = noOfBitsToShift / BigNumber.NBITSINWORD,
                noOfBitShifts = noOfBitsToShift % BigNumber.NBITSINWORD;
            List<uint> resList = new List<uint>(this.Size + 1);

            for (int i = 0; i < noOfWordShifts; i++)
                resList[i + noOfWordShifts] = _number[i];
*/
        }

        /// <summary>
        /// performs the Right Shift function with the specified number of bits
        /// </summary>
        public BigNumber RSHn(int noOfBitsToShift)
        {
            throw new NotImplementedException("På gång");
        }

        /// <summary>
        /// performs a right rotate of the bits in the number
        /// </summary>
        public BigNumber ROTRn(int noOfBitsToRotate)
        {
            throw new NotImplementedException("På gång");
        }

        /// <summary>
        /// performs a left rotate of the bits in the number
        /// </summary>
        public BigNumber ROTLn(int noOfBitsToRotate)
        {
            throw new NotImplementedException("På gång");
        }

        /// <summary>
        /// performs the logical NAND between
        /// this instance and in the supplied number 
        /// </summary>
        /// <remarks>
        ///     The NAND is a logical operation on two boolean
        ///     values, that is true if and only if at least one of
        ///     the operands are false. We treat the BigNumber on a WORD
        ///     having either 1=true or 0=false. The result if other values
        ///     than those are stored is indeterminate.
        ///     
        ///     Truth Table
        ///     INPUT OUTPUT
        ///     A B   
        ///     ------------
        ///     0 0 1
        ///     0 1 1
        ///     1 0 1
        ///     1 1 0
        /// </remarks>
        public BigNumber NAND(BigNumber num)
        {
            // The number having the longest list determines the size
            // of the resulting list
            int newSize = Math.Max(this.Size, num.Size);
            BigNumber res = new BigNumber(this._number);

            res.Resize(newSize);
            num.Resize(newSize);

            for (int i = 0; i < newSize; i++) // for each word in _number
            {
                uint valLhs, valRhs;

                valLhs = res.Number[i];
                valRhs = num.Number[i];

                uint result = 0;

                for (int j = 0; j < BigNumber.NBITSINWORD; j++) // for each bit in _number[i]
                {
                    uint lhsBit = valLhs & 0x1;
                    uint rhsBit = valRhs & 0x1;

                    valLhs = valLhs >> 1;
                    valRhs = valRhs >> 1;
                    result |= (BigNumber.NANDVEC[lhsBit, rhsBit] << j);
                }
                res.Number[i] = result;
            }

            return res;
        }

        /// <summary>
        /// performs the logical NOR between
        /// this instance and in the supplied number 
        /// </summary>
        /// <remarks>
        ///     The NOR is a logical operation on two boolean
        ///     values, that is true if and only if at least one of
        ///     the operands are false. We treat the BigNumber on a WORD
        ///     having either 1=true or 0=false. The result if other values
        ///     than those are stored is indeterminate.
        /// </remarks>
        public BigNumber NOR(BigNumber num)
        {
            // The number having the longest list determines the size
            // of the resulting list
            int newSize = Math.Max(this.Size, num.Size);
            BigNumber res = new BigNumber(this._number);

            res.Resize(newSize);
            num.Resize(newSize);

            for (int i = 0; i < newSize; i++) // for each word in _number
            {
                uint valLhs, valRhs;

                valLhs = res.Number[i];
                valRhs = num.Number[i];

                uint result = 0;

                for (int j = 0; j < BigNumber.NBITSINWORD; j++) // for each bit in _number[i]
                {
                    uint lhsBit = valLhs & 0x1;
                    uint rhsBit = valRhs & 0x1;

                    valLhs = valLhs >> 1;
                    valRhs = valRhs >> 1;
                    result |= (BigNumber.NORVEC[lhsBit, rhsBit] << j);
                }
                res.Number[i] = result;
            }

            return res;
        }

        /// <summary>
        /// performs the bitwise XNOR between 
        /// the bits in this instance and in the supplied number
        /// </summary>
        public BigNumber XNOR(BigNumber num)
        {
            // The number having the longest list determines the size
            // of the resulting list
            int newSize = Math.Max(this.Size, num.Size);
            BigNumber res = new BigNumber(this._number);

            res.Resize(newSize);
            num.Resize(newSize);

            for (int i = 0; i < newSize; i++) // for each word in _number
            {
                uint valLhs, valRhs;

                valLhs = res.Number[i];
                valRhs = num.Number[i];

                uint result = 0;

                for (int j = 0; j < BigNumber.NBITSINWORD; j++) // for each bit in _number[i]
                {
                    uint lhsBit = valLhs & 0x1;
                    uint rhsBit = valRhs & 0x1;

                    valLhs = valLhs >> 1;
                    valRhs = valRhs >> 1;
                    result |= (BigNumber.XNORVEC[lhsBit, rhsBit] << j);
                }
                res.Number[i] = result;
            }

            return res;
        }

        /// <summary>
        /// performs the bitwise XAND between 
        /// the bits in this instance and in the supplied number
        /// </summary>
        /// <remarks>a∧¬b</remarks>
        public BigNumber XAND(BigNumber num)
        {
            // The number having the longest list determines the size
            // of the resulting list
            int newSize = Math.Max(this.Size, num.Size);
            BigNumber res = new BigNumber(this._number);

            res.Resize(newSize);
            num.Resize(newSize);

            for (int i = 0; i < newSize; i++) // for each word in _number
            {
                uint valLhs, valRhs;

                valLhs = res.Number[i];
                valRhs = num.Number[i];

                uint result = 0;

                for (int j = 0; j < BigNumber.NBITSINWORD; j++) // for each bit in _number[i]
                {
                    uint lhsBit = valLhs & 0x1;
                    uint rhsBit = valRhs & 0x1;

                    valLhs = valLhs >> 1;
                    valRhs = valRhs >> 1;
                    result |= (BigNumber.XANDVEC[lhsBit, rhsBit] << j);
                }
                res.Number[i] = result;
            }

            return res;
        }

#endregion

#region Miscellaneous operators

        /// <summary>
        /// Unary minus
        /// </summary>
        /// <exception cref="ArgumentNullException">thrown when the argument is null</exception>
        /// <exception cref="ArgumentException">
        ///     thrown when the argument contains no data, 
        ///     most likely due to passing in a newly created object
        /// </exception>
        /// <remarks>sign changer</remarks>
        public static BigNumber operator -(BigNumber num)
        {
            if (num.Number.Count == 0)
                throw new ArgumentException("Parameter contains no data");

            BigNumber res = new BigNumber(num);

            res.IsNegative = !num.IsNegative;

            return res;
        }

        /// <summary>
        /// Unary plus
        /// </summary>
        /// <exception cref="ArgumentException">
        ///     thrown when the argument contains no data, 
        ///     most likely due to passing in a newly created object
        /// </exception>
        /// <remarks>essentially a no-op</remarks>
        public static BigNumber operator +(BigNumber num)
        {
            if (num.Number.Count == 0)
                throw new ArgumentException("Parameter contains no data");

            BigNumber res = new BigNumber(num);

            res.IsNegative = num.IsNegative;

            return res;
        }

        /// <summary>
        /// allows for assignment of a string to an already allocated BigNumber
        /// </summary>
        /// <param name="num">the string</param>
        /// <exception cref="ArgumentNullException">thrown when the parameter is null</exception>
        /// <exception cref="ArgumentException">thrown when the argument does contain other than [-]0..9</exception>
        /// <exception cref="Exception">thrown when there is some other, unknown exception</exception>
        public static implicit operator BigNumber(string num)
        {
            if (string.IsNullOrEmpty(num))
                throw new ArgumentNullException("Attempt to assign the BigNumber to a null value");
            if (!num.IsValidNumber())
                throw new ArgumentException("Parameter must only consist of digits [-]0..9");

            BigNumber res;

            try
            {
                res = new BigNumber(num);
                return res;
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentNullException("Attempt to assign a null value to a BigNumber was detected");
            }
            catch (ArgumentException)
            {
                throw new ArgumentException("The string you use to assign the BigNumber to must only contain the digits 0..9");
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Allows for creation of a BigNumber by means of a cast
        /// </summary>
        public static explicit operator BigNumber(int num)
        {
            uint val = num < 0 ? (uint)Math.Abs(num) : (uint)num;
            BigNumber res = new BigNumber(val);

            res.IsNegative = num < 0;

            return res;
        }
        
#endregion

#region Comparison operators

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

            //TODO kan nedanstående inträffa?
            if (lhs.Number.Count == 0)
                throw new ArgumentException("Left hand operand has no data");
            if (rhs.Number.Count == 0)
                throw new ArgumentException("Right hand operand has no data");

            int size = Math.Max(lhs.Number.Count, rhs.Number.Count);

            //TODO: felimplementerat. Se t.ex. XOR
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

#endregion
    }
}
