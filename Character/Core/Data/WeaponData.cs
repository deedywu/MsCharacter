using System.Collections.Generic;
using Character.Core.Audio;
using Character.MapleLib.WzLib.WzProperties;
using Character.Core.Character.Inventory;
using Character.Core.Common;
using Character.Core.Util;
using Character.MapleLib.WzLib;

namespace Character.Core.Data
{
    public class WeaponData
    {
        private readonly Dictionary<bool, WzObject> _useSounds = new Dictionary<bool, WzObject>();

        public readonly EquipData EquipData;

        public readonly Weapon.Type Type;

        public readonly bool TwoHanded;

        public readonly short Speed;

        public readonly short Attack;

        public readonly string AfterImage;

        public bool Valid => EquipData.Valid;

        public string SpeedString()
        {
            switch (Speed)
            {
                case 1:
                    return "快 (1)";
                case 2:
                    return "快 (2)";
                case 3:
                    return "快 (3)";
                case 4:
                    return "快 (4)";
                case 5:
                    return "普通 (5)";
                case 6:
                    return "普通 (6)";
                case 7:
                    return "慢 (7)";
                case 8:
                    return "慢 (8)";
                case 9:
                    return "慢 (9)";
            }

            return "";
        }

        public void Play(bool degenerate)
        {
            Sound.get().Play(_useSounds[degenerate]);
        }

        public short AttackDelay()
        {
            if (Type == Weapon.Type.NONE)
                return 0;
            return (short) (50 - 25 / Speed);
        }

        public WeaponData(int equipId)
        {
            EquipData = GameUtil.GetEquipData(equipId);
            var prefix = equipId / 10000;
            Type = Weapon.ByValue(prefix);
            TwoHanded = (prefix == (int) Weapon.Type.STAFF) ||
                        (prefix >= (int) Weapon.Type.SWORD_2H && prefix <= (int) Weapon.Type.POLEARM) ||
                        (prefix == (int) Weapon.Type.CROSSBOW);
            var src = (WzSubProperty) Wz.Character["Weapon"][$"0{equipId}.img"]["info"];
            Speed = (short) src["attackSpeed"];
            Attack = ((WzShortProperty) src["attack"]).Value;
            var soundSrc = (WzSubProperty) Wz.Sound["Weapon.img"][src.GetString("sfx")].GetByUol();

            _useSounds = new Dictionary<bool, WzObject>()
            {
                {false, soundSrc["Attack"]}
            };
            if (soundSrc["Attack2"] != null) _useSounds[true] = soundSrc["Attack2"];
            else _useSounds[true] = soundSrc["Attack"]; 
            AfterImage = src.GetString("afterImage");
        }
    }
}