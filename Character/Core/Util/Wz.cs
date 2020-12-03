using System;
using Character.MapleLib.WzLib;

namespace Character.Core.Util
{
    public static class Wz
    {
        public static readonly WzFile Character, Effect, Item, Sound, Skill, String;

        static Wz()
        {
            Character = Init("Character");
            Effect = Init("Effect");
            // Etc
            Item = Init("Item");
            // List
            // Map
            // Mob
            // Morph
            // Npc
            // Quest
            // Reactor
            Skill = Init("Skill");
            Sound = Init("Sound");
            String = Init("String");
            // TamingMob
            // UI
        }

        private static WzFile Init(string name)
        {
            var wzFile = new WzFile($"D:/games/C079/{name}.wz", 79, WzMapleVersion.Ems);
            wzFile.ParseWzFile();
            return wzFile;
        }
    }
}