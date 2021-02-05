using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;
using Terraria.Localization;
using static Terraria.ModLoader.ModContent;
using Microsoft.Xna.Framework;


namespace MinecraftAnimals
{
    class SpawnEdits : GlobalNPC
    {
        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)//thanks oli
        {
            if(MCAWorld.RaidEvent == true)
            {
                pool.Clear();
                pool.Add(MCAWorld.Raiders.Length, 20f);
                if(MCAWorld.RaiderCounter >= MCAWorld.progressPerWave + 1)
                {
                    pool.Clear();
                }
            }
        }
    }
}
