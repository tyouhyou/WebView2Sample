using System;
using System.Threading;
using System.Windows.Forms;

namespace WebView2Sample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            var browser = new MyBrowser();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            myBrowser1.Initialize();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            myBrowser1.Navigate("http://google.com");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            myBrowser1.Initialize();
            myBrowser1.Navigate("http://google.com");
            //Thread.Sleep(5000); // main thread blocking
            myBrowser1.Url = new Uri("http://bing.com");
        }
    }
}
