﻿#region Using Statements

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

namespace OSBO.GameObjects.Weapons
{
    // base class for weapons in the game
    abstract class Weapon : GameObject
    {

        #region Constructor

        // create a weapon with the specified properties
        public Weapon()
        {
        }

        #endregion

    }
}
