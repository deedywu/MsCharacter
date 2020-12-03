namespace Character.Core.Character.Inventory
{
    public static class Weapon
    {
        #region 静态方法

        public static Type ByValue(int value)
        {
            if (value < 130 || (value > 133 && value < 137) || value == 139 ||
                (value > 149 && value < 170) || value > 170)
                if (value != 100)
                    return Type.NONE;
            return (Type) value;
        }

        #endregion

        #region 枚举

        public enum Type
        {
            NONE = 0,
            SWORD_1H = 130,
            AXE_1H = 131,
            MACE_1H = 132,
            DAGGER = 133,
            WAND = 137,
            STAFF = 138,
            SWORD_2H = 140,
            AXE_2H = 141,
            MACE_2H = 142,
            SPEAR = 143,
            POLEARM = 144,
            BOW = 145,
            CROSSBOW = 146,
            CLAW = 147,
            KNUCKLE = 148,
            GUN = 149,
            CASH = 170
        }

        #endregion
    }
}