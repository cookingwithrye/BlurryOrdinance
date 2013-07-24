#region Using Statements

using System;
using System.Collections;
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
using OSBO.GameObjects.Weapons;

#endregion

namespace OSBO.GameObjects
{
    /// <summary>
    /// Class for ship game object
    /// </summary>
    class OldShip : GameObject
    {
        #region Constants & Variables

        public Map theMap;

        //Number for max amount of rotation
        public const float MAX_ROTATION = .1f;

        //Number for speed change
        public const float SPEED_CHANGE = 10f;

        //2D texture of ship
        public Texture2D shipTexture;

        //2D texture of overlay
        public Texture2D overlayTexture;
        
        //2D texture of sheild
        public Texture2D sheildTexture;

        //Current state of keyboard
        public KeyboardState currentKeyboardState;

        //Previous state of keyboard
        public KeyboardState previousKeyboardState;

        //Array of weapon fire
        public List<Laser> weaponFire = new List<Laser>();

        //Max fire rate while holding space down (in fps)
        private const int MAX_FIRE_RATE = 4;
        private int lastFired; // keeps track of when the last shot was fired

        public float maxSpeed;

        public ContentManager myContentManager;

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor for ship object
        /// </summary>
        public OldShip()
        {
            scale = 0.8f;
            maxSpeed = 350f;
            lastFired = MAX_FIRE_RATE; //set this up so the ship is capable of shooting immediately
            position.X = 300;
            position.Y = 300;
        }

        #endregion

        /// <summary>
        /// Draw the textures of:
        /// - Ship
        /// - Overlay
        /// - Sheild
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            //draw the weapons fire first so it looks like it's coming from the ship
            foreach (Laser shot in weaponFire)
            {
                spriteBatch.Draw(shot.sprite, shot.position, null, Color.White, shot.rotation, center, shot.scale, SpriteEffects.None, 0f);
            }
            spriteBatch.Draw(shipTexture, position, null, Color.White, rotation, center, scale, SpriteEffects.None, 1f);
            spriteBatch.Draw(overlayTexture, position, null, Color.Brown, rotation, center, scale, SpriteEffects.None, 1f);
            spriteBatch.Draw(sheildTexture, position, null, Color.Blue, rotation, center, scale, SpriteEffects.None, 1f);
        }

        //// draw this GameObject relative to another one
        //public override void DrawRelative(SpriteBatch spriteBatch, GameObject centeredObject)
        //{
        //    //draw the weapons fire first so it looks like it's coming from the ship
        //    foreach (Laser shot in weaponFire)
        //    {
        //        spriteBatch.Draw(shot.sprite, shot.position - centeredObject.position + theMap.center, null, Color.White, shot.rotation, center, shot.scale, SpriteEffects.None, 0f);
        //    }
        //    spriteBatch.Draw(shipTexture, position - centeredObject.position + theMap.center, null, Color.White, rotation, center, scale, SpriteEffects.None, 1f);
        //    spriteBatch.Draw(overlayTexture, position - centeredObject.position + theMap.center, null, Color.Brown, rotation, center, scale, SpriteEffects.None, 1f);
        //    spriteBatch.Draw(sheildTexture, position - centeredObject.position + theMap.center, null, Color.Blue, rotation, center, scale, SpriteEffects.None, 1f);
        //}

        /// <summary>
        /// Load the textures of:
        /// - Ship
        /// - Overlay
        /// - Sheild
        /// Set:
        /// - center
        /// </summary>
        /// <param name="ContentManager"></param>
        public void LoadContent(ContentManager ContentManager)
        {
            myContentManager = ContentManager;

            shipTexture = ContentManager.Load<Texture2D>("Textures/ship0");
            overlayTexture = ContentManager.Load<Texture2D>("Textures/ship0Overlay");
            sheildTexture = ContentManager.Load<Texture2D>("Textures/shipShields");
            
            center = new Vector2(shipTexture.Width / 2, shipTexture.Height / 2);
        }

        #region Update Methods

        /// <summary>
        /// Update the ship's:
        /// - Movement
        /// Set:
        /// - previousKeyboardState
        /// - currentKeyboardState
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            currentKeyboardState = Keyboard.GetState();

            UpdateMovement(currentKeyboardState, gameTime);

            previousKeyboardState = currentKeyboardState;
        }

        /// <summary>
        /// Method to update ship's movement
        /// Set:
        /// - speed
        /// - position
        /// - velocity
        /// - rotation
        /// </summary>
        /// <param name="currentKeyboardState"></param>
        private void UpdateMovement(KeyboardState currentKeyboardState, GameTime gameTime)
        {
            base.Update(gameTime);

            List<Laser> toKill = new List<Laser>(); //store a list of the laser objects that will be removed

            foreach (Laser shot in weaponFire) 
            {
                if (!shot.alive)
                    toKill.Add(shot);
                else
                {
                    shot.Update(gameTime);
                    theMap.ApplyBoundary(shot);
                }

                toKill.Add(null);
            }

            foreach (Laser shot in toKill)
            {
                weaponFire.Remove(shot);
            }

            if (currentKeyboardState.IsKeyDown(Keys.Up) == true)
            {
                Vector2 NewVelocity = velocity + SPEED_CHANGE * (new Vector2((float)Math.Sin(rotation), -(float)Math.Cos(rotation)));

                if (Math.Sqrt(Math.Pow(NewVelocity.X, 2) + Math.Pow(NewVelocity.Y, 2)) > maxSpeed)
                {
                    NewVelocity = velocity;
                }
                velocity = NewVelocity;
            }

            //if (currentKeyboardState.IsKeyDown(Keys.Down) == true)
            //{
            //    Vector2 NewVelocity = velocity - (SPEED_CHANGE / 2) * (new Vector2((float)Math.Sin(rotation), -(float)Math.Cos(rotation)));

            //    if (Math.Sqrt(Math.Pow(NewVelocity.X, 2) + Math.Pow(NewVelocity.Y, 2)) > maxSpeed)
            //    {
            //        NewVelocity = velocity;
            //    }
            //    velocity = NewVelocity;
            //}

            if (currentKeyboardState.IsKeyDown(Keys.Left) == true)
            {
                rotation -= MAX_ROTATION;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Right) == true)
            {
                rotation += MAX_ROTATION;
            }

            //allow firing by holding space down at a rate specified by GameTime
            lastFired++;
            if ((currentKeyboardState.IsKeyDown(Keys.Space)) && (lastFired > MAX_FIRE_RATE)) //&& (previousKeyboardState.IsKeyUp(Keys.Space)) == true)
            {
                weaponFire.Add(new Laser(position, center, this.rotation, velocity, myContentManager));
            }
            if (lastFired > MAX_FIRE_RATE)
                lastFired = 0;

        }

        #endregion

    }
}
