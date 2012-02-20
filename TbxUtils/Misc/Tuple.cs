using System;
namespace Tbx.Utils
{
    [Serializable]
    public class Tuple<T1, T2>
    {
        private T1 m_item1;
        private T2 m_item2;

        public T1 Item1
        {
            get { return m_item1; }
        }

        public T2 Item2
        {
            get { return m_item2; }
        }

        public Tuple(T1 first, T2 second)
        {
            m_item1 = first;
            m_item2 = second;
        }
    }

    [Serializable]
    public class Tuple<T1, T2, T3> : Tuple<T1, T2>
    {
        private T3 m_item3;

        public T3 Item3
        {
            get { return m_item3; }
        }

        public Tuple(T1 first, T2 second, T3 third) : base(first, second)
        {
            m_item3 = third;
        }
    }

    [Serializable]
    public class Tuple<T1, T2, T3, T4> : Tuple<T1, T2, T3>
    {
        private T4 m_item4;

        public T4 Item4
        {
            get { return m_item4; }
        }

        public Tuple(T1 first, T2 second, T3 third, T4 fourth)
            : base(first, second, third)
        {
            m_item4 = fourth;
        }
    }

    [Serializable]
    public class Triple<T1, T2, T3> : Tuple<T1, T2, T3>
    {
        public Triple(T1 first, T2 second, T3 third) : base(first, second, third) { }
    }

    [Serializable]
    public class Quad<T1, T2, T3, T4> : Tuple<T1, T2, T3, T4>
    {
        public Quad(T1 first, T2 second, T3 third, T4 fourth) : base(first, second, third, fourth) { }
    }
}