namespace ModBusSlave
{
    partial class Form1
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
            this.btnStart = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.grpData = new System.Windows.Forms.GroupBox();
            this.lbl_RegType = new System.Windows.Forms.Label();
            this.cboRegType = new System.Windows.Forms.ComboBox();
            this.lbl_RegAddr = new System.Windows.Forms.Label();
            this.txtRegAdr = new System.Windows.Forms.TextBox();
            this.txtRegVal = new System.Windows.Forms.TextBox();
            this.lbl_newValue = new System.Windows.Forms.Label();
            this.btnSetVal = new System.Windows.Forms.Button();
            this.grpData.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStart.Location = new System.Drawing.Point(12, 26);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(335, 66);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "START";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(12, 429);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(35, 12);
            this.lblStatus.TabIndex = 1;
            this.lblStatus.Text = "Status:";
            // 
            // grpData
            // 
            this.grpData.Controls.Add(this.btnSetVal);
            this.grpData.Controls.Add(this.txtRegVal);
            this.grpData.Controls.Add(this.lbl_newValue);
            this.grpData.Controls.Add(this.txtRegAdr);
            this.grpData.Controls.Add(this.lbl_RegAddr);
            this.grpData.Controls.Add(this.cboRegType);
            this.grpData.Controls.Add(this.lbl_RegType);
            this.grpData.Location = new System.Drawing.Point(14, 113);
            this.grpData.Name = "grpData";
            this.grpData.Size = new System.Drawing.Size(333, 295);
            this.grpData.TabIndex = 2;
            this.grpData.TabStop = false;
            this.grpData.Text = "Change Data";
            // 
            // lbl_RegType
            // 
            this.lbl_RegType.AutoSize = true;
            this.lbl_RegType.Location = new System.Drawing.Point(19, 26);
            this.lbl_RegType.Name = "lbl_RegType";
            this.lbl_RegType.Size = new System.Drawing.Size(70, 12);
            this.lbl_RegType.TabIndex = 0;
            this.lbl_RegType.Text = "Register Type";
            // 
            // cboRegType
            // 
            this.cboRegType.FormattingEnabled = true;
            this.cboRegType.Items.AddRange(new object[] {
            "Holding Register",
            "Input Register",
            "Digital Input",
            "Coil Output"});
            this.cboRegType.Location = new System.Drawing.Point(117, 24);
            this.cboRegType.Name = "cboRegType";
            this.cboRegType.Size = new System.Drawing.Size(121, 20);
            this.cboRegType.TabIndex = 1;
            // 
            // lbl_RegAddr
            // 
            this.lbl_RegAddr.AutoSize = true;
            this.lbl_RegAddr.Location = new System.Drawing.Point(19, 60);
            this.lbl_RegAddr.Name = "lbl_RegAddr";
            this.lbl_RegAddr.Size = new System.Drawing.Size(83, 12);
            this.lbl_RegAddr.TabIndex = 2;
            this.lbl_RegAddr.Text = "Register Address";
            // 
            // txtRegAdr
            // 
            this.txtRegAdr.Location = new System.Drawing.Point(117, 50);
            this.txtRegAdr.Name = "txtRegAdr";
            this.txtRegAdr.Size = new System.Drawing.Size(121, 22);
            this.txtRegAdr.TabIndex = 3;
            // 
            // txtRegVal
            // 
            this.txtRegVal.Location = new System.Drawing.Point(117, 78);
            this.txtRegVal.Name = "txtRegVal";
            this.txtRegVal.Size = new System.Drawing.Size(121, 22);
            this.txtRegVal.TabIndex = 5;
            // 
            // lbl_newValue
            // 
            this.lbl_newValue.AutoSize = true;
            this.lbl_newValue.Location = new System.Drawing.Point(19, 81);
            this.lbl_newValue.Name = "lbl_newValue";
            this.lbl_newValue.Size = new System.Drawing.Size(56, 12);
            this.lbl_newValue.TabIndex = 4;
            this.lbl_newValue.Text = "New Value";
            // 
            // btnSetVal
            // 
            this.btnSetVal.Location = new System.Drawing.Point(117, 118);
            this.btnSetVal.Name = "btnSetVal";
            this.btnSetVal.Size = new System.Drawing.Size(121, 23);
            this.btnSetVal.TabIndex = 6;
            this.btnSetVal.Text = "Set";
            this.btnSetVal.UseVisualStyleBackColor = true;
            this.btnSetVal.Click += new System.EventHandler(this.btnSetVal_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(359, 450);
            this.Controls.Add(this.grpData);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnStart);
            this.Name = "Form1";
            this.Text = "Modbus TCP Server";
            this.grpData.ResumeLayout(false);
            this.grpData.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.GroupBox grpData;
        private System.Windows.Forms.Button btnSetVal;
        private System.Windows.Forms.TextBox txtRegVal;
        private System.Windows.Forms.Label lbl_newValue;
        private System.Windows.Forms.TextBox txtRegAdr;
        private System.Windows.Forms.Label lbl_RegAddr;
        private System.Windows.Forms.ComboBox cboRegType;
        private System.Windows.Forms.Label lbl_RegType;
    }
}

