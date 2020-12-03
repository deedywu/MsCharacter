using System.Collections.Generic;
using Character.MapleLib.WzLib;
using Character.MapleLib.WzLib.WzProperties;
using Character.Core.Common;
using Character.Core.Util;

namespace Character.Core.Data
{
    public class ItemData
    {
        private readonly Dictionary<bool, TextureD> _icons;

        public readonly int itemId;

        public readonly bool valid;

        public readonly bool unTradAble;

        public readonly bool unique;

        public readonly bool unSellAble;

        public readonly bool cashItem;

        public readonly string category;

        public readonly int price;

        public readonly short gender;

        public readonly string name;

        public readonly string desc;

        public ItemData(int id)
        {
            _icons = new Dictionary<bool, TextureD>();
            itemId = id;
            unTradAble = false;
            unique = false;
            unSellAble = false;
            cashItem = false;
            WzObject src = null, strSrc = null;
            var strPrefix = $"0{GetItemPrefix(itemId)}";
            var strId = $"0{itemId}";
            var prefix = GetPrefix(itemId);
            switch (prefix)
            {
                case 1:
                    category = GetEqCategory(itemId);
                    src = Wz.Character[category][$"{strId}.img"]["info"];
                    strSrc = Wz.String["Eqp.img"]["Eqp"][category][$"{itemId}"];
                    break;
                case 2:
                    category = "Consume";
                    src = Wz.Item[category][$"{strPrefix}.img"][strId]["info"];
                    strSrc = Wz.String[$"{category}.img"][$"{itemId}"];
                    break;
                case 3:
                    category = "Install";
                    src = Wz.Item[category][$"{strPrefix}.img"][strId]["info"];
                    strSrc = Wz.String[$"{category}.img"][$"{itemId}"];
                    break;
                case 4:
                    category = "Etc";
                    src = Wz.Item[category][$"{strPrefix}.img"][strId]["info"];
                    strSrc = Wz.String[$"{category}.img"][$"{itemId}"];
                    break;
                case 5:
                    category = "Cash";
                    src = Wz.Item[category][$"{strPrefix}.img"][strId]["info"];
                    strSrc = Wz.String[$"{category}.img"][$"{itemId}"];
                    break;
            }

            if (src != null)
            {
                _icons[false] = new TextureD(src["icon"]);
                _icons[true] = new TextureD(src["iconRaw"]);
                price = ((WzIntProperty) src["price"])?.Value ?? 0;
                unTradAble = ((WzIntProperty) src["tradeBlock"])?.Value == 1;
                unique = ((WzIntProperty) src["only"])?.Value == 1;
                unSellAble = ((WzIntProperty) src["notSale"])?.Value == 1;
                cashItem = ((WzIntProperty) src["cash"])?.Value == 1;
                gender = GetItemGender(itemId);
                name = ((WzStringProperty) strSrc["name"]).Value;
                desc = ((WzStringProperty) strSrc["desc"])?.Value;
                valid = true;
            }
            else
            {
                valid = false;
            }
        }

        private static string GetEqCategory(int id)
        {
            var arr = new[]
            {
                "Cap", "Accessory", "Accessory", "Accessory", "Coat", "Longcoat", "Pants", "Shoes", "Glove", "Shield",
                "Cape", "Ring", "Accessory", "Accessory", "Accessory"
            };
            var index = GetItemPrefix(id) - 100;
            if (index < arr.Length)
                return arr[index];
            else if (index >= 30 && index <= 70)
                return "Weapon";
            return "";
        }

        private static int GetPrefix(int id)
        {
            return id / 1000000;
        }

        private static int GetItemPrefix(int id)
        {
            return id / 10000;
        }

        private static short GetItemGender(int id)
        {
            var itemPrefix = GetItemPrefix(id);
            if ((GetPrefix(id) != 1 && itemPrefix != 254) || itemPrefix == 119 || itemPrefix == 168)
                return 2;
            var genderDigit = id / 1000 % 10;
            return (short) (genderDigit > 1 ? 2 : genderDigit);
        }

        public TextureD GetIcon(bool raw)
        {
            return _icons[raw];
        }
    }
}