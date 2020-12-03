using System.Collections.Generic;
using System.Linq;
using Character.MapleLib.WzLib.WzProperties;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Character.Core.Character;
using Character.Core.Character.Inventory;
using Character.Core.Common;
using Character.Core.Util;

namespace Character.Core.Data
{
    public class SkillData
    {
        #region 私有成员

        private readonly Dictionary<int, Stats> _stats = new Dictionary<int, Stats>();
        private string _element;
        private readonly Weapon.Type _reqWeapon;
        private readonly int _masterLevel;
        private readonly int _flags;
        private readonly bool _passive;
        private readonly bool _invisible;
        private readonly string _name;
        private readonly string _desc;
        private readonly Dictionary<int, string> _levels = new Dictionary<int, string>();
        private readonly Dictionary<int, int> _reqSkills = new Dictionary<int, int>();
        private readonly TextureD[] _icons;

        #endregion

        #region FlagsOf todo

        private int FlagsOf(int id)
        {
            var skillFlags = new Dictionary<int, int>()
            {
                // 新手 
                {(int) SkillId.Id.THREE_SNAILS, (int) Flags.Attack},
                // 战士
                {(int) SkillId.Id.POWER_STRIKE, (int) Flags.Attack},
                {(int) SkillId.Id.SLASH_BLAST, (int) Flags.Attack},
                // Fighter
                // Page
                // Crusader
                {(int) SkillId.Id.SWORD_PANIC, (int) Flags.Attack},
                {(int) SkillId.Id.AXE_PANIC, (int) Flags.Attack},
                {(int) SkillId.Id.SWORD_COMA, (int) Flags.Attack},
                {(int) SkillId.Id.AXE_COMA, (int) Flags.Attack},
                // Hero
                {(int) SkillId.Id.RUSH_HERO, (int) Flags.Attack},
                {(int) SkillId.Id.BRANDISH, (int) Flags.Attack},
                // Page
                // White Knight
                {(int) SkillId.Id.CHARGE, (int) Flags.Attack},
                // Paladin
                {(int) SkillId.Id.RUSH_PALADIN, (int) Flags.Attack},
                {(int) SkillId.Id.BLAST, (int) Flags.Attack},
                {(int) SkillId.Id.HEAVENS_HAMMER, (int) Flags.Attack},
                // Spearman
                // Dragon Knight
                {(int) SkillId.Id.DRAGON_BUSTER, (int) Flags.Attack},
                {(int) SkillId.Id.DRAGON_FURY, (int) Flags.Attack},
                {(int) SkillId.Id.PA_BUSTER, (int) Flags.Attack},
                {(int) SkillId.Id.PA_FURY, (int) Flags.Attack},
                {(int) SkillId.Id.SACRIFICE, (int) Flags.Attack},
                {(int) SkillId.Id.DRAGONS_ROAR, (int) Flags.Attack},
                // Dark Knight
                {(int) SkillId.Id.RUSH_DK, (int) Flags.Attack},
                // Mage
                {(int) SkillId.Id.ENERGY_BOLT, (int) Flags.Attack | (int) Flags.Ranged},
                {(int) SkillId.Id.MAGIC_CLAW, (int) Flags.Attack | (int) Flags.Ranged},
                // F/P Mage
                {(int) SkillId.Id.SLOW_FP, (int) Flags.Attack},
                {(int) SkillId.Id.FIRE_ARROW, (int) Flags.Attack | (int) Flags.Ranged},
                {(int) SkillId.Id.POISON_BREATH, (int) Flags.Attack | (int) Flags.Ranged},
                // F/P ArchMage
                {(int) SkillId.Id.EXPLOSION, (int) Flags.Attack},
                {(int) SkillId.Id.POISON_BREATH, (int) Flags.Attack},
                {(int) SkillId.Id.SEAL_FP, (int) Flags.Attack},
                {(int) SkillId.Id.ELEMENT_COMPOSITION_FP, (int) Flags.Attack | (int) Flags.Ranged},
                //
                {(int) SkillId.Id.FIRE_DEMON, (int) Flags.Attack},
                {(int) SkillId.Id.PARALYZE, (int) Flags.Attack | (int) Flags.Ranged},
                {(int) SkillId.Id.METEOR_SHOWER, (int) Flags.Attack}
            };
            if (skillFlags.ContainsKey(id)) return skillFlags[id];
            return (int) Flags.None;
        }

        #endregion

        #region IsPassive todo

        // 返回是否被动技能
        public bool IsPassive() => _passive;

        #endregion

        #region IsAttack  

        // 返回是否技能是攻击技能
        public bool IsAttack() => !_passive && (_flags & (int) Flags.Attack) != 0;

        #endregion

        #region IsInvisible todo

        // 返回是否这个技能是无形的技能书ui。
        public bool IsInvisible() => _invisible;

        #endregion

        #region GetMasterLevel todo

        // 返回默认大师技能
        public int GetMasterLevel() => _masterLevel;

        #endregion

        #region GetRequiredWeapon todo

        // 返回所需的武器
        public Weapon.Type GetRequiredWeapon() => _reqWeapon;

        #endregion

        #region GetStats todo

        // 返回一个级别的统计数据
        // 如果没有统计水平,将返回一个默认对象。
        public Stats GetStats(int level)
        {
            return _stats.ContainsKey(level)
                ? _stats[level]
                : new Stats(0.0f, 0, 0, 0, 0, 0, 0, 0,
                    0, 0, 0.0f, 0.0f, 0.0f, 0.0f, new Rectangle());
        }

        #endregion

        #region GetName todo

        // 返回技能名称 
        public string GetName() => _name;

        #endregion

        #region GetDesc todo

        // 返回技能描述
        public string GetDesc() => _desc;

        #endregion

        #region GetLevelDesc todo

        // 返回等级描述
        // 如果没有描述这一水平,将返回一条警告消息
        public string GetLevelDesc(int level) => _levels.ContainsKey(level) ? _levels[level] : "";

        #endregion

        #region GetIcon

        // 返回技能图标
        // 如果是一个有效的枚举类型不能失败
        public TextureD GetIcon(Icon icon) => _icons[(short) icon];

        #endregion

        #region ReqSkills

        // 返回id和所有必需的技能水平。
        public Dictionary<int, int> GetReqSkills() => _reqSkills;

        #endregion

        #region 构造函数

        public SkillData(int id)
        {
            var strId = id.ToString().PadLeft(7, '0');
            var jobId = strId.Substring(0, 3);
            var src = Wz.Skill[$"{jobId}.img"]["skill"][strId];
            var strSrc = (WzSubProperty) Wz.String["Skill.img"][strId];
            // 加载图标
            _icons = new[]
            {
                new TextureD(src["icon"]), new TextureD(src["iconDisabled"]), new TextureD(src["iconMouseOver"]),
            };
            // 加载文字
            _name = strSrc["name"].GetString();
            _desc = strSrc["desc"].GetString();
            for (var level = 1; level < strSrc.WzProperties.Count; level++)
            {
                if (strSrc[$"h{level}"] == null) break;
                var hDesc = (WzStringProperty) strSrc[$"h{level}"];
                _levels[level] = hDesc.Value;
            }

            // 加载统计
            var levelSrc = (WzSubProperty) src["level"];
            foreach (var sub in levelSrc.WzProperties)
            {
                var damage = sub["damage"].GetFloat() / 100;
                var mAtx = sub["mad"]?.GetInt() ?? 0;
                var fixDamage = sub["fixdamage"]?.GetInt() ?? 0;
                var mastery = sub["mastery"]?.GetInt() ?? 0;
                var attackCount = sub["attackCount"]?.GetShort() ?? 1;
                var mobCount = sub["mobCount"]?.GetShort() ?? 1;
                var bulletCount = sub["bulletCount"]?.GetShort() ?? 1;
                var bulletCost = sub["bulletConsume"]?.GetShort() ?? bulletCount;
                var hpCost = sub["hpCon"]?.GetShort() ?? 1;
                var mpCost = sub["mpCon"]?.GetShort() ?? 1;

                var chance = sub["prop"]?.GetFloat() / 100 ?? 0;
                var critical = 0f;
                var ignoreDef = 0f;
                var hRange = sub["range"]?.GetFloat() / 100 ?? 0;
                var (left, top) = sub["lt"]?.Pos() ?? new Vector2();
                var (right, bottom) = sub["rb"]?.Pos() ?? new Vector2();
                var range = new Rectangle((int) left, (int) right, (int) top, (int) bottom);
                var b = int.TryParse(sub.Name, out var level);
                if (!b) level = -1;
                _stats[level] = new Stats(damage, mAtx, fixDamage, mastery, attackCount, mobCount, bulletCount,
                    bulletCost, hpCost, mpCost, chance, critical, ignoreDef, hRange, range);
            }

            _element = src["elemAttr"]?.GetString();
            if (jobId.Equals("900") || jobId.Equals("910"))
                _reqWeapon = Weapon.Type.NONE;
            else
                _reqWeapon = Weapon.ByValue(100 + src["weapon"]?.GetInt() ?? 0);
            _masterLevel = _stats.Count;
            _passive = id % 10000 / 100 == 0;
            _flags = FlagsOf(id);
            _invisible = src["invisible"] == 1;
            // 加载必须技能
            if (src["req"] == null) return;
            foreach (var sub in ((WzSubProperty) src["req"]).WzProperties.Cast<WzSubProperty>())
            {
                if (!int.TryParse(sub.Name, out var skillId)) continue;
                var reqLv = sub.GetInt();
                _reqSkills[skillId] = reqLv;
            }
        }

        #endregion

        #region 枚举

        public enum Flags
        {
            None = 0x0000,
            Attack = 0x0001,
            Ranged = 0x0002
        }

        public enum Icon : short
        {
            Normal,
            Disabled,
            Mouseover
        }

        #endregion

        #region struct Stats

        public struct Stats
        {
            public float Damage { get; }
            public int MAtk { get; }
            public int FixDamage { get; }
            public int Mastery { get; }
            public short AttackCount { get; }
            public short MobCount { get; }
            public short BulletCount { get; }
            public short BulletCost { get; }
            public int HpCost { get; }
            public int MpCost { get; }
            public float Chance { get; }
            public float Critical { get; }
            public float IgnoreDef { get; }
            public float HRange { get; }
            public Rectangle Range { get; }

            public Stats(float damage, int mAtk, int fixDamage, int mastery, short attackCount, short mobCount,
                short bulletCount, short bulletCost, int hpCost, int mpCost, float chance, float critical,
                float ignoreDef, float hRange, Rectangle range)
            {
                Damage = damage;
                MAtk = mAtk;
                FixDamage = fixDamage;
                Mastery = mastery;
                AttackCount = attackCount;
                MobCount = mobCount;
                BulletCount = bulletCount;
                BulletCost = bulletCost;
                HpCost = hpCost;
                MpCost = mpCost;
                Chance = chance;
                Critical = critical;
                IgnoreDef = ignoreDef;
                HRange = hRange;
                Range = range;
            }
        }

        #endregion
    }
}