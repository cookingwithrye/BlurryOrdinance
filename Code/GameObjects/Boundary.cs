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
        Transport, //transport the object to the other side of the boundary (like the original asteroids)
        Destroy,   //destroy the object, ships are excluded
        Bounce     //the object will bounce off the boundary edge (like a billiards table)
    }

    /// contains information about a rectangular boundary for GameObjects
    struct Boundary
    {
        public int Left, Right, Top, Bottom; //the edges for our boundary
        public BoundaryTypes BoundaryType; // the type of boundary
        public bool visible; //whether the boundary is visible or not on the screen
    }
}
