using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Gecko;
using Newtonsoft.Json;
namespace QZone
{

    public partial class Yellows : Form
    {
        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr FindFirstUrlCacheEntry([MarshalAs(UnmanagedType.LPTStr)] string lpszUrlSearchPattern, IntPtr lpFirstCacheEntryInfo, ref int lpdwFirstCacheEntryInfoBufferSize);
        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool FindNextUrlCacheEntry(IntPtr hEnumHandle, IntPtr lpNextCacheEntryInfo, ref int lpdwNextCacheEntryInfoBufferSize);
        [DllImport("wininet.dll")]
        public static extern bool FindCloseUrlCache(IntPtr hEnumHandle);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int FileTimeToSystemTime(IntPtr lpFileTime, IntPtr lpSystemTime);
        private const int ERROR_NO_MORE_ITEMS = 259;
        private int y = 0;
        private bool caiji = true;
        private string qq;
        private int counter = 0;
        private string line;
        private string x;
        static private string testUrl = "http://user.qzone.qq.com/你的QQ号";

        public Yellows()
        {
            InitializeComponent();
            //====================
            GeckoWebBrowser.UseCustomPrompt();
            string xulrunnerPath = Application.StartupPath + "\\xulrunner";
            Xpcom.Initialize(xulrunnerPath);
            GeckoPreferences.User["gfx.font_rendering.graphite.enabled"] = true;
            Application.ApplicationExit += (sender, e) => Xpcom.Shutdown();
            geckoWebBrowser1.Navigate(testUrl); 
        }


        private void btn_Fangwen_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text == "")
            {
                MessageBox.Show("先登陆空间");
            }
            this.geckoWebBrowser1.Navigate(this.textBox1.Text);
        }
        private string FILETIMEtoDataTime(FILETIME time)
        {
            IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(FILETIME)));
            IntPtr intPtr2 = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Yellows.SYSTEMTIME)));
            Marshal.StructureToPtr(time, intPtr, true);
            Yellows.FileTimeToSystemTime(intPtr, intPtr2);
            Yellows.SYSTEMTIME sYSTEMTIME = (Yellows.SYSTEMTIME)Marshal.PtrToStructure(intPtr2, typeof(Yellows.SYSTEMTIME));
            return string.Concat(new string[]
			{
				sYSTEMTIME.wYear.ToString(),
				".",
				sYSTEMTIME.wMonth.ToString(),
				".",
				sYSTEMTIME.wDay.ToString(),
				".",
				sYSTEMTIME.wHour.ToString(),
				".",
				sYSTEMTIME.wMinute.ToString(),
				".",
				sYSTEMTIME.wSecond.ToString()
			});
        }
        public struct SYSTEMTIME
        {
            public short wYear;
            public short wMonth;
            public short wDayOfWeek;
            public short wDay;
            public short wHour;
            public short wMinute;
            public short wSecond;
            public short wMilliseconds;
        }
        public struct INTERNET_CACHE_ENTRY_INFO
        {
            public int dwStructSize;
            public IntPtr lpszSourceUrlName;
            public IntPtr lpszLocalFileName;
            public int CacheEntryType;
            public int dwUseCount;
            public int dwHitRate;
            public int dwSizeLow;
            public int dwSizeHigh;
            public FILETIME LastModifiedTime;
            public FILETIME ExpireTime;
            public FILETIME LastAccessTime;
            public FILETIME LastSyncTime;
            public IntPtr lpHeaderInfo;
            public int dwHeaderInfoSize;
            public IntPtr lpszFileExtension;
            public int dwExemptDelta;
        }

        private void Btn_GetCookie_Click(object sender, EventArgs e)
        {
            if (this.geckoWebBrowser1.Url.ToString() == "http://qzone.qq.com/")
            {
                MessageBox.Show("先登陆QQ空间");
                MessageBox.Show("下侧滚动条右移点登陆");
                this.textBox1.Text = "http://user.qzone.qq.com/";
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

        private void FileOk_Click()
        {
            int num = 0;
            Yellows.FindFirstUrlCacheEntry(null, IntPtr.Zero, ref num);
            if (Marshal.GetLastWin32Error() != 259)
            {
                int num2 = num;
                IntPtr intPtr = Marshal.AllocHGlobal(num2);
                IntPtr hEnumHandle = Yellows.FindFirstUrlCacheEntry(null, intPtr, ref num);
                bool flag = true;
                while (flag)
                {
                    Yellows.INTERNET_CACHE_ENTRY_INFO iNTERNET_CACHE_ENTRY_INFO = (Yellows.INTERNET_CACHE_ENTRY_INFO)Marshal.PtrToStructure(intPtr, typeof(Yellows.INTERNET_CACHE_ENTRY_INFO));
                    string text = this.FILETIMEtoDataTime(iNTERNET_CACHE_ENTRY_INFO.LastModifiedTime);
                    string text2 = this.FILETIMEtoDataTime(iNTERNET_CACHE_ENTRY_INFO.ExpireTime);
                    string text3 = this.FILETIMEtoDataTime(iNTERNET_CACHE_ENTRY_INFO.LastAccessTime);
                    string text4 = this.FILETIMEtoDataTime(iNTERNET_CACHE_ENTRY_INFO.LastSyncTime);
                    try
                    {
                        string text5 = Marshal.PtrToStringAuto(iNTERNET_CACHE_ENTRY_INFO.lpszSourceUrlName);
                        bool flag2 = text5.Contains("g_tk=");
                        bool flag3 = text5.Contains(this.qq);
                        if (flag2 & flag3)
                        {
                            Regex regex = new Regex("g_tk=[0-9]{6,13}");
                            MatchCollection matchCollection = regex.Matches(text5);
                            if (matchCollection.Count != 0)
                            {
                                flag = false;
                                this.textBox2.Text = matchCollection[0].Value.Replace("g_tk=", "");
                                MessageBox.Show("提取成功！");
                                this.textBox1.Text = "http://user.qzone.qq.com/";
                            }
                            else
                            {
                                this.geckoWebBrowser1.Refresh();
                                this.FileOk_Click();
                            }
                            flag = false;
                            break;
                        }
                    }
                    catch
                    {
                    }
                    num = num2;
                    bool flag4 = Yellows.FindNextUrlCacheEntry(hEnumHandle, intPtr, ref num);
                    if (!flag4 && Marshal.GetLastWin32Error() == 259)
                    {
                        break;
                    }
                    if (!flag4 && num > num2)
                    {
                        num2 = num;
                        intPtr = Marshal.ReAllocHGlobal(intPtr, (IntPtr)num2);
                        Yellows.FindNextUrlCacheEntry(hEnumHandle, intPtr, ref num);
                    }
                }
                Marshal.FreeHGlobal(intPtr);
            }
        }

        void Getskey()
        {
            
        }

        private void btn_Ok_Click(object sender, EventArgs e)
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
                        string arg = "http://g.qzone.qq.com/cgi-bin/friendshow/cgi_get_visitor_simple?uin=" + this.listBox2.Items[this.y].ToString() + "&mask=2&g_tk=" + this.textBox2.Text;
                        geckoWebBrowser1.Navigate(arg + "&" + DateTime.Now.Ticks);
                        this.textBox1.Text = string.Concat(new object[]
				        {
					        "http://g.qzone.qq.com/cgi-bin/friendshow/cgi_get_visitor_simple?uin=",
					        this.listBox2.Items[this.y].ToString(),
					        "&mask=2&g_tk=",
					        this.textBox2.Text,
					        "&",
					        DateTime.Now.Ticks
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

        private void button1_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.ShowDialog();
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
        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.listBox2.Items.Count; i++)
            {
                this.listBox2.Items.RemoveAt(i);
            }
            this.y = 0;
        }
        public static void DeleteFile(string dirRoot)
        {
            try
            {
                string[] directories = Directory.GetDirectories(dirRoot);
                string[] files = Directory.GetFiles(dirRoot);
                string[] array = files;
                for (int i = 0; i < array.Length; i++)
                {
                    string path = array[i];
                    File.Delete(path);
                }
                array = directories;
                for (int i = 0; i < array.Length; i++)
                {
                    string dirRoot2 = array[i];
                    Yellows.DeleteFile(dirRoot2);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.textBox1.Text = "http://g.qzone.qq.com/cgi-bin/friendshow/cgi_get_visitor_simple?uin=" + this.listBox2.SelectedItem.ToString() + "&mask=2&g_tk=" + this.textBox2.Text;
            }
            catch
            {
            }
        }

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
            this.btn_Ok_Click(this, e);
        }
        private void tj(string tj1, string tj2,string tj3)
        {
            bool flag = false;
            for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
            {
                try
                {
                    if (this.dataGridView1.Rows[i].Cells[0].Value.ToString() == tj1)
                    {
                        flag = true;
                    }
                }
                catch
                {
                }
            }
            if (!flag)
            {
                if (this.textBox4.Text != "" & this.checkBox3.Checked)
                {
                    int num = this.textBox4.Lines.Length;
                    for (int j = 0; j < num; j++)
                    {
                        if (this.textBox4.Lines[j].ToString() != "")
                        {
                            if (tj2.Contains(this.textBox4.Lines[j].ToString()))
                            {
                                this.dataGridView1.Rows.Add(new object[]
								{
									tj1,
									tj2,
                                    Pub.TimeStamp(tj3)
								});
                                break;
                            }
                        }
                    }
                }
                else
                {
                    this.dataGridView1.Rows.Add(new object[]
					{
						tj1,
						tj2,
                        Pub.TimeStamp(tj3)
					});
                }
            }
            flag = false;
            this.label3.Text = "状态 :当前共" + (this.dataGridView1.Rows.Count - 1).ToString() + "个";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Text = testUrl;
        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {

        }
    }
}
