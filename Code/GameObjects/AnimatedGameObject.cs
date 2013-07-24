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

using System.Text;

namespace OSBO.GameObjects
{
    
    // loads in a spritesheet and animates it while preserving the standard gameobject behaviours
    class AnimatedGameObject : GameObject
    {

        #region Constants and private members

        // the total length of the animation
        private float cycleLength;

        // the length of the current animation cycle
        private float currentCycle;

        // whether the animation repeats
        private Boolean repeat;

        // the number of rows and columns of frames in a spritesheet - multiplying these together gives you the 
        //  total number of frames in an animation
        private int numFrames;

        //the size of an individual frame inside our spritesheet
        private Rectangle frameSize;

        #endregion

        //set the default constants and variables
        public AnimatedGameObject(float cycleLength, Boolean repeat, int numFrames, Rectangle frameSize)
        {
            this.numFrames = numFrames;
            this.repeat = repeat;
            this.cycleLength = cycleLength;
            this.frameSize = frameSize;
            this.currentCycle = 0; // we'll start drawing at the first frame
        }

        //create a AnimatedGameObject that expires
        public AnimatedGameObject(float cycleLength, Boolean repeat, int numFrames, Rectangle frameSize, float lifespan) 
            : this(cycleLength, repeat, numFrames, frameSize)
        {
            base.expires = true;
            base.lifespan = lifespan;
        }

        //override the draw method and only draw the desired frame
        public override void Draw(SpriteBatch spriteBatch)
        {
            //the animation doesn't need to be drawn if it's not on repeat and it has completed
            if (!repeat && (currentCycle > cycleLength))
                return;

            // actually draw the object
            Rectangle drawRectangle = new Rectangle();
            drawRectangle.X = (int)(currentCycle / cycleLength * numFrames) * frameSize.Width;
            this.center.X = frameSize.Width / 2 + (int)(currentCycle / cycleLength * numFrames);
            drawRectangle.Y = frameSize.Y;
            drawRectangle.Width = frameSize.Width;
            drawRectangle.Height = frameSize.Height;

            Console.WriteLine(drawRectangle);
            //Console.WriteLine((int)(currentCycle / cycleLength * numFrames) * frameSize.Width);

            spriteBatch.Draw(sprite, position, drawRectangle, Color.White, rotation, center, scale, SpriteEffects.None, 0f);
        }

        //override the relative drawing methods
        public override void DrawRelative(SpriteBatch spriteBatch, GameObject centeredObject, Vector2 centeredCoordinates)
        {
            // actually draw the object
            Rectangle drawRectangle = new Rectangle();
            drawRectangle.X = (int)(currentCycle / cycleLength * numFrames) * frameSize.Width;
            this.center.X = frameSize.Width / 2 + (int)(currentCycle / cycleLength * numFrames);
            drawRectangle.Y = frameSize.Y;
            drawRectangle.Width = frameSize.Width;
            drawRectangle.Height = frameSize.Height;

            spriteBatch.Draw(sprite, position - centeredObject.position + centeredCoordinates, drawRectangle,
                Color.White, rotation, center, scale, SpriteEffects.None, 0f);
        }

        //update the cycle time
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (this.currentCycle <= this.cycleLength)
                this.currentCycle += (float)gameTime.ElapsedGameTime.Milliseconds / 1000;

            if (repeat && (this.currentCycle > this.cycleLength))
                this.currentCycle = 0;
        }
    
    }
}
