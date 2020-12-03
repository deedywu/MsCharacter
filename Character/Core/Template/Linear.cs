using Character.Core.Util;

namespace Character.Core.Template
{
    public class Linear
    {
        private float _now;
        private float _before;

        public float Get()
        {
            return _now;
        }

        public float Get(float alpha)
        {
            return MxdUtil.Lerp(_before, _now, alpha);
        }

        public float Last() => _before;

        public void Set(float value)
        {
            _now = value;
            _before = value;
        }

        public void Normalize()
        {
            _before = _now;
        }

        public bool Normalized()
        {
            return _before.Equals(_now);
        }

        public void SetNew(float value)
        {
            _before = _now;
            _now = value;
        }

        public void AddSet(float value)
        {
            _before = _now;
            _now += value;
        }

        public void SubSet(float value)
        {
            _before = _now;
            _now -= value;
        }

        public bool Equals(float value)
        {
            return _now.Equals(value);
        }

        public static bool operator <(Linear o, float value)
        {
            return o._now < value;
        }

        public static bool operator <=(Linear o, float value)
        {
            return o._now <= value;
        }

        public static bool operator >(Linear o, float value)
        {
            return o._now > value;
        }

        public static bool operator >=(Linear o, float value)
        {
            return o._now >= value;
        }

        public static float operator +(Linear o, float value)
        {
            return o._now + value;
        }

        public static float operator -(Linear o, float value)
        {
            return o._now - value;
        }

        public static float operator *(Linear o, float value)
        {
            return o._now * value;
        }

        public static float operator /(Linear o, float value)
        {
            return o._now / value;
        }

        public static float operator +(Linear o, Linear value)
        {
            return o._now + value.Get();
        }

        public static float operator -(Linear o, Linear value)
        {
            return o._now - value.Get();
        }

        public static float operator *(Linear o, Linear value)
        {
            return o._now * value.Get();
        }

        public static float operator /(Linear o, Linear value)
        {
            return o._now / value.Get();
        }
    }
}