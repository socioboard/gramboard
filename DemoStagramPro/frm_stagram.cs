
#region namespace
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Globussoft;
using DemoStagramPro.Classes;
using InstagramManager.Classes;
using BaseLib;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing.Imaging;
using Input=Microsoft.VisualBasic.Interaction;
using StagramProxyChecker.classes;
using BaseLib;
using log4net.Config;
using System.Net;
using Microsoft.VisualBasic.CompilerServices;

#endregion


namespace DemoStagramPro
{
    public partial class frm_stagram : Form
    {
        static readonly Object _lockObject = new Object();

        public static bool _boolAddToLogger = false;
        public static readonly object lockerforProxies = new object();
        public static readonly object lockerforNonWorkingProxies = new object();
        List<string> lstCountImagedata = new List<string>();

        public frm_stagram()
        {
            // listBox1._frm_stagram = this;
            XmlConfigurator.Configure();
            InitializeComponent();
        }

        string FileDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\imageuploadProDataDirectory";
        public static string FileData = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\imageuploade\\LoggerError.txt";
        public static string Error = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\imageuploadProDataDirectory\\LoggerError.txt";

        public string LoggerProperty
        {
            set
            {
                this.lstHashLogger.Invoke(new MethodInvoker(delegate() { DoLoggerWork(value); }));
            }
        }

        public string LoggerImageProperty
        {
            set
            {
                this.lstImageLogger.Invoke(new MethodInvoker(delegate() { DoLoggerWorkForImage(value); }));
            }
        }

        private void DoLoggerWork(string log)
        {
            try
            {
                if (lstHashLogger.InvokeRequired)
                {
                    lstHashLogger.Invoke(new MethodInvoker(delegate
                    {
                        lstHashLogger.Items.Add(log);
                        lstHashLogger.SelectedIndex = lstHashLogger.Items.Count - 1;
                    }));
                }
                else
                {
                    lstHashLogger.Items.Add(log);
                    lstHashLogger.SelectedIndex = lstHashLogger.Items.Count - 1;
                }
            }
            catch
            {
                AddToLogger("[" + DateTime.Now + "]=>[DoLoggerWork");
            }
        }

        private void DoLoggerWorkForImage(string log)
        {
            try
            {
                if (lstImageLogger.InvokeRequired)
                {
                    lstImageLogger.Invoke(new MethodInvoker(delegate
                    {
                        lstImageLogger.Items.Add(log);
                        lstImageLogger.SelectedIndex = lstImageLogger.Items.Count - 1;
                    }));
                }
                else
                {
                    lstImageLogger.Items.Add(log);
                    lstImageLogger.SelectedIndex = lstImageLogger.Items.Count - 1;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        Bitmap Bacgroundimage;
        Bitmap PanelImage;
        List<Thread> Lst_Commentthread = new List<Thread>();
        bool CommentIstrueOrFals = false;
        int Maxthread = 1;
        int mindelay = 0;
        int maxdelay = 1;
      

        private void Form1_Load(object sender, EventArgs e)
        {
            CopyDatabase();
           
            System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
            ToolTip1.SetToolTip(this.txt_FollowUser, "Enter single Username or mutiple separated by comma to Follow");

            System.Windows.Forms.ToolTip ToolTip2 = new System.Windows.Forms.ToolTip();
            ToolTip1.SetToolTip(this.txtUnfollowSingle, "Enter single Username or mutiple separated by comma to UnFollow");

            System.Windows.Forms.ToolTip ToolTip3 = new System.Windows.Forms.ToolTip();
            ToolTip1.SetToolTip(this.txtphotosingaluser, "Enter Single Photo_id or multiple separated by commas to like pics");

            System.Windows.Forms.ToolTip ToolTip4 = new System.Windows.Forms.ToolTip();
            ToolTip1.SetToolTip(this.txtUploadSingleItem, "Enter single Username or mutiple separated by commas to download pics");

            System.Windows.Forms.ToolTip ToolTip5 = new System.Windows.Forms.ToolTip();
            ToolTip1.SetToolTip(this.txtusercomment, "Enter single photo_id or multiple separated by commas to comment");

            System.Windows.Forms.ToolTip ToolTip6 = new System.Windows.Forms.ToolTip();
            ToolTip1.SetToolTip(this.txtuploadUserScrper, "Give User name single or multiple to fetch details");



            //this.ActiveControl = label59;
            //txtphotosingaluser.GotFocus += new EventHandler(this.TextGotFocus);
            //txtphotosingaluser.LostFocus += new EventHandler(this.TextLostFocus);
            dgvFollowers.ForeColor = Color.Black;
            dgvAccount.ForeColor = Color.Black;
            
            loadAccount();
            loadComboboxData();
            loaddatafmdb();
            this.DoubleBuffered = true;
            this.BackColor = Color.Black;

            new Thread(() =>
                    {
                        makeFile();
                        makeFileScraper();
                    }).Start();

            //Draw Image in Tab Controls..
            Bacgroundimage = Properties.Resources.Bacground;
            
            PanelImage = Properties.Resources.tra;
            tab_instagram.DrawMode = TabDrawMode.OwnerDrawFixed;
            tab_instagram.DrawItem += new DrawItemEventHandler(tab_instagram_DrawItem);

            //HIDE SCRAPE FOLLOWERS
            //tab_instagram.TabPages.Remove(tabScrapeFollowers);
            //
            //tab_instagram.TabPages.Remove(tabPage6);
            //tab_instagram.TabPages.Remove(tabPage2);
            //tab_instagram.TabPages.Remove(tabPage3);
            //tab_instagram.TabPages.Remove(tabPage4);
            //tab_instagram.TabPages.Remove(tabPage5);
            //tab_instagram.TabPages.Remove(tabPage7);
             tab_instagram.TabPages.Remove(tabPage8);//tabPage8

            #region Event Methods Subscription
            DemoStagramPro.Classes.HashTag.loggerHashTag.addToLogger += new EventHandler(HashTagLogEvents_addToLogger);
            #endregion
        }


        //public void TextGotFocus(object sender, EventArgs e)
        //{
        //    TextBox tb = (TextBox)sender;
        //    if (tb.Text == "Your TextBox.....")
        //    {
        //        tb.Text = "";
        //        tb.ForeColor = Color.Black;
        //    }

        //}

        //public void TextLostFocus(object sender, EventArgs e)
        //{
        //    TextBox tb = (TextBox)sender;
        //    if (tb.Text == " ")
        //    {
        //        tb.Text = "Your TextBox.....";
        //        tb.ForeColor = Color.LightGray;

        //    }
        //}


        public void texthold(object sender, EventArgs e)
        {
 
        }
        public void loaddatafmdb()
        {

            try
            {
                DataTable NewDt2 = CreateUrltable();
                QueryExecuter Qm = new QueryExecuter();
                DataSet ds = Qm.getAccount();





                DataTable dt = ds.Tables[0];
                List<string> BunchList = new List<string>();
                int itemCounter = 0;


                foreach (DataRow item in dt.Rows)
                {
                    try
                    {
                        DataRow newdr = NewDt2.NewRow();
                        string Username = item["Username"].ToString();
                        string Password = item["Password"].ToString();
                        string proxyAddress = item["IPAddress"].ToString();
                        string proxyPort = item["IPPort"].ToString();
                        string proxyUsername = item["IPUsername"].ToString();
                        string proxyPassword = item["IPPassword"].ToString();
                        string Path = item["Path"].ToString();




                        newdr["Username"] = Username;
                        newdr["Password"] = Password;
                        newdr["IPAddress"] = proxyAddress;
                        newdr["IPPort"] = proxyPort;
                        newdr["IPUsername"] = proxyUsername;
                        newdr["IPPassword"] = proxyPassword;
                        newdr["Path"] = Path;


                        ClGlobul.lstUrls.Add(Username);
                        ClGlobul.lstUrls.Add(Password);
                        ClGlobul.lstUrls.Add(proxyAddress);
                        ClGlobul.lstUrls.Add(proxyPort);
                        ClGlobul.lstUrls.Add(proxyUsername);
                        ClGlobul.lstUrls.Add(proxyPassword);
                        ClGlobul.lstUrls.Add(Path);

                        NewDt2.Rows.Add(newdr);
                    }
                    catch { };
                }

                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate
                    {
                        dgvAccount.DataSource = NewDt2;
                    }));
                }
                else
                {
                    dgvAccount.DataSource = NewDt2;
                }

            }
            catch { };

        }


        public DataTable CreateUrltable()
        {
            DataTable dt = new DataTable();
            try
            {
               
                DataColumn DataColumn = new DataColumn("Username");
                DataColumn DataColumn1 = new DataColumn("Password");
                DataColumn DataColumn2 = new DataColumn("IPAddress");
                DataColumn DataColumn3 = new DataColumn("IPPort");
                DataColumn DataColumn4 = new DataColumn("IPUsername");
                DataColumn DataColumn5 = new DataColumn("IPPassword");
                DataColumn DataColumn6 = new DataColumn("Path");



                dt.Columns.Add(DataColumn);
                dt.Columns.Add(DataColumn1);
                dt.Columns.Add(DataColumn2);
                dt.Columns.Add(DataColumn3);
                dt.Columns.Add(DataColumn4);
                dt.Columns.Add(DataColumn5);
                dt.Columns.Add(DataColumn6);

            }
            catch { };

            return dt;
        }

        public void loadComboboxData()
        {
            try
            {
                this.Invoke(new MethodInvoker(delegate
                    {
                        cmbScrapeFollowersUsername.Items.Clear();
                    }));
                this.Invoke(new MethodInvoker(delegate
                    {
                        cmbScrapeFollowersSelection.Items.Clear();
                        cmbScrapeFollowersSelection.Items.Add("Follower");
                        //cmbScrapeFollowersSelection.Items.Add("Following");
                        cmbScrapeFollowersSelection.SelectedIndex = 0;
                    }));
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => loadComboboxData :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
            }
        }
        public static string filepath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Gram BoardPro\\Downloaded_Image";
        public static string filepathScraper = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Gram BoardPro\\Scrape_Followers";
        public static string CSVPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Gram BoardPro\\Scrape_Followers\\";
        public void makeFile()
        {
            //Make Folder 
            try
            {
                if (!Directory.Exists(GramBoardProFileHelper.path_AppDataFolder))
                {
                    Directory.CreateDirectory(GramBoardProFileHelper.path_AppDataFolder);
                }
                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }

            }
            catch (Exception)
            {
            }
        }

        public void makeFileScraper()
        {
            //Make Folder 
            try
            {
                if (!Directory.Exists(GlobusFileHelper.path_AppDataFolder))
                {
                    Directory.CreateDirectory(GlobusFileHelper.path_AppDataFolder);
                }
                if (!Directory.Exists(filepathScraper))
                {
                    Directory.CreateDirectory(filepathScraper);
                }

            }
            catch (Exception)
            {
            }
        }




        public void CopyDatabase()                                                 //*--------Copy DataBase method--------*//
        {
            string startUpDB = System.Windows.Forms.Application.StartupPath + "\\GramBoardPro.db";
            string localAppDataDB = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\GramBoardPro\\GramBoardPro.db";
            string startUpDB64 = Environment.GetEnvironmentVariable("ProgramFiles(x86)") + "\\GramBoardPro.db";


            if (!File.Exists(localAppDataDB))
            {
                if (File.Exists(startUpDB))
                {
                    try
                    {
                        Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\GramBoardPro");
                        File.Copy(startUpDB, localAppDataDB);
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("Could not find a part of the path"))
                        {
                            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\GramBoardPro");
                            File.Copy(startUpDB, localAppDataDB);
                        }
                    }
                }
                else if (File.Exists(startUpDB64))   //for 64 Bit
                {
                    try
                    {
                        File.Copy(startUpDB64, localAppDataDB);
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("Could not find a part of the path"))
                        {
                            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\GramBoardPro.db");
                            File.Copy(startUpDB64, localAppDataDB);
                        }
                    }
                }
            }
        }

        public void loadAccount()
        {

            string path = string.Empty;
            try
            {
                QueryExecuter qm1 = new QueryExecuter();
                DataSet ds = qm1.getAccount();
                if (ds.Tables[0].Rows.Count != 0)
                {
                    try
                    {
                        path = ds.Tables[0].Rows[0].ItemArray[6].ToString();
                        if (!string.IsNullOrEmpty(path))
                        {
                            try
                            {
                                txtAddAccounts1.Text = path;
                            }
                            catch { };
                        }
                    }
                    catch { }

                    for (int noRow = 0; noRow < ds.Tables[0].Rows.Count; noRow++)
                    {
                        string account = ds.Tables[0].Rows[noRow].ItemArray[0].ToString() + ":" + ds.Tables[0].Rows[noRow].ItemArray[1].ToString() + ":" + ds.Tables[0].Rows[noRow].ItemArray[2].ToString() + ":" + ds.Tables[0].Rows[noRow].ItemArray[3].ToString() + ":" + ds.Tables[0].Rows[noRow].ItemArray[4].ToString() + ":" + ds.Tables[0].Rows[noRow].ItemArray[5].ToString();
                        ClGlobul.accountList.Add(account);
                    }
                    AddToLogger("[ " + DateTime.Now + " ] => [ " + ClGlobul.accountList.Count + " Accounts Loaded ]");
                }
                else
                {
                    AddToLogger("[ " + DateTime.Now + " ] => [  No Accounts Loaded ]");
                }
            }


            catch (Exception ex)
            {
                AddToLogger("[ " + DateTime.Now + " ] => [  No Accounts Loaded ]");
            }


            DataTable dt = new DataTable();
            dt.Columns.Add("Username");
            dt.Columns.Add("Password");
            dt.Columns.Add("proxyAddress");
            dt.Columns.Add("proxyPort");
            dt.Columns.Add("proxyUsername");
            dt.Columns.Add("proxyPassword");
            dt.Columns.Add("Path");


        }

        //#region LoggerProxy
        //private void LoggerProxy(string message)
        //{
        //    EventsArgs eventArgs = new EventsArgs(message);
        //    ProxyLogger.LogText(eventArgs);
        //}
        //#endregion

        void tab_instagram_DrawItem(object sender, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;

            Brush _textBrush;


            try
            {
                // Get the item from the collection.

                TabPage _tabPage = tab_instagram.TabPages[e.Index];


                //Set Image lay out for Tab Screan ....

                tab_instagram.TabPages[e.Index].BackgroundImageLayout = ImageLayout.Stretch;


                // Get the real bounds for the tab rectangle.

                Rectangle _tabBounds = tab_instagram.GetTabRect(e.Index);


                //Set custom color for brush....

                Brush tabButtonbrush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(226, 75, 22));//226, 75, 22


                if (e.State == DrawItemState.Selected)
                {

                    // Draw a different background color, and don't paint a focus rectangle.

                    _textBrush = new SolidBrush(Color.Red);
                    g.FillRectangle(tabButtonbrush, e.Bounds);
                }

                else
                {


                    _textBrush = new System.Drawing.SolidBrush(Color.White);
                    e.DrawBackground();
                    g.FillRectangle(tabButtonbrush, e.Bounds);
                }

                // Use our own font.

                Font _tabFont = new Font("Verdana", (float)12.0, FontStyle.Bold, GraphicsUnit.Pixel);


                // Draw string Center the text.

                StringFormat _stringFlags = new StringFormat();

                _stringFlags.Alignment = StringAlignment.Center;

                _stringFlags.LineAlignment = StringAlignment.Center;

                g.DrawString(_tabPage.Text, _tabFont, _textBrush, _tabBounds, new StringFormat(_stringFlags));


                //Set a rectangle for hiding tab meanu spaces..... 


                StringFormat sf = new StringFormat();

                sf.Alignment = StringAlignment.Center;

                sf.LineAlignment = StringAlignment.Center;

                //Set On tab For hiding Side Space of Tab button....

                Brush background_brush = new SolidBrush(Color.FromArgb(226, 75, 22));

                Rectangle LastTabRect = tab_instagram.GetTabRect(tab_instagram.TabPages.Count - 1);

                Rectangle rect = new Rectangle();

                rect.Location = new Point(0, LastTabRect.Bottom + 3);

                rect.Size = new Size(tab_instagram.Right - (tab_instagram.Width - LastTabRect.Width), tab_instagram.Height);

                e.Graphics.FillRectangle(background_brush, rect);

                //Dispose All Objects ....

                background_brush.Dispose();

                sf.Dispose();

                _textBrush.Dispose();

                _textBrush.Dispose();

                _tabFont.Dispose();
            }
            catch (Exception)
            {
            }

        }

        //******************** User follow Codes *******************************************************************************************************//

        #region  Follow Module

        private void btn_uploadAccount_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.InitialDirectory = Application.StartupPath;
                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        txtAddAccounts1.Text = ofd.FileName;
                        ReadLargeAccountsFile1(ofd.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                AddToLogger("[ " + DateTime.Now + " ] => [  Upload Accounts Loaded ]");
            }
        }


        private void ReadLargeAccountsFile1(object filePath)
        {
            ClGlobul.accountList.Clear();
            QueryExecuter.deleteQuery();
            //get and add accounts in account list ...
            List<string> AccountsList = GramBoardProFileHelper.ReadFile((string)filePath);
            foreach (string AccountsList_item in AccountsList)
            {
                try
                {
                    string username = string.Empty;
                    string password = string.Empty;
                    string proxyAddress = string.Empty;
                    string proxyPort = string.Empty;
                    string proxyUsername = string.Empty;
                    string proxyPassword = string.Empty;
                    string[] arr = Regex.Split(AccountsList_item, ":");

                    if (arr.Length == 2)
                    {
                        username = arr[0];
                        password = arr[1];
                    }
                    else if (arr.Length == 6)
                    {
                        username = arr[0];
                        password = arr[1];
                        proxyAddress = arr[2];
                        proxyPort = arr[3];
                        proxyUsername = arr[4];
                        proxyPassword = arr[5];
                    }
                    else
                    {
                        AddToLogger("[ " + DateTime.Now + " ] => [ Account not in correct format ]");
                        continue;
                    }
                    ClGlobul.accountList.Add(AccountsList_item);
                    try
                    {
                        //  ThreadPool.QueueUserWorkItem(new WaitCallback(getpageSourceFromProxy), new object[] { AccountsList_item });
                        QueryExecuter.insertAccount(username, password, proxyAddress, proxyPort, proxyUsername, proxyPassword, filePath.ToString());
                    }
                    catch (Exception ex)
                    {
                        AddToLogger("Error : " + ex.StackTrace);

                    }
                }
                catch (Exception ex)
                {
                    AddToLogger("Error : " + ex.StackTrace);
                }
            }
            AddToLogger("[ " + DateTime.Now + " ] => [ " + ClGlobul.accountList.Count + " Accounts Loaded ]");

        }


        private void ReadLargeAccountsFile(object filePath)
        {
            ClGlobul.accountList.Clear();
            QueryExecuter.deleteQuery();
            //get and add accounts in account list ...
            List<string> AccountsList = GramBoardProFileHelper.ReadFile((string)filePath);
            foreach (string AccountsList_item in AccountsList)
            {
                try
                {
                    string username = string.Empty;
                    string password = string.Empty;
                    string proxyAddress = string.Empty;
                    string proxyPort = string.Empty;
                    string proxyUsername = string.Empty;
                    string proxyPassword = string.Empty;
                    string[] arr = Regex.Split(AccountsList_item, ":");

                    if (arr.Length == 2)
                    {
                        username = arr[0];
                        password = arr[1];
                    }
                    else if (arr.Length == 4)
                    {
                        username = arr[0];
                        password = arr[1];
                        proxyAddress = arr[2];
                        proxyPort = arr[3];
                    }
                    else if (arr.Length == 6)
                    {
                        username = arr[0];
                        password = arr[1];
                        proxyAddress = arr[2];
                        proxyPort = arr[3];
                        proxyUsername = arr[4];
                        proxyPassword = arr[5];
                    }
                    else
                    {
                        AddToLogger1("[ " + DateTime.Now + " ] => [ Account not in correct format ]");
                        continue;
                    }
                    ClGlobul.accountList.Add(AccountsList_item);
                    try
                    {
                        //  ThreadPool.QueueUserWorkItem(new WaitCallback(getpageSourceFromProxy), new object[] { AccountsList_item });
                        QueryExecuter.insertAccount(username, password, proxyAddress, proxyPort, proxyUsername, proxyPassword, filePath.ToString());
                    }
                    catch (Exception ex)
                    {
                        AddToLogger1("Error : " + ex.StackTrace);

                    }
                }
                catch (Exception ex)
                {
                    AddToLogger1("Error : " + ex.StackTrace);
                }
            }
            AddToLogger1("[ " + DateTime.Now + " ] => [ " + ClGlobul.accountList.Count + " Accounts Loaded ]");

        }

        #region getpageSourceFromProxy
        public static List<Thread> lstProxyThread = new List<Thread>();
        public static bool proxyStop = false;
        public void getpageSourceFromProxy(object item)
        {
            if (proxyStop)
                return;
            try
            {
                Thread.CurrentThread.IsBackground = true;
                lstProxyThread.Add(Thread.CurrentThread);
                lstProxyThread = lstProxyThread.Distinct().ToList();
            }
            catch { }

            Array Item_value = (Array)item;
            string ClGlobul_ProxyList_item = (string)Item_value.GetValue(0);
            DemoStagramPro.frm_stagram frm1 = new DemoStagramPro.frm_stagram();
            Globussoft.GlobDramProHttpHelper GlobusHttpHelper = new Globussoft.GlobDramProHttpHelper();
            string proxyad = string.Empty;
            string proxyport = string.Empty;
            string proxyusername = string.Empty;
            string proxyPassword = string.Empty;
            string pagesource1 = string.Empty;
            string pagesource = string.Empty;


            try
            {
                string[] proxyLst = ClGlobul_ProxyList_item.Split(':');
                if (proxyLst.Count() > 3)
                {
                    proxyad = proxyLst[2];
                    proxyport = proxyLst[3];
                    //proxyusername = proxyLst[2];
                    //proxyPassword = proxyLst[3];
                }
                else if (proxyLst.Count() > 0 && proxyLst.Count() < 3)
                {
                    proxyad = proxyLst[0];
                    proxyport = proxyLst[1];
                }
                else
                {
                    return;
                }

                try
                {
                    if (proxyStop)
                        return;
                    //pagesource1 = GlobusHttpHelper.getHtmlfromUrlProxy(new Uri("http://websta.me/login"), proxyad, Convert.ToInt16(proxyport), proxyusername, proxyPassword);
                    pagesource1 = GlobusHttpHelper.getHtmlfromUrlProxy(new Uri("http://web.stagram.com/"), proxyad, Convert.ToInt16(proxyport), proxyusername, proxyPassword);
                }
                catch { };

                if (string.IsNullOrEmpty(pagesource1))
                {
                    pagesource1 = string.Empty;
                    pagesource1 = GlobusHttpHelper.getHtmlfromUrlProxy(new Uri("http://web.stagram.com/"), proxyad, Convert.ToInt16(proxyport), proxyusername, proxyPassword);
                }

                if (!string.IsNullOrEmpty(pagesource1))
                {
                    if (proxyStop)
                        return;
                    addInFinalCheckedProxyist(proxyad, proxyport, proxyusername, proxyPassword, pagesource1);
                }
                else
                {
                    if (proxyStop)
                        return;

                    AddToLogger("[ " + DateTime.Now + " ] => [ Proxy Is not Working : " + proxyad + ":" + proxyport + ":" + proxyusername + ":" + proxyPassword + " ]");
                    lock (lockerforNonWorkingProxies)
                    {
                        GramBoardProFileHelper.AppendStringToTextfileNewLine(proxyad + ":" + proxyport + ":" + proxyusername + ":" + proxyPassword, GramBoardProFileHelper.NonWorkingProxiesList);
                    }
                }
            }
            catch (Exception)
            {
                if (proxyStop)
                    return;
                AddToLogger("[ " + DateTime.Now + " ] => [ Proxy Is not Working : " + proxyad + ":" + proxyport + ":" + proxyusername + ":" + proxyPassword + " ]");

                lock (lockerforNonWorkingProxies)
                {
                    GramBoardProFileHelper.AppendStringToTextfileNewLine(proxyad + ":" + proxyport + ":" + proxyusername + ":" + proxyPassword, GramBoardProFileHelper.NonWorkingProxiesList);
                }
            }
            finally
            {

            }
        }
        #endregion

        #region addInFinalCheckedProxyist
        public static readonly object locker_finalProxyList = new object();
        public void addInFinalCheckedProxyist(string proxyad, string proxyport, string proxyusername, string proxyPassword, string pagesource)
        {
            //if (checkStatuse(pagesource))
            //{
            if (proxyStop)
                return;
            try
            {
                if (proxyStop)
                    return;
                AddToLogger("[ " + DateTime.Now + " ] => [ Working Proxy : " + proxyad + ":" + proxyport + " ]");
            }
            catch { };


            if (!string.IsNullOrEmpty(proxyusername) && !string.IsNullOrEmpty(proxyPassword) && !string.IsNullOrEmpty(proxyad) && !string.IsNullOrEmpty(proxyport))
            {
                if (proxyStop)
                    return;
                try
                {
                    string add = proxyad + ":" + proxyport + ":" + proxyusername + ":" + proxyPassword;
                    lock (lockerforProxies)
                    {
                        GramBoardProFileHelper.AppendStringToTextfileNewLine(add, GramBoardProFileHelper.WorkingProxiesList);
                    }
                    lock (locker_finalProxyList)
                    {
                        DemoStagramPro.ClGlobul.finalProxyList.Add(add);
                        Monitor.Pulse(locker_finalProxyList);
                    }
                }
                catch (Exception)
                {
                }
            }
            else if (string.IsNullOrEmpty(proxyusername) && string.IsNullOrEmpty(proxyPassword) && !string.IsNullOrEmpty(proxyad) && !string.IsNullOrEmpty(proxyport))
            {
                try
                {
                    if (proxyStop)
                        return;
                    string add = proxyad + ":" + proxyport;
                    //lock (lockerforNonWorkingProxies)
                    //{
                    if (proxyStop)
                        return;
                    GramBoardProFileHelper.AppendStringToTextfileNewLine(add, GramBoardProFileHelper.WorkingProxiesList);

                    //}
                    //lock (locker_finalProxyList)
                    //{
                    DemoStagramPro.ClGlobul.finalProxyList.Add(add);
                    Monitor.Pulse(locker_finalProxyList);
                    //}
                }
                catch (Exception)
                {
                }
            }
            else
            {

            }
            //}
        }
        #endregion

        private void btn_StartProcess_Click(object sender, EventArgs e)
        {
            int parsedValue;
            if (!int.TryParse(txt_noofThread.Text, out parsedValue))
            {


                AddToLogger("[ " + DateTime.Now + " ] => [ This is a number only field ]");
                return;
            }
            bool ProcessStartORnot = false;

            if (chkDivideDataFollow1.Checked && rdbDivideGivenByUser1.Checked)
            {
                if (!int.TryParse(txtDiveideByUser1.Text, out parsedValue))
                {


                    AddToLogger("[ " + DateTime.Now + " ] => [ This is a number only field.Please enter proper value in divide given by user field. ]");
                    return;
                }
            }
            //get single following from text box ...
            if (!string.IsNullOrEmpty(txt_FollowUser.Text.Trim()))
            {
                //add following in followingList...
                ClGlobul.followingList.Clear();

                string s = txt_FollowUser.Text.ToString();

                if (s.Contains(','))
                {
                    string[] Data = s.Split(',');

                    foreach (var item in Data)
                    {


                        ClGlobul.followingList.Add(item);
                    }
                }
                else
                {
                    ClGlobul.followingList.Add(txt_FollowUser.Text.ToString());
                }

            }

            try
            {
                if (!string.IsNullOrEmpty(txtAddAccounts1.Text.Trim()) && pathValidation(txtAddAccounts1.Text.Trim()))
                {

                    if (ClGlobul.followingList.Count != 0)
                    {
                        if (!string.IsNullOrEmpty(txt_noofThread.Text.Trim()) && ValidateNumber(txt_noofThread.Text.Trim()))
                        {
                            try
                            {

                                Maxthread = System.Convert.ToInt32(txt_noofThread.Text.Trim());

                                lst_Thread.Clear();
                                ThreadIs = false;

                                new Thread(() =>
                                {
                                    logInAccountsForFollow();

                                }).Start();

                            }
                            catch (Exception exception)
                            {

                            }
                        }
                        else
                        {
                            MessageBox.Show("Please Add No Of Threads");
                            AddToLogger("[ " + DateTime.Now + " ] => [ Please Add No Of Threads ]");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Add Following User Name ");
                        AddToLogger("[ " + DateTime.Now + " ] => [ Add Following User Name ]");
                    }
                }
                else
                {
                    MessageBox.Show("Please Upload List Of accounts.");
                    AddToLogger("[ " + DateTime.Now + " ] => [ Please Upload List Of accounts. ]");
                }
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePath);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => btn_StartProcess_Click :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePath);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePath);
            }
        }


        //Make object for Globuss HTTP helper or chilkat classes ...
        Globussoft.GlobDramProHttpHelper globusshttpHellper = new GlobDramProHttpHelper();
        ChilkatHttpHelpr chilkatHelper = new ChilkatHttpHelpr();

        readonly object lockr_ThreadControllerForFollow = new object();
        readonly object lockr_ThreadControllerForFollow1 = new object();
        int count_ThreadControllerForFollow = 0;
        int count_ThreadControllerForFollow1 = 0;
        int counter_follow = 0;
        public void logInAccountsForFollow()
        {
            try
            {
                AddToLogger("[ " + DateTime.Now + " ] => [ Starting Follow Process ]");

                //Lock Threads If Proxy list is Empty...


                //Check if Queue is Empty and Fill again Queue List ...
                if (ClGlobul.ProxyQueue.Count == 0)
                {
                    ClGlobul.AddProxyInQueur();
                }

                ClGlobul.TotalNoOfIdsForFollow = 0;

                //get total No of accounts ...
                ClGlobul.TotalNoOfIdsForFollow = ClGlobul.accountList.Count;

                //Divide all Accounts in Bunches....
                List<List<string>> list_Accounts = new List<List<string>>();
                List<List<string>> list_lstTargetUsers = new List<List<string>>();
                counter_follow = DemoStagramPro.ClGlobul.accountList.Count();
                list_Accounts = ListUtilities.Split(DemoStagramPro.ClGlobul.accountList, Maxthread);


                //for divide by data logic here
                if (chkDivideDataFollow1.Checked)
                {
                    if (rdbDivideGivenByUser1.Checked || rdbDivideEqually1.Checked)
                    {
                        int splitNo = 0;
                        if (rdbDivideEqually1.Checked)
                        {
                            splitNo = ClGlobul.followingList.Count / ClGlobul.accountList.Count();
                        }
                        else if (rdbDivideGivenByUser1.Checked)
                        {
                            if (Convert.ToInt32(txtDiveideByUser1.Text.Trim()) != 0)
                            {
                                int res = Convert.ToInt32(Convert.ToInt32(txtDiveideByUser1.Text.Trim()));
                                splitNo = res;
                            }
                        }
                        if (splitNo == 0)
                        {
                            splitNo = RandomNumberGenerator.GenerateRandom(0, ClGlobul.followingList.Count - 1);
                        }
                        list_lstTargetUsers = Split(ClGlobul.followingList, splitNo);
                    }
                }

                //Start all Account from Bunch List..
                int LstCounter = 0;
                foreach (List<string> listAccounts in list_Accounts)
                {
                    foreach (string account in listAccounts)
                    {
                        //lock Thread if account is not availabe in list ...
                        lock (lockr_ThreadControllerForFollow)
                        {
                            try
                            {
                                if (count_ThreadControllerForFollow >= listAccounts.Count)
                                {
                                    Monitor.Wait(lockr_ThreadControllerForFollow);
                                }


                                if (LstCounter == list_lstTargetUsers.Count && (chkDivideDataFollow1.Checked))
                                {
                                    // AddToLogger("[ " + DateTime.Now + " ] => [ Account is grater than List of users. ]");
                                    // break;
                                }

                                List<string> list_lstTargetUsers_item = new List<string>();

                                if (chkDivideDataFollow1.Checked)
                                {
                                    list_lstTargetUsers_item = list_lstTargetUsers[LstCounter];
                                }
                                else
                                {

                                    list_lstTargetUsers_item = ClGlobul.followingList;
                                }

                                //break Account or pass...
                                string proxyValue = string.Empty;
                                string[] accountAndPass = account.Split(':');
                                string AccountName = string.Empty;
                                string AccountPass = string.Empty;
                                if (accountAndPass.Length == 2)
                                {
                                    AccountName = accountAndPass[0].Replace("\0", string.Empty).Trim();
                                    AccountPass = accountAndPass[1].Replace("\0", string.Empty).Trim();
                                }
                                else if (accountAndPass.Length == 4 || accountAndPass.Length == 6)
                                {
                                    AccountName = accountAndPass[0].Replace("\0", string.Empty).Trim();
                                    AccountPass = accountAndPass[1].Replace("\0", string.Empty).Trim();
                                    if (accountAndPass.Length == 4)
                                    {
                                        proxyValue = accountAndPass[2] + ":" + accountAndPass[3];
                                    }
                                    else if (accountAndPass.Length == 6)
                                    {
                                        proxyValue = accountAndPass[2] + ":" + accountAndPass[3] + ":" + accountAndPass[4] + ":" + accountAndPass[5];
                                    }
                                }
                                else
                                {
                                    if (ClGlobul.finalProxyList.Count > 0)
                                    {
                                        lock (StagramProxyChecker.classes.ClProxyChecker.locker_finalProxyList)
                                        {
                                            if (ClGlobul.finalProxyList.Count == 0)
                                            {
                                                AddToLogger("[ " + DateTime.Now + " ] => [ Waiting For Proxies To Be Loaded ]");
                                                Monitor.Wait(StagramProxyChecker.classes.ClProxyChecker.locker_finalProxyList);
                                            }
                                        }
                                        try
                                        {
                                            string[] tempQueue = ClGlobul.ProxyQueue.Dequeue();
                                            if (tempQueue.Length > 0)
                                            {
                                                proxyValue = tempQueue[0];
                                            }
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                    }
                                }
                                //If Account is containe colon its remove... 
                                string acc = account.Remove(account.IndexOf(':'));

                                //Create new thread...
                                Thread FollowThread = new Thread(getloginforFollow);
                                FollowThread.Name = "workerThread_Follow_" + acc;
                                FollowThread.IsBackground = true;
                                try
                                {
                                    //add Thread in list for No of threads...
                                    ClGlobul.ThreadList.Add(FollowThread.Name, FollowThread);
                                }
                                catch (Exception ex)
                                {
                                }

                                FollowThread.Start(new object[] { (AccountName), (AccountPass), proxyValue, list_lstTargetUsers_item });


                                count_ThreadControllerForFollow++;
                                LstCounter++;
                                Thread.Sleep(1000);
                            }
                            catch { }
                        }

                        if (ClGlobul.ProxyQueue.Count == 0)
                        {
                            ClGlobul.AddProxyInQueur();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => logInAccounts :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
            }
        }



        public static List<List<string>> Split(List<string> source, int splitNumber)
        {
            if (splitNumber <= 0)
            {
                splitNumber = 1;
            }

            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / splitNumber)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }

        List<Thread> lst_Thread = new List<Thread>();
        bool ThreadIs = false;
        public void getloginforFollow(object Accounts)
        {
            try
            {
                if (!ThreadIs)
                {
                    try
                    {
                        lst_Thread.Add(Thread.CurrentThread);
                        lst_Thread.Distinct();
                        Thread.CurrentThread.IsBackground = true;
                    }
                    catch
                    {
                    }

                    Array accountinfo = (Array)Accounts;

                    string account = (string)accountinfo.GetValue(0);
                    string pass = (string)accountinfo.GetValue(1);
                    string ProxyValue = (string)accountinfo.GetValue(2);
                    List<string> TargetedUser = (List<string>)accountinfo.GetValue(3);
                    string proxyadd = string.Empty;
                    string proxyport = string.Empty;
                    string proxyUser = string.Empty;
                    string proxyPass = string.Empty;
                    if (!string.IsNullOrEmpty(ProxyValue))
                    {
                        string[] proxyarray = ProxyValue.Split(':');
                        if (proxyarray.Count() == 4)
                        {
                            proxyadd = proxyarray[0];
                            proxyport = proxyarray[1];
                            proxyUser = proxyarray[2];
                            proxyPass = proxyarray[3];
                        }
                        else if (proxyarray.Count() == 2)
                        {
                            proxyadd = proxyarray[0];
                            proxyport = proxyarray[1];
                        }
                        else
                        {
                            AddToLogger("[ " + DateTime.Now + " ] => [ No Proxy for Account : " + account + " ]");
                        }
                    }

                    try
                    {
                        InstagramManager.Classes.InstagramAccountManager InstagramAccountManager = new InstagramManager.Classes.InstagramAccountManager(account, pass, proxyadd, proxyport, "", "");
                       // AddToLogger("[ " + DateTime.Now + " ] => [ Logging In With :" + InstagramAccountManager.Username + " ]");
                        string status = InstagramAccountManager.MyLoginandComment(ref InstagramAccountManager.httpHelper, "", "", "");
                        if (status.Contains("Stream was not readable"))
                        {
                            status = InstagramAccountManager.MyLoginandComment(ref InstagramAccountManager.httpHelper, "", "", "");
                        }
                        if (InstagramAccountManager.LoggedIn == true)
                        {
                            //AddToLogger("[ " + DateTime.Now + " ] => [ Logged In success with " + InstagramAccountManager.Username + " ]");
                            //AddToLogger("[ " + DateTime.Now + " ] => [ Starting Follow With : " + InstagramAccountManager.Username + " ]");
                            getFollow(ref InstagramAccountManager, TargetedUser);
                        }
                        else if (status.Contains("Failed"))
                        {
                           // AddToLogger("[ " + DateTime.Now + " ] => [ " + account + " Failed To Login ]");
                        }
                        else if (status.Contains("AccessIssue"))
                        {
                           // AddToLogger("[ " + DateTime.Now + " ] => [ " + account + " Access Issue In Login ]");
                        }

                        else if (status.Contains("The request was aborted: The operation has timed out."))
                        {

                            //AddToLogger("[ " + DateTime.Now + " ] => [ " + account + " Operation timed Out To Slow Request ]");

                        }
                        else if (status.Contains("503"))
                        {

                            //AddToLogger("[ " + DateTime.Now + " ] => [ " + account + " Failed To Login ]");
                        }
                        else if (status.Contains("403"))
                        {

                           // AddToLogger("[ " + DateTime.Now + " ] => [ " + account + " Failed To Login ]");

                        }
                        else if (status.Contains("Please enter a correct username and password."))
                        {
                           // AddToLogger("[ " + DateTime.Now + " ] => [ " + account + " : Incorect Username Or Password ]");
                        }
                        else if (status.Contains("Please enter a correct username and password."))
                        {
                            //AddToLogger("[ " + DateTime.Now + " ] => [ " + account + " : Incorect Username Or Password ]");
                        }
                        else
                        {
                            //AddToLogger("[ " + DateTime.Now + " ] => [ Could Not Login With :" + InstagramAccountManager.Username + " ]");
                        }

                    }
                    catch (Exception ex)
                    {
                        GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePath);
                        GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => GetLogin (1) :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePath);
                        GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePath);
                    }
                }
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePath);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => GetLogin (2) :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePath);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePath);
            }
            finally
            {
                {
                    count_ThreadControllerForFollow--;
                    lock (lockr_ThreadControllerForFollow)
                    {
                        if (!ThreadIs)
                        {
                            Monitor.Pulse(lockr_ThreadControllerForFollow);
                        }
                    }
                }
            }
        }

        public void AccountChecker(object Accounts)
        {
            try
            {
                if (!ThreadIs)
                {
                    try
                    {
                        lst_Thread.Add(Thread.CurrentThread);
                        lst_Thread.Distinct();
                        Thread.CurrentThread.IsBackground = true;
                    }
                    catch
                    {
                    }

                    Array accountinfo = (Array)Accounts;

                    string account = (string)accountinfo.GetValue(0);
                    string pass = (string)accountinfo.GetValue(1);
                    string ProxyValue = (string)accountinfo.GetValue(2);
                    string[] proxyarray = ProxyValue.Split(':');
                    try
                    {
                        string proxyadd = string.Empty;
                        string proxyport = string.Empty;
                        string proxyUser = string.Empty;
                        string proxyPass = string.Empty;

                        if (proxyarray.Count() == 4)
                        {
                            proxyadd = proxyarray[0];
                            proxyport = proxyarray[1];
                            proxyUser = proxyarray[2];
                            proxyPass = proxyarray[3];
                        }
                        else if (proxyarray.Count() == 2)
                        {
                            proxyadd = proxyarray[0];
                            proxyport = proxyarray[1];
                        }

                        InstagramManager.Classes.InstagramAccountManager InstagramAccountManager = new InstagramManager.Classes.InstagramAccountManager(account, pass, proxyadd, proxyport, proxyUser, proxyPass);
                       // AddToLogger("[ " + DateTime.Now + " ] => [ Logging In From Account : " + account + " ]");

                        // string statuse = InstagramAccountManager.MyLoginandComment(ref InstagramAccountManager.httpHelper, "", "", "");
                        string statuse = InstagramAccountManager.Login();

                        if (statuse == "Failed")
                        {
                            //AddToLogger("[ " + DateTime.Now + " ] => [ Login failed For : " + account + " ]");
                            GramBoardProFileHelper.AppendStringToTextfileNewLine(account + ":" + pass + ":" + proxyadd + ":" + proxyport + ":" + proxyUser + ":" + proxyPass, GramBoardProFileHelper.FailedAccounts);
                        }
                        else if (statuse == "AccessIssue")
                        {
                          //  AddToLogger("[ " + DateTime.Now + " ] => [ Access Issue For : " + account + " ]");
                            GramBoardProFileHelper.AppendStringToTextfileNewLine(account + ":" + pass + ":" + proxyadd + ":" + proxyport + ":" + proxyUser + ":" + proxyPass, GramBoardProFileHelper.AccessIssuesAccounts);
                        }
                        else if (statuse == "Success")
                        {
                           // AddToLogger("[ " + DateTime.Now + " ] => [ Login Successfull : " + account + " ]");
                            GramBoardProFileHelper.AppendStringToTextfileNewLine(account + ":" + pass + ":" + proxyadd + ":" + proxyport + ":" + proxyUser + ":" + proxyPass, GramBoardProFileHelper.successfullyLoggedaccounts);
                        }
                        else if (statuse == "Stream was not readable.")
                        {
                           // AddToLogger("[ " + DateTime.Now + " ] => [ Could Not Login With " + account + " ]");
                            GramBoardProFileHelper.AppendStringToTextfileNewLine(account + ":" + pass + ":" + proxyadd + ":" + proxyport + ":" + proxyUser + ":" + proxyPass, GramBoardProFileHelper.PostRequesterror);
                        }
                    }
                    catch (Exception ex)
                    {
                        GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePath);
                        GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => GetLogin (1) :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePath);
                        GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePath);
                    }
                }
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePath);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => GetLogin (2) :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePath);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePath);
            }
            finally
            {
                count_ThreadControllerForFollow--;

                lock (lockr_ThreadControllerForFollow)
                {
                    if (!ThreadIs)
                    {
                        Monitor.Pulse(lockr_ThreadControllerForFollow);
                    }
                }

                acc_checker_counter--;

                if (acc_checker_counter == 0)
                {
                    AddToLogger("-----------------------------------------------------------------------------------------------");
                   // AddToLogger("[ " + DateTime.Now + " ] => [ PROCESS COMPLETED ]");
                    AddToLogger("-----------------------------------------------------------------------------------------------");
                }

            }
        }


        public void AccountChecker1(object Accounts)
        {
            try
            {
                if (!ThreadIs)
                {
                    try
                    {
                        lst_Thread.Add(Thread.CurrentThread);
                        lst_Thread.Distinct();
                        Thread.CurrentThread.IsBackground = true;
                    }
                    catch
                    {
                    }

                    Array accountinfo = (Array)Accounts;

                    string account = (string)accountinfo.GetValue(0);
                    string pass = (string)accountinfo.GetValue(1);
                    string ProxyValue = (string)accountinfo.GetValue(2);
                    string[] proxyarray = ProxyValue.Split(':');
                    try
                    {
                        string proxyadd = string.Empty;
                        string proxyport = string.Empty;
                        string proxyUser = string.Empty;
                        string proxyPass = string.Empty;

                        if (proxyarray.Count() == 4)
                        {
                            proxyadd = proxyarray[0];
                            proxyport = proxyarray[1];
                            proxyUser = proxyarray[2];
                            proxyPass = proxyarray[3];
                        }
                        else if (proxyarray.Count() == 2)
                        {
                            proxyadd = proxyarray[0];
                            proxyport = proxyarray[1];
                        }

                        InstagramManager.Classes.InstagramAccountManager InstagramAccountManager = new InstagramManager.Classes.InstagramAccountManager(account, pass, proxyadd, proxyport, proxyUser, proxyPass);
                       // AddToLogger1("[ " + DateTime.Now + " ] => [ Logging In From Account : " + account + " ]");

                        // string statuse = InstagramAccountManager.MyLoginandComment(ref InstagramAccountManager.httpHelper, "", "", "");
                        string statuse = InstagramAccountManager.Login();

                        if (statuse == "Failed")
                        {
                            AddToLogger1("[ " + DateTime.Now + " ] => [ Login failed For : " + account + " ]");
                            GramBoardProFileHelper.AppendStringToTextfileNewLine(account + ":" + pass + ":" + proxyadd + ":" + proxyport + ":" + proxyUser + ":" + proxyPass, GramBoardProFileHelper.FailedAccounts);
                        }
                        else if (statuse == "AccessIssue")
                        {
                            AddToLogger1("[ " + DateTime.Now + " ] => [ Access Issue For : " + account + " ]");
                            GramBoardProFileHelper.AppendStringToTextfileNewLine(account + ":" + pass + ":" + proxyadd + ":" + proxyport + ":" + proxyUser + ":" + proxyPass, GramBoardProFileHelper.AccessIssuesAccounts);
                        }
                        else if (statuse == "Success")
                        {
                            //AddToLogger1("[ " + DateTime.Now + " ] => [ Login Successfull : " + account + " ]");
                            GramBoardProFileHelper.AppendStringToTextfileNewLine(account + ":" + pass + ":" + proxyadd + ":" + proxyport + ":" + proxyUser + ":" + proxyPass, GramBoardProFileHelper.successfullyLoggedaccounts);
                        }
                        else if (statuse == "Stream was not readable.")
                        {
                            AddToLogger1("[ " + DateTime.Now + " ] => [ Could Not Login With " + account + " ]");
                            GramBoardProFileHelper.AppendStringToTextfileNewLine(account + ":" + pass + ":" + proxyadd + ":" + proxyport + ":" + proxyUser + ":" + proxyPass, GramBoardProFileHelper.PostRequesterror);
                        }
                    }
                    catch (Exception ex)
                    {
                        GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePath);
                        GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => GetLogin (1) :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePath);
                        GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePath);
                    }
                }
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePath);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => GetLogin (2) :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePath);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePath);
            }
            finally
            {
                count_ThreadControllerForFollow1--;

                lock (lockr_ThreadControllerForFollow1)
                {
                    if (!ThreadIs)
                    {
                        Monitor.Pulse(lockr_ThreadControllerForFollow1);
                    }
                }

                acc_checker_counter1--;

                if (acc_checker_counter1 == 0)
                {
                    AddToLogger1("-----------------------------------------------------------------------------------------------");
                    AddToLogger1("[ " + DateTime.Now + " ] => [ PROCESS COMPLETED ]");
                    AddToLogger1("-----------------------------------------------------------------------------------------------");
                }

            }
        }


        public bool checkStatuse(string statuse)
        {
            //if()
            if (statuse.Contains("Success"))
            {
                return true;
            }
            if (statuse.Contains("AccessIssue"))
            {
                return true;
            }
            if (statuse.Contains("Failed"))
            {
                return true;
            }
            if (statuse.Contains("Stream was not readable."))
            {
                return true;
            }
            if (statuse.Contains("The request was aborted: The operation has timed out."))
            {
                return true;
            }
            if (statuse.Contains("503"))
            {
                return false;
            }
            return false;
        }


        List<string> NotFollowedlist = new List<string>();
        List<string> Followedlist = new List<string>();
        List<string> AlreadyFollowedlist = new List<string>();

        public void getFollow(ref InstagramManager.Classes.InstagramAccountManager accountManager, List<string> followingList)
        {
            InstagramManager.Classes.InstagramFollow Instagramfollow = new InstagramManager.Classes.InstagramFollow();

            try
            {
                if (ClGlobul.followingList.Count != 0)
                {
                    //foreach (string followingList_item in ClGlobul.followingList) //commented when divide data implemented.
                    foreach (string followingList_item in followingList)
                    {
                        try
                        {
                            string FollowerName = followingList_item;
                            string Result = Instagramfollow.Follow(FollowerName, ref accountManager);

                            if (Result == "Followed")
                            {
                                ClGlobul.TotalNoOfFollow++;

                                AddToLogger("[ " + DateTime.Now + " ] => [" + accountManager.Username + " Followed " + FollowerName + " ]");
                              
                                if (!string.IsNullOrEmpty(txtmindelay.Text) && NumberHelper.ValidateNumber(txtmindelay.Text))
                                {
                                    mindelay = Convert.ToInt32(txtmindelay.Text);
                                }
                                if (!string.IsNullOrEmpty(txtmaxdelay.Text) && NumberHelper.ValidateNumber(txtmaxdelay.Text))
                                {
                                    maxdelay = Convert.ToInt32(txtmaxdelay.Text);
                                }
                                lock (_lockObject)
                                {
                                    Random rn = new Random();
                                    int delay = RandomNumberGenerator.GenerateRandom(mindelay, maxdelay);
                                    delay = rn.Next(mindelay, maxdelay);
                                    AddToLogger("[ " + DateTime.Now + " ] => [ Delay For " + delay + " Seconds ]");
                                    Thread.Sleep(delay * 1000);
                                }

                                if (!Followedlist.Contains(accountManager.Username))
                                {
                                    Followedlist.Add(accountManager.Username);
                                    GramBoardProFileHelper.AppendStringToTextfileNewLine("Followed: " + FollowerName + " By: " + accountManager.Username + ":" + accountManager.Password, GramBoardProFileHelper.FollowIDFilePath);
                                }
                            }
                            else if (Result == "private")
                            {
                                AddToLogger("[ " + DateTime.Now + " ] => [ Followed: " + FollowerName + " is a private user and can not be followed. ]");
                                GramBoardProFileHelper.AppendStringToTextfileNewLine(accountManager.Username + ":" + accountManager.Password, GramBoardProFileHelper.FollowedOptionNotAvailableFilePath);
                            }
                            else if (Result == "Already Followed")
                            {

                                ClGlobul.TotalNoOfFollow++;
                                AddToLogger("[ " + DateTime.Now + " ] => [ Account:" + accountManager.Username + ClGlobul.TotalNoOfFollow + " Followed " + FollowerName + " ]");
                                if (!string.IsNullOrEmpty(txtmindelay.Text) && NumberHelper.ValidateNumber(txtmindelay.Text))
                                {
                                    mindelay = Convert.ToInt32(txtmindelay.Text);
                                }
                                if (!string.IsNullOrEmpty(txtmaxdelay.Text) && NumberHelper.ValidateNumber(txtmaxdelay.Text))
                                {
                                    maxdelay = Convert.ToInt32(txtmaxdelay.Text);
                                }
                                lock (_lockObject)
                                {
                                    Random rn = new Random();
                                    int delay = RandomNumberGenerator.GenerateRandom(mindelay, maxdelay);
                                    delay = rn.Next(mindelay, maxdelay);
                                    AddToLogger("[ " + DateTime.Now + " ] => [ Delay For " + delay + " Seconds ]");
                                    Thread.Sleep(delay * 1000);
                                }

                                if (!Followedlist.Contains(accountManager.Username))
                                {
                                    Followedlist.Add(accountManager.Username);
                                    GramBoardProFileHelper.AppendStringToTextfileNewLine("Followed: " + FollowerName + " By: " + accountManager.Username + ":" + accountManager.Password, GramBoardProFileHelper.FollowIDFilePath);
                                }

                                if (!AlreadyFollowedlist.Contains(accountManager.Username))
                                {
                                    AlreadyFollowedlist.Add(accountManager.Username);
                                    GramBoardProFileHelper.AppendStringToTextfileNewLine("Already Followed: " + FollowerName + " By: " + accountManager.Username + ":" + accountManager.Password, GramBoardProFileHelper.AllReadyFollowedIdFilePath);
                                }
                            }
                            else if (Result == "Follow option is not available In page...!!")
                            {

                                AddToLogger("[ " + DateTime.Now + " ] => [ Follow option is not available In page...!!" + accountManager.Username + " ]");
                                GramBoardProFileHelper.AppendStringToTextfileNewLine(accountManager.Username + ":" + accountManager.Password, GramBoardProFileHelper.FollowedOptionNotAvailableFilePath);
                            }
                            else
                            {

                                AddToLogger("[ " + DateTime.Now + " ] => [ " + accountManager.Username + " Not Followed " + FollowerName + " ]");


                                if (!NotFollowedlist.Contains(accountManager.Username))
                                {
                                    NotFollowedlist.Add(accountManager.Username);
                                    GramBoardProFileHelper.AppendStringToTextfileNewLine(accountManager.Username + ":" + accountManager.Password, GramBoardProFileHelper.UnFollowIdFilePath);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePath);
                            GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => getFollow1 :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePath);
                            GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePath);
                        }
                    }
                }

                else
                {
                    GramBoardProFileHelper.AppendStringToTextfileNewLine("Methode Name => GetFollow2 :=> ", GramBoardProFileHelper.ErrorLogFilePath);
                    AddToLogger("[ " + DateTime.Now + " ] => [ Please upload Following ID's ]");
                }
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePath);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => GetFollow3 :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePath);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePath);
            }


            finally
            {
                ClGlobul.FolloConpletedList.Add(accountManager.Username + ":" + accountManager.Password);
                ClGlobul.TotalNoOfIdsForFollow--;

                counter_follow--;
                if (counter_follow == 0)
                {
                    AddToLogger("-----------------------------------------------------------------------------------------------");
                    AddToLogger("[ " + DateTime.Now + " ] => [ PROCESS COMPLETED ]");
                    AddToLogger("-----------------------------------------------------------------------------------------------");
                }

                try
                {
                    string UserName = accountManager.Username.ToString();
                    var DicValue = ClGlobul.ThreadList.Single(s => s.Key.Contains(UserName));
                    Thread value = DicValue.Value;
                    if (value.IsAlive || value.IsBackground)
                    {
                        value = null;
                    }
                }
                catch (Exception ex)
                {

                    GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => Finally (Thread Spoile) :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePath);
                }
            }
        }
        private void btn_addProxy_Click(object sender, EventArgs e)
        {
            Frm_proxy frm_proxy = new Frm_proxy();
            frm_proxy.Show();
        }
        private void AddToLogger(string log)
        {
            while (_boolAddToLogger)
            {
                Thread.Sleep(1 * 1000);
            }

            this.Invoke(new MethodInvoker(delegate
            {
                lbLogger.Items.Add(log);
                lbLogger.SelectedIndex = lbLogger.Items.Count - 1;
            }));
        }
        private void AddToLogger1(string log)
        {
            while (_boolAddToLogger)
            {
                Thread.Sleep(1 * 1000);
            }

            this.Invoke(new MethodInvoker(delegate
            {
                lbLogger1.Items.Add(log);
                lbLogger1.SelectedIndex = lbLogger1.Items.Count - 1;
            }));
        }
        private void btn_followingFile_Click(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(txt_FollowUser.Text.ToString()))
            {
                AddToLogger("[ " + DateTime.Now + " ] => [ " + " Name of Following already added ]");
                return;
            }
            try
            {
                ClGlobul.followingList.Clear();
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => Finally (FollowingFile) :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePath);
            }
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.InitialDirectory = Application.StartupPath;
                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        txt_filepathofFollowing.Text = ofd.FileName;
                        ReadLargeFollowingFile(ofd.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => Finally (FollowingFile) :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePath);
            }
        }

        public void ReadLargeFollowingFile(object filePath)
        {
            List<string> AccountsList = GramBoardProFileHelper.ReadFile((string)filePath);
            foreach (string AccountsList_item in AccountsList)
            {
                ClGlobul.followingList.Add(AccountsList_item);
            }

            AddToLogger("[ " + DateTime.Now + " ] => [ " + ClGlobul.followingList.Count + " followingList Loaded ]");
        }
        private void btn_clearAccounts_Click(object sender, EventArgs e)
        {
            try
            {

                txt_filepathofFollowing.Text = string.Empty;
                txt_FollowUser.Text = string.Empty;
                //ClGlobul.accountList.Clear();
                ClGlobul.FolloConpletedList.Clear();
                ClGlobul.TotalNoOfFollow = 0;
                ClGlobul.TotalNoOfIdsForFollow = 0;
                ClGlobul.ThreadList.Clear();
                lst_Thread.Clear();
                ThreadIs = false;
                count_ThreadControllerForFollow = 0;
                lbLogger.Items.Clear();
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => Finally (clearAccounts) :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePath);
            }
        }
        private void btn_notfollow_Click(object sender, EventArgs e)
        {
            ClGlobul.FolloConpletedList = ClGlobul.FolloConpletedList.Distinct().ToList();
            if (ClGlobul.FolloConpletedList.Count() != 0)
            {
                foreach (string FolloConpletedList_item in ClGlobul.FolloConpletedList)
                {
                    ClGlobul.accountList.Remove(FolloConpletedList_item);
                }

                foreach (string accountList_item in ClGlobul.accountList)
                {
                    GramBoardProFileHelper.AppendStringToTextfileNewLine(accountList_item, GramBoardProFileHelper.UnFollowIdFilePath);
                }

                AddToLogger("[ " + DateTime.Now + " ] => [ Stored all Remaining account. ]");
            }
            else
            {
                AddTophotoLogger("[ " + DateTime.Now + " ] => [  Plase wait for Finishing Follow Process. ]");
            }
        }
        #endregion

        //********************************************* Photo Like Codes *******************************************************************************//

        #region Photo Like Module

        private void btn_photoListUpload_Click(object sender, EventArgs e)
        {
            try
            {


                using (OpenFileDialog ofdphotolike = new OpenFileDialog())
                {
                    ofdphotolike.InitialDirectory = Application.StartupPath;
                    ofdphotolike.Filter = "Text Files (*.txt)|*.txt";
                    if (ofdphotolike.ShowDialog() == DialogResult.OK)
                    {
                        txt_photoListPath.Text = ofdphotolike.FileName;
                        ReadLargePhotoFile(ofdphotolike.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => btn_photolistUpload :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
            }
        }
        public void ReadLargePhotoFile(string photoFilename)
        {
            ClGlobul.PhotoList.Clear();
            try
            {
                List<string> photolist = GramBoardProFileHelper.ReadFile((string)photoFilename);
                foreach (string phoyoList_item in photolist)
                {



                    ClGlobul.PhotoList.Add(phoyoList_item);
                }
                AddTophotoLogger("[ " + DateTime.Now + " ] => [ " + ClGlobul.PhotoList.Count + " Image IDs Uploaded. ]");
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => Read LargePhotoFile :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
            }
        }
        private void btn_starteLike_Click(object sender, EventArgs e)
        {
            try
            {

                if (string.IsNullOrEmpty(txt_photoListPath.Text) && string.IsNullOrEmpty(txtphotosingaluser.Text))
                {
                    MessageBox.Show("Please Upload  a Photo Id First");
                    return;
                }
            }
            catch { };
            try
            {

                //int parsedValue;
                //if (!int.TryParse(txt_noOfphotoLikethread.Text, out parsedValue))
                //{


                //    AddTophotoLogger("[ " + DateTime.Now + " ] => [  This is a number only field ]");
                //    return;
                //}


                if (!string.IsNullOrEmpty(txtphotosingaluser.Text.Trim()))
                {
                    //add following in followingList...
                    lstStoreDownloadImageKeyword.Clear();

                    string s = txtphotosingaluser.Text.ToString();

                    if (s.Contains(','))
                    {
                        string[] Data = s.Split(',');

                        foreach (var item in Data)
                        {


                            ClGlobul.PhotoList.Add(item);
                        }
                    }
                    else
                    {
                        ClGlobul.PhotoList.Add(txtphotosingaluser.Text.ToString());
                    }

                }

                bool ProcessStartORnot = false;
                if (ClGlobul.accountList.Count() != 0)
                {
                    //if (!string.IsNullOrEmpty(txt_photoListPath.Text.Trim()) && pathValidation(txt_photoListPath.Text.Trim()))
                    //{
                    if (!string.IsNullOrEmpty(txtAddAccounts1.Text.Trim()) && pathValidation(txtAddAccounts1.Text.Trim()))
                    {
                        if (string.IsNullOrEmpty(txt_noOfphotoLikethread.Text.Trim()) && ValidateNumber(txt_noOfphotoLikethread.Text.Trim()))
                        {
                            if (MessageBox.Show("Do you really want to Start Without Thread", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                ProcessStartORnot = true;
                                ClGlobul.NoOfPhotoLikeThread = 1;
                            }
                            else
                            {
                            }
                        }
                        else
                        {
                            try
                            {
                                ClGlobul.NoOfPhotoLikeThread = Convert.ToInt16(txt_noOfphotoLikethread.Text.Trim());
                                ProcessStartORnot = true;
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("Please Enter Numeric Value in Photo Like Thread..!!", "Wrong Value Insert In Photo Like Thread");
                            }
                        }
                    }


                    else
                    {
                        AddTophotoLogger("[ " + DateTime.Now + " ] => [ Please upload Photo Ids. ]");
                        MessageBox.Show("Please upload Photo Ids.", "Please upload Photo Ids.");
                    }
                    // }
                }


                else
                {
                    AddTophotoLogger("[ " + DateTime.Now + " ] => [ Please Upload Accounts. ]");
                    MessageBox.Show("Please Upload Accounts.", "Please Upload Accounts.");
                    tab_instagram.SelectedIndex = 0;
                }

                if (ClGlobul.ProxyList.Count > 0)
                {
                    Frm_proxy frmProxy = new Frm_proxy();

                    AddTophotoLogger("[ " + DateTime.Now + " ] => [ Please Upload Proxies. ]");
                    MessageBox.Show("Please Upload Proxies.", "Please Upload Proxies.");
                    frmProxy.Show();
                }


                if (ProcessStartORnot)
                {
                    Lst_photoLikethread.Clear();
                    PhotoLikeIstrueOrFals = false;

                    new Thread(() =>
                    {
                        logInAccountsForPhotoLike();

                    }).Start();
                }
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => btn_starteLike_Click :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
            }
        }

        readonly object lockr_ThreadControllerForPhotoLike = new object();
        int count_ThreadControllerForPhotoLike = 0;

        public void logInAccountsForPhotoLike()
        {

            try
            {
                GloBoardPro.lstThread.Add(Thread.CurrentThread);
                Thread.CurrentThread.IsBackground = true;
                GloBoardPro.lstThread = GloBoardPro.lstThread.Distinct().ToList();
            }
            catch { };
            try
            {
                AddTophotoLogger("[ " + DateTime.Now + " ] => [ Starting Photo like Process ]");
                int numberOfThreads = 0;
                if (ClGlobul.NoOfPhotoLikeThread != 0)
                {
                    numberOfThreads = ClGlobul.NoOfPhotoLikeThread;
                }

                List<List<string>> list_listAccounts = new List<List<string>>();
                list_listAccounts = ListUtilities.Split(DemoStagramPro.ClGlobul.accountList, numberOfThreads);
                ThreadPool.SetMaxThreads(numberOfThreads, 5);
                foreach (List<string> listAccounts in list_listAccounts)
                {
                    foreach (string account in listAccounts)
                    {

                        ThreadPool.QueueUserWorkItem(new WaitCallback(PhotoLikeProcess), new object[] { account });

                    }
                }
            }
            catch (Exception ex)
            {

                GramBoardProFileHelper.AppendStringToTextfileNewLine("------------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => LogInAccountsForPhotoLikes :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("------------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
            }
        }

        private void PhotoLikeProcess(object accountdata)
        {

            try
            {
                GloBoardPro.lstThread.Add(Thread.CurrentThread);
                Thread.CurrentThread.IsBackground = true;
                GloBoardPro.lstThread = GloBoardPro.lstThread.Distinct().ToList();
            }
            catch { };
            try
            {
                Array accountinfo = (Array)accountdata;

                string account = (string)accountinfo.GetValue(0);
                string proxyValue = string.Empty;
                string[] accountAndPass = account.Split(':');
                string AccountName = string.Empty;
                string AccountPass = string.Empty;
                if (accountAndPass.Length == 2)
                {
                    AccountName = accountAndPass[0].Replace("\0", string.Empty).Trim();
                    AccountPass = accountAndPass[1].Replace("\0", string.Empty).Trim();
                }
                else if (accountAndPass.Length == 4 || accountAndPass.Length == 6)
                {
                    AccountName = accountAndPass[0].Replace("\0", string.Empty).Trim();
                    AccountPass = accountAndPass[1].Replace("\0", string.Empty).Trim();
                    if (accountAndPass.Length == 4)
                    {
                        proxyValue = accountAndPass[2] + ":" + accountAndPass[3];
                    }
                    else if (accountAndPass.Length == 6)
                    {
                        proxyValue = accountAndPass[2] + ":" + accountAndPass[3] + ":" + accountAndPass[4] + ":" + accountAndPass[5];
                    }
                }
                else
                {
                    AddToLogger("[ " + DateTime.Now + " ] => [ Please Enter Accounts In Correct Format ]");
                    return;
                }
                if (ClGlobul.finalProxyList.Count > 0)
                {
                    lock (StagramProxyChecker.classes.ClProxyChecker.locker_finalProxyList)
                    {
                        if (ClGlobul.finalProxyList.Count == 0)
                        {
                            AddToLogger("[ " + DateTime.Now + " ] => [ Waiting For Proxies To Be Loaded ]");
                            Monitor.Wait(StagramProxyChecker.classes.ClProxyChecker.locker_finalProxyList);
                        }
                    }
                    try
                    {
                        string[] tempQueue = ClGlobul.ProxyQueue.Dequeue();
                        if (tempQueue.Length > 0)
                        {
                            proxyValue = tempQueue[0];
                        }
                    }
                    catch (Exception ex)
                    {
                        GramBoardProFileHelper.AppendStringToTextfileNewLine("------------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                        GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => PhotoLikeProcess :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                        GramBoardProFileHelper.AppendStringToTextfileNewLine("------------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);

                    }
                }

                string acc = account.Remove(account.IndexOf(':'));

                Thread PhotolikerThread = new Thread(getloginForPhotoLike);
                PhotolikerThread.Name = "workerThread_PhotoLiker_" + acc;
                PhotolikerThread.IsBackground = true;

                PhotolikerThread.Start(new object[] { (AccountName), (AccountPass), proxyValue });

                count_ThreadControllerForPhotoLike++;
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("------------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => PhotoLikeProcess :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("------------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
            }
            //}
            if (ClGlobul.ProxyQueue.Count == 0)
            {
                ClGlobul.AddProxyInQueur();
            }
        }

        List<Thread> Lst_photoLikethread = new List<Thread>();
        bool PhotoLikeIstrueOrFals = false;
        public void getloginForPhotoLike(object Accounts)
        {
            try
            {
                if (!PhotoLikeIstrueOrFals)
                {
                    try
                    {
                        GloBoardPro.lstThread.Add(Thread.CurrentThread);
                        Thread.CurrentThread.IsBackground = true;
                        GloBoardPro.lstThread = GloBoardPro.lstThread.Distinct().ToList();
                    }
                    catch { };

                    Array accountinfo = (Array)Accounts;

                    string account = (string)accountinfo.GetValue(0);
                    string pass = (string)accountinfo.GetValue(1);
                    string ProxyValue = (string)accountinfo.GetValue(2);
                    string proxyadd = string.Empty;
                    string proxyport = string.Empty;
                    string proxyUser = string.Empty;
                    string proxyPass = string.Empty;
                    if (!string.IsNullOrEmpty(ProxyValue))
                    {
                        string[] proxyarray = ProxyValue.Split(':');
                        if (proxyarray.Count() == 4)
                        {
                            proxyadd = proxyarray[0];
                            proxyport = proxyarray[1];
                            proxyUser = proxyarray[2];
                            proxyPass = proxyarray[3];
                        }
                        else if (proxyarray.Count() == 2)
                        {
                            proxyadd = proxyarray[0];
                            proxyport = proxyarray[1];
                        }
                        else
                        {
                            AddToLogger("[ " + DateTime.Now + " ] => [ No Proxy for Account : " + account + " ]");
                        }
                    }
                    try
                    {
                        InstagramManager.Classes.InstagramAccountManager InstagramAccountManager = new InstagramManager.Classes.InstagramAccountManager(account, pass, proxyadd, proxyport, proxyUser, proxyPass);

                        string status = InstagramAccountManager.MyLoginandComment(ref InstagramAccountManager.httpHelper, "", "", "");
                        if (status.Contains("Stream was not readable."))
                        {
                            status = InstagramAccountManager.MyLoginandComment(ref InstagramAccountManager.httpHelper, "", "", "");
                        }

                        if (InstagramAccountManager.LoggedIn == true)
                        {
                           // AddTophotoLogger("[ " + DateTime.Now + " ] => [ Logged In From : " + InstagramAccountManager.Username + " ]");
                           // AddTophotoLogger("[ " + DateTime.Now + " ] => [ Starting Photo Like From : " + InstagramAccountManager.Username + " ]");
                            getPhotoLike(ref InstagramAccountManager);

                        }

                        else if (status.Contains("Failed"))
                        {
                            //AddTophotoLogger("[ " + DateTime.Now + " ] => [ Failed To Login From : " + InstagramAccountManager.Username + " ]");

                        }
                        else if (status.Contains("AccessIssue"))
                        {
                          //  AddTophotoLogger("[ " + DateTime.Now + " ] => [ Failed To Login From : " + InstagramAccountManager.Username + " ]");

                        }
                        else if (status.Contains("Stream was not readable."))
                        {

                            //AddTophotoLogger("[ " + DateTime.Now + " ] => [ Failed To Login From : " + InstagramAccountManager.Username + " ]");
                            try
                            {
                                string removeingProxy = ClGlobul.finalProxyList.Where(e => e.Contains(ProxyValue)).ToArray()[0];
                                ClGlobul.finalProxyList.Remove(removeingProxy);
                            }
                            catch { };

                        }
                        else if (status.Contains("The request was aborted: The operation has timed out."))
                        {

                          //  AddTophotoLogger("[ " + DateTime.Now + " ] => [ Failed To Login From : " + InstagramAccountManager.Username + " ]");

                        }
                        else if (status.Contains("503"))
                        {
                           // AddTophotoLogger("[ " + DateTime.Now + " ] => [ Failed To Login From : " + InstagramAccountManager.Username + " ]");
                        }
                        else if (status.Contains("403"))
                        {
                          //  AddTophotoLogger("[ " + DateTime.Now + " ] => [ Failed To Login From : " + InstagramAccountManager.Username + " ]");
                        }
                        else
                        {
                           // AddTophotoLogger("[ " + DateTime.Now + " ] => [ Failed To Login From : " + InstagramAccountManager.Username + " ]");

                        }
                    }
                    catch (Exception ex)
                    {

                        GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                        GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => GetLoginForPhotoLikes (1):=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                        GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                    }
                }
            }
            catch (Exception ex)
            {

                GramBoardProFileHelper.AppendStringToTextfileNewLine("------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => GetLoginForPhotoLikes (2) :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
            }
            finally
            {
                count_ThreadControllerForPhotoLike--;
                lock (lockr_ThreadControllerForPhotoLike)
                {
                    if (!PhotoLikeIstrueOrFals)
                    {
                        Monitor.Pulse(lockr_ThreadControllerForPhotoLike);
                    }
                }
            }
        }

        public void getPhotoLike(ref InstagramManager.Classes.InstagramAccountManager accountManager)
        {
            InstagramManager.Classes.InstagramPhotoLike InstagramPhotoLike = new InstagramManager.Classes.InstagramPhotoLike();
            try
            {
                foreach (string PhotoList_item in ClGlobul.PhotoList)
                {

                    string query = "select * from LikeInfo where UseName='" + accountManager.Username + "' and LikePhotoId='" + PhotoList_item + "'";
                    DataSet ds = DataBaseHandler.SelectQuery(query, "LikeInfo");
                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        try
                        {
                            string LikeName = PhotoList_item;
                            // string Result = InstagramPhotoLike.photolike(LikeName, ref accountManager);
                            string photoId = string.Empty;

                            if (PhotoList_item.Contains("\0"))
                            {
                                photoId = PhotoList_item.Replace("\0", string.Empty).Trim();
                            }
                            else
                            {
                                photoId = PhotoList_item;
                            }
                            string Result = string.Empty;
                            try
                            {
                                Result = InstagramPhotoLike.photolike(photoId, ref accountManager);
                            }
                            catch { }


                            if (Result.Contains("LIKED") && !Result.Contains("All ready LIKED"))
                            {
                                try
                                {
                                    //QueryExecuter.insertPhotoId(accountManager.Username, photoId);
                                    QueryExecuter.insertLikeStatus(photoId, accountManager.Username, 1);


                                }
                                catch (Exception ex)
                                {
                                    GramBoardProFileHelper.AppendStringToTextfileNewLine("------------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                                    GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => getPhotoLike :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                                    GramBoardProFileHelper.AppendStringToTextfileNewLine("------------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                                }

                                try
                                {
                                    if (!ClGlobul.photoLikesCompletedList.Contains(accountManager.Username))// + ":" + accountManager.Password + ":" + accountManager.proxyAddress + ":" + accountManager.proxyPort + ":" + accountManager.proxyUsername + ":" + accountManager.proxyPassword))
                                    {
                                        ClGlobul.photoLikesCompletedList.Add(accountManager.Username);// + ":" + accountManager.Password + ":" + accountManager.proxyAddress + ":" + accountManager.proxyPort + ":" + accountManager.proxyUsername + ":" + accountManager.proxyPassword);
                                    }
                                    GramBoardProFileHelper.AppendStringToTextfileNewLine(accountManager.Username + ":" + photoId, GramBoardProFileHelper.LikePhotoAccountIdFilePath);
                                    // QueryExecuter.UpdateStatusPhotoId(accountManager.Username, photoId);

                                }
                                catch (Exception ex)
                                {
                                    GramBoardProFileHelper.AppendStringToTextfileNewLine("------------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                                    GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => getPhotoLike :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                                    GramBoardProFileHelper.AppendStringToTextfileNewLine("------------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);

                                }
                                try
                                {
                                    AddTophotoLogger("[ " + DateTime.Now + " ] => [ " + accountManager.Username + "   LIKED : " + PhotoList_item + " ]");
                                }
                                catch
                                {
                                    AddTophotoLogger("[ " + DateTime.Now + " ] => [ " + accountManager.Username + "   All ready LIKED : " + PhotoList_item + " ]");
 
                                }           
                            }
                            else if (Result.Contains("All ready LIKED"))
                            {
                                try
                                {
                                    GramBoardProFileHelper.AppendStringToTextfileNewLine(accountManager.Username + ":" + photoId, GramBoardProFileHelper.AllReadylikePhotoAccountIdFilePath);
                                }
                                catch (Exception ex)
                                {
                                    GramBoardProFileHelper.AppendStringToTextfileNewLine("------------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                                    GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => getPhotoLike :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                                    GramBoardProFileHelper.AppendStringToTextfileNewLine("------------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                                }

                                AddTophotoLogger("[ " + DateTime.Now + " ] => [ " + accountManager.Username + " All ready LIKED :  " + PhotoList_item + " ]");
                            }
                            else
                            {
                                try
                                {
                                    GramBoardProFileHelper.AppendStringToTextfileNewLine(accountManager.Username + ":" + photoId, GramBoardProFileHelper.NotlikePhotoAccountIdFilePath);
                                }
                                catch (Exception ex)
                                {
                                    GramBoardProFileHelper.AppendStringToTextfileNewLine("------------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                                    GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => getPhotoLike :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                                    GramBoardProFileHelper.AppendStringToTextfileNewLine("------------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                                }

                                AddTophotoLogger("[ " + DateTime.Now + " ] => [ " + accountManager.Username + " is Not LIKED : " + PhotoList_item + " ]");
                            }

                            if (!string.IsNullOrEmpty(txtdelaymin.Text) && NumberHelper.ValidateNumber(txtdelaymin.Text))
                            {
                                mindelay = Convert.ToInt32(txtdelaymin.Text);
                            }
                            if (!string.IsNullOrEmpty(txtdelaymax.Text) && NumberHelper.ValidateNumber(txtdelaymax.Text))
                            {
                                maxdelay = Convert.ToInt32(txtdelaymax.Text);
                            }

                            lock (_lockObject)
                            {
                                Random rn = new Random();
                                int delay = RandomNumberGenerator.GenerateRandom(mindelay, maxdelay);
                                delay = rn.Next(mindelay,maxdelay);
                                AddTophotoLogger("[ " + DateTime.Now + " ] => [ Delay For " + delay + " Seconds For " + accountManager.Username + " ]");
                                Thread.Sleep(delay * 1000);
                            }
                        }
                        catch (Exception ex)
                        {

                            GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => GetPhotolike (1) :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForPhotolike);

                        }
                    }
                    else
                    {
                        AddTophotoLogger("[ " + DateTime.Now + " ] => [ " + accountManager.Username + " All ready LIKED :  " + PhotoList_item + " ]");
                    }
                }
            }
            catch (Exception ex)
            {

                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => GetPhotolike (2)  :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForPhotolike);

            }
            finally
            {
                AddTophotoLogger("--------------------------------------------------------------------------------------------");
                AddTophotoLogger("[ " + DateTime.Now + " ] => [ Photo Like is Finished From =>" + accountManager.Username + " ]");
                AddTophotoLogger("--------------------------------------------------------------------------------------------");
            }
        }

        private void AddTophotoLogger(string log)
        {
            try
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    lst_PhotoLogger.Items.Add(log);
                    lst_PhotoLogger.SelectedIndex = lst_PhotoLogger.Items.Count - 1;
                }));
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("------------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => AddTophotoLogger :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("------------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
            }

        }

        private void btn_AccountClear_Click(object sender, EventArgs e)
        {
            try
            {
                txtAddAccounts1.Text = string.Empty;
                txt_photoListPath.Text = string.Empty;
                txt_noofThread.Text = string.Empty;

                ClGlobul.accountList.Clear();
                ClGlobul.photoLikesCompletedList.Clear();
                ClGlobul.PhotoList.Clear();
                ClGlobul.NoOfPhotoLikeThread = 0;

                Lst_photoLikethread.Clear();
                lst_Thread.Clear();
                PhotoLikeIstrueOrFals = false;
                lst_PhotoLogger.Items.Clear();
                lbLogger.Items.Clear();
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("------------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => btn_AccountClear :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("------------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
            }
        }


        private static int unlikeCompletionCount = 0;
        private static bool _boolUnlike = false;
        private void btn_NotLikes_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txt_photoListPath.Text) && string.IsNullOrEmpty(txtphotosingaluser.Text))
            {
                MessageBox.Show("Please Upload  a Photo Id First");
                return;
            }
            if (!string.IsNullOrEmpty(txtphotosingaluser.Text) && string.IsNullOrEmpty(txt_photoListPath.Text))
            {
                ClGlobul.PhotoList.Clear();
                ClGlobul.PhotoList.Add(txtphotosingaluser.Text);
            }
            try
            {
                //if (string.IsNullOrEmpty(txt_photoListPath.Text))
                //{
                //    MessageBox.Show("Please Upload a Photo Id First");
                //    return;
                //}
            }
            catch { };


            #region Old Commented Code
            //if (ClGlobul.photoLikesCompletedList.Count() != 0)
            //{
            //    foreach (string photlikeConpletedList_item in ClGlobul.photoLikesCompletedList)
            //    {
            //        string Accountdata = string.Empty;
            //        foreach(string data in ClGlobul.accountList)
            //        {
            //            if(data.Contains(photlikeConpletedList_item))
            //            {
            //                Accountdata = data;
            //                break;
            //            }
            //        }
            //        ClGlobul.accountList.Remove(Accountdata);
            //    }

            //    foreach (string accountList_item in ClGlobul.accountList)
            //    {
            //        GlobusFileHelper.AppendStringToTextfileNewLine(accountList_item, GlobusFileHelper.NotlikePhotoAccountIdFilePath);
            //    }

            //    AddToLogger("[ " + DateTime.Now + " ] => [ Stored all Remaining account. ]");
            //}
            //else
            //{
            //    AddTophotoLogger("[ " + DateTime.Now + " ] => [ Please start Process. ]");
            //    MessageBox.Show("This feature is working after Process start.");
            //}
            #endregion
            int parsedValue;
            if (!int.TryParse(txt_noOfphotoLikethread.Text, out parsedValue))
            {

                AddToCommentLogger("[ " + DateTime.Now + " ] => [ This is a number only field ]");
                return;
            }


            if (!string.IsNullOrEmpty(txtphotosingaluser.Text.Trim()))
            {
                //add following in followingList...
                lstStoreDownloadImageKeyword.Clear();

                string s = txtphotosingaluser.Text.ToString();

                if (s.Contains(','))
                {
                    string[] Data = s.Split(',');

                    foreach (var item in Data)
                    {


                        ClGlobul.PhotoList.Add(item);
                    }
                }
                else
                {
                    ClGlobul.PhotoList.Add(txtphotosingaluser.Text.ToString());
                }
            }
            bool ProcessStartORnot = false;

            if (ClGlobul.accountList.Count > 0)
            {
                if (ClGlobul.PhotoList.Count > 0)
                {
                    unlikeCompletionCount = 0;
                    Lst_photoLikethread.Clear();
                    _boolUnlike = false;
                    AddTophotoLogger("[ " + DateTime.Now + " ] => [ Starting Photo Unlike Process ]");
                    ThreadPool.SetMaxThreads(5, 5);
                    foreach (string accounts in ClGlobul.accountList)
                    {
                        ThreadPool.QueueUserWorkItem(new WaitCallback(unlike), new object[] { accounts });
                    }
                }
                //else
                //{
                //    AddTophotoLogger("[ " + DateTime.Now + " ] => [ Please upload Photo Ids. ]");
                //}
            }
            else
            {
                AddTophotoLogger("[ " + DateTime.Now + " ] => [ Please Upload Accounts. ]");
                MessageBox.Show("Please Upload Accounts.", "Please Upload Accounts.");
                tab_instagram.SelectedIndex = 0;
            }
        }

        private void unlike(object parameters)
        {
            try
            {
                try
                {
                    GloBoardPro.lstThread.Add(Thread.CurrentThread);
                    Thread.CurrentThread.IsBackground = true;
                    GloBoardPro.lstThread = GloBoardPro.lstThread.Distinct().ToList();
                }
                catch { };

                unlikeCompletionCount++;
                Array accountinfo = (Array)parameters;
                string account = (string)accountinfo.GetValue(0);
                string UserName = string.Empty;
                string Password = string.Empty;
                UserName = Regex.Split(account, ":")[0];
                Password = Regex.Split(account, ":")[1];

                InstagramManager.Classes.InstagramAccountManager InstagramAccountManager = new InstagramManager.Classes.InstagramAccountManager(UserName, Password, "", "", "", "");
                string statuse = InstagramAccountManager.MyLoginandComment(ref InstagramAccountManager.httpHelper, "", "", "");

                if (_boolUnlike) return;
                if (InstagramAccountManager.LoggedIn == true)
                {
                    if (_boolUnlike) return;
                    AddTophotoLogger("[ " + DateTime.Now + " ] => [ Logged In From : " + InstagramAccountManager.Username + " ]");
                    if (_boolUnlike) return;
                    AddTophotoLogger("[ " + DateTime.Now + " ] => [ Starting Photo Unlike From : " + InstagramAccountManager.Username + " ]");
                    if (_boolUnlike) return;
                    getPhotoUnlike(ref InstagramAccountManager);
                    //break;
                }

                #region If-Else Conditions
                else if (statuse.Contains("Failed"))
                {
                    if (_boolUnlike) return;
                    AddTophotoLogger("[ " + DateTime.Now + " ] => [ Failed To Login From : " + InstagramAccountManager.Username + " ]");
                    //break;
                }
                else if (statuse.Contains("AccessIssue"))
                {
                    if (_boolUnlike) return;
                    AddTophotoLogger("[ " + DateTime.Now + " ] => [ Failed To Login From : " + InstagramAccountManager.Username + " ]");
                    //AddTophotoLogger("AccessIssue");
                    //break;
                }
                else if (statuse.Contains("Stream was not readable."))
                {
                    if (_boolUnlike) return;
                    //proxy is currect but have some login issues 
                    //AddTophotoLogger(" Stream was not readable.");
                    AddTophotoLogger("[ " + DateTime.Now + " ] => [ Failed To Login From : " + InstagramAccountManager.Username + " ]");
                    //try
                    //{
                    //    string removeingProxy = ClGlobul.finalProxyList.Where(e => e.Contains(ProxyValue)).ToArray()[0];
                    //    ClGlobul.finalProxyList.Remove(removeingProxy);
                    //}
                    //catch { };
                    //break;
                }
                else if (statuse.Contains("The request was aborted: The operation has timed out."))
                {
                    if (_boolUnlike) return;
                    //proxy is currect but request time out 
                    AddTophotoLogger("[ " + DateTime.Now + " ] => [ Failed To Login From : " + InstagramAccountManager.Username + " ]");
                    //AddTophotoLogger("The request was aborted: The operation has timed out.");
                    //break;
                }
                else if (statuse.Contains("503"))
                {
                    if (_boolUnlike) return;
                    //some request problem 
                    //AddTophotoLogger("503 some request problem /Server unavailable.");
                    AddTophotoLogger("[ " + DateTime.Now + " ] => [ Failed To Login From : " + InstagramAccountManager.Username + " ]");
                    //break;
                }
                else if (statuse.Contains("403"))
                {
                    if (_boolUnlike) return;
                    AddTophotoLogger("[ " + DateTime.Now + " ] => [ Failed To Login From : " + InstagramAccountManager.Username + " ]");
                    //AddTophotoLogger("403");
                    //break;
                }
                else
                {
                    if (_boolUnlike) return;
                    AddTophotoLogger("[ " + DateTime.Now + " ] => [ Failed To Login From : " + InstagramAccountManager.Username + " ]");
                    //AddTophotoLogger(account + " username and password is wrong.");
                    // break;
                }
                #endregion

            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("------------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => unlike :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("------------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);

            }
            finally
            {
                unlikeCompletionCount--;
                lock (_lockObject)
                {
                    if (unlikeCompletionCount == 0)
                    {
                        AddTophotoLogger("[ " + DateTime.Now + " ] => [ Process Completed ]");
                    }
                }
            }
        }

        private void getPhotoUnlike(ref InstagramManager.Classes.InstagramAccountManager accountManager)
        {

            foreach (string itemPhotos in ClGlobul.PhotoList)
            {
                if (_boolUnlike) return;

                try
                {
                    GloBoardPro.lstThread.Add(Thread.CurrentThread);
                    Thread.CurrentThread.IsBackground = true;
                    GloBoardPro.lstThread = GloBoardPro.lstThread.Distinct().ToList();
                }
                catch { };
                try
                {
                    string pageSource = string.Empty;
                    pageSource = accountManager.httpHelper.getHtmlfromUrlProxy(new Uri("http://websta.me/p/" + itemPhotos), "", 80, "", "");
                    if (!string.IsNullOrEmpty(pageSource))
                    {
                        string like = string.Empty;
                        if (pageSource.Contains("likeButton") & pageSource.Contains("</button>"))
                        {
                            try
                            {
                                like = ScrapUserName.getBetween(pageSource, "likeButton", "</button>");
                            }
                            catch (Exception ex)
                            {
                                GramBoardProFileHelper.AppendStringToTextfileNewLine("------------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => getPhotoUnlike :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                                GramBoardProFileHelper.AppendStringToTextfileNewLine("------------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                            }
                        }

                        if (like.Contains("Liked"))
                        {
                            string url_Unlike = "http://websta.me/api/remove_like/";
                            url_Unlike += itemPhotos;
                            string response = string.Empty;
                            response = accountManager.httpHelper.getHtmlfromUrlProxy(new Uri(url_Unlike), "", 80, "", "");

                            if (!string.IsNullOrEmpty(response))
                            {
                                if (response.Contains("OK"))
                                {
                                    if (_boolUnlike) return;
                                    AddTophotoLogger("[ " + DateTime.Now + " ] => [ Unliked " + itemPhotos + " From : " + accountManager.Username + " ]");
                                }
                                else
                                {
                                    if (_boolUnlike) return;
                                    AddTophotoLogger("[ " + DateTime.Now + " ] => [ Failed To Unlike From : " + accountManager.Username + " ]");
                                }
                            }
                            else
                            {
                                if (_boolUnlike) return;
                                AddTophotoLogger("[ " + DateTime.Now + " ] => [ Failed To Unlike From : " + accountManager.Username + " ]");
                            }

                        }//End of if (like.Contains("Liked"))




                        else
                        {
                            if (_boolUnlike) return;
                            AddTophotoLogger("[ " + DateTime.Now + " ] => [ " + itemPhotos + " is not liked previously From : " + accountManager.Username + " ]");
                        }

                        if (!string.IsNullOrEmpty(txtdelaymin.Text) && NumberHelper.ValidateNumber(txtdelaymin.Text))
                        {
                            mindelay = Convert.ToInt32(txtdelaymin.Text);
                        }
                        if (!string.IsNullOrEmpty(txtdelaymax.Text) && NumberHelper.ValidateNumber(txtdelaymax.Text))
                        {
                            maxdelay = Convert.ToInt32(txtdelaymax.Text);
                        }

                        lock (_lockObject)
                        {
                            Random rn = new Random();
                            int delay = RandomNumberGenerator.GenerateRandom(mindelay, maxdelay);
                            delay = rn.Next(mindelay,maxdelay);
                            AddTophotoLogger("[ " + DateTime.Now + " ] => [ Delay For " + delay + " Seconds For " + accountManager.Username + " ]");
                            Thread.Sleep(delay * 1000);
                        }

                    }//End of if (!string.IsNullOrEmpty(pageSource))
                }
                catch (Exception ex)
                {
                    GramBoardProFileHelper.AppendStringToTextfileNewLine("------------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                    GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => getPhotoUnlike :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                    GramBoardProFileHelper.AppendStringToTextfileNewLine("------------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                }
            }
        }

        #endregion

        //************************************************************************* Stope All Process ***************************************************//

        #region Stope All Process

        private void btn_StopFollow_Click(object sender, EventArgs e)
        {
            try
            {
                new Thread(() =>
                     {
                         ThreadIs = true;
                         List<Thread> tempLst = new List<Thread>();
                         tempLst.AddRange(lst_Thread);
                         foreach (Thread tempLst_item in tempLst)
                         {

                             try
                             {
                                 tempLst_item.Abort();
                                 lst_Thread.Remove(tempLst_item);
                                 //AddToLogger("{0}" + tempLst_item.CurrentCulture.Name);
                             }
                             catch (Exception ex)
                             {
                                 GramBoardProFileHelper.AppendStringToTextfileNewLine("------------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                                 GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => btn_StopFollow :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                                 GramBoardProFileHelper.AppendStringToTextfileNewLine("------------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                             }
                         }
                         AddToLogger("[ " + DateTime.Now + " ] => [ All Process is Aborted ]");
                     }).Start();
            }
            catch (Exception ex)
            {
                AddTophotoLogger("[ " + DateTime.Now + " ] => [ Error : StopFollow" + ex.Message + " ]");
            }

            try
            {
                new Thread(() => stopUnfollow()).Start();
            }
            catch (Exception ex)
            {
                AddTophotoLogger("[ " + DateTime.Now + " ] => [ Error : StopFollow" + ex.Message + " ]");
            }
        }

        private void stopUnfollow()
        {
            try
            {
                _boolStopUnfollow = true;
                AddToLogger("[ " + DateTime.Now + " ] => [ Unfollow Process Aborted ]");
            }
            catch (Exception ex)
            {
                AddToLogger("[ " + DateTime.Now + " ] => [ Unfollow Process Aborted ]");

            }
        }

       
        public static bool stopflag = true;

        private void btn_stopPhotoLike_Click(object sender, EventArgs e)
         
        {
            if (System.Windows.Forms.MessageBox.Show("Do you really want to stop now?", "Expedia Scraper", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                stopflag = false;
                List<Thread> lstTemp = GloBoardPro.lstThread.Distinct().ToList();

                foreach (Thread item in lstTemp)
                {
                    try
                    {
                        item.Abort();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }
                }
                AddTophotoLogger("[ " + DateTime.Now + " ] => [ Process Stopped ]");
               

           }

            //try
            //{
            //    new Thread(() =>
            //           {
            //               PhotoLikeIstrueOrFals = true;
            //               _boolUnlike = true;
            //               List<Thread> tempLst = new List<Thread>();
            //               tempLst.AddRange(Lst_photoLikethread);
            //               foreach (Thread tempLst_item in tempLst)
            //               {
            //                   try
            //                   {
            //                       tempLst_item.Abort();
            //                       Lst_photoLikethread.Remove(tempLst_item);
            //                   }
            //                   catch (Exception ex)
            //                   {
            //                       AddToLogger("[ " + DateTime.Now + " ] => [ Process Stopped ]");
            //                   }
            //               }
            //               AddTophotoLogger("[ " + DateTime.Now + " ] => [ Process Stopped ]");
            //          }).Start();
            //    //Lst_photoLikethread
            //}
            //catch (Exception ex)
            //{
            //    AddTophotoLogger("Error : stopPhotoLike_" + ex.Message);
            //}
        }

        #endregion

        //*************************************************************************** Validations *****************************************************//

        #region Validation

        public bool pathValidation(string value)
        {
            try
            {
                System.Text.RegularExpressions.Regex regexObj = new System.Text.RegularExpressions.Regex(@"(([a-z]:|\\\\[a-z0-9_.$]+\\[a-z0-9_.$]+)?(\\?(?:[^\\/:*?""<>|\r\n]+\\)+)[^\\/:*?""<>|\r\n]+)");
                System.Text.RegularExpressions.Match matchResult = regexObj.Match(value);

                if (matchResult.Success)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                AddTophotoLogger("Error : pathValidation" + ex.Message);
            }
            return false;
        }


        public static bool ValidateNumber(string strInputNo)
        {
            Regex IdCheck = new Regex("^[0-9]*$");

            if (!string.IsNullOrEmpty(strInputNo) && IdCheck.IsMatch(strInputNo))
            {
                return true;
            }

            return false;
        }


        #endregion

        //************************************************ Comment **********************************************************************************//

        #region Comment Module
        private void btn_UploadForCommentIds_Click(object sender, EventArgs e)
        {
            try
            {
                //get the comment id from selected file...

                using (OpenFileDialog ofdphotolike = new OpenFileDialog())
                {
                    ofdphotolike.InitialDirectory = Application.StartupPath;
                    ofdphotolike.Filter = "Text Files (*.txt)|*.txt";
                    if (ofdphotolike.ShowDialog() == DialogResult.OK)
                    {
                        //Add File path in Text Box....
                        txt_UploadForCommentIds.Text = ofdphotolike.FileName;
                        //call Read Method file when we select a file 
                        readcommentidFile(ofdphotolike.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("***********************************************************************************************", GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => btn_UploadForCommentIds :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("***********************************************************************************************", GramBoardProFileHelper.ErrorLogFilePathForComment);
            }

        }


        public void readcommentidFile(string commentidFilePath)
        {
            try
            {
                ClGlobul.CommentIdsForMSG.Clear();
                //Read Data From Selected File ....
                List<string> commentidlist = GramBoardProFileHelper.ReadFile((string)commentidFilePath);
                foreach (string commentidlist_item in commentidlist)
                {
                    //add Comment Id's In Globol Comment Id List ...
                    ClGlobul.CommentIdsForMSG.Add(commentidlist_item);
                }
                ClGlobul.CommentIdsForMSG = ClGlobul.CommentIdsForMSG.Distinct().ToList();
                //Show No Of Data Count In logger...
                AddToCommentLogger("[ " + DateTime.Now + " ] => [ " + ClGlobul.CommentIdsForMSG.Count + " Image IDs Uploaded. ]");
            }
            catch (Exception ex)
            {
                //when its generate any Error From here which is write in Text file ...
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => ReadLargeCommentIdFile :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
            }
        }

        private void btn_UploadCommentFile_Click(object sender, EventArgs e)
        {
            try
            {
                //get the comment file and MSG's from selected file...

                using (OpenFileDialog ofComment = new OpenFileDialog())
                {
                    ofComment.InitialDirectory = Application.StartupPath;
                    ofComment.Filter = "Text Files (*.txt)|*.txt";
                    if (ofComment.ShowDialog() == DialogResult.OK)
                    {
                        txt_UploadedFilepath.Text = ofComment.FileName;
                        //read 
                        readcommentFile(ofComment.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => btn_UploadCommentFile:=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
            }
        }

        public void readcommentFile(string commentFilePath)
        {
            try
            {
                ClGlobul.commentMsgList.Clear();
                List<string> MSGlist = GramBoardProFileHelper.ReadFile((string)commentFilePath);
                foreach (string MSGlist_item in MSGlist)
                {
                    //add Photo Id's In maine photo list...
                    ClGlobul.commentMsgList.Add(MSGlist_item);
                }
                ClGlobul.commentMsgList = ClGlobul.commentMsgList.Distinct().ToList();
                AddToCommentLogger("[ " + DateTime.Now + " ] => [ " + ClGlobul.commentMsgList.Count + " Messages Uploaded. ]");
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => ReadLargeCommentMSGFile :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
            }
        }

        private void btn_startComment_Click(object sender, EventArgs e)
        {
            try
            {

                if (string.IsNullOrEmpty(txt_UploadForCommentIds.Text) && string.IsNullOrEmpty(txt_UploadedFilepath.Text) && string.IsNullOrEmpty(txtusercomment.Text) && string.IsNullOrEmpty(txtsingalmsg.Text))
                {
                    MessageBox.Show("Please Upload Comment ID and Comment Messege");
                    return;
                }
            }
            catch { };

            try
            {
                bool ProcessStartORnot = false;


                int parsedValue;
                if (!int.TryParse(txt_NoOfCommentThread.Text, out parsedValue))
                {

                    AddToCommentLogger("[ " + DateTime.Now + " ] => [ This is a number only field ]");
                    return;
                }


                if (!string.IsNullOrEmpty(txtusercomment.Text.Trim()))
                {
                    //add following in followingList...
                    lstStoreDownloadImageKeyword.Clear();

                    string s = txtusercomment.Text.ToString();

                    if (s.Contains(','))
                    {
                        string[] Data = s.Split(',');

                        foreach (var item in Data)
                        {


                            ClGlobul.CommentIdsForMSG.Add(item);
                        }
                    }
                    else
                    {
                        ClGlobul.CommentIdsForMSG.Add(txtusercomment.Text.ToString());
                    }

                }

                //bool ProcessStartORnot = false;

                if (ClGlobul.accountList.Count() != 0)
                {
                   // if (!string.IsNullOrEmpty(txt_UploadForCommentIds.Text.Trim()) && pathValidation(txt_UploadForCommentIds.Text.Trim()))
                        if (!string.IsNullOrEmpty(txtAddAccounts1.Text.Trim()) && pathValidation(txtAddAccounts1.Text.Trim()))
                    {
                        if (!string.IsNullOrEmpty(txt_UploadedFilepath.Text.Trim()) && pathValidation(txt_UploadedFilepath.Text.Trim()))
                        {
                            if (string.IsNullOrEmpty(txt_NoOfCommentThread.Text.Trim()))
                            {
                                if (MessageBox.Show("Do you really want to Start Without Thread", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                {
                                    ProcessStartORnot = true;
                                    ClGlobul.NoOfcommentThread = 1;
                                }
                                else
                                {
                                }
                            }
                            else
                            {
                                try
                                {
                                    ClGlobul.NoOfcommentThread = Convert.ToInt16(txt_NoOfCommentThread.Text.Trim());
                                    ProcessStartORnot = true;
                                }
                                catch (Exception)
                                {
                                    MessageBox.Show("Please Enter Numeric Value in Comment Thread..!!", "Wrong Value Insert In Commnet Thread");
                                }
                            }
                        }


                        else if (!string.IsNullOrEmpty(txtsingalmsg.Text.Trim()) && string.IsNullOrEmpty(txt_UploadedFilepath.Text.Trim()))//txtsingalmsg
                        {
                            ClGlobul.commentMsgList.Clear();
                            if (string.IsNullOrEmpty(txt_NoOfCommentThread.Text.Trim()))
                            {
                                if (MessageBox.Show("Do you really want to Start Without Thread", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)  //txtsingalmsg
                                {

                                    ProcessStartORnot = true;
                                    ClGlobul.NoOfcommentThread = 1;
                                    try
                                    {
                                        string AllMessege = txtsingalmsg.Text.Trim();
                                        string[] ListMessages = Regex.Split(AllMessege, ",");
                                        foreach (string str in ListMessages)
                                        {
                                            ClGlobul.commentMsgList.Add(str);
                                        }
                                    }
                                    catch { };
                                }
                                else
                                {
                                }
                            }
                            else
                            {
                                try
                                {
                                    ClGlobul.NoOfcommentThread = Convert.ToInt16(txt_NoOfCommentThread.Text.Trim());  // ClGlobul.commentMsgList = ClGlobul.commentMsgList.Distinct().ToList();
                                    ProcessStartORnot = true;
                                    try
                                    {
                                        string AllMessege = txtsingalmsg.Text.Trim();
                                        string[] ListMessages = Regex.Split(AllMessege, ",");
                                        foreach (string str in ListMessages)
                                        {
                                            ClGlobul.commentMsgList.Add(str);
                                        }
                                    }
                                    catch { };
                                    //  ClGlobul.commentMsgList.Add(txtsingalmsg.Text.Trim());
                                }
                                catch (Exception)
                                {
                                    MessageBox.Show("Please Enter Numeric Value in Comment Thread..!!", "Wrong Value Insert In Commnet Thread");
                                }
                            }
                        }

                        else
                        {
                            AddToCommentLogger("[ " + DateTime.Now + " ] => [ Please upload Comments. ]");
                            MessageBox.Show("Please upload Comments.", "Please upload Comments Messege.");
                        }

                    }


                    else
                    {
                        AddToCommentLogger("[ " + DateTime.Now + " ] => [ Please upload Comment Id's . ]");
                        MessageBox.Show("Please upload Comment Id .", "Please upload Comment Id.");
                    }

                }
                else
                {
                    AddToCommentLogger("[ " + DateTime.Now + " ] => [ Please Upload Accounts. ]");
                    MessageBox.Show("Please Upload Accounts.", "Please Upload Accounts.");
                    tab_instagram.SelectedIndex = 0;
                }

                if (ProcessStartORnot)
                {
                    Lst_Commentthread.Clear();
                    CommentIstrueOrFals = false;

                    new Thread(() =>
                    {
                        logInAccountsForcomment();

                    }).Start();
                }
                //}
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => btn_startComment :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
            }
        }

        readonly object lockr_ThreadControllerForComment = new object();
        int count_ThreadControllerForComment = 0;
        int counter_comment = 0;
        public void logInAccountsForcomment()
        {
            try
            {
                AddToCommentLogger("[ " + DateTime.Now + " ] => [ Start Commenting On Photos ]");
                int numberOfThreads = 0;
                if (ClGlobul.NoOfcommentThread != 0)
                {
                    numberOfThreads = ClGlobul.NoOfcommentThread;
                }

                //check No of Id's and MSG's will be same or not  ....
                List<string> tempMSGList = new List<string>();
                if (ClGlobul.CommentIdsForMSG.Count > ClGlobul.commentMsgList.Count)
                {
                    //if (MessageBox.Show("You have uploaded less messages,Do you want to repeat messages? ", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    //{
                        try
                        {
                            AddToCommentLogger("[ " + DateTime.Now + " ] => [ we are storing messages in list. Please wait for next process. ]");

                            int IdCount = ClGlobul.CommentIdsForMSG.Count;
                            int MSGCount = ClGlobul.commentMsgList.Count;
                            int remainingMSG = IdCount - MSGCount;
                            int counter = 0;

                            tempMSGList.AddRange(ClGlobul.commentMsgList);
                            while (true)
                            {
                                try
                                {
                                    if (tempMSGList.Count > counter)
                                    {
                                        tempMSGList.Add(ClGlobul.commentMsgList[counter]);
                                    }
                                    counter++;
                                    remainingMSG = remainingMSG - 1;
                                }
                                catch
                                {
                                    counter = 0;
                                }
                                if (remainingMSG <= 0)
                                {
                                    ClGlobul.commentMsgList.AddRange(tempMSGList);
                                    break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                            GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => logInAccountsForcomment :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                            GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                        }
                   // }
                    AddToCommentLogger("[ " + DateTime.Now + " ] => [ Next Process is continue. ]");
                }

                List<List<string>> list_listAccounts = new List<List<string>>();

                counter_comment = DemoStagramPro.ClGlobul.accountList.Count();
                list_listAccounts = ListUtilities.Split(DemoStagramPro.ClGlobul.accountList, numberOfThreads);

                foreach (List<string> listAccounts in list_listAccounts)
                {
                    foreach (string account in listAccounts)
                    {
                        lock (lockr_ThreadControllerForComment)
                        {
                            try
                            {
                                if (count_ThreadControllerForComment >= listAccounts.Count)
                                {
                                    Monitor.Wait(lockr_ThreadControllerForComment);
                                }

                                string proxyValue = string.Empty;
                                string[] accountAndPass = account.Split(':');
                                string AccountName = string.Empty;
                                string AccountPass = string.Empty;
                                if (accountAndPass.Length == 2)
                                {
                                    AccountName = accountAndPass[0].Replace("\0", string.Empty).Trim();
                                    AccountPass = accountAndPass[1].Replace("\0", string.Empty).Trim();
                                }
                                else if (accountAndPass.Length == 4 || accountAndPass.Length == 6)
                                {
                                    AccountName = accountAndPass[0].Replace("\0", string.Empty).Trim();
                                    AccountPass = accountAndPass[1].Replace("\0", string.Empty).Trim();
                                    if (accountAndPass.Length == 4)
                                    {
                                        proxyValue = accountAndPass[2] + ":" + accountAndPass[3];
                                    }
                                    else if (accountAndPass.Length == 6)
                                    {
                                        proxyValue = accountAndPass[2] + ":" + accountAndPass[3] + ":" + accountAndPass[4] + ":" + accountAndPass[5];
                                    }
                                }
                                else
                                {
                                    AddToCommentLogger("[ " + DateTime.Now + " ] => [ Account Not In Correct Format ]");
                                }
                                if (ClGlobul.finalProxyList.Count > 0)
                                {
                                    lock (StagramProxyChecker.classes.ClProxyChecker.locker_finalProxyList)
                                    {
                                        if (ClGlobul.finalProxyList.Count == 0)
                                        {
                                            Monitor.Wait(StagramProxyChecker.classes.ClProxyChecker.locker_finalProxyList);
                                        }
                                    }
                                    if (ClGlobul.ProxyQueue.Count == 0)
                                    {
                                        ClGlobul.AddProxyInQueur();
                                    }
                                    try
                                    {
                                        string[] tempQueue = ClGlobul.ProxyQueue.Dequeue();
                                        if (tempQueue.Length > 0)
                                        {
                                            proxyValue = tempQueue[0];
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                                        GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => logInAccountsForcomment :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                                        GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                                    }
                                }

                                //string[] tempQueue = ClGlobul.ProxyQueue.Dequeue();
                                //string[] accountAndPass = account.Split(':');
                                //string AccountName = accountAndPass[0].Replace("\0", string.Empty).Trim();
                                //string AccountPass = accountAndPass[1].Replace("\0", string.Empty).Trim();

                                string acc = account.Remove(account.IndexOf(':'));

                                Thread CommentThread = new Thread(getloginForComment);
                                CommentThread.Name = "workerThread_Comment_" + acc;
                                CommentThread.IsBackground = true;

                                CommentThread.Start(new object[] { (AccountName), (AccountPass), proxyValue });

                                count_ThreadControllerForComment++;
                            }
                            catch (Exception ex)
                            {
                                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => logInAccountsForcomment :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                            }
                        }
                        if (ClGlobul.ProxyQueue.Count == 0)
                        {
                            ClGlobul.AddProxyInQueur();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //AddToCommentLogger("[ " + DateTime.Now + " ] => [ Methode Name => LogInAccountsForComment :=> " + ex.Message + " ]");
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => LogInAccountsForComment :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
            }

        }

        public void getloginForComment(object Accounts)
        {
            string proxyadd = string.Empty;
            string proxyport = string.Empty;
            string proxyUser = string.Empty;
            string proxyPass = string.Empty;
            string account = string.Empty;
            string pass = string.Empty;

            InstagramManager.Classes.InstagramAccountManager InstagramAccountManager = new InstagramManager.Classes.InstagramAccountManager(account, pass, proxyadd, proxyport, proxyUser, proxyPass);


            try
            {
                if (!CommentIstrueOrFals)
                {
                    try
                    {
                        Lst_Commentthread.Add(Thread.CurrentThread);
                        Lst_Commentthread = Lst_Commentthread.Distinct().ToList();
                        Thread.CurrentThread.IsBackground = true;
                    }
                    catch (Exception ex)
                    {
                        GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                        GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => getloginForComment :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                        GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                    }

                    Array accountinfo = (Array)Accounts;

                    account = (string)accountinfo.GetValue(0);
                    pass = (string)accountinfo.GetValue(1);
                    string ProxyValue = (string)accountinfo.GetValue(2);
                    string[] proxyarray = null;
                    if (!string.IsNullOrEmpty(ProxyValue))
                    {
                        proxyarray = ProxyValue.Split(':');
                        if (proxyarray.Count() > 2)
                        {
                            proxyadd = proxyarray[0];
                            proxyport = proxyarray[1];
                            proxyUser = proxyarray[2];
                            proxyPass = proxyarray[3];
                        }
                        else
                        {
                            proxyadd = proxyarray[0];
                            proxyport = proxyarray[1];
                        }
                    }
                    try
                    {
                        //string proxyadd = string.Empty;
                        //string proxyport = string.Empty;
                        //string proxyUser = string.Empty;
                        //string proxyPass = string.Empty;
                        InstagramAccountManager = new InstagramManager.Classes.InstagramAccountManager(account, pass, proxyadd, proxyport, proxyUser, proxyPass);
                        #region My Code
                        GlobDramProHttpHelper _GlobusHttpHelper = new GlobDramProHttpHelper();
                        #endregion


                        InstagramAccountManager.logEvents.addToLogger += new EventHandler(logEvents_addToLogger);

                        ///Get login from account....
                        //string statuse = InstagramAccountManager.Login();
                        //string statuse = InstagramAccountManager.MyLogin(ref _GlobusHttpHelper);
                        string statuse = InstagramAccountManager.MyLoginandComment(ref InstagramAccountManager.httpHelper, "", "", "");
                        if (InstagramAccountManager.LoggedIn == true)
                        {
                            //get comment from Login account ....
                            AddToCommentLogger("[ " + DateTime.Now + " ] => [ Logged in With " + account + " ]");
                            AddToCommentLogger("[ " + DateTime.Now + " ] => [ Starting Comment with " + account + " ]");

                            foreach (var CommentIdsForMSG_item in ClGlobul.CommentIdsForMSG)
                            {
                                getComment(ref InstagramAccountManager, CommentIdsForMSG_item);
                            }
                        }

                        else if (statuse.Contains("Failed"))
                        {
                            AddToCommentLogger("[ " + DateTime.Now + " ] => [ Login Failed For  " + account + " ]");
                        }
                        else if (statuse.Contains("AccessIssue"))
                        {
                            AddToCommentLogger("[ " + DateTime.Now + " ] => [ Login Failed For  " + account + " ]");
                        }
                        else if (statuse.Contains("Stream was not readable."))
                        {
                            //proxy is currect but have some login issues 
                            AddToCommentLogger("[ " + DateTime.Now + " ] => [ Stream was not readable. ]");
                            try
                            {
                                string removeingProxy = ClGlobul.finalProxyList.Where(e => e.Contains(proxyarray[0])).ToArray()[0];
                                ClGlobul.finalProxyList.Remove(removeingProxy);
                            }
                            catch (Exception ex)
                            {
                                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => getloginForComment :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                            };
                        }
                        else if (statuse.Contains("The request was aborted: The operation has timed out."))
                        {
                            //proxy is currect but request time out 
                            //AddToCommentLogger("The request was aborted: The operation has timed out.");
                            AddToCommentLogger("[ " + DateTime.Now + " ] => [ Login Failed For  " + account + " ]");
                        }
                        else if (statuse.Contains("503"))
                        {
                            //some request problem 
                            AddToCommentLogger("[ " + DateTime.Now + " ] => [ Login Failed For  " + account + " ]");
                            //AddToCommentLogger("503 some request problem /Server unavailable.");
                        }
                        else if (statuse.Contains("403"))
                        {
                            AddToCommentLogger("[ " + DateTime.Now + " ] => [ Login Failed For  " + account + " ]");
                            //AddToCommentLogger("403");
                        }
                        else
                        {
                            AddToCommentLogger("[ " + DateTime.Now + " ] => [ Login Failed For  " + account + " ]");
                        }
                    }
                    catch (Exception ex)
                    {
                        //AddTophotoLogger("[ " + DateTime.Now + " ] => [ " + account + " is not login  Error : GetLoginForComment (1) => MSG :" + ex.Message + " ]");
                        GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                        GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => GetLoginForComment (1):=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                        GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                    }
                    finally
                    {
                        InstagramAccountManager.logEvents.addToLogger -= new EventHandler(logEvents_addToLogger);
                    }
                }
            }
            catch (Exception ex)
            {
                //AddTophotoLogger(DateTime.Now + ":=> Methode Name => GetLoginForComment (2) :=> " + ex.Message);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => GetLoginForComment (2) :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
            }
            finally
            {
                count_ThreadControllerForComment--;
                lock (lockr_ThreadControllerForComment)
                {
                    if (!CommentIstrueOrFals)
                    {
                        Monitor.Pulse(lockr_ThreadControllerForComment);
                    }
                }

                counter_comment--;
                if (counter_comment == 0)
                {
                    AddToCommentLogger("----------------------------------------------------------------------------------------------------------------------------------");
                    AddToCommentLogger("[ " + DateTime.Now + " ] => [ PROCESS COMPLETED ]");
                    AddToCommentLogger("----------------------------------------------------------------------------------------------------------------------------------");
                }

            }
        }

        void logEvents_addToLogger(object sender, EventArgs e)
        {
            if (e is EventsArgs)
            {
                EventsArgs eArgs = e as EventsArgs;
                AddToCommentLogger(eArgs.log);
            }
        }

        public void getComment(ref InstagramManager.Classes.InstagramAccountManager accountManager, string CommentIdsForMSG_item)
        {
            Queue<string> CommentIdQueue = new Queue<string>();
            Queue<string> MsgQueue = new Queue<string>();

            InstagramManager.Classes.Comments ObjComments = new Comments();
            InstagramManager.Classes.InstagramPhotoLike ObjPotoLike = new InstagramPhotoLike();
            try
            {
                //Fill MSg's In Queue List ...
                //foreach (string commentMsgList_item in ClGlobul.commentMsgList)
                //{
                //    MsgQueue.Enqueue(commentMsgList_item);
                //}

                string photoLikeresult = string.Empty;

                //If the user Choose somment all given Id from every login Accounts ...
                //Then will be choose check bOx ...
                //if (chk_MsgAllCommentId.Checked == true)
                //{
                //foreach (string CommentIdsForMSG_item in ClGlobul.CommentIdsForMSG)
                //{
                photoLikeresult = string.Empty;
                //string message = MsgQueue.Dequeue();
                string message = ClGlobul.commentMsgList[RandomNumberGenerator.GenerateRandom(0, ClGlobul.commentMsgList.Count)];
                try
                {
                    string status = ObjComments.Comment(CommentIdsForMSG_item, message, ref accountManager);
                    if (status == "success")
                    {
                        //AddToCommentLogger("[ " + DateTime.Now + " ] => [ comment is successfully posted from " + accountManager.Username + " ]");
                        AddToCommentLogger("[ " + DateTime.Now + " ] => [ " + accountManager.Username + "  comment is successfully posted from " + CommentIdsForMSG_item + " ]");
                    }
                    else
                    {
                        //AddToCommentLogger("[ " + DateTime.Now + " ] => [ comment is successfully posted from " + accountManager.Username + " ]");
                        //AddToCommentLogger("[ " + DateTime.Now + " ] => [ Failed for  posted from " + accountManager.Username + " ]");
                        AddToCommentLogger("[ " + DateTime.Now + " ] => [ " + accountManager.Username + "  comment is successfully posted from This Id " + CommentIdsForMSG_item + " ]");
                    }

                    if (chk_CommentWithLike.Checked == true)
                    {
                        try
                        {
                            //Get Photo like if user is required ....
                            photoLikeresult = ObjPotoLike.photolike(CommentIdsForMSG_item, ref accountManager);
                        }
                        catch (Exception ex)
                        {
                            //AddToCommentLogger(DateTime.Now + ":=> Methode Name :- GetComment ()- Photo like :-  " + ex.Message);
                            GramBoardProFileHelper.AppendStringToTextfileNewLine((DateTime.Now + ":=> Methode Name :- GetComment ()- Photo like :- " + ex.Message), GramBoardProFileHelper.ErrorLogFilePathForComment);
                        }

                        //Print statuse in logger
                        printStatus(accountManager.Username, status, CommentIdsForMSG_item, photoLikeresult);
                    }
                    else
                    {
                        //Print statuse in logger
                        printStatus(accountManager.Username, status, CommentIdsForMSG_item);
                    }

                    if (!string.IsNullOrEmpty(txtmindelayComment.Text) && NumberHelper.ValidateNumber(txtmindelayComment.Text))
                    {
                        mindelay = Convert.ToInt32(txtmindelayComment.Text);
                    }
                    if (!string.IsNullOrEmpty(txtmaxdelayComments.Text) && NumberHelper.ValidateNumber(txtmaxdelayComments.Text))
                    {
                        maxdelay = Convert.ToInt32(txtmaxdelayComments.Text);
                    }

                    int delay = RandomNumberGenerator.GenerateRandom(mindelay, maxdelay);
                    AddToCommentLogger("[ " + DateTime.Now + " ] => [ Delay For " + delay + " Seconds For " + accountManager.Username + " ]");
                    Thread.Sleep(delay * 1000);
                }
                catch (Exception ex)
                {
                    //AddToCommentLogger(DateTime.Now + ":=> Methode Name => GetComment () :=> " + ex.Message);
                    GramBoardProFileHelper.AppendStringToTextfileNewLine("---------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                    GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => GetPhotolike ()  :=> comment Error :-  " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                    GramBoardProFileHelper.AppendStringToTextfileNewLine("---------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                }

                //if (MsgQueue.Count == 0)
                //{
                //    break;
                //}
                //}

                //}


                #region From Queue
                //else
                //{
                //    /// When user is choosend single comment in single Photo Id's 
                //    /// then program is Full fill this condion 
                //    /// and which is Commented in one Id and one MSg 
                //    /// also try to avoid duplicacy for it.
                //    if (CommentIdQueue.Count==0)
                //    {
                //        foreach (string CommentIdsForMSG_item in ClGlobul.CommentIdsForMSG)
                //        {
                //            CommentIdQueue.Enqueue(CommentIdsForMSG_item);
                //        } 
                //    }


                //    while (true)
                //    {
                //        Thread.Sleep(1000);
                //        photoLikeresult = string.Empty;
                //        string CommentIdForMSG = CommentIdQueue.Dequeue();
                //        string message = "";                      
                //        message = MsgQueue.Dequeue();
                //        try
                //        {
                //            string status = ObjComments.Comment(CommentIdForMSG, message, ref accountManager);

                //            if (chk_CommentWithLike.Checked == true)
                //            {
                //                try
                //                {
                //                    ///..get photo like from given Id 
                //                    photoLikeresult = ObjPotoLike.photolike(CommentIdForMSG, ref accountManager);
                //                }
                //                catch (Exception)
                //                {
                //                    photoLikeresult = "All ready Like";
                //                }
                //                //Print statuse in logger
                //                printStatus(accountManager.Username, status, CommentIdForMSG, photoLikeresult);
                //            }
                //            else
                //            {
                //                //Print statuse in logger
                //                printStatus(accountManager.Username, status, CommentIdForMSG);
                //            }
                //        }
                //        catch (Exception ex)
                //        {
                //            AddTophotoLogger(" Methode Name => GetLoginForComment (2) :=> " + ex.Message);
                //            AddTophotoLogger(accountManager.Username+"is not Comment to "+CommentIdForMSG);
                //            GlobusFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => GetLoginForComment (2) :=> " + ex.Message, GlobusFileHelper.ErrorLogFilePathForComment);
                //            GlobusFileHelper.AppendStringToTextfileNewLine(accountManager.ClientId + ":" + accountManager.Password + ":" + CommentIdForMSG, GlobusFileHelper.NotCommentedFilePath);
                //        }


                //        //if MESSAGE and Photo ID's are finished
                //        // Then break the while loop ...
                //        if (CommentIdQueue.Count == 0 || MsgQueue.Count==0)
                //        {
                //            if (CommentIdQueue.Count == 0 && MsgQueue.Count == 0)
                //            {
                //            }
                //            else if (CommentIdQueue.Count == 0)
                //                AddToCommentLogger("Photo id's are Finished ");
                //            else if (MsgQueue.Count == 0)
                //                AddToCommentLogger("Messages are Finished ");

                //            break;
                //        }

                //    }
                //}
                #endregion
            }
            catch (Exception ex)
            {
                //AddToCommentLogger(DateTime.Now + ":=> Methode Name => GetComment () :=> " + ex.Message);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => GetPhotolike (2)  :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
            }
            finally
            {
                AddToCommentLogger("[ " + DateTime.Now + " ] => [ Comment is Finished From Account : " + accountManager.Username + " ]");
            }
        }

        public void printStatus(string Username, string status, string CommentedId)
        {
            //if (string.IsNullOrEmpty(status))
            //{
            //    try
            //    {

            //    }
            //    catch (Exception)
            //    {
            //    }

            //    AddToCommentLogger("Comment option is not Avalible on " + CommentedId );
            //}
            //else 
            //if (status.Contains("message\":\"Success.") && !string.IsNullOrEmpty(status))
            //if (status.Contains("status\":\"OK") && !string.IsNullOrEmpty(status))
            if (status.Contains("Success") && !string.IsNullOrEmpty(status))
            {
                try
                {
                    ClGlobul.CommentCompletedList.ForEach(i =>
                                {
                                    if (!i.Contains(Username) && !i.Contains(CommentedId))
                                    {
                                        ClGlobul.CommentCompletedList.Add(Username + " : " + CommentedId);
                                    }
                                });
                }
                catch (Exception ex)
                {
                    GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => GetPhotolike (2)  :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                }
                GramBoardProFileHelper.AppendStringToTextfileNewLine(Username + " : " + CommentedId, GramBoardProFileHelper.SuccesfullyCommentedFilePath);
                AddToCommentLogger("[ " + DateTime.Now + " ] => [ " + Username + " is commented on " + CommentedId + " ]");
            }
            else
            {
                try
                {
                    ClGlobul.NotCommentList.ForEach(i =>
                    {
                        if (!i.Contains(Username) && !i.Contains(CommentedId))
                        {
                            ClGlobul.NotCommentList.Add(Username + " : " + CommentedId);
                        }
                    });

                    //Write in file if Comment is fail...
                    GramBoardProFileHelper.AppendStringToTextfileNewLine(Username + " : " + CommentedId, GramBoardProFileHelper.NotCommentedFilePath);
                }
                catch (Exception ex)
                {
                    GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => GetPhotolike (2)  :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                }

                AddToCommentLogger("[ " + DateTime.Now + " ] => [ " + Username + " Not commented " + CommentedId + " ]");
            }
        }

        public void printStatus(string Username, string status, string CommentedId, string photolikeresult)
        {
            if (status.Contains("message\":\"Success.\"") && photolikeresult.Contains("message\":\"LIKED\"}"))
            {
                // Add User name In List if Comment with photo likes is SuccessFully completed ... 
                try
                {
                    ClGlobul.photoLikesCompletedList.ForEach(i =>
                    {
                        if (!i.Contains(Username) && !i.Contains(CommentedId))
                        {
                            ClGlobul.photoLikesCompletedList.Add(Username);
                        }
                    });
                }
                catch (Exception ex)
                {
                    GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => printStatus (0)  :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                }

                // Add user name if comment is successflly Completed ...
                try
                {
                    ClGlobul.CommentCompletedList.ForEach(c =>
                    {
                        if (!c.Contains(Username) && !c.Contains(CommentedId))
                        {
                            ClGlobul.CommentCompletedList.Add(Username + " : " + CommentedId);
                        }
                    });
                }
                catch (Exception ex)
                {
                    GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => printStatus (1)   :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                }
                GramBoardProFileHelper.AppendStringToTextfileNewLine(Username + " : " + CommentedId, GramBoardProFileHelper.SuccesfullyCommentedFilePath);
                AddToCommentLogger("[ " + DateTime.Now + " ] => [ " + Username + " is commented & Liked " + CommentedId + " ]");
            }
            else if (status.Contains("message\":\"Success.\"}") && photolikeresult.Contains("All ready Like"))
            {
                try
                {
                    ClGlobul.photoLikesCompletedList.ForEach(i =>
                    {
                        if (!i.Contains(Username) && !i.Contains(CommentedId))
                        {
                            ClGlobul.photoLikesCompletedList.Add(Username);
                        }
                    });
                }
                catch (Exception ex)
                {
                    GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => printStatus (2)  :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                }

                // Add user name if comment is successflly Completed ...
                try
                {
                    ClGlobul.CommentCompletedList.ForEach(c =>
                    {
                        if (!c.Contains(Username) && !c.Contains(CommentedId))
                        {
                            ClGlobul.CommentCompletedList.Add(Username + " : " + CommentedId);
                        }
                    });
                }
                catch (Exception ex)
                {
                    GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => printStatus (2)  :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                }
                GramBoardProFileHelper.AppendStringToTextfileNewLine(Username + " : " + CommentedId, GramBoardProFileHelper.SuccesfullyCommentedFilePath);
                AddToCommentLogger("[ " + DateTime.Now + " ] => [ " + Username + " is commented & All ready like to " + CommentedId + " ]");
            }
            else if (status.Contains("message\":\"Success.\"}"))
            {
                if (status.Contains("message\":\"Success.\"}"))
                {
                    try
                    {
                        ClGlobul.CommentCompletedList.ForEach(c =>
                        {
                            if (!c.Contains(Username) && !c.Contains(CommentedId))
                            {
                                ClGlobul.CommentCompletedList.Add(Username + " : " + CommentedId);
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                        GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => printStatus (1):=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                        GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                    }
                }
                GramBoardProFileHelper.AppendStringToTextfileNewLine(Username + " : " + CommentedId, GramBoardProFileHelper.SuccesfullyCommentedFilePath);
                AddToCommentLogger("[ " + DateTime.Now + " ] => [ " + Username + "  is commented & Not LIKED  " + CommentedId + " ]");
            }
            else if (photolikeresult.Contains("LIKED"))
            {

                try
                {
                    ClGlobul.CommentCompletedList.ForEach(c =>
                    {
                        if (!c.Contains(Username) && !c.Contains(CommentedId))
                        {
                            ClGlobul.CommentCompletedList.Add(Username + " : " + CommentedId);
                        }
                    });
                }
                catch (Exception ex)
                {
                    GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                    GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => printStatus (1):=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                    GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                }

                GramBoardProFileHelper.AppendStringToTextfileNewLine(Username + " : " + CommentedId, GramBoardProFileHelper.SuccesfullyCommentedFilePath);
                AddToCommentLogger("[ " + DateTime.Now + " ] => [ " + Username + "  is LIKED from id " + CommentedId + " ]");
            }
            else
            {
                //Write in file if Comment is fail...
                GramBoardProFileHelper.AppendStringToTextfileNewLine(Username + " : " + CommentedId, GramBoardProFileHelper.NotCommentedFilePath);

                //Print in logger..
                AddToCommentLogger("[ " + DateTime.Now + " ] => [ " + Username + " Not commented  & not Like " + CommentedId + " ]");
            }
        }

        private void AddToCommentLogger(string log)
        {
            try
            {
                if (lst_commentLogger.InvokeRequired)
                {
                    lst_commentLogger.Invoke(new MethodInvoker(delegate
                       {
                           lst_commentLogger.Items.Add(log);
                           lst_commentLogger.SelectedIndex = lst_commentLogger.Items.Count - 1;
                       }));
                }
                else
                {
                    lst_commentLogger.Items.Add(log);
                    lst_commentLogger.SelectedIndex = lst_commentLogger.Items.Count - 1;
                }
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => AddToCommentLogger (1):=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
            }
        }

        public void AddToHashTagLogger(string log)
        {
            try
            {
                if (lstHashLogger.InvokeRequired)
                {
                    lstHashLogger.Invoke(new MethodInvoker(delegate
                    {
                        lstHashLogger.Items.Add(log);
                        lstHashLogger.SelectedIndex = lstHashLogger.Items.Count - 1;
                    }));
                }
                else
                {
                    lstHashLogger.Items.Add(log);
                    lstHashLogger.SelectedIndex = lstHashLogger.Items.Count - 1;
                }
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => AddToHashTagLogger (1):=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
            }
        }

        private void AddToImageTagLogger(string log)
        {
            try
            {
                if (lstImageLogger.InvokeRequired)
                {
                    lstImageLogger.Invoke(new MethodInvoker(delegate
                    {
                        lstImageLogger.Items.Add(log);
                        lstImageLogger.SelectedIndex = lstHashLogger.Items.Count - 1;
                    }));
                }
                else
                {
                    lstImageLogger.Items.Add(log);
                    lstImageLogger.SelectedIndex = lstHashLogger.Items.Count - 1;
                }
            }
            catch (Exception ex)
            {
                // AddToCommentLogger("Error : AddToImageTagLogger :-" + ex.Message);
            }
        }

        private void btn_stopComment_Click(object sender, EventArgs e)
        {
            if (Lst_Commentthread.Count > 0)
            {
                try
                {
                    new Thread(() =>
                           {
                               PhotoLikeIstrueOrFals = true;
                               List<Thread> tempLst = new List<Thread>();
                               tempLst.AddRange(Lst_Commentthread);
                               foreach (Thread tempLst_item in tempLst)
                               {
                                   try
                                   {
                                       tempLst_item.Abort();
                                       Lst_Commentthread.Remove(tempLst_item);
                                   }
                                   catch (Exception)
                                   {
                                   }
                               }
                               AddToCommentLogger("[ " + DateTime.Now + " ] => [ All Process are Aborted ]");
                           }).Start();
                }
                catch (Exception ex)
                {
                    AddToCommentLogger("Error : stopComment :-" + ex.Message);
                }
            }
            else
            {
                AddToCommentLogger("[ " + DateTime.Now + " ] => [ Please start Process. ]");
                MessageBox.Show("This feature is working after Process start.");
            }
        }

        private void btn_AccountClearForComment_Click(object sender, EventArgs e)
        {
            try
            {
                //Clear Account txt box & account list ..
                txtAddAccounts1.Text = string.Empty;
                ClGlobul.accountList.Clear();

                //Clear Comment Id & List...
                txt_UploadForCommentIds.Text = string.Empty;
                ClGlobul.CommentIdsForMSG.Clear();

                //Clear All MSg from List and test box..
                txt_UploadedFilepath.Text = string.Empty;
                ClGlobul.commentMsgList.Clear();

                //Clear No Of thread and & txt box..
                txt_NoOfCommentThread.Text = string.Empty;
                ClGlobul.NoOfcommentThread = 0;

                //clear Runing thread list ...
                Lst_Commentthread.Clear();

                //clear all Logger ...
                lst_commentLogger.Items.Clear();
                lbLogger.Items.Clear();
            }
            catch (Exception)
            {
                AddToCommentLogger("[ " + DateTime.Now + " ] => [ AccountClearForComment ]");
            }
        }

        private void btn_commentModule_Notcomment_Click(object sender, EventArgs e)
        {
            if (ClGlobul.photoLikesCompletedList.Count() != 0)
            {
                foreach (string CommentCompletedList_item in ClGlobul.CommentCompletedList)
                {
                    ClGlobul.accountList.Remove(CommentCompletedList_item);
                }

                foreach (string accountList_item in ClGlobul.accountList)
                {
                    GramBoardProFileHelper.AppendStringToTextfileNewLine(accountList_item, GramBoardProFileHelper.NotCommentedFilePath);
                }

                AddToCommentLogger("[ " + DateTime.Now + " ] => [ Stored all Remaining account. ]");
            }
            else
            {
                AddToCommentLogger("[ " + DateTime.Now + " ] => [ Please start Process. ]");
                MessageBox.Show("This feature is working after Process start.");
            }
        }

        private void btn_commentModule_NotPhotoLike_Click(object sender, EventArgs e)
        {
            if (ClGlobul.photoLikesCompletedList.Count() != 0)
            {
                foreach (string photlikeConpletedList_item in ClGlobul.photoLikesCompletedList)
                {
                    ClGlobul.accountList.Remove(photlikeConpletedList_item);
                }

                foreach (string accountList_item in ClGlobul.accountList)
                {
                    GramBoardProFileHelper.AppendStringToTextfileNewLine(accountList_item, GramBoardProFileHelper.NotlikePhotoAccountIdFilePath);
                }

                AddToCommentLogger("[ " + DateTime.Now + " ] => [ Stored all Remaining account. ]");
            }
            else
            {
                AddToCommentLogger("[ " + DateTime.Now + " ] => [ Please start Process. ]");
                MessageBox.Show("This feature is working after Process start.");
            }
        }

        #endregion

        //********************************************************************************************************************************************//

        #region Desktop Background

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g;

            g = e.Graphics;

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //// Draw the image.
            g.DrawImage(Bacgroundimage, 0, 0, this.Width, this.Height);

        }

        private void panel1_Follower_Paint(object sender, PaintEventArgs e)
        {
            Graphics g;

            g = e.Graphics;

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //// Draw the image.
            g.DrawImage(PanelImage, 0, 0, this.Width, this.Height);
        }

        private void groupBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g;

            g = e.Graphics;

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //// Draw the image.
            //g.DrawImage(Properties.Resources.tra1, 0, 0, this.Width, this.Height);
        }

        private void panel1_PhotoLike_Paint(object sender, PaintEventArgs e)
        {
            Graphics g;

            g = e.Graphics;

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //// Draw the image.
            g.DrawImage(PanelImage, 0, 0, this.Width, this.Height);


        }

        private void panel1_comment_Paint(object sender, PaintEventArgs e)
        {
            Graphics g;

            g = e.Graphics;

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //// Draw the image.
            g.DrawImage(PanelImage, 0, 0, this.Width, this.Height);
        }

        private void tabPage1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g;

            g = e.Graphics;

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //// Draw the image.
            g.DrawImage(Bacgroundimage, 0, 0, this.Width, this.Height);
        }

        private void tabPage2_Paint(object sender, PaintEventArgs e)
        {
            Graphics g;

            g = e.Graphics;

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //// Draw the image.
            g.DrawImage(Bacgroundimage, 0, 0, this.Width, this.Height);
        }

        private void tabPage3_Paint(object sender, PaintEventArgs e)
        {
            Graphics g;

            g = e.Graphics;

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //// Draw the image.
            g.DrawImage(Bacgroundimage, 0, 0, this.Width, this.Height);
        }


        private void tabPage4_Paint(object sender, PaintEventArgs e)
        {
            Graphics g;

            g = e.Graphics;

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //// Draw the image.
            g.DrawImage(PanelImage, 0, 0, this.Width, this.Height);
        }


        private void tabPage5_Paint(object sender, PaintEventArgs e)
        {

            Graphics g;

            g = e.Graphics;

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //// Draw the image.
            g.DrawImage(PanelImage, 0, 0, this.Width, this.Height);


        }
        #endregion

        //************************************************ Account checker **********************************************************************************//

        #region Account Checker
        public int Noofthreads = 100;
        private void btnAccountcheck_Click(object sender, EventArgs e)
        {
            if (ClGlobul.accountList.Count() == 0)
            {
                MessageBox.Show("Please upload the accounts !");
               // AddToLogger1("[ " + DateTime.Now + " ] => [ Please upload the accounts ! ]");
                return;
            }

            if (!string.IsNullOrEmpty(txt_noofThread.Text) && ValidateNumber(txt_noofThread.Text))
            {
                try
                {
                    Noofthreads = Convert.ToInt32(txt_noofThread.Text);
                }
                catch (Exception ex)
                {
                    GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => btnAccountcheck_Click :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                }
            }
            else
            {

                return;
            }
            new Thread(() =>
            {
                Accountchecker1();

            }).Start();
        }

        int acc_checker_counter1 = 0;
        public void Accountchecker1()
        {
            try
            {
                //AddToLogger1("[ " + DateTime.Now + " ] => [ Starting Process For Account Checker ]");

                if (ClGlobul.ProxyQueue.Count == 0)
                {
                    ClGlobul.AddProxyInQueur();
                }


                List<List<string>> list_Accounts = new List<List<string>>();
                acc_checker_counter1 = DemoStagramPro.ClGlobul.accountList.Count();
                list_Accounts = ListUtilities.Split(DemoStagramPro.ClGlobul.accountList, Maxthread);
                ThreadPool.SetMaxThreads(Noofthreads, 5);

                foreach (List<string> listAccounts in list_Accounts)
                {
                    foreach (string account in listAccounts)
                    {
                        ThreadPool.QueueUserWorkItem(new WaitCallback(StartAccountChecking1), new object[] { account });

                    }

                    if (ClGlobul.ProxyQueue.Count == 0)
                    {
                        ClGlobul.AddProxyInQueur();
                    }

                }
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => logInAccounts :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
            }
        }




        int acc_checker_counter = 0;
        public void Accountchecker()
        {
            try
            {
               // AddToLogger("[ " + DateTime.Now + " ] => [ Starting Process For Account Checker ]");

                if (ClGlobul.ProxyQueue.Count == 0)
                {
                    ClGlobul.AddProxyInQueur();
                }


                List<List<string>> list_Accounts = new List<List<string>>();
                acc_checker_counter = DemoStagramPro.ClGlobul.accountList.Count();
                list_Accounts = ListUtilities.Split(DemoStagramPro.ClGlobul.accountList, Maxthread);
                ThreadPool.SetMaxThreads(Noofthreads, 5);

                foreach (List<string> listAccounts in list_Accounts)
                {
                    foreach (string account in listAccounts)
                    {
                        ThreadPool.QueueUserWorkItem(new WaitCallback(StartAccountChecking), new object[] { account });

                    }

                    if (ClGlobul.ProxyQueue.Count == 0)
                    {
                        ClGlobul.AddProxyInQueur();
                    }

                }
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => logInAccounts :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
            }
        }

        public void StartAccountChecking(object Account)
        {
            try
            {
                Array accountinfo = (Array)Account;

                //string account = (string)accountinfo.GetValue(0);
                //string pass = (string)accountinfo.GetValue(1);
                string accountdata = (string)accountinfo.GetValue(0);
                string proxyValue = string.Empty;
                string[] accountAndPass = accountdata.Split(':');
                string AccountName = string.Empty;
                string AccountPass = string.Empty;
                if (accountAndPass.Length == 2)
                {
                    AccountName = accountAndPass[0].Replace("\0", string.Empty).Trim();
                    AccountPass = accountAndPass[1].Replace("\0", string.Empty).Trim();
                }
                else if (accountAndPass.Length == 4 || accountAndPass.Length == 6)
                {
                    AccountName = accountAndPass[0].Replace("\0", string.Empty).Trim();
                    AccountPass = accountAndPass[1].Replace("\0", string.Empty).Trim();
                    if (accountAndPass.Length == 4)
                    {
                        proxyValue = accountAndPass[2] + ":" + accountAndPass[3];
                    }
                    else if (accountAndPass.Length == 6)
                    {
                        proxyValue = accountAndPass[2] + ":" + accountAndPass[3] + ":" + accountAndPass[4] + ":" + accountAndPass[5];
                    }
                }
                else
                {
                    if (ClGlobul.finalProxyList.Count > 0)
                    {
                        lock (StagramProxyChecker.classes.ClProxyChecker.locker_finalProxyList)
                        {
                            if (ClGlobul.finalProxyList.Count == 0)
                            {
                               // AddToLogger("[ " + DateTime.Now + " ] => [ Waiting For Proxies To Be Loaded ]");
                                Monitor.Wait(StagramProxyChecker.classes.ClProxyChecker.locker_finalProxyList);
                            }
                        }
                        try
                        {
                            string[] tempQueue = ClGlobul.ProxyQueue.Dequeue();
                            if (tempQueue.Length > 0)
                            {
                                proxyValue = tempQueue[0];
                            }
                        }
                        catch (Exception ex)
                        {
                            GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                            GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => StartAccountChecking :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                            GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                        }
                    }
                }
                //If Account is containe colon its remove... 
                ThreadPool.SetMaxThreads(Noofthreads, 5);
                ThreadPool.QueueUserWorkItem(new WaitCallback(AccountChecker), new object[] { (AccountName), (AccountPass), proxyValue });

            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => StartAccountChecking :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
            }
        }


        public void StartAccountChecking1(object Account)
        {
            try
            {
                Array accountinfo = (Array)Account;

                string accountdata = (string)accountinfo.GetValue(0);
                string proxyValue = string.Empty;
                string[] accountAndPass = accountdata.Split(':');
                string AccountName = string.Empty;
                string AccountPass = string.Empty;
                if (accountAndPass.Length == 2)
                {
                    AccountName = accountAndPass[0].Replace("\0", string.Empty).Trim();
                    AccountPass = accountAndPass[1].Replace("\0", string.Empty).Trim();
                }
                else if (accountAndPass.Length == 4 || accountAndPass.Length == 6)
                {
                    AccountName = accountAndPass[0].Replace("\0", string.Empty).Trim();
                    AccountPass = accountAndPass[1].Replace("\0", string.Empty).Trim();
                    if (accountAndPass.Length == 4)
                    {
                        proxyValue = accountAndPass[2] + ":" + accountAndPass[3];
                    }
                    else if (accountAndPass.Length == 6)
                    {
                        proxyValue = accountAndPass[2] + ":" + accountAndPass[3] + ":" + accountAndPass[4] + ":" + accountAndPass[5];
                    }
                }
                else
                {
                    if (ClGlobul.finalProxyList.Count > 0)
                    {
                        lock (StagramProxyChecker.classes.ClProxyChecker.locker_finalProxyList)
                        {
                            if (ClGlobul.finalProxyList.Count == 0)
                            {
                                AddToLogger1("[ " + DateTime.Now + " ] => [ Waiting For Proxies To Be Loaded ]");
                                Monitor.Wait(StagramProxyChecker.classes.ClProxyChecker.locker_finalProxyList);
                            }
                        }
                        try
                        {
                            string[] tempQueue = ClGlobul.ProxyQueue.Dequeue();
                            if (tempQueue.Length > 0)
                            {
                                proxyValue = tempQueue[0];
                            }
                        }
                        catch (Exception ex)
                        {
                            GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                            GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => StartAccountChecking :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                            GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                        }
                    }
                }

                ThreadPool.SetMaxThreads(Noofthreads, 5);
                ThreadPool.QueueUserWorkItem(new WaitCallback(AccountChecker1), new object[] { (AccountName), (AccountPass), proxyValue });

            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => StartAccountChecking :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
            }
        }

        #endregion

        private void frm_stagram_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                try
                {
                    StopAllProcess();
                    foreach (var item in System.Diagnostics.Process.GetProcesses())
                    {
                        try
                        {
                            if (item.ProcessName.Contains("ID_LicensingManager"))
                            {
                                item.Kill();
                            }
                        }
                        catch (Exception ex)
                        {
                            GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                            GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => frm_stagram_FormClosed :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                            GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                        }
                    }
                }
                catch (Exception ex)
                {
                    GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                    GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => frm_stagram_FormClosed :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                    GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                }
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => frm_stagram_FormClosed :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
            }
        }

        public void StopAllProcess()
        {
            try
            {
                foreach (Thread item in lst_Thread)
                {
                    try
                    {
                        Thread thread = item;
                        if (thread != null)
                        {
                            if (thread.ThreadState == ThreadState.Running || thread.ThreadState == ThreadState.WaitSleepJoin || thread.ThreadState == ThreadState.Background) { thread.Abort(); }
                            thread.Abort();
                        }
                    }
                    catch (Exception ex)
                    {
                        GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                        GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => StopAllProcess :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                        GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                    }
                }
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => StopAllProcess :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
            }
        }

        private void txtmindelay_TextChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(txtmindelay.Text, out mindelay))
            {
                MessageBox.Show("Please Enter Only No");
            }
        }

        private void txtmaxdelay_TextChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(txtmaxdelay.Text, out maxdelay))
            {
                MessageBox.Show("Please Enter Only No");
            }
        }

        private void frm_stagram_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                DialogResult _DialogResult = MessageBox.Show("Do You Really Want To Exit?", "Gram BoardPro", MessageBoxButtons.YesNo);
                if (_DialogResult.ToString().Equals("Yes"))
                {
                    var prc = System.Diagnostics.Process.GetProcesses();
                    foreach (var item in prc)
                    {
                        try
                        {
                            if (item.ProcessName.Contains("ID_LicensingManager"))
                            {
                                item.Kill();
                            }
                        }
                        catch (Exception ex)
                        {
                            GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                            GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => frm_stagram_FormClosing :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                            GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                        }
                    }
                    Application.ExitThread();
                    Application.Exit();
                }
                else
                {
                    e.Cancel = true;
                    this.Activate();
                }
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => frm_stagram_FormClosing :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
            }
        }

        private void btnClearUploadAccount_Click(object sender, EventArgs e)
        {
            try
            {
                QueryExecuter.deleteQuery();
                ClGlobul.accountList.Clear();
                txtAddAccounts1.Text = string.Empty;
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => btnClearUploadAccount_Click :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
            }

            AddToLogger("[ " + DateTime.Now + " ] => [  All Accounts Cleared ]");
        }

        private void btnClearLike_Click(object sender, EventArgs e)
        {
            try
            {
                txt_photoListPath.Text = string.Empty;
                txt_noOfphotoLikethread.Text = string.Empty;
                txtphotosingaluser.Text = string.Empty;
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => btnClearLike_Click :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
            }
        }

        private void txt_FollowUser_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (!string.IsNullOrEmpty(txt_filepathofFollowing.Text.ToString()))
            {
                AddToLogger("[ " + DateTime.Now + " ] => [ File Of Following already added ]");
                txt_FollowUser.Text = string.Empty;
                try
                {
                    Thread th = new Thread(clearText);
                    th.Start();

                }
                catch (Exception ex)
                {
                    GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                    GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => btnClearLike_Click :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                    GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                }
                return;
            }
        }

        //public delegate void EventHandler();
        //public static event EventHandler _eventClear;

        public void clearText()
        {
            try
            {
                Thread.Sleep(10);
                txt_FollowUser.Invoke(new MethodInvoker(delegate() { txt_FollowUser.Text = string.Empty; }));
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => clearText :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
            }
        }

        private void btnClearComment_Click(object sender, EventArgs e)
        {
            txt_UploadForCommentIds.Text = string.Empty;
            txt_UploadedFilepath.Text = string.Empty;
            if (chk_CommentWithLike.Checked)
            {
                chk_CommentWithLike.Checked = false;
            }
        }

        private void btnHashTagUpload_Click(object sender, EventArgs e)
        {
            try
            {
                //get the hash tags from selected file...
                using (OpenFileDialog ofdhashTag = new OpenFileDialog())
                {
                    ofdhashTag.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                    ofdhashTag.Filter = "Text Files (*.txt)|*.txt";
                    if (ofdhashTag.ShowDialog() == DialogResult.OK)
                    {
                        //Add File path in Text Box....
                        txtHashTagUpload.Text = ofdhashTag.FileName;
                        //call Read Method file when we select a file
                        readHashTagFile(ofdhashTag.FileName);

                    }
                }
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("***********************************************************************************************", GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => btnHashTagUpload :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("***********************************************************************************************", GramBoardProFileHelper.ErrorLogFilePathForComment);
            }
        }


        public void readHashTagFile(string HashTagFilePath)
        {
            try
            {
                ClGlobul.HashTagForScrap.Clear();
                //Read Data From Selected File ....
                List<string> hashTaglist = GramBoardProFileHelper.ReadFile((string)HashTagFilePath);
                foreach (string itemHash in hashTaglist)
                {
                    char checkstring = itemHash.ElementAt(0);
                    if (itemHash.ElementAt(0) == '#')
                    {
                        string itemHashNew = itemHash.Replace("#", "");
                        //add Comment Id's In Globol Comment Id List ...
                        ClGlobul.HashTagForScrap.Add(itemHash);
                    }
                    else
                    {
                        AddToHashTagLogger("[ " + DateTime.Now + " ] => [ " + itemHash + " is not upLoaded.Because Its not Starting with # ]");
                    }

                }
                ClGlobul.HashTagForScrap = ClGlobul.HashTagForScrap.Distinct().ToList();
                //Show No Of Data Count In logger...
                AddToHashTagLogger("[ " + DateTime.Now + " ] => [ " + ClGlobul.HashTagForScrap.Count + " Hash Tags Uploaded. ]");
            }
            catch (Exception ex)
            {
                //when its generate any Error From here which is write in Text file ...
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => readHashTagFile :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
            }
        }




        private void btnHashTagClear_Click(object sender, EventArgs e)
        {
            try
            {
                ClGlobul.HashTagForScrap.Clear();
                txtHashTagUpload.Text = string.Empty;
                AddToHashTagLogger("[ " + DateTime.Now + " ] => [ All Hash Tags Cleared ]");


            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => btnHashTagClear_Click :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
            }
        }

        private void btnHashStart_Click(object sender, EventArgs e)
        {

            int StaTus = 1;
            bool NoOfCount = int.TryParse(txtUserScraperCounter.Text.Trim(), out StaTus);
            if (NoOfCount)
            {
                ClGlobul.countNOOfFollowersandImageDownload = StaTus;
            }
            else
            {
                GramBoardLogHelper.log.Info("[ " + DateTime.Now + " ] =>[Pleae enter valid count");
                return;
            }

            try
            {
                if (string.IsNullOrEmpty(txtHashTagUpload.Text) && string.IsNullOrEmpty(txtDelayHashTag.Text) && string.IsNullOrEmpty(txtuploadUserScrper.Text))
                {
                    MessageBox.Show("Please Upload a # Tag File and Delay");
                    return;
                }
            }
            catch { };



            int parsedValue;
            if (!int.TryParse(txtDelayHashTag.Text, out parsedValue))
            {

                //AddToCommentLogger("[ " + DateTime.Now + " ] => [ This is a number only field ]");
                GramBoardLogHelper.log.Info("[ " + DateTime.Now + " ] => [ This is a number only field ]");

                return;
            }

            if (!string.IsNullOrEmpty(txtuploadUserScrper.Text.Trim()))
            {
                //add following in followingList...
                lstStoreDownloadImageKeyword.Clear();

                string s = txtuploadUserScrper.Text.ToString();

                if (s.Contains(','))
                {
                    string[] Data = s.Split(',');

                    foreach (var item in Data)
                    {


                        ClGlobul.HashTagForScrap.Add(item);
                    }
                }
                else
                {
                    ClGlobul.HashTagForScrap.Add(txtuploadUserScrper.Text.ToString());
                }

            }

            bool ProcessStartORnot = false;

            if (ClGlobul.HashTagForScrap.Count > 0)
            {
                if (!string.IsNullOrEmpty(txtDelayHashTag.Text.ToString()))
                {
                    if (NumberHelper.ValidateNumber(txtDelayHashTag.Text.ToString()))
                    {
                       // AddToHashTagLogger("[ " + DateTime.Now + " ] => [ Scraping Started for User Ids ]");
                        GramBoardLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Scraping Started for User Ids ]");
                        try
                        {
                            Thread _ThreadHashStart = new Thread(() => new ScrapUserName().startHash(Convert.ToInt32(txtDelayHashTag.Text.ToString())));
                            _ThreadHashStart.Start();
                        }
                        catch (Exception ex)
                        {
                            GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                            GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => btnHashTagClear_Click :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                            GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                        }
                    }
                    else
                    {
                        AddToHashTagLogger("[ " + DateTime.Now + " ] => [ Delay time not in correct format ]");
                    }
                }
                else
                {
                    AddToHashTagLogger("[ " + DateTime.Now + " ] => [ Please enter preferred delay time ]");
                }
            }
            else
            {
                AddToHashTagLogger("[ " + DateTime.Now + " ] => [ No Tags Uploaded ]");
            }
        }

        private void btnHashStop_Click(object sender, EventArgs e)
        {
            new Thread(() => stopScrap()).Start();
        }

        private void stopScrap()
        {
            List<Thread> lstTemp = ScrapUserName.lstScrapThread.Distinct().ToList();
            foreach (Thread items in lstTemp)
            {
                try
                {
                    items.Abort();
                }
                catch (Exception ex)
                {
                    GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                    GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => stopScrap :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                    GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                }
            }
            Thread.Sleep(1500);
            AddToHashTagLogger("[ " + DateTime.Now + " ] => [ Scraping User Ids Aborted ]");
        }

        private void btnImageUpload_Click(object sender, EventArgs e)
        {
            try
            {

                //get the hash tags from selected file...
                using (OpenFileDialog ofdImageTag = new OpenFileDialog())
                {
                    ofdImageTag.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                    ofdImageTag.Filter = "Text Files (*.txt)|*.txt";
                    if (ofdImageTag.ShowDialog() == DialogResult.OK)
                    {
                        //Add File path in Text Box....
                        txtUploadImage.Text = ofdImageTag.FileName;
                        //call Read Method file when we select a file
                        readImageTagFile(ofdImageTag.FileName);

                    }
                }
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("***********************************************************************************************", GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => btnHashTagUpload :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("***********************************************************************************************", GramBoardProFileHelper.ErrorLogFilePathForComment);
            }
        }

        public void readImageTagFile(string HashTagFilePath)
        {
            try
            {
                ClGlobul.ImageTagForScrap.Clear();
                //Read Data From Selected File ....
                List<string> hashTaglist = GramBoardProFileHelper.ReadFile((string)HashTagFilePath);
                foreach (string itemHash in hashTaglist)
                {
                    if (itemHash.ElementAt(0) == '#')
                    {
                        string itemHashNew = itemHash.Replace("#", "");
                        //add Comment Id's In Globol Comment Id List ...
                        ClGlobul.ImageTagForScrap.Add(itemHashNew);
                    }
                    else
                    {
                        AddToImageTagLogger("[ " + DateTime.Now + " ] => [ " + itemHash + " is not Uploaded as Its not starting with # ]");

                    }
                }
                ClGlobul.ImageTagForScrap = ClGlobul.ImageTagForScrap.Distinct().ToList();
                //Show No Of Data Count In logger...
                AddToImageTagLogger("[ " + DateTime.Now + " ] => [ " + ClGlobul.ImageTagForScrap.Count + " Hash Tags Uploaded. ]");
            }
            catch (Exception ex)
            {
                //when its generate any Error From here which is write in Text file ...
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => ReadLargeCommentIdFile :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
            }
        }

        private void btnImageClear_Click(object sender, EventArgs e)
        {
            try
            {
                ClGlobul.ImageTagForScrap.Clear();
                txtUploadImage.Text = string.Empty;
                AddToImageTagLogger("[ " + DateTime.Now + " ] => [ All Hash Tags Cleared ]");
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => btnImageClear_Click :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
            }
        }

        private bool _boolCanStop = false;
        
        private void btnImageStart_Click(object sender, EventArgs e)
        {
            List<string> imageloardcount = new List<string>();
            int Status = 1;
            bool NoOfCount = int.TryParse(txtcountImageScraper.Text.Trim(), out Status);
            if (NoOfCount)
            {
                ClGlobul.countNOOfFollowersandImageDownload = Status;
            }
            else
            {
                AddTophotoLogger("[ " + DateTime.Now + " ] =>[Pleae enter valid count");
                return;
            }


            try
            {
                if (string.IsNullOrEmpty(txtUploadImage.Text) && string.IsNullOrEmpty(txtmaxDelayGetImageImage.Text) && string.IsNullOrEmpty(txtuserimagescraper.Text))
                {
                    MessageBox.Show("Please Upload a # Tag File and Delay");
                    return;
                }
            }
            catch { };


            int parsedValue;
            if (!int.TryParse(txtmaxDelayGetImageImage.Text, out parsedValue))
            {
                AddToImageTagLogger("[ " + DateTime.Now + " ] => [ This is a number only field  ]");

                return;
            }


            if (!string.IsNullOrEmpty(txtuserimagescraper.Text.Trim()))
            {
                //add following in followingList...
                lstStoreDownloadImageKeyword.Clear();

                string s = txtuserimagescraper.Text.ToString();

                if (s.Contains(','))
                {
                    string[] Data = s.Split(',');

                    foreach (var item in Data)
                    {
                        ClGlobul.ImageTagForScrap.Add(item);
                    }
                }
                else
                {
                    ClGlobul.ImageTagForScrap.Add(txtuserimagescraper.Text.ToString());
                }

            }

            bool ProcessStartORnot = false;

            if (ClGlobul.ImageTagForScrap.Count > 0)
            {
                if (!string.IsNullOrEmpty(txtmaxDelayGetImageImage.Text.ToString()))
                {
                    if (NumberHelper.ValidateNumber(txtmaxDelayGetImageImage.Text.ToString()))
                    {
                        AddToImageTagLogger("[ " + DateTime.Now + " ] => [ Scraping Started for Image Urls ]");
                        try
                        {
                            _boolCanStop = true;
                            Thread _ThreadImageStart = new Thread(() => new ScrapImages().DownloadingImage(Convert.ToInt32(txtmaxDelayGetImageImage.Text.ToString())));
                            _ThreadImageStart.Start();
                        }
                        catch (Exception ex)
                        {
                            GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                            GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => btnImageStart_Click :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                            GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                        }
                    }
                    else
                    {
                        AddToImageTagLogger("[ " + DateTime.Now + " ] => [ Delay time not in correct format ]");
                    }
                }
                else
                {
                    AddToImageTagLogger("[ " + DateTime.Now + " ] => [ Please enter preferred delay time ]");
                }
            }
            else
            {
                AddToImageTagLogger("[ " + DateTime.Now + " ] => [ No Tags Uploaded ]");
            }
        }

        private void btnImageStop_Click(object sender, EventArgs e)
        {
            if (_boolCanStop)
            {
                _boolCanStop = false;
                try
                {
                    new Thread(() => ImageScrapStop()).Start();
                }
                catch (Exception ex)
                {
                    GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                    GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => btnImageStop_Click :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                    GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                }
            }
        }

        private void ImageScrapStop()
        {
            try
            {
                ScrapImages.stopScrapImageBool = true;
                List<Thread> lstTemp = ScrapImages.lstScrapImageThread.Distinct().ToList();
                foreach (Thread items in lstTemp)
                {
                    try
                    {
                        items.Abort();
                    }
                    catch (Exception ex)
                    {
                        GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                        GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => ImageScrapStop :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                        GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                    }
                }
                Thread.Sleep(1500);
                AddToImageTagLogger("[ " + DateTime.Now + " ] => [ Scraping Image Urls Aborted ]");
                AddToImageTagLogger("[ " + DateTime.Now + " ] => [ Process Stopped ]");
            }
            catch { }
        }

        private static bool _boolStopUnfollow = false;
        private static int delayUnfollow = 0;
        private static int noOfAccountUnfollow = 0;
        private static int timeToUnfollow = 0;

        private void btnStartUnfollow_Click(object sender, EventArgs e)
        {
            int parsedValue;
            if (!int.TryParse(txt_noofThread.Text, out parsedValue))
            {


                AddToUnfollowLogger("[ " + DateTime.Now + " ] => [ This is a number only field]");
                return;
            }
            bool ProcessStartORnot = false;
            int num;
            int minDelay = 0;
            int maxDelay = 0;
            delayUnfollow = 0;
            noOfAccountUnfollow = 0;

            if (!(ClGlobul.accountList.Count > 0))
            {
                AddToUnfollowLogger("[ " + DateTime.Now + " ] => [ Please upload accounts first ]"); return;
            }
            #region Min Delay
            if (!string.IsNullOrEmpty(mintxtdelay.Text.ToString()))
            {
                if (!string.IsNullOrEmpty(mintxtdelay.Text) && NumberHelper.ValidateNumber(mintxtdelay.Text))
                {
                    mindelay = Convert.ToInt32(mintxtdelay.Text);
                }
                if (!string.IsNullOrEmpty(maxtxtdelay.Text) && NumberHelper.ValidateNumber(maxtxtdelay.Text))
                {
                    maxdelay = Convert.ToInt32(maxtxtdelay.Text);
                }

                lock (_lockObject)
                {
                    int delay = RandomNumberGenerator.GenerateRandom(mindelay, maxdelay);
                    AddToUnfollowLogger("[ " + DateTime.Now + " ] => [ Delay For " + delay + " Seconds ]");
                    Thread.Sleep(delay * 1000);
                }

                //if (Int32.TryParse(txtmindelay1.Text.ToString(), out num))
                //{
                //    minDelay = Convert.ToInt32(txtmindelay1.Text.ToString());
                //}

                //else
                //{
                //    AddToUnfollowLogger("[ " + DateTime.Now + " ] => [ Minimum Delay not in correct format ]");
                //    return;
                //}
            }
            else
            {
                AddToUnfollowLogger("[ " + DateTime.Now + " ] => [ Please Enter Minimum Delay ]"); return;
            }
            #endregion

            #region Max Delay
            if (!string.IsNullOrEmpty(txtmaxdelay1.Text.ToString()))
            {
                if (Int32.TryParse(txtmindelay1.Text.ToString(), out num))
                {
                    maxDelay = Convert.ToInt32(txtmaxdelay1.Text.ToString());
                }
                else
                {
                    AddToUnfollowLogger("[ " + DateTime.Now + " ] => [ Maximum Delay not in correct format ]"); return;
                }
            }
            else
            {
                AddToUnfollowLogger("[ " + DateTime.Now + " ] => [ Please Enter Maximum Delay ]"); return;
            }
            #endregion

            try
            {
                delayUnfollow = new Random().Next(minDelay, maxDelay);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                AddToUnfollowLogger("[ " + DateTime.Now + " ] => [ Maximum Delay must be greater than Minimum delay ]"); return;
            }
            catch { delayUnfollow = 2; }

            string noOfAccountsToUnfollow = string.Empty;
            noOfAccountsToUnfollow = Input.InputBox("mention number of Followers you want to unfollow who did't followed you back?", "Enter Number", "5");

            if (!string.IsNullOrEmpty(noOfAccountsToUnfollow))
            {
                int n;
                if (Int32.TryParse(noOfAccountsToUnfollow, out n))
                {
                    try
                    {
                        #region Get Time Elapsed
                        string timeelapsed = string.Empty;
                        timeelapsed = Input.InputBox("Enter days from where you want to unfollow?", "Enter Days", "7");
                        if (!string.IsNullOrEmpty(timeelapsed))
                        {
                            int x;
                            if (Int32.TryParse(timeelapsed, out x))
                            {
                                lst_Thread.Clear();
                                _boolStopUnfollow = false;
                                noOfAccountUnfollow = n;
                                timeToUnfollow = x;
                                AddToUnfollowLogger("[ " + DateTime.Now + " ] => [ Starting Unfollow Process ]");
                                //Process Start
                                new Thread(() => startUnfollow()).Start();
                            }
                            else
                            {
                                AddToUnfollowLogger("[ " + DateTime.Now + " ] => [ Number Format Incorrect]");
                            }
                        }
                        else
                        {
                            AddToUnfollowLogger("[ " + DateTime.Now + " ] => [ Please enter number of days ]");
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                        GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => btnStartUnfollow_Click :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                        GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                    }
                }
                else
                {
                    AddToUnfollowLogger("[ " + DateTime.Now + " ] => [ Number Format Incorrect]");
                }
            }
            else
            {
                AddToUnfollowLogger("[ " + DateTime.Now + " ] => [ Please enter a number]");
            }
        }

        private void startUnfollow()
        {
            //Looping for every account
            foreach (string accounts in ClGlobul.accountList)
            {
                if (_boolStopUnfollow) return;
                ThreadPool.SetMaxThreads(5, 5);
                try
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(unfollwMultiThreaded), accounts);
                }
                catch (Exception ex)
                {
                    GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                    GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => startUnfollow :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                    GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                }
            }
        }

        private void unfollwMultiThreaded(object account)
        {
            try
            {
                try
                {
                    Thread.CurrentThread.IsBackground = true;
                    lst_Thread.Add(Thread.CurrentThread);
                    lst_Thread = lst_Thread.Distinct().ToList();
                }
                catch (Exception ex)
                {
                    AddToUnfollowLogger("[" + DateTime.Now + "]=>[unfollwMultiThreaded 1");
                }
                string accountDetails = string.Empty;
                try
                {
                    accountDetails = (string)account;
                }
                catch
                {
                    AddToUnfollowLogger("[" + DateTime.Now + "]=>[unfollwMultiThreaded 2");
                }

                if (!string.IsNullOrEmpty(accountDetails))
                {
                    string[] arrAccounts;
                    try
                    {
                        arrAccounts = Regex.Split(accountDetails, ":");

                        string userName = string.Empty;
                        string password = string.Empty;
                        string proxyAddress = string.Empty;
                        string ProxyPort = string.Empty;

                        try
                        {
                            if (arrAccounts.Count() == 6)
                            {
                                userName = arrAccounts[0];
                                password = arrAccounts[1];
                                proxyAddress = arrAccounts[2];
                                ProxyPort = arrAccounts[3];
                            }
                            else if (arrAccounts.Count() == 4)
                            {
                                userName = arrAccounts[0];
                                password = arrAccounts[1];
                                proxyAddress = arrAccounts[2];
                                ProxyPort = arrAccounts[3];
                            }
                            else
                            {
                                userName = arrAccounts[0];
                                password = arrAccounts[1];

                            }

                        }
                        catch
                        {
                            AddToUnfollowLogger("[" + DateTime.Now + "]=>[unfollwMultiThreaded 3");
                        }


                        InstagramManager.Classes.InstagramAccountManager InstagramAccountManager = new InstagramManager.Classes.InstagramAccountManager(userName, password, proxyAddress, ProxyPort, "", "");
                        if (_boolStopUnfollow) return;
                        //AddToUnfollowLogger("[ " + DateTime.Now + " ] => [ Logging In With :" + InstagramAccountManager.Username + " ]");
                        //string statuse = InstagramAccountManager.Login();
                        string statuse = InstagramAccountManager.MyLoginForUnfollow(ref InstagramAccountManager.httpHelper, "", "", "");
                        if (InstagramAccountManager.LoggedIn == true)
                        {
                            try
                            {
                                if (_boolStopUnfollow) return;
                                AddToUnfollowLogger("[ " + DateTime.Now + " ] => [ Logged In success with " + InstagramAccountManager.Username + " ]");
                                AddToUnfollowLogger("[ " + DateTime.Now + " ] => [ Starting UnFollow With : " + InstagramAccountManager.Username + " ]");
                                unFollow(ref InstagramAccountManager, statuse, proxyAddress, ProxyPort);
                            }
                            catch (Exception ex)
                            {

                                AddToUnfollowLogger("[" + DateTime.Now + "]=>[you already sent request to user & it seems to be private user");
                            }
                        }
                        else
                        {
                            if (_boolStopUnfollow) return;
                            AddToUnfollowLogger("[ " + DateTime.Now + " ] => [ Not Logged In with " + InstagramAccountManager.Username + " ]");
                            AddToUnfollowLogger("[ " + DateTime.Now + " ] => [ Proxy is not working " + InstagramAccountManager.Username + " ]");
                        }
                    }
                    catch (Exception ex)
                    {
                        AddToUnfollowLogger("[" + DateTime.Now + "]=>[you already sent request to user & it seems to be private user");
                    }

                }//End of if (!string.IsNullOrEmpty(accountDetails))
                else
                {
                    // Account Details Empty;
                }

            }
            catch
            {
                AddToUnfollowLogger("[" + DateTime.Now + "]=>[you already sent request to user & it seems to be private user");
            }
        }

        private void unFollow(ref InstagramManager.Classes.InstagramAccountManager accountManager, string userId, string proxyAddress, string proxyPort)
        {
            List<string> lstToUnfollow = new List<string>();
            List<string> lstGetFollowing = new List<string>();
            List<string> lstGetFollower = new List<string>();
            List<string> lstFollowFromDatabase = new List<string>();

            //Get List From Database
            if (_boolStopUnfollow) return;
            lstFollowFromDatabase = getDataList(accountManager.Username);
            lstFollowFromDatabase = lstFollowFromDatabase.Distinct().ToList();


            //Getting all the following List
            if (_boolStopUnfollow) return;
            AddToUnfollowLogger("[ " + DateTime.Now + " ] => [ Getting all following List for " + accountManager.Username + " ]");
            lstGetFollowing = getFollowing(ref accountManager, "follows", userId, proxyAddress, proxyPort);
            lstGetFollowing = lstGetFollowing.Distinct().ToList();


            //Get all the follower List
            if (_boolStopUnfollow) return;
            AddToUnfollowLogger("[ " + DateTime.Now + " ] => [ Getting all follower List for " + accountManager.Username + " ]");
            lstGetFollower = getFollower(ref accountManager, "followed-by", userId);
            lstGetFollower = lstGetFollower.Distinct().ToList();

            //Getting list that have not followed back            
            try
            {
                if (_boolStopUnfollow) return;
                lstToUnfollow = (lstGetFollowing.Except(lstGetFollower)).ToList();
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => unFollow :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
            }


            if (lstFollowFromDatabase.Count >= 0)
            {
                if (lstToUnfollow.Count >= 0)
                {
                    int count = 0;
                    foreach (string item in lstFollowFromDatabase)
                    {
                        string itemCopy = "";
                        try
                        {
                            if (item.Contains("http://websta.me/n/"))
                            {
                                itemCopy = getBetween(item, "http://websta.me/n/", "/");

                            }

                            if (lstToUnfollow.Contains(itemCopy))//lstToUnfollow
                            {
                                try
                                {
                                    unfollowAccount(ref accountManager, item);
                                }
                                catch { };
                                lock (_lockObject)
                                {
                                    if (_boolStopUnfollow) return;
                                    AddToUnfollowLogger("[ " + DateTime.Now + " ] => [ Delaying for " + delayUnfollow + " seconds ]");
                                    Thread.Sleep(delayUnfollow * 1000);
                                }
                                count++;
                                if (count == noOfAccountUnfollow)
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            //GlobusFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GlobusFileHelper.ErrorLogFilePathForComment);
                            //GlobusFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => unFollow :=> " + ex.Message, GlobusFileHelper.ErrorLogFilePathForComment);
                            //GlobusFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GlobusFileHelper.ErrorLogFilePathForComment);
                        }
                    }
                }

                else
                {
                    AddToUnfollowLogger("[ " + DateTime.Now + " ] => [ No Accounts to Unfollow for " + accountManager.Username + " ]");
                }
            }


            else
            {
                AddToUnfollowLogger("[ " + DateTime.Now + " ] => [ None followed for " + accountManager.Username + " during specified time ]");
            }

            AddToUnfollowLogger("[ " + DateTime.Now + " ] => [ Unfollow completed with " + accountManager.Username + " ]");
        }

        private List<string> getFollowing(ref InstagramManager.Classes.InstagramAccountManager accountManager, string type, string userId, string ProxyAddress, string proxyPort)
        {
            const string websta = "http://websta.me/";
            List<string> lst = new List<string>();
            string pageSource = string.Empty;
            try
            {
                try
                {
                    pageSource = accountManager.httpHelper.getHtmlfromUrlProxy(new Uri(websta + type + "/" + userId), ProxyAddress, int.Parse(proxyPort), "", "");
                }
                catch
                {
                    pageSource = accountManager.httpHelper.getHtmlfromUrlProxy(new Uri(websta + type + "/" + userId), "", 80, "", "");
                }
                if (string.IsNullOrEmpty(pageSource))
                {
                    pageSource = accountManager.httpHelper.getHtmlfromUrlProxy(new Uri(websta + type + "/" + userId), ProxyAddress, int.Parse(proxyPort), "", "");
                }
                else
                {
                    pageSource = accountManager.httpHelper.getHtmlfromUrlProxy(new Uri(websta + type + "/" + userId), "", 80, "", "");
                }

                if (!string.IsNullOrEmpty(pageSource) && pageSource.Contains("<div class=\"pull-left\""))
                {
                    string[] arr = Regex.Split(pageSource, "<div class=\"pull-left\"");
                    arr = Enumerable.Skip(arr, 1).ToArray();

                    foreach (string arrItems in arr)
                    {
                        try
                        {
                            if (arrItems.Contains("<a href=\"/n/"))
                            {
                                string user = string.Empty;
                                try
                                {
                                    user = ScrapUserName.getBetween(arrItems, "<a href=\"/n/", "\"");
                                }
                                catch (Exception ex)
                                {
                                    GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                                    GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => getFollowing :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                                    GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                                }
                                if (!string.IsNullOrEmpty(user))
                                {
                                    lst.Add(user);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                            GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => getFollowing :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                            GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                        }
                    }
                }

                #region pagination
                while (pageSource.Contains("<ul class=\"pager\">"))
                {
                    string zone = string.Empty;
                    string link = string.Empty;
                    try
                    {
                        try
                        {
                            zone = ScrapUserName.getBetween(pageSource, "<ul class=\"pager\">", "</ul>");
                        }
                        catch { }

                        #region Get Pagination Link in Variable link
                        if (!string.IsNullOrEmpty(zone) && zone.Contains("Next Page") && zone.Contains("<a href=\"/"))
                        {
                            try
                            {
                                try
                                {
                                    zone = zone.Substring(zone.LastIndexOf("<a href=\"/"));
                                }
                                catch { }
                                if (!string.IsNullOrEmpty(zone))
                                {
                                    int indexStart = zone.LastIndexOf("?");
                                    int indexEnd = zone.LastIndexOf("\"");
                                    try
                                    {
                                        link = zone.Substring(indexStart, indexEnd - indexStart);
                                    }
                                    catch { }
                                }
                            }
                            catch { }

                        }//end of if (!string.IsNullOrEmpty(zone))
                        else
                        {
                            break;
                        }
                        #endregion

                        if (!string.IsNullOrEmpty(link))
                        {
                            try
                            {
                                pageSource = accountManager.httpHelper.getHtmlfromUrlProxy(new Uri(websta + type + "/" + userId + link), "", 80, "", "");
                            }
                            catch { }
                            if (string.IsNullOrEmpty(pageSource))
                            {
                                pageSource = accountManager.httpHelper.getHtmlfromUrlProxy(new Uri(websta + type + "/" + userId + link), "", 80, "", "");
                            }

                            if (!string.IsNullOrEmpty(pageSource) && pageSource.Contains("<div class=\"pull-left\""))
                            {
                                string[] arr = Regex.Split(pageSource, "<div class=\"pull-left\"");
                                arr = Enumerable.Skip(arr, 1).ToArray();

                                foreach (string arrItems in arr)
                                {
                                    try
                                    {
                                        if (arrItems.Contains("<a href=\"/n/"))
                                        {
                                            string user = string.Empty;
                                            try
                                            {
                                                user = ScrapUserName.getBetween(arrItems, "<a href=\"/n/", "\"");
                                            }
                                            catch { }
                                            if (!string.IsNullOrEmpty(user))
                                            {
                                                lst.Add(user);
                                            }
                                        }
                                    }
                                    catch { }
                                }
                            }
                            else
                            {
                                //Exit from the while loop
                                break;
                            }



                        }//End of if (!string.IsNullOrEmpty(link))
                        else
                        {
                            //Exit from the while loop
                            break;
                        }
                    }
                    catch { }
                }//End of while
                #endregion

            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => getFollowing :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
            }
            return lst;
        }

        private List<string> getFollower(ref InstagramManager.Classes.InstagramAccountManager accountManager, string type, string userId)
        {
            const string websta = "http://websta.me/";
            List<string> lst = new List<string>();
            string pageSource = string.Empty;
            string followerZone = string.Empty;
            try
            {
                pageSource = accountManager.httpHelper.getHtmlfromUrlProxy(new Uri(websta + type + "/" + userId), "", 80, "", "");

                if (!string.IsNullOrEmpty(pageSource) && pageSource.Contains("<ul class=\"userlist\">") && pageSource.Contains("<ul class=\"pager nm\">"))
                {
                    try
                    {
                        followerZone = ScrapUserName.getBetween(pageSource, "<ul class=\"userlist\">", "<ul class=\"pager nm\">");
                    }
                    catch { }

                    if (!string.IsNullOrEmpty(followerZone))
                    {
                        string[] arr = Regex.Split(followerZone, "<li>");
                        arr = Enumerable.Skip(arr, 1).ToArray();
                        foreach (string item in arr)
                        {
                            string user = string.Empty;
                            try
                            {
                                if (item.Contains("<a href=\"/n/") && item.Contains("\""))
                                {
                                    user = ScrapUserName.getBetween(item, "<a href=\"/n/", "\"");
                                }
                                if (!string.IsNullOrEmpty(user))
                                {
                                    lst.Add(user);
                                }
                            }
                            catch (Exception ex)
                            {
                                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => getFollowing :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                            }
                        }
                    }
                }

                #region Pagination
                while (pageSource.Contains("<ul class=\"pager nm\">"))
                {
                    string zone = string.Empty;
                    string link = string.Empty;
                    try
                    {
                        try
                        {
                            zone = ScrapUserName.getBetween(pageSource, "<ul class=\"pager nm\">", "</ul>");
                        }
                        catch { }

                        #region Get Pagination Link in Variable link
                        if (!string.IsNullOrEmpty(zone) && zone.Contains("Next Page") && zone.Contains("<a href=\"/"))
                        {
                            try
                            {
                                try
                                {
                                    zone = zone.Substring(zone.LastIndexOf("<a href=\"/"));
                                }
                                catch { }
                                if (!string.IsNullOrEmpty(zone))
                                {
                                    int indexStart = zone.LastIndexOf("?");
                                    int indexEnd = zone.LastIndexOf("\"");
                                    try
                                    {
                                        link = zone.Substring(indexStart, indexEnd - indexStart);
                                    }
                                    catch (Exception ex)
                                    {
                                        GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                                        GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => Pagination :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                                        GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => Pagination :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                            }
                        }
                        else
                        {
                            break;
                        }
                        #endregion

                        if (!string.IsNullOrEmpty(link))
                        {
                            pageSource = accountManager.httpHelper.getHtmlfromUrlProxy(new Uri(websta + type + "/" + userId + link), "", 80, "", "");
                            if (string.IsNullOrEmpty(pageSource))
                            {
                                pageSource = accountManager.httpHelper.getHtmlfromUrlProxy(new Uri(websta + type + "/" + userId + link), "", 80, "", "");
                            }
                            if (!string.IsNullOrEmpty(pageSource) && pageSource.Contains("<ul class=\"userlist\">") && pageSource.Contains("<ul class=\"pager nm\">"))
                            {
                                followerZone = string.Empty;
                                try
                                {
                                    followerZone = ScrapUserName.getBetween(pageSource, "<ul class=\"userlist\">", "<ul class=\"pager nm\">");
                                }
                                catch (Exception ex)
                                {
                                    GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                                    GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => Pagination :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                                    GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                                }

                                if (!string.IsNullOrEmpty(followerZone))
                                {
                                    string[] arr = Regex.Split(followerZone, "<li>");
                                    arr = Enumerable.Skip(arr, 1).ToArray();
                                    foreach (string item in arr)
                                    {
                                        string user = string.Empty;
                                        try
                                        {
                                            if (item.Contains("<a href=\"/n/") && item.Contains("\""))
                                            {
                                                user = ScrapUserName.getBetween(item, "<a href=\"/n/", "\"");
                                            }
                                            if (!string.IsNullOrEmpty(user))
                                            {
                                                lst.Add(user);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                                            GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => Pagination :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                                            GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                        GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => Pagination :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                        GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                    }
                }
                #endregion

            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => Pagination :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
            }
            return lst;
        }

        private List<string> getDataList(string Username)
        {
            List<string> lstFollowFromDatabase = new List<string>();
            DataSet ds = new DataSet();
            try
            {
                ds = QueryExecuter.getFollowInfo(Username);
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => getDataList :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
            }
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                try
                {
                    string dateOfFollow = string.Empty;
                    dateOfFollow = ds.Tables[0].Rows[i].ItemArray[1].ToString();
                    DateTime followDate = Convert.ToDateTime(dateOfFollow);

                    TimeSpan TimeSpan = DateTime.Now - followDate;
                    int Days = TimeSpan.Days;
                    if (Days > timeToUnfollow)
                    {
                        lstFollowFromDatabase.Add(ds.Tables[0].Rows[i].ItemArray[0].ToString());
                    }
                }
                catch (Exception ex)
                {
                    GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                    GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => getDataList :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                    GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                }
            }
            return lstFollowFromDatabase;
        }

        private void unfollowAccount(ref InstagramManager.Classes.InstagramAccountManager accountManager, string account)
        {
            string pageSource = string.Empty;
            string response = string.Empty;
            string profileId = string.Empty;
            const string websta = "http://websta.me/api/relationships/";
            const string accountUrl = "http://websta.me/n/";
            try
            {
                try
                {
                    UnfollowListListThread.Add(Thread.CurrentThread);
                    UnfollowListListThread.Distinct();
                    Thread.CurrentThread.IsBackground = true;
                }
                catch
                {
                }
                try
                {

                    pageSource = accountManager.httpHelper.getHtmlfromUrlProxy(new Uri(accountUrl + account), "", 80, "", "");
                }
                catch { };
                if (!string.IsNullOrEmpty(pageSource))
                {
                    if (pageSource.Contains("<ul class=\"list-inline user-"))
                    {
                        try
                        {
                            profileId = ScrapUserName.getBetween(pageSource, "<ul class=\"list-inline user-", "\">");
                        }
                        catch { }
                        if (!string.IsNullOrEmpty(profileId) && NumberHelper.ValidateNumber(profileId))
                        {
                            try
                            {
                                response = accountManager.httpHelper.postFormData(new Uri(websta + profileId), "action=unfollow", accountUrl + account, "");
                            }
                            catch { }
                            if (!string.IsNullOrEmpty(response) && response.Contains("OK"))
                            {
                                if (_boolStopUnfollow) return;
                                string status = string.Empty;
                                try
                                {
                                    status = QueryExecuter.getFollowStatus(accountManager.Username, account);
                                  
                                    switch (status)
                                    {
                                        //case "Followed": QueryExecuter.updateFollowStatus(accountManager.Username, account, "Unfollowed");
                                        case "Followed": QueryExecuter.updateFollowStatus(accountManager.Username, account, "Unfollowed");
                                            AddToUnfollowLogger("[ " + DateTime.Now + " ] => [ Unfollowed " + account + " from " + accountManager.Username + " ]");
                                            break;
                                        case "Unfollowed":
                                            AddToUnfollowLogger("[ " + DateTime.Now + " ] => [ Not Followed " + account + " from " + accountManager.Username + " ]");
                                            break;
                                        default:
                                            AddToUnfollowLogger("[ " + DateTime.Now + " ] => [ " + account + " = Unfollowed " + accountManager.Username + " ]");
                                            QueryExecuter.updateFollowStatus(accountManager.Username, account, "Unfollowed");
                                            break;
                                    }

                                }
                                catch { }

                                //AddToUnfollowLogger("[ " + DateTime.Now + " ] => [ Unfollowed " + account + " from " + accountManager.Username + " ]");
                            }
                            else
                            {
                                if (_boolStopUnfollow) return;
                                AddToUnfollowLogger("[ " + DateTime.Now + " ] => [ Could not Unfollow " + account + " from " + accountManager.Username + " ]");
                            }
                        }
                    }
                }
                else
                {
                    AddToUnfollowLogger("[ " + DateTime.Now + " ] => [ The remote server returned an error: (404) Not Found. " + account + " from " + accountManager.Username + " ]");
                }
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => unfollowAccount :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
            }
        }

        private void panelUserScrapper_Paint(object sender, PaintEventArgs e)
        {
            Graphics g;

            g = e.Graphics;

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //// Draw the image.
            g.DrawImage(PanelImage, 0, 0, this.Width, this.Height);
        }

        private void panelHashtag_Paint(object sender, PaintEventArgs e)
        {
            Graphics g;

            g = e.Graphics;

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //// Draw the image.
            g.DrawImage(PanelImage, 0, 0, this.Width, this.Height);
        }

        private void btnUploadUnfollower_Click(object sender, EventArgs e)
        {
            try
            {
                ClGlobul.lstUnfollowerList.Clear();
                TextUnfollower.Text = "";
                txtUnfollowSingle.Enabled = false;

                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    if (ofd.ShowDialog().ToString().Equals("OK"))
                    {
                        TextUnfollower.Text = ofd.FileName;
                        ClGlobul.lstUnfollowerList = Globussoft.GramBoardProFileHelper.ReadFiletoStringList(TextUnfollower.Text);
                        //Add Logger
                        //GlobusLogHelper.log.Info("[ " + ClGlobul.lstUnfollowerList.Count + " Posts Uploaded ]");
                        AddToUnfollowLogger("[ " + ClGlobul.lstUnfollowerList.Count + " User Uploaded ]");
                    }
                    else
                    {
                        //If canceled
                        ClGlobul.lstUnfollowerList.Clear();
                        TextUnfollower.Text = "";
                        //ADD Logger
                        //GlobusLogHelper.log.Info("[ No Posts uploaded ]");
                        AddToUnfollowLogger("[ No User uploaded ]");
                    }
                }
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => btnUploadUnfollower_Click :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
            }
        }
        public static List<Thread> UnfollowListListThread;
        private void btnStarUnfollowList_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(TextUnfollower.Text) && string.IsNullOrEmpty(txtUnfollowSingle.Text))
                {
                    MessageBox.Show("Please Upload File or Name to Unfollow");
                    return;
                }
            }
            catch { };


            new Thread(() =>
            {
                try
                {
                    UnfollowListListThread.Add(Thread.CurrentThread);
                    UnfollowListListThread.Distinct();
                    Thread.CurrentThread.IsBackground = true;
                }
                catch
                {
                }

                try
                {
                    ThreadPool.SetMaxThreads(5, 5);

                    if (!string.IsNullOrEmpty(txtUnfollowSingle.Text.Trim()))
                    {
                        //add following in followingList...
                        ClGlobul.lstUnfollowerList.Clear();

                        string unfollower = txtUnfollowSingle.Text.ToString();

                        if (unfollower.Contains(','))
                        {
                            string[] Data = unfollower.Split(',');

                            foreach (var item in Data)
                            {


                                ClGlobul.lstUnfollowerList.Add(item);
                            }
                        }
                        else
                        {
                            ClGlobul.lstUnfollowerList.Add(txtUnfollowSingle.Text);
                        }
                    }




                    //if (ClGlobul.isUnfollowSingle)
                    //{
                    //    try
                    //    {

                    //        ClGlobul.lstUnfollowerList.Add(txtUnfollowSingle.Text);
                    //    }
                    //    catch
                    //    { }
                    //}

                    foreach (string account in DemoStagramPro.ClGlobul.accountList)
                    {
                        try
                        {
                            AddToUnfollowLogger("[ " + DateTime.Now + " ] => [ Process Started ]");
                            string[] accountAndPass = account.Split(':');
                            string AccountName = string.Empty;
                            string AccountPass = string.Empty;
                            if (accountAndPass.Length == 2)
                            {
                                AccountName = accountAndPass[0].Replace("\0", string.Empty).Trim();
                                AccountPass = accountAndPass[1].Replace("\0", string.Empty).Trim();
                            }
                            else if (accountAndPass.Length == 4 || accountAndPass.Length == 6)
                            {
                                AccountName = accountAndPass[0].Replace("\0", string.Empty).Trim();
                                AccountPass = accountAndPass[1].Replace("\0", string.Empty).Trim();

                            }
                            ThreadPool.QueueUserWorkItem(new WaitCallback(unfollowListMultiThreaded), new object[] { AccountName, AccountPass });



                        }
                        catch (Exception ex)
                        {
                            GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                            GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => btnUploadUnfollower_Click :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                            GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                        }
                    }
                }
                catch (Exception ex)
                {
                    GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                    GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => btnUploadUnfollower_Click :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                    GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                }
            }).Start();
        }

        private void unfollowListMultiThreaded(object parameters)
        {

            try
            {
                UnfollowListListThread.Add(Thread.CurrentThread);
                UnfollowListListThread.Distinct();
                Thread.CurrentThread.IsBackground = true;
            }
            catch
            {
            }

            try
            {
               

                Array paramArray = new object[1];
                paramArray = (Array)parameters;

                string AccountName = string.Empty;
                try
                {
                    AccountName = paramArray.GetValue(0).ToString();
                }
                catch (Exception ex)
                {
                    GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                    GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => unfollowListMultiThreaded :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                    GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                }

                string AccountPass = string.Empty;
                try
                {
                    AccountPass = paramArray.GetValue(1).ToString();
                }
                catch (Exception ex)
                {
                    GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                    GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => unfollowListMultiThreaded :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                    GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                }
                string account = AccountName + ":" + AccountPass;
             

                InstagramManager.Classes.InstagramAccountManager InstagramAccountManager = new InstagramManager.Classes.InstagramAccountManager(AccountName, AccountPass, "", "", "", "");
                string statuse = InstagramAccountManager.MyLoginandComment(ref InstagramAccountManager.httpHelper, "", "", "");
                AddToUnfollowLogger("[ " + DateTime.Now + " ] => [ Logged In success with " + InstagramAccountManager.Username + " ]");
                AddToUnfollowLogger("[ " + DateTime.Now + " ] => [ Starting UnFollow With : " + InstagramAccountManager.Username + " ]");

                if (InstagramAccountManager.LoggedIn == true)
                {
                    foreach (string item in ClGlobul.lstUnfollowerList)
                    {
                        try
                        {
                            unfollowAccount(ref InstagramAccountManager, item);

                            if (!string.IsNullOrEmpty(textmindelay.Text) && NumberHelper.ValidateNumber(textmindelay.Text))
                            {
                                mindelay = Convert.ToInt32(textmindelay.Text);
                            }
                            if (!string.IsNullOrEmpty(textmaxdelay.Text) && NumberHelper.ValidateNumber(textmaxdelay.Text))
                            {
                                maxdelay = Convert.ToInt32(textmaxdelay.Text);
                            }

                            lock (_lockObject)
                            {
                                Random rn = new Random();
                                int delay = RandomNumberGenerator.GenerateRandom(mindelay, maxdelay);
                                delay = rn.Next(mindelay,maxdelay);
                                AddToUnfollowLogger("[ " + DateTime.Now + " ] => [ Delay For " + delay + " Seconds ]");
                                Thread.Sleep(delay * 1000);
                            }

                        }


                        catch (Exception ex)
                        {
                            GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                            GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => unfollowListMultiThreaded :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                            GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                        }
                       
                    }
                    AddToUnfollowLogger("[ " + DateTime.Now + " ] => [Process Completed from " + InstagramAccountManager.Username + " ]");
                }
                else if (statuse.Contains("Failed"))
                {
                    AddToUnfollowLogger("[ " + DateTime.Now + " ] => [ " + account + " Failed To Login ]");
                }
                else if (statuse.Contains("AccessIssue"))
                {
                    AddToUnfollowLogger("[ " + DateTime.Now + " ] => [ " + account + " Access Issue In Login ]");
                }
                else if (statuse.Contains("Stream was not readable."))
                {

                    AddToUnfollowLogger("[ " + DateTime.Now + " ] => [ " + account + " Error in Post Data ]");
                }
                else if (statuse.Contains("The request was aborted: The operation has timed out."))
                {

                    AddToUnfollowLogger("[ " + DateTime.Now + " ] => [ " + account + " Operation timed Out To Slow Request ]");

                }
                else if (statuse.Contains("503"))
                {

                    AddToUnfollowLogger("[ " + DateTime.Now + " ] => [ " + account + " Failed To Login ]");
                }
                else if (statuse.Contains("403"))
                {

                    AddToUnfollowLogger("[ " + DateTime.Now + " ] => [ " + account + " Failed To Login ]");

                }
                else if (statuse.Contains("Please enter a correct username and password."))
                {
                    AddToUnfollowLogger("[ " + DateTime.Now + " ] => [ " + account + " : Incorect Username Or Password ]");
                }
                else if (statuse.Contains("Please enter a correct username and password."))
                {
                    AddToUnfollowLogger("[ " + DateTime.Now + " ] => [ " + account + " : Incorect Username Or Password ]");
                }
                else
                {
                    AddToUnfollowLogger("[ " + DateTime.Now + " ] => [ Could Not Login With :" + InstagramAccountManager.Username + " ]");
                }






            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => unfollowListMultiThreaded :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g;

            g = e.Graphics;

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //// Draw the image.
            g.DrawImage(PanelImage, 0, 0, this.Width, this.Height);
        }

        private void AddToUnfollowLogger(string log)
        {
            try
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    lst_UnfollowLogger.Items.Add(log);
                    lst_UnfollowLogger.SelectedIndex = lst_UnfollowLogger.Items.Count - 1;
                }));
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => AddToUnfollowLogger :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
            }

        }

        private void btnClearUploadUnFollowUser_Click(object sender, EventArgs e)
        {
            try
            {

                txtUnfollowSingle.Text = string.Empty;
                TextUnfollower.Text = string.Empty;
                //ClGlobul.accountList.Clear();
                ClGlobul.FolloConpletedList.Clear();
                ClGlobul.TotalNoOfFollow = 0;
                ClGlobul.TotalNoOfIdsForFollow = 0;
                ClGlobul.ThreadList.Clear();
                lst_Thread.Clear();
                ThreadIs = false;
                count_ThreadControllerForFollow = 0;
                lbLogger.Items.Clear();
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => Finally (clearAccounts) :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePath);
            }



            //try
            //{
            //    QueryExecuter.deleteQuery();
            //    ClGlobul.accountList.Clear();
            //    TextUnfollower.Enabled = true;
            //    TextUnfollower.Text = string.Empty;
            //    txtUnfollowSingle.Enabled = true;

            //}
            //catch (Exception ex)
            //{
            //    GlobusFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GlobusFileHelper.ErrorLogFilePathForComment);
            //    GlobusFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => btnClearUploadUnFollowUser_Click :=> " + ex.Message, GlobusFileHelper.ErrorLogFilePathForComment);
            //    GlobusFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GlobusFileHelper.ErrorLogFilePathForComment);
            //}

            //AddToLogger("[ " + DateTime.Now + " ] => [  All Accounts Cleared ]");
        }



        //************************************************ Account checker  **********************************************************************************//


        //************************************************ HashFollower Liker Comment  **********************************************************************************//
        private void btnhashFollower_Click(object sender, EventArgs e)
        {
            try
            {
                //get the hash tags from selected file...
                using (OpenFileDialog ofdImageTag = new OpenFileDialog())
                {
                    ofdImageTag.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                    ofdImageTag.Filter = "Text Files (*.txt)|*.txt";
                    if (ofdImageTag.ShowDialog() == DialogResult.OK)
                    {
                        //Add File path in Text Box....
                        txtHashFollower.Text = ofdImageTag.FileName;
                        //call Read Method file when we select a file
                        readImageTagFollowerFile(ofdImageTag.FileName);

                    }
                }
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("***********************************************************************************************", GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => btnHashTagUpload :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("***********************************************************************************************", GramBoardProFileHelper.ErrorLogFilePathForComment);
            }

        }

        public void readImageTagFollowerFile(string HashTagFilePath)
        {
            try
            {
                ClGlobul.HashTagForScrap.Clear();
                //Read Data From Selected File ....
                List<string> hashTaglist = GramBoardProFileHelper.ReadFile((string)HashTagFilePath);
                foreach (string itemHash in hashTaglist)
                {
                    if (itemHash.ElementAt(0) == '#')
                    {
                        string itemHashNew = itemHash;
                        //add Comment Id's In Globol Comment Id List ...
                        ClGlobul.HashFollower.Add(itemHashNew);
                    }
                    else
                    {
                        AddToHashLoggerDAta("[ " + DateTime.Now + " ] => [ " + itemHash + " is not Uploaded as Its not starting with # ]");

                    }
                }
                ClGlobul.HashFollower = ClGlobul.HashFollower.Distinct().ToList();
                //Show No Of Data Count In logger...
                AddToHashLoggerDAta("[ " + DateTime.Now + " ] => [ " + ClGlobul.HashFollower.Count + " Hashtags uploaded to follow. ]");
            }
            catch (Exception ex)
            {
                //when its generate any Error From here which is write in Text file ...
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => readHashTagFile :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
            }
        }

        private void btnhashLike_Click(object sender, EventArgs e)
        {
            try
            {
                //get the hash tags from selected file...
                using (OpenFileDialog ofdImageTag = new OpenFileDialog())
                {
                    ofdImageTag.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                    ofdImageTag.Filter = "Text Files (*.txt)|*.txt";
                    if (ofdImageTag.ShowDialog() == DialogResult.OK)
                    {
                        //Add File path in Text Box....
                        txtHashLike.Text = ofdImageTag.FileName;
                        //call Read Method file when we select a file
                        readImageTaghashLikerFile(ofdImageTag.FileName);

                    }
                }
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("***********************************************************************************************", GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => btnHashTagUpload :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("***********************************************************************************************", GramBoardProFileHelper.ErrorLogFilePathForComment);
            }
        }

        public void readImageTaghashLikerFile(string HashTagFilePath)
        {
            try
            {
                ClGlobul.HashTagForScrap.Clear();
                //Read Data From Selected File ....
                List<string> hashTaglist = GramBoardProFileHelper.ReadFile((string)HashTagFilePath);
                foreach (string itemHash in hashTaglist)
                {
                    if (itemHash.ElementAt(0) == '#')
                    {
                        string itemHashNew = itemHash;
                        //add Comment Id's In Globol Comment Id List ...
                        ClGlobul.HashLiker.Add(itemHashNew);
                    }
                    else
                    {

                        string itemHashNew = itemHash;
                        //add Comment Id's In Globol Comment Id List ...
                        ClGlobul.HashLiker.Add(itemHashNew);
                        //AddToHashLoggerDAta("[ " + DateTime.Now + " ] => [ " + itemHash + " is not Uploaded as Its not starting with # ]");
                    }
                }
                ClGlobul.HashLiker = ClGlobul.HashLiker.Distinct().ToList();
                //Show No Of Data Count In logger...
                AddToHashLoggerDAta("[ " + DateTime.Now + " ] => [ " + ClGlobul.HashLiker.Count + " Hashtags uploaded to like. ]");
            }
            catch (Exception ex)
            {
                //when its generate any Error From here which is write in Text file ...
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => readHashTagFile :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
            }
        }

        private void btnHashClear_Click(object sender, EventArgs e)
        {
            try
            {
                ClGlobul.ImageTagForScrap.Clear();
                txtUploadImage.Text = string.Empty;
                txtHashFollower.Text = string.Empty;
                txtHashLike.Text = string.Empty;
                txtHashComment.Text = string.Empty;
                txtHashCommentMessage.Text = string.Empty;
                txtNumberProfilesFollow.Text = string.Empty;
                txtNumberPicsVideosLike.Text = string.Empty;
                txtNumberSnapsVideosComment.Text = string.Empty;





                AddToHashLoggerDAta("[ " + DateTime.Now + " ] => [ All Hash Tags Cleared ]");
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => btnImageClear_Click :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
            }
        }

        private void btnStaop_Click(object sender, EventArgs e)
        {
            // if (_boolCanStop)
            {
                _boolCanStop = false;
                try
                {
                    new Thread(() => ImageDataStop()).Start();
                }
                catch (Exception ex)
                {
                    GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                    GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => btnImageStop_Click :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                    GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                }
            }
        }

        private void ImageDataStop()
        {
            try
            {
                ScrapImages.stopScrapImageBool = true;
                List<Thread> lstTemp = GloBoardPro.HasTagListListThread.Distinct().ToList();
                foreach (Thread items in lstTemp)
                {
                    try
                    {
                        items.Abort();
                    }
                    catch (Exception ex)
                    {
                        GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                        GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => ImageScrapStop :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                        GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                    }
                }
                // Thread.Sleep(1500);
                AddToHashLoggerDAta("[ " + DateTime.Now + " ] => [ Scraping Image Urls Aborted ]");
                AddToHashLoggerDAta("[ " + DateTime.Now + " ] => [ Process Stopped ]");
            }
            catch { }
        }
        private Object thisLock = new Object();





        private void btnhashtagStart_Click(object sender, EventArgs e)
        {
            //startProcessUsingHashTag();
            new Thread(() =>
            {
                startProcessUsingHashTag();
            }).Start();
        }
        private static int unlikeCompletionLikeCount = 0;
        private static int unlikecommentcount = 0;
        private static int unlikeLikecount = 0;

        private void startProcessUsingHashTag()
        {



            try
            {
                GloBoardPro.HasTagListListThread.Add(Thread.CurrentThread);
                GloBoardPro.HasTagListListThread.Distinct();
                Thread.CurrentThread.IsBackground = true;
            }
            catch
            {
            }

            int parsedValue;
            bool ProcessStartORnot = false;
            AddToHashLoggerDAta("[ " + DateTime.Now + " ] => [Pleasewait ]");
            List<List<string>> list_Accounts = new List<List<string>>();
            acc_checker_counter = DemoStagramPro.ClGlobul.accountList.Count();
            list_Accounts = ListUtilities.Split(DemoStagramPro.ClGlobul.accountList, Maxthread);
            ThreadPool.SetMaxThreads(Noofthreads, 5);
            GlobDramProHttpHelper httphelper = new GlobDramProHttpHelper();
            HashTag objHashTag = new HashTag();
            List<string> Usercount = new List<string>();

            try
            {
                if (!(string.IsNullOrEmpty(txtHashTagDelay.Text)))
                {
                    ClGlobul.hashTagDelay = Convert.ToInt32(txtHashTagDelay.Text);
                }
                else
                {
                    ClGlobul.hashTagDelay = 10;
                }
            }
            catch
            { }
            if (chkDivideDataFollow1.Checked && rdbDivideGivenByUser1.Checked)
            {
                if (!int.TryParse(txtDiveideByUser1.Text, out parsedValue))
                {

                    AddToLogger("[ " + DateTime.Now + " ] => [ This is a number only field.Please enter proper value in divide given by user field. ]");
                    return;
                }
            }
            try
            {
                if (ClGlobul.HashFollower.Count != 0)
                {
                    if (!string.IsNullOrEmpty(txtNumberProfilesFollow.Text))
                    {
                        ClGlobul.NumberOfProfilesToFollow = Convert.ToInt32(txtNumberProfilesFollow.Text);
                        //ClGlobul.SnapVideosCounterfollow = Convert.ToInt32(txtNumberProfilesFollow.Text) * ClGlobul.HashFollower.Count;
                    }
                    else
                    {
                        MessageBox.Show("Please enter the numbers of profiles to follow and continue.");
                        AddToHashLoggerDAta("[ " + DateTime.Now + "] " + "[ Please enter the numbers of profiles to follow and continue. ]");
                        return;
                    }
                }
                if (ClGlobul.HashLiker.Count != 0)
                {
                    if (!string.IsNullOrEmpty(txtNumberPicsVideosLike.Text))
                    {
                        ClGlobul.NumberofSnapsVideosToLike = Convert.ToInt32(txtNumberPicsVideosLike.Text);
                        //ClGlobul.SnapVideosCounter = Convert.ToInt32(txtNumberPicsVideosLike.Text) * ClGlobul.HashLiker.Count;
                    }
                    else
                    {
                        MessageBox.Show("Please enter the numbers of snaps or videos to like and continue.");
                        AddToHashLoggerDAta("[ " + DateTime.Now + "] " + "[ Please enter the numbers of snaps or videos to like and continue. ]");
                        return;
                    }
                }
                if (ClGlobul.HashComment.Count != 0)
                {
                    if (!string.IsNullOrEmpty(txtNumberSnapsVideosComment.Text))
                    {
                        ClGlobul.NumberofSnapsVideosToComment = Convert.ToInt32(txtNumberSnapsVideosComment.Text);
                       // ClGlobul.SnapVideosCounterComment = Convert.ToInt32(txtNumberSnapsVideosComment.Text) * ClGlobul.HashComment.Count;
                    }
                    else
                    {
                        MessageBox.Show("Please enter the numbers of snaps or videos to like and continue.");
                        AddToHashLoggerDAta("[ " + DateTime.Now + "] " + "[ Please enter the numbers of snaps or videos to like and continue. ]");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
            }

            #region scrapHashTagFollowr

            List<string> lstHashTagUserIdTemp = new List<string>();
            List<string> lstHashTagUserId = new List<string>();
            if (!string.IsNullOrEmpty(txtHashFollower.Text.Trim()))
            {


                foreach (string hashKeyword in ClGlobul.HashFollower)
                {

                    AddToHashLoggerDAta("[ " + DateTime.Now + "] " + "[ Scraping Users with HashTag " + hashKeyword + "]");
                    lstHashTagUserIdTemp = GetUser(hashKeyword);
                    lstHashTagUserId.AddRange(lstHashTagUserIdTemp);
                    AddToHashLoggerDAta("[ " + DateTime.Now + "] " + "[ Scraped Users with HashTag " + hashKeyword + "]");
                }
            }
            #endregion



            #region ScraperhashLiker
            List<string> lstHashTagUserIdLikeTemp = new List<string>();
            List<string> lstHashLikeTagUserLikeId = new List<string>();
            if (!string.IsNullOrEmpty(txtHashLike.Text.Trim()))
            {
                foreach (string hashLikerKeyword in ClGlobul.HashLiker)
                {

                    AddToHashLoggerDAta("[ " + DateTime.Now + "] " + "[ Scraping PhotoId with HashTag " + hashLikerKeyword + "]");
                    lstHashTagUserIdLikeTemp = GetPhotoId(hashLikerKeyword);
                    lstHashLikeTagUserLikeId.AddRange(lstHashTagUserIdLikeTemp);
                    AddToHashLoggerDAta("[ " + DateTime.Now + "] " + "[ Scraped Users with HashTag " + hashLikerKeyword + "]");
                }
            }
            #endregion

            #region hashtagcomment
            List<string> lstHashTagUserIdCommentTemp = new List<string>();
            List<string> lstHashTagCommentUserId = new List<string>();
            if (!string.IsNullOrEmpty(txtHashComment.Text.Trim()))
            {


                foreach (string hashCommentKeyword in ClGlobul.HashComment)
                {

                    AddToHashLoggerDAta("[ " + DateTime.Now + "] " + "[ Scraping Users PhotoId HashTag " + hashCommentKeyword + "]");
                    lstHashTagUserIdCommentTemp = GetPhotoId1(hashCommentKeyword);
                    lstHashTagCommentUserId.AddRange(lstHashTagUserIdCommentTemp);
                    AddToHashLoggerDAta("[ " + DateTime.Now + "] " + "[ Scraped Users PhotoId HashTag " + hashCommentKeyword + "]");

                }
            }
            #endregion
            List<List<string>> list_lstTargetUsers = new List<List<string>>();
            counter_follow = DemoStagramPro.ClGlobul.accountList.Count();
            list_Accounts = ListUtilities.Split(DemoStagramPro.ClGlobul.accountList, Maxthread);
            List<List<string>> list_lstTargetHashTag = new List<List<string>>();
            List<List<string>> list_lstTargetHashTagLike = new List<List<string>>();
            List<List<string>> list_lstTargetHashTagComment = new List<List<string>>();



            //for divide by data logic here
            if (chkDivideDataFollow1.Checked)
            {
                if (rdbDivideGivenByUser1.Checked || rdbDivideEqually1.Checked)
                {
                    int splitNo = 0;
                    if (rdbDivideEqually1.Checked)
                    {
                        splitNo = lstHashTagUserId.Count / ClGlobul.accountList.Count;
                    }
                    else if (rdbDivideGivenByUser1.Checked)
                    {
                        if (Convert.ToInt32(txtDiveideByUser1.Text.Trim()) != 0)
                        {
                            int res = Convert.ToInt32(Convert.ToInt32(txtDiveideByUser1.Text.Trim()));
                            splitNo = res;
                        }
                    }
                    if (splitNo == 0)
                    {
                        splitNo = RandomNumberGenerator.GenerateRandom(0, lstHashTagUserId.Count - 1);
                    }
                    list_lstTargetHashTag = Split(lstHashTagUserId, splitNo);
                }
            }


            if (chkDivideDataFollow1.Checked)
            {
                if (rdbDivideGivenByUser1.Checked || rdbDivideEqually1.Checked)
                {
                    int splitNo = 0;
                    if (rdbDivideEqually1.Checked)
                    {
                        splitNo = lstHashLikeTagUserLikeId.Count / ClGlobul.accountList.Count;
                    }
                    else if (rdbDivideGivenByUser1.Checked)
                    {
                        if (Convert.ToInt32(txtDiveideByUser1.Text.Trim()) != 0)
                        {
                            int res = Convert.ToInt32(Convert.ToInt32(txtDiveideByUser1.Text.Trim()));
                            splitNo = res;
                        }
                    }
                    if (splitNo == 0)
                    {
                        splitNo = RandomNumberGenerator.GenerateRandom(0, lstHashLikeTagUserLikeId.Count - 1);
                    }
                    list_lstTargetHashTagLike = Split(lstHashLikeTagUserLikeId, splitNo);
                }
            }


            if (chkDivideDataFollow1.Checked)
            {
                if (rdbDivideGivenByUser1.Checked || rdbDivideEqually1.Checked)
                {
                    int splitNo = 0;
                    if (rdbDivideEqually1.Checked)
                    {
                        splitNo = lstHashTagCommentUserId.Count / ClGlobul.accountList.Count;
                    }
                    else if (rdbDivideGivenByUser1.Checked)
                    {
                        if (Convert.ToInt32(txtDiveideByUser1.Text.Trim()) != 0)
                        {
                            int res = Convert.ToInt32(Convert.ToInt32(txtDiveideByUser1.Text.Trim()));
                            splitNo = res;
                        }
                    }
                    if (splitNo == 0)
                    {
                        splitNo = RandomNumberGenerator.GenerateRandom(0, lstHashTagCommentUserId.Count - 1);
                    }
                    list_lstTargetHashTagComment = Split(lstHashTagCommentUserId, splitNo);
                }
            }



            int LstCounter = 0;
            // unlikeCompletionCount = ClGlobul.accountList.Count;
            unlikeCompletionLikeCount = ClGlobul.accountList.Count;
            try
            {
                if (list_Accounts.Count == 0)
                {
                    AddToHashLoggerDAta("[ " + DateTime.Now + "] " + "[ Please upload account. ]");
                    return;
                }
                else
                {
                    ClGlobul.countNoOFAccountHashFollower = ClGlobul.accountList.Count;
                    ClGlobul.countNoOFAccountHashComment = ClGlobul.accountList.Count;
                    ClGlobul.countNOOfFollowersandImageDownload = ClGlobul.accountList.Count;

                    //try
                    //{
                    //    ClGlobul.SnapVideosCounterfollow = ClGlobul.SnapVideosCounterfollow * list_Accounts.Count;
                    //}
                    //catch { };
                    //try
                    //{
                    //    ClGlobul.SnapVideosCounter = ClGlobul.SnapVideosCounter * list_Accounts.Count;
                    //}
                    //catch { };
                    //try
                    //{

                    //    ClGlobul.SnapVideosCounterComment = ClGlobul.SnapVideosCounterComment * list_Accounts.Count;
                    //}
                    //catch { };

                    foreach (List<string> listAccounts in list_Accounts)
                    {
                        foreach (string val in listAccounts)
                        {
                            //soniaghezzo94:mdag123456:93.118.68.148:3128
                            string[] StrArrar = Regex.Split(val, ":");
                            string account = string.Empty;
                            string pass = string.Empty;
                            string proxyadd = string.Empty;
                            string proxyport = string.Empty;
                            string proxyUser = string.Empty;
                            string proxyPass = string.Empty;
                            try
                            {
                                account = StrArrar[0];
                            }
                            catch
                            {
                                account = StrArrar[0];
                            }
                            try
                            {
                                pass = StrArrar[1];
                            }
                            catch
                            {
                                pass = StrArrar[1];
                            }
                            try
                            {
                                proxyadd = StrArrar[2];
                            }

                            catch
                            {
                                proxyadd = StrArrar[2];
                            }
                            try
                            {
                                proxyport = StrArrar[3];
                            }
                            catch
                            {
                                proxyport = StrArrar[3];
                            }
                            try
                            {
                                proxyUser = StrArrar[4];
                            }
                            catch
                            {
                                proxyUser = StrArrar[4];
                            }
                            try
                            {
                                proxyPass = StrArrar[5];
                            }
                            catch
                            {
                                proxyPass = StrArrar[5];
                            }
                            string txtUrl = txtHashFollower.Text;
                            string txtLikeComment = txtHashLike.Text;


                            if (LstCounter == list_lstTargetHashTag.Count && (chkDivideDataFollow1.Checked))
                            {
                                //AddToLogger("[ " + DateTime.Now + " ] => [ Account is grater than List of users. ]");
                                // break;
                            }

                            List<string> list_lstTargetHashTag_item = new List<string>();

                            try
                            {
                                if (chkDivideDataFollow1.Checked)
                                {
                                    list_lstTargetHashTag_item = list_lstTargetHashTag[LstCounter];
                                }
                                else
                                {
                                    list_lstTargetHashTag_item = lstHashTagUserId;
                                }

                            }
                            catch { }

                            List<string> list_lstTargetHashTagLike_item = new List<string>();
                            try
                            {
                                if (chkDivideDataFollow1.Checked)
                                {
                                    list_lstTargetHashTagLike_item = list_lstTargetHashTagLike[LstCounter];
                                }
                                else
                                {
                                    list_lstTargetHashTagLike_item = lstHashLikeTagUserLikeId;
                                }

                            }
                            catch { }


                            List<string> list_lstTargetHashTagComment_item = new List<string>();
                            try
                            {
                                if (chkDivideDataFollow1.Checked)
                                {
                                    list_lstTargetHashTagComment_item = list_lstTargetHashTagComment[LstCounter];
                                }
                                else
                                {
                                    list_lstTargetHashTagComment_item = lstHashTagCommentUserId;
                                }

                            }
                            catch { }

                            InstagramManager.Classes.InstagramAccountManager InstagramAccountManager = new InstagramManager.Classes.InstagramAccountManager(account, pass, proxyadd, proxyport, proxyUser, proxyPass);
                            string status = InstagramAccountManager.LoginNew(ref InstagramAccountManager.httpHelper);
                            if (!status.Contains("Success"))
                            {
                                status = InstagramAccountManager.LoginNew(ref InstagramAccountManager.httpHelper);
                            }
                            AddToHashLoggerDAta("[ " + DateTime.Now + "] " + "[ Logged in with account : " + InstagramAccountManager.Username + " ]");
                            // string ddd = Uri.EscapeDataString(txtUrl);
                            try
                            {
                                if (ClGlobul.HashFollower.Count != 0)
                                {
                                    //foreach (string hashTagFollow in ClGlobul.HashFollower) //commented for divide data
                                    //foreach (string hashTagFollow in list_lstTargetHashTag_item)
                                    {
                                        new Thread(() =>
                                        {
                                            try
                                            {
                                                GloBoardPro.HasTagListListThread.Add(Thread.CurrentThread);
                                                GloBoardPro.HasTagListListThread.Distinct();
                                                Thread.CurrentThread.IsBackground = true;
                                            }
                                            catch
                                            {
                                            }
                                            objHashTag.HashTagFollow(ref InstagramAccountManager, list_lstTargetHashTag_item);
                                        }).Start();
                                    }
                                }
                                if (ClGlobul.HashLiker.Count != 0)
                                {
                                    //foreach (string hashTagLike in ClGlobul.HashLiker)
                                    {
                                        new Thread(() =>
                                      {
                                          // objHashTag.HashTagLike(ref InstagramAccountManager, hashTagLike);
                                          objHashTag.HashTagLike(ref InstagramAccountManager, list_lstTargetHashTagLike_item);
                                      }).Start();
                                    }
                                }
                                if (ClGlobul.HashComment.Count != 0)
                                {
                                    //foreach (string hashTagComment in ClGlobul.HashComment)
                                    {
                                        new Thread(() =>
                                        {
                                            objHashTag.HashTagComment(ref InstagramAccountManager, list_lstTargetHashTagComment_item);
                                        }).Start();
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                            LstCounter++;
                            Thread.Sleep(1000);
                        }
                    }
                }
            }

            catch (Exception ex)
            { }

            finally
            {

                {
                    if (unlikeCompletionLikeCount == 0)
                    {

                    }
                }
            }
        }


        public List<string> GetUser(string hashTag)
        {
            List<string> lstUser = new List<string>();
            try
            {
                string url = "http://websta.me/search/" + hashTag.Replace("#", "%23");
                GlobDramProHttpHelper objInstagramUser = new GlobDramProHttpHelper();

                string pageSource = objInstagramUser.getHtmlfromUrl(new Uri(url),"");
                if (string.IsNullOrEmpty(pageSource))
                {
                    pageSource = objInstagramUser.getHtmlfromUrl(new Uri(url),"");
                }

                if (!string.IsNullOrEmpty(pageSource))
                {
                    if (pageSource.Contains("username\" href="))
                    {
                        string ScrapUser = string.Empty;
                        string[] arr = Regex.Split(pageSource, "username\" href=");
                        if (arr.Length > 1)
                        {
                            arr = arr.Skip(1).ToArray();
                            foreach (string itemarr in arr)
                            {
                                try
                                {
                                    if (itemarr.Contains("<a href="))
                                    {
                                        ScrapUser = getBetween(itemarr, "", "</a>");
                                        string[] urldata = Regex.Split(ScrapUser, ">");
                                        string username = urldata[1];
                                        if (!string.IsNullOrEmpty(username))
                                        {
                                            lstUser.Add(username);
                                            lstUser = lstUser.Distinct().ToList();

                                            if (lstUser.Count >= ClGlobul.NumberOfProfilesToFollow)
                                            {
                                                return lstUser;
                                            }
                                        }
                                    }
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            return lstUser;
        }

        public List<string> GetPhotoId(string hashTag)
        {

            try
            {
                GloBoardPro.HasTagListListThread.Add(Thread.CurrentThread);
                GloBoardPro.HasTagListListThread.Distinct();
                Thread.CurrentThread.IsBackground = true;
            }
            catch
            {
            }

            //string url = "http://websta.me/" + "tag/" + hashTag;
            string url = "http://websta.me/" + "tag/" + hashTag.Replace("%23", "").Replace("#", "");
            GlobDramProHttpHelper objInstagramUser = new GlobDramProHttpHelper();
            List<string> lstPhotoId = new List<string>();

            string pageSource = objInstagramUser.getHtmlfromUrl(new Uri(url),"");
            if (string.IsNullOrEmpty(pageSource))
            {
                pageSource = objInstagramUser.getHtmlfromUrl(new Uri(url),"");
            }
            if (!string.IsNullOrEmpty(pageSource))
            {
                if (pageSource.Contains("<div class=\"mainimg_wrapper\">"))
                {
                    string[] arr = Regex.Split(pageSource, "<div class=\"mainimg_wrapper\">");
                    if (arr.Length > 1)
                    {
                        arr = arr.Skip(1).ToArray();
                        foreach (string itemarr in arr)
                        {
                            try
                            {
                                string startString = "<a href=\"/p/";
                                string endString = "\" class=\"mainimg\"";
                                string imageId = string.Empty;
                                string imageSrc = string.Empty;
                                if (itemarr.Contains("<a href=\"/p/"))
                                {
                                    int indexStart = itemarr.IndexOf("<a href=\"/p/");
                                    string itemarrNow = itemarr.Substring(indexStart);
                                    if (itemarrNow.Contains(startString) && itemarrNow.Contains(endString))
                                    {
                                        try
                                        {
                                            imageId = getBetween(itemarrNow, startString, endString).Replace("/", "");
                                        }
                                        catch { }
                                        if (!string.IsNullOrEmpty(imageId))
                                        {
                                            lstPhotoId.Add(imageId);
                                            lstPhotoId.Distinct();
                                            if (lstPhotoId.Count >= ClGlobul.NumberofSnapsVideosToLike)
                                            {
                                                return lstPhotoId;
                                            }

                                            //imageId = "http://websta.me"+imageId;
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {

                            }

                        }

                        #region pagination

                        string pageLink = string.Empty;
                        while (true)
                        {
                            //if (stopScrapImageBool) return;
                            string startString = "<a href=\"/p/";
                            string endString = "\" class=\"mainimg\"";
                            string imageId = string.Empty;
                            string imageSrc = string.Empty;

                            if (!string.IsNullOrEmpty(pageLink))
                            {
                                pageSource = objInstagramUser.getHtmlfromUrl(new Uri(pageLink),"");
                            }

                            if (pageSource.Contains("<ul class=\"pager\">") && pageSource.Contains("rel=\"next\">"))
                            {
                                try
                                {
                                    pageLink = getBetween(pageSource, "<ul class=\"pager\">", "rel=\"next\">");
                                }
                                catch { }
                                if (!string.IsNullOrEmpty(pageLink))
                                {
                                    try
                                    {
                                        int len = pageLink.IndexOf("<a href=\"");
                                        len = len + ("<a href=\"").Length;
                                        pageLink = pageLink.Substring(len);
                                        pageLink = pageLink.Trim();
                                        pageLink = pageLink.TrimEnd(new char[] { '"' });
                                        pageLink = "http://websta.me/" + pageLink;
                                    }
                                    catch { }
                                    if (!string.IsNullOrEmpty(pageLink))
                                    {
                                        string response = string.Empty;
                                        try
                                        {
                                            response = objInstagramUser.getHtmlfromUrl(new Uri(pageLink),"");
                                        }
                                        catch { }
                                        if (!string.IsNullOrEmpty(response))
                                        {
                                            if (response.Contains("<div class=\"mainimg_wrapper\">"))
                                            {
                                                try
                                                {
                                                    string[] arr1 = Regex.Split(response, "<div class=\"mainimg_wrapper\">");
                                                    if (arr1.Length > 1)
                                                    {
                                                        arr1 = arr1.Skip(1).ToArray();
                                                        foreach (string items in arr1)
                                                        {
                                                            try
                                                            {
                                                                //if (stopScrapImageBool) return;
                                                                if (items.Contains("<a href=\"/p/"))
                                                                {
                                                                    int indexStart = items.IndexOf("<a href=\"/p/");
                                                                    string itemarrNow = items.Substring(indexStart);

                                                                    try
                                                                    {
                                                                        imageId = getBetween(itemarrNow, startString, endString).Replace("/", "");
                                                                    }
                                                                    catch { }
                                                                    if (!string.IsNullOrEmpty(imageId))
                                                                    {
                                                                        lstPhotoId.Add(imageId);
                                                                        lstPhotoId.Distinct();
                                                                        if (lstPhotoId.Count >= ClGlobul.NumberofSnapsVideosToLike)
                                                                        {
                                                                            return lstPhotoId;
                                                                        }

                                                                        //imageId = "http://websta.me"+imageId;
                                                                    }


                                                                    //counter++;

                                                                    //Addtologger("Image DownLoaded with ImageName  "+imageId+"_"+counter);
                                                                    if (lstPhotoId.Count >= ClGlobul.NumberofSnapsVideosToLike)
                                                                    {
                                                                        return lstPhotoId;
                                                                    }
                                                                }
                                                            }

                                                            catch { }
                                                        }
                                                        if (lstPhotoId.Count >= ClGlobul.NumberofSnapsVideosToLike)
                                                        {
                                                            return lstPhotoId;
                                                        }
                                                    }
                                                }
                                                catch { }

                                            }
                                        }
                                        else
                                        {

                                        }

                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        #endregion
                    }
                }
            }

            return lstPhotoId;
        }



        public List<string> GetPhotoId1(string hashTag)
        {

            try
            {
                GloBoardPro.HasTagListListThread.Add(Thread.CurrentThread);
                GloBoardPro.HasTagListListThread.Distinct();
                Thread.CurrentThread.IsBackground = true;
            }
            catch
            {
            }

            //string url = "http://websta.me/" + "tag/" + hashTag;
            string url = "http://websta.me/" + "tag/" + hashTag.Replace("%23", "").Replace("#", "");
            GlobDramProHttpHelper objInstagramUser = new GlobDramProHttpHelper();
            List<string> lstPhotoId = new List<string>();

            string pageSource = objInstagramUser.getHtmlfromUrl(new Uri(url),"");
            if (string.IsNullOrEmpty(pageSource))
            {
                pageSource = objInstagramUser.getHtmlfromUrl(new Uri(url),"");
            }
            if (!string.IsNullOrEmpty(pageSource))
            {
                if (pageSource.Contains("<div class=\"mainimg_wrapper\">"))
                {
                    string[] arr = Regex.Split(pageSource, "<div class=\"mainimg_wrapper\">");
                    if (arr.Length > 1)
                    {
                        arr = arr.Skip(1).ToArray();
                        foreach (string itemarr in arr)
                        {
                            try
                            {
                                string startString = "<a href=\"/p/";
                                string endString = "\" class=\"mainimg\"";
                                string imageId = string.Empty;
                                string imageSrc = string.Empty;
                                if (itemarr.Contains("<a href=\"/p/"))
                                {
                                    int indexStart = itemarr.IndexOf("<a href=\"/p/");
                                    string itemarrNow = itemarr.Substring(indexStart);
                                    if (itemarrNow.Contains(startString) && itemarrNow.Contains(endString))
                                    {
                                        try
                                        {
                                            imageId = getBetween(itemarrNow, startString, endString).Replace("/", "");
                                        }
                                        catch { }
                                        if (!string.IsNullOrEmpty(imageId))
                                        {
                                            lstPhotoId.Add(imageId);
                                            lstPhotoId.Distinct();
                                            if (lstPhotoId.Count >= ClGlobul.NumberofSnapsVideosToComment)
                                            {
                                                return lstPhotoId;
                                            }

                                            //imageId = "http://websta.me"+imageId;
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {

                            }

                        }

                        #region pagination

                        string pageLink = string.Empty;
                        while (true)
                        {

                            try
                            {
                                GloBoardPro.HasTagListListThread.Add(Thread.CurrentThread);
                                GloBoardPro.HasTagListListThread.Distinct();
                                Thread.CurrentThread.IsBackground = true;
                            }
                            catch
                            {
                            }
                            //if (stopScrapImageBool) return;
                            string startString = "<a href=\"/p/";
                            string endString = "\" class=\"mainimg\"";
                            string imageId = string.Empty;
                            string imageSrc = string.Empty;

                            if (!string.IsNullOrEmpty(pageLink))
                            {
                                pageSource = objInstagramUser.getHtmlfromUrl(new Uri(pageLink),"");
                            }

                            if (pageSource.Contains("<ul class=\"pager\">") && pageSource.Contains("rel=\"next\">"))
                            {
                                try
                                {
                                    pageLink = getBetween(pageSource, "<ul class=\"pager\">", "rel=\"next\">");
                                }
                                catch { }
                                if (!string.IsNullOrEmpty(pageLink))
                                {
                                    try
                                    {
                                        int len = pageLink.IndexOf("<a href=\"");
                                        len = len + ("<a href=\"").Length;
                                        pageLink = pageLink.Substring(len);
                                        pageLink = pageLink.Trim();
                                        pageLink = pageLink.TrimEnd(new char[] { '"' });
                                        pageLink = "http://websta.me/" + pageLink;
                                    }
                                    catch { }
                                    if (!string.IsNullOrEmpty(pageLink))
                                    {
                                        string response = string.Empty;
                                        try
                                        {
                                            response = objInstagramUser.getHtmlfromUrl(new Uri(pageLink),"");
                                        }
                                        catch { }
                                        if (!string.IsNullOrEmpty(response))
                                        {
                                            if (response.Contains("<div class=\"mainimg_wrapper\">"))
                                            {
                                                try
                                                {
                                                    string[] arr1 = Regex.Split(response, "<div class=\"mainimg_wrapper\">");
                                                    if (arr1.Length > 1)
                                                    {
                                                        arr1 = arr1.Skip(1).ToArray();
                                                        foreach (string items in arr1)
                                                        {
                                                            try
                                                            {
                                                                //if (stopScrapImageBool) return;
                                                                if (items.Contains("<a href=\"/p/"))
                                                                {
                                                                    int indexStart = items.IndexOf("<a href=\"/p/");
                                                                    string itemarrNow = items.Substring(indexStart);

                                                                    try
                                                                    {
                                                                        imageId = getBetween(itemarrNow, startString, endString).Replace("/", "");
                                                                    }
                                                                    catch { }
                                                                    if (!string.IsNullOrEmpty(imageId))
                                                                    {
                                                                        lstPhotoId.Add(imageId);
                                                                        lstPhotoId.Distinct();
                                                                        if (lstPhotoId.Count >= ClGlobul.NumberofSnapsVideosToComment)
                                                                        {
                                                                            return lstPhotoId;
                                                                        }

                                                                        //imageId = "http://websta.me"+imageId;
                                                                    }


                                                                    //counter++;

                                                                    //Addtologger("Image DownLoaded with ImageName  "+imageId+"_"+counter);
                                                                    if (lstPhotoId.Count >= ClGlobul.NumberofSnapsVideosToComment)
                                                                    {
                                                                        return lstPhotoId;
                                                                    }
                                                                }
                                                            }

                                                            catch { }
                                                        }
                                                        if (lstPhotoId.Count >= ClGlobul.NumberofSnapsVideosToComment)
                                                        {
                                                            return lstPhotoId;
                                                        }
                                                    }
                                                }
                                                catch { }

                                            }
                                        }
                                        else
                                        {

                                        }

                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        #endregion
                    }
                }
            }

            return lstPhotoId;
        }








        #region HashTagLogEvents_addToLogger

        void HashTagLogEvents_addToLogger(object sender, EventArgs e)
        {
            try
            {
                if (e is EventsArgs)
                {
                    EventsArgs eventArgs = e as EventsArgs;
                    AddToHashLoggerDAta(eventArgs.log);
                }
            }
            catch
            {
            }
        }

        #endregion



        //ADD TO Logger
        #region Logger
        private void AddToHashLoggerDAta(string log)
        {
            try
            {
                if (AddToHashLogger.InvokeRequired)
                {
                    AddToHashLogger.Invoke(new MethodInvoker(delegate
                    {
                        AddToHashLogger.Items.Add(log);
                        AddToHashLogger.SelectedIndex = AddToHashLogger.Items.Count - 1;
                    }));
                }
                else
                {
                    AddToHashLogger.Items.Add(log);
                    AddToHashLogger.SelectedIndex = AddToHashLogger.Items.Count - 1;
                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        private void btnHashComment_Click(object sender, EventArgs e)
        {
            try
            {
                //get the hash tags from selected file...
                using (OpenFileDialog ofdImageTag = new OpenFileDialog())
                {
                    ofdImageTag.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                    ofdImageTag.Filter = "Text Files (*.txt)|*.txt";
                    if (ofdImageTag.ShowDialog() == DialogResult.OK)
                    {
                        //Add File path in Text Box....
                        txtHashComment.Text = ofdImageTag.FileName;
                        //call Read Method file when we select a file
                        readImageTaghashCommentFile(ofdImageTag.FileName);

                    }
                }
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("***********************************************************************************************", GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => btnHashTagUpload :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("***********************************************************************************************", GramBoardProFileHelper.ErrorLogFilePathForComment);
            }
        }
        public void readImageTaghashCommentFile(string HashTagFilePath)
        {
            try
            {
                ClGlobul.HashTagForScrap.Clear();
                //Read Data From Selected File ....
                List<string> hashTaglist = GramBoardProFileHelper.ReadFile((string)HashTagFilePath);
                foreach (string itemHash in hashTaglist)
                {
                    if (itemHash.ElementAt(0) == '#')
                    {
                        string itemHashNew = itemHash;
                        //add Comment Id's In Globol Comment Id List ...
                        ClGlobul.HashComment.Add(itemHashNew);
                    }
                    else
                    {
                        AddToHashLoggerDAta("[ " + DateTime.Now + " ] => [ " + itemHash + " is not Uploaded as Its not starting with # ]");
                    }
                }
                ClGlobul.HashComment = ClGlobul.HashComment.Distinct().ToList();
                //Show No Of Data Count In logger...
                AddToHashLoggerDAta("[ " + DateTime.Now + " ] => [ " + ClGlobul.HashComment.Count + " Hashtags Uploaded to comment. ]");
            }
            catch (Exception ex)
            {
                //when its generate any Error From here which is write in Text file ...
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => readHashTagFile :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
            }
        }

        private void btnHashCommentMessage_Click(object sender, EventArgs e)
        {
            try
            {
                //get the hash tags from selected file...
                using (OpenFileDialog ofdImageTag = new OpenFileDialog())
                {
                    ofdImageTag.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                    ofdImageTag.Filter = "Text Files (*.txt)|*.txt";
                    if (ofdImageTag.ShowDialog() == DialogResult.OK)
                    {

                        //Add File path in Text Box....
                        txtHashCommentMessage.Text = ofdImageTag.FileName;
                        //call Read Method file when we select a file
                        readImageTaghashCommentMessageFile(ofdImageTag.FileName);

                    }
                }
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("***********************************************************************************************", GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => btnHashTagUpload :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("***********************************************************************************************", GramBoardProFileHelper.ErrorLogFilePathForComment);
            }
        }

        public void readImageTaghashCommentMessageFile(string HashTagFilePath)
        {
            try
            {
                ClGlobul.HashTagForScrap.Clear();
                //Read Data From Selected File ....
                List<string> hashTaglist = GramBoardProFileHelper.ReadFile((string)HashTagFilePath);
                foreach (string itemHash in hashTaglist)
                {
                    //if (itemHash.ElementAt(0) == '#')
                    //{
                       // string itemHashNew = itemHash.Replace("#", "");
                        //add Comment Id's In Globol Comment Id List ...
                    ClGlobul.HashCommentMessage.Add(itemHash);
                   // }
                    //else
                    //{
                    //    AddToHashLoggerDAta("[ " + DateTime.Now + " ] => [ " + itemHash + " is not uploaded as Its not starting with # ]");
                    //}

                }
                ClGlobul.HashCommentMessage = ClGlobul.HashCommentMessage.Distinct().ToList();
                //Show No Of Data Count In logger...
                AddToHashLoggerDAta("[ " + DateTime.Now + " ] => [ " + ClGlobul.HashCommentMessage.Count + " Comment messages uploaded. ]");
            }
            catch (Exception ex)
            {
                //when its generate any Error From here which is write in Text file ...
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => readHashTagFile :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForComment);
            }
        }

        private void txtUnfollowSingle_TextChanged(object sender, EventArgs e)
        {
            ClGlobul.isUnfollowSingle = true;
            TextUnfollower.Enabled = false;
            ClGlobul.lstUnfollowerList.Clear();
        }


        #region AddToScrapeFollowersLogger
        private void AddToScrapeFollowersLogger(string log)
        {
            try
            {
                if (lstBoxLoggerGetFollowers.InvokeRequired)
                {
                    lstBoxLoggerGetFollowers.Invoke(new MethodInvoker(delegate
                    {
                        lstBoxLoggerGetFollowers.Items.Add(log);
                        lstBoxLoggerGetFollowers.SelectedIndex = lstBoxLoggerGetFollowers.Items.Count - 1;
                    }));
                }
                else
                {
                    lstBoxLoggerGetFollowers.Items.Add(log);
                    lstBoxLoggerGetFollowers.SelectedIndex = lstBoxLoggerGetFollowers.Items.Count - 1;
                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion
        private void btnUploadScrapeFollowers_Click(object sender, EventArgs e)
        {
            try
            {
                //get the hash tags from selected file...
                using (OpenFileDialog ofdImageTag = new OpenFileDialog())
                {
                    ofdImageTag.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                    ofdImageTag.Filter = "Text Files (*.txt)|*.txt";
                    if (ofdImageTag.ShowDialog() == DialogResult.OK)
                    {
                        //Add File path in Text Box....
                        txtUsernameScrapeFollowers.Text = ofdImageTag.FileName;
                        //call Read Method file when we select a file
                        ReadScrapeFollowersUsernameFile(ofdImageTag.FileName);

                    }
                }
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("***********************************************************************************************", GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => btnHashTagUpload :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForComment);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("***********************************************************************************************", GramBoardProFileHelper.ErrorLogFilePathForComment);
            }
        }

        public void ReadScrapeFollowersUsernameFile(string HashTagFilePath)
        {
            try
            {
                ClGlobul.listUsernameScrapeFollowers.Clear();
                //Read Data From Selected File ....
                List<string> usernameList = GramBoardProFileHelper.ReadFile((string)HashTagFilePath);
                foreach (string itemHash in usernameList)
                {
                    //add Comment Id's In Globol Comment Id List ...
                    ClGlobul.listUsernameScrapeFollowers.Add(itemHash);
                }
                ClGlobul.listUsernameScrapeFollowers = ClGlobul.listUsernameScrapeFollowers.Distinct().ToList();
                //Show No Of Data Count In logger...
                AddToScrapeFollowersLogger("[ " + DateTime.Now + " ] => [ " + ClGlobul.listUsernameScrapeFollowers.Count + " usernames uploaded. ]");
            }
            catch (Exception ex)
            {
                //when its generate any Error From here which is write in Text file ...
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => ReadScrapeFollowersUsernameFile :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
            }
        }

        int accountIndex = 0;
        private void btnStartScrapeFollowers_Click(object sender, EventArgs e)
        {

            Thread newThread = new Thread(StartProcessScrapeFollower);
            newThread.Start();
        }

        private void StartProcessScrapeFollower()
        {
            try
            {

                AddToScrapeFollowersLogger("[ " + DateTime.Now + " ] => [Pleasewait ]");
                List<List<string>> list_Accounts = new List<List<string>>();

                acc_checker_counter = DemoStagramPro.ClGlobul.accountList.Count();
                list_Accounts = ListUtilities.Split(DemoStagramPro.ClGlobul.accountList, Maxthread);
                ThreadPool.SetMaxThreads(5, 5);
                ClGlobul.listUsernameScrapeFollowers.Add(txtUsernameScrapeFollowers.Text);
                //GlobusHttpHelper httphelper = new GlobusHttpHelper();


                if (!string.IsNullOrEmpty(txtUsernameScrapeFollowers.Text))
                {
                    if (ClGlobul.listUsernameScrapeFollowers.Count != 0)
                    {
                        if (!ClGlobul.isStopScrapeFollowers)
                        {

                        LoopBackAccount:

                            string proxyadd = string.Empty;
                            string proxyport = string.Empty;
                            string proxyUser = string.Empty;
                            string proxyPass = string.Empty;


                            List<string> list_account = list_Accounts[ClGlobul.accountIndexForLoopingBack];


                            foreach (string val in list_account)
                            {
                                cmbScraperAccounts.Invoke(new MethodInvoker(delegate
                                {
                                    cmbScraperAccounts.SelectedIndex = accountIndex;
                                }));

                                string[] StrArrar = Regex.Split(val, ":");
                                string account = StrArrar[0];
                                string pass = StrArrar[1];
                                if (StrArrar.Count() == 4)
                                {
                                    proxyadd = StrArrar[2];
                                    proxyport = StrArrar[3];
                                }
                                if (StrArrar.Count() == 6)
                                {
                                    proxyUser = StrArrar[4];
                                    proxyPass = StrArrar[5];
                                }

                                InstagramManager.Classes.InstagramAccountManager InstagramAccountManager = new InstagramManager.Classes.InstagramAccountManager(account, pass, proxyadd, proxyport, proxyUser, proxyPass);

                                string status = InstagramAccountManager.LoginNew(ref InstagramAccountManager.httpHelper);


                                if (!status.Contains("Success"))
                                {

                                    status = InstagramAccountManager.LoginNew(ref InstagramAccountManager.httpHelper);
                                }
                                if (status == "Success")
                                {
                                    AddToScrapeFollowersLogger("[ " + DateTime.Now + "] " + "[ Logged in with account : " + InstagramAccountManager.Username + " ]");


                                    foreach (string username in ClGlobul.listUsernameScrapeFollowers)
                                    {
                                        ClGlobul.scrapeFollowerAndFollowingUsername = username;

                                        //new Thread(() => StartScrapingFollowers(ref InstagramAccountManager, username)).Start();

                                        Thread StartThread = new Thread(() => StartScrapingFollowers(ref InstagramAccountManager, username));

                                        StartThread.Start();

                                        StartThread.Join();
                                    }
                                }
                                else
                                {
                                    AddToScrapeFollowersLogger("[ " + DateTime.Now + "] " + "[ Not logged in with account : " + InstagramAccountManager.Username + " ]");
                                    return;
                                }


                                accountIndex++;
                            }


                            if (!ClGlobul.isStopScrapeFollowers)
                            {
                                if (!ClGlobul.userOverFollower)
                                {
                                    if (accountIndex >= list_Accounts.Count())
                                    {
                                        Thread.Sleep(5000);
                                        AddToScrapeFollowersLogger("[ " + DateTime.Now + " ] => [ All accounts used. Looping Back to the first account.]");
                                        AddToScrapeFollowersLogger("[ " + DateTime.Now + " ] => [ Delay of 5 seconds. ]");
                                        ClGlobul.accountIndexForLoopingBack = 0;
                                        accountIndex = 0;
                                        goto LoopBackAccount;
                                    }
                                    else
                                    {
                                        if (ClGlobul.switchAccount)
                                        {
                                            Thread.Sleep(5000);
                                            AddToScrapeFollowersLogger("[ " + DateTime.Now + " ] => [ Delay of 5 seconds. ]");
                                            ClGlobul.accountIndexForLoopingBack++;
                                            goto LoopBackAccount;
                                        }
                                    }
                                }
                            }

                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please write username.");
                        AddToScrapeFollowersLogger("[ " + DateTime.Now + " ] => [ Please write username. ]");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Please write username.");
                    AddToScrapeFollowersLogger("[ " + DateTime.Now + " ] => [ Please write username. ]");
                    return;
                }
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Method Name => btnStartScrapeFollowers_Click :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
            }
        }
        public string deletePreviousTimeAfterAnHour()
        {
            string ret = "not deleted";
            try
            {
                string time = string.Empty;
                DateTime timeNow = new DateTime();
                DateTime fetchedTime = new DateTime();
                timeNow = DateTime.Now;
                DataSet DS = DataBaseHandler.SelectQuery("select time from manage_time where process_status='" + "yes'", "manage_time");
                if (DS.Tables[0].Rows.Count != 0)
                {
                    time = DS.Tables[0].Rows[0]["time"].ToString();
                }
                fetchedTime = Convert.ToDateTime(time);

                TimeSpan timeDifference = timeNow.Subtract(fetchedTime);
                if (timeDifference.Hours > 0)
                {
                    DataBaseHandler.DeleteQuery("delete from manage_time", "manage_time");
                    ret = "deleted";
                }
                else
                {
                    MessageBox.Show("Please wait for another " + (60 - timeDifference.Minutes) + " minutes before you resume your process.");
                }
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => deletePreviousTimeAfterAnHour :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
            }
            return ret;
        }

        public string CheckHourlyLimit()
        {
            string ret = "limit over";
            try
            {
                DataSet DS = DataBaseHandler.SelectQuery("select time from manage_time", "manage_time");
                if (DS.Tables[0].Rows.Count == 0)
                {
                    return ret;
                }
                else
                {
                    string time = DS.Tables[0].Rows[0]["time"].ToString();
                    DateTime yourDate = new DateTime();
                    yourDate = DateTime.Now;
                    DateTime fetchedTime = Convert.ToDateTime(time);

                    TimeSpan dt_Difference = yourDate.Subtract(fetchedTime);
                    if (dt_Difference.Hours < 1)
                    {
                        MessageBox.Show("Wait for another " + (60 - dt_Difference.Minutes) + " minutes before you start the process again.");
                        ret = "wait";
                        return ret;
                    }
                }
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => CheckHourlyLimit :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
            }
            return ret;
        }

        public string GetNewWorkingProxy()
        {
            string fetchedProxy = string.Empty;
            List<string> proxies = new List<string>();
            try
            {
                proxies.Add("60.169.78.218:808");
                proxies.Add("125.39.66.69:80");
                proxies.Add("121.10.252.139:3128");
                proxies.Add("218.90.174.167:3128");
                proxies.Add("183.230.53.92:8123");
                proxies.Add("218.4.236.117:80");

                fetchedProxy = proxies[0].ToString();
                proxies.Remove(fetchedProxy);


            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => GetNewWorkingProxy :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
            }
            return fetchedProxy;
        }


        public void StartScrapingFollowers(ref InstagramAccountManager accountManager, string username)
        {
            try
            {
                ClGlobul.switchAccount = false;

                if (!ClGlobul.isStopScrapeFollowers)
                {
                    Thread.CurrentThread.IsBackground = true;
                    ClGlobul.lstThreadsScrapeFollowers.Add(Thread.CurrentThread);
                    ClGlobul.lstThreadsScrapeFollowers = ClGlobul.lstThreadsScrapeFollowers.Distinct().ToList();
                }
                else
                {
                    return;
                }
                string usernameUrl = "http://websta.me/n/" + username;
                string usernamePgSource = accountManager.httpHelper.getHtmlfromUrl(new Uri(usernameUrl), "");
                string rawUsernameID = getBetween(usernamePgSource, "<div class=\"userinfo\">", "</div>");
                string[] userInfoSplit = Regex.Split(rawUsernameID, "<li>");

                string rawFollowerUrl = getBetween(userInfoSplit[2], "<a href=\"", "\">");
                string rawFollowingUrl = getBetween(userInfoSplit[3], "<a href=\"", "\">");


                string usernameFollowerUrl = "http://websta.me" + rawFollowerUrl;
                string usernameFollowingUrl = "http://websta.me" + rawFollowingUrl;

                string usernameFollowerCount = getBetween(userInfoSplit[2], "class=\"counts_followed_by\">", "</span>");
                string usernameFollowingCount = getBetween(userInfoSplit[3], "class=\"following\">", "</span>");


                AddToScrapeFollowersLogger("[ " + DateTime.Now + "] " + "[ Scraping Followers ]");
                ScrapeFollowerUrl(ref accountManager, usernameFollowerUrl, username);


                //AddToScrapeFollowersLogger("[ " + DateTime.Now + "] " + "[ Scraping Following ]");
                //ScrapeFollowingUrl(ref accountManager, usernameFollowingUrl, username);

                #region ExportData
                //if (!ClGlobul.isStopScrapeFollowers)
                //{
                //    string filePath = GramBoardProFileHelper.path_AppDataFolder + "\\" + username;
                //    ExportData(filePath, username, "Follower");
                //    //MessageBox.Show("Follower Data Exported to CSV file with username : " + username + ".");
                //    AddToScrapeFollowersLogger("[ " + DateTime.Now + " ] => [ Follower Data Exported to CSV file with username : " + username + " ]");
                //}

                #endregion

                #region LoadCombobox
                DataSet DS = DataBaseHandler.SelectQuery("select distinct username from tb_scrape_follower", "tb_scrape_follower");
                if (DS.Tables[0].Rows.Count != 0)
                {
                    foreach (DataRow DR in DS.Tables[0].Rows)
                    {
                        cmbScrapeFollowersUsername.Invoke(new MethodInvoker(delegate
                        {
                            if (!cmbScrapeFollowersUsername.Items.Contains(DR[0].ToString()))
                            {
                                cmbScrapeFollowersUsername.Items.Add(DR[0].ToString());
                            }
                        }));
                    }
                }
                else
                {
                    MessageBox.Show("No data present. Scrape first to choose from usernames.");
                }

                cmbScrapeFollowersSelection.Invoke(new MethodInvoker(delegate
                {
                    if (!cmbScrapeFollowersSelection.Items.Contains("Follower"))
                    {
                        cmbScrapeFollowersSelection.Items.Add("Follower");
                    }
                    if (!cmbScrapeFollowersSelection.Items.Contains("Following"))
                    {
                        cmbScrapeFollowersSelection.Items.Add("Following");
                    }
                }));
                cmbScrapeFollowersUsername.Invoke(new MethodInvoker(delegate
                {
                    cmbScrapeFollowersUsername.SelectedIndex = 0;
                }));
                cmbScrapeFollowersSelection.Invoke(new MethodInvoker(delegate
                {
                    cmbScrapeFollowersSelection.SelectedIndex = 0;
                }));
                #endregion

            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => StartScrapingFollowers :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
            }
            finally
            {
                if (!ClGlobul.isStopScrapeFollowers)
                {
                    DataBaseHandler.UpdateQuery("update manage_time set process_status='yes' where process_status='no'", "manage_time");
                    AddToScrapeFollowersLogger("[ " + DateTime.Now + "] " + "[ Switching onto next account. ]");
                    ClGlobul.switchAccount = true;

                }
                if (ClGlobul.userOverFollower)
                {
                    AddToScrapeFollowersLogger("[ " + DateTime.Now + "] => " + "[ Scraped all users who follows " + username + " ]");
                }
                if (ClGlobul.userOverFollowing)
                {
                    AddToScrapeFollowersLogger("[ " + DateTime.Now + "] => " + "[ Scraped all users who is followed by " + username + " ]");
                }
                ClGlobul.oneHourProcessCompleted = true;
            }
        }
        private void btnStopScrapeFollowers_Click(object sender, EventArgs e)
        {
            try
            {
                ClGlobul.isStopScrapeFollowers = true;
                List<Thread> tempList = new List<Thread>();
                tempList = ClGlobul.lstThreadsScrapeFollowers.Distinct().ToList();
                stopThreadScrapeFollwers(tempList);
                AddToScrapeFollowersLogger("[ " + DateTime.Now + " ] => [ Process stopped. ]");
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => btnStopScrapeFollowers_Click :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
            }
        }

        public void stopThreadScrapeFollwers(List<Thread> tempList)
        {
            try
            {
                foreach (Thread item in tempList)
                {
                    item.Abort();
                }
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => stopThreadScrapeFollwers :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
            }
        }
        public void ScrapeFollowerUrl(ref InstagramAccountManager accountManager, string usernameFollowerUrl, string username)
        {

            if (!ClGlobul.isStopScrapeFollowers)
            {
                Thread.CurrentThread.IsBackground = true;
                ClGlobul.lstThreadsScrapeFollowers.Add(Thread.CurrentThread);
                ClGlobul.lstThreadsScrapeFollowers = ClGlobul.lstThreadsScrapeFollowers.Distinct().ToList();
            }
            else
            {
                return;
            }

            List<string> listFollowers = new List<string>();
            try
            {
                string followerListPgSource = string.Empty;
                DataSet DS = DataBaseHandler.SelectQuery("select url from tb_follower_url where username ='" + username + "' and used='no'", "tb_follower_url");
                if (DS.Tables[0].Rows.Count == 0)
                {
                    followerListPgSource = accountManager.httpHelper.getHtmlfromUrl(new Uri(usernameFollowerUrl),"");
                }
                else
                {
                    usernameFollowerUrl = DS.Tables[0].Rows[0]["url"].ToString();
                    followerListPgSource = accountManager.httpHelper.getHtmlfromUrl(new Uri(usernameFollowerUrl),"");
                    DataBaseHandler.UpdateQuery("update tb_follower_url set used='yes' where used='no'", "tb_follower_url");
                }

                string rawFollowerList = getBetween(followerListPgSource, "<ul class=\"userlist\">", "</ul>");
                string[] followerListSplit = Regex.Split(rawFollowerList, "<li");
                foreach (string item in followerListSplit)
                {
                    if (item.Contains("<strong><a href"))
                    {
                        string followerName = getBetween(item, "<strong><a href=\"/n/", "\">");
                        listFollowers.Add(followerName);
                        AddToScrapeFollowersLogger("[ " + DateTime.Now + "] => " + "[ Follower Url added to scrape-" + followerName + " ]");
                    }
                }

                if (followerListPgSource.Contains("Next Page"))
                {
                    string rawPagination = string.Empty;
                    if (followerListPgSource.Contains("<ul class=\"pager\">"))
                    {
                        rawPagination = getBetween(followerListPgSource, "<ul class=\"pager\">", "Next Page");
                    }
                    else
                    {
                        rawPagination = getBetween(followerListPgSource, "<ul class=\"pager nm\">", "Next Page");
                    }

                    string[] paginationUrlSplit = Regex.Split(rawPagination, "<li>");
                    string rawPaginationUrl = getBetween(paginationUrlSplit[2], "<a href=\"", "\">");
                    string paginationUrl = "http://websta.me" + rawPaginationUrl;


                    followerListPgSource = accountManager.httpHelper.getHtmlfromUrl(new Uri(paginationUrl),"");
                    rawFollowerList = getBetween(followerListPgSource, "<ul class=\"userlist\">", "</ul>");
                    followerListSplit = Regex.Split(rawFollowerList, "<li");
                    foreach (string item in followerListSplit)
                    {
                        if (item.Contains("<strong><a href"))
                        {
                            string followerName = getBetween(item, "<strong><a href=\"/n/", "\">");
                            listFollowers.Add(followerName);
                            AddToScrapeFollowersLogger("[ " + DateTime.Now + "] => " + "[ Follower Url added to scrape-" + followerName + " ]");
                        }
                    }

                    if (followerListPgSource.Contains("Next Page"))
                    {
                        rawPagination = string.Empty;
                        if (followerListPgSource.Contains("<ul class=\"pager\">"))
                        {
                            rawPagination = getBetween(followerListPgSource, "<ul class=\"pager\">", "Next Page");
                        }
                        else
                        {
                            rawPagination = getBetween(followerListPgSource, "<ul class=\"pager nm\">", "Next Page");
                        }

                        paginationUrlSplit = Regex.Split(rawPagination, "<li>");
                        rawPaginationUrl = getBetween(paginationUrlSplit[2], "<a href=\"", "\">");
                        paginationUrl = "http://websta.me" + rawPaginationUrl;

                        DataBaseHandler.InsertQuery("insert into tb_follower_url (url, username, used) values ('" + paginationUrl + "','" + username + "','" + "no" + "')", "tb_follower_url");


                    }
                    else
                    {
                        ClGlobul.userOverFollower = true;
                    }

                }
                else
                {
                    ClGlobul.userOverFollower = true;
                }

                #region Commented-Pagination

                #endregion

                listFollowers = listFollowers.Distinct().ToList();


                foreach (string followerName in listFollowers)
                {
                    string followerUrl = "http://websta.me/n/" + followerName;
                    ScrapeFollowerDetails(ref accountManager, followerUrl, usernameFollowerUrl);

                }


            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Method Name => ScrapeFollowerUrl :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
            }
        }
        public void ScrapeFollowingUrl(ref InstagramAccountManager accountManager, string usernameFollowingUrl, string username)
        {
            List<string> listFollowing = new List<string>();
            if (!ClGlobul.isStopScrapeFollowers)
            {
                Thread.CurrentThread.IsBackground = true;
                ClGlobul.lstThreadsScrapeFollowers.Add(Thread.CurrentThread);
                ClGlobul.lstThreadsScrapeFollowers = ClGlobul.lstThreadsScrapeFollowers.Distinct().ToList();
            }
            else
            {
                return;
            }
            try
            {
                string followingListPgSource = string.Empty;
                DataSet DS = DataBaseHandler.SelectQuery("select url from tb_following_url where username='" + username + "' and used='no'", "tb_following_url");
                if (DS.Tables[0].Rows.Count == 0)
                {
                    followingListPgSource = accountManager.httpHelper.getHtmlfromUrl(new Uri(usernameFollowingUrl),"");
                }
                else
                {
                    usernameFollowingUrl = DS.Tables[0].Rows[0]["url"].ToString();
                    followingListPgSource = accountManager.httpHelper.getHtmlfromUrl(new Uri(usernameFollowingUrl),"");
                    DataBaseHandler.UpdateQuery("update tb_following_url set used='yes' where used='no'", "tb_following_url");
                }

                followingListPgSource = accountManager.httpHelper.getHtmlfromUrl(new Uri(usernameFollowingUrl),"");
                string rawFollowingList = getBetween(followingListPgSource, "<ul class=\"userlist\">", "</ul>");
                string[] followingListSplit = Regex.Split(rawFollowingList, "<li");
                foreach (string item in followingListSplit)
                {
                    if (item.Contains("<strong><a href"))
                    {
                        string followingName = getBetween(item, "<strong><a href=\"/n/", "\">");
                        listFollowing.Add(followingName);
                        AddToScrapeFollowersLogger("[ " + DateTime.Now + "] => " + "[ Following Url added to scrape-" + followingName + " ]");
                    }
                }

                if (followingListPgSource.Contains("Next Page"))
                {
                    string rawPagination = string.Empty;
                    if (followingListPgSource.Contains("<ul class=\"pager\">"))
                    {
                        rawPagination = getBetween(followingListPgSource, "<ul class=\"pager\">", "Next Page");
                    }
                    else
                    {
                        rawPagination = getBetween(followingListPgSource, "<ul class=\"pager nm\">", "Next Page");
                    }

                    string[] paginationUrlSplit = Regex.Split(rawPagination, "<li>");
                    string rawPaginationUrl = getBetween(paginationUrlSplit[2], "<a href=\"", "\">");
                    string paginationUrl = "http://websta.me" + rawPaginationUrl;

                    DataBaseHandler.InsertQuery("insert into tb_following_url (url, username, used) values ('" + paginationUrl + "','" + username + "','" + "no" + "')", "tb_following_url");
                }
                else
                {
                    ClGlobul.userOverFollowing = true;
                }


                listFollowing = listFollowing.Distinct().ToList();


                foreach (string followingName in listFollowing)
                {
                    string followingUrl = "http://websta.me/n/" + followingName;
                    ScrapeFollowingDetails(ref accountManager, followingUrl);
                }
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => ScrapeFollowingUrl :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
            }
        }

         private const string CSVHeader = "Username, Name, Follower count, Following count, Picture count, Day, Month, Year";
        public void ScrapeFollowerDetails(ref InstagramAccountManager accountManager, string followerUrl, string usernameFollowerUrl)
        {
            try
            {

                if (!ClGlobul.isStopScrapeFollowers)
                {
                    Thread.CurrentThread.IsBackground = true;
                    ClGlobul.lstThreadsScrapeFollowers.Add(Thread.CurrentThread);
                    ClGlobul.lstThreadsScrapeFollowers = ClGlobul.lstThreadsScrapeFollowers.Distinct().ToList();
                }
                else
                {
                    return;
                }


                Thread.Sleep(1000);

                string followerPageSource = accountManager.httpHelper.getHtmlfromUrl1(new Uri(followerUrl), usernameFollowerUrl);
                if (string.IsNullOrEmpty(followerPageSource))
                {
                    Thread.Sleep(10000);
                    AddToScrapeFollowersLogger("[ " + DateTime.Now + "] => " + "[ Pagesource is empty. Delaying for 10 seconds. ]");
                    followerPageSource = accountManager.httpHelper.getHtmlfromUrl1(new Uri(followerUrl), usernameFollowerUrl);
                    if (string.IsNullOrEmpty(followerPageSource))
                    {
                        AddToScrapeFollowersLogger("[ " + DateTime.Now + "] => " + "[ Pagesource is still empty. Please restart the software. The process will resume. ]");
                    }
                }

                string followerUsername = getBetween(followerUrl + "@", ".me/n/", "@");

                if (followerPageSource.Contains("This user is private."))
                {
                    AddToScrapeFollowersLogger("[ " + DateTime.Now + "] => " + "[ Data can not be scraped of the  private user with user name : " + followerUsername + ". ]");
                    return;
                }

                string rawFollowerInfo = string.Empty;
                AddToScrapeFollowersLogger("[ " + DateTime.Now + "] => " + "[ Scraping detail from profile with username-" + followerUsername + " ]");
                try
                {
                     rawFollowerInfo = getBetween(followerPageSource, "<div class=\"userinfo\">", "</div>");
                }
                catch
                { }
               
                string[] infoSplit = Regex.Split(rawFollowerInfo, "<li>");

                string subFollowerCount = getBetween(infoSplit[2], "<span class=\"counts_followed_by\">", "</span>");
                string subFollowingCount = getBetween(infoSplit[3], "<span class=\"following\">", "</span>");


                string countAndLatestPostUrl = GetPictureCountAndLatestSnapUrlfollower(ref accountManager, followerPageSource);
                if (countAndLatestPostUrl == "stop")
                {
                    return;
                }
                string[] count_SPlit_Url = Regex.Split(countAndLatestPostUrl, "splitHere");
                string latestPostUrl = count_SPlit_Url[0].Trim();

                string pictureCount = getBetween(followerPageSource, "\"counts_media\">", "</span>");



                string latestPostPageResponse = accountManager.httpHelper.getHtmlfromUrl(new Uri(latestPostUrl), followerUrl);
                double uTimestamp = Convert.ToDouble(getBetween(latestPostPageResponse, "data-utime=\"", "\">"));
                DateTime rawDate = UnixTimeStampToDateTime(uTimestamp);
                string date = rawDate.ToString("dd-MM-yyyy");
                string[] date_split = Regex.Split(date, "-");
                string day = date_split[0].ToString();
                string month = date_split[1].ToString();
                string year = date_split[2].ToString();


                DataBaseHandler.InsertQuery("insert into tb_scrape_follower(username, name, follower_count, following_count, picture_count, day, month, year) values('" + ClGlobul.scrapeFollowerAndFollowingUsername + "','" + followerUsername + "','" + subFollowerCount + "','" + subFollowingCount + "','" + pictureCount + "','" + day + "','" + month + "','" + year + "')", "tb_scrape_follower");
                AddToScrapeFollowersLogger("[ " + DateTime.Now + "] => " + "[ Scraped detail saved of username-" + followerUsername + " ]");

                 
                  if (!string.IsNullOrEmpty(followerUsername))
                  {
                      #region CSV Write
                      try
                      {
                          string CSVData = ClGlobul.scrapeFollowerAndFollowingUsername.Replace(",", string.Empty) + "," + followerUsername.Replace(",", string.Empty) + "," + subFollowerCount.Replace(",", string.Empty) + "," + subFollowingCount.Replace(",", string.Empty) + "," + pictureCount.Replace(",", string.Empty) + "," + day.Replace(",", string.Empty) + "," + month.Replace(",", string.Empty) + "," + year.Replace(",", string.Empty);
                          GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSVData, CSVPath + ClGlobul.scrapeFollowerAndFollowingUsername +".csv");
                      }
                      catch { }
                      try
                      {

                          AddToScrapeFollowersLogger("[" + followerUsername + "," + "followerUsername:" + "," + subFollowingCount + "," + "subFollowingCount:" + "," + pictureCount + "," + "pictureCount" + "," + day + "," + "day" + "," + month + "," + "month" + "," + year + "," + "year]");

                      }
                      catch { };

                      #endregion
                      if (!string.IsNullOrEmpty(txtminScrapuser.Text) && NumberHelper.ValidateNumber(txtminScrapuser.Text))
                      {
                          mindelay = Convert.ToInt32(txtminScrapuser.Text);
                      }
                      if (!string.IsNullOrEmpty(txtmaxscrapuser.Text) && NumberHelper.ValidateNumber(txtmaxscrapuser.Text))
                      {
                          maxdelay = Convert.ToInt32(txtmaxscrapuser.Text);
                      }

                      lock (_lockObject)
                      {
                          int delay = RandomNumberGenerator.GenerateRandom(mindelay, maxdelay);
                          AddToScrapeFollowersLogger("[ " + DateTime.Now + " ] => [ Delay For " + delay + " Seconds ]");
                          Thread.Sleep(delay * 1000);
                      }

                  }
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => ScrapeFollowerDetails :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
            }
        }
        public string GetPictureCountAndLatestSnapUrlfollower(ref InstagramAccountManager accountManager, string followerPageSource)
        {
            string retCountAndUrl = string.Empty;
            try
            {
                if (!ClGlobul.isStopScrapeFollowers)
                {
                    Thread.CurrentThread.IsBackground = true;
                    ClGlobul.lstThreadsScrapeFollowers.Add(Thread.CurrentThread);
                    ClGlobul.lstThreadsScrapeFollowers = ClGlobul.lstThreadsScrapeFollowers.Distinct().ToList();
                }
                else
                {
                    retCountAndUrl = "stop";
                    return retCountAndUrl;
                }
                bool enterOnce = false;
                string rawLatestPostUrl = string.Empty;
                string latestPostUrl = string.Empty;
                int pictureCount = 0;
                string[] Picture_Split = Regex.Split(followerPageSource, "<div class=\"mainimg_wrapper\">");
                foreach (string picture in Picture_Split)
                {
                    if (!picture.Contains("<!DOCTYPE html>"))
                    {
                        if (!picture.Contains("fancy-video"))
                        {
                            if (!enterOnce)
                            {
                                rawLatestPostUrl = getBetween(picture, "<a href=\"", "\"");
                                latestPostUrl = "http://websta.me" + rawLatestPostUrl;
                                enterOnce = true;
                            }
                            pictureCount++;
                        }
                    }
                }

                string snapCount = pictureCount.ToString();
                retCountAndUrl = latestPostUrl + "splitHere" + snapCount;
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => GetPictureCountAndLatestSnapUrl :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
            }
            return retCountAndUrl;
        }

        public string GetPictureCountAndLatestSnapUrlfollowing(ref InstagramAccountManager accountManager, string followingPageSource)
        {
            string retCountAndUrl = string.Empty;
            try
            {
                if (!ClGlobul.isStopScrapeFollowers)
                {
                    Thread.CurrentThread.IsBackground = true;
                    ClGlobul.lstThreadsScrapeFollowers.Add(Thread.CurrentThread);
                    ClGlobul.lstThreadsScrapeFollowers = ClGlobul.lstThreadsScrapeFollowers.Distinct().ToList();
                }
                else
                {
                    retCountAndUrl = "stop";
                    return retCountAndUrl;
                }
                bool enterOnce = false;
                string rawLatestPostUrl = string.Empty;
                string latestPostUrl = string.Empty;
                int pictureCount = 0;
                string[] Picture_Split = Regex.Split(followingPageSource, "<div class=\"mainimg_wrapper\">");
                foreach (string picture in Picture_Split)
                {
                    if (!picture.Contains("<!DOCTYPE html>"))
                    {
                        if (!picture.Contains("fancy-video"))
                        {
                            if (!enterOnce)
                            {
                                rawLatestPostUrl = getBetween(picture, "<a href=\"", "\"");
                                latestPostUrl = "http://websta.me" + rawLatestPostUrl;
                                enterOnce = true;
                            }
                            pictureCount++;
                        }
                    }
                }

                string snapCount = pictureCount.ToString();
                retCountAndUrl = latestPostUrl + "splitHere" + snapCount;
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => GetPictureCountAndLatestSnapUrl :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
            }
            return retCountAndUrl;
        }

        public void ScrapeFollowingDetails(ref InstagramAccountManager accountManager, string followingUrl)
        {
            try
            {
                if (!ClGlobul.isStopScrapeFollowers)
                {
                    Thread.CurrentThread.IsBackground = true;
                    ClGlobul.lstThreadsScrapeFollowers.Add(Thread.CurrentThread);
                    ClGlobul.lstThreadsScrapeFollowers = ClGlobul.lstThreadsScrapeFollowers.Distinct().ToList();
                }
                else
                {
                    return;
                }

                Thread.Sleep(1000);
                string followingPageSource = accountManager.httpHelper.getHtmlfromUrl(new Uri(followingUrl),"");
                string followingUsername = getBetween(followingUrl + "@", ".me/n/", "@");
                AddToScrapeFollowersLogger("[ " + DateTime.Now + "] " + "[ Scraping detail from profile with username-" + followingUsername + " ]");
                string rawFollowingInfo = getBetween(followingPageSource, "<div class=\"userinfo\">", "</div>");
                string[] infoSplit = Regex.Split(rawFollowingInfo, "<li>");

                string subFollowerCount = getBetween(infoSplit[2], "<span class=\"counts_followed_by\">", "</span>");
                string subFollowingCount = getBetween(infoSplit[3], "<span class=\"following\">", "</span>");

                string countAndLatestPostUrl = GetPictureCountAndLatestSnapUrlfollowing(ref accountManager, followingPageSource);
                if (countAndLatestPostUrl == "stop")
                {
                    return;
                }
                string[] count_SPlit_Url = Regex.Split(countAndLatestPostUrl, "splitHere");
                string latestPostUrl = count_SPlit_Url[0].Trim();
                //string pictureCount = count_SPlit_Url[1].Trim();
                string pictureCount = getBetween(followingPageSource, "\"counts_media\">", "</span>");

                string latestPostPageResponse = accountManager.httpHelper.getHtmlfromUrl(new Uri(latestPostUrl),"");
                double uTimestamp = Convert.ToDouble(getBetween(latestPostPageResponse, "data-utime=\"", "\">"));
                DateTime rawDate = UnixTimeStampToDateTime(uTimestamp);
                string date = rawDate.ToString("dd-MM-yyyy");
                string[] date_split = Regex.Split(date, "-");
                string day = date_split[0].ToString();
                string month = date_split[1].ToString();
                string year = date_split[2].ToString();

                DataBaseHandler.InsertQuery("insert into tb_scrape_following(username, name, follower_count, following_count, picture_count, day, month, year) values('" + ClGlobul.scrapeFollowerAndFollowingUsername + "','" + followingUsername + "','" + subFollowerCount + "','" + subFollowingCount + "','" + pictureCount + "','" + day + "','" + month + "','" + year + "')", "tb_scrape_following");
                AddToScrapeFollowersLogger("[ " + DateTime.Now + "] " + "[ Scraped detail saved of username-" + followingUsername + " ]");

            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => ScrapeFollowingDetails :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
            }
        }

        #region Convert Unix Timestamp to DateTime
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
        #endregion

        #region GetBetween
        public string getBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
            }
        }
        #endregion

        private void cmbScrapeFollowersUsername_DropDown(object sender, EventArgs e)
        {
            DataSet DS = DataBaseHandler.SelectQuery("select distinct username from tb_scrape_follower", "tb_scrape_follower");
            if (DS.Tables[0].Rows.Count != null)
            {
                foreach (DataRow DR in DS.Tables[0].Rows)
                {
                    if (!cmbScrapeFollowersUsername.Items.Contains(DR[0].ToString()))
                    {
                        cmbScrapeFollowersUsername.Items.Add(DR[0].ToString());
                    }
                }
            }
            else
            {
                MessageBox.Show("No data present. Scrape first to choose from usernames.");
            }
        }

        private void cmbScrapeFollowersSelection_DropDown(object sender, EventArgs e)
        {
        }

        private void btnRefreshScrapeFollowers_Click(object sender, EventArgs e)
        {
            try
            {
                string selectedUsername = cmbScrapeFollowersUsername.SelectedItem.ToString();
                if (!string.IsNullOrEmpty(selectedUsername))
                {
                    if (cmbScrapeFollowersSelection.SelectedIndex != -1)
                    {
                        string selectedSelection = cmbScrapeFollowersSelection.SelectedItem.ToString();
                        DataTable dt = UsernameFilter(selectedUsername, selectedSelection);
                        DataSet dataSet = new DataSet();
                        dataSet.Tables.Add(dt);
                        dgvFollowers.DataSource = dataSet.Tables[0];


                    }
                    else
                    {
                        AddToScrapeFollowersLogger("[ " + DateTime.Now + " ] => [Please select \"Follower\" or \"Following\" ]");
                    }
                }
                else
                {
                    AddToScrapeFollowersLogger("[ " + DateTime.Now + " ] => [ Please select username ]");
                }

            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => btnRefreshScrapeFollowers_Click :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
            }
        }

        private void cmbScrapeFollowersUsername_SelectionChangeCommitted(object sender, EventArgs e)
        {

            try
            {
                string selectedUsername = string.Empty;
                cmbScrapeFollowersSelection.Invoke(new MethodInvoker(delegate
                {
                    cmbScrapeFollowersSelection.SelectedIndex = 0;
                }));

                if (cmbScrapeFollowersUsername.SelectedIndex != -1)
                {
                    selectedUsername = cmbScrapeFollowersUsername.SelectedItem.ToString();
                }

                if (cmbScrapeFollowersSelection.SelectedIndex != -1)
                {
                    string selectedSelection = cmbScrapeFollowersSelection.SelectedItem.ToString();
                    DataTable dt = UsernameFilter(selectedUsername, selectedSelection);
                    DataSet dataSet = new DataSet();
                    dataSet.Tables.Add(dt);
                    dgvFollowers.Invoke(new MethodInvoker(delegate
                    {
                        dgvFollowers.DataSource = dataSet.Tables[0];
                    }));
                }

            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => cmbScrapeFollowersUsername_SelectionChangeCommitted :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
            }

        }

        public DataTable UsernameFilter(string selectedUsername, string selectedSelection)
        {
            DataGridView dataGridView = new DataGridView();
            DataTable dataGridTable = new DataTable();
            DataTable dataTable = new DataTable();

            int countFlwr = 0;
            int countFllwng = 0;
            //string selection = string.Empty;


            try
            {
                if (selectedSelection == "Follower")
                {
                    dataTable.Columns.Add("Id", typeof(string));
                    dataTable.Columns.Add("Username", typeof(string));
                    dataTable.Columns.Add("Follower", typeof(string));
                    dataTable.Columns.Add("Follower count", typeof(string));
                    dataTable.Columns.Add("Following count", typeof(string));
                    dataTable.Columns.Add("Snap count", typeof(string));
                    dataTable.Columns.Add("Day", typeof(string));
                    dataTable.Columns.Add("Month", typeof(string));
                    dataTable.Columns.Add("Year", typeof(string));
                    DataSet DS = DataBaseHandler.SelectQuery("select username, name, follower_count, following_count, picture_count, day, month, year from tb_scrape_follower where username = '" + selectedUsername + "'", "tb_scrape_follower");
                    foreach (DataRow DR in DS.Tables[0].Rows)
                    {
                        countFlwr++;
                        DataRow dRow = dataTable.NewRow();
                        string username = DR["username"].ToString();
                        string name = DR["name"].ToString();
                        string follower_count = DR["follower_count"].ToString();
                        string following_count = DR["following_count"].ToString();
                        string picture_count = DR["picture_count"].ToString();
                        string day = DR["day"].ToString();
                        string month = DR["month"].ToString();
                        string year = DR["year"].ToString();

                        dRow["Id"] = Convert.ToString(countFlwr);
                        dRow["Username"] = username;
                        dRow["Follower"] = name;
                        dRow["Follower count"] = follower_count;
                        dRow["Following count"] = following_count;
                        dRow["Snap count"] = picture_count;
                        dRow["Day"] = day;
                        dRow["Month"] = month;
                        dRow["Year"] = year;

                        dataTable.Rows.Add(dRow);
                    }
                }
                if (selectedSelection == "Following")
                {
                    dataTable.Columns.Add("Id", typeof(string));
                    dataTable.Columns.Add("Username", typeof(string));
                    dataTable.Columns.Add("Following", typeof(string));
                    dataTable.Columns.Add("Follower count", typeof(string));
                    dataTable.Columns.Add("Following count", typeof(string));
                    dataTable.Columns.Add("Snap count", typeof(string));
                    dataTable.Columns.Add("Day", typeof(string));
                    dataTable.Columns.Add("Month", typeof(string));
                    dataTable.Columns.Add("Year", typeof(string));
                    DataSet DS = DataBaseHandler.SelectQuery("select username, name, follower_count, following_count, picture_count, day, month, year from tb_scrape_following where username = '" + selectedUsername + "'", "tb_scrape_following");
                    foreach (DataRow DR in DS.Tables[0].Rows)
                    {
                        countFllwng++;
                        DataRow dRow = dataTable.NewRow();
                        string username = DR["username"].ToString();
                        string name = DR["name"].ToString();
                        string follower_count = DR["follower_count"].ToString();
                        string following_count = DR["following_count"].ToString();
                        string picture_count = DR["picture_count"].ToString();
                        string day = DR["day"].ToString();
                        string month = DR["month"].ToString();
                        string year = DR["year"].ToString();

                        dRow["Id"] = Convert.ToString(countFllwng);
                        dRow["Username"] = username;
                        dRow["Following"] = name;
                        dRow["Follower count"] = follower_count;
                        dRow["Following count"] = following_count;
                        dRow["Snap count"] = picture_count;
                        dRow["Day"] = day;
                        dRow["Month"] = month;
                        dRow["Year"] = year;

                        dataTable.Rows.Add(dRow);
                    }
                }
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => UsernameFilter :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
            }
            return dataTable;
        }

        private void cmbScrapeFollowersSelection_SelectionChangeCommitted(object sender, EventArgs e)
        {

            try
            {
                string selectedUsername = string.Empty;
                string selectedSelection = string.Empty;
                cmbScrapeFollowersUsername.Invoke(new MethodInvoker(delegate
                {
                    selectedUsername = cmbScrapeFollowersUsername.SelectedItem.ToString();
                }));
                cmbScrapeFollowersSelection.Invoke(new MethodInvoker(delegate
                {
                    selectedSelection = cmbScrapeFollowersSelection.SelectedItem.ToString();
                }));

                DataTable dt = UsernameFilter(selectedUsername, selectedSelection);
                DataSet dataSet = new DataSet();
                dataSet.Tables.Add(dt);
                dgvFollowers.Invoke(new MethodInvoker(delegate
                {
                    dgvFollowers.DataSource = dataSet.Tables[0];
                }));

            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => cmbScrapeFollowersSelection_SelectionChangeCommitted :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
            }

        }

        private void btnExportScrapeFollowers_Click(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => btnExportScrapeFollowers_Click :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
            }
        }

        public void ExportData(string filePath, string selectedUsername, string selection)
        {

            try
            {
                DataSet DS = new DataSet();
                if (selection == "Follower")
                {
                    filePath = filePath + "-Follower.CSV";
                    DS = DataBaseHandler.SelectQuery("select username, name, follower_count, following_count, picture_count, day, month, year from tb_scrape_follower where username = '" + selectedUsername + "'", "tb_scrape_follower");
                }
                if (selection == "Following")
                {
                    filePath = filePath + "-Following.CSV";
                    DS = DataBaseHandler.SelectQuery("select username, name, follower_count, following_count, picture_count, day, month, year from tb_scrape_following where username = '" + selectedUsername + "'", "tb_scrape_following");
                }

                DataTable DT = DS.Tables[0];
                System.IO.File.Delete(filePath);
                foreach (DataRow DataRow in DT.Rows)
                {
                    try
                    {
                        string username = DataRow[0].ToString().Replace(",", ";");
                        string name = DataRow[1].ToString().Replace(",", ";");
                        string follower_count = DataRow[2].ToString().Replace(",", "");
                        string following_count = DataRow[3].ToString().Replace(",", "");
                        string picture_count = DataRow[4].ToString().Replace(",", "");
                        string day = DataRow[5].ToString().Replace(",", "");
                        string month = DataRow[6].ToString().Replace(",", "");
                        string year = DataRow[7].ToString().Replace(",", "");

                        string Header = "Username" + "," + "Name" + "," + "Follower count" + "," + "Following count" + "," + "Picture count" + "," + "Day" + "," + "Month" + "," + "Year";
                        string data = username.Replace(",", ";") + "," + name + "," + follower_count.Replace(",", ";") + "," + following_count.Replace(",", ";") + "," + picture_count.Replace(",", ";") + "," + day.Replace(",", ";") + "," + month.Replace(",", ";") + "," + year.Replace(",", ";");

                        Globussoft.GramBoardProFileHelper.ExportDataCSVFile(Header, data, filePath);
                    }
                    catch (Exception ex)
                    {
                        GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
                        GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => ExportData :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
                        GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
                    }
                }

            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => ExportData :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
            }
        }

        private void btnClearScrapeFollowers_Click(object sender, EventArgs e)
        {
            try
            {
                string selectedUsername = cmbScrapeFollowersUsername.SelectedItem.ToString();
                if (!string.IsNullOrEmpty(selectedUsername))
                {
                    if (cmbScrapeFollowersSelection.SelectedIndex != -1)
                    {
                        string selectedSelection = cmbScrapeFollowersSelection.SelectedItem.ToString();
                        if (selectedSelection == "Follower")
                        {
                            DataBaseHandler.DeleteQuery("delete from tb_scrape_follower where username ='" + selectedUsername + "'", "tb_scrape_follower");
                        }
                        if (selectedSelection == "Following")
                        {
                            DataBaseHandler.DeleteQuery("delete from tb_scrape_following where username ='" + selectedUsername + "'", "tb_scrape_following");
                        }

                        DataTable dt = UsernameFilter(selectedUsername, selectedSelection);
                        DataSet dataSet = new DataSet();
                        dataSet.Tables.Add(dt);
                        dgvFollowers.DataSource = dataSet.Tables[0];
                    }
                    else
                    {
                        AddToScrapeFollowersLogger("[ " + DateTime.Now + " ] => [Please select \"Follower\" or \"Following\". ]");
                    }
                }
                else
                {
                    AddToScrapeFollowersLogger("[ " + DateTime.Now + " ] => [Please select username. ]");
                }
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => btnClearScrapeFollowers_Click :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathScrapeFollowers);
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://pvadomination.com/");
        }

        private void tab_instagram_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tab_instagram.SelectedTab.Name == "tabScrapeFollowers")
            {
                List<List<string>> list_Accounts = new List<List<string>>();

                acc_checker_counter = DemoStagramPro.ClGlobul.accountList.Count();
                list_Accounts = ListUtilities.Split(DemoStagramPro.ClGlobul.accountList, Maxthread);
                if (ClGlobul.accountList.Count != 0)
                {
                    btnStartScrapeFollowers.Enabled = true;
                    foreach (List<string> list_account in list_Accounts)
                    {
                        foreach (string val in list_account)
                        {
                            string[] StrArrar = Regex.Split(val, ":");
                            string account = StrArrar[0];
                            if (!cmbScraperAccounts.Items.Contains(account))
                            {
                                cmbScraperAccounts.Items.Add(account);
                            }
                        }
                    }
                    cmbScraperAccounts.SelectedIndex = 0;
                }
                else
                {
                    AddToScrapeFollowersLogger("[ " + DateTime.Now + " ] => [Please upload account. ]");
                }
            }
        }



        private void cmbScraperAccounts_SelectionChangeCommitted(object sender, EventArgs e)
        {
            cmbScraperAccounts.SelectedIndex = accountIndex;
        }


        private void btnUnfollerStop_Click(object sender, EventArgs e)
        {
            try
            {
                new Thread(() =>
                {
                    ThreadIs = true;
                    List<Thread> tempLst = new List<Thread>();
                    try
                    {
                        tempLst.AddRange(UnfollowListListThread);
                    }
                    catch { };
                    foreach (Thread tempLst_item in tempLst)
                    {
                        
                        try
                        {
                            tempLst_item.Abort();
                            lst_Thread.Remove(tempLst_item);
                            //AddToLogger("{0}" + tempLst_item.CurrentCulture.Name);
                        }
                        catch (Exception ex)
                        {
                            GramBoardProFileHelper.AppendStringToTextfileNewLine("------------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                            GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => btn_StopFollow :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                            GramBoardProFileHelper.AppendStringToTextfileNewLine("------------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                        }
                    }
                   // AddToUnfollowLogger("[ " + DateTime.Now + " ] => [ All Process is Aborted ]");
                }).Start();
            }
            catch (Exception ex)
            {
                AddTophotoLogger("[ " + DateTime.Now + " ] => [ Error : StopFollow" + ex.Message + " ]");
            }

            try
            {
                new Thread(() => stopUnfollow()).Start();
            }
            catch (Exception ex)
            {
                AddTophotoLogger("[ " + DateTime.Now + " ] => [ Error : StopFollow" + ex.Message + " ]");
            }
        }


        private void txt_NoOfCommentThread_TextChanged(object sender, EventArgs e)
        {

        }

        private void chkDivideDataFollow_CheckedChanged(object sender, EventArgs e)
        {

            if (chkDivideDataFollow1.Checked == true)
            {
                rdbDivideGivenByUser1.Enabled = true;
                rdbDivideEqually1.Enabled = true;
            }
            else
            {
                rdbDivideGivenByUser1.Enabled = false;
                rdbDivideEqually1.Enabled = false;
            }
        }

        private void rdbDivideGivenByUser_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbDivideGivenByUser.Checked)
            {
                txtDiveideByUser.Enabled = true;
            }
            else
            {
                txtDiveideByUser.Enabled = false;
            }
        }



        private void btnUploadImage_Click(object sender, EventArgs e)
        {
            try
            {
                int status = 1;
                bool NoOfCount = int.TryParse(txtcountimage.Text.Trim(), out status);
                if (NoOfCount)
                {
                    ClGlobul.countNOOfFollowersandImageDownload = status;
                }
                else
                {
                    AddTophotoLogger("[ " + DateTime.Now + " ] =>[Pleae enter valid count");
                    return;
                }

                //if (!string.IsNullOrEmpty(txtUploadSingleItem.Text.Trim()))
                //{

                if (!string.IsNullOrEmpty(txtUploadSingleItem.Text.Trim()))
                {

                    //add following in followingList...
                    lstStoreDownloadImageKeyword.Clear();

                    string s = txtUploadSingleItem.Text.ToString();

                    if (s.Contains(','))
                    {
                        string[] Data = s.Split(',');

                        foreach (var item in Data)
                        {


                            lstStoreDownloadImageKeyword.Add(item);
                        }
                    }
                    else
                    {
                        lstStoreDownloadImageKeyword.Add(txtUploadSingleItem.Text.ToString());
                    }
                }
                   
                  

                
               
                //lstStoreDownloadImageKeyword.Clear();
                //lstStoreDownloadImageKeyword.Add(txtUploadSingleItem.Text.Trim());
                //  }
                //else
                //{
                //    lstStoreDownloadImageKeyword.Clear();
                //    lstStoreDownloadImageKeyword.Add(txtDownLoadImageUploadHashTag.Text.Trim());
                //}
               


                Thread newThread = new Thread(DownloadingImage);
                newThread.Start();
            }

            catch (Exception)
            {

                throw;
            }
        }
       


        public void DownloadingImage()
        {

            try
            {
                GloBoardPro.lstThread.Add(Thread.CurrentThread);
                Thread.CurrentThread.IsBackground = true;
                GloBoardPro.lstThread = GloBoardPro.lstThread.Distinct().ToList();
            }
            catch { };

            try
            {

                foreach (string item_image in lstStoreDownloadImageKeyword)
                {
                    startDownloadingImage(item_image);

                    Thread.Sleep(5000);
                    //write delay here.

                    //if (!string.IsNullOrEmpty(delayimagemin.Text) && NumberHelper.ValidateNumber(delayimagemin.Text))
                    //{
                    //    mindelay = Convert.ToInt32(delayimagemin.Text);
                    //}
                    //if (!string.IsNullOrEmpty(imagedelaymax.Text) && NumberHelper.ValidateNumber(imagedelaymax.Text))
                    //{
                    //    maxdelay = Convert.ToInt32(imagedelaymax.Text);
                    //}

                    //lock (_lockObject)
                    //{
                    //    int delay = RandomNumberGenerator.GenerateRandom(mindelay, maxdelay);
                    //    AddTophotoLogger("[ " + DateTime.Now + " ] => [ Delay For " + delay + " Seconds ]");
                    //    Thread.Sleep(delay * 1000);
                    //}

                }

                AddTophotoLogger("[ " + DateTime.Now + " ] =>[Process completed.");
            }




            catch { }
        }

        public void startDownloadingImage(string itemImageTag)
        {

            try
            {
                Thread.CurrentThread.IsBackground = true;
                Lst_photoLikethread.Add(Thread.CurrentThread);
                Lst_photoLikethread = Lst_photoLikethread.Distinct().ToList();
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("------------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => unlike :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("------------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);

            }

            string pageSource = string.Empty;
            int counter = 0;
            GlobDramProHttpHelper objGlobusHttpHelper = new GlobDramProHttpHelper();
            List<string> lstCountImage = new List<string>();
            string url = string.Empty;
            string mainUrl = "http://websta.me/";
            if (IsDownLoadImageUsingHashTag)
            {
                url = mainUrl + "tag/" + itemImageTag.Replace("#",string.Empty);
                AddTophotoLogger("[ " + DateTime.Now + " ] =>[Process Using HashTag =" + itemImageTag);
            }
           
            else if (IsDownLoadImageUsingUserName)
            {
                url = mainUrl + "n/" + itemImageTag;
                AddTophotoLogger("[ " + DateTime.Now + " ] =>[Process Selected Using UserName  =" + itemImageTag);
            }
           


            else
            {
                url = mainUrl + "tag/" + itemImageTag;
                // AddTophotoLogger("Process Using HashTag =" + itemImageTag);
                AddTophotoLogger("[ " + DateTime.Now + " ] =>[Process Using HashTag" + itemImageTag);
            }
    

            try
            {
                pageSource = objGlobusHttpHelper.getHtmlfromUrl(new Uri(url),"");
            }
            catch { }

            if (string.IsNullOrEmpty(pageSource))
            {
                pageSource = objGlobusHttpHelper.getHtmlfromUrl(new Uri(url),"");
            }
          
           

            if (!string.IsNullOrEmpty(pageSource))
            {
                if (pageSource.Contains("<div class=\"mainimg_wrapper\">"))
                {
                    string[] arr = Regex.Split(pageSource, "<div class=\"mainimg_wrapper\">");
                    if (arr.Length > 1)
                    {
                        arr = arr.Skip(1).ToArray();

                        foreach (string itemarr in arr)
                        {

                            try
                            {
                                string startString = "<a href=\"";
                                string endString = "\" class=\"mainimg\"";
                                string imageId = string.Empty;
                                string imageSrc = string.Empty;
                                if (itemarr.Contains("<a href=\"/p/"))
                                {
                                    int indexStart = itemarr.IndexOf("<a href=\"/p/");
                                    string itemarrNow = itemarr.Substring(indexStart);
                                    if (itemarrNow.Contains(startString) && itemarrNow.Contains(endString))
                                    {
                                        try
                                        {
                                            //FileData
                                            imageId = getBetween(itemarrNow, startString, endString).Replace("/", "");
                                        }
                                        catch { }
                                        if (!string.IsNullOrEmpty(imageId))
                                        {
                                            //counter++;
                                            //imageId = "http://websta.me"+imageId;
                                        }
                                    }

                                    if (itemarrNow.Contains("<img src=\""))
                                    {
                                        try
                                        {
                                            imageSrc = getBetween(itemarrNow, "<img src=\"", "\"");
                                        }
                                        catch { }
                                        counter++;


                                        SaveImageWithUrl(imageSrc, FileData, imageId + "_" + counter);//FileData //Globals.imageDesktopPath


                                        lstCountImage.Add(imageSrc);
                                        lstCountImage = lstCountImage.Distinct().ToList();
                                        // AddTophotoLogger("Image DownLoaded with ImageName  " + imageId + "_" + counter);
                                        AddTophotoLogger("[ " + DateTime.Now + " ] =>[Image DownLoaded with user=" + itemImageTag + "_" + imageId + "_" + counter);
                                        if (!string.IsNullOrEmpty(delayimagemin.Text) && NumberHelper.ValidateNumber(delayimagemin.Text))
                                        {
                                            mindelay = Convert.ToInt32(delayimagemin.Text);
                                        }
                                        if (!string.IsNullOrEmpty(imagedelaymax.Text) && NumberHelper.ValidateNumber(imagedelaymax.Text))
                                        {
                                            maxdelay = Convert.ToInt32(imagedelaymax.Text);
                                        }
                                        lock (_lockObject)
                                        {
                                            int delay = RandomNumberGenerator.GenerateRandom(mindelay, maxdelay);
                                            AddTophotoLogger("[ " + DateTime.Now + " ] => [ Delay For " + delay + " Seconds ]");
                                            Thread.Sleep(delay *1000);
                                        }

                                        if (lstCountImage.Count >= ClGlobul.countNOOfFollowersandImageDownload)
                                        {
                                            return;
                                        }


                                        try
                                        {

                                        }
                                        catch { }
                                    }
                                }
                            }
                            catch { }




                        }



                        #region pagination
                        string pageLink = string.Empty;
                        while (true)
                        {
                            //if (stopScrapImageBool) return;
                            string startString = "<a href=\"";
                            string endString = "\" class=\"mainimg\"";
                            string imageId = string.Empty;
                            string imageSrc = string.Empty;

                            if (!string.IsNullOrEmpty(pageLink))
                            {
                                pageSource = objGlobusHttpHelper.getHtmlfromUrl(new Uri(pageLink),"");
                                if (string.IsNullOrEmpty(pageSource))
                                {
                                    pageSource = objGlobusHttpHelper.getHtmlfromUrl(new Uri(pageLink),"");
                                }
                            }

                            if (pageSource.Contains("<ul class=\"pager\">") && pageSource.Contains("rel=\"next\">"))
                            {
                                try
                                {
                                    pageLink = getBetween(pageSource, "<ul class=\"pager\">", "rel=\"next\">");
                                }
                                catch { }
                                if (!string.IsNullOrEmpty(pageLink))
                                {
                                    try
                                    {
                                        int len = pageLink.IndexOf("<a href=\"");
                                        len = len + ("<a href=\"").Length;
                                        pageLink = pageLink.Substring(len);
                                        pageLink = pageLink.Trim();
                                        pageLink = pageLink.TrimEnd(new char[] { '"' });
                                        pageLink = "http://websta.me/" + pageLink;
                                    }
                                    catch { }
                                    if (!string.IsNullOrEmpty(pageLink))
                                    {
                                        string response = string.Empty;
                                        try
                                        {
                                            response = objGlobusHttpHelper.getHtmlfromUrl(new Uri(pageLink),"");
                                        }
                                        catch { }
                                        if (!string.IsNullOrEmpty(response))
                                        {
                                            if (response.Contains("<div class=\"mainimg_wrapper\">"))
                                            {
                                                try
                                                {
                                                    string[] arr1 = Regex.Split(response, "<div class=\"mainimg_wrapper\">");
                                                    if (arr1.Length > 1)
                                                    {
                                                        arr1 = arr1.Skip(1).ToArray();
                                                        foreach (string items in arr1)
                                                        {
                                                            try
                                                            {
                                                                //if (stopScrapImageBool) return;
                                                                if (items.Contains("<a href=\"/p/"))
                                                                {
                                                                    int indexStart = items.IndexOf("<a href=\"/p/");
                                                                    string itemarrNow = items.Substring(indexStart);

                                                                    try
                                                                    {
                                                                        imageId = getBetween(itemarrNow, startString, endString).Replace("/", "");
                                                                    }
                                                                    catch { }
                                                                    if (!string.IsNullOrEmpty(imageId))
                                                                    {
                                                                        //counter++;
                                                                        //imageId = "http://websta.me"+imageId;
                                                                    }

                                                                    if (itemarrNow.Contains("<img src=\""))
                                                                    {
                                                                        try
                                                                        {
                                                                            imageSrc = getBetween(itemarrNow, "<img src=\"", "\"");
                                                                        }
                                                                        catch { }
                                                                    }

                                                                    counter++;

                                                                    {
                                                                        SaveImageWithUrl(imageSrc, FileData, imageId + "_" + counter);
                                                                    }
                                                                    lstCountImage.Add(imageSrc);
                                                                    lstCountImage = lstCountImage.Distinct().ToList();
                                                                    //AddTophotoLogger("Image DownLoaded with ImageName  " + imageId + "_" + counter);
                                                                    AddTophotoLogger("[ " + DateTime.Now + " ] =>[Image DownLoaded with ImageName  " + imageId + "_" + counter);
                                                                    if (!string.IsNullOrEmpty(delayimagemin.Text) && NumberHelper.ValidateNumber(delayimagemin.Text))
                                                                    {
                                                                        mindelay = Convert.ToInt32(delayimagemin.Text);
                                                                    }
                                                                    if (!string.IsNullOrEmpty(imagedelaymax.Text) && NumberHelper.ValidateNumber(imagedelaymax.Text))
                                                                    {
                                                                        maxdelay = Convert.ToInt32(imagedelaymax.Text);
                                                                    }
                                                                    lock (_lockObject)
                                                                    {
                                                                        int delay = RandomNumberGenerator.GenerateRandom(mindelay, maxdelay);
                                                                        AddToLogger("[ " + DateTime.Now + " ] => [ Delay For " + delay + " Seconds ]");
                                                                        Thread.Sleep(delay * 1000);
                                                                    }
                                                                    if (lstCountImage.Count >= ClGlobul.countNOOfFollowersandImageDownload)
                                                                    {
                                                                        return;
                                                                    }

                                                                }

                                                            }
                                                            catch { }
                                                        }
                                                        if (lstCountImage.Count >= ClGlobul.countNOOfFollowersandImageDownload)
                                                        {
                                                            return;
                                                        }
                                                    }
                                                }
                                                catch { }

                                            }
                                        }
                                        else
                                        {

                                        }

                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        #endregion

                    }
                }
            }
            if (!string.IsNullOrEmpty(pageSource))
            {
                url = mainUrl + "n/" + itemImageTag;
                AddTophotoLogger("[ " + DateTime.Now + " ] =>[" + itemImageTag  +  " = Username is Private, so unable to fetch Image ");
            
            }
            else
            {
                AddTophotoLogger("[ " + DateTime.Now + " ] =>[" + itemImageTag + " = This user does not exist.");
            }
          
            //else
            //{
            //    AddToLogger("[ " + DateTime.Now + " ] => [  No Image found.]");
            //    //return;
            //}


        }

          

        private void SaveImageWithUrl(string imgeUri, string saveto, string imageName)
        {

            try
            {
                GloBoardPro.lstThread.Add(Thread.CurrentThread);
                Thread.CurrentThread.IsBackground = true;
                GloBoardPro.lstThread = GloBoardPro.lstThread.Distinct().ToList();
            }
            catch { };
            try
            {
                // GlobusFileHelper.AppendStringToTextfileNewLine(imgeUri, ImageUrlData);
                using (WebClient webClient = new WebClient())
                {
                    using (Stream stream = webClient.OpenRead(imgeUri))
                    {
                        byte[] oImageBytes = webClient.DownloadData(imgeUri);
                        {


                            File.WriteAllBytes(filepath + "\\" + imageName + ".jpg", oImageBytes);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbDivideGivenByUser1.Checked)
            {
                txtDiveideByUser1.Enabled = true;
            }
            else
            {
                txtDiveideByUser1.Enabled = false;
            }
        }

        private void btn_uploadAccount1_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    using (OpenFileDialog ofd = new OpenFileDialog())
            //    {
            //        ofd.InitialDirectory = Application.StartupPath;
            //        ofd.Filter = "Text Files (*.txt)|*.txt";
            //        if (ofd.ShowDialog() == DialogResult.OK)
            //        {
            //            txtAddAccounts1.Text = ofd.FileName;
            //            ReadLargeAccountsFile(ofd.FileName);
            //        }
            //    }
            //    loaddatafmdb();
            //}
            //catch (Exception ex)
            //{
            //    AddToLogger1("[ " + DateTime.Now + " ] => [  Upload Accounts Loaded ]");
            //}
        }

        private void btnAccountcheck_Click_1(object sender, EventArgs e)
        {

        }

        private void btnAccountcheck_Click1(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

            if (chkDivideDataFollow1.Checked == true)
            {
                rdbDivideGivenByUser1.Enabled = true;
                rdbDivideEqually1.Enabled = true;
            }
            else
            {
                rdbDivideGivenByUser1.Enabled = false;
                rdbDivideEqually1.Enabled = false;
            }
        }

        private void rdbphotoid_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            //if (radioButton2.Checked)
            //{
            //    rdbphotoid.Enabled = true;
            //}
            //else
            //{
            //    rdbphotoid.Enabled = false;
            //}
        }

        private void btn_followingFile1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txt_FollowUser.Text.ToString()))
            {
                AddToLogger("[ " + DateTime.Now + " ] => [ " + " Name of Following already added ]");
                return;
            }
            try
            {
                ClGlobul.followingList.Clear();
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => Finally (FollowingFile) :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePath);
            }
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.InitialDirectory = Application.StartupPath;
                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        txt_filepathofFollowing1.Text = ofd.FileName;
                        ReadLargeFollowingFile(ofd.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => Finally (FollowingFile) :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePath);
            }
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {
            Graphics g;

            g = e.Graphics;

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //// Draw the image.
            g.DrawImage(PanelImage, 0, 0, this.Width, this.Height);
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            Graphics g;

            g = e.Graphics;

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //// Draw the image.
            g.DrawImage(PanelImage, 0, 0, this.Width, this.Height);
        }

        private void tabScrapeFollowers_Paint(object sender, PaintEventArgs e)
        {
            Graphics g;

            g = e.Graphics;

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //// Draw the image.
            g.DrawImage(PanelImage, 0, 0, this.Width, this.Height);
        }

        private void btnClearUploadAccounts_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want to clear all accounts from database ? ", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {

                try
                {
                    QueryExecuter.deleteQuery();
                    ClGlobul.accountList.Clear();
                    txtAddAccounts1.Text = string.Empty;
                    loaddatafmdb();


                }
                catch (Exception ex)
                {
                    GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                    GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => btnClearUploadAccount_Click :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                    GramBoardProFileHelper.AppendStringToTextfileNewLine("-----------------------------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                }
                AddToLogger1("[ " + DateTime.Now + " ] => [  All Accounts Cleared ]");
            }


        }

        private void btnUploadHashtag_Click(object sender, EventArgs e)
        {
            try
            {


                using (OpenFileDialog ofdphotolike = new OpenFileDialog())
                {
                    ofdphotolike.InitialDirectory = Application.StartupPath;
                    ofdphotolike.Filter = "Text Files (*.txt)|*.txt";
                    if (ofdphotolike.ShowDialog() == DialogResult.OK)
                    {
                        txtDownLoadImageUploadHashTag.Text = ofdphotolike.FileName;
                        List<string> photolist = GramBoardProFileHelper.ReadFile(txtDownLoadImageUploadHashTag.Text);
                        lstStoreDownloadImageKeyword = new List<string>();
                        lstStoreDownloadImageKeyword = (photolist);
                        AddTophotoLogger("[ " + DateTime.Now + " ] => [ " + lstStoreDownloadImageKeyword.Count + " Image IDs Uploaded. ]");
                    }
                }
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                GramBoardProFileHelper.AppendStringToTextfileNewLine(DateTime.Now + ":=> Methode Name => btn_HashTagUpload :=> " + ex.Message, GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
                GramBoardProFileHelper.AppendStringToTextfileNewLine("------------------------------------------------------------------------", GramBoardProFileHelper.ErrorLogFilePathForPhotolike);
            }
        }

        bool IsDownLoadImageUsingHashTag = false;
        bool IsDownLoadImageUsingUserName = true;
        List<string> lstStoreDownloadImageKeyword = new List<string>();
        private void rdbDownLoadUserName_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                UnfollowListListThread.Add(Thread.CurrentThread);
                UnfollowListListThread.Distinct();
                Thread.CurrentThread.IsBackground = true;
            }
            catch
            {
            }

            if (rdbDownLoadUserName.Checked)
            {
                IsDownLoadImageUsingUserName = true;
                IsDownLoadImageUsingHashTag = false;
            }
            else
            {
                IsDownLoadImageUsingUserName = false;
            }
        }

        private void rdbDownLoadHashTag_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbDownLoadHashTag.Checked)
            {
                IsDownLoadImageUsingUserName = false;
                IsDownLoadImageUsingHashTag = true;
            }
            else
            {
                IsDownLoadImageUsingHashTag = false;
            }
        }

        private void txt_FollowUser_TextChanged(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void txtphotosingaluser_TextChanged(object sender, EventArgs e)
        {
          
        }

        private void chkHashCommentLike_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void groupBox13_Enter(object sender, EventArgs e)
        {

        }

        private void btnUnfollerStop1_Click(object sender, EventArgs e)
        {

        }

        private void BtnAccountCreater_Click(object sender, EventArgs e)
        {
            try
            {
                if (Globals.IsAccountopen)
                {

                    Globals.IsAccountopen = false;
                    FrmAccount frmaccounts = new FrmAccount();
                    frmaccounts.Show();
                    try
                    {
                        loaddatafmdb();
                    }
                    catch { };
                }
                else
                {
                    MessageBox.Show("Account Form is already Open.", "Alert");
                }
            }
            catch { };
        }

        private void cmbScrapeFollowersUsername_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
    class classsforlogger
    {

        public void AddToImageTagLogger(string log)
        {
            try
            {

                frm_stagram objbbbFrmMain = (frm_stagram)Application.OpenForms["frm_stagram"];

                if (objbbbFrmMain.lstImageLogger.InvokeRequired)
                {
                    objbbbFrmMain.lstImageLogger.Invoke(new MethodInvoker(delegate
                    {
                        objbbbFrmMain.lstImageLogger.Items.Add(log);
                        objbbbFrmMain.lstImageLogger.SelectedIndex = objbbbFrmMain.lstHashLogger.Items.Count - 1;
                    }));
                }
                else
                {
                    objbbbFrmMain.lstImageLogger.Items.Add(log);
                    objbbbFrmMain.lstImageLogger.SelectedIndex = objbbbFrmMain.lstHashLogger.Items.Count - 1;
                }
            }
            catch (Exception ex)
            {
                // AddToCommentLogger("Error : AddToImageTagLogger :-" + ex.Message);
            }
        }
        
    }

    class WriteInLoggerClass : Form
    {
       
    }

    #region LogFornetclass
    public class GlobusLogAppender : log4net.Appender.AppenderSkeleton
    {

        private static readonly object lockerLog4Append = new object();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loggingEvent"></param>
        protected override void Append(log4net.Core.LoggingEvent loggingEvent)
        {
            try
            {
                string loggerName = loggingEvent.Level.Name;

                frm_stagram objbbbFrmMain = (frm_stagram)Application.OpenForms["frm_stagram"];

                lock (lockerLog4Append)
                {
                    switch (loggingEvent.Level.Name)
                    {
                        case "DEBUG":
                            try
                            {

                                {
                                    if (objbbbFrmMain.lstHashLogger.InvokeRequired)
                                    {
                                        objbbbFrmMain.lstHashLogger.Invoke(new MethodInvoker(delegate
                                        {
                                            try
                                            {
                                                if (objbbbFrmMain.lstHashLogger.Items.Count > 1000)
                                                {
                                                    objbbbFrmMain.lstHashLogger.Items.RemoveAt(objbbbFrmMain.lstHashLogger.Items.Count - 1);//.Add(frmDominator.listBoxLogs.Items.Add(loggingEvent.TimeStamp + "\t" + loggingEvent.LoggerName + "\r\t\t" + loggingEvent.RenderedMessage);
                                                }

                                                //objbbbFrmMain.lstLogger.Items.Insert(0, loggingEvent.TimeStamp + "\t" + loggingEvent.LoggerName + "\r\t" + loggingEvent.RenderedMessage);
                                                objbbbFrmMain.lstHashLogger.Items.Add(loggingEvent.TimeStamp + "\t" + loggingEvent.RenderedMessage);
                                            }
                                            catch (Exception ex)
                                            {
                                                GramBoardLogHelper.log.Error(" Error : " + ex.StackTrace);
                                            }

                                        }));

                                    }
                                    else
                                    {
                                        try
                                        {
                                            if (objbbbFrmMain.lstHashLogger.Items.Count > 1000)
                                            {
                                                objbbbFrmMain.lstHashLogger.Items.RemoveAt(objbbbFrmMain.lstHashLogger.Items.Count - 1);
                                            }

                                            //objbbbFrmMain.lstLogger.Items.Insert(0, loggingEvent.TimeStamp + "\t" + loggingEvent.LoggerName + "\r\t" + loggingEvent.RenderedMessage);
                                            objbbbFrmMain.lstHashLogger.Items.Add(loggingEvent.TimeStamp + "\t" + loggingEvent.RenderedMessage);
                                        }

                                        catch (Exception ex)
                                        {
                                            GramBoardLogHelper.log.Error("Error : 74" + ex.Message);
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error Case Debug : " + ex.StackTrace);
                                Console.WriteLine("Error Case Debug : " + ex.Message);
                                GramBoardLogHelper.log.Error(" Error : " + ex.StackTrace);
                            }
                            break;
                        case "INFO":
                            try
                            {


                                if (objbbbFrmMain.lstHashLogger.InvokeRequired)
                                {
                                    objbbbFrmMain.lstHashLogger.Invoke(new MethodInvoker(delegate
                                    {
                                        try
                                        {
                                            if (objbbbFrmMain.lstHashLogger.Items.Count > 1000)
                                            {
                                                objbbbFrmMain.lstHashLogger.Items.RemoveAt(objbbbFrmMain.lstHashLogger.Items.Count - 1);
                                            }

                                            // objbbbFrmMain.lstLogger.Items.Insert(0, loggingEvent.TimeStamp + "\t" + loggingEvent.LoggerName + "\t\t" + loggingEvent.RenderedMessage);
                                            objbbbFrmMain.lstHashLogger.Items.Add(loggingEvent.TimeStamp + "\t" + loggingEvent.RenderedMessage);
                                        }
                                        catch (Exception ex)
                                        {
                                            GramBoardLogHelper.log.Error(" Error : " + ex.StackTrace);
                                        }

                                    }));

                                }
                                else
                                {
                                    try
                                    {
                                        if (objbbbFrmMain.lstHashLogger.Items.Count > 1000)
                                        {
                                            objbbbFrmMain.lstHashLogger.Items.RemoveAt(objbbbFrmMain.lstHashLogger.Items.Count - 1);
                                        }

                                        //objbbbFrmMain.lstLogger.Items.Insert(0, loggingEvent.TimeStamp + "\t" + loggingEvent.LoggerName + "\t\t" + loggingEvent.RenderedMessage);
                                        objbbbFrmMain.lstHashLogger.Items.Add(loggingEvent.TimeStamp + "\t" + loggingEvent.RenderedMessage);
                                    }
                                    catch (Exception ex)
                                    {
                                        GramBoardLogHelper.log.Error("Error : 75" + ex.Message);
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error Case INFO : " + ex.StackTrace);
                                Console.WriteLine("Error Case INFO : " + ex.Message);
                                GramBoardLogHelper.log.Error(" Error : " + ex.StackTrace);
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                GramBoardLogHelper.log.Error("Error : 76" + ex.Message);
            }
        }
    }
    #endregion




}
                            
                       
               


         
    
