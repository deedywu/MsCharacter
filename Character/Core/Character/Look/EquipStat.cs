﻿﻿﻿using System.Collections;

namespace Character.Core.Character.Look
{
    public class EquipStat
    {
        public enum Id : int
        {
            STR,
            DEX,
            INT,
            LUK,
            HP,
            MP,
            WATK,
            MAGIC,
            WDEF,
            MDEF,
            ACC,
            AVOID,
            HANDS,
            SPEED,
            JUMP,
            LENGTH
        }

        public int ValueOf(Id value)
        {
            return (int) value;
        }

        public Id ById(int id)
        {
            return (Id) id;
        }
        

        internal static readonly Hashtable Names = new Hashtable();

        static EquipStat()
        {
            var arr = new[]
            {
                "STR",
                "DEX",
                "INT",
                "LUK",
                "MaxHP",
                "MaxMP",
                "Attack Power",
                "Magic Attack",
                "Defense",

                // TODO: Does curret GMS use these anymore?
                "MAGIC DEFENSE",
                "ACCURACY",
                "AVOID",
                "HANDS",

                "Speed",
                "Jump"
            };
            for (short i = 0; i < arr.Length; i++)
            {
                Names.Add(arr[i], i);
            }
        }
        
        
    }
}