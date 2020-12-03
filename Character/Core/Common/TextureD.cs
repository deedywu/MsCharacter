﻿﻿using Character.MapleLib.WzLib;
using Character.MapleLib.WzLib.WzProperties;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Character.Core.Graphics;
using Character.Core.Util;

namespace Character.Core.Common
{
    public class TextureD
    {
        private Texture2D _tex;

        public void Draw(DrawArgument args)
        {
            var origin = args.XScale.Equals(-1) ? new Vector2(Rectangle.Width - Origin.X, Origin.Y) : Origin;
            var eff = args.XScale.Equals(-1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            var rect = Rectangle;
            if (args.XScale.Equals(-1))
                rect.X = -rect.Right;
            // GameUtil.SpriteBatch.Draw(
            //     Tex,
            //     args.Pos,
            //     AtlasRect,
            //     color: Color.White,
            //     origin: origin,
            //     effects: eff);

            GameUtil.SpriteBatch.Draw(Tex, args.Pos, AtlasRect,Color.White, 0f, origin, Vector2.One, eff, 0f);
        }

        public Texture2D Tex => _tex ?? (_tex = Png.ToTexture2D(GameUtil.SpriteBatch.GraphicsDevice));

        public TextureD Shift(Vector2 amount)
        {
            Origin -= amount;
            return this;
        }

        public Rectangle? AtlasRect { get; }

        public WzPngProperty Png { get; }

        public Vector2 Origin { get; private set; }

        public Vector2 Dimensions { get; private set; }
        public int Delay { get; set; }
        public int A0 { get; set; }
        public int A1 { get; set; }

        public bool Blend { get; set; }

        public Rectangle Rectangle
        {
            get
            {
                if (AtlasRect != null)
                {
                    return new Rectangle((int) -Origin.X, (int) -Origin.Y, AtlasRect.Value.Width,
                        AtlasRect.Value.Height);
                }
                else if (Tex != null)
                {
                    return new Rectangle((int) -Origin.X, (int) -Origin.Y, Tex.Width, Tex.Height);
                }
                else
                {
                    return Rectangle.Empty;
                }
            }
        }

        public TextureD(WzObject source)
        {
            Png = ((WzPngProperty) source.WzValue);
            AtlasRect = null;
            A0 = 255;
            A1 = 255;
            A0 = source["a0"]?.GetInt() ?? 0;
            A1 = source["a1"]?.GetInt() ?? 0;
            Delay = source["delay"]?.GetInt() ?? 0;
            Origin = source["origin"].Pos();
            Dimensions = new Vector2(Png.Width, Png.Height);
            if (Delay == 0)
                Delay = 100; //给予默认delay
        }
    }
}