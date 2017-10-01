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
            bool answer = false;
            string t = string.Empty;

            try
            {
                List<uint> la = new List<uint>();
                List<uint> lb = new List<uint>();

                //la.Add( (uint)((ulong)0xFFFFFFFF % BigNumber.BASE) ); la.Add((uint)((ulong)0xFFFFFFFF / BigNumber.BASE));
                la.Add(9); la.Add(1);
                lb.Add(1);

                a = new BigNumber(la);
                b = new BigNumber(lb);
                c = a.NAND(b);
                t = c.ToString();
                //a = "451";
                //a = new BigNumber(450);
                //b = new BigNumber("450");
                //t = a.ToString();
                //t = b.ToString();
                ////t = b.ToString();

                ////c = a * b;
                //t = c.ToString();

                //string t = c.ToString();
                //a = "5123456789";
                //la.Add(828489493); la.Add(1);
                //a = new BigNumber(la);
                //a = "5123456789";
                la.Add(0x0000000D); //la.Add(1); la.Add(9);
                lb.Add(0x0000001B); //lb.Add(9); //lb.Add(99);
                /*
                uint xx = 13;
                uint r = 0;
                for (int i = 0; i < 4; i++)
                {
                    r = (r << 1) | (xx & 1);
                    xx >>= 1;
                }
                */

                //b = new BigNumber(lb);
                //a = "5123456789";
                //a = "10";
                //b = "11";
                //c = a.OR(b);
                //a.Compress();
                //b = "-1"; //nu blir det kul!
                //c = a * b;
                //t = c.ToString();
                //c = a;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                MessageBox.Show(ex.Message,
                    Properties.Settings.Default.ProgramName + " ArgumentOutOfRangeException",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, 
                    Properties.Settings.Default.ProgramName + " ArgumentException", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show(ex.Message,
                    Properties.Settings.Default.ProgramName + " NullReferenceException",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message,
                    Properties.Settings.Default.ProgramName + " FormatException",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch(ArithmeticException ex)
            {
                MessageBox.Show(ex.Message,
                    Properties.Settings.Default.ProgramName + " ArithmeticException",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, 
                    Properties.Settings.Default.ProgramName + " Exception", 
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
