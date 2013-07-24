using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace OSBO.GameObjects
{
    class Score
    {
        private SpriteFont font;
        private static int score = 0;
        private Vector2 scorePosition = new Vector2(0,0);

        public Score(ContentManager contentManager)
        {
            font = contentManager.Load<SpriteFont>("Fonts/Arial");
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            
            spriteBatch.DrawString(font, "Score: " + score.ToString(), scorePosition, Color.White);
        }

        public static void Update()
        {
            score++;
        }

        public static void reset()
        {
            score = 0;
        }
    }
}
