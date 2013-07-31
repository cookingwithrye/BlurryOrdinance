using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OSBO.GameObjects
{
    /// predefined types of boundaries 
    // - ie how the edges will interact with any GameObjects that touch them
    enum BoundaryTypes 
    {
        /// <summary>
        /// transport the object to the other side of the boundary (like the original asteroids)
        /// </summary>
        Transport,
        
        /// <summary>
        /// the object will bounce off the boundary edge (like a billiards table)
        /// </summary>
        Destroy,   
        
        /// <summary>
        /// the object will bounce off the boundary edge (like a billiards table)
        /// </summary>
        Bounce     
    }

    /// <summary>
    /// Contains information about a rectangular boundary for GameObjects
    /// </summary>
    struct Boundary
    {
        public int Left, Right, Top, Bottom; //the edges for our boundary
        
        /// <summary>
        /// The type of boundary
        /// </summary>
        public BoundaryTypes BoundaryType;

        /// <summary>
        /// Is this boundary rendered visually to the user
        /// </summary>
        public bool visible; //whether the boundary is visible or not on the screen
    }
}
