using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OSBO.GameObjects
{
    /// used to store information about how a GameObject behaves when it hits another one
    enum CollisionBehaviours
    {
        /// <summary>
        /// Destroy both objects
        /// </summary>
        DestroyBoth,      
        
        /// <summary>
        /// Objects bounce off of things
        /// </summary>
        Bounce,       

        /// <summary>
        /// Object passes through other ones (basically no clipping)
        /// </summary>
        None,         

        /// <summary>
        /// Only this object gets destroyed
        /// </summary>
        DestroyThis,  

        /// <summary>
        /// Only the other object gets destroyed
        /// </summary>
        DestroyOther, //only other object gets destroyed
    }
}