#region Using Statements

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

#endregion

namespace OSBO.GameObjects.Weapons
{
    /// <summary>
    /// Class for Laser object
    /// </summary>
    class OldLaser : GameObject
    {

        #region Constructor

        /// <summary>
        /// Constructor for laser object
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="velocity"></param>
        public OldLaser(Vector2 position, Vector2 center, float rotation, ContentManager contentManager)
        {
            imageName = "Textures//laserShot";
            this.sprite = contentManager.Load<Texture2D>(imageName);
            scale = 1.0f;
            float speed = 650f;

            this.position = position;
            this.center = new Vector2(this.sprite.Width / 2, this.sprite.Height / 2);
            
            this.rotation = rotation;
            this.velocity = new Vector2(
                    (float)Math.Sin(this.rotation),
                    -(float)Math.Cos(this.rotation))
                    * speed;
            this.expires = true;
            this.lifespan = 3;
        }

        #endregion

    }
}
