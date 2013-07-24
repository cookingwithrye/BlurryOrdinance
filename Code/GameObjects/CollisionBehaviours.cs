using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OSBO.GameObjects
{
    /// used to store information about how a GameObject behaves when it hits another one
    enum CollisionBehaviours
    {
        DestroyBoth,      //destroy both objects
        Bounce,       //object bounces off things
        None,         //object passes through other ones (no clipping)
        DestroyThis,  //only this object gets destroyed
        DestroyOther, //only other object gets destroyed
    }
}