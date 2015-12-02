using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace EX4
{
    class GameSprite
    {
        public Texture2D image;
        public Vector2 position;
        public Vector2 velocity;
        public int animationframes;
        public int currentAnimationTime;
        public int animationTimeMax;
        public float rotationOffset = (float)Math.PI / 2; // lesson 9, slide 8
        public float rotation; // lesson 9, slide 8

        public GameSprite(Texture2D initialTexture, Vector2 initialPosition)
        {
            image = initialTexture;
            position = initialPosition;
        }

        public void Draw(SpriteBatch batch)
        {
            // lesson 9, slide 8
            rotation = (float)Math.Atan2(velocity.Y, velocity.X) - rotationOffset;
            try
            {
                batch.Draw(image, position, null, Color.White, rotation, Vector2.Zero,1, SpriteEffects.None, 0);
            }
            catch (InvalidOperationException)
            {
                batch.Begin();
                batch.Draw(image, position, null, Color.White, rotation, Vector2.Zero, 1, SpriteEffects.None, 0);
                batch.End();
            }
        }

        public void Draw(SpriteBatch batch, Color tint)
        {
            batch.Draw(image, position, null, tint, rotation, Vector2.Zero, 1, SpriteEffects.None, 0);
        }

        public void DrawAnimated(SpriteBatch batch)
        {
            if (animationTimeMax <= 0) return;
            while (currentAnimationTime > animationTimeMax) currentAnimationTime -= animationTimeMax;
            int currentFrame = (animationframes * currentAnimationTime) / animationTimeMax;
            int pixelsPerFrame = image.Width / animationframes;
            Rectangle animRect = new Rectangle(currentFrame * pixelsPerFrame, 0, pixelsPerFrame, image.Height);
            batch.Draw(image, position, animRect, Color.White);
        }

        public bool collision(GameSprite target)
        {
            if (target == null) return false;
            Rectangle sourceRectangle = new Rectangle(
                (int)position.X, (int)position.Y, image.Width, image.Height);
            Rectangle targetRectangle = new Rectangle(
                (int)target.position.X, (int)target.position.Y, target.image.Width, target.image.Height);
            return (sourceRectangle.Intersects(targetRectangle));
        }
    }

}
