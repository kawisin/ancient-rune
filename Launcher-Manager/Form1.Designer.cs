namespace Launcher_Manager
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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            dataGridView1 = new DataGridView();
            InstanceName = new DataGridViewTextBoxColumn();
            InstanceId = new DataGridViewTextBoxColumn();
            Port = new DataGridViewTextBoxColumn();
            PlayerCount = new DataGridViewTextBoxColumn();
            CpuUsage = new DataGridViewTextBoxColumn();
            RamUsage = new DataGridViewTextBoxColumn();
            StartTime = new DataGridViewTextBoxColumn();
            EndTime = new DataGridViewTextBoxColumn();
            richTextBox1 = new RichTextBox();
            folderBrowserDialog1 = new FolderBrowserDialog();
            StartServer = new Button();
            KillServer = new Button();
            ServerMoniter = new RichTextBox();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.BackgroundColor = SystemColors.GradientInactiveCaption;
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = SystemColors.ActiveBorder;
            dataGridViewCellStyle1.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { InstanceName, InstanceId, Port, PlayerCount, CpuUsage, RamUsage, StartTime, EndTime });
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = SystemColors.InactiveBorder;
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            dataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
            dataGridView1.GridColor = SystemColors.InactiveCaption;
            dataGridView1.Location = new Point(12, 157);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.RowTemplate.Height = 25;
            dataGridView1.Size = new Size(798, 310);
            dataGridView1.TabIndex = 0;
            dataGridView1.CellContentClick += dataGridView1_CellContentClick;
            // 
            // InstanceName
            // 
            InstanceName.HeaderText = "InstanceId";
            InstanceName.Name = "InstanceName";
            // 
            // InstanceId
            // 
            InstanceId.HeaderText = "InstanceName";
            InstanceId.Name = "InstanceId";
            // 
            // Port
            // 
            Port.HeaderText = "Port";
            Port.Name = "Port";
            // 
            // PlayerCount
            // 
            PlayerCount.HeaderText = "PlayerCount";
            PlayerCount.Name = "PlayerCount";
            // 
            // CpuUsage
            // 
            CpuUsage.HeaderText = "CPU Usage";
            CpuUsage.Name = "CpuUsage";
            // 
            // RamUsage
            // 
            RamUsage.HeaderText = "Ram Usage";
            RamUsage.Name = "RamUsage";
            // 
            // StartTime
            // 
            StartTime.HeaderText = "Start Time";
            StartTime.Name = "StartTime";
            // 
            // EndTime
            // 
            EndTime.HeaderText = "End Time";
            EndTime.Name = "EndTime";
            // 
            // richTextBox1
            // 
            richTextBox1.BackColor = SystemColors.InactiveCaption;
            richTextBox1.Location = new Point(12, 12);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(440, 139);
            richTextBox1.TabIndex = 1;
            richTextBox1.Text = "";
            // 
            // StartServer
            // 
            StartServer.BackColor = Color.DarkSeaGreen;
            StartServer.Cursor = Cursors.Hand;
            StartServer.FlatStyle = FlatStyle.System;
            StartServer.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            StartServer.Location = new Point(666, 12);
            StartServer.Name = "StartServer";
            StartServer.Size = new Size(144, 35);
            StartServer.TabIndex = 2;
            StartServer.Text = "Start - Server";
            StartServer.UseVisualStyleBackColor = false;
            StartServer.Click += StartServer_Click;
            // 
            // KillServer
            // 
            KillServer.BackColor = Color.LightCoral;
            KillServer.Cursor = Cursors.Hand;
            KillServer.FlatStyle = FlatStyle.System;
            KillServer.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            KillServer.Location = new Point(666, 53);
            KillServer.Name = "KillServer";
            KillServer.Size = new Size(144, 35);
            KillServer.TabIndex = 3;
            KillServer.Text = "Kill - Server";
            KillServer.UseVisualStyleBackColor = false;
            KillServer.Click += KillServer_ClickAsync;
            // 
            // ServerMoniter
            // 
            ServerMoniter.BackColor = SystemColors.InactiveCaption;
            ServerMoniter.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            ServerMoniter.Location = new Point(458, 12);
            ServerMoniter.Name = "ServerMoniter";
            ServerMoniter.Size = new Size(202, 139);
            ServerMoniter.TabIndex = 4;
            ServerMoniter.Text = "";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaption;
            ClientSize = new Size(822, 479);
            Controls.Add(ServerMoniter);
            Controls.Add(KillServer);
            Controls.Add(StartServer);
            Controls.Add(richTextBox1);
            Controls.Add(dataGridView1);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "Form1";
            Text = "Launcher Service";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
        }

        private System.Windows.Forms.Timer refreshTimer;

        #endregion

        private DataGridView dataGridView1;
        private RichTextBox richTextBox1;
        private FolderBrowserDialog folderBrowserDialog1;
        private Button StartServer;
        private Button KillServer;
        private DataGridViewTextBoxColumn InstanceName;
        private DataGridViewTextBoxColumn InstanceId;
        private DataGridViewTextBoxColumn Port;
        private DataGridViewTextBoxColumn PlayerCount;
        private DataGridViewTextBoxColumn CpuUsage;
        private DataGridViewTextBoxColumn RamUsage;
        private DataGridViewTextBoxColumn StartTime;
        private DataGridViewTextBoxColumn EndTime;
        private RichTextBox ServerMoniter;
        //private ListView ServerListView;
    }
}