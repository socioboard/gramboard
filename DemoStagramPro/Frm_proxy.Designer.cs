namespace DemoStagramPro
{
    partial class Frm_proxy
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_proxy));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lb_IPNoOfThread = new System.Windows.Forms.Label();
            this.txt_ProxyNoOfThread = new System.Windows.Forms.TextBox();
            this.txt_proxylistPath = new System.Windows.Forms.TextBox();
            this.btn_proxyUpload = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.IPlbLogger = new System.Windows.Forms.ListBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnStopCheckProxy = new System.Windows.Forms.Button();
            this.btn_startProxyChecking = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.lb_IPNoOfThread);
            this.groupBox1.Controls.Add(this.txt_ProxyNoOfThread);
            this.groupBox1.Controls.Add(this.txt_proxylistPath);
            this.groupBox1.Controls.Add(this.btn_proxyUpload);
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(22, 14);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(432, 100);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "IP Upload ";
            // 
            // lb_IPNoOfThread
            // 
            this.lb_IPNoOfThread.AutoSize = true;
            this.lb_IPNoOfThread.Location = new System.Drawing.Point(36, 74);
            this.lb_IPNoOfThread.Name = "lb_IPNoOfThread";
            this.lb_IPNoOfThread.Size = new System.Drawing.Size(90, 13);
            this.lb_IPNoOfThread.TabIndex = 3;
            this.lb_IPNoOfThread.Text = "IP No Of Threads";
            // 
            // txt_ProxyNoOfThread
            // 
            this.txt_ProxyNoOfThread.Location = new System.Drawing.Point(196, 71);
            this.txt_ProxyNoOfThread.Name = "txt_ProxyNoOfThread";
            this.txt_ProxyNoOfThread.Size = new System.Drawing.Size(100, 20);
            this.txt_ProxyNoOfThread.TabIndex = 2;
            this.txt_ProxyNoOfThread.Text = "5";
            // 
            // txt_proxylistPath
            // 
            this.txt_proxylistPath.Enabled = false;
            this.txt_proxylistPath.Location = new System.Drawing.Point(36, 37);
            this.txt_proxylistPath.Name = "txt_proxylistPath";
            this.txt_proxylistPath.Size = new System.Drawing.Size(260, 20);
            this.txt_proxylistPath.TabIndex = 1;
            // 
            // btn_proxyUpload
            // 
            this.btn_proxyUpload.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(42)))), ((int)(((byte)(3)))));
            this.btn_proxyUpload.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_proxyUpload.ForeColor = System.Drawing.Color.Black;
            this.btn_proxyUpload.Image = global::DemoStagramPro.Properties.Resources.Bacground;
            this.btn_proxyUpload.Location = new System.Drawing.Point(315, 37);
            this.btn_proxyUpload.Name = "btn_proxyUpload";
            this.btn_proxyUpload.Size = new System.Drawing.Size(91, 23);
            this.btn_proxyUpload.TabIndex = 0;
            this.btn_proxyUpload.Text = "Upload IP";
            this.btn_proxyUpload.UseVisualStyleBackColor = false;
            this.btn_proxyUpload.Click += new System.EventHandler(this.btn_proxyUpload_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.Transparent;
            this.groupBox2.Controls.Add(this.IPlbLogger);
            this.groupBox2.ForeColor = System.Drawing.Color.White;
            this.groupBox2.Location = new System.Drawing.Point(22, 136);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(565, 199);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Logger";
            // 
            // IPlbLogger
            // 
            this.IPlbLogger.FormattingEnabled = true;
            this.IPlbLogger.Location = new System.Drawing.Point(7, 19);
            this.IPlbLogger.Name = "IPlbLogger";
            this.IPlbLogger.Size = new System.Drawing.Size(552, 173);
            this.IPlbLogger.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.Transparent;
            this.groupBox3.Controls.Add(this.btnStopCheckProxy);
            this.groupBox3.Controls.Add(this.btn_startProxyChecking);
            this.groupBox3.ForeColor = System.Drawing.Color.White;
            this.groupBox3.Location = new System.Drawing.Point(460, 14);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(127, 100);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            // 
            // btnStopCheckProxy
            // 
            this.btnStopCheckProxy.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(42)))), ((int)(((byte)(3)))));
            this.btnStopCheckProxy.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStopCheckProxy.ForeColor = System.Drawing.Color.Black;
            this.btnStopCheckProxy.Image = global::DemoStagramPro.Properties.Resources.Bacground;
            this.btnStopCheckProxy.Location = new System.Drawing.Point(15, 60);
            this.btnStopCheckProxy.Name = "btnStopCheckProxy";
            this.btnStopCheckProxy.Size = new System.Drawing.Size(95, 26);
            this.btnStopCheckProxy.TabIndex = 1;
            this.btnStopCheckProxy.Text = "Stop";
            this.btnStopCheckProxy.UseVisualStyleBackColor = false;
            this.btnStopCheckProxy.Click += new System.EventHandler(this.btnStopCheckProxy_Click);
            // 
            // btn_startProxyChecking
            // 
            this.btn_startProxyChecking.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(42)))), ((int)(((byte)(3)))));
            this.btn_startProxyChecking.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_startProxyChecking.ForeColor = System.Drawing.Color.Black;
            this.btn_startProxyChecking.Image = global::DemoStagramPro.Properties.Resources.Bacground;
            this.btn_startProxyChecking.Location = new System.Drawing.Point(15, 19);
            this.btn_startProxyChecking.Name = "btn_startProxyChecking";
            this.btn_startProxyChecking.Size = new System.Drawing.Size(95, 26);
            this.btn_startProxyChecking.TabIndex = 0;
            this.btn_startProxyChecking.Text = "Check IP";
            this.btn_startProxyChecking.UseVisualStyleBackColor = false;
            this.btn_startProxyChecking.Click += new System.EventHandler(this.btn_startProxyChecking_Click);
            // 
            // Frm_proxy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.BackgroundImage = global::DemoStagramPro.Properties.Resources.Bacground;
            this.ClientSize = new System.Drawing.Size(604, 347);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Frm_proxy";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "IP Upload";
            this.Load += new System.EventHandler(this.Frm_proxy_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Frm_proxy_Paint);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txt_proxylistPath;
        private System.Windows.Forms.Button btn_proxyUpload;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox IPlbLogger;
        private System.Windows.Forms.Label lb_IPNoOfThread;
        private System.Windows.Forms.TextBox txt_ProxyNoOfThread;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btn_startProxyChecking;
        private System.Windows.Forms.Button btnStopCheckProxy;
    }
}