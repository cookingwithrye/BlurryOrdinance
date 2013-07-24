#region Using Statements

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

#endregion

namespace OSBO
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //map
        Map theMap;
        
        //Player ship
        OldShip player;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            //Create a new map
            theMap = new Map(this.Content, "Textures//stars1");

            //override default boundaries for testing
            theMap.mapBoundary.Left = 0;
            theMap.mapBoundary.Right = 800;
            theMap.mapBoundary.Bottom = 600;
            theMap.mapBoundary.Top = 0;
            theMap.mapBoundary.visible = true;
            theMap.mapBoundary.BoundaryType = BoundaryTypes.Transport;

            theMap.debug("");

            //Create player ship
            player = new OldShip();
            player.theMap = theMap;
            player.position = new Vector2(200,200);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            //Load player ship
            player.LoadContent(this.Content);

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            //Update player ship
            theMap.ApplyBoundary(player);
            player.Update(gameTime);
            
            //theMap.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            //Draw the map
            theMap.Draw(this.spriteBatch);

            //Draw the map relative to the ship
            //theMap.DrawRelative(this.spriteBatch, player);

            //Draw player ship
            player.Draw(this.spriteBatch);

            //Draw the ship relative to tself being centered on the screen
            //player.DrawRelative(this.spriteBatch, player);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
