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

namespace OSBO.GameObjects.Weapons
{
    /// a specific weapon
    class Laser : GameObject
    {
        #region Constructor

        public Laser(Vector2 position, Vector2 center, float rotation, Vector2 initialVelocity, ContentManager contentManager)
        {
            // speed and size of the laser
            scale = 1.0f;
            float speed = 1000f;

            //texture for this weapons
            imageName = "Textures//laserShot";
            base.LoadContent(contentManager,imageName);

            // collision behaviour for a laser is to destroy a hit object
            collisionBehaviour = CollisionBehaviours.DestroyBoth;
            
            // laser starts in the same location as the ship that fired it
            this.position = position;

            //laser is aimed in the same direction as the ship that fired it
            this.rotation = rotation; 
            
            // calculate the velocity of the laser
            this.velocity = new Vector2(
                    (float)Math.Sin(this.rotation),
                    -(float)Math.Cos(this.rotation))
                    * speed + initialVelocity;

            this.expires = true; //lasers only last 1.5 seconds
            this.lifespan = 0.5f;
        }

        #endregion

        #region Collision

        /// overide the default GameObject collision to ignore collisions between lasers
        public override void Collide(GameObject otherObject)
        {
            // lasers don't collide with each other
            if (!(otherObject is Laser))
                base.Collide(otherObject);
        }

        #endregion

    }
}
