using System.Collections.Generic;

namespace Character.Core.Character.Look
{
    public static class Stance
    {
        public enum Id : short
        {
            None = 0,
            Alert, // 招架
            Dead, // 死亡
            Fly, // 飞
            Heal, // 治愈
            Jump, // 跳跃
            Ladder, // 爬梯子
            Prone, // 趴下
            ProneStab, // 趴下攻击
            Rope, // 爬绳子
            Shot, // 拉弓
            Shoot1,
            Shoot2,
            ShootF,
            Sit, // 坐
            StabO1,
            StabO2,
            StabOf,
            StabT1,
            StabT2,
            StabTf,
            Stand1,
            Stand2,
            SwingO1,
            SwingO2,
            SwingO3,
            SwingOf,
            SwingP1,
            SwingP2,
            SwingPf,
            SwingT1,
            SwingT2,
            SwingT3,
            SwingTf,
            Walk1,
            Walk2,
            Length
        }

        public static Id ByState(short state)
        {
            var index = (short) (state / 2 - 1);
            if (index < 0 || index > 10)
            {
                return Id.Walk1;
            }

            var stateValues = new[]
                {Id.Walk1, Id.Stand1, Id.Jump, Id.Alert, Id.Prone, Id.Fly, Id.Ladder, Id.Rope, Id.Dead, Id.Sit};
            return stateValues[index];
        }

        public static Id ById(short id)
        {
            if (id <= (short) Id.None || id >= (short) Id.Length)
            {
                return Id.None;
            }

            return (Id) id;
        }

        public static Id ByString(string name)
        {
            foreach (var keyValuePair in Names)
                if (name.Equals(keyValuePair.Value))
                    return keyValuePair.Key;
            return Id.None;
        }

        public static bool IsClimbing(Id value)
        {
            return value == Id.Ladder || value == Id.Rope;
        }

        public static Id BaseOf(Id value)
        {
            switch (value)
            {
                case Id.Stand2:
                    return Id.Stand1;
                case Id.Walk2:
                    return Id.Walk1;
            }

            return value;
        }

        public static Id SecondOf(Id value)
        {
            switch (value)
            {
                case Id.Stand1:
                    return Id.Stand2;
                case Id.Walk1:
                    return Id.Walk2;
            }

            return value;
        }

        public static readonly Dictionary<Id, string> Names = new Dictionary<Id, string>();

        static Stance()
        {
            var arr = new[]
            {
                "", "alert", "dead", "fly", "heal", "jump", "ladder", "prone", "proneStab",
                "rope", "shot", "shoot1", "shoot2", "shootF", "sit", "stabO1", "stabO2", "stabOF",
                "stabT1", "stabT2", "stabTF", "stand1", "stand2", "swingO1", "swingO2",
                "swingO3", "swingOF", "swingP1", "swingP2", "swingPF", "swingT1", "swingT2",
                "swingT3", "swingTF", "walk1", "walk2"
            };
            for (short i = 0; i < arr.Length; i++)
            {
                Names.Add((Id) i, arr[i]);
            }
        }
    }
}