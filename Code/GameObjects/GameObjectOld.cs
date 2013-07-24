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

namespace OSBO.GameObjects
{
    /// <summary>
    /// A base class for all game objects
    /// Notes:
    /// - Get/Set for center?
    /// </summary>
    class GameObjectOld
    {
        public void debug(String indent) {
            Console.WriteLine(indent + "GameObject Debug Data");
            Console.WriteLine(indent + "----------------------------");
            Console.WriteLine(indent + "sprite:" + imageName);
            Console.WriteLine(indent + "position: (" + position.X + "," + position.Y + ")");
            Console.WriteLine(indent + "center: (" + center.X + "," + center.Y + ")");
            Console.WriteLine(indent + "velocity: (" + velocity.X + "," + velocity.Y + ")");
            Console.WriteLine(indent + "alive: " + alive.ToString());
            Console.WriteLine(indent + "rotation: " + rotation.ToString());
            Console.WriteLine(indent + "maxSpeed: " + maxSpeed.ToString());
            Console.WriteLine(indent + "lifespan: " + lifespan.ToString());
        }


        #region Constants & Variables

        //2D texture of game object
        public Texture2D sprite;

        //Vector2 co-ordinates for game object's center, position and velocity
        public Vector2 center, position, velocity;

        //True/False variable for game object's alive check
        public bool alive;

        //Number variable for rotation of game object
        public float rotation;

        //Number variable for scale of game object
        public float scale;

        //Number variable for speed
        public float speed;

        //Number for max speed
        public float maxSpeed;

        //String for image file source
        public string imageName;

        //lifespan (in ticks) before dying
        public int lifespan;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor for a new game object
        /// </summary>
        /// <param name="mySprite"></param>
        public GameObject()
        {
            alive = true;
            lifespan = -1; // by default lifespan is inifinite
        }

        #endregion

        public virtual void LoadContent(ContentManager contentManager, string imageName)
        {
            this.imageName = imageName;
            sprite = contentManager.Load<Texture2D>(imageName);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, position, null, Color.White, rotation, center, scale, SpriteEffects.None, 0f);
        }

        public virtual void Update(GameTime gameTime)
        {
            UpdatePosition(gameTime);
        }

        /// <summary>
        /// Method to set velocity
        /// </summary>
        public void SetVelocity(float newSpeed)
        {
           /*velocity = new Vector2(
                    (float)Math.Sin(rotation),
                    -(float)Math.Cos(rotation))
                    * speed;*/

            //velocity = velocity - newSpeed*(new Vector2((float)Math.Sin(rotation),-(float)Math.Cos(rotation)));
        }

        /// <summary>
        /// Update position every second to keep velocity
        /// </summary>
        public void UpdatePosition(GameTime gameTime)
        {
            // check if the object should expire
            //if (lifespan != -1)
            //{
            //    if (lifespan-- <= 0)
            //        alive = false;
            //}

            //if (position.X > 800 || position.X < 0)
            //{
            //    velocity = new Vector2(-velocity.X, velocity.Y);
            //    rotation = -rotation;
            //}

            //if (position.Y > 600 || position.Y < 0)
            //{
            //    velocity = new Vector2(velocity.X, -velocity.Y);
            //    rotation = -rotation;
            //}

            if (velocity != Vector2.Zero)
                position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }

}
