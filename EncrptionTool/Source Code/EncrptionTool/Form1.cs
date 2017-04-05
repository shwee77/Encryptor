using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;
using System.Diagnostics;

namespace EncryptionTool
{
    public partial class Pr0t3ct3d : Form
    {
        public Pr0t3ct3d()
        {
            InitializeComponent();
            txtPass.PasswordChar = '*';    //Set Password Field to "*"
            txtDecPass.PasswordChar = '*';
            

        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            txtFile.Text = openFileDialog1.FileName;
            try
            {

                FileInfo FI = new FileInfo(openFileDialog1.FileName);  //get file size and display
                float size = (FI.Length / 1024f); ;
                string outString = size.ToString("####0.00"); // setting up for 2 decimals
                size1.Text = " File size : " + outString + " KB ";
            }
            catch (Exception ex)
            { 
            }



        }

        private void button2_Click(object sender, EventArgs e)
        {
            

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            //Unmask Password Field
            if (checkBox1.Checked)
            {
                txtPass.PasswordChar = '\0';
            }
            else
            {
                txtPass.PasswordChar = '*';
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
          
            if (((txtFile.Text) == "" )|| ((txtPass.Text) == "")) //check if any of the fields are empty 
            {
                MessageBox.Show("Please choose a file and a password ", "Error! ", MessageBoxButtons.OK, MessageBoxIcon.Error); //Error message if fields are empty 
           
            }
            else
            {


                
                AES_Encrypt(txtFile.Text, txtPass.Text); // calling the encryption method

                MessageBox.Show("File Encrypted", "Encryption Completed", MessageBoxButtons.OK, MessageBoxIcon.Asterisk); //Prompt Message 

                String Dir = @"C:\Users\Public\EncryptionTool";  //create a backup password file . 
                if (!Directory.Exists(Dir)) //check if the directory exists 
                {
                    Directory.CreateDirectory(Dir);  //create if not
                    WriteToFile(txtFile.Text, txtPass.Text);
                }
                else
                {
                    WriteToFile(txtFile.Text, txtPass.Text); //write to file if the file existing 


                }

                 // Reset text fields
                txtFile.Text = null;
                txtPass.Text = null;
                size1.Text = null;

            }

        }

        //AES Encryption Algorithm 

        private static void AES_Encrypt(string inputFile, string password)
        {
            
            {
               

                //generate random salt
                byte[] salt = GenerateRandomSalt();

                //create output file extension to ".aes"
                FileStream fsCrypt = new FileStream(inputFile + ".aes", FileMode.Create);

                //convert password string to byte arrray
                byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);

                //Set Rijndael symmetric encryption algorithm
                RijndaelManaged AES = new RijndaelManaged();
                AES.KeySize = 256;
                AES.BlockSize = 128;
                AES.Padding = PaddingMode.PKCS7;

                //Hash the user password along with the salt
                var key = new Rfc2898DeriveBytes(passwordBytes, salt, 50000);
                AES.Key = key.GetBytes(AES.KeySize / 8);
                AES.IV = key.GetBytes(AES.BlockSize / 8);

               
                AES.Mode = CipherMode.CFB;

                //write salt to the begining of the output file, so in this case can be random every time
                fsCrypt.Write(salt, 0, salt.Length);

                CryptoStream cs = new CryptoStream(fsCrypt, AES.CreateEncryptor(), CryptoStreamMode.Write);

                FileStream fsIn = new FileStream(inputFile, FileMode.Open);

                //create a buffeer. only this amount will allocate in the memory 
                byte[] buffer = new byte[1048576];
                int read;

                try
                {
                    while ((read = fsIn.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        Application.DoEvents(); // -> for responsive GUI, using Task will be better!
                        cs.Write(buffer, 0, read);
                    }

                    //close up
                    fsIn.Close();

                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error: " + ex.Message);
                }
                finally
                {
                    cs.Close();
                    fsCrypt.Close();
                }

            }
        }
        //Method to generate a random salt value
        public static byte[] GenerateRandomSalt()
        {
           
            byte[] data = new byte[32];

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                // Ten iterations.
                for (int i = 0; i < 10; i++)
                {
                    // Fill buffer.
                    rng.GetBytes(data);
                }
            }
            return data;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            

            if (((txtDecFile.Text) == "") || ((txtDecPass.Text) == "")) //check if fields are empty
            {
                MessageBox.Show("Please choose a file and a password ", "Error! ", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

            else
            {

               
             AES_Decrypt(txtDecFile.Text,txtDecPass.Text); //calling the decryption algorithm 

              

                   txtDecFile.Text = null; //resetting the fields
                   txtDecPass.Text = null;
                   size2.Text = null;
              
               


            }
        }

        
        //writing the password to a text file 
        private static void WriteToFile(String File, String Psswd)
        {
        using (System.IO.StreamWriter file = 
            new System.IO.StreamWriter(@"C:\Users\Public\EncryptionTool\PasswordFile.txt", true))
        
            file.WriteLine("File name :"+File+"  Password : "+Psswd);
        }

        
        //Decryptoin Method 
        private static void AES_Decrypt(string inputFile, string password)
        {
          

            {
                string ext = inputFile.Split('.')[1]; //Substring the file extension 
                
 
                byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);//get the password into a byte array
                byte[] salt = new byte[32]; 

                FileStream fsCrypt = new FileStream(inputFile, FileMode.Open); //get the encrypted file
                fsCrypt.Read(salt, 0, salt.Length);

                RijndaelManaged AES = new RijndaelManaged();
                AES.KeySize = 256;
                AES.BlockSize = 128;
                var key = new Rfc2898DeriveBytes(passwordBytes, salt, 50000);
                AES.Key = key.GetBytes(AES.KeySize / 8);
                AES.IV = key.GetBytes(AES.BlockSize / 8);
                AES.Padding = PaddingMode.PKCS7;
                AES.Mode = CipherMode.CFB;

                CryptoStream cs = new CryptoStream(fsCrypt, AES.CreateDecryptor(), CryptoStreamMode.Read);

                FileStream fsOut = new FileStream(inputFile + "."+ext, FileMode.Create); //Set the file extension of the decrypted file  and create a file stream

                int read;
                byte[] buffer = new byte[1048576];

                try
                {
                    while ((read = cs.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        Application.DoEvents();
                        fsOut.Write(buffer, 0, read);
                    }   


                    MessageBox.Show("File Decrypted", "Decryption Completed", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                catch (System.Security.Cryptography.CryptographicException ex_CryptographicException)
                {
                   
                    MessageBox.Show("Incorrect Password ");  //Cryptographic Error Prompt ( incorrect Password ) 
                }
                catch (Exception ex)
                {
                   // MessageBox.Show("Error: " + ex.Message);
                }

                try
                {
                    cs.Close();
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Error by closing CryptoStream: " + ex.Message);
                }
                finally
                {
                    fsOut.Close();
                    fsCrypt.Close();
                }
            }
           



        }

        private void button8_Click(object sender, EventArgs e)
         {
            //Display File size
            openFileDialog2.ShowDialog();
            txtDecFile.Text = openFileDialog2.FileName;
            try
            {

                FileInfo FI = new FileInfo(openFileDialog2.FileName);
                float size = (FI.Length / 1024f); //get the file size
                string outString1 = size.ToString("####0.00");
                size2.Text = " File size : " + outString1 + " KB ";
            }
            catch (Exception ex)
                {
                }
            

        }

        private void button7_Click(object sender, EventArgs e)
        {
          
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e) //Mask and unmask password field
        {
            if (checkBox2.Checked)  
            {
                txtDecPass.PasswordChar = '\0';
            }
            else
            {
                txtDecPass.PasswordChar = '*';
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
          
            txtDecFile.Text = null;
            txtDecPass.Text = null;
            size2.Text = null;

        }

        private void button4_Click(object sender, EventArgs e)
        {
            
            txtFile.Text = null;
            txtPass.Text = null;
            size1.Text = null;

        }

        private void button9_Click(object sender, EventArgs e)
        {
            

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {
           
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"C:\Users\Public\EncryptionTool");

        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://prot3ct3d.wordpress.com");

        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            
        }

        private void label8_Click_1(object sender, EventArgs e)
        {

        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            string f1 = txtDecFile.Text.Split('.')[0];
            MessageBox.Show(f1);
        }

        private void openFileDialog1_FileOk_1(object sender, CancelEventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

       




       
    }
}
