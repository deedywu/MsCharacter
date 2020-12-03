using System;
using Microsoft.Xna.Framework;
using Character.Core.Template;
using Character.Core.Util;

namespace Character.Core.GamePlay.Physics
{
    public class MovingObject
    {
        public Linear X { get; set; } = new Linear();
        public Linear Y { get; set; } = new Linear();
        
        public float HSpeed;
        public float VSpeed;

        public void Normalize()
        {
            X.Normalize();
            Y.Normalize();
        }

        public void Move()
        {
            X.Set(X.Get() + HSpeed);
            Y.Set(Y.Get() + VSpeed);
        }

        public void SetX(float f) => X.Set(f);

        public void SetY(float f) => Y.Set(f);

        public void LimitX(float f)
        {
            X.Set(f);
            HSpeed = 0;
        }

        public void LimitY(float f)
        {
            Y.Set(f);
            VSpeed = 0;
        }

        public void MoveXUnitL(float f, short delay)
        {
            if (delay == 0) return;
            var hDelta = f - X.Get();
            HSpeed = GameUtil.TimeStep * hDelta / delay;
        }

        public void MoveYUnitL(float f, short delay)
        {
            if (delay == 0) return;
            var vDelta = f - Y.Get();
            VSpeed = GameUtil.TimeStep * vDelta / delay;
        }

        public bool HMobile => !HSpeed.Equals(0);

        public bool VMobile => !VSpeed.Equals(0);

        public bool Mobile => HMobile || VMobile;

        public float CurrentX => X.Get();

        public float CurrentY => Y.Get();

        public float NextX => X + HSpeed;

        public float NextY => Y + VSpeed;

        public short GetX => (short) Math.Round(X.Get());

        public short GetY => (short) Math.Round(Y.Get());

        public short GetLastX => (short) Math.Round(X.Last());

        public short GetLastY => (short) Math.Round(Y.Last());

        public Vector2 GetPosition => new Vector2(GetX, GetY);

        public short GetAbsoluteX(float viewX, float alpha)
        {
            var interX = X.Normalized() ? Math.Round(X.Get()) : X.Get(alpha);
            return (short) Math.Round((float) interX + viewX);
        }

        public short GetAbsoluteY(float viewY, float alpha)
        {
            var interY = Y.Normalized() ? Math.Round(Y.Get()) : Y.Get(alpha);
            return (short) Math.Round((float) interY + viewY);
        }

        public Vector2 GetAbsolute(float viewX, float viewY, float alpha) =>
            new Vector2(GetAbsoluteX(viewX, alpha), GetAbsoluteY(viewY, alpha));
    }

    public class PhysicsObject : MovingObject
    {
        public Type Types = Type.Normal;
        public int Flags { get; set; }
        public short FhId { get; set; }
        public float FhSlope { get; set; }

        public short FhLayer { get; set; }

        public float GroundBelow { get; set; }

        public bool OnGround { get; set; } = true;

        public bool EnableJd { get; set; }

        public float HForce { get; set; } = 0f;

        public float VForce { get; set; } = 0f;

        public float HAcc { get; set; } = 0f;

        public float VAcc { get; set; } = 0f;

        public bool IsFlagSet(Flag f) => (Flags & (int) f) != 0;

        public bool IsFlagNotSet(Flag f) => !IsFlagSet(f);

        public void SetFlag(Flag f) => Flags |= (int) f;

        public void ClearFlag(Flag f) => Flags &= (int) ~f;

        public void ClearFlags()
        {
            Flags = 0;
        }

        public enum Type
        {
            Normal,
            Ice,
            Swimming,
            Flying,
            Fixated
        }

        public enum Flag
        {
            NoGravity = 0x0001,
            TurnAtEdges = 0x0002,
            CheckBelow = 0x0004
        }
    }
}