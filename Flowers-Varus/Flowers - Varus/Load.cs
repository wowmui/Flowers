﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;


namespace Flowers___Varus
{
    class Load
    {
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += lost.Game_OnGameLoad;
        }
    }
}
