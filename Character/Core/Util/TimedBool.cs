namespace Character.Core.Util
{
    public class TimedBool
    {
        private long last;
        private long delay;

        public bool Bool { get; private set; }

        public void SetFor(long millis)
        {
            last = millis;
            delay = millis;
            Bool = true;
        }

        public void Update()
        {
            Update(Util.GameUtil.TimeStep);
        }

        public void Update(short timeStep)
        {
            if (!Bool) return;
            if (timeStep >= delay)
            {
                Bool = false;
                delay = 0;
            }
            else
            {
                delay -= timeStep;
            }
        }

        public void SetNew(bool b)
        {
            Bool = b;
            delay = 0;
            last = 0;
        }
        
        public float Alpha => 1.0f - ((float) (delay) / last);

        public TimedBool()
        {
            Bool = false;
            delay = 0;
            last = 0;
        }
    }
}