//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.Xna.Framework;

//namespace WallAll.Game.LaserGraze
//{

//    class Particle 
//    {
//        public static int gravity = 0;
//        public static Rectangle rect = new Rectangle();
//        public static int drawIndex, color;

//        public Vector3 pos = new Vector3();
//        public Vector3 vel = new Vector3();

//        public float size, attenuation;

//        public Particle(Vector3 p, int vx, int vy, int size, int attenuation) 
//        {
//            pos.X = p.X; pos.Y = p.Y; vel.X = vx; vel.Y = vy;
//            this.size = size + 0.9f;

//            this.attenuation = attenuation;
//        }
//        public bool Update()
//        {
//            pos.X += vel.X; pos.Y += vel.Y;
//            vel.Y += gravity;
//            size *= attenuation;
        
//            return IsInScreen(pos) && size >= 1.0;
//        }

//        public void SetDrawIndex(int i)
//        {
//            drawIndex = i;
//            int bright = 0xff - i * 0x55;
//            color = bright * 0x10000 + bright * 0x100 + bright;
//        }
    
//        public void draw()
//        {
//            float sz = size * (1.0f + drawIndex * 0.5f);
//            rect.X = (int) (pos.X - sz / 2); rect.Y = (int)(pos.Y - sz / 2);
//            rect.Width = rect.Height = (int)(sz);
        
//            //bd.fillRect(rect, color);
//        }
//    }
//}
