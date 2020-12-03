namespace Character.Core.Character.Inventory
{
    public class Item
    {
        #region 私有成员

        private int _itemId;
        private long _expiration;
        private string _owner;
        private short _flags;

        #endregion

        #region 构造函数

        public Item(int itemId, long expiration, string owner, short flags)
        {
            _itemId = itemId;
            _expiration = expiration;
            _owner = owner;
            _flags = flags;
        }

        #endregion
    }
}