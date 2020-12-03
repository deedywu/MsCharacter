﻿using Character.Core.Character.Inventory;
using Character.Core.Character.Look;

namespace Character.Core.Character
{
    public class Job
    {
        #region 私有的成员

        private string _name;

        private short _id;

        private Level _level;

        #endregion

        #region GetName

        private string GetName(short jId)
        {
            switch (jId)
            {
                case 0:
                    return "新手"; // Beginner
                case 100:
                    return "战士"; // Swordsman
                case 110:
                    return "剑客"; // Fighter
                case 111:
                    return "勇士"; // Crusader
                case 112:
                    return "英雄"; // Hero
                case 120:
                    return "准骑士"; // Page
                case 121:
                    return "骑士"; // White Knight
                case 122:
                    return "圣骑士"; // Paladin
                case 130:
                    return "枪战士"; // Spearman
                case 131:
                    return "龙骑士"; // Dragon Knight
                case 132:
                    return "黑骑士"; // Dark Knight
                case 200:
                    return "魔法师"; // Magician
                case 210:
                    return "火毒法师"; // Wizard (F/P)
                case 211:
                    return "火毒巫师"; // Mage (F/P)
                case 212:
                    return "火毒魔导士"; // Archmage (F/P)
                case 220:
                    return "冰雷法师"; // Wizard (I/L)
                case 221:
                    return "冰雷巫师"; // Mage (I/L)
                case 222:
                    return "冰雷魔导士"; // Archmage (I/L)
                case 230:
                    return "牧师"; // Cleric
                case 231:
                    return "祭司"; // Priest
                case 232:
                    return "主教"; // Bishop
                case 300:
                    return "弓箭手"; // Archer
                case 310:
                    return "猎人"; // Hunter
                case 311:
                    return "射手"; // Ranger
                case 312:
                    return "神射手"; // Bowmaster
                case 320:
                    return "弩弓手"; // Crossbowman
                case 321:
                    return "游侠"; // Sniper
                case 322:
                    return "箭神"; // Marksman
                case 400:
                    return "飞侠"; // Rogue
                case 410:
                    return "刺客"; // Assassin
                case 411:
                    return "无影人"; // Hermit
                case 412:
                    return "隐士"; // Nightlord
                case 420:
                    return "侠客"; // Bandit
                case 421:
                    return "独行客"; // Chief Bandit
                case 422:
                    return "侠盗"; // Shadower
                // 双刀
                // case 430:
                //     return "见习刀客";
                // case 431:
                //     return "双刀客";
                // case 432:
                //     return "双刀侠";
                // case 433:
                //     return "血刀";
                // case 434:
                //     return "暗影双刀";
                case 500:
                    return "海盗"; // Pirate
                case 510:
                    return "拳手"; // Brawler
                case 511:
                    return "斗士"; // Marauder
                case 512:
                    return "冲锋队长"; // Buccaneer
                case 520:
                    return "火枪手"; // Gunslinger
                case 521:
                    return "大副"; // Outlaw
                case 522:
                    return "船长"; // Corsair
                case 1000:
                    return "初心者"; // Noblesse
                case 1100:
                case 1110:
                case 1111:
                case 1112:
                    return "魂骑士";
                case 1200:
                case 1210:
                case 1211:
                case 1212:
                    return "炎术士";
                case 1300:
                case 1310:
                case 1311:
                case 1312:
                    return "风灵使者";
                case 1400:
                case 1410:
                case 1411:
                case 1412:
                    return "夜行者";
                case 1500:
                case 1510:
                case 1511:
                case 1512:
                    return "奇袭者";
                case 2000:
                    return "战童";
                // 龙神
                // case 2001:
                // case 2200:
                // case 2210:
                // case 2211:
                // case 2212:
                // case 2213:
                // case 2214:
                // case 2215:
                // case 2216:
                // case 2217:
                // case 2218:
                //     return "龙神";
                case 2100:
                case 2110:
                case 2111:
                case 2112:
                    return "战神"; // Aran
                case 900:
                    return "管理员"; // GM
                case 910:
                    return "超级管理员"; // SuperGM  ? CMS存在?
                default:
                    return "";
            }
        }

        #endregion

        #region ChangeJob

        public void ChangeJob(short i)
        {
            _id = i;
            _name = GetName(i);
            if (_id == 0)
                _level = Level.Beginner;
            else if (_id % 100 == 0)
                _level = Level.First;
            else
                switch (_id % 10)
                {
                    case 0:
                        _level = Level.Second;
                        break;
                    case 1:
                        _level = Level.Third;
                        break;
                    default:
                        _level = Level.Fourth;
                        break;
                }
        }

        #endregion

        #region IsSubJob

        public bool IsSubJob(short subId)
        {
            for (var lvIt = (short) Level.Beginner; lvIt <= (short) Level.Fourth; lvIt++)
            {
                var lv = (Level) lvIt;
                if (subId == GetSubJob(lv)) return true;
            }

            return false;
        }

        #endregion

        #region GetSubJob

        public short GetSubJob(Level lv)
        {
            if (lv <= _level)
            {
                switch (lv)
                {
                    case Level.Beginner:
                        return 0;
                    case Level.First:
                        return (short) ((_id / 100) * 100);
                    case Level.Second:
                        return (short) ((_id / 10) * 10);
                    case Level.Third:
                        return (short) ((_level == Level.Fourth) ? _id - 1 : _id);
                    case Level.Fourth:
                        return _id;
                }
            }

            return 0;
        }

        #endregion

        #region CanUse

        public bool CanUse(int skillId)
        {
            var required = (short) (skillId / 10000);
            return IsSubJob(required);
        }

        #endregion

        #region GetId

        public short GetId() => _id;

        #endregion

        #region GetName

        public string GetName() => _name;

        #endregion

        #region GetLevel

        public Level GetLevel() => _level;

        #endregion

        #region GetPrimary

        public EquipStat.Id GetPrimary(Weapon.Type weaponType)
        {
            switch (_id / 100)
            {
                case 2:
                    return EquipStat.Id.INT;
                case 3:
                    return EquipStat.Id.DEX;
                case 4:
                    return EquipStat.Id.LUK;
                case 5:
                    return (weaponType == Weapon.Type.GUN) ? EquipStat.Id.DEX : EquipStat.Id.STR;
                default:
                    return EquipStat.Id.STR;
            }
        }

        #endregion

        #region GetSecondary

        public EquipStat.Id GetSecondary(Weapon.Type weaponType)
        {
            switch (_id / 100)
            {
                case 2:
                    return EquipStat.Id.LUK;
                case 3:
                    return EquipStat.Id.STR;
                case 4:
                    return EquipStat.Id.DEX;
                case 5:
                    return (weaponType == Weapon.Type.GUN) ? EquipStat.Id.STR : EquipStat.Id.DEX;
                default:
                    return EquipStat.Id.DEX;
            }
        }

        #endregion

        #region 构造函数

        public Job(short id)
        {
            ChangeJob(id);
        }

        public Job()
        {
            ChangeJob(0);
        }

        #endregion

        #region 枚举

        public enum Level : short
        {
            Beginner,
            First,
            Second,
            Third,
            Fourth
        }

        #endregion

        #region 静态方法

        public static Level GetNextLevel(Level level)
        {
            switch (level)
            {
                case Level.Beginner:
                    return Level.First;
                case Level.First:
                    return Level.Second;
                case Level.Second:
                    return Level.Third;
                default:
                    return Level.Fourth;
            }
        }

        #endregion
    }
}