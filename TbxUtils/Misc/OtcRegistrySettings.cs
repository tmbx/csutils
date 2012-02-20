using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace Tbx.Utils
{
    /// <summary>
    /// Represent the set of settings relevant to the OTC. The default values set here
    /// must be sync'ed with the GetValue calls of UpdateRegistrySettings method.
    /// </summary>
    public class OtcSettings
    {
        /// <summary>
        /// True if the plugin is enabled. If set to false, the user
        /// should not ever be aware that a plugin is installed in his
        /// Outlook.
        /// </summary>
        public bool OtcEnabledFlag = true;

        /// <summary>
        /// True if mails without attached workspaces should have
        /// a SKURL appended. Disabled until further notice. The feature
        /// is no longer handled in the plugin's code, meaning the correct
        /// email footer will NOT be added even though this flag is set to
        /// true.
        /// </summary>
        public bool SkurlEnabledFlag = false;

        /// <summary>
        /// Set to true if a specific registry key is found on startup. When
        /// that key is found, override the log settings set in the KWM.
        /// </summary>
        public bool LogToFileFlag = false;

        /// <summary>
        /// Set to true if the attachment management welcome window
        /// must be hidden on startup.
        /// </summary>
        public bool HideAmWelcomeFlag = false;

        /// <summary>
        /// Attachment Management-specific settings.
        /// </summary>
        public AmRegistrySettings AmSettings;

        /// <summary>
        /// Singleton.
        /// </summary>
        private OtcSettings() { }

        public static OtcSettings Spawn()
        {
            OtcSettings self = new OtcSettings();
            self.AmSettings = AmRegistrySettings.Spawn();
            self.ReadRegistry();
            return self;
        }

        /// <summary>
        /// Reads the registry and update the object's variables. Includes updating the
        /// attachment management specific settings.
        /// </summary>
        public void ReadRegistry()
        {
            RegistryKey regKey = null;
            try
            {
                regKey = Base.GetOtcRegKey();

                int val;

                if (Int32.TryParse(regKey.GetValue("EnableOtc", "1") as String, out val))
                    OtcEnabledFlag = Convert.ToBoolean(val);
                else
                    OtcEnabledFlag = true;

                // First, look up the legacy key name. Use its value if present.
                String skurl = regKey.GetValue("Public_Workspace", "DEFAULT") as String;
                if (skurl != null && skurl != "DEFAULT" && Int32.TryParse(skurl, out val))
                {
                    // Convert to new key name.
                    SkurlEnabledFlag = Convert.ToBoolean(val);
                    regKey.DeleteValue("Public_Workspace", false);
                    regKey.SetValue("EnableSkurl", val.ToString());
                }
                else
                {
                    if (Int32.TryParse(regKey.GetValue("EnableSkurl", "1") as String, out val))
                        SkurlEnabledFlag = Convert.ToBoolean(val);
                    else
                        SkurlEnabledFlag = true;
                }

                if (Int32.TryParse(regKey.GetValue("EnableOtcDebugging", "0") as String, out val))
                {
                    if (val > 0)
                    {
                        LogToFileFlag = true;
                        Logging.Log("Enabling OTC logging.");
                    }
                    else
                    {
                        Logging.Log("Disabling OTC logging.");
                        LogToFileFlag = false;
                    }
                }

                else
                {
                    LogToFileFlag = true;
                }

                if (Int32.TryParse(regKey.GetValue("HideAmWelcomeFlag", "0") as String, out val))
                    HideAmWelcomeFlag = Convert.ToBoolean(val);
                else
                    HideAmWelcomeFlag = false;

                // Update the attachment management settings.
                AmSettings.ReadRegistry();
            }

            finally
            {
                if (regKey != null) regKey.Close();
            }
        }

        /// <summary>
        /// Writes the settings to the registry, including the attachment management 
        /// specific settings.
        /// </summary>
        public void WriteRegistry()
        {
            Logging.Log("Writing OTC settings from the registry.");
            RegistryKey regKey = null;
            try
            {
                regKey = Base.GetOtcRegKey();
                regKey.SetValue("EnableOTC", Convert.ToInt32(OtcEnabledFlag).ToString());
                regKey.SetValue("EnableSkurl", Convert.ToInt32(SkurlEnabledFlag).ToString());
                regKey.SetValue("EnableOtcDebugging", Convert.ToInt32(LogToFileFlag).ToString());
                regKey.SetValue("HideAmWelcomeFlag", Convert.ToInt32(HideAmWelcomeFlag).ToString());

                AmSettings.WriteRegistry();
            }

            catch (Exception ex)
            {
                Base.HandleException(ex, true);
            }
            finally
            {
                if (regKey != null) regKey.Close();
            }
        }
    }

    /// <summary>
    /// Represents the registry settings related to the attachment 
    /// management feature.
    /// </summary>
    public class AmRegistrySettings
    {
        public static UInt64 DefaultThreshold = 1024 * 1024;

        /// <summary>
        /// Is the feature enabled or not.
        /// </summary>
        public bool EnableAM = true;

        /// <summary>
        /// Threshold at which activate AM, in bytes.
        /// </summary>
        public UInt64 ManagedAttachmentThreshold = DefaultThreshold;

        /// <summary>
        /// Flag telling wether to prompt or to automatically manage the 
        /// attachments when the AM is activated.
        /// </summary>
        public bool ManagedAttachmentAsk = false;

        /// <summary>
        /// Generic expiry setting. Values are:
        /// 0 -> 1 week
        /// 1 -> 1 month
        /// 2 -> Never expire
        /// 3 -> Custom
        /// </summary>
        public int ExpiryGenSetting = 1;

        /// <summary>
        /// Numeric value for the Custom generic setting.
        /// </summary>
        public int ExpiryCustomValue = 1;

        /// <summary>
        /// Unit related to ExpiryCustomValue. Possible values are:
        /// 0 -> day
        /// 1 -> week
        /// 2 -> month
        /// </summary>
        public int ExpiryCustomUnit = 0;

        private const UInt64 SecondsInOneDay = 60 * 60 * 24;
        private const UInt64 SecondsInOneWeek = 7 * SecondsInOneDay;
        private const UInt64 SecondsInOneMonth = 30 * SecondsInOneDay;

        public UInt64 ExpiryInSeconds
        {
            get
            {
                if (ExpiryGenSetting == 0) return SecondsInOneWeek;
                else if (ExpiryGenSetting == 1) return SecondsInOneMonth;
                else if (ExpiryGenSetting == 2) return 0;
                else if (ExpiryCustomUnit == 0) return (UInt64)ExpiryCustomValue * SecondsInOneDay;
                else if (ExpiryCustomUnit == 1) return (UInt64)ExpiryCustomValue * SecondsInOneWeek;
                else if (ExpiryCustomUnit == 2) return (UInt64)ExpiryCustomValue * SecondsInOneMonth;
                else return SecondsInOneMonth;
            }
        }

        public TimeSpan ExpiryInTimespan
        {
            get { return TimeSpan.FromSeconds(ExpiryInSeconds);}
        }


        /// <summary>
        /// Return the threshold in Mb, rounded.
        /// </summary>
        public Int32 GetThresholdInMb()
        {
            try
            {
                return Convert.ToInt32((double)ManagedAttachmentThreshold / 1024 / 1024);
            }

            catch (Exception ex)
            {
                Logging.LogException(ex);
                return 1;
            }
        }

        public static AmRegistrySettings Spawn()
        {
            AmRegistrySettings self = new AmRegistrySettings();
            self.ReadRegistry();
            return self;
        }

        public static bool IsValidGenValue(int val)
        {
            return (val > 0 && val < 4);
        }

        public static bool IsValidCustomUnit(int val)
        {
            return (val >= 0 && val < 3);
        }

        /// <summary>
        /// Read the values of this instance from the registry.
        /// </summary>
        public void ReadRegistry()
        {
            RegistryKey regKey = null;
            try
            {
                regKey = Base.GetOtcRegKey();
                int val;
                UInt64 uval;

                if (Int32.TryParse(regKey.GetValue("EnableAM", "1") as String, out val))
                    EnableAM = Convert.ToBoolean(val);
                else
                    EnableAM = true;

                if (Int32.TryParse(regKey.GetValue("ManagedAttachmentAsk", "1") as String, out val))
                    ManagedAttachmentAsk = Convert.ToBoolean(val);
                else
                    ManagedAttachmentAsk = true;

                if (UInt64.TryParse(regKey.GetValue("ManagedAttachmentThreshold", DefaultThreshold.ToString()) as String, out uval))
                    ManagedAttachmentThreshold = uval;
                else
                    ManagedAttachmentThreshold = DefaultThreshold;

                if (Int32.TryParse(regKey.GetValue("ManagedAttachmentExpiryGen", "0") as String, out val) &&
                    IsValidGenValue(val))
                {
                    ExpiryGenSetting = val;
                }
                else
                {
                    ExpiryGenSetting = 1;
                }

                if (Int32.TryParse(regKey.GetValue("ManagedAttachmentExpiryCustomValue", "1") as String, out val))
                    ExpiryCustomValue = val;
                else
                    ExpiryCustomValue = 1;

                if (Int32.TryParse(regKey.GetValue("ManagedAttachmentExpiryCustomUnit", "0") as String, out val) &&
                    IsValidCustomUnit(val))
                {
                    ExpiryCustomUnit = val;
                }
                else
                {
                    ExpiryCustomUnit = 0;
                }
            }

            catch (Exception ex)
            {
                Base.HandleException(ex, true);
            }

            finally
            {
                if (regKey != null) regKey.Close();
            }
        }

        /// <summary>
        /// Write the values of this instance to the registry.
        /// </summary>
        public void WriteRegistry()
        {
            RegistryKey regKey = null;
            try
            {
                regKey = Base.GetOtcRegKey();
                regKey.SetValue("EnableAm", Convert.ToInt32(EnableAM).ToString());
                regKey.SetValue("ManagedAttachmentAsk", Convert.ToInt32(ManagedAttachmentAsk).ToString());
                regKey.SetValue("ManagedAttachmentThreshold", ManagedAttachmentThreshold.ToString());
                regKey.SetValue("ManagedAttachmentExpiryGen", ExpiryGenSetting.ToString());
                regKey.SetValue("ManagedAttachmentExpiryCustomValue", ExpiryCustomValue.ToString());
                regKey.SetValue("ManagedAttachmentExpiryCustomUnit", ExpiryCustomUnit.ToString());
            }

            catch (Exception ex)
            {
                Base.HandleException(ex, true);
            }

            finally
            {
                if (regKey != null) regKey.Close();
            }
        }
    }
}
