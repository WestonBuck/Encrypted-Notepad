///////////////////////////////////////////////////////////
//
// Weston Buck
// Encryption Notepad project
// 
// Help from:
// http://www.amfastech.com/2015/01/making-notepad-application-in-visualstudio-2013.html
// Microsoft's MSDN website: Various pages about AES encryption.
//
// Bugs: Doesn't decrypt, error: specified key is not a valid size for this algorithm
// can't check if it encrypts correctly because there is no decryption.
///////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography; // for aes and sha functionality
using Microsoft.VisualBasic;    // for inputdialog box so i didn't have to make my own.

namespace NotepadProject
{
    public partial class Form1 : Form
    {
        RichTextBox textBox;

        public Form1()
        {
            InitializeComponent();

            //create new tab for user when they start the program.
            TabPage newTab = new TabPage("New Document");
            textBox = new RichTextBox();
            textBox.Dock = DockStyle.Fill;
            newTab.Controls.Add(textBox);
            tabControl1.TabPages.Add(newTab);
            textBox.Select();
        }

        private RichTextBox GetRichTextBox()
        {
            textBox = null;
            TabPage tab = tabControl1.SelectedTab;
            if (tab != null)
            {
                textBox = tab.Controls[0] as RichTextBox;
            }
            return textBox;
        }

        public static string getHashSha256(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            SHA256Managed hashstring = new SHA256Managed();
            byte[] hash = hashstring.ComputeHash(bytes);// get hashed bytes
            string hashString = string.Empty;
            foreach (byte x in hash)                    // put each hashed byte into a string to be returned.
            {
                hashString += String.Format("{0:x2}", x);
            }

            return hashString;
        }


        static byte[] EncryptString(string plainText)
        {
            byte[] encrypted;
            // Create an Aes object
            using (Aes aesNew = Aes.Create())
            {
                ICryptoTransform encryptor = aesNew.CreateEncryptor(aesNew.Key, aesNew.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }


            // Return the encrypted bytes from the memory stream.
            return encrypted;

        }

        // decrypt example from msdn site. //error: specified key is not a valid size for this algorithm
        static string Decrypt(byte[] cipherText, byte[] Key, byte[] IV)
        {

            // Declare the string used to hold
            // the decrypted text.
            string text = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesNew = Aes.Create())
            {
                aesNew.Key = Key;
                aesNew.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesNew.CreateDecryptor(aesNew.Key, aesNew.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            text = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return text;

        }
        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Create new tab, make the new tab fill the screen and add it to tabs .
            TabPage newTab = new TabPage("New Document");
            textBox = new RichTextBox();
            textBox.Dock = DockStyle.Fill;
            newTab.Controls.Add(textBox);
            tabControl1.TabPages.Add(newTab);
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetRichTextBox().Cut(); //built in cut function
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetRichTextBox().Copy(); //built in copy function
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetRichTextBox().Paste(); // built in paste function
        }

        // open file to decrypt. Commented out code is what I had been working on so that it'll be able to open files without crashing.
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // string userPassword;
            Stream newStream;
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if ((newStream = openFileDialog1.OpenFile()) != null)
                {
                   // userPassword = Interaction.InputBox("Enter your key to decrypt this file!", "Password", "Enter Password Here");
                   // string userIV = Interaction.InputBox("Enter your IV", "Password", "Enter IV Here");
                    string filepath = openFileDialog1.FileName;//copies the path of the file into a variable
                    string readfiletext = File.ReadAllText(filepath);//reads all the text from the opened file

                    //code to try an specify a specifc key and IV to try and get the decrypt function to work, it didn't.
                    //byte[] Key = Encoding.UTF8.GetBytes(userPassword);
                    // byte[] IV = Encoding.UTF8.GetBytes(userIV);
                    //byte[] text = Encoding.UTF8.GetBytes(readfiletext);
                    //string decrytedText = Decrypt(text, Key, IV);           //Not working error = specified key is not a valid size for this algorithm

                    //create new tab for opened file.
                    TabPage newTab = new TabPage(openFileDialog1.FileName); 
                    textBox = new RichTextBox(); 
                    textBox.Dock = DockStyle.Fill;  
                    newTab.Controls.Add(textBox); 
                    tabControl1.TabPages.Add(newTab); 
                    textBox.Text = readfiletext;


                }
            }
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string userPassword, userPasswordhashed;
            DialogResult DialogBox = MessageBox.Show("Would you like to encrypt this file?", "Confirmation", MessageBoxButtons.YesNoCancel);

            //hash a user password if user chooses yes
            if (DialogBox == DialogResult.Yes)
            {
                //hash password using sha256. Doesn't save sha256 with file becasue I didn't get decryption working yet.
                userPassword = Interaction.InputBox("Enter a password to get hashed", "Password", "Enter Password Here");
                userPasswordhashed = getHashSha256(userPassword); 

                //encrypt whats in the textbox
                byte [] Encrypted = EncryptString(textBox.Text);
                string textEncrypt = System.Text.Encoding.UTF8.GetString(Encrypted);

                //open windows explorer to save file.
                SaveFileDialog savefile1 = new SaveFileDialog();
                savefile1.Filter = "*.txt(textfile)|*.txt";
                if (savefile1.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(savefile1.FileName, textEncrypt);
                   
                }
            }

            //else just save a normal unencrypted file
            else if (DialogBox == DialogResult.No)
            {
                SaveFileDialog savefile2 = new SaveFileDialog();
                savefile2.Filter = "*.txt(textfile)|*.txt";
                if (savefile2.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(savefile2.FileName, textBox.Text);
                }
            }           
        }

        private void closeTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.TabPages.Remove(tabControl1.SelectedTab);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        //renames the tab if a user wants, not needed for assignment, just bugged me that it didn't allow user to change name of a tab.
        private void renameTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            string text="";
            text = Interaction.InputBox("Enter a new name for the tab", "Tab Name", "");
            tabControl1.SelectedTab.Text = text;
        }

       
    }
}
