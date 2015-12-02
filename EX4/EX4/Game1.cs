using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;

namespace EX4
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        

        GameSprite player;
        List<GameSprite> enemies = new List<GameSprite>();
        Texture2D baddiePic;
        int lives=3;
        SpriteFont gameFont;
        SoundEffect hitSound;
        SoundEffect shootSound;
        SoundEffect baddieHitSound;
        Song BGmusic, PauseMusic;
        Texture2D pauseImage;
        int score = 0;
        int highscore; 

        List<GameSprite> bullets = new List<GameSprite>();
        List<GameSprite> enemyBullets = new List<GameSprite>();
        Texture2D bulletPic;
        
       int gunCooldown = 0;
        const int MAXGUNCOOLDOWN = 100;

        Random rng = new Random();
        int screenWidth = 1024, screenHeight = 768;
        List<GameSprite> explosions = new List<GameSprite>();
        Texture2D explosionImage;

        enum GameState { playing, gameOver, menu, pause }; 

        GameState currentGameState = GameState.menu;

        int pointsUntilNextBonusLife = POINTSPERLIFE;
        const int POINTSPERLIFE = 1000;

        
        float difficulty = 100f;  
        float difficultyIncreaseRate = 0.005f;    

       
        int invulnerableTime = 0;
        const int DEFAULT_INVULNERABLE_TIME = 2000;

        
        KeyboardState currentKeys, oldKeys;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;
        }

        protected override void Initialize()
        {
            base.Initialize();
            string inputfile = "highscores.txt";
            if (File.Exists(inputfile))
            {
                StreamReader inputFileStream = new StreamReader(inputfile);
                string inputText = inputFileStream.ReadLine();
                highscore = Convert.ToInt32(inputText);   
                inputFileStream.Close();
            }
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice); 
            baddiePic = Content.Load<Texture2D>("badguys");
            Texture2D playerimage = Content.Load<Texture2D>("player");
            player = new GameSprite(playerimage,
                new Vector2(screenWidth / 2 - playerimage.Width / 2, screenHeight - playerimage.Height));
            gameFont = Content.Load<SpriteFont>("War Hero");
            hitSound = Content.Load<SoundEffect>("asplode");
            BGmusic = Content.Load<Song>("POL-battle-march-long");
            PauseMusic = Content.Load<Song>("PauseMusic");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(BGmusic);
            MediaPlayer.Volume = 0.2f;
            shootSound = Content.Load<SoundEffect>("shoot");
            bulletPic = Content.Load<Texture2D>("bullet");
            baddieHitSound = Content.Load<SoundEffect>("asplode2");
           
            
            explosionImage = Content.Load<Texture2D>("badguyexplosion");
            pauseImage = Content.Load<Texture2D>("pauseImage");
        }

        protected override void UnloadContent() {        }

        protected override void Update(GameTime gameTime)
        {
            
            currentKeys = Keyboard.GetState();
            if (currentKeys.IsKeyDown(Keys.Enter) && oldKeys.IsKeyDown(Keys.Enter) &&
                (currentKeys.IsKeyDown(Keys.LeftAlt) || currentKeys.IsKeyDown(Keys.RightAlt)))
                graphics.ToggleFullScreen();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) this.Exit();

            switch (currentGameState)
            {
                case GameState.playing:
                    UpdateGameplay(gameTime);
                    break;
                case GameState.gameOver:
                    UpdateGameover();
                    break;
                case GameState.menu:
                    UpdateDrawMenu();
                    break;
                case GameState.pause: 
                    if (currentKeys.IsKeyDown(Keys.P) && oldKeys.IsKeyUp(Keys.P))
                    {
                        currentGameState = GameState.playing;
                        MediaPlayer.Play(BGmusic);
                        MediaPlayer.Volume = 0.2f;
                    }
                    break;
            }
            
            oldKeys = currentKeys;

            base.Update(gameTime);
        }

        private void UpdateGameover()
        {
            if(Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                currentGameState = GameState.menu;
                shootSound.Play();
            }
        }

        private void UpdateDrawMenu()
        {
            if(Keyboard.GetState().IsKeyDown(Keys.P))
            {
                currentGameState = GameState.playing;
                lives = 3;
                score = 0;
                enemies.Clear();
                enemyBullets.Clear(); 
                bullets.Clear(); 

                player.position = new Vector2(screenWidth / 2 - player.image.Width / 2, screenHeight - player.image.Height);
                shootSound.Play();
                
                pointsUntilNextBonusLife = POINTSPERLIFE;
               
                difficulty = 100;   
            }
        }

        private void UpdateGameplay(GameTime gameTime)
        {
           
            if (currentKeys.IsKeyDown(Keys.P) && oldKeys.IsKeyUp(Keys.P))
            {
                currentGameState = GameState.pause;
                MediaPlayer.Play(PauseMusic);
                MediaPlayer.Volume = 1;
            }

            player.position.X += GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X * 4;

           
            if (Keyboard.GetState().IsKeyDown(Keys.Left)) player.velocity.X -= 1;
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) player.velocity.X += 1;
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) player.velocity.Y -= 1;
            else if (player.position.Y < screenHeight / 2) player.velocity.Y += 0.2f;
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) player.velocity.Y += 1;
            player.velocity *= 0.95f;
            player.position += player.velocity;

            
            if (player.position.X < 0) player.position.X = 0;
            if (player.position.X > screenWidth - player.image.Width) player.position.X = screenWidth - player.image.Width;
            if (player.position.Y < 0) player.position.Y = 0;
            if (player.position.Y > screenHeight - player.image.Height) player.position.Y = screenHeight - player.image.Height;

           
            gunCooldown -= gameTime.ElapsedGameTime.Milliseconds;
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && gunCooldown <= 0)
            {
                bullets.Add(new GameSprite(bulletPic, player.position + new Vector2(player.image.Width / 2, 0)));
                bullets[bullets.Count() - 1].velocity.Y = -15;  
                shootSound.Play();
                gunCooldown = MAXGUNCOOLDOWN;
            }

            
            for (int bulletNum = 0; bulletNum < bullets.Count(); bulletNum++)
            {
                bullets[bulletNum].position += bullets[bulletNum].velocity; 
            }

            
            invulnerableTime -= gameTime.ElapsedGameTime.Milliseconds;
            
            for (int bulletNum = 0; bulletNum < enemyBullets.Count(); bulletNum++)
            {
                enemyBullets[bulletNum].position += enemyBullets[bulletNum].velocity;
                if (enemyBullets[bulletNum].collision(player))
                {
                    enemyBullets.RemoveAt(bulletNum);
                    bulletNum--;
                    if(invulnerableTime<=0) PlayerDeath();
                }
            }

           
            difficulty += gameTime.ElapsedGameTime.Milliseconds * difficultyIncreaseRate;
            float spawnChance = 10;
            if (rng.Next(0, 1000) < (difficulty / 100 * spawnChance))
            {
                enemies.Add(new GameSprite(baddiePic,
                    new Vector2(rng.Next(0, screenWidth - baddiePic.Width), -100)));
            }

           
            float bulletSpawnChance = 5;
            foreach (GameSprite enemy in enemies)
            {
                if (rng.Next(0, 1000) < (difficulty / 100 * bulletSpawnChance))
                {
                    GameSprite newBullet = new GameSprite(bulletPic, enemy.position);
                    newBullet.velocity = player.position - enemy.position;
                    newBullet.velocity.Normalize();
                    newBullet.velocity *= (6 * difficulty / 100);   
                    enemyBullets.Add(newBullet);
                }
            }

      
            for (int enemyNum = 0; enemyNum < enemies.Count(); enemyNum++)
            {
                enemies[enemyNum].velocity.Y = (3 * difficulty / 100);

                
                if (enemies[enemyNum].position.X + enemies[enemyNum].image.Width/2 < player.position.X + player.image.Width/2-4) enemies[enemyNum].velocity.X = 2;
                else if (enemies[enemyNum].position.X + enemies[enemyNum].image.Width / 2 > player.position.X + player.image.Width / 2+4) enemies[enemyNum].velocity.X = -2;
                else enemies[enemyNum].velocity.X = 0;

                enemies[enemyNum].position += enemies[enemyNum].velocity;

                if (enemies[enemyNum].position.Y > screenHeight)
                {
                    enemies.RemoveAt(enemyNum);
                    break;
                }
            }

      
            for (int enemyNum = 0; enemyNum < enemies.Count(); enemyNum++)
            {
                if (enemies[enemyNum].collision(player))
                {
                    enemies.RemoveAt(enemyNum);
                    enemyNum--;
                    if(invulnerableTime<=0) PlayerDeath(); 
                }
            }


            
            for (int enemyNum = 0; enemyNum < enemies.Count(); enemyNum++)
            {
                for (int bulletNum = 0; bulletNum < bullets.Count(); bulletNum++)
                {
                    if (enemies[enemyNum].collision(bullets[bulletNum]))
                    {
                        
                        GameSprite newExplosion = new GameSprite(explosionImage, enemies[enemyNum].position);
                        newExplosion.animationframes = 6;
                        newExplosion.animationTimeMax = 1000;
                        explosions.Add(newExplosion);

                        
                        enemies.RemoveAt(enemyNum);
                        enemyNum--;
                        bullets.RemoveAt(bulletNum);
                        bulletNum--;
                        baddieHitSound.Play();
                        score += 10;
                        
                        pointsUntilNextBonusLife -= 10;
                        if (pointsUntilNextBonusLife <= 0)
                        {
                            lives++;
                            pointsUntilNextBonusLife += POINTSPERLIFE;
                        }
                        break;
                    }
                }
            }
            if (score > highscore)
            {
                highscore = score;
            }

            
            if (lives <= 0)
            {
                currentGameState = GameState.gameOver;
                string outputFile = "highscores.txt";
                StreamWriter outputFileStream = new StreamWriter(outputFile);
                outputFileStream.WriteLine(highscore);
                outputFileStream.Close();
            }
        }

        private void PlayerDeath()
        {
            lives--;
            invulnerableTime = DEFAULT_INVULNERABLE_TIME;
            player.position = new Vector2(screenWidth / 2 - player.image.Width / 2, screenHeight - player.image.Height);
            hitSound.Play();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            if (currentGameState != GameState.pause)   
            {
               
            }
           

            switch (currentGameState)
            {
                case GameState.playing:
                    DrawGameplay(gameTime);
                    break;
                case GameState.gameOver:
                    DrawGameover();
                    break;
                case GameState.menu:
                    DrawMenu();
                    break;
                case GameState.pause:   
                    DrawGameplay(gameTime);
                    float time = (float)gameTime.TotalGameTime.TotalMilliseconds;
                    time = time / 2000 * (float)Math.PI*2;
                    Vector2 offset = new Vector2((float)Math.Sin(time) * 75, 0);
                    spriteBatch.DrawString(gameFont, "paws", new Vector2(240, 300)+offset,
                        Color.Yellow, 0, new Vector2(0, 0), 3, SpriteEffects.None, 0);
                    offset = PauseImage(offset);
                    break;
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        Color dColor;
        int dTime;
        bool specialPause = true;
        private Vector2 PauseImage(Vector2 offset)
        {
            if (specialPause)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.F1)) specialPause = !specialPause;
                dTime--;
                if (dTime < 0)
                {
                    dColor = new Color(rng.Next(0, 255), rng.Next(0, 255), rng.Next(0, 255));
                    dTime = 15; // frames
                }
                spriteBatch.Draw(pauseImage, new Vector2(0, 0), new Rectangle(0, 0, 1, 1), dColor, 0, Vector2.Zero, 1024f, SpriteEffects.None, 0);
                spriteBatch.Draw(pauseImage, new Vector2(512, 384), null, dColor, offset.X / 1000, new Vector2(pauseImage.Width / 2, pauseImage.Height / 2), 2.5f, SpriteEffects.None, 0);
            }
            return offset;
        }

        private void DrawMenu()
        {
            string menuText = "Press P to play";
            Vector2 menuTextSize = gameFont.MeasureString(menuText);
            spriteBatch.DrawString(gameFont, menuText, new Vector2(
                screenWidth / 2 - menuTextSize.X / 2, screenHeight / 2 - menuTextSize.Y / 2), Color.White);
        }

        private void DrawGameover()
        {
            spriteBatch.DrawString(gameFont, "Lives: " + lives, new Vector2(0, 0), Color.White);
            spriteBatch.DrawString(gameFont, "Score: " + score, new Vector2(
                screenWidth - gameFont.MeasureString("Score: " + score).X, 0), Color.White);
            string gameoverText = "BTFO.";
            Vector2 gameovertextSize = gameFont.MeasureString(gameoverText);
            spriteBatch.DrawString(gameFont, gameoverText, new Vector2(
                screenWidth / 2 - gameovertextSize.X / 2, screenHeight / 2 - gameovertextSize.Y / 2), Color.White);
        }

        private void DrawGameplay(GameTime gameTime)
        {
            foreach (GameSprite enemy in enemies)
            {
                enemy.Draw(spriteBatch);
            }
            foreach (GameSprite bullet in enemyBullets)
            {
                bullet.Draw(spriteBatch);
            }
            if(invulnerableTime > 0 && (invulnerableTime/250)%2==0)
                player.Draw(spriteBatch, Color.Red);    
            else
                player.Draw(spriteBatch, Color.White);    
            spriteBatch.DrawString(gameFont, "Lives: " + lives, new Vector2(0, 0), Color.White);
            spriteBatch.DrawString(gameFont, "Score: " + score, new Vector2(
                screenWidth - gameFont.MeasureString("Score: " + score).X, 0), Color.White);

            string menuText = "Highscore: " + highscore;
            Vector2 menuTextSize = gameFont.MeasureString(menuText);
            spriteBatch.DrawString(gameFont, menuText, new Vector2(
                screenWidth / 2 - menuTextSize.X / 2, 50), Color.White);

            for (int expNum = 0; expNum < explosions.Count(); expNum++)
            {
                explosions[expNum].currentAnimationTime += gameTime.ElapsedGameTime.Milliseconds;
                if (explosions[expNum].currentAnimationTime > explosions[expNum].animationTimeMax)
                {
                    explosions.RemoveAt(expNum);
                    expNum--;
                }
                else
                {
                    explosions[expNum].DrawAnimated(spriteBatch);
                }
            }

            for (int bulletNum = 0; bulletNum < bullets.Count(); bulletNum++)
            {
                bullets[bulletNum].Draw(spriteBatch);
            }
        }
    }
}
