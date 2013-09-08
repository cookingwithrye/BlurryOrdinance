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

namespace OSBO
{
    public class Menu
    {

        public String state; // store the item selected on the menu when the user pressed enter
        public bool showMenu; // store whether to show the menu or not
        
        Texture2D back;

        KeyboardState previousKeyState;
        KeyboardState keyboardState;

        int keyboardCounter = 1;

        #region "Selector"

        Texture2D menuSelector;

        Vector2 selectSingle = new Vector2(295, 198);
        Vector2 selectSounds = new Vector2(295, 288);
        Vector2 selectGraphics = new Vector2(295, 383);
        Vector2 selectExit = new Vector2(295, 478);

        Vector2 selectorPos;

        #endregion

        #region "Menu"

        Vector2 menuSingle = new Vector2(360, 210);
        Vector2 menuSounds = new Vector2(360, 300);
        Vector2 menuGraphics = new Vector2(360, 395);
        Vector2 menuExit = new Vector2(360, 490);

        #endregion

        String Sound = "Sounds On";
        String Graphics = "Windowed";

        private SpriteFont comicSansFont;

        public Menu()
        {
            this.state = "";
            showMenu = true;
            selectorPos = selectSingle;
        }

        protected void Initialize()
        {
        }

        public void LoadContent(ContentManager contentManager)
        {
            menuSelector = contentManager.Load<Texture2D>(@"Textures/ship0Overlay");
            back = contentManager.Load<Texture2D>(@"Textures/green");
            comicSansFont = contentManager.Load<SpriteFont>(@"Textures/SpriteFont1");
        }

        public void UnloadContent()
        {
        }

        public void Update(GameTime gameTime)
        {

            previousKeyState = keyboardState;
            keyboardState = Keyboard.GetState();

            // hide the menu
            if (keyboardState.IsKeyDown(Keys.Escape) && !previousKeyState.IsKeyDown(Keys.Escape))
            {
                this.showMenu = false;
            }

            if ((keyboardState.IsKeyDown(Keys.Down) && !previousKeyState.IsKeyDown(Keys.Down)))
            {
                if (keyboardCounter > 0 && keyboardCounter < 4)
                {
                    keyboardCounter += 1;

                    if (keyboardCounter == 1) { selectorPos = selectSingle; }
                    else if (keyboardCounter == 2) { selectorPos = selectSounds; }
                    else if (keyboardCounter == 3) { selectorPos = selectGraphics; }
                    else if (keyboardCounter == 4) { selectorPos = selectExit; }
                }
            }
            else if ((keyboardState.IsKeyDown(Keys.Up) && !previousKeyState.IsKeyDown(Keys.Up)))
            {
                if (keyboardCounter > 1 && keyboardCounter < 5)
                {
                    keyboardCounter -= 1;

                    if (keyboardCounter == 1) { selectorPos = selectSingle; }
                    else if (keyboardCounter == 2) { selectorPos = selectSounds; }
                    else if (keyboardCounter == 3) { selectorPos = selectGraphics; }
                    else if (keyboardCounter == 4) { selectorPos = selectExit; }

                }
            }

            if (keyboardState.IsKeyDown(Keys.Enter) && !previousKeyState.IsKeyDown(Keys.Enter))
            {
                if (keyboardCounter == 1)
                {
                    this.state = "RUN GAME";
                }
                else if (keyboardCounter == 2)
                {
                    if (Sound == "Sounds On")
                    {
                        this.state = "SOUND OFF";
                        Sound = "Sounds Off";
                    }
                    else
                    {
                        Sound = "Sounds On";
                        this.state = "SOUND ON";
                    }
                }
                else if (keyboardCounter == 3)
                {
                    Graphics = "Toggle Fullscreen";
                    this.state = "TOGGLE SCREEN";
                }
                else if (keyboardCounter == 4) 
                {
                    this.state = "EXIT";
                }
            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //The game menu panel background color
            spriteBatch.Draw(back, new Vector2(240, 160), null, Color.White, 0, Vector2.Zero, new Vector2(320, 390), SpriteEffects.None, 0);

            //Menu selector
            spriteBatch.Draw(menuSelector, selectorPos, Color.White);

            //Menu items
            spriteBatch.DrawString(comicSansFont, "Start Game", menuSingle, Color.Black);
            spriteBatch.DrawString(comicSansFont, Sound, menuSounds, Color.Black);
            spriteBatch.DrawString(comicSansFont, Graphics, menuGraphics, Color.Black);
            spriteBatch.DrawString(comicSansFont, "Exit", menuExit, Color.Black);

            spriteBatch.DrawString(comicSansFont, keyboardCounter.ToString(), new Vector2(10, 10), Color.White);
        }
    }
}
