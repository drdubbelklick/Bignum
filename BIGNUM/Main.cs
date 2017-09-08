using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BIGNUM
{
    /// <summary>
    /// GUI class for the Factorization program
    /// </summary>
    public partial class Factorization : Form
    {
        /// <summary>
        /// Set to true once the Stop button has been clicked.
        /// It is vital that we poll this flag often to provide
        /// a responsive program for the user
        /// </summary>
        static bool _stopExecution = false;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Factorization()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Fired when the user clicks the Start button
        /// </summary>
        private void startButton_Click(object sender, EventArgs e)
        {
            BigNumber a,b,c;

            try
            {
                List<uint> la = new List<uint>();
                List<uint> lb = new List<uint>();

                la.Add(uint.MaxValue); la.Add(9);
                
                a = new BigNumber(la);
                string s = a.ToString();
                b = new BigNumber(lb);
                c = a + b;
                string t = c.ToString();
                //a = new BigNumber("5123456789");
                //b = new BigNumber("5123456789");
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, 
                    Properties.Settings.Default.ProgramName, 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show(ex.Message,
                    Properties.Settings.Default.ProgramName,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message,
                    Properties.Settings.Default.ProgramName,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, 
                    Properties.Settings.Default.ProgramName, 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            _stopExecution = false;
        }

        /// <summary>
        /// Fired when the user clicks the Stop button
        /// </summary>
        private void stopButton_Click(object sender, EventArgs e)
        {
            _stopExecution = true;
        }

    }
}
