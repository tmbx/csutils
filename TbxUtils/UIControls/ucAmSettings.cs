using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace Tbx.Utils
{
    public partial class ucAmSettings : UserControl
    {
        public AmRegistrySettings Settings = AmRegistrySettings.Spawn();
        
        public ucAmSettings()
        {
            InitializeComponent();
            InitUI();
        }

        private void InitUI()
        {
            Debug.Assert(Settings != null);
            chkUseAttachMngt.Checked = Settings.EnableAM;
            txtThreshold.Text = Settings.GetThresholdInMb().ToString();
            radAMAlwaysAsk.Checked = Settings.ManagedAttachmentAsk;
            radAMAlwaysUse.Checked = !radAMAlwaysAsk.Checked;
            cboGenExpirySetting.SelectedIndex = (int)Settings.ExpiryGenSetting;
            txtCustomVal.Text = Settings.ExpiryCustomValue.ToString();
            cboCustomExpiryUnit.SelectedIndex = (int)Settings.ExpiryCustomUnit;
        }

        private void chkUseAttachMngt_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                Settings.EnableAM = chkUseAttachMngt.Checked;
                grpAttachmentManagement.Enabled = chkUseAttachMngt.Checked;                    
            }

            catch (Exception ex)
            {
                Base.HandleException(ex);
            }
        }

        private void txtThreshold_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                Settings.ManagedAttachmentThreshold = Convert.ToUInt64(txtThreshold.Value) * AmRegistrySettings.DefaultThreshold;
                grpAttachmentManagement.Enabled = chkUseAttachMngt.Checked;
            }

            catch (Exception ex)
            {
                Base.HandleException(ex);
            }
        }

        private void cboGenExpirySetting_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                txtCustomVal.Visible = cboGenExpirySetting.SelectedIndex == cboGenExpirySetting.Items.Count - 1;
                cboCustomExpiryUnit.Visible = cboGenExpirySetting.SelectedIndex == cboGenExpirySetting.Items.Count - 1;
                Settings.ExpiryGenSetting = cboGenExpirySetting.SelectedIndex;
            }

            catch (Exception ex)
            {
                Base.HandleException(ex);
            }
        }

        private void cboCustomExpiryUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {                
                Settings.ExpiryCustomUnit = cboCustomExpiryUnit.SelectedIndex;
            }

            catch (Exception ex)
            {
                Base.HandleException(ex);
            }
        }

        private void radAMAlwaysAsk_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                Settings.ManagedAttachmentAsk = radAMAlwaysAsk.Checked;
            }

            catch (Exception ex)
            {
                Base.HandleException(ex);
            }
        }

        private void txtCustomVal_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int val = 0;
                if (Int32.TryParse(txtCustomVal.Text, out val))
                    Settings.ExpiryCustomValue = val;
                else
                    Settings.ExpiryCustomValue = 1;
            }

            catch (Exception ex)
            {
                Base.HandleException(ex);
            }
        }

        private void picHelpExpiry_click(object sender, EventArgs e)
        {
            Base.ShowHelpTooltip("Select when attachments should be purged from the server. When attachments are purged, recipients will not be able to access the files and will get an explanatory message to that effect.", sender as Control);
        }

        private void picThreshold_Click(object sender, EventArgs e)
        {
            Base.ShowHelpTooltip("Threshold activating the Attachment Management module. When the total size of all the attached files of a given email exceeds this value, all the attachments will be sent by the Attachment Management module.", sender as Control);
        }
    }
}
