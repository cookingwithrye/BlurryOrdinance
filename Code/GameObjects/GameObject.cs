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

namespace OSBO.GameObjects
{
    /// generic class for all game object textures and their physics
    class GameObject
    {

        #region Debug methods

        ///show debug information, with a specified indentation to make reading easier (use "" if you don't understand this)
        public void debug(String indent)
        {
            Console.WriteLine(indent + "GameObject Debug Data");
            Console.WriteLine(indent + "----------------------------");
            Console.WriteLine(indent + "sprite:" + imageName);
            Console.WriteLine(indent + "position: (" + position.X + "," + position.Y + ")");
            Console.WriteLine(indent + "center: (" + center.X + "," + center.Y + ")");
            Console.WriteLine(indent + "rotation: " + rotation.ToString());
            Console.WriteLine(indent + "velocity: (" + velocity.X + "," + velocity.Y + ")");
            Console.WriteLine(indent + "alive: " + alive.ToString());
            Console.WriteLine(indent + "expires: " + expires.ToString());
            Console.WriteLine(indent + "lifespan: " + lifespan.ToString());
        }

        #endregion

        #region Constants & Variables

        //image filename for this game object's texture
        public string imageName;
        
        //2D texture of this game object
        public Texture2D sprite;

        // the game object's texture center, map position, and physics speed, and a new velocity that is applied on the next update
        public Vector2 center, position, velocity, newVelocity;

        //the texture's rotation and scale
        public float rotation, scale;

        //this object's rotation momentum (a contant spinning)
        public float rotationConstant;

        //whether this game object has a limited lifespan until it dies
        public bool expires;

        //this game object's lifespan (in game seconds) - only applies if 'expires' is set to 'true'
        public float lifespan;

        //whether this game object is currently alive
        public bool alive;

        //whether this game object has crossed a border on the last movement update, used by the Map to apply border logic
        public bool alreadyCrossedBorder;

        //collision behaviour of the object
        public CollisionBehaviours collisionBehaviour;

        //array of pixels - used for collision detection purposes
        protected Color[,] colours;

        //the owner of this object
        public GameObject owner;

        //countdown timer until this object can hit it's owner
        public float hitOwnerCountdown;

        #endregion

        #region Constructors

        ///set the default constants and variables
        public GameObject()
        {
            alive = true; //object starts out alive
            alreadyCrossedBorder = false;
            expires = false; // by default an object will not expire
            rotation = 0;
            rotationConstant = 0; //by default the object is not rotating
            scale = 1.0f;
            imageName = ""; // no image has been loaded yet, will be handled by LoadContent
            collisionBehaviour = CollisionBehaviours.Bounce; //by default, GameObjects bounce off each other
            newVelocity = Vector2.Zero; //there has not yet been an update
        }

        ///create a GameObject that expires
        public GameObject(float lifespan) : this()
        {
            this.expires = true;
            this.lifespan = lifespan;
        }

        #endregion

        #region Load,Draw,Update methods

        /// load the specified texture into this GameObject
        public virtual void LoadContent(ContentManager contentManager, string imageName)
        {
            this.imageName = imageName;
            sprite = contentManager.Load<Texture2D>(imageName);
            center = new Vector2(sprite.Width / 2, sprite.Height / 2);
            colours = TextureTo2DArray(sprite);
        }

        /// draw this GameObject
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, position, null, Color.White, rotation, center, scale, SpriteEffects.None, 0f);
        }

        /// draw this GameObject relative to another one, and center it
        public virtual void DrawRelative(SpriteBatch spriteBatch, GameObject centeredObject, Vector2 centeredCoordinates)
        {
            spriteBatch.Draw(sprite, position - centeredObject.position + centeredCoordinates, null,
                Color.White, rotation, center, scale, SpriteEffects.None, 0f);
        }

        ///draw the game object as translated to coordinates X,Y and relative to centeredObject
        public virtual void DrawRelativeCube(SpriteBatch spriteBatch, GameObject centeredObject, Vector2 centeredCoordinates, float X, float Y) 
        {
            //right-top
            DrawRelative(spriteBatch, centeredObject, new Vector2(centeredCoordinates.X + X, centeredCoordinates.Y - Y));

            //right-middle
            DrawRelative(spriteBatch, centeredObject, new Vector2(centeredCoordinates.X + X, centeredCoordinates.Y));

            //right-bottom
            DrawRelative(spriteBatch, centeredObject, new Vector2(centeredCoordinates.X + X, centeredCoordinates.Y + Y));

            //left-top
            DrawRelative(spriteBatch, centeredObject, new Vector2(centeredCoordinates.X - X, centeredCoordinates.Y - Y));

            //left-middle
            DrawRelative(spriteBatch, centeredObject, new Vector2(centeredCoordinates.X - X, centeredCoordinates.Y));

            //left-bottom
            DrawRelative(spriteBatch, centeredObject, new Vector2(centeredCoordinates.X - X, centeredCoordinates.Y + Y));

            //bottom-middle
            DrawRelative(spriteBatch, centeredObject, new Vector2(centeredCoordinates.X, centeredCoordinates.Y + Y));

            //top-middle
            DrawRelative(spriteBatch, centeredObject, new Vector2(centeredCoordinates.X, centeredCoordinates.Y - Y));
        }

        /// update the position of this GameObject
        public virtual void Update(GameTime gameTime)
        {
            if (this.alive)
            {
                // if there has been an update in velocity, apply it before updating position
                if (newVelocity != Vector2.Zero) {
                    velocity = newVelocity;
                    newVelocity = Vector2.Zero;
                }

                // update position based on velocity
                position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

                // apply rotation constant
                rotation += rotationConstant;

                //check if the object expires, and if so adjust it's lifespan countdown
                if (this.expires)
                {
                    this.lifespan -= (float)gameTime.ElapsedGameTime.Milliseconds / 1000;

                    //see if the object has now expired
                    if (this.lifespan <= 0)
                        this.alive = false; //the object has died
                }

                //countdown for when this object can hit the owner of it
                if (hitOwnerCountdown > 0)
                    hitOwnerCountdown -= (float)gameTime.ElapsedGameTime.Milliseconds / 1000;
            }
        }


        #endregion

        /// <summary>
        /// Calculates the absolute distance from another game object in pixels
        /// </summary>
        /// <param name="other"></param>
        public virtual float DistanceFrom(GameObject other) 
        {
            float temp1 = this.position.X - other.position.X;
            float temp2 = this.position.Y - other.position.Y;
            return (float)Math.Sqrt(Math.Pow(temp1,2) + Math.Pow(temp2,2));
        }

        #region Destroy method

        /// destroys this GameObject and returns a list of GameObjects that take it's place
        public virtual List<GameObject> Destroy(ContentManager contentManager)
        {
            alive = false;

            return new List<GameObject>();
        }

        #endregion

        #region Fuzzy collision detection

        /// returns true if the current GameObject is colliding with the other one 
        //  by creating a circle around each object and see if they overlap
        public virtual bool CollisionWith(GameObject otherObject, bool precise)
        {
            //check if the object being check is the same as the current one - an object can't collide with itself
            if (this == otherObject)
                return false;

            //check if the collision is between an object and it's parent object
            if (otherObject.owner == this)
            {
                if (otherObject.hitOwnerCountdown > 0)
                    return false;
            }

            //check if the collision is between an object and it's parent object
            if (this.owner == otherObject)
            {
                if (this.hitOwnerCountdown > 0)
                    return false;
            }

            //create the circles
            Vector3 thisCircle = new Vector3(this.position.X, this.position.Y, (new Vector2(this.sprite.Width * this.scale / 2, this.sprite.Height * this.scale / 2)).Length());
            Vector3 otherCircle = new Vector3(otherObject.position.X, otherObject.position.Y, (new Vector2(otherObject.sprite.Width * otherObject.scale / 2, otherObject.sprite.Height * otherObject.scale / 2)).Length());

            //check if the distance between them is <= 0
            if ((Math.Abs(thisCircle.X - otherCircle.X) < (thisCircle.Z + otherCircle.Z)) &&
               (Math.Abs(thisCircle.Y - otherCircle.Y) < (thisCircle.Z + otherCircle.Z)))
            {
                if (!precise) // if the request didn't want pixel-erfect precision, then a collision has already happened
                    return true;
                
                //possible collision detected, now do a per-pixel comparison
                Vector2 collisionLocation;

                if (TexturesCollide(this, otherObject, out collisionLocation))
                {
                    // collision confirmed
                    return true;
                }
                else
                    return false;

            } else
                return false;
        }

        #endregion

        #region Per-pixel collision detection

        /// turns a texture into an array of pixels for collision detection
        //sample code found from http://www.riemers.net/eng/Tutorials/XNA/Csharp/Series2D/Coll_Detection_Overview.php
        protected Color[,] TextureTo2DArray(Texture2D texture)
        {
            Color[] colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(colors1D);

            Color[,] colors2D = new Color[texture.Width, texture.Height];
            for (int x = 0; x < texture.Width; x++)
                for (int y = 0; y < texture.Height; y++)
                    colors2D[x, y] = colors1D[x + y * texture.Width];

            return colors2D;
        }

        /// perform per-pixel collision detection on two textures rendered on the screen
        // this method is based on code found here: http://www.virtualrealm.com.au/blogs/xna-rotated-perpixel-collision/
        private bool TexturesCollide(GameObject A, GameObject B, out Vector2 collisionLocation)
        {
            //create our transformation matrices for the two textures
            Matrix matrixA 
                = Matrix.CreateTranslation(new Vector3(-A.center, 0.0f)) 
                * Matrix.CreateScale(A.scale) 
                * Matrix.CreateRotationZ(A.rotation) 
                * Matrix.CreateTranslation(new Vector3(A.position, 0.0f));

            Matrix matrixB
                = Matrix.CreateTranslation(new Vector3(-B.center, 0.0f))
                * Matrix.CreateScale(B.scale)
                * Matrix.CreateRotationZ(B.rotation)
                * Matrix.CreateTranslation(new Vector3(B.position, 0.0f));
            
            // create a translation matrix from A to B
            Matrix matrixAtoB = matrixA * Matrix.Invert(matrixB);
            int widthA = A.colours.GetLength(0);
            int heightA = A.colours.GetLength(1);
            int widthB = B.colours.GetLength(0);
            int heightB = B.colours.GetLength(1);

            // step through each transformed image and see if any pixels overlap
            for (int x1 = 0; x1 < widthA; x1++)
            {
                for (int y1 = 0; y1 < heightA; y1++)
                {
                    Vector2 pos1 = new Vector2(x1, y1);
                    Vector2 pos2 = Vector2.Transform(pos1, matrixAtoB);

                    int x2 = (int)pos2.X;
                    int y2 = (int)pos2.Y;
                    if ((x2 >= 0) && (x2 < widthB))
                    {
                        if ((y2 >= 0) && (y2 < heightB))
                        {
                            if (A.colours[x1, y1].A > 0)
                            {
                                if (B.colours[x2, y2].A > 0)
                                {
                                    // collision found
                                    collisionLocation = Vector2.Transform(pos1, matrixA);
                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            // no collision detected
            collisionLocation = new Vector2(-1, -1);
            return false;
        }


        #endregion

        #region Apply collision behaviour

        /// performs collision action
        public virtual void Collide(GameObject otherObject)
        {
            //resolve the behaviour that should happen between a collision of these two objects

            if (this.collisionBehaviour == CollisionBehaviours.None ||
                otherObject.collisionBehaviour == CollisionBehaviours.None)
            {
                // one or both of the objects has no clipping, they pass through each other
                return;
            }

            if (this.collisionBehaviour == CollisionBehaviours.DestroyBoth || 
                otherObject.collisionBehaviour == CollisionBehaviours.DestroyBoth) 
            {
                // one or both of the objects destroys them on contact
                this.alive = false;
                otherObject.alive = false;
                return;
            }

            if (this.collisionBehaviour == CollisionBehaviours.Bounce &&
                otherObject.collisionBehaviour == CollisionBehaviours.Bounce)
            {
                if (this.scale < otherObject.scale)
                {
                    this.newVelocity += 1.1f * otherObject.velocity;
                }
                else
                {
                    this.newVelocity += 0.9f * otherObject.velocity;
                }

                if (this is Ship)
                {
                    this.newVelocity *= 1.2f;
                }

                //sanity check on object speeds
                if (this.newVelocity.Length() > 600)
                {
                    this.newVelocity = this.velocity;
                    this.position += 0.01f * this.velocity;
                }

                
                // move the objects apart until they aren't colliding
                int limit = 100;
                while (this.CollisionWith(otherObject,true))
                {
                    this.position += -0.01f * this.velocity;
                    otherObject.position += -0.01f * otherObject.velocity;

                    // temporarily make the object able to move freely
                    this.hitOwnerCountdown = 0.1f;
                    this.owner = otherObject;

                    // we've tried 100 times, these objects aren't going to avoid colliding - remove one
                    if ((limit-- <= 0) && !(this is Ship))
                    {
                        this.alive = false;
                        Console.WriteLine("Force killing");
                        break;
                    }
                }

                return;
            }

            if (this.collisionBehaviour == CollisionBehaviours.DestroyThis)
            {
                // destroy only one object
                this.alive = false;
                return;
            }

            if (otherObject.collisionBehaviour == CollisionBehaviours.DestroyThis)
            {
                // destroy only one object
                otherObject.alive = false;
                return;
            }

            if (this.collisionBehaviour == CollisionBehaviours.DestroyOther)
            {
                // destroy only one object
                otherObject.alive = false;
                return;
            }

            if (otherObject.collisionBehaviour == CollisionBehaviours.DestroyOther)
            {
                // destroy only one object
                this.alive = false;
                return;
            }
        }

        #endregion

    }
}
