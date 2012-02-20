namespace Tbx.Utils
{
    partial class ucAmSettings
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label7 = new System.Windows.Forms.Label();
            this.lblMb = new System.Windows.Forms.Label();
            this.txtThreshold = new System.Windows.Forms.NumericUpDown();
            this.chkUseAttachMngt = new System.Windows.Forms.CheckBox();
            this.lblThreshold = new System.Windows.Forms.Label();
            this.radAMAlwaysAsk = new System.Windows.Forms.RadioButton();
            this.grpAttachmentManagement = new System.Windows.Forms.GroupBox();
            this.picThreshold = new System.Windows.Forms.PictureBox();
            this.picHelpExpiry = new System.Windows.Forms.PictureBox();
            this.txtCustomVal = new NullFX.Controls.NumericTextBox();
            this.cboCustomExpiryUnit = new System.Windows.Forms.ComboBox();
            this.cboGenExpirySetting = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.radAMAlwaysUse = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.txtThreshold)).BeginInit();
            this.grpAttachmentManagement.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picHelpExpiry)).BeginInit();
            this.SuspendLayout();
            // 
            // label7
            // 
            this.label7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label7.Location = new System.Drawing.Point(12, 36);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(218, 15);
            this.label7.TabIndex = 4;
            this.label7.Text = "Do the following:";
            // 
            // lblMb
            // 
            this.lblMb.AutoSize = true;
            this.lblMb.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblMb.Location = new System.Drawing.Point(286, 19);
            this.lblMb.Name = "lblMb";
            this.lblMb.Size = new System.Drawing.Size(22, 13);
            this.lblMb.TabIndex = 3;
            this.lblMb.Text = "Mb";
            // 
            // txtThreshold
            // 
            this.txtThreshold.Location = new System.Drawing.Point(238, 15);
            this.txtThreshold.Maximum = new decimal(new int[] {
            -1486618625,
            232830643,
            0,
            0});
            this.txtThreshold.Name = "txtThreshold";
            this.txtThreshold.Size = new System.Drawing.Size(46, 20);
            this.txtThreshold.TabIndex = 1;
            this.txtThreshold.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtThreshold.ValueChanged += new System.EventHandler(this.txtThreshold_ValueChanged);
            // 
            // chkUseAttachMngt
            // 
            this.chkUseAttachMngt.AutoSize = true;
            this.chkUseAttachMngt.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chkUseAttachMngt.Location = new System.Drawing.Point(4, 5);
            this.chkUseAttachMngt.Name = "chkUseAttachMngt";
            this.chkUseAttachMngt.Size = new System.Drawing.Size(231, 17);
            this.chkUseAttachMngt.TabIndex = 11;
            this.chkUseAttachMngt.Text = "Enable Teambox Attachment Management.";
            this.chkUseAttachMngt.UseVisualStyleBackColor = true;
            this.chkUseAttachMngt.CheckedChanged += new System.EventHandler(this.chkUseAttachMngt_CheckedChanged);
            // 
            // lblThreshold
            // 
            this.lblThreshold.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblThreshold.Location = new System.Drawing.Point(12, 18);
            this.lblThreshold.Name = "lblThreshold";
            this.lblThreshold.Size = new System.Drawing.Size(218, 15);
            this.lblThreshold.TabIndex = 0;
            this.lblThreshold.Text = "When an outbound email reaches this size:";
            // 
            // radAMAlwaysAsk
            // 
            this.radAMAlwaysAsk.AutoSize = true;
            this.radAMAlwaysAsk.Checked = true;
            this.radAMAlwaysAsk.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radAMAlwaysAsk.Location = new System.Drawing.Point(29, 54);
            this.radAMAlwaysAsk.Name = "radAMAlwaysAsk";
            this.radAMAlwaysAsk.Size = new System.Drawing.Size(156, 17);
            this.radAMAlwaysAsk.TabIndex = 2;
            this.radAMAlwaysAsk.TabStop = true;
            this.radAMAlwaysAsk.Text = "Prompt for your confirmation";
            this.radAMAlwaysAsk.UseVisualStyleBackColor = true;
            this.radAMAlwaysAsk.CheckedChanged += new System.EventHandler(this.radAMAlwaysAsk_CheckedChanged);
            // 
            // grpAttachmentManagement
            // 
            this.grpAttachmentManagement.Controls.Add(this.picThreshold);
            this.grpAttachmentManagement.Controls.Add(this.picHelpExpiry);
            this.grpAttachmentManagement.Controls.Add(this.txtCustomVal);
            this.grpAttachmentManagement.Controls.Add(this.cboCustomExpiryUnit);
            this.grpAttachmentManagement.Controls.Add(this.cboGenExpirySetting);
            this.grpAttachmentManagement.Controls.Add(this.label1);
            this.grpAttachmentManagement.Controls.Add(this.label7);
            this.grpAttachmentManagement.Controls.Add(this.lblMb);
            this.grpAttachmentManagement.Controls.Add(this.lblThreshold);
            this.grpAttachmentManagement.Controls.Add(this.txtThreshold);
            this.grpAttachmentManagement.Controls.Add(this.radAMAlwaysAsk);
            this.grpAttachmentManagement.Controls.Add(this.radAMAlwaysUse);
            this.grpAttachmentManagement.Enabled = false;
            this.grpAttachmentManagement.Location = new System.Drawing.Point(26, 28);
            this.grpAttachmentManagement.Name = "grpAttachmentManagement";
            this.grpAttachmentManagement.Size = new System.Drawing.Size(339, 150);
            this.grpAttachmentManagement.TabIndex = 10;
            this.grpAttachmentManagement.TabStop = false;
            // 
            // picThreshold
            // 
            this.picThreshold.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.picThreshold.Image = global::TbxUtils.Properties.Resources.help_16x16;
            this.picThreshold.Location = new System.Drawing.Point(317, 19);
            this.picThreshold.Name = "picThreshold";
            this.picThreshold.Size = new System.Drawing.Size(16, 16);
            this.picThreshold.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picThreshold.TabIndex = 12;
            this.picThreshold.TabStop = false;
            this.picThreshold.Click += new System.EventHandler(this.picThreshold_Click);
            // 
            // picHelpExpiry
            // 
            this.picHelpExpiry.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.picHelpExpiry.Image = global::TbxUtils.Properties.Resources.help_16x16;
            this.picHelpExpiry.Location = new System.Drawing.Point(317, 101);
            this.picHelpExpiry.Name = "picHelpExpiry";
            this.picHelpExpiry.Size = new System.Drawing.Size(16, 16);
            this.picHelpExpiry.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picHelpExpiry.TabIndex = 11;
            this.picHelpExpiry.TabStop = false;
            this.picHelpExpiry.Click += new System.EventHandler(this.picHelpExpiry_click);
            // 
            // txtCustomVal
            // 
            this.txtCustomVal.Location = new System.Drawing.Point(144, 118);
            this.txtCustomVal.Name = "txtCustomVal";
            this.txtCustomVal.Size = new System.Drawing.Size(41, 20);
            this.txtCustomVal.TabIndex = 10;
            this.txtCustomVal.TextChanged += new System.EventHandler(this.txtCustomVal_TextChanged);
            // 
            // cboCustomExpiryUnit
            // 
            this.cboCustomExpiryUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCustomExpiryUnit.FormattingEnabled = true;
            this.cboCustomExpiryUnit.Items.AddRange(new object[] {
            "day(s)",
            "week(s)",
            "month(s)"});
            this.cboCustomExpiryUnit.Location = new System.Drawing.Point(193, 118);
            this.cboCustomExpiryUnit.Name = "cboCustomExpiryUnit";
            this.cboCustomExpiryUnit.Size = new System.Drawing.Size(76, 21);
            this.cboCustomExpiryUnit.TabIndex = 8;
            this.cboCustomExpiryUnit.Visible = false;
            this.cboCustomExpiryUnit.SelectedIndexChanged += new System.EventHandler(this.cboCustomExpiryUnit_SelectedIndexChanged);
            // 
            // cboGenExpirySetting
            // 
            this.cboGenExpirySetting.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboGenExpirySetting.FormattingEnabled = true;
            this.cboGenExpirySetting.Items.AddRange(new object[] {
            "1 week",
            "1 month",
            "No expiration",
            "Custom..."});
            this.cboGenExpirySetting.Location = new System.Drawing.Point(29, 117);
            this.cboGenExpirySetting.Name = "cboGenExpirySetting";
            this.cboGenExpirySetting.Size = new System.Drawing.Size(111, 21);
            this.cboGenExpirySetting.TabIndex = 6;
            this.cboGenExpirySetting.SelectedIndexChanged += new System.EventHandler(this.cboGenExpirySetting_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 101);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(161, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Attachments will be available for:";
            // 
            // radAMAlwaysUse
            // 
            this.radAMAlwaysUse.AutoSize = true;
            this.radAMAlwaysUse.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radAMAlwaysUse.Location = new System.Drawing.Point(29, 77);
            this.radAMAlwaysUse.Name = "radAMAlwaysUse";
            this.radAMAlwaysUse.Size = new System.Drawing.Size(261, 17);
            this.radAMAlwaysUse.TabIndex = 0;
            this.radAMAlwaysUse.Text = "Automatically send the attachments with Teambox";
            this.radAMAlwaysUse.UseVisualStyleBackColor = true;
            // 
            // ucAmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkUseAttachMngt);
            this.Controls.Add(this.grpAttachmentManagement);
            this.Name = "ucAmSettings";
            this.Size = new System.Drawing.Size(386, 188);
            ((System.ComponentModel.ISupportInitialize)(this.txtThreshold)).EndInit();
            this.grpAttachmentManagement.ResumeLayout(false);
            this.grpAttachmentManagement.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picHelpExpiry)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblMb;
        private System.Windows.Forms.NumericUpDown txtThreshold;
        private System.Windows.Forms.CheckBox chkUseAttachMngt;
        private System.Windows.Forms.Label lblThreshold;
        private System.Windows.Forms.RadioButton radAMAlwaysAsk;
        private System.Windows.Forms.GroupBox grpAttachmentManagement;
        private System.Windows.Forms.RadioButton radAMAlwaysUse;
        private System.Windows.Forms.ComboBox cboGenExpirySetting;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboCustomExpiryUnit;
        private NullFX.Controls.NumericTextBox txtCustomVal;
        private System.Windows.Forms.PictureBox picHelpExpiry;
        private System.Windows.Forms.PictureBox picThreshold;

    }
}
