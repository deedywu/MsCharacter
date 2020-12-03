using System.Collections.Generic;
using Character.Core.Character.Look;
using Character.Core.GamePlay;
using Character.Core.Graphics;
using Character.Core.Net;
using Character.Core.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Character
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        private readonly CharLook _charLook;
        private readonly DrawArgument _drawArgs; 
        
        public Game1()
        {
            GameUtil.Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            GameUtil.Graphics.PreferredBackBufferWidth = 800;
            GameUtil.Graphics.PreferredBackBufferHeight = 600;

            var lookEntry = new LookEntry();
            lookEntry.Skin = 1;
            lookEntry.Hair = 30003;
            lookEntry.Face = 20001;
            lookEntry.Equips = new Dictionary<short, int>();

            lookEntry.Equips[0] = 1060000; // 裤子
            lookEntry.Equips[1] = 1302000; // 武器
            lookEntry.Equips[2] = 1040007; // 上衣

            // _equips.AddEquip(1302003, _drawInfo); // 武器
            // _equips.AddEquip(1060000, _drawInfo); // 裤子
            // _equips.AddEquip(1040007, _drawInfo); // 上衣
            // _equips.AddEquip(1071001, _drawInfo); // 鞋子
            // _equips.AddEquip(1000019, _drawInfo); // 帽子
            // _equips.AddEquip(1102008, _drawInfo); // 披风


            _charLook = new CharLook(lookEntry);
            _drawArgs = new DrawArgument(new Vector2(100, 100), true);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        { 
            // Create a new SpriteBatch, which can be used to draw textures.
            GameUtil.SpriteBatch = new SpriteBatch(GraphicsDevice); 
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Up)) _charLook.SetStance(Stance.Id.Fly);

            if (Keyboard.GetState().IsKeyDown(Keys.Down)) _charLook.SetStance(Stance.Id.Prone);


            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                _drawArgs.XScale = -1f;
                _charLook.SetStance(Stance.Id.Walk1);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                _drawArgs.XScale = 1f;
                _charLook.SetStance(Stance.Id.Walk2);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.A)) _charLook.Attack(false);

            if (Keyboard.GetState().IsKeyDown(Keys.S)) _charLook.SetStance(Stance.Id.Stand1);

            if (Keyboard.GetState().IsKeyDown(Keys.D)) _charLook.SetStance(Stance.Id.Dead);

            if (Keyboard.GetState().IsKeyDown(Keys.X)) _charLook.SetStance(Stance.Id.Jump);

            if (Keyboard.GetState().IsKeyDown(Keys.F1)) _charLook.SetExpression(Expression.Id.Hit);

            if (Keyboard.GetState().IsKeyDown(Keys.F2)) _charLook.SetExpression(Expression.Id.Smile);

            if (Keyboard.GetState().IsKeyDown(Keys.F3)) _charLook.SetExpression(Expression.Id.Troubled);

            if (Keyboard.GetState().IsKeyDown(Keys.F4)) _charLook.SetExpression(Expression.Id.Cry);

            if (Keyboard.GetState().IsKeyDown(Keys.F5)) _charLook.SetExpression(Expression.Id.Angry);

            if (Keyboard.GetState().IsKeyDown(Keys.F6)) _charLook.SetExpression(Expression.Id.Bewildered);

            if (Keyboard.GetState().IsKeyDown(Keys.F7)) _charLook.SetExpression(Expression.Id.Stunned);
            
            _charLook.Update(GameUtil.TimeStep);
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GameUtil.SpriteBatch.Begin();
            _charLook.Draw(_drawArgs, 1f);
            GameUtil.SpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
