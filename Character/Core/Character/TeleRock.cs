﻿using System.Collections.Generic;

namespace Character.Core.Character
{
    public class TeleRock
    {
        public List<int> locations = new List<int>();
        public List<int> vipLocations = new List<int>();

        public void AddLocation(int mapId)
        {
            locations.Add(mapId);
        }

        public void AddVipLocation(int mapId)
        {
            vipLocations.Add(mapId);
        }
    }
}