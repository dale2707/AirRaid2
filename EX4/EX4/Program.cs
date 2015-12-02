using System;

namespace EX4
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Game1 game = new Game1())
            {
                game.Run();
            }
        }
    }
#endif
}





//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Audio;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Media;

//namespace EX4
//{
//    public class Game1 : Microsoft.Xna.Framework.Game
//    {
//        GraphicsDeviceManager graphics;
//        SpriteBatch spriteBatch;

//        class GameSprite
//        {
//            public Texture2D image;
//            public Vector2 position;

//            public GameSprite(Texture2D initialTexture, Vector2 initialPosition)
//            {
//                image = initialTexture;
//                position = initialPosition;
//            }

//            public void Draw(SpriteBatch batch)
//            {
//                try
//                {
//                    batch.Draw(image, position, Color.White);
//                }
//                catch(InvalidOperationException)
//                {
//                    batch.Begin();
//                    batch.Draw(image, position, Color.White);
//                    batch.End();
//                }
//            }

//            public bool collision(GameSprite target)
//            {
//                if (target == null) return false;
//                Rectangle sourceRectangle = new Rectangle(
//                    (int)position.X, (int)position.Y, image.Width, image.Height);
//                Rectangle targetRectangle = new Rectangle(
//                    (int)target.position.X, (int)target.position.Y, target.image.Width, target.image.Height);
//                return(sourceRectangle.Intersects(targetRectangle));
//            }
//        }

//        List<GameSprite> enemies = new List<GameSprite>();
//        Texture2D baddiePic;
//        GameSprite player;

//        /// <summary>
//        /// KEEEEEEEEEEEEEEEEEEEP
//        /// </summary>
//        Random rng = new Random();
//        int screenWidth = 1024, screenHeight = 768;

//        public Game1()
//        {
//            graphics = new GraphicsDeviceManager(this);
//            Content.RootDirectory = "Content";
//            graphics.PreferredBackBufferWidth = screenWidth;
//            graphics.PreferredBackBufferHeight = screenHeight;
//        }

//        protected override void Initialize()
//        {
//            base.Initialize();
//        }

//        protected override void LoadContent()
//        {
//            spriteBatch = new SpriteBatch(GraphicsDevice); // Create a new SpriteBatch, which can be used to draw textures.
//            baddiePic = Content.Load<Texture2D>("badguys");
//            Texture2D playerimage = Content.Load<Texture2D>("player");
//            player = new GameSprite(playerimage,
//                new Vector2(screenWidth/2 - playerimage.Width/2, screenHeight - playerimage.Height));
//        }

//        protected override void UnloadContent() {        }

//        protected override void Update(GameTime gameTime)
//        {
//            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) this.Exit();

//            player.position.X += GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X * 4;
//            if (Keyboard.GetState().IsKeyDown(Keys.Left)) player.position.X -= 4;
//            if (Keyboard.GetState().IsKeyDown(Keys.Right)) player.position.X += 4;

//            if (rng.Next(0, 1000) < 10)
//            {
//                enemies.Add(new GameSprite(baddiePic,
//                    new Vector2(rng.Next(0, screenWidth - baddiePic.Width), -100)));
//            }

//            foreach (GameSprite enemy in enemies)
//            {
//                enemy.position.Y += 5;
//                if (enemy.position.Y > screenHeight)
//                {
//                    enemies.Remove(enemy);
//                    break;
//                }
//            }

//            foreach (GameSprite enemy in enemies)
//            {
//                if(enemy.collision(player))
//                {
//                    enemies.Remove(enemy);
//                    break;
//                }
//            }

//            base.Update(gameTime);
//        }

//        protected override void Draw(GameTime gameTime)
//        {
//            GraphicsDevice.Clear(Color.CornflowerBlue);

//            spriteBatch.Begin();
//            foreach (GameSprite enemy in enemies)
//            {
//                enemy.Draw(spriteBatch);
//            }
//            player.Draw(spriteBatch);
//            spriteBatch.End();

//            base.Draw(gameTime);
//        }
//    }
//}
