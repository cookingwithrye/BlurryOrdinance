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

namespace OSBO.GameObjects
{
    /// <summary>
    /// A user's spaceship object.
    /// TODO: Decouple keyboard input from ship movement to allow this code to work in multiplayer over a network.
    /// </summary>
    class Ship : GameObject
    {
        #region Constants & Variables

        /// <summary>
        /// The rotation speed of the ship
        /// </summary>
        public float ROTATION_SPEED;

        /// <summary>
        /// The acceleration of the ship
        /// </summary>
        public float ACCELERATION_SPEED;

        /// <summary>
        /// Maximum speed that the ship is allowed to travel at
        /// </summary>
        public float MAX_SPEED;

        /// <summary>
        /// Is the specified speed limit enforced upon the ship?
        /// </summary>
        public bool ENFORCE_SPEED_LIMIT; //whether the speed limit is being enforced

        /// <summary>
        /// The maximum rate of fire for the ship (the repeat rate for the weapons being usable)
        /// </summary>
        public float FIRE_RATE, firingCountdown;

        /// <summary>
        /// Is the current state of the ship that it's firing? Required flag to prevent the buffer filling up with a bunch of fire instructions
        /// </summary>
        public bool isFiring;

        /// <summary>
        /// Does the ship currently have the shields up?
        /// </summary>
        public bool hasShieldsUp;
        
        /// <summary>
        /// Delay before the shields aren't rendered after they become visible from a hit
        /// </summary>
        public float shieldsShowCountdown;

        /// <summary>
        /// The amount of time to show the shields when a collision takes place
        /// </summary>
        public float SHIELDS_SHOW_SECONDS;

        /// <summary>
        /// The amount of "health" or "power" that shields have before they disappear
        /// </summary>
        public float SHIELDS_TOTAL_LIFESPAN;

        public float shieldsRemainingTime; //how much shield power you have
        
        //Current and previous states of the keyboard
        private KeyboardState currentKeyboardState,previousKeyboardState;

        //the ship's overlay (the part that changes colours to distinguish different ships)
        private Texture2D shipOverlay;
        private Texture2D shipShield;

        //the Ship overlay colour
        public Color shipColour;

        Texture2D shipHealth; 

        #endregion

        #region Constructors

        // initialize our ship, and give it a starting position
        public Ship(Vector2 startingPosition)
        {
            ROTATION_SPEED = 0.12f; //default rotation speed
            ACCELERATION_SPEED = 15f; //default acceleration speed
            FIRE_RATE = 0.2f; //default firing rate for ship
            ENFORCE_SPEED_LIMIT = true; //default is to enforce speed limit
            MAX_SPEED = 250f; //default maximum speed of the ship
            SHIELDS_SHOW_SECONDS = 0.3f; //default show shields time
            SHIELDS_TOTAL_LIFESPAN = 5f; //you have 10 seconds of shields before they run out
            
            firingCountdown = 0; //default is to be able to shoot immediately
            isFiring = false; //the ship doesn't start out firing
            shieldsShowCountdown = 0; //the ship doesn't start with shields visible
            shieldsRemainingTime = SHIELDS_TOTAL_LIFESPAN; //initialize your shield value

            position = startingPosition; //set the starting position

            shipColour = Color.White; //default ship colour has no tint

            //default ship behaviour when it hits an object is to be destroyed
            collisionBehaviour = CollisionBehaviours.Bounce;
        }

        #endregion

        #region Load Content

        public void LoadContent(ContentManager ContentManager)
        {
            //load the ship sprite and the overlay
            imageName = "Textures/ship0";
            base.LoadContent(ContentManager, imageName);

            shipOverlay = ContentManager.Load<Texture2D>("Textures/ship0Overlay");
            shipShield = ContentManager.Load<Texture2D>("Textures/shipShields");
            shipHealth = ContentManager.Load<Texture2D>("Textures//boundaryBorder");
        }

        #endregion
        
        #region Update method

        public override void Update(GameTime gameTime)
        {
            if (!alive)
                return;

            //get the keys being pressed, and remember them for the next update
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            if ((currentKeyboardState.IsKeyDown(Keys.Down)) && (!previousKeyboardState.IsKeyDown(Keys.Down)))
            {
                Console.Write("Down");
            }

            //update movements for the ship
            base.Update(gameTime);

            if (currentKeyboardState.IsKeyDown(Keys.Up)) //the user is pressing accelerate
            {
                //adjust our velocity
                Vector2 newVelocity = velocity + 
                    ACCELERATION_SPEED * (new Vector2((float)Math.Sin(rotation), -(float)Math.Cos(rotation)));

                //don't allow acceleration if we're already at max speed and it's being enforced
                if ((ENFORCE_SPEED_LIMIT) && (newVelocity.Length() > MAX_SPEED))
                    newVelocity = velocity;
                //if (Math.Sqrt(Math.Pow(newVelocity.X, 2) + Math.Pow(newVelocity.Y, 2)) > maxSpeed)

                velocity = newVelocity; //update the velocity based on any acceleration
            }

            if (currentKeyboardState.IsKeyDown(Keys.Left)) //the user is pressing left
            {
                rotation -= ROTATION_SPEED;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Right)) //the user is pressing right
            {
                rotation += ROTATION_SPEED;
            }

            // update the countdown until the ship is allowed to shoot again
            firingCountdown -= (float)gameTime.ElapsedGameTime.Milliseconds/1000;
            if (firingCountdown < 0)
                firingCountdown = 0;

            // update the shield countdown and determine whether the shields are up
            if ((shieldsShowCountdown > 0) && (shieldsRemainingTime > 0))
            {
                shieldsShowCountdown -= (float)gameTime.ElapsedGameTime.Milliseconds / 1000;
                shieldsRemainingTime -= (float)gameTime.ElapsedGameTime.Milliseconds / 1000;
                hasShieldsUp = true;
            }
            else
                hasShieldsUp = false;

            if (currentKeyboardState.IsKeyDown(Keys.Space)) //the user is pressing shoot
            {
                if (firingCountdown <= 0) //the ship is allowed to fire
                {
                    firingCountdown = FIRE_RATE; //reset the countdown
                    isFiring = true;
                }
            }
        }

        #endregion

        #region Draw

        ///overrides the default draw method to also draw the ship overlay
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!alive)
                return;

            //draw ship trunk
            base.Draw(spriteBatch);

            // draw ship overlay
            spriteBatch.Draw(shipOverlay, position, null, shipColour, rotation, center, scale, SpriteEffects.None, 0f);

            // if shields are up, then show them
            if (hasShieldsUp)
                spriteBatch.Draw(shipShield, position, null, shipColour, rotation, center, scale, SpriteEffects.None, 0f);
        }

        public override void DrawRelative(SpriteBatch spriteBatch, GameObject centeredObject, Vector2 centeredCoordinates)
        {
            if (!alive) //don't draw the ship if it's dead
                return;

            //draw ship trunk
            base.DrawRelative(spriteBatch, centeredObject, centeredCoordinates);

            // draw ship overlay
            spriteBatch.Draw(shipOverlay, position - centeredObject.position + centeredCoordinates, null,
                shipColour, rotation, center, scale, SpriteEffects.None, 0f);

            // if shields are up, then show them
            if (hasShieldsUp)
                spriteBatch.Draw(shipShield, position - centeredObject.position + centeredCoordinates, null, 
                    shipColour, rotation, center, scale, SpriteEffects.None, 0f);

            // draw ship health
            // draw ship health
            Rectangle healthBar = new Rectangle(
                (int)centeredCoordinates.X - 32,
                (int)centeredCoordinates.Y + 30,
                (int)((this.shieldsRemainingTime/this.SHIELDS_TOTAL_LIFESPAN)*this.sprite.Width), 10);

            spriteBatch.Draw(shipHealth, healthBar, Color.Green);
        }

        #endregion

        #region Shoot method

        ///returns a list of objects which are the shots from the ship
        public virtual List<GameObject> Shoot(ContentManager contentManager)
        {
            //ships by default only fire one laser
            List<GameObject> shots = new List<GameObject>();

            //add a laser shot
            Laser shot = new Laser(position, center, rotation, velocity, contentManager);
            shot.owner = this;
            shot.hitOwnerCountdown = 5.0f; //the shot can only hit the ship that fired it after a 1 second delay

            shots.Add(shot);

            return shots;
        }

        #endregion

        /// <summary>
        /// Apply collision behaviour between this ship and the specified object
        /// </summary>
        /// <param name="otherObject"></param>
        public override void Collide(GameObject otherObject)
        {
            //don't let the ship be destroyed in a collision if there are shields
            if (shieldsRemainingTime > 0)
            {
                shieldsShowCountdown = SHIELDS_SHOW_SECONDS; //show the shields up for the specified duration around the ship
                this.collisionBehaviour = CollisionBehaviours.Bounce;
            }
            else
            {
                //whatever blew the ship up should probably go too
                this.collisionBehaviour = CollisionBehaviours.DestroyBoth; 
            }

            base.Collide(otherObject);
        }
    }
}
