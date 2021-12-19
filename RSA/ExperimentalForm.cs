using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Numerics;

namespace RSA
{
    public partial class ExperimentalForm : Form
    {
        public ExperimentalForm()
        {
            InitializeComponent();
        }

        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(3072);

        private void ButtonEncrypt_Click(object sender, EventArgs e)
        {
            byte[] inputData = Encoding.Unicode.GetBytes(richTextBox1.Text);
            byte[] encryptedData = rsa.Encrypt(inputData, false);
            richTextBox2.Text = Convert.ToBase64String(encryptedData);
        }

        private void ButtonDecrypt_Click(object sender, EventArgs e)
        {
            byte[] inputData = Convert.FromBase64String(richTextBox2.Text);
            byte[] decryptedData = rsa.Decrypt(inputData, false);
            richTextBox3.Text = Encoding.Unicode.GetString(decryptedData);
        }
    }
}
