using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;

namespace RSA
{
    public partial class MainForm : Form
    {
        BigInteger p; // náhodné prvočíslo 
        BigInteger q; // náhodné prvočíslo 
        BigInteger n;// = p * q; // modul
        BigInteger eulerFunction;// = (p - 1) * (q - 1); // Eulerova funkce
        BigInteger e; // šifrovací, veřejný exponent.
        BigInteger d; // dešifrovací či soukromý exponent
        BigInteger m; // numerická podoba části zprávy
        BigInteger c;  // zašifrovaná část zprávy
        BigInteger mDecyphered; //dešifrovaná část zprávy
        int bitLength = 512; //default lenght

        public MainForm()
        {
            InitializeComponent();
        }

        private void SetRandomPrimes()
        {
            do
            {
                do
                {
                    //p = p - 1;
                    p = GenerateRandomBigInteger(bitLength);
                    if(p < 0)
                    {
                        p *= -1;
                    }
                } while (IsPrime(p, 4) != true);

                do
                {
                    //q = q - 1;
                    q = GenerateRandomBigInteger(bitLength);
                    if (q < 0)
                    {
                        q *= -1;
                    }
                } while (IsPrime(q, 4) != true);

            } while (p == q);
        }
        

        private BigInteger GenerateRandomBigInteger(int lenght)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[lenght / 8];
            rng.GetBytes(bytes);
            return new BigInteger(bytes);
        }
        
        private void SetN()
        {
            n = p * q;
        }

        private void SetEulerFunction()
        {
            eulerFunction = (p - 1) * (q - 1);
        }

        private void SetE()
        {            
            do
            {
                e = GenerateRandomBigInteger(bitLength);
                if(e < 0)
                {
                    e *= -1;
                }
            } while (e >= eulerFunction || BigInteger.GreatestCommonDivisor(e, eulerFunction) != 1);
            
        }

        private void SetD()
        {
            d = StaticMethods.extendedGCD(e, eulerFunction);
        }

        private void ButtonEncrypt_Click(object sender, EventArgs e)
        {
            StringBuilder input = new StringBuilder();

            if (this.e.IsZero == true || n.IsZero == true)
            {
                MessageBox.Show("Keys are empty!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + DateTime.Now.ToString("MM/dd/yyyy HH/mm/ss") + ".txt";
                string outputParameters = $"Your public key n: {n}\nYour public key e: {this.e}\n\nYour private key n: {n}\nYour private key d: {d}";
                File.AppendAllText(path, outputParameters);
                List<BigInteger> message = StaticMethods.GenerateMessageNumber(richTextBoxInput.Text, bitLength);

                for (int i = 0; i < message.Count; i++)
                {
                    m = message[i];
                    c = BigInteger.ModPow(m, this.e, n);
                    input.Append(c);
                    input.Append(" ");
                }

                string output = input.ToString();

                richTextBoxOutput.Text = output.Remove(output.Length - 1, 1);
            }
        }

        private void ButtonDecrypt_Click(object sender, EventArgs e)
        {
            try
            {
                if (richTextBoxInput.Text.Last() == ' ')
                {
                    MessageBox.Show("Remove the last space!!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    StringBuilder output = new StringBuilder();
                    if (d.IsZero == true || n.IsZero == true)
                    {
                        MessageBox.Show("Keys are empty!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        BigInteger[] toDecrypt = richTextBoxInput.Text.Split(' ').Select(n => BigInteger.Parse(n)).ToArray();

                        for (int i = 0; i < toDecrypt.Length; i++)
                        {
                            c = toDecrypt[i];
                            mDecyphered = BigInteger.ModPow(c, d, n);
                            output.Append(StaticMethods.ReturnMessageFromNumber(mDecyphered));
                        }
                    }

                    richTextBoxOutput.Text = output.ToString();
                }
            }
            catch
            {
                MessageBox.Show("Input must contain only numbers seperated by spaces!!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RadioButtonsCheckChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                bitLength = Convert.ToInt32(((RadioButton)sender).Text);
            }
        }


        private void ButtonGenerateParameters_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Generating parameters. This might take a while", "Aaaargh matey!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            SetRandomPrimes();
            SetN();
            SetEulerFunction();
            SetE();
            SetD();
            richTextBoxNPublic.Text = Convert.ToString(n);
            richTextBoxNPrivate.Text = Convert.ToString(n);
            richTextBoxE.Text = Convert.ToString(this.e);
            richTextBoxD.Text = Convert.ToString(d);
            MessageBox.Show("Parameters set!", "Aaaargh matey!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ButtonSetParameters_Click(object sender, EventArgs e)
        {
            try
            {
                bool handled = false;

                if (richTextBoxNPublic.Text != "")
                {
                    n = BigInteger.Parse(richTextBoxNPublic.Text);
                    handled = true;
                }
                if (richTextBoxNPrivate.Text != "")
                {
                    n = BigInteger.Parse(richTextBoxNPrivate.Text);
                    handled = true;
                }
                if (richTextBoxE.Text != "")
                {
                    this.e = BigInteger.Parse(richTextBoxE.Text);
                    handled = true;
                }
                if (richTextBoxD.Text != "")
                {
                    d = BigInteger.Parse(richTextBoxD.Text);
                    handled = true;
                }

                if(handled == true)
                {
                    MessageBox.Show("Parameters set!", "Aaaargh matey!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else MessageBox.Show("No parameters to set!", "Oh no, matey!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch
            {
                MessageBox.Show("Keys must be numbers!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool IsPrime(BigInteger number, int certainty)
        {
            if (number == 2 || number == 3)
            {
                return true;
            }
            if (number < 2 || number % 2 == 0)
            {
                return false;
            }

            BigInteger d = number - 1;
            int s = 0;

            while (d % 2 == 0)
            {
                d /= 2;
                s += 1;
            }

            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] bytes = new byte[number.ToByteArray().LongLength];
            BigInteger a;

            for (int i = 0; i < certainty; i++)
            {
                do
                {
                    rng.GetBytes(bytes);
                    a = new BigInteger(bytes);
                }
                while (a < 2 || a >= number - 2);

                BigInteger x = BigInteger.ModPow(a, d, number);
                if (x == 1 || x == number - 1)
                {
                    continue;
                }

                for (int r = 1; r < s; r++)
                {
                    x = BigInteger.ModPow(x, 2, number);
                    if (x == 1) return false;
                    if (x == number - 1) break;
                }

                if (x != number - 1) return false; else return true;
            }

            return false;
        }
    }
}
