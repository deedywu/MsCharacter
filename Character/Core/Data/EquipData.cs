using System.Collections.Generic;
using Character.MapleLib.WzLib.WzProperties;
using Character.Core.Character;
using Character.Core.Character.Look;
using Character.Core.Util;

namespace Character.Core.Data
{
    public class EquipData
    {
        public ItemData ItemData { get; }

        public readonly bool Cash;

        public readonly bool TradeBlock;

        public readonly short Slots;

        public readonly string Type;

        public EquipSlot.Id EqSlot { get; }

        private readonly Dictionary<MapleStat.Id, short> _reqStats = new Dictionary<MapleStat.Id, short>();

        private readonly Dictionary<EquipStat.Id, short> _defStats = new Dictionary<EquipStat.Id, short>();

        private void SetDefStatsValue(EquipStat.Id id, WzSubProperty src, string srcKey)
        {
            if (src[srcKey] != null)
                _defStats[id] = (short) src[srcKey];
        }

        public short GetReqStat(MapleStat.Id stat)
        {
            return _reqStats[stat];
        }

        public short GetDefStat(EquipStat.Id stat)
        {
            return _defStats[stat];
        }

        public bool Valid => ItemData.valid;

        public bool IsWeapon => EqSlot == EquipSlot.Id.Weapon;

        public EquipData(int id)
        {
            ItemData = new ItemData(id);
            var strId = $"0{id}";
            var src = (WzSubProperty) Wz.Character[ItemData.category][$"{strId}.img"]["info"];
            Cash = ((WzIntProperty) src["cash"])?.Value == 1;
            TradeBlock = ((WzIntProperty) src["tradeBlock"])?.Value == 1;

            Slots = (short) (((WzIntProperty) src["tuc"])?.Value ?? 0);
            _reqStats[MapleStat.Id.LEVEL] = (short) src["reqLevel"];
            _reqStats[MapleStat.Id.JOB] = (short) src["reqJob"];
            _reqStats[MapleStat.Id.STR] = (short) src["reqSTR"];
            _reqStats[MapleStat.Id.DEX] = (short) src["reqDEX"];
            _reqStats[MapleStat.Id.INT] = (short) src["reqINT"];
            _reqStats[MapleStat.Id.LUK] = (short) src["reqLUK"];
            SetDefStatsValue(EquipStat.Id.STR, src, "incSTR");
            SetDefStatsValue(EquipStat.Id.DEX, src, "incDEX");
            SetDefStatsValue(EquipStat.Id.INT, src, "incINT");
            SetDefStatsValue(EquipStat.Id.LUK, src, "incLUK");
            SetDefStatsValue(EquipStat.Id.WATK, src, "incPAD");
            SetDefStatsValue(EquipStat.Id.WDEF, src, "incPDD");
            SetDefStatsValue(EquipStat.Id.MAGIC, src, "incMAD");
            SetDefStatsValue(EquipStat.Id.MDEF, src, "incMDD");
            SetDefStatsValue(EquipStat.Id.HP, src, "incMHP");
            SetDefStatsValue(EquipStat.Id.MP, src, "incMMP");
            SetDefStatsValue(EquipStat.Id.ACC, src, "incACC");
            SetDefStatsValue(EquipStat.Id.AVOID, src, "incEVA");
            SetDefStatsValue(EquipStat.Id.HANDS, src, "incHANDS");
            SetDefStatsValue(EquipStat.Id.SPEED, src, "incSPEED");
            SetDefStatsValue(EquipStat.Id.JUMP, src, "incJUMP");
            const int noneWeaponTypes = 15;
            const int weaponOffset = noneWeaponTypes + 15;
            const int weaponTypes = 20;
            var index = (id / 10000) - 100;

            string[] types;
            if (index < noneWeaponTypes)
            {
                types = new[]
                {
                    "HAT", "FACE ACCESSORY", "EYE ACCESSORY", "EARRINGS", "TOP", "OVERALL", "BOTTOM", "SHOES", "GLOVES",
                    "SHIELD", "CAPE", "RING", "PENDANT", "BELT", "MEDAL"
                };

                var equipSlots = new[]
                {
                    EquipSlot.Id.Hat, EquipSlot.Id.Face, EquipSlot.Id.EyeAcc, EquipSlot.Id.EarAcc, EquipSlot.Id.Top,
                    EquipSlot.Id.Top, EquipSlot.Id.Bottom, EquipSlot.Id.Shoes, EquipSlot.Id.Gloves, EquipSlot.Id.Shield,
                    EquipSlot.Id.Cape, EquipSlot.Id.Ring1, EquipSlot.Id.Pendant1, EquipSlot.Id.Belt, EquipSlot.Id.Medal
                };

                Type = types[index];
                EqSlot = equipSlots[index];
            }
            else if (index >= weaponOffset && index < weaponOffset + weaponTypes)
            {
                types = new[]
                {
                    "单手剑", // "ONE-HANDED SWORD",
                    "单手斧", // "ONE-HANDED AXE",
                    "单手钝器", // "ONE-HANDED MACE",
                    "短刀", // "DAGGER",
                    "", "", "", // "", "", "",
                    "短杖", // "WAND",
                    "长杖", // "STAFF",
                    "", // "",
                    "双手剑", // "TWO-HANDED SWORD",
                    "双手斧", // "TWO-HANDED AXE",
                    "双手钝器", // "TWO-HANDED MACE",
                    "长枪", // "SPEAR",
                    "长杖", // "POLEARM",
                    "弓", // "BOW",
                    "弩", // "CROSSBOW",
                    "拳套", // "CLAW",
                    "指节", // "KNUCKLE",
                    "短枪" // "GUN"
                };

                var weaponIndex = index - weaponOffset;
                Type = types[weaponIndex];
                EqSlot = EquipSlot.Id.Weapon;
            }
            else
            {
                Type = "现金道具";
                EqSlot = EquipSlot.Id.None;
            }
        }
    }
}