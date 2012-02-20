using System;
using System.Collections.Generic;
using System.Text;
using Tbx.Utils;

namespace Tbx.Utils
{
    /// <summary>
    /// Contain the global OANP protocol information.
    /// </summary>
    public static class OAnp
    {
        public const UInt32 Major = 1;
        public const UInt32 Minor = 2;

        /// <summary>
        /// Return the full path to the KppMso-KWM shared data directory, backslash-terminated.
        /// </summary>
        public static String GetKppMsoKwmDataPath()
        {
            String appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return appData + "\\teambox\\kcs\\kwm\\";
        }

        /// <summary>
        /// Return the path to the KWM information file.
        /// </summary>
        public static String GetKppMsoKwmInfoPath()
        {
            return GetKppMsoKwmDataPath() + "info.txt";
        }
    }

    /* Here is how the 32-bits ANP 'type' field is structured for the
     * OANP protocol :
     * 
     *    3                   2                   1                   0
     *  1 0 9 8 7 6 5 4 3 2 1 0 9 8 7 6 5 4 3 2 1 0 9 8 7 6 5 4 3 2 1 0
     * +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
     * |Pr type|Rle|  Namespace ID     |           Reserved            |
     * +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
     * 
     * Protocol type :
     * KANP_PROTO
     * OANP_PROTO
     * 
     * Role :
     * OANP_CMD
     * OANP_RES
     * OANP_EVT
     * 
     * Namespace ID :
     * Various commands ...
     * 
     * */
    public static class OAnpType
    {
        /* Protocol type */
        public const UInt32 PROTO_TYPE_MASK = 0xf0000000;
        public const UInt32 OANP_PROTO = (2 << 28);

        /* Role */
        public const UInt32 ROLE_MASK = 0x0C000000;
        public const UInt32 OANP_CMD = (0 << 26);
        public const UInt32 OANP_RES = (1 << 26);
        public const UInt32 OANP_EVT = (2 << 26);
        public const UInt32 OANP_RESERVED = (3 << 26);

        /* Namespace ID */
        public const UInt32 NAMESPACE_ID_MASK = 0x03ff0000;
        
        /* Generic success. */
        public const UInt32 OANP_RES_OK = (OANP_PROTO | OANP_RES | (0 << 16));

        /* Command failed.
         *   UInt32 Failure type.
         *   STR    Failure explanation.
         *   <Failure specific data>.
         */
        public const UInt32 OANP_RES_FAIL = (OANP_PROTO | OANP_RES | (1 << 16));

        /* Failure types */

        /* No other data */
        public const UInt32 OANP_RES_FAIL_GEN_ERROR = 1;

        
        /* Cancel the execution of the command having the ID specified, if possible.
         * This command does not generate a result.
         *   UINT64 Command ID.
         */
        public const UInt32 OANP_CMD_CANCEL_CMD = (OANP_PROTO | OANP_CMD | (1 << 16));

        /* Test if the workspace users specified exist.
         *   UINT64 Internal workspace ID.
         *   UINT32 Number of users.
         *     STR    User email address.
         */
        public const UInt32 OANP_CMD_IS_KWS_USER = (OANP_PROTO | OANP_CMD | (2 << 16));

        /* User existence result.
         *   UINT32 Number of users.
         *     UINT32 True if the user exists.
         */
        public const UInt32 OANP_RES_IS_KWS_USER = (OANP_PROTO | OANP_RES | (2 << 16));


        /* Open (display in foreground) the workspace specified in the KWM.
         *   UINT64 Internal workspace ID.
         */
        public const UInt32 OANP_CMD_OPEN_KWS = (OANP_PROTO | OANP_CMD | (3 << 16));


        /* Join the workspace specified in the KWM.
         *   STR    String containing the data of the credentials file.
         */
        public const UInt32 OANP_CMD_JOIN_KWS = (OANP_PROTO | OANP_CMD | (4 << 16));

        /* Join workspace result.
         *   UINT64 Internal workspace ID.
         */
        public const UInt32 OANP_RES_JOIN_KWS = (OANP_PROTO | OANP_RES | (4 << 16));


        /* Create the workspace specified in the KWM.
         *   STR    Name of the workspace.
         *   UINT32 Secure flag.
         *   UINT32 Number of people invited.
         *     STR    User real name.
         *     STR    User email address.
         *     UINT64 Key ID. 0 if none.
         *     STR    Organization name. Empty if none.
         *     STR    Password. Empty if none.
         */
        public const UInt32 OANP_CMD_CREATE_KWS = (OANP_PROTO | OANP_CMD | (5 << 16));

        /* Workspace created.
         *   STR    Workspace-linked email URL (provided for convenience, but unused).
         *   UINT32 Number of people invited.
         *     STR    User email address.
         *     STR    Invitation URL.
         */
        public const UInt32 OANP_RES_CREATE_KWS = (OANP_PROTO | OANP_RES | (5 << 16));


        /* Invite users to the workspace specified.
         *   UINT64 Internal workspace ID.
         *   UINT32 Number of people invited.
         *     STR    User real name.
         *     STR    User email address.
         *     UINT64 Key ID. 0 if none.
         *     STR    Organization name. Empty if none.
         *     STR    Password. Empty if none.
         */
        public const UInt32 OANP_CMD_INVITE_TO_KWS = (OANP_PROTO | OANP_CMD | (6 << 16));

        /* Users invited.
         *   STR    Workspace-linked email URL.
         *   UINT32 Number of people actually invited (this may be less than requested).
         *     STR    User email address.
         *     STR    Invitation URL.
         */
        public const UInt32 OANP_RES_INVITE_TO_KWS = (OANP_PROTO | OANP_RES | (6 << 16));


        /* Get a SKURL.
         *   STR    Subject.
         *   UINT32 Number of recipient.
         *     STR    Recipient name.
         *     STR    Recipient email address.
         *   UINT32 Number of attachments to manage.
         *     STR    Path to file on local HD.
         *   UINT64 Attachment expiration delay in seconds. 0 is infinite.
         * 
         * Outlook current hands the control to the KWM over the file passed through
         * that command. It doesn't expect the files to remain in place.
         */
        public const UInt32 OANP_CMD_GET_SKURL = (OANP_PROTO | OANP_CMD | (7 << 16));

        /* SKURL result.
         *   STR    SKURL, if any.
         *   UINT32 Public workspace expiration support flag.
         */
        public const UInt32 OANP_RES_GET_SKURL = (OANP_PROTO | OANP_RES | (7 << 16));


        /* Lookup the specified recipient addresses.
         *   UINT32 Number of addresses.
         *     STR    Email address.
         */
        public const UInt32 OANP_CMD_LOOKUP_REC_ADDR = (OANP_PROTO | OANP_CMD | (8 << 16));

        /* Looked up the recipient addresses.
         *   UINT32 Number of addresses.
         *     STR    Email address.
         *     UINT64 Key ID. 0 if none.
         *     STR    Organization name. Empty if none.
         */
        public const UInt32 OANP_RES_LOOKUP_REC_ADDR = (OANP_PROTO | OANP_RES | (8 << 16));

        /* Start a screen share, prompting for configs
         *   UINT64   WorkspaceID
         *   UINT32   Flags
         *   UINT32   Prompt on top of this window (HWND).
         */
        public const UInt32 OANP_CMD_START_SCREEN_SHARE = (OANP_PROTO | OANP_CMD | (9 << 16));

        public enum OanpScreenShareFlags
        {
            Prompt = 1,
            GiveControl = 2,
        };

        /* Join a screen Sharing session.
         *   UINT64   Workspace ID.
         *   UINT64   Screen Sharing Session ID.
         */
        public const UInt32 OANP_CMD_JOIN_SCREEN_SHARE = (OANP_PROTO | OANP_CMD | (10 << 16));

        /* Subscribe/Unscribe to/from Workspace.
         *   UINT64  WorkspaceID
         *   UINT32  Subscription flag (1 means subscribe, 0 means unsubscribe)
         *   UINT64  Last Event Date
         */
        public const UInt32 OANP_CMD_WORKSPACE_SUBSCRIBE = (OANP_PROTO | OANP_CMD | (11 << 16));

        /* Drop a file in a workspace.
         *   UINT64  WorkspaceID
         *   UINT32  ShareID
         *   STR     Src
         *   STR     Dst
         */
        public const UInt32 OANP_CMD_DROPFILE = (OANP_PROTO | OANP_CMD | (12 << 16));

        /* Send a chat message.
         *   UINT64  WorkspaceID
         *   UINT32  ChatID
         *   STR     Message
         */
        public const UInt32 OANP_CMD_CHAT = (OANP_PROTO | OANP_CMD | (13 << 16));

        /* Set the notification mode.
         *   UINT64  WorkspaceID (0 == ALL)
         *   UINT32  Mode (flag)
         */
        public const UInt32 OANP_CMD_SET_NOTIFY_MODE = (OANP_PROTO | OANP_CMD | (14 << 16));

        public enum NotifyMode
        {
            Email = 1,
            TrayIcon = 2,
            External = 4, //TOC
        };

        /* Get a file
         *   UINT64  WorkspaceID
         *   UINT32  ShareID
         *   UINT64  Inode
         *   UINT64  CommitID
         *   STR     Dst
         */
        public const UInt32 OANP_CMD_GET_FILE = (OANP_PROTO | OANP_CMD | (15 << 16));

        /* Open the file directly from the share.
         *   UINT64  WorkspaceID
         *   UINT32  ShareID
         *   UINT64  Inode
         */
        public const UInt32 OANP_CMD_OPEN_FILE = (OANP_PROTO | OANP_CMD | (16 << 16));

        /* Query file status.
         *   UINT64  WorkspaceID
         *   UINT32  ShareID
         *   UINT64  Inode
         */
        public const UInt32 OANP_CMD_FILE_STATUS = (OANP_PROTO | OANP_CMD | (17 << 16));
        
        /* File status
         *   UINT32  Status
         *   UINT64  Last Commit Version On Kwm
         */
        public const UInt32 OANP_RES_FILE_STATUS = (OANP_PROTO | OANP_RES | (17 << 16));

        public enum FileStatus
        {
            None,
            Modified,
            Unmodified,
            Absent,
            Error
        };

        /* Workspace list refresh.
         *   UINT32 True if the user is authorized to create a workspace.
         *   UINT32 True if the OTC settings in the registry have been modified by the KWM.
         *   UINT32 Number of workspaces.
         *     UINT64 Internal ID.
         *     UINT64 External ID.
         *     STR    KCD address, if any.
         *     STR    KWMO address, if any.
         *     STR    Workspace name.
         *     STR    Parent folder path in KWM format.
         *     UINT32 Secure flag.
         *     UINT32 Invite flag.
         *     UInt64 Creation date.
         */
        public const UInt32 OANP_EVT_NEW_KWM_STATE = (OANP_PROTO | OANP_EVT | (1 << 16));

        /* Some users Update.
         *   UINT64  WorkspaceID
         *   UINT64  Date (Seconds since epoch)
         *   UINT32  Count
         *     UINT32  UserID
         *     STR     AdminName
         *     STR     UserName
         *     STR     EmailAddress
         *     UINT32  Power
         *     STR     OrgName
         */
        public const UInt32 OANP_EVT_USER_UPDATE = (OANP_PROTO | OANP_EVT | (2 << 16));

        /* Chat message received
         *   UINT64  WorkspaceID
         *   UINT64  Date
         *   UINT32  ChatID
         *   UINT32  UserID
         *   STR     Message
         */
        public const UInt32 OANP_EVT_CHAT_MSG = (OANP_PROTO | OANP_EVT | (3 << 16));

        /* Change to KFS
         *   UINT64  WorkspaceID
         *   UINT64  Date
         *   UINT32  UserID
         *   UINT32  ShareID
         *   UINT64  CommitID
         *   UINT32  Count
         *     <changes>
         *   
         * Create file / dir:
         *   UINT32 Number of elements in this message (for compatibility).
         *   UINT32 Change type ("create file" / "create dir").
         *   UINT64 Inode created.
         *   UINT64 Parent inode.
         *   STR    Entry name.
         *
         * Update file:
         *   UINT32 Number of elements in this message (for compatibility).
         *   UINT32 Change type ("update file").
         *   UINT64 Inode updated.
         *
         * Delete file / dir:
         *   UINT32 Number of elements in this message (for compatibility).
         *   UINT32 Change type ("delete file" / "delete dir")
         *   UINT64 Inode deleted.
         *
         * Move file / dir:
         *   UINT32 Number of elements in this message (for compatibility).
         *   UINT32 Change type ("move file" / "move directory").
         *   UINT64 Inode moved.
         *   UINT64 New parent inode.
         *   STR    New entry name.
         */
        public const UInt32 OANP_EVT_FS_UPDATE = (OANP_PROTO | OANP_EVT | (4 << 16));

        /* Application sharing session started.
         *   UINT64 Workspace ID.
         *   UINT64 Date (seconds since UNIX epoch).
         *   UINT32 User ID.
         *   UINT64 Session ID.
         *   STR    Subject.
         */
        public const UInt32 OANP_EVT_VNC_START = (OANP_PROTO | OANP_EVT | (5 << 16));

        /* Application sharing session ended.
         *   UINT64 Workspace ID.
         *   UINT64 Date (seconds since UNIX epoch).
         *   UINT32 User ID.
         *   UINT64 Session ID.
         */
        public const UInt32 OANP_EVT_VNC_END = (OANP_PROTO | OANP_EVT | (6 << 16));

        public static String TypeToString(UInt32 type)
        {
            String res;

            switch (type & PROTO_TYPE_MASK)
            {
                case OANP_PROTO:
                    res = "OANP_PROTO";
                    break;
                default:
                    res = "UNKNOWN_PROTO";
                    return res;
            }
            switch (type & ROLE_MASK)
            {
                case OANP_CMD:
                    res += " | OANP_CMD";
                    break;
                case OANP_EVT:
                    res += " | OANP_EVT";
                    break;
                case OANP_RES:
                    res += " | OANP_EVT";
                    break;
                default:
                    res += " | UNKNOWN_ROLE";
                    return res;
            }

            res += " | " + (type & NAMESPACE_ID_MASK); 

            return res;
        }
    }

}
