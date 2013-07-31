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
    // an explosion
    class Explosion : GameObject
    {

        public Explosion(Texture2D sprite, Vector2 position, float scale, ContentManager contentManager, SoundBank soundBank)
        {
            //texture for the explosion
            this.sprite = sprite;

            this.position = position;
            this.center = new Vector2(this.sprite.Width / 2, this.sprite.Height / 2);

            this.lifespan = 1.0f;
            this.expires = true;

            this.scale = scale/2;
            colours = TextureTo2DArray(sprite);
            this.collisionBehaviour = CollisionBehaviours.None;
        }
    }
}
