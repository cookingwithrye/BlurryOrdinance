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
    // runs a test of the animation gameobjects
    public class Game4AnimationTest : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Sounds (Load Content, Update)
        AudioEngine audioEngine;
        SoundBank soundBank;
        WaveBank waveBank;

        //Score
        Score gameScore;

        //map
        Map theMap;

        //screen center
        Vector2 screenCenter;

        //Player ship
        Ship player;

        //other objects
        List<GameObject> gameObjects;

        //draw the game relative to the player's ship?
        bool drawRelative;

        public Game4AnimationTest()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            gameObjects = new List<GameObject>();
        }

        protected override void Initialize()
        {
            //Create a new map
            theMap = new Map(this.Content, "Textures//starsLarge");

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

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            AnimatedGameObject a = new AnimatedGameObject(0.3f, true, 4, new Rectangle(0,0,512,512));
            a.LoadContent(this.Content, "Textures//AnimExplosion");
            a.position = new Vector2(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2);
            gameObjects.Add(a);
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Q))
                this.Exit();

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

            theMap.Draw(this.spriteBatch);

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
                    if (gameObject.CollisionWith(otherGameObject, !gameTime.IsRunningSlowly))
                    {
                        //apply the collision
                        gameObject.Collide(otherGameObject);
                        otherGameObject.Collide(gameObject);
                    }
                }

            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
