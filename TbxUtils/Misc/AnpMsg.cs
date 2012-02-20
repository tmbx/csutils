using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Tbx.Utils
{
    public class AnpException : Exception
    {
        public AnpException(string str) : base(str) { }
    }

    public class AnpMsg
    {
        // Element type, as we are not using inheritance.
        public enum AnpType
        {
            UInt32 = 1,
            UInt64,
            String,
            Bin
        }

        public class Element
        {
            private AnpType m_Type;

            // Element value for each type.
            private UInt32 m_UInt32;
            private UInt64 m_UInt64;
            private string m_String;
            private byte[] m_Bin;

            // Constructors, pass an element of the desired type.
            public Element(UInt32 val)
            {
                m_Type = AnpType.UInt32;
                m_UInt32 = val;
            }
            public Element(UInt64 val)
            {
                m_Type = AnpType.UInt64;
                m_UInt64 = val;
            }
            public Element(String val)
            {
                m_Type = AnpType.String;
                m_String = val;
            }
            public Element(byte[] val)
            {
                m_Type = AnpType.Bin;
                m_Bin = val;
            }

            public AnpType Type
            {
                get
                {
                    return m_Type;
                }
            }
            public UInt32 UInt32
            {
                get
                {
                    if (m_Type != AnpType.UInt32)
                        throw new AnpException("AnpType is " + m_Type.ToString() + ", not UInt32");
                    return m_UInt32;
                }

                set
                {
                    if (m_Type != AnpType.UInt32)
                        throw new AnpException("AnpType is " + m_Type.ToString() + ", not UInt32");
                    m_UInt32 = value;
                }
            }
            public UInt64 UInt64
            {
                get
                {
                    if (m_Type != AnpType.UInt64)
                        throw new AnpException("AnpType is " + m_Type.ToString() + ", not UInt64");
                    return m_UInt64;
                }

                set
                {
                    if (m_Type != AnpType.UInt64)
                        throw new AnpException("AnpType is " + m_Type.ToString() + ", not UInt64");
                    m_UInt64 = value;
                }
            }
            public string String
            {
                get
                {
                    if (m_Type != AnpType.String)
                        throw new AnpException("AnpType is " + m_Type.ToString() + ", not String");
                    return m_String;
                }

                set
                {
                    if (m_Type != AnpType.String)
                        throw new AnpException("AnpType is " + m_Type.ToString() + ", not String");
                    m_String = value;
                }
            }
            public byte[] Bin
            {
                get
                {
                    if (m_Type != AnpType.Bin)
                        throw new AnpException("AnpType is " + m_Type.ToString() + ", not Bin");
                    return m_Bin;
                }
                
                set
                {
                    if (m_Type != AnpType.Bin)
                        throw new AnpException("AnpType is " + m_Type.ToString() + ", not Bin");
                    m_Bin = value;
                }
            }
        }

        /// <summary>
        /// Size of the header, in bytes.
        /// </summary>
        public const int HdrSize = 24;

        /// <summary>
        /// Maximum size of an ANP message.
        /// </summary>
        public const int MaxSize = 100 * 1024 * 1024;

        public UInt32 Major;
        public UInt32 Minor;
        public UInt32 Type;
        public UInt64 ID;
        
        public List<Element> Elements = new List<Element>();

        public Element PopHead()
        {
            try
            {

                Element e = Elements[0];
                Elements.RemoveAt(0);
                return e;
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new AnpException("PopHead : no more elements in AnpMsg");
            }
        }

        public void Add(UInt32 i)
        {
            Elements.Add(new Element(i));
        }
        public void Add(UInt64 i)
        {
            Elements.Add(new Element(i));
        }
        public void Add(string i)
        {
            if (i == null) i = "";
            Elements.Add(new Element(i));
        }
        public void Add(byte[] i)
        {
            if (i == null) i = new byte[0];
            Elements.Add(new Element(i));
        }
        public void AddUInt32(UInt32 i)
        {
            Add(i);
        }
        public void AddUInt64(UInt64 i)
        {
            Add(i);
        }
        public void AddString(string i)
        {
            Add(i);
        }
        public void AddBin(byte[] i)
        {
            Add(i);
        }

        /// <summary>
        /// Write the header and the elements of the payload in the stream 
        /// specified if requested.
        /// </summary>
        public void ToStream(Stream s, bool headerFlag)
        {
            BinaryWriter w = new BinaryWriter(s, Encoding.GetEncoding("iso-8859-1"));

            if (headerFlag)
            {
                w.Write(Base.hton(Major));
                w.Write(Base.hton(Minor));
                w.Write(Base.hton(Type));
                w.Write(Base.hton(ID));
                w.Write(Base.hton(PayloadSize()));
            }

            foreach (Element e in Elements)
            {
                switch (e.Type)
                {
                    case AnpType.UInt32:
                        w.Write((byte)1);
                        w.Write(Base.hton(e.UInt32));
                        break;
                    case AnpType.UInt64:
                        w.Write((byte)2);
                        w.Write(Base.hton(e.UInt64));
                        break;
                    case AnpType.String:
                        w.Write((byte)3);
                        w.Write(Base.hton((UInt32)e.String.Length));
                        w.Write(e.String.ToCharArray());
                        break;
                    case AnpType.Bin:
                        w.Write((byte)4);
                        w.Write(Base.hton((UInt32)e.Bin.Length));
                        w.Write(e.Bin);
                        break;
                }
            }
        }

        /// <summary>
        /// Format the message, including the header if requested, as a byte array.
        /// </summary>
        public byte[] ToByteArray(bool headerFlag)
        {
            MemoryStream s = new MemoryStream();
            ToStream(s, headerFlag);
            return s.ToArray();
        }

        /// <summary>
        /// Retrieve the message data, including the header, from the byte
        /// array specified.
        /// </summary>
        public void FromByteArray(byte[] byteArray)
        {
            UInt32 size = 0;
            ParseHdr(byteArray, ref Major, ref Minor, ref Type, ref ID, ref size);
            
            // C# doesn't have slices.
            byte[] payloadArray = new byte[size];
            for (int i = 0; i < size; i++) payloadArray[i] = byteArray[i + HdrSize];
            Elements = ParsePayload(payloadArray);
        }

        public UInt32 PayloadSize()
        {
            UInt32 s = 0;

            foreach (Element e in Elements)
            {
                switch (e.Type)
                {
                    case AnpType.UInt32: s += 5; break;
                    case AnpType.UInt64: s += 9; break;
                    case AnpType.String: s += 5 + (UInt32)e.String.Length; break;
                    case AnpType.Bin: s += 5 + (UInt32)e.Bin.Length; break;
                }
            }
            return s;
        }

        public static void ParseHdr(byte[] hdr, ref UInt32 major, ref UInt32 minor, ref UInt32 type, ref UInt64 id, ref UInt32 size)
        {
            BinaryReader r = new BinaryReader(new MemoryStream(hdr));
            major = Base.ntoh(r.ReadUInt32());
            minor = Base.ntoh(r.ReadUInt32());
            type = Base.ntoh(r.ReadUInt32());
            id = Base.ntoh(r.ReadUInt64());
            size = Base.ntoh(r.ReadUInt32());
        }

        public static List<Element> ParsePayload(byte[] payload)
        {
            List<Element> a = new List<Element>();
            MemoryStream s = new MemoryStream(payload);
            BinaryReader r = new BinaryReader(s, Encoding.GetEncoding("iso-8859-1"));

            while (s.Position != s.Length)
            {
                Element e = null;

                // To whoever wants to fix this code: beware that old KWMs
                // send incorrectly formatted application packets. It is
                // probably preferrable to ignore type 0 elements.

                // Evil.
                AnpType t = (AnpType)r.ReadByte();

                switch (t)
                {
                    case AnpType.UInt32:
                        e = new Element(Base.ntoh(r.ReadUInt32()));
                        break;
                    case AnpType.UInt64:
                        e = new Element(Base.ntoh(r.ReadUInt64()));
                        break;
                    case AnpType.String:
                        e = new Element(new String(r.ReadChars((Int32)Base.ntoh((UInt32)r.ReadUInt32()))));
                        break;
                    case AnpType.Bin:
                        e = new Element(r.ReadBytes((Int32)Base.ntoh((UInt32)r.ReadUInt32())));
                        break;
                }
                a.Add(e);
            }

            return a;
        }
    }
}