using System.Collections.Generic;
using System.Linq;

namespace Character.Core.Character
{
    public class QuestLog
    {
        #region 私有成员

        private readonly Dictionary<short, string> _started = new Dictionary<short, string>();

        private readonly Dictionary<short, Dictionary<short, string>> _inProgress =
            new Dictionary<short, Dictionary<short, string>>();

        private readonly Dictionary<short, long> _completed = new Dictionary<short, long>();

        #endregion

        #region AddStarted

        public void AddStarted(short qId, string qDate)
        {
            _started[qId] = qDate;
        }

        #endregion

        #region AddInProGress

        public void AddInProgress(short qId, short qIdl, string qData)
        {
            if (!_inProgress.ContainsKey(qId)) _inProgress[qId] = new Dictionary<short, string>();
            _inProgress[qId][qIdl] = qData;
        }

        #endregion

        #region AddCompleted

        public void AddCompleted(short qId, long time)
        {
            _completed[qId] = time;
        }

        #endregion

        public bool IsStarted(short qId)
        {
            return _started.ContainsKey(qId);
        }

        #region GetLastStated

        public short GetLastStated()
        {
            return _started.Last().Key;
        }

        #endregion
    }
}