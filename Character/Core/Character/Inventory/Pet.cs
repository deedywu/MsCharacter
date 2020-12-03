namespace Character.Core.Character.Inventory
{
    public class Pet
    {
        #region 私有成员

        private int _itemId;
        private long _expiration;
        private string _petName;
        private short _petLevel;
        private short _closeness;
        private short _fullness;

        #endregion

        #region 构造函数

        public Pet(int itemId, long expiration, string name, short level, short closeness, short fullness)
        {
            _itemId = itemId;
            _expiration = expiration;
            _petName = name;
            _petLevel = level;
            _closeness = closeness;
            _fullness = fullness;
        }

        #endregion
    }
}