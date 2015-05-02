using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Globussoft;
using BaseLib;
using System.Threading;
using System.Drawing.Drawing2D;


namespace DemoStagramPro
{   
    public partial class Frm_proxy : Form
    {
        #region global declaration
        bool ProcessStartORnot = false;
        Bitmap image; 
        #endregion

        #region Frm_proxy()
        public Frm_proxy()
        {
            InitializeComponent();
        } 
        #endregion

        #region Frm_proxy_Load
        private void Frm_proxy_Load(object sender, EventArgs e)
        {
            StagramProxyChecker.classes.ClProxyChecker.ProxyLogger.addToLogger += new EventHandler(ProxyLogger_addToLogger);
            // change the page backImage
            //image = (Bitmap)Image.FromFile(@"C:\Users\user\Downloads/8 cute baby.jpg", true);
        } 
        #endregion

        #region ProxyLogger_addToLogger
        void ProxyLogger_addToLogger(object sender, EventArgs e)
        {
            if (e is EventsArgs)
            {
                EventsArgs eventArgs = e as EventsArgs;
                AddToLogger(eventArgs.log);
            }
        } 
        #endregion

        #region btn_proxyUpload_Click
        private void btn_proxyUpload_Click(object sender, EventArgs e)
        {
            //if (ProcessStartORnot)
            //{
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.InitialDirectory = Application.StartupPath;
                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        txt_proxylistPath.Text = ofd.FileName;
                        Loadproxy(ofd.FileName);
                    }
                }
            }
            catch { };
            //}
        } 
        #endregion

        #region btn_startProxyChecking_Click
        private void btn_startProxyChecking_Click(object sender, EventArgs e)
        {
            StagramProxyChecker.classes.ClProxyChecker.proxyStop = false;
            if (string.IsNullOrEmpty(txt_ProxyNoOfThread.Text))
            {
                if (MessageBox.Show("Do you really want to Start WitoutThread", "Confirm ", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    ProcessStartORnot = true;
                    ClGlobul.ProxyCheckNoOfThread = 1;
                }
                else
                {
                }
            }
            else
            {
                try
                {
                    ClGlobul.ProxyCheckNoOfThread = Convert.ToInt16(txt_ProxyNoOfThread.Text);
                    ProcessStartORnot = true;
                }
                catch (Exception)
                {
                    MessageBox.Show("Please Enter Numeric Value in IP Thread..!!", "Wrong Value Insert In Photo Like Thread");
                }
            }

            if (ProcessStartORnot)
            {
                new Thread(() =>
                    {
                        getchekingproxy();


                    }).Start();
            }
        } 
        #endregion

        #region Loadproxy
        public void Loadproxy(string filePath)
        {
            try
            {
                string ValidIpAddressRegex = @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$";
                List<string> proxyList = GramBoardProFileHelper.ReadFile(filePath);
                List<string> getTheFinaleProxyList = proxyList.Where(e => e.StartsWith(System.Text.RegularExpressions.Regex.Match(e.ToString(), ValidIpAddressRegex).ToString())).ToList();
                ClGlobul.ProxyList = getTheFinaleProxyList;
                //getchekingproxy();
                AddToLogger("[ " + DateTime.Now + " ] => [ " + ClGlobul.ProxyList.Count() + " IPUploaded ]");
            }
            catch (Exception)
            {
            }
        } 
        #endregion

        #region getchekingproxy
        public void getchekingproxy()
        {
            try
            {
                Thread.CurrentThread.IsBackground = true;
                StagramProxyChecker.classes.ClProxyChecker.lstProxyThread.Add(Thread.CurrentThread);
                StagramProxyChecker.classes.ClProxyChecker.lstProxyThread = StagramProxyChecker.classes.ClProxyChecker.lstProxyThread.Distinct().ToList();
            }
            catch { }

            new Thread(() =>
            {
                StagramProxyChecker.classes.ClProxyChecker ClProxyChecker = new StagramProxyChecker.classes.ClProxyChecker();
                ClProxyChecker.proxycheckStart();
            }).Start();
        } 
        #endregion

        #region AddToLogger
        private void AddToLogger(string log)
        {
            try
            {
                if (IPlbLogger.InvokeRequired)
                {
                    IPlbLogger.Invoke(new MethodInvoker(delegate
                       {
                           IPlbLogger.Items.Add(log);
                           IPlbLogger.SelectedIndex = IPlbLogger.Items.Count - 1;
                       }));
                }
                else
                {
                    IPlbLogger.Items.Add(log);
                    IPlbLogger.SelectedIndex = IPlbLogger.Items.Count - 1;
                }
            }
            catch (Exception ex)
            {

            }

        } 
        #endregion

        #region Frm_proxy_Paint
        private void Frm_proxy_Paint(object sender, PaintEventArgs e)
        {
            Graphics g;
            g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.DrawImage(Properties.Resources.Bacground, 0, 0, this.Width, this.Height);
        } 
        #endregion

        private void btnStopCheckProxy_Click(object sender, EventArgs e)
        {
            StagramProxyChecker.classes.ClProxyChecker.proxyStop = true;

            StagramProxyChecker.classes.ClProxyChecker _ClProxyChecker = new StagramProxyChecker.classes.ClProxyChecker();
            Thread _ThreadStopProxyCheck = new Thread(()=> _ClProxyChecker.stopProxy());
            _ThreadStopProxyCheck.Name = "StopIP";
            _ThreadStopProxyCheck.Start();
        }

    }
}
