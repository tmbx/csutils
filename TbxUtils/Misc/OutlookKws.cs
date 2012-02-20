using System;
using System.Collections.Generic;
using System.Text;

namespace Tbx.Utils
{
    /// <summary>
    /// Used to marshal an OutlookKws between threads using com.
    /// </summary>
    public struct OutlookKwsStruct
    {
        public bool IsNew;
        public UInt64 InternalID;
        public UInt64 ExternalID;
        public String KcdAddress;
        public String KwmoAddress;
        public String KwsName;
        public String FolderPath;
        public bool SecureFlag;
        public bool InvitePowerFlag;
        public bool ConnectedFlag;
        public bool FreezeFlag;
        public bool DeepFreezeFlag;
        public bool PublicFlag;
        public UInt64 CreationDate;
    }

    /// <summary>
    /// Workspace information used by Outlook.
    /// </summary>
    public class OutlookKws
    {
        public bool IsNew;
        public UInt64 InternalID;
        public UInt64 ExternalID;
        public String KcdAddress;
        public String KwmoAddress;
        public String KwsName;
        public String FolderPath;
        public bool SecureFlag;
        public bool InvitePowerFlag;
        public bool ConnectedFlag;
        public bool FreezeFlag;
        public bool DeepFreezeFlag;
        public bool PublicFlag;
        public UInt64 CreationDate;

        public OutlookKws() { }

        /// <summary>
        /// Creates a new object from an AnpMsg. The AnpMsg elements
        /// before the actual workspace details must be consumed.
        /// </summary>
        /// <param name="msg"></param>
        public OutlookKws(AnpMsg msg)
        {
            InternalID = msg.PopHead().UInt64;
            ExternalID = msg.PopHead().UInt64;
            KcdAddress = msg.PopHead().String;
            KwmoAddress = msg.PopHead().String;
            KwsName = msg.PopHead().String;
            FolderPath = msg.PopHead().String;
            SecureFlag = (msg.PopHead().UInt32 > 0);
            InvitePowerFlag = (msg.PopHead().UInt32 > 0);
            ConnectedFlag = (msg.PopHead().UInt32 > 0);
            FreezeFlag = (msg.PopHead().UInt32 > 0);
            DeepFreezeFlag = (msg.PopHead().UInt32 > 0);
            PublicFlag = (msg.PopHead().UInt32 > 0);
            CreationDate = (msg.PopHead().UInt64);
        }

        public OutlookKws(OutlookKwsStruct kws)
        {
            IsNew = kws.IsNew;
            InternalID = kws.InternalID;
            ExternalID = kws.ExternalID;
            KcdAddress = kws.KcdAddress;
            KwmoAddress = kws.KwmoAddress;
            KwsName = kws.KwsName;
            FolderPath = kws.FolderPath;
            SecureFlag = kws.SecureFlag;
            InvitePowerFlag = kws.InvitePowerFlag;
            ConnectedFlag = kws.ConnectedFlag;
            FreezeFlag = kws.FreezeFlag;
            DeepFreezeFlag = kws.DeepFreezeFlag;
            PublicFlag = kws.PublicFlag;
            CreationDate = kws.CreationDate;
        }

        /// <summary>
        /// Fill the given AnpMsg.
        /// </summary>
        public void FillAnp(ref AnpMsg msg)
        {
            msg.AddUInt64(InternalID);
            msg.AddUInt64(ExternalID);
            msg.AddString(KcdAddress);
            msg.AddString(KwmoAddress);
            msg.AddString(KwsName);
            msg.AddString(FolderPath);
            msg.AddUInt32(Convert.ToUInt32(SecureFlag));
            msg.AddUInt32(Convert.ToUInt32(InvitePowerFlag));
            msg.AddUInt32(Convert.ToUInt32(ConnectedFlag));
            msg.AddUInt32(Convert.ToUInt32(FreezeFlag));
            msg.AddUInt32(Convert.ToUInt32(DeepFreezeFlag));
            msg.AddUInt32(Convert.ToUInt32(PublicFlag));
            msg.AddUInt64(CreationDate);
        }

        public override bool Equals(object obj)
        {
            OutlookKws k2 = obj as OutlookKws;
            if (k2 == null) return false;
            return (InternalID == k2.InternalID &&
                    ExternalID == k2.ExternalID &&
                    KcdAddress == k2.KcdAddress &&
                    KwmoAddress == k2.KwmoAddress &&
                    KwsName == k2.KwsName &&
                    FolderPath == k2.FolderPath &&
                    InvitePowerFlag == k2.InvitePowerFlag &&
                    SecureFlag == k2.SecureFlag &&
                    ConnectedFlag == k2.ConnectedFlag &&
                    FreezeFlag == k2.FreezeFlag &&
                    DeepFreezeFlag == k2.DeepFreezeFlag &&
                    PublicFlag == k2.PublicFlag &&
                    CreationDate == k2.CreationDate);
        }

        public override int GetHashCode()
        {
            return (int)(InternalID.GetHashCode() ^
                         ExternalID.GetHashCode() ^
                         KcdAddress.GetHashCode() ^
                         KwmoAddress.GetHashCode() ^
                         KwsName.GetHashCode() ^
                         FolderPath.GetHashCode() ^
                         InvitePowerFlag.GetHashCode() ^
                         SecureFlag.GetHashCode() ^
                         ConnectedFlag.GetHashCode() ^
                         FreezeFlag.GetHashCode() ^
                         DeepFreezeFlag.GetHashCode() ^
                         PublicFlag.GetHashCode() ^
                         CreationDate.GetHashCode());
        }

        public OutlookKwsStruct GetStruct()
        {
            OutlookKwsStruct kws = new OutlookKwsStruct();
            kws.IsNew = IsNew;
            kws.InternalID = InternalID;
            kws.ExternalID = ExternalID;
            kws.KcdAddress = KcdAddress;
            kws.KwmoAddress = KwmoAddress;
            kws.KwsName = KwsName;
            kws.FolderPath = FolderPath;
            kws.SecureFlag = SecureFlag;
            kws.InvitePowerFlag = InvitePowerFlag;
            kws.ConnectedFlag = ConnectedFlag;
            kws.FreezeFlag = FreezeFlag;
            kws.DeepFreezeFlag = DeepFreezeFlag;
            kws.PublicFlag = PublicFlag;
            kws.CreationDate = CreationDate;
            return kws;
        }
    }
}
