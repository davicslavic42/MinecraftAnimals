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
        public override void EditSpawnPool(System.Collections.Generic.IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            pool.Clear();
        }
    }
}
