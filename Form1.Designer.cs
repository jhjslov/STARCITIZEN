namespace SC_StepByStep_v1
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lstLog = new ListBox();
            txtRatioX = new TextBox();
            txtRatioY = new TextBox();
            label1 = new Label();
            label2 = new Label();
            txtRepeat = new TextBox();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            lblRole = new Label();
            btnToggleRole = new Button();
            btnConnect = new Button();
            txtTargetIP = new TextBox();
            label4 = new Label();
            label3 = new Label();
            txtMyIP = new Label();
            tabPage2 = new TabPage();
            chkSubManual = new CheckBox();
            txtSyncDist = new TextBox();
            chkSyncCagoLV = new CheckBox();
            label16 = new Label();
            label13 = new Label();
            label15 = new Label();
            label7 = new Label();
            label14 = new Label();
            label6 = new Label();
            txtWaitNext = new TextBox();
            label5 = new Label();
            chkCaptureEnable = new CheckBox();
            picUp = new PictureBox();
            txtTolerance = new TextBox();
            txtWaitMatch = new TextBox();
            picConfirm = new PictureBox();
            picDown = new PictureBox();
            tabPage3 = new TabPage();
            txtWaitEsc = new TextBox();
            label12 = new Label();
            label11 = new Label();
            txtWaitUp = new TextBox();
            label10 = new Label();
            label9 = new Label();
            label8 = new Label();
            btnSaveConfig = new Button();
            txtWaitConfirm = new TextBox();
            txtWaitClick = new TextBox();
            txtWaitAction = new TextBox();
            lblStatus = new Label();
            chkAlwaysOnTop = new CheckBox();
            chkSoundEnable = new CheckBox();
            chkAntiKick = new CheckBox();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picUp).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picConfirm).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picDown).BeginInit();
            tabPage3.SuspendLayout();
            SuspendLayout();
            // 
            // lstLog
            // 
            lstLog.Font = new Font("맑은 고딕", 8F, FontStyle.Regular, GraphicsUnit.Point, 129);
            lstLog.FormattingEnabled = true;
            lstLog.Location = new Point(9, 244);
            lstLog.Name = "lstLog";
            lstLog.Size = new Size(253, 160);
            lstLog.TabIndex = 0;
            // 
            // txtRatioX
            // 
            txtRatioX.Location = new Point(73, 92);
            txtRatioX.Name = "txtRatioX";
            txtRatioX.Size = new Size(26, 23);
            txtRatioX.TabIndex = 4;
            txtRatioX.Text = "1.0";
            // 
            // txtRatioY
            // 
            txtRatioY.Location = new Point(151, 92);
            txtRatioY.Name = "txtRatioY";
            txtRatioY.Size = new Size(26, 23);
            txtRatioY.TabIndex = 5;
            txtRatioY.Text = "1.0";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(31, 95);
            label1.Name = "label1";
            label1.Size = new Size(28, 15);
            label1.TabIndex = 6;
            label1.Text = "X %";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(107, 95);
            label2.Name = "label2";
            label2.Size = new Size(28, 15);
            label2.TabIndex = 7;
            label2.Text = "Y %";
            // 
            // txtRepeat
            // 
            txtRepeat.Location = new Point(92, 121);
            txtRepeat.Name = "txtRepeat";
            txtRepeat.Size = new Size(39, 23);
            txtRepeat.TabIndex = 8;
            txtRepeat.Text = "10";
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Location = new Point(5, 9);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(261, 200);
            tabControl1.TabIndex = 9;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(lblRole);
            tabPage1.Controls.Add(btnToggleRole);
            tabPage1.Controls.Add(label2);
            tabPage1.Controls.Add(btnConnect);
            tabPage1.Controls.Add(label1);
            tabPage1.Controls.Add(txtRatioY);
            tabPage1.Controls.Add(txtRatioX);
            tabPage1.Controls.Add(txtTargetIP);
            tabPage1.Controls.Add(label4);
            tabPage1.Controls.Add(label3);
            tabPage1.Controls.Add(txtMyIP);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(253, 172);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Main";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // lblRole
            // 
            lblRole.AutoSize = true;
            lblRole.Location = new Point(160, 11);
            lblRole.Name = "lblRole";
            lblRole.Size = new Size(59, 15);
            lblRole.TabIndex = 19;
            lblRole.Text = "MODE: --";
            // 
            // btnToggleRole
            // 
            btnToggleRole.Location = new Point(151, 57);
            btnToggleRole.Name = "btnToggleRole";
            btnToggleRole.Size = new Size(75, 23);
            btnToggleRole.TabIndex = 16;
            btnToggleRole.Text = "역할 전환";
            btnToggleRole.UseVisualStyleBackColor = true;
            btnToggleRole.Click += btnToggleRole_Click;
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(8, 57);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(127, 23);
            btnConnect.TabIndex = 15;
            btnConnect.Text = "연결 / 대기";
            btnConnect.UseVisualStyleBackColor = true;
            btnConnect.Click += btnConnect_Click;
            // 
            // txtTargetIP
            // 
            txtTargetIP.Location = new Point(64, 28);
            txtTargetIP.Name = "txtTargetIP";
            txtTargetIP.Size = new Size(100, 23);
            txtTargetIP.TabIndex = 13;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(8, 31);
            label4.Name = "label4";
            label4.Size = new Size(50, 15);
            label4.TabIndex = 12;
            label4.Text = "TargetIP";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(8, 11);
            label3.Name = "label3";
            label3.Size = new Size(34, 15);
            label3.TabIndex = 11;
            label3.Text = "MyIP";
            // 
            // txtMyIP
            // 
            txtMyIP.AutoSize = true;
            txtMyIP.Location = new Point(64, 11);
            txtMyIP.Name = "txtMyIP";
            txtMyIP.Size = new Size(39, 15);
            txtMyIP.TabIndex = 10;
            txtMyIP.Text = "label3";
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(chkSubManual);
            tabPage2.Controls.Add(txtSyncDist);
            tabPage2.Controls.Add(chkSyncCagoLV);
            tabPage2.Controls.Add(label16);
            tabPage2.Controls.Add(label13);
            tabPage2.Controls.Add(label15);
            tabPage2.Controls.Add(label7);
            tabPage2.Controls.Add(label14);
            tabPage2.Controls.Add(label6);
            tabPage2.Controls.Add(txtWaitNext);
            tabPage2.Controls.Add(label5);
            tabPage2.Controls.Add(chkCaptureEnable);
            tabPage2.Controls.Add(picUp);
            tabPage2.Controls.Add(txtTolerance);
            tabPage2.Controls.Add(txtWaitMatch);
            tabPage2.Controls.Add(picConfirm);
            tabPage2.Controls.Add(txtRepeat);
            tabPage2.Controls.Add(picDown);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(253, 172);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "CagoWork";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // chkSubManual
            // 
            chkSubManual.AutoSize = true;
            chkSubManual.Font = new Font("맑은 고딕", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 129);
            chkSubManual.Location = new Point(129, 65);
            chkSubManual.Name = "chkSubManual";
            chkSubManual.Size = new Size(117, 17);
            chkSubManual.TabIndex = 25;
            chkSubManual.Text = "Sub Manual MOD";
            chkSubManual.UseVisualStyleBackColor = true;
            // 
            // txtSyncDist
            // 
            txtSyncDist.Location = new Point(220, 81);
            txtSyncDist.Name = "txtSyncDist";
            txtSyncDist.Size = new Size(29, 23);
            txtSyncDist.TabIndex = 24;
            txtSyncDist.Text = "5";
            // 
            // chkSyncCagoLV
            // 
            chkSyncCagoLV.AutoSize = true;
            chkSyncCagoLV.Font = new Font("맑은 고딕", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 129);
            chkSyncCagoLV.Location = new Point(131, 84);
            chkSyncCagoLV.Name = "chkSyncCagoLV";
            chkSyncCagoLV.Size = new Size(87, 17);
            chkSyncCagoLV.TabIndex = 23;
            chkSyncCagoLV.Text = "SyncCagoLV";
            chkSyncCagoLV.UseVisualStyleBackColor = true;
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Location = new Point(182, 47);
            label16.Name = "label16";
            label16.Size = new Size(40, 15);
            label16.TabIndex = 22;
            label16.Text = "btnUp";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(8, 147);
            label13.Name = "label13";
            label13.Size = new Size(80, 15);
            label13.TabIndex = 16;
            label13.Text = "WaitNextTurn";
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new Point(93, 47);
            label15.Name = "label15";
            label15.Size = new Size(69, 15);
            label15.TabIndex = 21;
            label15.Text = "btnConfirm";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(8, 124);
            label7.Name = "label7";
            label7.Size = new Size(43, 15);
            label7.TabIndex = 15;
            label7.Text = "Repeat";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(22, 47);
            label14.Name = "label14";
            label14.Size = new Size(57, 15);
            label14.TabIndex = 17;
            label14.Text = "btnDown";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(6, 104);
            label6.Name = "label6";
            label6.Size = new Size(58, 15);
            label6.TabIndex = 14;
            label6.Text = "Tolerance";
            // 
            // txtWaitNext
            // 
            txtWaitNext.Location = new Point(92, 144);
            txtWaitNext.Name = "txtWaitNext";
            txtWaitNext.Size = new Size(39, 23);
            txtWaitNext.TabIndex = 4;
            txtWaitNext.Text = "1500";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(8, 86);
            label5.Name = "label5";
            label5.Size = new Size(65, 15);
            label5.TabIndex = 13;
            label5.Text = "WaitMatch";
            // 
            // chkCaptureEnable
            // 
            chkCaptureEnable.AutoSize = true;
            chkCaptureEnable.Location = new Point(12, 65);
            chkCaptureEnable.Name = "chkCaptureEnable";
            chkCaptureEnable.Size = new Size(78, 19);
            chkCaptureEnable.TabIndex = 10;
            chkCaptureEnable.Text = "순차 캡쳐";
            chkCaptureEnable.UseVisualStyleBackColor = true;
            // 
            // picUp
            // 
            picUp.BorderStyle = BorderStyle.FixedSingle;
            picUp.Location = new Point(186, 14);
            picUp.Name = "picUp";
            picUp.Size = new Size(34, 30);
            picUp.SizeMode = PictureBoxSizeMode.StretchImage;
            picUp.TabIndex = 12;
            picUp.TabStop = false;
            // 
            // txtTolerance
            // 
            txtTolerance.Location = new Point(92, 101);
            txtTolerance.Name = "txtTolerance";
            txtTolerance.Size = new Size(39, 23);
            txtTolerance.TabIndex = 9;
            txtTolerance.Text = "15";
            // 
            // txtWaitMatch
            // 
            txtWaitMatch.Location = new Point(92, 83);
            txtWaitMatch.Name = "txtWaitMatch";
            txtWaitMatch.Size = new Size(39, 23);
            txtWaitMatch.TabIndex = 2;
            txtWaitMatch.Text = "3000";
            // 
            // picConfirm
            // 
            picConfirm.BorderStyle = BorderStyle.FixedSingle;
            picConfirm.Location = new Point(110, 14);
            picConfirm.Name = "picConfirm";
            picConfirm.Size = new Size(34, 30);
            picConfirm.SizeMode = PictureBoxSizeMode.StretchImage;
            picConfirm.TabIndex = 11;
            picConfirm.TabStop = false;
            // 
            // picDown
            // 
            picDown.BorderStyle = BorderStyle.FixedSingle;
            picDown.Location = new Point(34, 14);
            picDown.Name = "picDown";
            picDown.Size = new Size(34, 30);
            picDown.SizeMode = PictureBoxSizeMode.StretchImage;
            picDown.TabIndex = 10;
            picDown.TabStop = false;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(txtWaitEsc);
            tabPage3.Controls.Add(label12);
            tabPage3.Controls.Add(label11);
            tabPage3.Controls.Add(txtWaitUp);
            tabPage3.Controls.Add(label10);
            tabPage3.Controls.Add(label9);
            tabPage3.Controls.Add(label8);
            tabPage3.Controls.Add(btnSaveConfig);
            tabPage3.Controls.Add(txtWaitConfirm);
            tabPage3.Controls.Add(txtWaitClick);
            tabPage3.Controls.Add(txtWaitAction);
            tabPage3.Location = new Point(4, 24);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new Size(253, 172);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Setting";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // txtWaitEsc
            // 
            txtWaitEsc.Location = new Point(128, 88);
            txtWaitEsc.Name = "txtWaitEsc";
            txtWaitEsc.Size = new Size(39, 23);
            txtWaitEsc.TabIndex = 8;
            txtWaitEsc.Text = "7500";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(29, 115);
            label12.Name = "label12";
            label12.Size = new Size(41, 15);
            label12.TabIndex = 5;
            label12.Text = "Wait F";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(29, 91);
            label11.Name = "label11";
            label11.Size = new Size(59, 15);
            label11.TabIndex = 4;
            label11.Text = "Wait click";
            // 
            // txtWaitUp
            // 
            txtWaitUp.Location = new Point(128, 64);
            txtWaitUp.Name = "txtWaitUp";
            txtWaitUp.Size = new Size(39, 23);
            txtWaitUp.TabIndex = 7;
            txtWaitUp.Text = "7800";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(29, 67);
            label10.Name = "label10";
            label10.Size = new Size(46, 15);
            label10.TabIndex = 3;
            label10.Text = "WaitUp";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(29, 44);
            label9.Name = "label9";
            label9.Size = new Size(57, 15);
            label9.TabIndex = 2;
            label9.Text = "WaitClick";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(29, 20);
            label8.Name = "label8";
            label8.Size = new Size(75, 15);
            label8.TabIndex = 1;
            label8.Text = "WaitConfirm";
            // 
            // btnSaveConfig
            // 
            btnSaveConfig.Location = new Point(92, 141);
            btnSaveConfig.Name = "btnSaveConfig";
            btnSaveConfig.Size = new Size(75, 23);
            btnSaveConfig.TabIndex = 0;
            btnSaveConfig.Text = "SAVE";
            btnSaveConfig.UseVisualStyleBackColor = true;
            btnSaveConfig.Click += btnSaveConfig_Click;
            // 
            // txtWaitConfirm
            // 
            txtWaitConfirm.Location = new Point(128, 17);
            txtWaitConfirm.Name = "txtWaitConfirm";
            txtWaitConfirm.Size = new Size(39, 23);
            txtWaitConfirm.TabIndex = 5;
            txtWaitConfirm.Text = "500";
            // 
            // txtWaitClick
            // 
            txtWaitClick.Location = new Point(128, 41);
            txtWaitClick.Name = "txtWaitClick";
            txtWaitClick.Size = new Size(39, 23);
            txtWaitClick.TabIndex = 6;
            txtWaitClick.Text = "50";
            // 
            // txtWaitAction
            // 
            txtWaitAction.Location = new Point(128, 112);
            txtWaitAction.Name = "txtWaitAction";
            txtWaitAction.Size = new Size(39, 23);
            txtWaitAction.TabIndex = 3;
            txtWaitAction.Text = "500";
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Font = new Font("맑은 고딕", 8.25F);
            lblStatus.Location = new Point(219, 216);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(37, 13);
            lblStatus.TabIndex = 17;
            lblStatus.Text = "signal";
            // 
            // chkAlwaysOnTop
            // 
            chkAlwaysOnTop.AutoSize = true;
            chkAlwaysOnTop.Font = new Font("맑은 고딕", 8.25F);
            chkAlwaysOnTop.Location = new Point(9, 215);
            chkAlwaysOnTop.Name = "chkAlwaysOnTop";
            chkAlwaysOnTop.Size = new Size(61, 17);
            chkAlwaysOnTop.TabIndex = 14;
            chkAlwaysOnTop.Text = "OnTop";
            chkAlwaysOnTop.UseVisualStyleBackColor = true;
            // 
            // chkSoundEnable
            // 
            chkSoundEnable.AutoSize = true;
            chkSoundEnable.Font = new Font("맑은 고딕", 8.25F);
            chkSoundEnable.Location = new Point(76, 215);
            chkSoundEnable.Name = "chkSoundEnable";
            chkSoundEnable.Size = new Size(58, 17);
            chkSoundEnable.TabIndex = 19;
            chkSoundEnable.Text = "Sound";
            chkSoundEnable.UseVisualStyleBackColor = true;
            // 
            // chkAntiKick
            // 
            chkAntiKick.AutoSize = true;
            chkAntiKick.Font = new Font("맑은 고딕", 8.25F);
            chkAntiKick.Location = new Point(140, 215);
            chkAntiKick.Name = "chkAntiKick";
            chkAntiKick.Size = new Size(66, 17);
            chkAntiKick.TabIndex = 20;
            chkAntiKick.Text = "AntiKick";
            chkAntiKick.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(270, 416);
            Controls.Add(chkAntiKick);
            Controls.Add(lblStatus);
            Controls.Add(tabControl1);
            Controls.Add(chkSoundEnable);
            Controls.Add(lstLog);
            Controls.Add(chkAlwaysOnTop);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picUp).EndInit();
            ((System.ComponentModel.ISupportInitialize)picConfirm).EndInit();
            ((System.ComponentModel.ISupportInitialize)picDown).EndInit();
            tabPage3.ResumeLayout(false);
            tabPage3.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox lstLog;
        private TextBox txtRatioX;
        private TextBox txtRatioY;
        private Label label1;
        private Label label2;
        private TextBox txtRepeat;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private Label label3;
        private Label txtMyIP;
        private CheckBox chkAlwaysOnTop;
        private TextBox txtTargetIP;
        private Label label4;
        private Button btnToggleRole;
        private Button btnConnect;
        private Label lblStatus;
        private Button btnSaveConfig;
        private TextBox txtTolerance;
        private TextBox txtWaitNext;
        private TextBox txtWaitAction;
        private TextBox txtWaitMatch;
        private TextBox txtWaitClick;
        private TextBox txtWaitConfirm;
        private TextBox txtWaitEsc;
        private TextBox txtWaitUp;
        private PictureBox picUp;
        private PictureBox picConfirm;
        private PictureBox picDown;
        private CheckBox chkCaptureEnable;
        private CheckBox chkSoundEnable;
        private CheckBox chkAntiKick;
        private Label label5;
        private Label label7;
        private Label label6;
        private Label label12;
        private Label label11;
        private Label label10;
        private Label label9;
        private Label label8;
        private Label label13;
        private Label label14;
        private Label label15;
        private Label label16;
        private TextBox txtSyncDist;
        private CheckBox chkSyncCagoLV;
        private CheckBox chkSubManual;
        private Label lblRole;
    }
}
