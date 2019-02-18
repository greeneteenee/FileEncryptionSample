using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace FileEncryptionSample
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
            
        }

        private string generateKey()
        {
            DESCryptoServiceProvider dESCrypto = (DESCryptoServiceProvider)DESCryptoServiceProvider.Create();
            return ASCIIEncoding.ASCII.GetString(dESCrypto.Key);
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Multiselect = false;//user can only open one file
            if(open.ShowDialog() == DialogResult.OK)
            {
                //get path of file
                string filePath = open.FileName;
                txtFilePath.Text = filePath;

                //read contents of file into a stream
                var fileStream = open.OpenFile();

                using(StreamReader reader = new StreamReader(filePath))
                {
                    txtShowFile.Text = reader.ReadToEnd();
                }

            }

            btnSave.Enabled = true;

        }

        private void rbEncrypt_CheckedChanged(object sender, EventArgs e)
        {
            if (rbEncrypt.Checked)
            {
                rbDecrypt.Checked = false;
            }
        }

        private void rbDecrypt_CheckedChanged(object sender, EventArgs e)
        {
            if (rbDecrypt.Checked)
            {
                rbEncrypt.Checked = false;
            }
                
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            rbEncrypt.Checked = true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!File.Exists(txtFilePath.Text))
            {
                MessageBox.Show("Must open file first");
                return;
            }

            string hash = "f0xle@rn";
            
            if(rbEncrypt.Checked == true)
            {
                encrypt(txtShowFile.Text, hash);
            }
            else
            {
                decrypt(txtShowFile.Text, hash);
            }

            SaveFileDialog save = new SaveFileDialog();
            if(save.ShowDialog() == DialogResult.OK)
            {
                string saveFileName = save.FileName;
                File.WriteAllText(saveFileName, txtShowFile.Text);
                txtSaveFilePath.Text = save.FileName;
            }

            btnSave.Enabled = false;
       
        }

        private void encrypt(string input, string StrHash)
        {
            byte[] data = UTF8Encoding.UTF8.GetBytes(input);
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] keys = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(StrHash));
                using(TripleDESCryptoServiceProvider tripDes = new TripleDESCryptoServiceProvider() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                {
                    ICryptoTransform transform = tripDes.CreateEncryptor();
                    byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
                    txtShowFile.Text = "";
                    txtShowFile.Text = Convert.ToBase64String(results, 0, results.Length);
                }
            }
        }

        private void decrypt(string input, string StrHash)
        {
            byte[] data = Convert.FromBase64String(input);
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] keys = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(StrHash));
                using (TripleDESCryptoServiceProvider tripDes = new TripleDESCryptoServiceProvider() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                {
                    ICryptoTransform transform = tripDes.CreateDecryptor();
                    byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
                    txtShowFile.Text = "";
                    txtShowFile.Text = UTF8Encoding.UTF8.GetString(results);
                }
            }
        }

    }
}
