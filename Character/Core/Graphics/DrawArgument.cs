﻿using Microsoft.Xna.Framework;

namespace Character.Core.Graphics
{
    public class DrawArgument
    {
        public Vector2 Pos { get; }

        public Vector2 Center { get; }

        public Vector2 Stretch { get; }

        public float XScale { get; set; }

        public float YScale { get; }

        public float Angle { get; }

        public Color Color { get; }

        public static DrawArgument operator +(DrawArgument args, Vector2 argPos)
        {
            return new DrawArgument(args.Pos + argPos,
                args.Center + argPos,
                args.Stretch, args.XScale, args.YScale, args.Color, args.Angle);
        }

        public static DrawArgument operator +(DrawArgument args, float argOpc)
        {
            return new DrawArgument(args.Pos, args.Center, args.Stretch, args.XScale, args.YScale,
                args.Color.A * argOpc,
                args.Angle);
        }

        public static DrawArgument operator +(DrawArgument args, DrawArgument o)
        {
            return new DrawArgument(
                args.Pos + o.Pos,
                args.Center + o.Center,
                args.Stretch + o.Stretch,
                args.XScale * o.XScale,
                args.YScale * o.YScale,
                new Color
                {
                    A = (byte) (args.Color.A * o.Color.A),
                    R = (byte) (args.Color.R * o.Color.R),
                    G = (byte) (args.Color.G * o.Color.G),
                    B = (byte) (args.Color.B * o.Color.B)
                },
                args.Angle + o.Angle);
        }

        public static DrawArgument operator -(DrawArgument args, DrawArgument o)
        {
            return new DrawArgument(
                args.Pos - o.Pos,
                args.Center - o.Center,
                args.Stretch - o.Stretch,
                args.XScale / o.XScale,
                args.YScale / o.YScale,
                new Color
                {
                    A = (byte) (args.Color.A / o.Color.A),
                    R = (byte) (args.Color.R / o.Color.R),
                    G = (byte) (args.Color.G / o.Color.G),
                    B = (byte) (args.Color.B / o.Color.B)
                },
                args.Angle - o.Angle);
        }

        public Rectangle GetRectangle(Vector2 origin, Vector2 dimensions)
        {
            var w = Stretch.X;
            if (w.Equals(0))
                w = dimensions.X;
            var h = Stretch.Y;
            if (h.Equals(0))
                h = dimensions.Y;
            // var (rl, rt) = Pos - Center - origin;

            var rlt = Pos - Center - origin;
            var rl = rlt.X;
            var rr = rlt.X + w;
            var rt = rlt.Y;
            var rb = rlt.Y + h;
            var cx = Center.X;
            var cy = Center.Y;

            // var rr = rl + w;
            // var rb = rt + h;
            // var cx = Center.X;
            // var cy = Center.Y;
            return new Rectangle((int) (cx + XScale * rl),
                (int) (cx + XScale * rr),
                (int) dimensions.X,
                (int) dimensions.Y);
            // (int) (cy + YScale * rt),
            // (int) (cy + YScale * rb));
        }

        #region 构造函数

        public DrawArgument() : this(0, 0)
        {
        }

        public DrawArgument(float x, float y) : this(new Vector2(x, y))
        {
        }

        public DrawArgument(Vector2 position) : this(position, 1.0f)
        {
        }

        public DrawArgument(Vector2 position, float xScale, float yScale) : this(position, position, xScale, yScale,
            1.0f)
        {
        }

        public DrawArgument(Vector2 position, Vector2 stretch) : this(position, position, stretch, 1.0f, 1.0f, 1.0f,
            0.0f)
        {
        }

        public DrawArgument(Vector2 position, bool flip) : this(position, flip, 1.0f)
        {
        }

        public DrawArgument(float angle, Vector2 position, float opacity) : this(angle, position, false, opacity)
        {
        }

        public DrawArgument(Vector2 position, float opacity) : this(position, false, opacity)
        {
        }

        public DrawArgument(Vector2 position, Color color) : this(position, position,
            new Vector2(0, 0), 1.0f, 1.0f,
            color, 0.0f)
        {
        }

        public DrawArgument(Vector2 position, bool flip, Vector2 center) : this(position, center, flip ? -1.0f : 1.0f,
            1.0f, 1.0f)
        {
        }

        public DrawArgument(Vector2 position, Vector2 center, float xScale, float yScale, float opacity) : this(
            position, center, new Vector2(0, 0), xScale, yScale, opacity, 0.0f)
        {
        }

        public DrawArgument(bool flip) : this(flip ? -1.0f : 1.0f, 1.0f, 1.0f)
        {
        }

        public DrawArgument(float xScale, float yScale, float opacity) : this(new Vector2(0, 0), xScale, yScale,
            opacity)
        {
        }

        public DrawArgument(Vector2 position, float xScale, float yScale, float opacity) : this(position, position,
            xScale, yScale, opacity)
        {
        }

        public DrawArgument(Vector2 position, bool flip, float opacity) : this(position, position, flip ? -1.0f : 1.0f,
            1.0f, opacity)
        {
        }

        public DrawArgument(float angle, Vector2 position, bool flip, float opacity) : this(position, position,
            new Vector2(0, 0), flip ? -1.0f : 1.0f, 1.0f, opacity, angle)
        {
        }


        public DrawArgument(Vector2 position, Vector2 center, Vector2 stretch, float xScale, float yScale,
            float opacity, float angle)
        {
            Pos = position;
            this.Center = center;
            this.Stretch = stretch;
            this.XScale = xScale;
            this.YScale = yScale;
            Color = new Color {A = (byte) opacity, R = 1, G = 1, B = 1};
            this.Angle = angle;
        }

        public DrawArgument(Vector2 position, Vector2 center, Vector2 stretch, float xScale, float yScale,
            Color color,
            float angle)
        {
            Pos = position;
            this.Center = center;
            this.Stretch = stretch;
            this.XScale = xScale;
            this.YScale = yScale;
            this.Color = color;
            this.Angle = angle;
        }

        #endregion
    }
}