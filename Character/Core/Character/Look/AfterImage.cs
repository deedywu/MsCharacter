﻿using System.Linq;
using Character.MapleLib.WzLib;
using Character.MapleLib.WzLib.WzProperties;
using Microsoft.Xna.Framework;
using Character.Core.Common;
using Character.Core.Graphics;
using Character.Core.Util;

namespace Character.Core.Character.Look
{
    public class AfterImage
    {
        private readonly Animation _animation;
        
        private bool _displayed;

        public void Draw(short stFrame, DrawArgument args, float alpha)
        {
            if (!_displayed && stFrame >= FirstFrame)
                _animation.Draw(args, alpha);
        }

        public void Update(short stFrame, short timeStep = GameUtil.TimeStep)
        {
            if (!_displayed && stFrame >= FirstFrame)
                _displayed = _animation.Update(timeStep);
        }

        public short FirstFrame { get; }

        public Rectangle Range { get; }

        #region 构造函数

        public AfterImage(int skillId, string name, string stanceName, short level)
        {
            WzObject src = null;
            if (skillId > 0)
            {
                var strId = skillId.ToString().PadLeft(7, '0');
                src = Wz.Skill[$"{strId.Substring(0, 3)}.img"]["skill"][strId]["afterimage"][name][stanceName];
            }

            if (src == null)
                src = Wz.Character["Afterimage"][$"{name}.img"][(level / 10).ToString()][stanceName];

            var (left, top) = src["lt"]?.Pos() ?? new Vector2();
            var (right, bottom) = src["rb"]?.Pos() ?? new Vector2();
            Range = new Rectangle((int) left, (int) right, (int) top, (int) bottom);
            FirstFrame = 0;
            _displayed = false;

            foreach (var sub in ((WzSubProperty) src).WzProperties.Select(sub0 => sub0.GetByUol()))
            {
                var b = short.TryParse(sub.Name, out var frame);
                if (!b)
                    frame = 255;
                if (frame >= 255) continue;
                _animation = new Animation(sub);
                FirstFrame = frame;
            }
        }

        public AfterImage()
        {
            FirstFrame = 0;
            _displayed = true;
        }

        #endregion
    }
}