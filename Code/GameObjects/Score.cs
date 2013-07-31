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
    /// <summary>
    /// Store information about the user's score
    /// </summary>
    class Score
    {
        private SpriteFont font;
        private static int score = 0;
        
        /// <summary>
        /// Render location of the score in the game
        /// </summary>
        private Vector2 scorePosition = new Vector2(0,0);

        public Score(ContentManager contentManager)
        {
            font = contentManager.Load<SpriteFont>("Fonts/Arial");
        }

        /// <summary>
        /// Draw the user's score on the screen
        /// </summary>
        /// <param name="spriteBatch">Reference the batch writer for screen sprites. Required for performance reasons.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            
            spriteBatch.DrawString(font, "Score: " + score.ToString(), scorePosition, Color.White);
        }

        /// <summary>
        /// Increase the player's score
        /// </summary>
        public static void Update()
        {
            score++;
        }

        /// <summary>
        /// Reset the player's score to zero
        /// </summary>
        public static void reset()
        {
            score = 0;
        }
    }
}
