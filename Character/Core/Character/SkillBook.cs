using System.Collections.Generic;
using System.Linq;
using Character.Core.Util;

namespace Character.Core.Character
{
    public class SkillBook
    {
        #region 私有成员

        private Dictionary<int, SkillEntry> _skillEntries = new Dictionary<int, SkillEntry>();

        #endregion

        #region SetSkill

        public void SetSkill(int id, int level, int masterLevel, long expiration)
        {
            _skillEntries[id] = new SkillEntry(level, masterLevel, expiration);
        }

        #endregion

        #region HasSkill

        public bool HasSkill(int id) => _skillEntries.ContainsKey(id);

        #endregion

        #region GetLevel

        public int GetLevel(int id) => _skillEntries.ContainsKey(id) ? _skillEntries[id].Level : 0;

        #endregion

        #region GetMasterLevel

        public int GetMasterLevel(int id) => _skillEntries.ContainsKey(id) ? _skillEntries[id].MasterLevel : 0;

        #endregion

        #region GetExpiration

        public long GetExpiration(int id) => _skillEntries.ContainsKey(id) ? _skillEntries[id].Expiration : 0;

        #endregion

        #region CollectPassives

        // 返回id和被动技能水平。
        // 有序映射使用,这样低的被动技能不高覆盖的。
        public Dictionary<int, int> CollectPassives()
        {
            var passives = new Dictionary<int, int>();
            foreach (var keyValuePair in _skillEntries.Where(keyValuePair =>
                GameUtil.GetSkillData(keyValuePair.Key).IsPassive()))
                passives[keyValuePair.Key] = keyValuePair.Value.Level;
            return passives;
        }

        #endregion

        #region CollectRequired

        // 返回id和所有必需的技能水平。
        public Dictionary<int, int> CollectRequired(int id)
        {
            return _skillEntries.ContainsKey(id) ? GameUtil.GetSkillData(id).GetReqSkills() : null;
        }

        #endregion

        #region class SkillEntry

        private struct SkillEntry
        {
            public int MasterLevel { get; }
            public long Expiration { get; }

            public int Level { get; }

            public SkillEntry(int level, int masterLevel, long expiration)
            {
                Level = level;
                MasterLevel = masterLevel;
                Expiration = expiration;
            }
        }

        #endregion
    }
}