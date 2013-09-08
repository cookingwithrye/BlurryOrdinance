using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using OSBO.GameObjects;
using OSBO.GameObjects.Weapons;

namespace OSBO
{
    public class Game2 : Microsoft.Xna.Framework.Game
    {
        #region Sound and Graphics asset content managers

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        AudioEngine audioEngine;
        SoundBank soundBank;

        #endregion

        /// <summary>
        /// Score
        /// </summary>
        Score gameScore;

        /// <summary>
        /// Game map rendered to the user
        /// </summary>
        Map theMap;

        /// <summary>
        /// Reference to the coordinates for the center of the screen. Used to draw relative positions of objects 
        /// </summary>
        Vector2 screenCenter;

        /// <summary>
        /// Player ship
        /// </summary>
        private Ship player;

        /// <summary>
        /// other objects that are part of the game
        /// </summary>
        private List<GameObject> gameObjects;

        /// <summary>
        /// draw the game relative to the player's ship?
        /// </summary>
        private bool drawRelative;
         
        /// <summary>
        /// keep track of the game's state - either playing or showing the menu
        /// </summary>
        private bool showingMenu;

        /// <summary>
        /// The menu to show
        /// </summary>
        Menu theMenu;

        /// <summary>
        /// Store the previous button to avoid the keyboard buffer filling up when a key is held down
        /// </summary>
        KeyboardState previousKeyState;


                public Game2()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            gameObjects = new List<GameObject>();

            showingMenu = false; //start the game out by showing the menu
            theMenu = new Menu(); // create the menu
            theMenu.showMenu = showingMenu;
        }

        // resets the game back to the starting state
        private void reset()
        {
            // reset the score to zero
            Score.reset();

            //Create player ship
            player = new Ship(theMap.center);
            player.shipColour = Color.Green;
            player.ENFORCE_SPEED_LIMIT = false;

            //Load player ship
            player.LoadContent(this.Content);

            //the game begins with all previous game objects removed
            gameObjects.Clear();
            
            //create some random asteroids, but disallow them from starting out in a collision with any other asteroid
            Random N = new Random();
            for (int i = 0; i < 10; i++)
            {
                Asteroid temp = new Asteroid(theMap, this.Content, N, soundBank);
                bool isCollision = false;
                foreach (GameObject otherAsteroid in gameObjects)
                {
                    if (temp.CollisionWith(otherAsteroid, true))
                    {
                        isCollision = true;
                        i--;
                        break;
                    }
                }
                if (!isCollision)
                    gameObjects.Add(temp);
            }

            //collect list of asteroids to remove from the game
            List<GameObject> toRemove = new List<GameObject>();
            
            //iterate over each object in the game and determine which (if any) other objects are colliding with it
            foreach (GameObject a in gameObjects)
            {
                foreach (GameObject b in gameObjects)
                {
                    if (a.CollisionWith(b, precise:true))
                    {
                        toRemove.Add(a);
                        break;
                    }
                }
            }

            //no longer process objects in the game loop if they aren't present in the map anymore
            foreach (GameObject bad in toRemove)
            {
                gameObjects.Remove(bad);
            }
        }
        
        protected override void Initialize()
        {
            // store the previous keyboard state
            previousKeyState = Keyboard.GetState();

            //Create a new map
            theMap = new Map(this.Content, "Textures//starsLarge");

            //Create a new score
            gameScore = new Score(this.Content);

            //draw the game relative to the player
            drawRelative = true;

            theMap.mapBoundary.visible = false;

            graphics.ToggleFullScreen();

            if (!graphics.IsFullScreen)
            {
                screenCenter = new Vector2(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2);
            }

            if (graphics.IsFullScreen)
            {
                //set the center of the screen based on the resolution
                graphics.PreferredBackBufferWidth = 1366;//1280;
                graphics.PreferredBackBufferHeight = 768;//1024;
                graphics.ApplyChanges();

                //set screen center
                screenCenter = new Vector2(graphics.GraphicsDevice.DisplayMode.Width / 2, graphics.GraphicsDevice.DisplayMode.Height / 2);
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            //load the menu content
            theMenu.LoadContent(this.Content);

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            audioEngine = new AudioEngine("Content//Audio//GameAudio.xgs");
            soundBank = new SoundBank(audioEngine, "Content//Audio//Sound Bank.xsb");

            //Create player ship
            player = new Ship(theMap.center);
            player.shipColour = Color.Green;
            player.ENFORCE_SPEED_LIMIT = false;

            //Load player ship
            player.LoadContent(this.Content);

            //load in the explosion
            Texture2D temp2 = Content.Load<Texture2D>("Textures//explosion");
            PreloadedTextures.preloadedTextures.Add(temp2);

            //load in the asteroids
            temp2 = Content.Load<Texture2D>("Textures//asteroidSmall1");
            PreloadedTextures.preloadedTextures.Add(temp2);

            temp2 = Content.Load<Texture2D>("Textures//asteroidSmall2");
            PreloadedTextures.preloadedTextures.Add(temp2);

            temp2 = Content.Load<Texture2D>("Textures//asteroidSmall3");
            PreloadedTextures.preloadedTextures.Add(temp2);

            // call the game's reset
            reset();

        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Escape) && !previousKeyState.IsKeyDown(Keys.Escape))
            {
                theMenu.showMenu = true; //pause the game and show the menu
            }

            // store the previous keyboard state
            previousKeyState = Keyboard.GetState();
            
            // only display the menu
            if (theMenu.showMenu)
            {
                theMenu.Update(gameTime);

                if (theMenu.state == "EXIT")
                {
                    // exit the game
                    this.Exit();
                }

                if (theMenu.state == "RUN GAME")
                {
                    theMenu.state = "";
                    theMenu.showMenu = false;
                    reset();
                }

                if (theMenu.state == "TOGGLE SCREEN")
                {
                    theMenu.state = "";

                    graphics.ToggleFullScreen();
                    if (!graphics.IsFullScreen)
                    {
                        screenCenter = new Vector2(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2);
                    }

                    if (graphics.IsFullScreen)
                    {
                        //set the center of the screen based on the resolution
                        graphics.PreferredBackBufferWidth = 1280;
                        graphics.PreferredBackBufferHeight = 1024;
                        graphics.ApplyChanges();

                        //set screen center
                        screenCenter = new Vector2(graphics.GraphicsDevice.DisplayMode.Width / 2, graphics.GraphicsDevice.DisplayMode.Height / 2);
                    }
                }

                base.Update(gameTime);
                return;
            }

            audioEngine.Update();

            //Update player ship
            player.Update(gameTime);
            theMap.ApplyBoundary(player);

            //if the ship is firing, receive the shots and add them to the game objects
            if (player.isFiring)
            {
                gameObjects.AddRange(player.Shoot(this.Content));
                player.isFiring = false;
                //soundBank.PlayCue("FireLaser");
            }

            //list of objects that have died and will be destroyed
            List<GameObject> toKill = new List<GameObject>();
            
            // iterate through each gameObject and update the position
            foreach (GameObject gameObject in gameObjects) 
            {
                gameObject.Update(gameTime); //update the position of each object

                theMap.ApplyBoundary(gameObject); //apply the map boundary to each object

                if (!gameObject.alive) //if the object is dead, add it to the list of things to kill
                    toKill.Add(gameObject);
            }

            //some objects return new items when they are destroyed 
            // -eg an asteroid returning several smaller ones when it's blown up
            List<GameObject> toAdd = new List<GameObject>(); 

            //iterate through the list of things to kill and destroy them
            foreach (GameObject gameObject in toKill)
            {
                toAdd.AddRange(gameObject.Destroy(this.Content));

                //remove the object from the game, unless it refused to be destroyed by setting alive back to true
                if (!gameObject.alive)
                    gameObjects.Remove(gameObject);
            }

            //add any daughter objects that were created when others were destroyed into our system
            gameObjects.AddRange(toAdd);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
           
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            // only display the menu
            if (theMenu.showMenu)
            {
                theMenu.Draw(spriteBatch);
                base.Draw(gameTime);

                spriteBatch.End();

                return;
            }
            
            if (drawRelative)
            {
                // draw the game object 8 more times so that the play looks continue
                theMap.DrawRelativeCube(spriteBatch, player, screenCenter, theMap.sprite.Width, theMap.sprite.Height);

                //Draw the map relative to the player
                theMap.DrawRelative(spriteBatch, player, screenCenter);
            }
            else
                theMap.Draw(this.spriteBatch);

            gameScore.Draw(spriteBatch);

            //Draw the objects
            foreach (GameObject gameObject in gameObjects)
            {
                if (drawRelative)
                {
                    // draw the game object relative to the ship
                    gameObject.DrawRelative(spriteBatch, player, screenCenter);

                    // draw the game object 8 more times so that the play looks continue
                    gameObject.DrawRelativeCube(spriteBatch, player, screenCenter, theMap.sprite.Width, theMap.sprite.Height);
                    
                }
                else
                    gameObject.Draw(spriteBatch);
        
                
                
                //check for collisions between this and other game objects
                foreach (GameObject otherGameObject in gameObjects)
                {
                    if (!((otherGameObject.collisionBehaviour == CollisionBehaviours.None) ||
                        (gameObject.collisionBehaviour == CollisionBehaviours.None)))
                    {
                        if (gameObject.CollisionWith(otherGameObject, !gameTime.IsRunningSlowly))
                        {
                            //apply the collision
                            gameObject.Collide(otherGameObject);
                            otherGameObject.Collide(gameObject);
                        }
                    }
                }

                // check for a collision between the ship and any of the game objects
                if (player.alive && player.CollisionWith(gameObject,true) && gameObject.collisionBehaviour != CollisionBehaviours.None)
                {
                    //apply the collision behaviour
                    player.Collide(gameObject);
                }
            }
            
            //Draw player ship
            player.DrawRelative(spriteBatch,player,screenCenter);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
