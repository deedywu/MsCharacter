using System.Collections.Generic;
using Character.Core.Character.Look;

namespace Character.Core.Character.Inventory
{
    public class Equip
    {
        #region 私有成员

        private readonly Dictionary<EquipStat.Id, short> _stats;
        private readonly int _itemId;
        private readonly long _expiration;
        private readonly string _owner;
        private readonly short _flags;
        private readonly short _slots;
        private readonly short _level;
        private readonly short _itemLevel;
        private readonly short _itemExp;
        private readonly int _vicious;
        private readonly Potential _potRank;
        private readonly EquipQuality.Id _quality;

        #endregion

        #region GetItemId

        public int GetItemId() => _itemId;

        #endregion

        #region GetExpiration

        public long GetExpiration() => _expiration;

        #endregion

        #region GetOwner

        public string GetOwner() => _owner;

        #endregion

        #region GetFlags

        public short GetFlags() => _flags;

        #endregion

        #region GetSlots

        public short GetSlots() => _slots;

        #endregion

        #region GetLevel

        public short GetLevel() => _level;

        #endregion

        #region GetItemLevel

        public short GetItemLevel() => _itemLevel;

        #endregion

        #region GetStat

        public short GetStat(EquipStat.Id type) => _stats[type];

        #endregion

        #region GetVicious

        public int GetVicious() => _vicious;

        #endregion

        #region GetPotRank

        public Potential GetPotRank() => _potRank;

        #endregion

        #region GetQuality

        public EquipQuality.Id GetQuality() => _quality;

        #endregion

        #region GetItemExp

        public short GetItemExp() => _itemExp;

        #endregion

        #region 构造函数

        public Equip(int itemId, long expiration, string owner, short flags, short slots, short level,
            Dictionary<EquipStat.Id, short> stats, short itemLevel, short itemExp, int vicious)
        {
            _itemId = itemId;
            _expiration = expiration;
            _owner = owner;
            _flags = flags;
            _slots = slots;
            _level = level;
            _stats = stats;
            _itemLevel = itemLevel;
            _itemExp = itemExp;
            _vicious = vicious;
            _potRank = Potential.POT_NONE;
            _quality = EquipQuality.CheckQuality(itemId, level > 0, stats);
        }

        #endregion

        #region 枚举

        public enum Potential
        {
            POT_NONE,
            POT_HIDDEN,
            POT_RARE,
            POT_EPIC,
            POT_UNIQUE,
            POT_LEGENDARY,
            LENGTH
        };

        #endregion
    }
}