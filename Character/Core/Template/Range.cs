using Microsoft.Xna.Framework;

namespace Character.Core.Template
{

    public class Range
    {
        public short First { get; }

        public short Second { get; }

        public short Greater => (First > Second) ? First : Second;

        public short Smaller => (First < Second) ? First : Second;

        public short Delta => (short) (Second - First);

        public short Length => (short) (Greater - Smaller);

        public short Center => (short) ((First + Second) / 2);

        public bool Empty => First == Second;

        public bool Contains(short v) => v >= First && v <= Second;

        public bool Contains(Range v) => v.First >= First && v.Second <= Second;

        public bool Overlaps(Range v) => Contains(v.First) || Contains(v.Second) || v.Contains(First) || v.Contains(Second);

        public bool Equals(Range v)
        {
            return First == v.First && Second == v.Second;
        }

        public static Range operator +(Range o, Range v)
        {
            return new Range((short) (o.First + v.First), (short) (o.Second + v.Second));
        }

        public static Range operator -(Range o, Range v)
        {
            return new Range((short) (o.First - v.First), (short) (o.Second - v.Second));
        }

        public static Range operator -(Range o)
        {
            return new Range((short) -o.First, (short) -o.Second);
        }

        public static Range Symmetric(short mid, short tail)
        {
            return new Range((short) (mid - tail), (short) (mid + tail));
        }

        #region 构造函数

        public Range(short first, short second)
        {
            First = first;
            Second = second;
        }

        public Range() : this(0, 0)
        {
        }

        #endregion
    }
    
    public class RangeFloat
    {
        public float First { get; }

        public float Second { get; }

        public float Greater => (First > Second) ? First : Second;

        public float Smaller => (First < Second) ? First : Second;

        public float Delta => Second - First;

        public float Length => Greater - Smaller;

        public float Center => (First + Second) / 2;

        public bool Empty => First .Equals(Second);

        public bool Contains(float v) => v >= First && v <= Second;

        public bool Contains(RangeFloat v) => v.First >= First && v.Second <= Second;

        public bool Overlaps(RangeFloat v) => Contains(v.First) || Contains(v.Second) || v.Contains(First) || v.Contains(Second);

        public bool Equals(RangeFloat v)
        {
            return First .Equals(v.First) && Second .Equals( v.Second);
        }

        public static RangeFloat operator +(RangeFloat o, RangeFloat v)
        {
            return new RangeFloat(o.First + v.First, o.Second + v.Second);
        }

        public static RangeFloat operator -(RangeFloat o, RangeFloat v)
        {
            return new RangeFloat(o.First - v.First, o.Second - v.Second);
        }

        public static RangeFloat operator -(RangeFloat o)
        {
            return new RangeFloat(-o.First, -o.Second);
        }

        public static RangeFloat Symmetric(float mid, float tail)
        {
            return new RangeFloat(mid - tail, mid + tail);
        }

        #region 构造函数

        public RangeFloat(float first, float second)
        {
            First = first;
            Second = second;
        }

        public RangeFloat() : this(0, 0)
        {
        }

        #endregion
    }
}