using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Tbx.Utils
{
    /// <summary>
    /// This class contains static methods to manipulate paths and obtain
    /// file information.
    /// </summary>
    public static class KfsPath
    {
        /// <summary>
        /// Return a reference to the StringComparer object used to perform 
        /// case-insensitive string comparisons.
        /// </summary>
        public static StringComparer Comparer { get { return StringComparer.OrdinalIgnoreCase; } }

        /// <summary>
        /// Return true if the two strings specified are equal without 
        /// considering the case.
        /// </summary>
        public static bool Eq(String a, String b)
        {
            return Comparer.Equals(a, b);
        }
        
        /// <summary>
        /// This method converts every backslash in the path to slash. 
        /// If the slashTerminated param is true, the path is appended a trailing
        /// delimiter if necessary, otherwise it is removed if necessary.
        /// </summary>
        /// <param name="pathToConvert">The path to convert.</param>
        /// <param name="slashTerminated">True if a trailing delimiter must be appended.</param>
        public static String GetUnixFilePath(String pathToConvert, bool slashTerminated)
        {
            String tempPath = pathToConvert.Replace("\\", "/");
            if (tempPath.Length > 0)
            {
                if (slashTerminated)
                {
                    if (tempPath[tempPath.Length - 1] != '/')
                    {
                        tempPath = tempPath + "/";
                    }
                }
                else
                {
                    if (tempPath[tempPath.Length - 1] == '/')
                    {
                        tempPath = tempPath.Substring(0, tempPath.Length - 1);
                    }
                }
            }
            return tempPath;
        }

        /// <summary>
        /// This method converts every slash in the path to backslashes. 
        /// If the backslashTerminated param is true, the path is appended a trailing
        /// delimiter if necessary, otherwise it is removed if necessary.
        /// </summary>
        /// <param name="pathToConvert">The path to convert.</param>
        /// <param name="backslashTerminated">True if a trailing delimiter must be appended.</param>
        public static String GetWindowsFilePath(String pathToConvert, bool backslashTerminated)
        {
            String tempPath = pathToConvert.Replace("/", "\\");
            if (tempPath.Length > 0)
            {
                if (backslashTerminated)
                {
                    if (tempPath[tempPath.Length - 1] != '\\')
                    {
                        tempPath = tempPath + "\\";
                    }
                }
                else
                {
                    if (tempPath[tempPath.Length - 1] == '\\')
                    {
                        tempPath = tempPath.Substring(0, tempPath.Length - 1);
                    }
                }
            }
            return tempPath;
        }

        /// <summary>
        /// Test if fileName contains invalid characters for a Windows file name. 
        /// </summary>
        public static bool IsValidFileName(String fileName)
        {
            if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1 ||
                fileName.Length == 0 ||
                fileName.StartsWith(" "))
            {
                return false;
            }

            try
            {
                Encoding latinEuropeanEncoding = Encoding.GetEncoding("iso-8859-1", EncoderExceptionFallback.ExceptionFallback, DecoderExceptionFallback.ExceptionFallback);
                Encoding uniCode = Encoding.Unicode;
                Encoding.Convert(uniCode, latinEuropeanEncoding, uniCode.GetBytes(fileName));
            }
            catch (EncoderFallbackException)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Return a list with each portion of the path. 
        /// Example : a\b\c\allo.txt
        /// The list returned is:
        /// |a|b|c|allo.txt|
        /// 
        /// The function doesn't care whether the path is a UNIX or a Windows path. It
        /// splits portions at slashses and backslashes. The function does not work on
        /// absolute paths.
        /// </summary>
        public static String[] SplitRelativePath(String relativePath)
        {
            return relativePath.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Return the directory portion of the path specified. The directory
        /// portion will have a trailing delimiter if it is non-empty.
        /// </summary>
        public static String DirName(String path)
        {
            if (path == "") return "";
            int LastIndex = path.Length - 1;
            for (; LastIndex > 0 && !IsDelim(path[LastIndex]); LastIndex--) { }
            if (!IsDelim(path[LastIndex])) return "";
            return path.Substring(0, LastIndex + 1);
        }

        /// <summary>
        /// Return the file portion of the path specified.
        /// </summary>
        public static String BaseName(String path)
        {
            if (path == "") return "";
            int LastIndex = path.Length - 1;
            for (; LastIndex > 0 && !IsDelim(path[LastIndex]); LastIndex--) { }
            if (!IsDelim(path[LastIndex])) return path;
            return path.Substring(LastIndex + 1, path.Length - LastIndex - 1);
        }

        /// <summary>
        /// Add a trailing slash to the path specified if the path is non-empty
        /// and it does not already end with a delimiter.
        /// </summary>
        public static String AddTrailingSlash(String path)
        {
            return AddTrailingSlash(path, false);
        }

        /// <summary>
        /// Add a trailing slash to the path specified if the path does not already 
        /// end with a delimiter, or if the path is empty and slashIfEmpty is set to true.
        /// </summary>
        public static String AddTrailingSlash(String path, bool slashIfEmpty)
        {
            if (path == "")
            {
                if (slashIfEmpty)
                    return "/";
                else
                    return "";
            }

            if (IsDelim(path[path.Length - 1])) return path;
            return path + "/";
        }

        /// <summary>
        /// Add a trailing backslash to the path specified if the path is non-empty
        /// and it does not already end with a delimiter.
        /// </summary>
        public static String AddTrailingBackslash(String path)
        {
            if (path == "") return "";
            if (IsDelim(path[path.Length - 1])) return path;
            return path + @"\";
        }

        /// <summary>
        /// Remove the trailing delimiter from the string specified, if there
        /// is one.
        /// </summary>
        public static String StripTrailingDelim(String path)
        {
            if (path == "") return "";
            if (IsDelim(path[path.Length - 1])) return path.Substring(0, path.Length - 1);
            return path;
        }

        /// <summary>
        /// Return true if the character specified is a slash or a backslash.
        /// </summary>
        public static bool IsDelim(Char c)
        {
            return (c == '/' || c == '\\');
        }

        /// <summary>
        /// Return of the file specified. The file must exist.
        /// </summary>
        public static UInt64 GetFileSize(String path)
        {
            return (UInt64)(new FileInfo(path)).Length;
        }

        /// <summary>
        /// Obtain the low-level information of the file stream specified.
        /// </summary>
        public static void GetLowLevelFileInfo(FileStream stream, out UInt64 fileID, out UInt64 fileSize, out DateTime fileDate)
        {
            Syscalls.BY_HANDLE_FILE_INFORMATION bhfi;
            Syscalls.GetFileInformationByHandle(stream.SafeFileHandle.DangerousGetHandle(), out bhfi);
            fileID = (bhfi.FileIndexHigh << 32) + bhfi.FileIndexLow;
            fileSize = (bhfi.FileSizeHigh << 32) + bhfi.FileSizeLow;
            fileDate = DateTime.FromFileTime((Int64)(((UInt64)bhfi.LastWriteTime.dwHighDateTime << 32) +
                                                     (UInt64)bhfi.LastWriteTime.dwLowDateTime));
        }

        /// <summary>
        /// Move the file or directory specified at the location specified.
        /// Be careful: if only the case is changed, Windows Explorer will
        /// NOT report the change even though it has been made. For 
        /// directories, be careful: both paths must be absolute and have 
        /// the same format. This method handles case sensitivity issues. 
        /// I hate all the fucktards at Microsoft.
        /// </summary>
        public static void MovePath(bool dirFlag, String src, String dst)
        {
            // Work around directory already exists crap.
            if (dirFlag && Eq(src, dst))
            {
                int i = 0;
                String tmpPath;
                while (true)
                {
                    tmpPath = dst + i++;
                    if (!File.Exists(tmpPath) && !Directory.Exists(tmpPath)) break;
                }
                Directory.Move(src, tmpPath);
                src = tmpPath;
            }

            if (dirFlag) Directory.Move(src, dst);
            else File.Move(src, dst);
        }
    }
}
