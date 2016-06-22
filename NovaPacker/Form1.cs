using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime;
using System.Reflection;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace NovaPacker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Packer.PackAndroid((int p) =>
            {
                progressBar1.Maximum = 100;
                progressBar1.Value = p;
                label4.Text = p + "%";
                label3.Text = DateTime.Now.ToShortTimeString();
            }, true);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Packer.PackWindows((int p) =>
            {
                progressBar1.Maximum = 100;
                progressBar1.Value = p;
                label4.Text = p + "%";
                label3.Text = DateTime.Now.ToShortTimeString();
            }, true);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            bool r = false;
            if (Packer.ClearAndExportAllCsvText("Windows")
                && Packer.CopyAllCsvCs("Windows", "Android")
                && Packer.CopyAllCsvCs("Windows", "IOS"))
            {
                MessageBox.Show("成功");
            }
            else
                MessageBox.Show("导表失败，请检查日志");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog d = new OpenFileDialog();
            d.FileName = Packer.UnityPath;
            if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                label1.Text = d.FileName;
                Packer.UnityPath = d.FileName;
                SavePath();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog d = new FolderBrowserDialog();
            d.SelectedPath = Packer.ProjectPath;
            if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                label2.Text = d.SelectedPath;
                Packer.ProjectPath = d.SelectedPath;
                SavePath();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadPath();
        }

        void LoadPath()
        {
            if (System.IO.File.Exists("path.txt"))
            {
                System.IO.StreamReader r = new System.IO.StreamReader("path.txt");
                Packer.UnityPath = r.ReadLine();
                Packer.ProjectPath = r.ReadLine();
                r.Close();
                label1.Text = Packer.UnityPath;
                label2.Text = Packer.ProjectPath;
            }
        }

        void SavePath()
        {
            System.IO.StreamWriter w = new System.IO.StreamWriter("path.txt", false);
            w.WriteLine(Packer.UnityPath);
            w.WriteLine(Packer.ProjectPath);
            w.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Packer.CompileServer();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Packer.UpdateSVN();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Packer.PackIOS((int p) =>
            {
                progressBar1.Maximum = 100;
                progressBar1.Value = p;
                label4.Text = p + "%";
                label3.Text = DateTime.Now.ToShortTimeString();
            }, true);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (Packer.ClearAndExportAllCsvText("Android"))
                MessageBox.Show("成功");
            else
                MessageBox.Show("导表失败，请检查日志");
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (Packer.ClearAndExportAllCsvText("IOS"))
                MessageBox.Show("成功");
            else
                MessageBox.Show("导表失败，请检查日志");
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (Packer.ClearAndExportAllCsvText("Windows"))
                MessageBox.Show("成功");
            else
                MessageBox.Show("导表失败，请检查日志");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Packer.version = textBox1.Text;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (!Packer.DoInfoPlistModifier())
                MessageBox.Show("执行 InfoPlistModifier 失败");
            else
                MessageBox.Show("执行 InfoPlistModifier 成功");
        }
    }
}
