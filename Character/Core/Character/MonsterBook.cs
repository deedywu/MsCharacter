using System.Collections.Generic;

namespace Character.Core.Character
{
    public class MonsterBook
    {
        private int _cover;
        private readonly Dictionary<short, short> _cards = new Dictionary<short, short>();

        #region SetCover

        public void SetCover(int cover)
        {
            _cover = cover;
        }

        #endregion

        #region AddCard


        public void AddCard(short a,short b)
        {
            _cards[a] =b;
        }

        #endregion


        #region 构造函数

        public MonsterBook()
        {
            _cover = 0;
        }

        #endregion
    }
}