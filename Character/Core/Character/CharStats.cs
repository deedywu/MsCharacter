﻿﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Character.Core.Character.Inventory;
using Character.Core.Character.Look;
using Character.Core.Net;

namespace Character.Core.Character
{
    public class CharStats
    {
        #region 私有成员

        private readonly string _name;
        private List<long> _petIds = new List<long>();
        private readonly Job _job = new Job();
        private long _exp;
        private int _mapId;
        private short _portal;
        private Dictionary<int, short> _rank = new Dictionary<int, short>();
        private Dictionary<int, short> _jobRank = new Dictionary<int, short>();
        private readonly Dictionary<MapleStat.Id, short> _baseStats = new Dictionary<MapleStat.Id, short>();
        private readonly Dictionary<EquipStat.Id, int> _totalStats = new Dictionary<EquipStat.Id, int>();
        private readonly Dictionary<EquipStat.Id, int> _buffDeltas = new Dictionary<EquipStat.Id, int>();
        private readonly Dictionary<EquipStat.Id, float> _percentages = new Dictionary<EquipStat.Id, float>();
        private int _maxDamage;
        private int _minDamage;
        private short _honor;
        private short _attackSpeed;
        private short _projectileRange;
        private Weapon.Type _weaponType;
        private float _mastery;
        private float _critical;
        private float _minCri;
        private float _maxCri;
        private float _damagePercent;
        private float _bossDmg;
        private float _ignoreDef;
        private float _stance;
        private float _resistStatus;
        private float _reduceDamage;
        private readonly bool _female;

        private readonly Dictionary<EquipStat.Id, int> _eqStatCaps = new Dictionary<EquipStat.Id, int>()
        {
            {EquipStat.Id.STR, 999},
            {EquipStat.Id.DEX, 999},
            {EquipStat.Id.INT, 999},
            {EquipStat.Id.LUK, 999},
            {EquipStat.Id.HP, 30000},
            {EquipStat.Id.MP, 30000},
            {EquipStat.Id.WATK, 999},
            {EquipStat.Id.MAGIC, 2000},
            {EquipStat.Id.WDEF, 999},
            {EquipStat.Id.MDEF, 999},
            {EquipStat.Id.ACC, 999},
            {EquipStat.Id.AVOID, 999},
            {EquipStat.Id.HANDS, 999},
            {EquipStat.Id.SPEED, 140},
            {EquipStat.Id.JUMP, 123},
        };

        #endregion

        #region InitTotalStats

        public void InitTotalStats()
        {
            _totalStats.Clear();
            _buffDeltas.Clear();
            _percentages.Clear();

            _totalStats[EquipStat.Id.HP] = GetStat(MapleStat.Id.MAXHP);
            _totalStats[EquipStat.Id.MP] = GetStat(MapleStat.Id.MAXMP);
            _totalStats[EquipStat.Id.STR] = GetStat(MapleStat.Id.STR);
            _totalStats[EquipStat.Id.DEX] = GetStat(MapleStat.Id.DEX);
            _totalStats[EquipStat.Id.INT] = GetStat(MapleStat.Id.INT);
            _totalStats[EquipStat.Id.LUK] = GetStat(MapleStat.Id.LUK);
            _totalStats[EquipStat.Id.SPEED] = 100;
            _totalStats[EquipStat.Id.JUMP] = 100;

            _maxDamage = 0;
            _minDamage = 0;
            _honor = 0;
            _attackSpeed = 0;
            _projectileRange = 400;
            _mastery = 0.0f;
            _critical = 0.05f;
            _minCri = 0.5f;
            _maxCri = 0.75f;
            _damagePercent = 0.0f;
            _bossDmg = 0.0f;
            _ignoreDef = 0.0f;
            _stance = 0.0f;
            _resistStatus = 0.0f;
            _reduceDamage = 0.0f;
        }

        #endregion

        #region CloseTotalStats

        public void CloseTotalStats()
        {
            _totalStats[EquipStat.Id.ACC] += CalculateAccuracy();
            foreach (var keyValuePair in _percentages)
            {
                var stat = keyValuePair.Key;
                var total = _totalStats[stat];
                total += (int) (total * keyValuePair.Value);
                SetTotal(stat, total);
            }

            var primary = GetPrimaryStat();
            var secondary = GetSecondaryStat();
            var attack = GetTotal(EquipStat.Id.WATK);
            var multiplier = _damagePercent + (float) attack / 100;
            _maxDamage = (int) ((primary + secondary) * multiplier);
            _minDamage = (int) (((primary * 0.9f * _mastery) + secondary) * multiplier);
        }

        #endregion

        #region CalculateAccuracy

        public int CalculateAccuracy()
        {
            var totalDex = GetTotal(EquipStat.Id.DEX);
            var totalLuk = GetTotal(EquipStat.Id.LUK);
            return (int) (totalDex * 0.8f + totalLuk * 0.5f);
        }

        #endregion

        #region GetPrimaryStat

        private int GetPrimaryStat()
        {
            var primary = _job.GetPrimary(_weaponType);
            return (int) (GetMultiplier() * GetTotal(primary));
        }

        #endregion

        #region GetSecondaryStat

        private int GetSecondaryStat()
        {
            var secondary = _job.GetSecondary(_weaponType);
            return GetTotal(secondary);
        }

        #endregion

        #region GetMultiplier

        private float GetMultiplier()
        {
            switch (_weaponType)
            {
                case Weapon.Type.SWORD_1H:
                    return 4.0f;
                case Weapon.Type.AXE_1H:
                case Weapon.Type.MACE_1H:
                case Weapon.Type.WAND:
                case Weapon.Type.STAFF:
                    return 4.4f;
                case Weapon.Type.DAGGER:
                case Weapon.Type.CROSSBOW:
                case Weapon.Type.CLAW:
                case Weapon.Type.GUN:
                    return 3.6f;
                case Weapon.Type.SWORD_2H:
                    return 4.6f;
                case Weapon.Type.AXE_2H:
                case Weapon.Type.MACE_2H:
                case Weapon.Type.KNUCKLE:
                    return 4.8f;
                case Weapon.Type.SPEAR:
                case Weapon.Type.POLEARM:
                    return 5.0f;
                case Weapon.Type.BOW:
                    return 3.4f;
                default:
                    return 0.0f;
            }
        }

        #endregion

        #region Stat(get/set)

        public void SetStat(MapleStat.Id stat, short value)
        {
            _baseStats[stat] = value;
        }

        public short GetStat(MapleStat.Id stat) => _baseStats[stat];

        #endregion

        #region Total(get/set)

        public void SetTotal(EquipStat.Id stat, int value)
        {
            if (_eqStatCaps.ContainsKey(stat))
            {
                var capValue = _eqStatCaps[stat];
                if (value > capValue)
                    value = capValue;
            }

            _totalStats[stat] = value;
        }

        public int GetTotal(EquipStat.Id stat) => _totalStats[stat];

        #endregion

        #region AddBuff

        public void AddBuff(EquipStat.Id stat, int value)
        {
            var current = GetTotal(stat);
            SetTotal(stat, current + value);
            _buffDeltas[stat] += value;
        }

        #endregion

        #region AddValue

        public void AddValue(EquipStat.Id stat, int value)
        {
            var current = GetTotal(stat);
            SetTotal(stat, current + value);
        }

        #endregion

        #region AddPercent

        public void AddPercent(EquipStat.Id stat, float percent)
        {
            _percentages[stat] += percent;
        }

        #endregion

        #region SetWeaponType

        public void SetWeaponType(Weapon.Type w)
        {
            _weaponType = w;
        }

        #endregion

        #region Exp(get/set)

        public void SetExp(long e)
        {
            _exp = e;
        }

        public long GetExp() => _exp;

        #endregion

        #region SetPortal

        public void SetPortal(short p)
        {
            _portal = p;
        }

        #endregion

        #region SetMastery

        public void SetMastery(float m)
        {
            _mastery = 0.5f + m;
        }

        #endregion

        #region SetDamagePercent

        public void SetDamagePercent(float d)
        {
            _damagePercent = d;
        }

        #endregion

        #region SetReduceDamage

        public void SetReduceDamage(float r)
        {
            _reduceDamage = r;
        }

        #endregion

        #region ChangeJob

        public void ChangeJob(short id)
        {
            _baseStats[MapleStat.Id.JOB] = id;
            _job.ChangeJob(id);
        }

        #endregion

        #region CalculateDamage

        public int CalculateDamage(int mobAtk)
        {
            // TODO 随机的东西,需要找到实际的公式
            var weaponDef = GetTotal(EquipStat.Id.WDEF);
            if (weaponDef == 0) return mobAtk;
            var reduceAtk = mobAtk / 2 + mobAtk / weaponDef;
            return reduceAtk - (int) (reduceAtk + _reduceDamage);
        }

        #endregion

        #region GetBuffDelta

        public int GetBuffDelta(EquipStat.Id stat) => _buffDeltas[stat];

        #endregion

        #region GetRange

        public Rectangle GetRange() => new Rectangle(-_projectileRange, -5, -50, 50);

        #endregion

        #region MapId(get/set)

        public void SetMapId(int id)
        {
            _mapId = id;
        }

        public int GetMapId() => _mapId;

        #endregion

        #region IsDamageBuffed

        public bool IsDamageBuffed() => GetBuffDelta(EquipStat.Id.WATK) > 0 || GetBuffDelta(EquipStat.Id.MAGIC) > 0;

        #endregion

        #region GetName

        public string GetName() => _name;

        #endregion

        #region GetJobName

        public string GetJobName() => _job.GetName();

        #endregion

        #region GetWeaponType

        public Weapon.Type GetWeaponType() => _weaponType;

        #endregion

        #region GetMastery

        public float GetMastery() => _mastery;

        #endregion

        #region GetCritical

        public float GetCritical() => _critical;

        #endregion

        #region GetMinCrit

        public float GetMinCri() => _minCri;

        #endregion

        #region GetMaxCrit

        public float GetMaxCri() => _maxCri;

        #endregion

        #region GetReduceDamage

        public float GetReduceDamage() => _reduceDamage;

        #endregion

        #region GetBossDmg

        public float GetBossDmg() => _bossDmg;

        #endregion

        #region GetIgnoreDef

        public float GetIgnoreDef() => _ignoreDef;

        #endregion

        #region Stance(get/set)

        public void SetStance(float s)
        {
            _stance = s;
        }

        public float GetStance() => _stance;

        #endregion

        #region GetResiStance

        public float GetResStance() => _resistStatus;

        #endregion

        #region GetMaxDamage

        public int GetMaxDamage() => _maxDamage;

        #endregion

        #region GetMinDamage

        public int GetMinDamage() => _minDamage;

        #endregion

        #region GetHonor

        public short GetHonor() => _honor;

        #endregion

        #region AttackSpped(get/set)

        public void SetAttackSpeed(short b)
        {
            _attackSpeed = b;
        }

        public short GetAttackSpeed() => _attackSpeed;

        #endregion

        #region GetJob

        public Job GetJob() => _job;

        #endregion

        #region GetFemale

        public bool GetFemale() => _female;

        #endregion

        #region 构造函数

        public CharStats(StatsEntry s)
        {
            _name = s.Name;
            _petIds = s.PetIds;
            _exp = s.Exp;
            _mapId = s.MapId;
            _portal = s.Portal;
            _rank = s.Rank;
            _jobRank = s.JobRank;
            _baseStats = s.Stats;
            _female = s.Female;
            _job = new Job(_baseStats[MapleStat.Id.JOB]);
            InitTotalStats();
        }

        public CharStats()
        {
        }

        #endregion
    }
}