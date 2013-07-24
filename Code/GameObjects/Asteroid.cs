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
    // an asteroid
    class Asteroid : GameObject
    {
        private ContentManager contentManager; //store a reference for later use
        private SoundBank soundBank; //store a reference for later use
        private Random N;
        
        // create an asteroid randomly for a given map
        public Asteroid(Map theMap, ContentManager contentManager, Random N, SoundBank soundBank)
        {
            this.N = N;
            this.contentManager = contentManager;
            this.soundBank = soundBank;

            String [] asteroidTextures = new String[3] { 
                "Textures//asteroidSmall1", "Textures//asteroidSmall2", "Textures//asteroidSmall3"};
            
            this.imageName = asteroidTextures[N.Next(3)];

            this.position = new Vector2(
                N.Next(theMap.mapBoundary.Left, theMap.mapBoundary.Right - theMap.mapBoundary.Left),
                N.Next(theMap.mapBoundary.Top, theMap.mapBoundary.Bottom - theMap.mapBoundary.Top));

            this.velocity = new Vector2(N.Next(1, 500) - 250, N.Next(1, 500) - 250);
            this.scale = 3.0f / (float)N.Next(1,6);
            this.rotationConstant = 0.01f * (float)N.Next(5) - 0.01f * (float)N.Next(5);

            this.sprite = PreloadedTextures.preloadedTextures.ElementAt<Texture2D>(N.Next(3) + 1);
            center = new Vector2(sprite.Width / 2, sprite.Height / 2);
            colours = TextureTo2DArray(sprite);

            //load the texture
            //base.LoadContent(contentManager, imageName);
        }

        // create an asteroid smaller than the given one
        public Asteroid(Asteroid parent, ContentManager contentManager, Random N, SoundBank soundBank)
        {
            this.contentManager = contentManager;
            this.soundBank = soundBank;
            this.N = N;

            String[] asteroidTextures = new String[3] { 
                "Textures//asteroidSmall1", "Textures//asteroidSmall2", "Textures//asteroidSmall3"};

            this.imageName = asteroidTextures[N.Next(3)];

            this.scale = parent.scale * 3 / N.Next(4, 6);

            this.position = parent.position + new Vector2(N.Next(100)-50,N.Next(100)-50);
            this.velocity = new Vector2(N.Next(100) - 50, N.Next(100) - 50) + (N.Next(1, 2) - 2) * 0.3f * parent.velocity - (N.Next(1, 2) - 2) * 0.3f * parent.velocity;

            this.rotationConstant = 0.01f * (float)N.Next(5) - 0.01f * (float)N.Next(5);

            this.sprite = PreloadedTextures.preloadedTextures.ElementAt<Texture2D>(N.Next(3) + 1);
            center = new Vector2(sprite.Width / 2, sprite.Height / 2);
            colours = TextureTo2DArray(sprite);

            //load the texture
            //base.LoadContent(contentManager, imageName);
        }

        public override List<GameObject> Destroy(ContentManager contentManager)
        {
            List<GameObject> temp = base.Destroy(contentManager);
            
            if (this.scale > 0.4f)
            {
                int j = N.Next(2,5);
                //j = -1;
                for (int i = 0; i < j; i++)
                    temp.Add(new Asteroid(this, contentManager, N, soundBank));    
            }

            //AnimatedGameObject a = new AnimatedGameObject(0.2f, false, 4, new Rectangle(0, 0, 512, 512), 0.2f);
            //temp.Add(a);
            temp.Add(new Explosion(OSBO.GameObjects.PreloadedTextures.preloadedTextures.ElementAt<Texture2D>(0),position, scale, contentManager, soundBank));

            OSBO.GameObjects.Score.Update();
            
            return temp;
        }

    }
}
