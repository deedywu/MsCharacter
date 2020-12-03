namespace Character.Core.Character.Inventory
{
    public static class InventoryType
    {
        public static Id ByItemId(int itemId)
        {
            var valuesById = new Id[] {Id.NONE, Id.EQUIP, Id.USE, Id.SETUP, Id.ETC, Id.CASH};

            var prefix = itemId / 1000000;
            return (prefix > (int) Id.NONE && prefix <= (int) Id.CASH ? valuesById[prefix] : Id.NONE);
        }

        public static Id ByValue(short value)
        {
            switch (value)
            {
                case -1:
                    return Id.EQUIPPED;
                case 1:
                    return Id.EQUIP;
                case 2:
                    return Id.USE;
                case 3:
                    return Id.SETUP;
                case 4:
                    return Id.ETC;
                case 5:
                    return Id.CASH;
            }


            return Id.NONE;
        }


        #region 枚举

        public enum Id : short
        {
            NONE,
            EQUIP,
            USE,
            SETUP,
            ETC,
            CASH,
            EQUIPPED,
            LENGTH
        }

        #endregion
    }

    public class InventoryPosition
    {
        public InventoryType.Id Type;
        public short Slot;
    }
}