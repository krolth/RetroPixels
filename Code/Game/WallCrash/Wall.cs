using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WallAll
{
    class Wall
    {
        Texture2D txSegment;
        public Rectangle rect;

        public Wall(Texture2D txSeg, int x, int y, int width, int height)
        {
            this.txSegment = txSeg;
            rect = new Rectangle(x, y, width, height);
        }

        public void Update(int speed)
        {
            rect.Y += speed;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                        txSegment,
                        rect,
                        null,
                        Color.White,
                        0,
                        Vector2.Zero,
                        SpriteEffects.None,
                        1);
        }
    }
}
