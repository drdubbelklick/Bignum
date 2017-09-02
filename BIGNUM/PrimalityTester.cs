using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIGNUM
{

    /// <summary>
    /// default constructor
    /// </summary>
    public class PrimalityTester
    {
        /// <summary>
        /// set to true once the caller decides to halt execution
        /// </summary>
        bool _stopExecution = false;
       
        /// <summary>
        /// public property that allows the caller 
        /// to abort the computations
        /// </summary>
        public bool StopExecution
        {
            set
            {
                _stopExecution = value;
            }
        }

        /// <summary>
        /// default constructor
        /// </summary>
        public PrimalityTester()
        {
            _stopExecution = false;
        }

        /// <summary>
        /// given a BigNumber, it applies certain tests,
        /// and gives a response whether the number is prime or not
        /// </summary>
        public bool IsPrime(BigNumber number)
        {
            _stopExecution = false;
            //TODO: check _haltExecution at the various stages and 
            //      throw an exception if we have been aborted
            return false;
        }
    }

    
}
