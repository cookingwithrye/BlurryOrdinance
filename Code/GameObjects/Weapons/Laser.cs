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
    /// <summary>
    /// Generic weapon capabilities
    /// </summary>
    public class Weapon : GameObject
    {
        #region Constructor
        
        public Weapon(Vector2 position, Vector2 center, float rotation, Vector2 initialVelocity, ContentManager contentManager)
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

            this.expires = true; //by default all weapons expire after some timespan
            this.lifespan = 0.5f;
        }

        #endregion

        #region Collision

        /// <summary>
        /// Override the default collision behaviour so that lazers pass through each other but other collision effects are still applied.
        /// </summary>
        /// <param name="otherObject"></param>
        public override void Collide(GameObject otherObject)
        {
            // lasers don't collide with each other, but all other objects still have collision behaviour
            if (!(otherObject is Laser))
                base.Collide(otherObject);
        }

        #endregion
    }

    /// <summary>
    /// Create a lazer weapon
    /// </summary>
    public class Laser : Weapon
    {
        /// <summary>
        /// Construct a basic lazer weapon
        /// </summary>
        /// <param name="position"></param>
        /// <param name="center"></param>
        /// <param name="rotation"></param>
        /// <param name="initialVelocity"></param>
        /// <param name="contentManager"></param>
        public void Weapon(Vector2 position, Vector2 center, float rotation, Vector2 initialVelocity, ContentManager contentManager) 
            : base(position, center, rotation, initialVelocity, contentManager)
        {
            
        }
    }

}
