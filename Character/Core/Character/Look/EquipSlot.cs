namespace Character.Core.Character.Look
{
    public static class EquipSlot
    {
        public static Id ById(short id)
        {
            if (id > (short) Id.Length)
                return Id.None;
            return (Id) id;
        }

        public enum Id : short
        {
            None = 0,
            Hat = 1,
            Face = 2,
            EyeAcc = 3,
            EarAcc = 4,
            Top = 5,
            Bottom = 6,
            Shoes = 7,
            Gloves = 8,
            Cape = 9,
            Shield = 10,
            Weapon = 11,
            Ring1 = 12,
            Ring2 = 13,
            Ring3 = 15,
            Ring4 = 16,
            Pendant1 = 17,
            TamedMob = 18,
            Saddle = 19,
            Medal = 49,
            Belt = 50,
            Pocket,
            Book,
            Pendant2,
            Shoulder,
            Android,
            Emblem,
            Badge,
            SubWeapon,
            Heart,
            Length
        }
    }
}