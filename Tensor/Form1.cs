using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tensor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        BackgroundWorker worker=new BackgroundWorker();
        private Image _imagen;

        private void button1_Click(object sender, EventArgs e)
        {
            using (var open=new OpenFileDialog())
            {
                open.Filter = "(*.*)|*.*";
                open.ShowDialog();
                if (!string.IsNullOrWhiteSpace(open.FileName))
                {
                   //pictureBox1.
                    pictureBox1.Image = Image.FromFile(open.FileName);
                }

                
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
           

           

        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pictureBox1.Image = _imagen;
            Cursor = Cursors.Default;
            MessageBox.Show("Finish");
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Detector d = new Detector();
            _imagen = pictureBox1.Image;
            d.Detect(ref _imagen);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            outputTextbox.Text = Utils.DownloadDefaultTexts();
            outputTextbox.Text += Environment.NewLine + Utils.DownloadDefaultModel();
            Cursor = Cursors.Default;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
               worker.RunWorkerAsync();
              

            }
            catch (Exception exception)
            {
                Cursor = Cursors.Default;
                MessageBox.Show(exception.Message);
            }
        }
    }
}
