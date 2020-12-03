using Character.Core.GamePlay.Physics;

namespace Character.Core.GamePlay
{
    public class Movement
    {
        public Type Types { get; }
        public short Command { get; }
        public short XPos { get; }
        public short YPos { get; }
        public short LastX { get; }
        public short LastY { get; }
        public short Fh { get; }
        public short NewState { get; }
        public short Duration { get; }

        #region HasMoved

        public bool HasMoved(Movement newMove)
        {
            return newMove.NewState != NewState || newMove.XPos != XPos || newMove.YPos != YPos ||
                   newMove.LastX != LastX || newMove.LastY != LastY;
        }

        #endregion

        #region 构造函数

        public Movement(Type t, short c, short x, short y, short lx, short ly, short f, short s, short d)
        {
            Types = t;
            Command = c;
            XPos = x;
            YPos = y;
            LastX = lx;
            LastY = ly;
            Fh = f;
            NewState = s;
            Duration = d;
        }

        public Movement(short x, short y, short lx, short ly, short s, short d) :
            this(Type.ABSOLUTE, 0, x, y, lx, ly, 0, s, d)
        {
        }

        public Movement(PhysicsObject phObj, short s) :
            this(Type.ABSOLUTE, 0, phObj.GetX, phObj.GetY, phObj.GetLastX, phObj.GetLastY, phObj.FhId, s, 1)
        {
        }

        public Movement() : this((short) Type.NONE, 0, 0, 0, 0, 0)
        {
        }

        #endregion

        #region 枚举

        public enum Type
        {
            NONE,
            ABSOLUTE,
            RELATIVE,
            CHAIR,
            JUMPDOWN
        }

        #endregion
    }
}