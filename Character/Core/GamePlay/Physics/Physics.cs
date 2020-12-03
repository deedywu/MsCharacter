using Character.MapleLib.WzLib;
using Microsoft.Xna.Framework;

namespace Character.Core.GamePlay.Physics
{
    public class Physics
    {
        private const float GraVForce = 0.14f;
        private const float SwimGraVForce = 0.03f;
        private const float Friction = 0.3f;
        private const float SlopeFactor = 0.1f;
        private const float GroundSlip = 3.0f;
        private const float FlyFriction = 0.05f;
        private const float SwimFriction = 0.08f;

        public FootholdTree Fht { get; }

        public void MoveObject(PhysicsObject phObj)
        {
            Fht.UpdateFh(phObj);
            switch (phObj.Types)
            {
                case PhysicsObject.Type.Normal:
                    MoveNormal(phObj);
                    Fht.LimitMovement(phObj);
                    break;
                case PhysicsObject.Type.Flying:
                    MoveFlying(phObj);
                    Fht.LimitMovement(phObj);
                    break;
                case PhysicsObject.Type.Swimming:
                    MoveSwimming(phObj);
                    Fht.LimitMovement(phObj);
                    break;
            }

            phObj.Move();
        }

        public void MoveNormal(PhysicsObject phObj)
        {
            phObj.VAcc = 0;
            phObj.HAcc = 0;

            if (phObj.OnGround)
            {
                phObj.VAcc += phObj.VForce;
                phObj.HAcc += phObj.HForce;
                if (phObj.HAcc.Equals(0) && phObj.HSpeed < 0.1f && phObj.HSpeed > -0.1f)
                    phObj.HSpeed = 0;
                else
                {
                    var inertia = phObj.HSpeed / GroundSlip;
                    var slope = phObj.FhSlope;
                    if (slope > 0.5)
                        slope = 0.5f;
                    else if (slope < -0.5f)
                        slope = -0.5f;
                    phObj.HAcc -= (Friction + SlopeFactor * (1.0f + slope * -inertia)) * inertia;
                }
            }
            else if (phObj.IsFlagNotSet(PhysicsObject.Flag.NoGravity))
            {
                phObj.VAcc += GraVForce;
            }

            phObj.HForce = 0;
            phObj.VForce = 0;
            phObj.HSpeed += phObj.HAcc;
            phObj.VSpeed += phObj.VAcc;
        }

        public void MoveFlying(PhysicsObject phObj)
        {
            phObj.HAcc = phObj.HForce;
            phObj.VAcc = phObj.VForce;
            phObj.HForce = 0;
            phObj.VForce = 0;
            phObj.HAcc -= FlyFriction * phObj.HSpeed;
            phObj.VAcc -= FlyFriction * phObj.VSpeed;
            phObj.HSpeed += phObj.HAcc;
            phObj.VSpeed += phObj.VAcc;
            if (phObj.HAcc.Equals(0) && phObj.HSpeed < 0.1 && phObj.HSpeed > -0.1)
                phObj.HSpeed = 0.0f;
            if (phObj.VAcc.Equals(0) && phObj.VSpeed < 0.1 && phObj.VSpeed > -0.1)
                phObj.VSpeed = 0.0f;
        }

        public void MoveSwimming(PhysicsObject phObj)
        {
            phObj.HAcc = phObj.HForce;
            phObj.VAcc = phObj.VForce;
            phObj.HForce = 0f;
            phObj.VForce = 0f;

            phObj.HAcc -= SwimFriction * phObj.HSpeed;
            phObj.VAcc -= SwimFriction * phObj.VSpeed;

            if (phObj.IsFlagNotSet(PhysicsObject.Flag.NoGravity))
                phObj.VAcc += SwimGraVForce;

            phObj.HSpeed += phObj.HAcc;
            phObj.VSpeed += phObj.VAcc;

            if (phObj.HAcc.Equals(0) && phObj.HSpeed < 0.1 && phObj.HSpeed > -0.1)
                phObj.HSpeed = 0f;

            if (phObj.VAcc.Equals(0) && phObj.VSpeed < 0.1 && phObj.VSpeed > -0.1)
                phObj.VSpeed = 0f;
        }

        public Vector2 GetYBelow(Vector2 position)
        {
            var ground = Fht.GetYBelow(position);
            return new Vector2(position.X, ground - 1);
        }

        #region 构造函数

        public Physics(WzObject src)
        {
            Fht = new FootholdTree(src);
        }

        public Physics()
        {
        }

        #endregion
    }
}