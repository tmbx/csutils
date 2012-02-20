using System.Collections;
using System.Net.Sockets;

namespace Tbx.Utils
{
    /// By supplying -2 instead of -1 as the timeout value to Socket.Select,
    /// it will result in the timeout for the underlying winsock select call 
    /// to be more than 1 hour. Windows crap workaround.
    /// http://msdn.microsoft.com/en-us/library/system.net.sockets.socket.select.aspx

    public class SelectSockets
    {
        private ArrayList m_ReadSockets = new ArrayList();
        private ArrayList m_WriteSockets = new ArrayList();
        private ArrayList m_ErrorSockets = new ArrayList();
        private int m_Timeout = -2;

        public ArrayList ReadSockets
        {
            get { return m_ReadSockets; }
        }

        public ArrayList WriteSockets
        {
            get { return m_WriteSockets; }
        }

        public ArrayList ErrorSockets
        {
            get { return m_ErrorSockets; }
        }

        /// <summary>
        /// In microsecond.
        /// </summary>
        public int Timeout
        {
            get { return m_Timeout; }
            set { m_Timeout = value; }
        }

        private void Add(ArrayList l, Socket sock)
        {
            if (!ErrorSockets.Contains(sock))
            {
                l.Add(sock);
                ErrorSockets.Add(sock);
            }
            else if (!l.Contains(sock))
            {
                l.Add(sock);
            }
        }
        public void AddRead(Socket sock)
        {
            Add(ReadSockets, sock);
        }
        public void AddWrite(Socket sock)
        {
            Add(WriteSockets, sock);
        }
        public void AddRW(Socket sock)
        {
            AddRead(sock);
            AddWrite(sock);
        }
        public bool InRead(Socket sock)
        {
            return ReadSockets.Contains(sock) || ErrorSockets.Contains(sock);
        }
        public bool InWrite(Socket sock)
        {
            return WriteSockets.Contains(sock) || ErrorSockets.Contains(sock);
        }
        public bool InReadOrWrite(Socket sock)
        {
            return InRead(sock) || InWrite(sock);
        }
        public void Select()
        {
            Socket.Select(
                ReadSockets.Count > 0 ? ReadSockets : null,
                WriteSockets.Count > 0 ? WriteSockets : null,
                ErrorSockets.Count > 0 ? ErrorSockets : null,
                Timeout);
        }
    }
}