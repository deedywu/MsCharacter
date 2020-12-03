﻿﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Character.Core.Character.Look;
using Character.Core.Util;

namespace Character.Core.Character.Inventory
{
    public class Inventory
    {
        #region 私有成员

        private readonly Dictionary<InventoryType.Id, Dictionary<short, Slot>> _inventories;
        private readonly Dictionary<int, Item> _items = new Dictionary<int, Item>();
        private readonly Dictionary<int, Equip> _equips = new Dictionary<int, Equip>();
        private readonly Dictionary<int, Pet> _pets = new Dictionary<int, Pet>();
        private int _runningUid;
        private readonly Dictionary<EquipStat.Id, short> _totalStats = new Dictionary<EquipStat.Id, short>();
        private readonly Dictionary<InventoryType.Id, short> _slotMaxIma = new Dictionary<InventoryType.Id, short>();
        private long _meso;
        private short _bulletSlot;

        #endregion

        #region AddSlot

        private int AddSlot(InventoryType.Id type, short slot, int itemId, short count, bool cash)
        {
            _runningUid++;
            _inventories[type][slot] = new Slot(_runningUid, itemId, count, cash);
            return _runningUid;
        }

        #endregion

        #region ChangeCount

        private void ChangeCount(InventoryType.Id type, short slot, short count)
        {
            if (!_inventories.ContainsKey(type) || !_inventories[type].ContainsKey(slot)) return;
            var s = _inventories[type][slot];
            s.Count = count;
        }

        #endregion

        #region Swap

        private void Swap(InventoryType.Id firstType, short firstSlot, InventoryType.Id secondType, short secondSlot)
        {
            var first = _inventories[firstType][firstSlot].Clone();
            var second = _inventories[secondType][secondSlot].Clone();
            _inventories[firstType][firstSlot] = null;
            _inventories[firstType][firstSlot] = second;
            _inventories[secondType][secondSlot] = null;
            _inventories[secondType][secondSlot] = first;
            if (_inventories[firstType][firstSlot].ItemId == 0) Remove(firstType, firstSlot);
            if (_inventories[secondType][secondSlot].ItemId == 0) Remove(secondType, secondSlot);
        }

        #endregion

        #region Remove

        private void Remove(InventoryType.Id type, short slot)
        {
            if (!_inventories.ContainsKey(type) || !_inventories[type].ContainsKey(slot)) return;
            var s = _inventories[type][slot];
            var uniqueId = s.UniqueId;
            _inventories[type].Remove(slot);
            switch (type)
            {
                case InventoryType.Id.EQUIPPED:
                case InventoryType.Id.EQUIP:
                    _equips.Remove(uniqueId);
                    break;
                case InventoryType.Id.CASH:
                    _items.Remove(uniqueId);
                    _pets.Remove(uniqueId);
                    break;
                default:
                    _items.Remove(uniqueId);
                    break;
            }
        }

        #endregion

        #region Modify

        // 修改库存信息的数据包。
        public void Modify(InventoryType.Id type, short slot, short mode, short arg, Movement move)
        {
            if (slot < 0)
            {
                slot = (short) -slot;
                type = InventoryType.Id.EQUIPPED;
            }

            arg = (short) (arg < 0 ? -arg : arg);
            switch ((Modification) mode)
            {
                case Modification.CHANGECOUNT:
                    ChangeCount(type, slot, arg);
                    break;
                case Modification.SWAP:
                    switch (move)
                    {
                        case Movement.MOVE_INTERNAL:
                            Swap(type, slot, type, arg);
                            break;
                        case Movement.MOVE_UNEQUIP:
                            Swap(InventoryType.Id.EQUIPPED, slot, InventoryType.Id.EQUIP, arg);
                            break;
                        case Movement.MOVE_EQUIP:
                            Swap(InventoryType.Id.EQUIP, slot, InventoryType.Id.EQUIPPED, arg);
                            break;
                    }

                    break;
                case Modification.REMOVE:
                    Remove(type, slot);
                    break;
            }
        }

        #endregion

        #region ReCalcStats

        // 重新计算装备属性
        public void ReCalcStats(Weapon.Type type)
        {
            _totalStats.Clear();
            foreach (var keyValuePair in _inventories[InventoryType.Id.EQUIPPED])
            {
                if (!_equips.ContainsKey(keyValuePair.Value.UniqueId)) continue;

                var equip = _equips[keyValuePair.Value.UniqueId];

                foreach (var kv2 in _totalStats)
                {
                    _totalStats[kv2.Key] = (short) (kv2.Value + equip.GetStat(kv2.Key));
                }
            }

            int prefix;
            switch (type)
            {
                case Weapon.Type.BOW:
                    prefix = 2060;
                    break;
                case Weapon.Type.CROSSBOW:
                    prefix = 2061;
                    break;
                case Weapon.Type.CLAW:
                    prefix = 2070;
                    break;
                case Weapon.Type.GUN:
                    prefix = 2330;
                    break;
                default:
                    prefix = 0;
                    break;
            }

            _bulletSlot = 0;
            if (prefix > 0)
            {
                foreach (var keyValuePair in _inventories[InventoryType.Id.USE])
                {
                    var slot = keyValuePair.Value;
                    if (slot.Count > 0 && slot.ItemId / 1000 == prefix)
                    {
                        _bulletSlot = keyValuePair.Key;
                        break;
                    }
                }
            }

            // todo 
            //	if (int bulletid = get_bulletid())
            // totalstats[EquipStat.Id.WATK] += BulletData.get(bulletid).get_watk();
        }

        #endregion

        #region SetMeso

        // 设置枫币
        public void SetMeso(long m)
        {
            _meso = m;
        }

        #endregion

        #region SlotMax(get/set)

        // 给指定的格子插入库存
        public void SetSlotMax(InventoryType.Id type, short value)
        {
            _slotMaxIma[type] = value;
        }

        // 返回指定的格子库存
        public short GetSlotMax(InventoryType.Id type) => _slotMaxIma[type];

        #endregion

        #region AddItem

        // 添加一个一般物品
        public void AddItem(InventoryType.Id type, short slot, int itemId, bool cash, long expire, short count,
            string owner, short flags)
        {
            var addSlot = AddSlot(type, slot, itemId, count, cash);
            _items[addSlot] = new Item(itemId, expire, owner, flags);
        }

        #endregion

        #region AddPet

        // 添加一个宠物物品
        public void AddPet(InventoryType.Id type, short slot, int itemId, bool cash, long expire, string name,
            short level, short closeness, short fullness)
        {
            var addSlot = AddSlot(type, slot, itemId, 1, cash);
            _pets[addSlot] = new Pet(itemId, expire, name, level, closeness, fullness);
        }

        #endregion

        #region AddEquip

        // 添加一个装备物品
        public void AddEquip(InventoryType.Id type, short slot, int itemId, bool cash, long expire, short slots,
            short level, Dictionary<EquipStat.Id, short> stats, string owner, short flag, short itemLevel,
            short itemExp, int vicious)
        {
            var addSlot = AddSlot(type, slot, itemId, 1, cash);
            _equips[addSlot] = new Equip(itemId, expire, owner, flag, slots, level, stats, itemLevel, itemExp, vicious);
        }

        #endregion

        #region HasProjectile

        // 检查是否使用库存包含至少一个弹。
        public bool HasProjectile() => _bulletSlot > 0;

        #endregion

        #region HasEquipped

        // 是否已装备
        public bool HasEquipped(EquipSlot.Id slot)
        {
            return _inventories.ContainsKey(InventoryType.Id.EQUIPPED) &&
                   _inventories[InventoryType.Id.EQUIPPED].ContainsKey((short) slot);
        }

        #endregion

        #region GetBulletSlot

        // 返回当前活跃的抛物槽
        public short GetBulletSlot() => _bulletSlot;

        #endregion

        #region GetBulletCount

        // 返回当前的计数活动弹
        public short GetBulletCount()
        {
            return GetItemCount(InventoryType.Id.USE, _bulletSlot);
        }

        #endregion

        #region GetBulletId

        // 返回当前的itemId主动弹
        public int GetBulletId()
        {
            return GetItemId(InventoryType.Id.USE, _bulletSlot);
        }

        #endregion

        #region GetStat

        // 返回一个总计数
        public short GetStat(EquipStat.Id type) => _totalStats[type];

        #endregion

        #region GetMeso

        public long GetMeso() => _meso;

        #endregion

        #region FindEquipSlot

        // 找到一个可插槽指定的装备
        public EquipSlot.Id FindEquipSlot(int itemId)
        {
            var cloth = GameUtil.GetEquipData(itemId);
            if (!cloth.Valid) return EquipSlot.Id.None;
            var eqSlot = cloth.EqSlot;
            if (eqSlot == EquipSlot.Id.Ring1)
            {
                if (!HasEquipped(EquipSlot.Id.Ring2))
                    return EquipSlot.Id.Ring2;

                if (!HasEquipped(EquipSlot.Id.Ring3))
                    return EquipSlot.Id.Ring3;

                return !HasEquipped(EquipSlot.Id.Ring4) ? EquipSlot.Id.Ring4 : EquipSlot.Id.Ring1;
            }

            return eqSlot;
        }

        #endregion

        #region FindFreeSlot

        // 找到一个免费的槽在指定的库存。
        public short FindFreeSlot(InventoryType.Id type)
        {
            short counter = 1;
            foreach (var keyValuePair in _inventories[type])
            {
                if (keyValuePair.Key != counter) return counter;
                counter++;
            }

            return (short) (counter <= _slotMaxIma[type] ? counter : 0);
        }

        #endregion

        #region FindItem

        // 返回指定的项包含第一个槽
        public short FindItem(InventoryType.Id type, int itemId)
        {
            return (from keyValuePair in _inventories[type]
                where keyValuePair.Value.ItemId == itemId
                select keyValuePair.Key).FirstOrDefault();
        }

        #endregion

        #region GetItemCount

        public short GetItemCount(InventoryType.Id type, short slot)
        {
            // 返回条目的数量.如果插槽是空的返回0 
            if (_inventories.ContainsKey(type) && _inventories[type].ContainsKey(slot))
                return _inventories[type][slot].Count;
            return 0;
        }

        #endregion

        #region GetTotalItemCount

        // 返回条目的总数。如果没有发现项目的实例返回0。
        public short GetTotalItemCount(int itemId)
        {
            var type = InventoryType.ByItemId(itemId);
            short totalCount = 0;
            foreach (var keyValuePair in _inventories[type])
                if (keyValuePair.Value.ItemId == itemId)
                    totalCount += keyValuePair.Value.Count;
            return totalCount;
        }

        #endregion

        #region GetItemId

        // 返回一个项目的id.如果插槽是空的返回0
        public int GetItemId(InventoryType.Id type, short slot)
        {
            if (_inventories.ContainsKey(type) && _inventories[type].ContainsKey(slot))
                return _inventories[type][slot].ItemId;
            return 0;
        }

        #endregion

        #region GetEquip

        public Equip GetEquip(InventoryType.Id type, short slot)
        {
            if (type != InventoryType.Id.EQUIPPED && type != InventoryType.Id.EQUIP) return null;
            if (!_inventories.ContainsKey(type) || !_inventories[type].ContainsKey(slot)) return null;
            var equip = _inventories[type][slot];
            return _equips.ContainsKey(equip.UniqueId) ? _equips[equip.UniqueId] : null;
        }

        #endregion

        #region 构造函数

        public Inventory()
        {
            _bulletSlot = 0;
            _meso = 0;
            _runningUid = 0;
            _slotMaxIma[InventoryType.Id.EQUIPPED] = (short) EquipSlot.Id.Length;
            _inventories = new Dictionary<InventoryType.Id, Dictionary<short, Slot>>();
        }

        #endregion

        #region 静态方法

        public static Movement MovementByValue(short value)
        {
            if (value >= (short) Movement.MOVE_INTERNAL && value <= (short) Movement.MOVE_EQUIP)
                return (Movement) value;
            return Movement.MOVE_NONE;
        }

        #endregion

        #region 枚举

        public enum Movement : short
        {
            MOVE_NONE = -1,
            MOVE_INTERNAL = 0,
            MOVE_UNEQUIP = 1,
            MOVE_EQUIP = 2
        }

        public enum Modification : short
        {
            ADD,
            CHANGECOUNT,
            SWAP,
            REMOVE,
            ADDCOUNT
        }

        #endregion

        #region class Slot

        private class Slot
        {
            public int UniqueId { get; }
            public int ItemId { get; }
            public short Count { get; set; }
            public bool Cash { get; }


            public Slot(int uniqueId, int itemId, short count, bool cash)
            {
                UniqueId = uniqueId;
                ItemId = itemId;
                Count = count;
                Cash = cash;
            }

            public Slot Clone()
            {
                return new Slot(UniqueId, ItemId, Count, Cash);
            }
        }

        #endregion
    }
}