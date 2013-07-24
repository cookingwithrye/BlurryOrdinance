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
    // defines a map that will be the background texture and boundary management for all other GameObjects
    class Map : GameObject
    {

        public Boundary mapBoundary; // store the maps boundary edge information
        private Texture2D boundaryBorder; //used to draw the boundary edge

        #region Debug methods

        ///show debug information, with a specified indentation to make reading easier (use "" if you don't understand this)
        new public void debug(String indent)
        {
            if (indent == null) indent = ""; // make the debug output pretty

            Console.WriteLine(indent+"Map Debug Data");
            Console.WriteLine(indent+"---------------------");
            Console.WriteLine(indent + "Map Boundary");
            Console.WriteLine(indent + " Left:"+mapBoundary.Left);
            Console.WriteLine(indent + " Right:" + mapBoundary.Right);
            Console.WriteLine(indent + " Top:" + mapBoundary.Top);
            Console.WriteLine(indent + " Bottom:" + mapBoundary.Bottom);
            Console.WriteLine(indent + " BoundaryType:" + mapBoundary.BoundaryType);
            Console.WriteLine(indent + " Visible:" + mapBoundary.visible);
            base.debug(indent + "  "); // call the base class debug
        }

        #endregion

        #region Constructors

        public Map(ContentManager theContentManager, String imageName)
        {
            // load the map texture
            this.imageName = imageName;
            sprite = theContentManager.Load<Texture2D>(imageName);
            center = new Vector2(sprite.Width / 2, sprite.Height / 2);

            // load a texture that is used to draw the border;
            boundaryBorder = theContentManager.Load<Texture2D>("Textures//boundaryBorder");
            
            this.scale = 1.0f; // the map texture should be scale at 100% (1.0f), although this won't affect gameplay

            // the map doesn't move as a physics object
            this.velocity = Vector2.Zero;

            // the map defaults to top-left alignment in the window
            this.position = new Vector2(sprite.Width / 2, sprite.Height / 2); 
            this.center = new Vector2(sprite.Width / 2, sprite.Height / 2);

            // set the default boundary to be the size of the map texture
            mapBoundary.Bottom = this.sprite.Height;
            mapBoundary.Right = this.sprite.Width;
            mapBoundary.Top = 0;
            mapBoundary.Left = 0;
            
            // set the default behaviour of the boundary
            mapBoundary.BoundaryType = BoundaryTypes.Transport;

            //by default boundaries are not visible
            mapBoundary.visible = false;
        }

        #endregion
        
        #region Draw methods

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            //draw the map boundary if it's visible
            if (mapBoundary.visible)
            {
                //draw the border using line segment projections of a 1x1 pixel texture, because XNA doesn't appear to be able to draw simple shapes

                Rectangle leftSide = new Rectangle(mapBoundary.Left - 4, mapBoundary.Top, 4, mapBoundary.Bottom - mapBoundary.Top);
                spriteBatch.Draw(boundaryBorder, leftSide, Color.White);

                Rectangle rightSide = new Rectangle(mapBoundary.Right, mapBoundary.Top, 4, mapBoundary.Bottom - mapBoundary.Top);
                spriteBatch.Draw(boundaryBorder, rightSide, Color.White);

                Rectangle topSide = new Rectangle(mapBoundary.Left - 4, mapBoundary.Top - 4, mapBoundary.Right - mapBoundary.Left + 8, 4);
                spriteBatch.Draw(boundaryBorder, topSide, Color.White);

                Rectangle bottomSide = new Rectangle(mapBoundary.Left - 4, mapBoundary.Bottom, mapBoundary.Right - mapBoundary.Left + 8, 4);
                spriteBatch.Draw(boundaryBorder, bottomSide, Color.White);
            }
        }

        public override void DrawRelative(SpriteBatch spriteBatch, GameObject centeredObject, Vector2 centeredCoordinates)
        {
            base.DrawRelative(spriteBatch, centeredObject, centeredCoordinates);

            //draw the map boundary if it's visible
            if (mapBoundary.visible)
            {
                //adjust the position of the edges relative to the centeredObject
                int dX = Convert.ToInt32(-centeredObject.position.X + centeredCoordinates.X);
                int dY = Convert.ToInt32(-centeredObject.position.Y + centeredCoordinates.Y);

                Rectangle leftSide = new Rectangle(mapBoundary.Left - 4 + dX, mapBoundary.Top + dY, 4, mapBoundary.Bottom - mapBoundary.Top);
                spriteBatch.Draw(boundaryBorder, leftSide, Color.White);

                Rectangle rightSide = new Rectangle(mapBoundary.Right + dX, mapBoundary.Top + dY, 4, mapBoundary.Bottom - mapBoundary.Top);
                spriteBatch.Draw(boundaryBorder, rightSide, Color.White);

                Rectangle topSide = new Rectangle(mapBoundary.Left - 4 + dX, mapBoundary.Top - 4 + dY, mapBoundary.Right - mapBoundary.Left + 8, 4);
                spriteBatch.Draw(boundaryBorder, topSide, Color.White);

                Rectangle bottomSide = new Rectangle(mapBoundary.Left - 4 + dX, mapBoundary.Bottom + dY, mapBoundary.Right - mapBoundary.Left + 8, 4);
                spriteBatch.Draw(boundaryBorder, bottomSide, Color.White);
            }
        }

        public override void DrawRelativeCube(SpriteBatch spriteBatch, GameObject centeredObject, Vector2 centeredCoordinates, float X, float Y)
        {
            // if the map boundary is visible, only draw it for the center map drawing and not the edge ones
            if (!mapBoundary.visible)
                base.DrawRelativeCube(spriteBatch, centeredObject, centeredCoordinates, X, Y);
            else
            {
                mapBoundary.visible = false;
                base.DrawRelativeCube(spriteBatch, centeredObject, centeredCoordinates, X, Y);
                mapBoundary.visible = true;
            }
        }

        #endregion

        #region ApplyBoundary - takes a GameObject and enforces the boundary rules on it

        /// Takes in a GameObject and checks to see that it is within the map boundary. 
        // If it's not, the position and velocity information for the GameObject is adjusted by reference
        public void ApplyBoundary(GameObject theObject)
        {
            // don't aply the border if this object has already crossed one
            if (mapBoundary.BoundaryType == BoundaryTypes.Transport && theObject.alreadyCrossedBorder)
            {
                theObject.alreadyCrossedBorder = false;
                return;
            }

            #region left boundary

            // check the left boundary
            if (theObject.position.X <= mapBoundary.Left)
            {
                switch (mapBoundary.BoundaryType)
                {
                    case BoundaryTypes.Bounce:
                        {
                            // reverse the direction of the object
                            theObject.velocity = new Vector2(-theObject.velocity.X, theObject.velocity.Y);

                            // ships aren't rotated when hitting a boundary, but all other GameObject are
                            if (!(theObject is Ship))
                                theObject.rotation *= -1;

                            break;
                        }
                    case BoundaryTypes.Transport:
                        {
                            //this is used to avoid a loop of an object going back and forth between boundary edges continuously
                            theObject.alreadyCrossedBorder = true;

                            // shift the object to the left side of the boundary
                            theObject.position.X = (mapBoundary.Right - mapBoundary.Left);
                            return;

                            break;
                        }
                    case BoundaryTypes.Destroy:
                        {
                            //destroy the object
                            theObject.alive = false;
                            break;
                        }
                }
            }

            #endregion

            #region right boundary

            // check the right boundary
            if (theObject.position.X >= mapBoundary.Right)
            {
                switch (mapBoundary.BoundaryType)
                {
                    case BoundaryTypes.Bounce:
                        {
                            // reverse the direction of the object
                            theObject.velocity = new Vector2(-theObject.velocity.X, theObject.velocity.Y);

                            // ships aren't rotated when hitting a boundary, but all other GameObject are
                            if (!(theObject is Ship))
                                theObject.rotation *= -1;

                            break;
                        }
                    case BoundaryTypes.Transport:
                        {
                            //this is used to avoid a loop of an object going back and forth between boundary edges continuously
                            theObject.alreadyCrossedBorder = true;

                            // shift the object to the left side of the boundary
                            theObject.position.X = mapBoundary.Left;
                            return;

                            break;
                        }
                    case BoundaryTypes.Destroy:
                        {
                            //destroy the object
                            theObject.alive = false;
                            break;
                        }
                }
            }

            #endregion

            #region bottom boundary

            // check the top boundary
            if ((theObject.position.Y > mapBoundary.Bottom))
            {
                //this is used to avoid a loop of an object going back and forth between boundary edges continuously
                theObject.alreadyCrossedBorder = true;
                switch (mapBoundary.BoundaryType)
                {
                    case BoundaryTypes.Bounce:
                        {
                            // reverse the direction of the object
                            theObject.velocity = new Vector2(theObject.velocity.X, -theObject.velocity.Y);

                            // ships aren't rotated when hitting a boundary, but all other GameObject are
                            if (!(theObject is Ship))
                                theObject.rotation *= -1;

                            break;
                        }
                    case BoundaryTypes.Transport:
                        {
                            //this is used to avoid a loop of an object going back and forth between boundary edges continuously
                            theObject.alreadyCrossedBorder = true;

                            // shift the object to the top side of the boundary
                            theObject.position.Y = mapBoundary.Top;
                            return;

                            break;
                        }
                    case BoundaryTypes.Destroy:
                        {
                            //destroy the object
                            theObject.alive = false;
                            break;
                        }
                }
            }

            #endregion

            #region top boundary

            // check the top boundary
            if (theObject.position.Y <= mapBoundary.Top)
            {
                //this is used to avoid a loop of an object going back and forth between boundary edges continuously
                theObject.alreadyCrossedBorder = true;
                switch (mapBoundary.BoundaryType)
                {
                    case BoundaryTypes.Bounce:
                        {
                            // reverse the direction of the object
                            theObject.velocity = new Vector2(theObject.velocity.X, -theObject.velocity.Y);

                            // ships aren't rotated when hitting a boundary, but all other GameObject are
                            if (!(theObject is Ship))
                                theObject.rotation *= -1;

                            break;
                        }
                    case BoundaryTypes.Transport:
                        {
                            //this is used to avoid a loop of an object going back and forth between boundary edges continuously
                            theObject.alreadyCrossedBorder = true;

                            // shift the object to the top side of the boundary
                            theObject.position.Y = (mapBoundary.Bottom - mapBoundary.Top);
                            return;

                            break;
                        }
                    case BoundaryTypes.Destroy:
                        {
                            //destroy the object
                            theObject.alive = false;
                            break;
                        }
                }
            }

            #endregion

        }

        #endregion

        /// returns the location of the center of the map (based on the boundary, not the map sprite)
        public Vector2 getCenter()
        {
            return center + new Vector2(mapBoundary.Left, mapBoundary.Top);
        }
    }
}
