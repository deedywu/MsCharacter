using System;
using System.Collections.Generic;
using System.Linq;
using Character.MapleLib.WzLib;
using Character.MapleLib.WzLib.WzProperties;
using Microsoft.Xna.Framework;
using Character.Core.Template;
using Character.Core.Util;
using Range = Character.Core.Template.Range;

namespace Character.Core.GamePlay.Physics
{
    public class FootholdTree
    {
        public Range Walls { get; }

        public Range Borders { get; }

        private readonly Dictionary<short, Foothold> _footholds = new Dictionary<short, Foothold>();

        private readonly Dictionary<Short, short> _footholdSbYx = new Dictionary<Short, short>();

        public void LimitMovement(PhysicsObject phObj)
        {
            if (phObj.HMobile)
            {
                var currentX = phObj.CurrentX;
                var nextX = phObj.NextX;
                var left = phObj.HSpeed < 0.0f;
                var wall = GetWall(phObj.FhId, left, phObj.NextY);
                var collision = left ? currentX >= wall && nextX <= wall : currentX <= wall && nextX >= wall;
                if (!collision && phObj.IsFlagSet(PhysicsObject.Flag.TurnAtEdges))
                {
                    wall = GetEdge(phObj.FhId, left);
                    collision = left ? currentX >= wall && nextX <= wall : currentX <= wall && nextX >= wall;
                }

                if (collision)
                {
                    phObj.LimitX(wall);
                    phObj.ClearFlag(PhysicsObject.Flag.TurnAtEdges);
                }
            }

            if (!phObj.VMobile) return;
            {
                var currentY = phObj.CurrentY;
                var nextY = phObj.NextY;
                var ground = new RangeFloat(GetFh(phObj.FhId).GroundBelow(phObj.CurrentY),
                    GetFh(phObj.FhId).GroundBelow(phObj.NextX));

                var collision = currentY <= ground.First && nextY >= ground.Second;

                if (collision)
                {
                    phObj.LimitY(ground.Second);
                    LimitMovement(phObj);
                }
                else
                {
                    phObj.LimitY(nextY < Borders.First ? Borders.First : Borders.Second);
                }
            }
        }

        public void UpdateFh(PhysicsObject phObj)
        {
            if (phObj.Types == PhysicsObject.Type.Fixated && phObj.FhId > 0) return;
            var curFh = GetFh(phObj.FhId);
            var checkSlope = false;
            var x = phObj.CurrentX;
            var y = phObj.CurrentY;
            if (phObj.OnGround)
            {
                if (Math.Floor(x) > curFh.R)
                    phObj.FhId = curFh.Next;
                else if (Math.Ceiling(x) < curFh.L)
                    phObj.FhId = curFh.Prev;

                if (phObj.FhId == 0)
                    phObj.FhId = GetFhIdBelow(x, y);
                else
                    checkSlope = true;
            }
            else
            {
                phObj.FhId = GetFhIdBelow(x, y);
            }

            var nextFh = GetFh(phObj.FhId);
            phObj.FhSlope = nextFh.Slope;
            var ground = nextFh.GroundBelow(x);
            if (phObj.VSpeed.Equals(0) && checkSlope)
            {
                var vDelta = Math.Abs(phObj.FhSlope);
                if (phObj.FhSlope < 0)
                    vDelta *= (ground - y);
                else if (phObj.FhSlope > 0)
                    vDelta *= (y - ground);
                if (!curFh.Slope.Equals(0) || !nextFh.Slope.Equals(0))
                {
                    if (phObj.HSpeed > 0 && vDelta <= phObj.HSpeed)
                        phObj.Y.Set(ground);
                    else if (phObj.HSpeed < 0 && vDelta >= phObj.HSpeed)
                        phObj.Y.Set(ground);
                }
            }

            phObj.OnGround = phObj.Y.Get().Equals(ground);

            if (phObj.EnableJd || phObj.IsFlagSet(PhysicsObject.Flag.CheckBelow))
            {
                var belowId = GetFhIdBelow(x, nextFh.GroundBelow(x) + 1f);
                if (belowId > 0)
                {
                    var nextGround = GetFh(belowId).GroundBelow(x);
                    phObj.EnableJd = (nextGround - ground) < 600;
                    phObj.GroundBelow = ground + 1f;
                }
                else
                {
                    phObj.EnableJd = false;
                }

                phObj.ClearFlag(PhysicsObject.Flag.CheckBelow);
            }

            if (phObj.FhLayer == 0 || phObj.OnGround)
                phObj.FhLayer = nextFh.Layer;
            if (phObj.FhId == 0)
            {
                phObj.FhId = curFh.Id;
                phObj.LimitX(curFh.X1);
            }
        }

        public Foothold GetFh(short fhId) => _footholds.ContainsKey(fhId) ? _footholds[fhId] : new Foothold();

        public float GetWall(short curId, bool left, float fy)
        {
            var shortY = (short) fy;
            var vertical = new Range((short) (shortY - 50), (short) (shortY - 1));
            var cur = GetFh(curId);
            if (left)
            {
                var prev = GetFh(cur.Prev);
                if (prev.IsBlocking(vertical))
                    return cur.L;
                var prev2 = GetFh(prev.Prev);
                return prev2.IsBlocking(vertical) ? prev.L : Walls.First;
            }
            else
            {
                var next = GetFh(cur.Next);
                if (next.IsBlocking(vertical))
                    return cur.R;
                var next2 = GetFh(next.Next);
                return next2.IsBlocking(vertical) ? next.R : Walls.Second;
            }
        }

        public float GetEdge(short curId, bool left)
        {
            var fh = GetFh(curId);
            if (left)
            {
                var prevId = fh.Prev;
                if (prevId == 0) return fh.L;

                var prev = GetFh(prevId);
                var prev2Id = prev.Prev;
                return prev2Id == 0 ? prev.L : Walls.First;
            }
            else
            {
                var nextId = fh.Next;
                if (nextId == 0) return fh.R;

                var next = GetFh(nextId);
                var next2Id = next.Next;
                return next2Id == 0 ? next.R : Walls.Second;
            }
        }

        public short GetFhIdBelow(float fx, float fy)
        {
            short ret = 0;
            float comp = Borders.Second;

            var x = (short) fx;

            foreach (var keyValuePair in _footholdSbYx.Where(keyValuePair => keyValuePair.Key.S == x))
            {
                var fh = _footholds[keyValuePair.Value];
                var yComp = fh.GroundBelow(fx);

                if (!(comp >= yComp) || !(yComp >= fy)) continue;
                comp = yComp;
                ret = fh.Id;
            }

            return ret;
        }

        public short GetYBelow(Vector2 position)
        {
            var (x, y) = position;
            var fhId = GetFhIdBelow(x, y);
            if (fhId == 0) return Borders.Second;
            var fh = GetFh(fhId);
            return (short) fh.GroundBelow(x);
        }

        #region 构造函数

        public FootholdTree(WzObject source)
        {
            short leftW = 30000;
            short rightW = -30000;
            short botB = -30000;
            short topB = 30000;

            foreach (var baseF0 in ((WzSubProperty) source).WzProperties)
            {
                short layer;
                var baseF = (WzSubProperty) baseF0.GetByUol();
                try
                {
                    short.TryParse(baseF.Name, out layer);
                }
                catch (Exception)
                {
                    continue;
                }

                foreach (var midF0 in baseF.WzProperties)
                {
                    var midF = (WzSubProperty) midF0.GetByUol();
                    foreach (var lastF0 in midF.WzProperties)
                    {
                        var lastF = (WzSubProperty) lastF0.GetByUol();
                        short id;
                        try
                        {
                            short.TryParse(lastF.Name, out id);
                        }
                        catch (Exception)
                        {
                            continue;
                        }

                        var foothold = new Foothold(lastF0, id, layer);
                        _footholds[id] = foothold;
                        var start = foothold.L;
                        var end = foothold.R;
                        if (start > leftW)
                            leftW = start;
                        if (end > rightW)
                            rightW = end;
                        if (foothold.B > botB)
                            botB = foothold.B;
                        if (foothold.T < topB)
                            topB = foothold.T;
                        if (foothold.IsWall)
                            continue;
                        for (var i = start; i <= end; i++)
                            _footholdSbYx[new Short(i)] = id;
                    }
                }
            }

            Walls = new Range((short) (leftW + 25), (short) (rightW - 25));
            Borders = new Range((short) (topB - 300), (short) (botB + 100));
        }

        public FootholdTree()
        {
        }

        #endregion

        /// <summary>
        /// 允许出现重复key使用的
        /// </summary>
        private class Short
        {
            public short S { get; }

            public Short(short s)
            {
                S = s;
            }
        }
    }
}