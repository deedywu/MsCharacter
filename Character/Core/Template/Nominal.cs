namespace Character.Core.Template
{
    public class Nominal<T>
    {
        private T _now;
        private T _before;
        private float _threshold;

        public T Get()
        {
            return _now;
        }

        public T Get(float alpha)
        {
            return alpha >= _threshold ? _now : _before;
        }

        public T Last()
        {
            return _before;
        }

        public void Set(T value)
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

        public void Next(T value, float thr)
        {
            _before = _now;
            _now = value;
            _threshold = thr;
        }
    }
}