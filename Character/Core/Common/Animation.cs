﻿using System.Collections.Generic;
using System.Linq;
using Character.MapleLib.WzLib;
using Character.MapleLib.WzLib.WzProperties;
using Microsoft.Xna.Framework;
using Character.Core.Graphics;
using Character.Core.Template;
using Character.Core.Util;

namespace Character.Core.Common
{
    public class Animation
    {
        private readonly List<Frame> _frames = new List<Frame>();
        private bool _animated;
        private readonly bool _zigzag;
        private readonly Linear _opacity = new Linear();
        private readonly Linear _xyScale = new Linear();

        private short _delay;

        private short _frameStep;

        private readonly Nominal<short> _frame = new Nominal<short>();

        public short GetDelay(short frameId) => frameId < _frames.Count ? _frames[frameId].Delay : (short) 0;

        public short GetDelayUnit(short frameId)
        {
            short total = 0;
            for (short i = 0; i < frameId; i++)
            {
                if (i >= _frames.Count)
                    break;
                total += _frames[frameId].Delay;
            }

            return total;
        }

        public Vector2 Origin => Frame.Origin;

        public Vector2 Dimensions => Frame.Dimensions;

        public Vector2 Head => Frame.Head;

        public Rectangle Bounds => Frame.Bounds;

        public Frame Frame => _frames[_frame.Get()];

        public void Draw(DrawArgument args, float alpha)
        {
            var interFrame = _frame.Get(alpha);
            var interOpc = _opacity.Get(alpha) / 255;
            var interScale = _xyScale.Get(alpha) / 100;
            var modifyOpc = !interOpc.Equals(1.0f);
            var modifyScale = !interScale.Equals(1.0f);
            if (modifyOpc || modifyScale)
                _frames[interFrame].Draw(args + new DrawArgument(interScale, interScale, interOpc));
            else
                _frames[interFrame].Draw(args);
        }

        public bool Update(short timeStep = GameUtil.TimeStep)
        {
            var frameData = Frame;
            _opacity.Set(_opacity + frameData.OpcStep(timeStep));
            if (_opacity.Last() < 0f)
                _opacity.Set(0f);
            else if (_opacity.Last() > 255f)
                _opacity.Set(255f);
            _xyScale.Set(_xyScale + frameData.ScaleStep(timeStep));
            if (_xyScale.Last() < 0f)
                _opacity.Set(0f);
            if (timeStep >= _delay)
            {
                var lastFrame = (short) (_frames.Count - 1);
                short nextFrame;
                bool endEd;
                if (_zigzag && lastFrame > 0)
                {
                    if (_frameStep == 1 && _frame.Get() == lastFrame)
                    {
                        _frameStep = (short) -_frameStep;
                        endEd = false;
                    }
                    else if (_frameStep == -1 && _frame.Get() == 0)
                    {
                        _frameStep = (short) -_frameStep;
                        endEd = true;
                    }
                    else
                        endEd = false;

                    nextFrame = (short) (_frame.Get() + _frameStep);
                }
                else
                {
                    if (_frame.Get() == lastFrame)
                    {
                        nextFrame = 0;
                        endEd = true;
                    }
                    else
                    {
                        nextFrame = (short) (_frame.Get() + 1);
                        endEd = false;
                    }
                }

                var delta = (short) (timeStep - _delay);
                var threshold = (float) delta / timeStep;
                _frame.Next(nextFrame, threshold);

                _delay = _frames[nextFrame].Delay;
                if (_delay >= delta)
                    _delay -= delta;
                _opacity.Set(_frames[nextFrame].StartOpacity);
                _xyScale.Set(_frames[nextFrame].StartScale);
                return endEd;
            }
            else
            {
                _frame.Normalize();
                _delay -= timeStep;
                return false;
            }
        }

        public void Reset()
        {
            _frame.Set(0);
            _opacity.Set(_frames[0].StartOpacity);
            _xyScale.Set(_frames[0].StartScale);
            _delay = _frames[0].Delay;
            _frameStep = 1;
        }

        #region 构造函数

        public Animation(WzObject src)
        {
            if (src is WzCanvasProperty)
            {
                _frames.Add(new Frame(src));
            }
            else
            {
                var frameIds = new HashSet<short>();

                foreach (var sub0 in ((WzSubProperty) src).WzProperties)
                {
                    var sub = sub0.GetByUol();
                    if (!(sub is WzCanvasProperty)) continue;
                    var b = short.TryParse(sub.Name, out var fid);
                    if (!b)
                        fid = -1;
                    if (fid >= 0)
                        frameIds.Add(fid);
                }

                foreach (var sub in frameIds.Select(id => src[$"{id}"])) _frames.Add(new Frame(sub));
                if (_frames.Count == 0) _frames.Add(new Frame());
            }

            _animated = _frames.Count > 1;
            _zigzag = src["zigzag"]?.GetInt() != 0;
            Reset();
        }

        public Animation()
        {
            _animated = false;
            _zigzag = false;
            _frames.Add(new Frame());
            Reset();
        }

        #endregion
    }


    #region Frame类

    public class Frame
    {
        private readonly TextureD _texture;

        private readonly Dictionary<bool, short> _opacity = new Dictionary<bool, short>();

        private readonly Dictionary<bool, short> _scales = new Dictionary<bool, short>();

        public void Draw(DrawArgument args)
        {
            _texture.Draw(args);
        }

        public short StartOpacity => _opacity[true];

        public short StartScale => _scales[true];
        public short Delay { get; }

        public Vector2 Origin => _texture.Origin;

        public Vector2 Dimensions => _texture.Dimensions;

        public Vector2 Head { get; }

        public Rectangle Bounds { get; }

        public float OpcStep(short timeStep) => ((float) (_opacity[false] - _opacity[true])) / Delay;

        public float ScaleStep(short timeStep) => ((float) (_scales[false] - _scales[true])) / Delay;

        #region 构造函数

        public Frame(WzObject src)
        {
            _texture = new TextureD(src);

            var (left, top) = src["lt"]?.Pos() ?? new Vector2();
            var (right, bottom) = src["rb"]?.Pos() ?? new Vector2();
            Bounds = new Rectangle((int) left, (int) right, (int) top, (int) bottom);
            Head = src["head"].Pos();
            Delay = src["delay"]?.GetShort() ?? 0;
            if (Delay == 0)
                Delay = 100;
            var a0 = (short) src["a0"];
            var a1 = (short) src["a1"];
            if (a0 != 0 && a1 != 0)
            {
                _opacity[true] = a0;
                _opacity[false] = a1;
            }
            else if (a0 != 0)
            {
                _opacity[true] = a0;
                _opacity[false] = (short) (255 - a0);
            }
            else if (a1 != 0)
            {
                _opacity[true] = (short) (255 - a1);
                _opacity[false] = a1;
            }
            else
            {
                _opacity[true] = 255;
                _opacity[false] = 255;
            }

            var z0 = (short) src["z0"];
            var z1 = (short) src["z1"];
            if (z0 != 0 && z1 != 0)
            {
                _scales[true] = z0;
                _scales[false] = z1;
            }
            else if (z0 != 0)
            {
                _scales[true] = z0;
                _scales[false] = 0;
            }
            else if (z1 != 0)
            {
                _scales[true] = 100;
                _scales[false] = z1;
            }
            else
            {
                _scales[true] = 100;
                _scales[false] = 100;
            }
        }

        public Frame()
        {
            Delay = 0;
            _opacity[true] = 0;
            _opacity[false] = 0;
            _scales[true] = 0;
            _scales[false] = 0;
        }

        #endregion
    }

    #endregion
}