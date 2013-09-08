using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace OSBO.GameObjects.Weapons
{
    /// <summary>
    /// A missile that homes in on the nearest asteroid within range and pursues it
    /// </summary>
    public class HomingMissile : Weapon
    {
        protected GameObject _targettedAsteroid = null;
        
        /// <summary>
        /// Acquire a target 
        /// </summary>
        /// <returns>True if asteroid is targetted, false otherwise</returns>
        public bool AcquireTarget(IList<GameObject> asteroidsInRange, Ship theShip)
        {
            //identify the closest asteroid to the current ship position
            GameObject closest = null;
            float closestDistance = float.MaxValue;
            foreach (var asteroid in asteroidsInRange)
            {
                float tempDistance = theShip.DistanceFrom(asteroid);
                if (closest == null || tempDistance < closestDistance)
                {
                    closest = asteroid;
                    closestDistance = tempDistance;
                }
            }

            if (closest != null)
                return true;
            else
                return false;
        }

        public void HomingMissile()
        {
        }

        /// update the position of this GameObject
        public override void Update(GameTime gameTime)
        {
            if (this.alive)
            {
                // if there has been an update in velocity, apply it before updating position
                if (newVelocity != Vector2.Zero)
                {
                    velocity = newVelocity;
                    newVelocity = Vector2.Zero;
                }

                // update position based on velocity
                position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

                // apply rotation constant
                rotation += rotationConstant;

                //check if the object expires, and if so adjust it's lifespan countdown
                if (this.expires)
                {
                    this.lifespan -= (float)gameTime.ElapsedGameTime.Milliseconds / 1000;

                    //see if the object has now expired
                    if (this.lifespan <= 0)
                        this.alive = false; //the object has died
                }

                //countdown for when this object can hit the owner of it
                if (hitOwnerCountdown > 0)
                    hitOwnerCountdown -= (float)gameTime.ElapsedGameTime.Milliseconds / 1000;

                //conserve speed
                float currentSpeed = (float)Math.Sqrt(Math.Pow(this.velocity.X, 2) + Math.Pow(this.velocity.Y, 2));

                //change direction
            }
        }

    }
}
