using System.Collections.Generic;
using Character.Core.Character.Look;
using Character.Core.Util;

namespace Character.Core.Character.Inventory
{
    public static class EquipQuality
    {
        #region 静态方法

        public static Id CheckQuality(int itemId, bool scrolled, Dictionary<EquipStat.Id, short> stats)
        {
            var data = GameUtil.GetEquipData(itemId);
            short delta = 0;
            foreach (var keyValuePair in stats)
            {
                var es = keyValuePair.Key;
                var stat = keyValuePair.Value;
                var defStat = data.GetDefStat(es);
                delta += (short) (stat - defStat);
            }

            if (delta < -5)
                return scrolled ? Id.ORANGE : Id.GREY;
            else if (delta < 7)
                return scrolled ? Id.ORANGE : Id.WHITE;
            else if (delta < 14)
                return Id.BLUE;
            else if (delta < 21)
                return Id.VIOLET;
            else
                return Id.GOLD;
        }

        #endregion

        #region 枚举

        public enum Id
        {
            GREY,
            WHITE,
            ORANGE,
            BLUE,
            VIOLET,
            GOLD
        };

        #endregion
    }
}