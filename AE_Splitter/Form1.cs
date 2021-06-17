using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace AE_Splitter
{
    public partial class Form1 : Form
    {

        string FileToSplit;
        string FolderToSplit;

        string FileToGlue;
        string FolderToGlue;

        public Form1()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedItem = "kb";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog _dialog_sp = new OpenFileDialog();
            if (_dialog_sp.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = _dialog_sp.FileName;
                FileToSplit = _dialog_sp.FileName;
                FolderToSplit = _dialog_sp.FileName.Replace(_dialog_sp.SafeFileName, "");
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if(FileToSplit != null)
            {
                button2.Enabled = false;

                int size = 1024;

                switch (comboBox1.SelectedIndex)
                {
                    case 0:
                        size *= 1;
                        break;
                    case 1:
                        size *= 1024;
                        break;
                }

                size *= ((int)numericUpDown1.Value);

                await Cutter.SplitFile(FileToSplit, size, FolderToSplit);

                button2.Enabled = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog _dialog_sp = new OpenFileDialog();
            if (_dialog_sp.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = _dialog_sp.FileName;
                FileToGlue = _dialog_sp.FileName;
                FolderToGlue = _dialog_sp.FileName.Replace(_dialog_sp.SafeFileName, "");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (FileToGlue != null)
            {
                button4.Enabled = false;

                char[] separators = new char[] {'.'};
                string[] _splitfile = FileToGlue.Replace(FolderToGlue, "").Split(separators, StringSplitOptions.None);

                int maxCount = 0;
                string fileType = null;
                int istCount = 0;
                string originalFile = null;

                try
                {
                    if(_splitfile.Length > 3)
                    {
                        maxCount = Convert.ToInt32(_splitfile[_splitfile.Length - 1]);
                        fileType = _splitfile[_splitfile.Length - 2];
                        istCount = Convert.ToInt32(_splitfile[_splitfile.Length - 3]);

                        for (int i = 0; i < _splitfile.Length-3; i++)
                        {
                            if(originalFile == null)
                            {
                                originalFile += _splitfile[i];
                            }
                            else
                            {
                                originalFile += "." + _splitfile[i];
                            }
                            
                        }

                        
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    button4.Enabled = true;
                    return;
                }

                List<byte[]> _allBytes = new List<byte[]>();
                int _readBytes = 0;

                for (int i = 0; i < maxCount; i++)
                {
                    if(File.Exists(FolderToGlue + originalFile + "." + i + "." + fileType + "." + maxCount))
                    {
                        using (Stream input = File.OpenRead(FolderToGlue + originalFile + "." + i + "." + fileType + "." + maxCount))
                        {
                            byte[] allread = new byte[input.Length];
                            _readBytes += input.Read(allread, 0, allread.Length);

                            _allBytes.Add(allread);
                        }
                    }
                    else
                    {
                        MessageBox.Show(FolderToGlue + originalFile + "." + i + "." + fileType + "." + maxCount + " nicht gefudnen.");
                        button4.Enabled = true;
                        return;
                    }
                }

                if(_readBytes > 0)
                {
                    byte[] _originalFile = new byte[_readBytes];
                    int _position = 0;
                    for (int i = 0; i < _allBytes.Count(); i++)
                    {
                        for (int i2 = 0; i2 < _allBytes[i].Length; i2++)
                        {
                            _originalFile[_position] = _allBytes[i][i2];
                            _position++;
                        }
                    }

                    using (Stream output = File.Create(FolderToGlue + originalFile))
                    {
                        output.Write(_originalFile, 0, _originalFile.Length);
                    }
                }

                button4.Enabled = true;
            }
        }
    }
}
