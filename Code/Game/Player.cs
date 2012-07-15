using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WallAll
{
    class Player
    {
        Texture2D txSegment;

        Vector2 pos;
        Rectangle rect;

        public Player(Texture2D txSeg)
        {
            this.txSegment = txSeg;
        }

        public bool Update(List<Wall> walls, Vector2 updatedPos, int size)
        {
            pos = updatedPos;

            int posOffset = size / 2;
            rect = new Rectangle((int)pos.X - posOffset, (int)pos.Y - posOffset, size, size);

            // Check for collisions
            foreach (var wall in walls)
            {
                if (wall.rect.Intersects(rect))
                    return false;
            }

            return true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                        txSegment,
                        rect,
                        null,
                        Color.Red,
                        0,
                        Vector2.Zero,
                        SpriteEffects.None,
                        1);
        }
    }
}
