using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Gecko;

namespace QZone
{
    public partial class QQPhone : Form
    {
        private const string QQPHOTO = "http://p{0}.photo.qq.com/{1}/16"; //{0} = {1} % 13 + 1 {1}为qq号
        //取相册的另一个有用路径，在第一个可用的情况下用。
        private const string QQPHOTO_B = "http://photo.qq.com/cgi-bin/common/cgi_list_album?uin={0}";
        private const string ALBUMURL = "http://sz.photo.store.qq.com/http_staload.cgi?{0}/{1}"; //{0}qq号 {1}album号
        //  
        private int y = 0;
        private bool caiji = true;
        private string qq;
        private int counter = 0;
        private string line;
        private string x;
        public QQPhone()
        {
            InitializeComponent();
            //
            GeckoWebBrowser.UseCustomPrompt();
            string xulrunnerPath = Application.StartupPath + "\\xulrunner";
            Xpcom.Initialize(xulrunnerPath);
            GeckoPreferences.User["gfx.font_rendering.graphite.enabled"] = true;
            Application.ApplicationExit += (sender, e) => Xpcom.Shutdown();
            geckoWebBrowser1.Navigate("http://i.qq.com"); 
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (this.listBox2.Items.Count == 0)
            {
                MessageBox.Show("先导入文本文件，一行一个号码");
            }
            else
            {
                if (!this.caiji)
                {
                    this.y = 0;
                    this.caiji = true;
                    this.timer1.Stop();
                }
                else
                {
                    try
                    {
                        string qq = this.listBox2.Items[this.y].ToString();
                        string arg = string.Format(QQPHOTO, (int.Parse(qq) % 13 + 1), qq);
                        
                        geckoWebBrowser1.Navigate(arg + "&" + DateTime.Now.Ticks);
                        this.txtUrl.Text = string.Concat(new object[]
				        {
					        arg
				        });
                        this.labmsg.Text = "正在采集第" + (this.y + 1).ToString() + "个";
                    }
                    catch
                    {
                        this.timer1.Stop();
                    }
                    this.y++;
                    if (this.checkBox2.Checked & this.y == this.listBox2.Items.Count)
                    {
                        this.y = 0;
                    }
                    if (!this.checkBox2.Checked & this.y == this.listBox2.Items.Count)
                    {
                        MessageBox.Show("采集完成");
                        this.caiji = false;
                    }
                    this.timer1.Interval = Convert.ToInt32(this.textBox3.Text);
                    this.timer1.Start();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.listBox2.Items.Count; i++)
            {
                this.listBox2.Items.RemoveAt(i);
            }
            this.y = 0;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (this.geckoWebBrowser1.Url.ToString() == "http://qzone.qq.com/")
            {
                MessageBox.Show("先登陆QQ空间");
                MessageBox.Show("下侧滚动条右移点登陆");
                this.textBox2.Text = "http://user.qzone.qq.com/";
            }
            else
            {
                if (this.textBox2.Text == "")
                {
                    try
                    {
                        this.qq = this.geckoWebBrowser1.Url.ToString().Replace("http://user.qzone.qq.com/", "");
                        //this.FileOk_Click();
                        geckoWebBrowser1.Navigate("http://user.qzone.qq.com/troubleshooter/");
                        //this.textBox1.Text = "http://g.qzone.qq.com/cgi-bin/friendshow/cgi_get_visitor_simple?uin=" + this.qq + "&mask=2&g_tk=" + this.textBox2.Text;
                        //geckoWebBrowser1.Navigate(this.textBox1.Text);

                    }
                    catch
                    {
                        MessageBox.Show("先登陆空间");
                    }
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string documentText = this.geckoWebBrowser1.Document.Body.InnerHtml;
            Regex regex = new Regex("uin\":[0-9]{7,12}");
            MatchCollection matchCollection = regex.Matches(documentText);
            Regex regex2 = new Regex("name\":.*\"");
            MatchCollection matchCollection2 = regex2.Matches(documentText);
            Regex regex3 = new Regex("time\":[0-9]{10}");
            MatchCollection matchCollection3 = regex3.Matches(documentText);
            for (int i = 0; i < matchCollection.Count; i++)
            {
                string text = matchCollection2[i].ToString().Replace("name\":", "");
                text = text.Replace("\"", "");
                this.tj(matchCollection[i].ToString().Replace("uin\":", ""), text, matchCollection3[i].ToString().Replace("time\":", ""));
            }
            this.timer1.Stop();
            this.button4_Click(this, e);
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            StreamReader streamReader = new StreamReader(this.openFileDialog1.FileName);
            while ((this.line = streamReader.ReadLine()) != null)
            {
                this.listBox2.Items.Add(this.line);
                this.counter++;
            }
            streamReader.Close();
        }
        private void tj(string tj1, string tj2,string tj3)
        { }

        private void geckoWebBrowser1_DocumentCompleted(object sender, Gecko.Events.GeckoDocumentCompletedEventArgs e)
        {
            if (geckoWebBrowser1.Document.Title != "QQ空间小助手")
                return;
            string documentText = this.geckoWebBrowser1.Document.Body.InnerHtml;
            Regex regex = new Regex("skey=@[a-zA-Z0-9]{9}");
            MatchCollection matchCollection = regex.Matches(documentText);
            for (int i = 0; i < matchCollection.Count; i++)
            {
                string text = matchCollection[i].ToString().Replace("skey=", "");
                textBox2.Text = Pub.sKey(text);
            }
        }
    }
}
