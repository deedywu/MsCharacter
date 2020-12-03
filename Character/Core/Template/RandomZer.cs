using System;

namespace Character.Core.Template
{
    public class RandomZer
    {
        public static int NextInt(int to) => NextInt(0, to);

        public static int NextInt(int from, int to)
        {
            if (from >= to) return from;
            var ran = new Random();
            return ran.Next(from, to - 1);
        }
    }
}