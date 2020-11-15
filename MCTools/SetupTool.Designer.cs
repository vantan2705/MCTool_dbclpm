namespace MCTools
{
    partial class SetupTool
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cbbExam = new System.Windows.Forms.ComboBox();
            this.cbbSubject = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnDone = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.jobLoadDataFromDB = new System.ComponentModel.BackgroundWorker();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tb_WorkspacePath = new System.Windows.Forms.TextBox();
            this.btnSelectWorkspacePath = new System.Windows.Forms.Button();
            this.jobLoadExamInfo = new System.ComponentModel.BackgroundWorker();
            this.jobLoadSubjectInfo = new System.ComponentModel.BackgroundWorker();
            this.cbbTemplate = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "Thông tin thi";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Chọn đợt thi";
            // 
            // cbbExam
            // 
            this.cbbExam.FormattingEnabled = true;
            this.cbbExam.Items.AddRange(new object[] {
            "Kỳ thi học kỳ I năm học 2019-2020",
            "Kỳ thi học kỳ II năm học 2019-2020",
            "Kỳ thi học kỳ I năm học 2020-2021"});
            this.cbbExam.Location = new System.Drawing.Point(134, 53);
            this.cbbExam.Name = "cbbExam";
            this.cbbExam.Size = new System.Drawing.Size(315, 21);
            this.cbbExam.TabIndex = 2;
            this.cbbExam.SelectedIndexChanged += new System.EventHandler(this.cbbExam_SelectedIndexChanged);
            // 
            // cbbSubject
            // 
            this.cbbSubject.FormattingEnabled = true;
            this.cbbSubject.Location = new System.Drawing.Point(134, 97);
            this.cbbSubject.Name = "cbbSubject";
            this.cbbSubject.Size = new System.Drawing.Size(315, 21);
            this.cbbSubject.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 100);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Chọn môn thi";
            // 
            // btnDone
            // 
            this.btnDone.Location = new System.Drawing.Point(293, 280);
            this.btnDone.Name = "btnDone";
            this.btnDone.Size = new System.Drawing.Size(75, 23);
            this.btnDone.TabIndex = 5;
            this.btnDone.Text = "Xác nhận";
            this.btnDone.UseVisualStyleBackColor = true;
            this.btnDone.Click += new System.EventHandler(this.btnDone_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(374, 280);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Hủy bỏ";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // jobLoadDataFromDB
            // 
            this.jobLoadDataFromDB.DoWork += new System.ComponentModel.DoWorkEventHandler(this.jobLoadDataFromDB_DoWork);
            this.jobLoadDataFromDB.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.jobLoadDataFromDB_RunWorkerCompleted);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 178);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(102, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Đường dẫn làm việc";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(12, 135);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(190, 24);
            this.label5.TabIndex = 8;
            this.label5.Text = "Thông tin chấm bài";
            // 
            // tb_WorkspacePath
            // 
            this.tb_WorkspacePath.Enabled = false;
            this.tb_WorkspacePath.Location = new System.Drawing.Point(134, 175);
            this.tb_WorkspacePath.Name = "tb_WorkspacePath";
            this.tb_WorkspacePath.Size = new System.Drawing.Size(208, 20);
            this.tb_WorkspacePath.TabIndex = 9;
            // 
            // btnSelectWorkspacePath
            // 
            this.btnSelectWorkspacePath.Location = new System.Drawing.Point(348, 173);
            this.btnSelectWorkspacePath.Name = "btnSelectWorkspacePath";
            this.btnSelectWorkspacePath.Size = new System.Drawing.Size(101, 23);
            this.btnSelectWorkspacePath.TabIndex = 10;
            this.btnSelectWorkspacePath.Text = "Chọn đường dẫn";
            this.btnSelectWorkspacePath.UseVisualStyleBackColor = true;
            this.btnSelectWorkspacePath.Click += new System.EventHandler(this.btnSelectWorkspacePath_Click);
            // 
            // jobLoadExamInfo
            // 
            this.jobLoadExamInfo.DoWork += new System.ComponentModel.DoWorkEventHandler(this.jobLoadExamInfo_DoWork);
            this.jobLoadExamInfo.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.jobLoadExamInfo_RunWorkerCompleted);
            // 
            // jobLoadSubjectInfo
            // 
            this.jobLoadSubjectInfo.DoWork += new System.ComponentModel.DoWorkEventHandler(this.jobLoadSubjectInfo_DoWork);
            // 
            // cbbTemplate
            // 
            this.cbbTemplate.FormattingEnabled = true;
            this.cbbTemplate.Items.AddRange(new object[] {
            "MC.TEMPLATE45Q"});
            this.cbbTemplate.Location = new System.Drawing.Point(134, 214);
            this.cbbTemplate.Name = "cbbTemplate";
            this.cbbTemplate.Size = new System.Drawing.Size(315, 21);
            this.cbbTemplate.TabIndex = 12;
            this.cbbTemplate.SelectedIndexChanged += new System.EventHandler(this.cbbTemplate_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(20, 217);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Chọn mẫu";
            // 
            // SetupTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 315);
            this.Controls.Add(this.cbbTemplate);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnSelectWorkspacePath);
            this.Controls.Add(this.tb_WorkspacePath);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnDone);
            this.Controls.Add(this.cbbSubject);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbbExam);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SetupTool";
            this.Text = "Cấu hình chấm thi";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbbExam;
        private System.Windows.Forms.ComboBox cbbSubject;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnDone;
        private System.Windows.Forms.Button btnCancel;
        private System.ComponentModel.BackgroundWorker jobLoadDataFromDB;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tb_WorkspacePath;
        private System.Windows.Forms.Button btnSelectWorkspacePath;
        private System.ComponentModel.BackgroundWorker jobLoadExamInfo;
        private System.ComponentModel.BackgroundWorker jobLoadSubjectInfo;
        private System.Windows.Forms.ComboBox cbbTemplate;
        private System.Windows.Forms.Label label6;
    }
}