using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.Windows.Forms;

namespace Tbx.Utils
{
    /// <summary>
    /// Manage the mechanic of sending and receiving ANP messages.
    /// </summary>
    public class AnpTransport
    {
        enum InState
        {
            NoMsg,
            RecvHdr,
            RecvPayload,
            Received
        };

        enum OutState
        {
            NoPacket,
            Sending,
        };

        private InState inState = InState.NoMsg;
        private AnpMsg inMsg;
        private byte[] inBuf;
        private int inPos;
        private OutState outState = OutState.NoPacket;
        private byte[] outBuf;
        private int outPos;
        private Socket sock;

        public bool isReceiving
        {
            get { return (inState != InState.NoMsg); }
        }
        public bool doneReceiving
        {
            get { return (inState == InState.Received); }
        }
        public bool isSending
        {
            get { return (outState != OutState.NoPacket); }
        }
        public void reset()
        {
            flushRecv();
            flushSend();
            sock = null;
        }

        public void flushRecv() { inState = InState.NoMsg; }
        public void flushSend() { outState = OutState.NoPacket; }

        public AnpTransport(Socket s)
        {
            sock = s;
        }

        public void beginRecv()
        {
            inState = InState.RecvHdr;
            inBuf = new byte[AnpMsg.HdrSize];
            inPos = 0;
        }

        public AnpMsg getRecv()
        {
            Debug.Assert(doneReceiving);
            AnpMsg m = inMsg;
            flushRecv();
            return m;
        }

        public void sendMsg(AnpMsg msg)
        {
            outState = OutState.Sending;
            outBuf = msg.ToByteArray(true);
            outPos = 0;
        }

        public void doXfer()
        {
            bool loop = true;

            while (loop)
            {
                loop = false;

                if (inState == InState.RecvHdr)
                {
                    int r = Base.SockRead(sock, inBuf, inPos, inBuf.Length - inPos);

                    if (r > 0)
                    {
                        loop = true;
                        inPos += r;

                        if (inPos == inBuf.Length)
                        {
                            inMsg = new AnpMsg();

                            UInt32 size = 0;
                            AnpMsg.ParseHdr(inBuf, ref inMsg.Major, ref inMsg.Minor, ref inMsg.Type, ref inMsg.ID, ref size);

                            if (size > AnpMsg.MaxSize)
                            {
                                throw new AnpException("ANP message is too large");
                            }

                            if (size > 0)
                            {
                                inState = InState.RecvPayload;
                                inBuf = new byte[size];
                                inPos = 0;
                            }

                            else
                            {
                                inState = InState.Received;
                            }
                        }
                    }
                }

                if (inState == InState.RecvPayload)
                {
                    int r = Base.SockRead(sock, inBuf, inPos, inBuf.Length - inPos);

                    if (r > 0)
                    {
                        loop = true;
                        inPos += r;

                        if (inPos == inBuf.Length)
                        {
                            inMsg.Elements = AnpMsg.ParsePayload(inBuf);
                            inState = InState.Received;
                        }
                    }
                }

                if (outState == OutState.Sending)
                {
                    int r = Base.SockWrite(sock, outBuf, outPos, outBuf.Length - outPos);

                    if (r > 0)
                    {
                        loop = true;
                        outPos += r;

                        if (outPos == outBuf.Length)
                        {
                            outState = OutState.NoPacket;
                            break;
                        }
                    }
                }
            }
        }
    }
}
