using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.World.Generation;

namespace MinecraftAnimals.Worldgen
{
    public class Treegen : ModWorld
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            int genIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Micro Biomes"));
            int x = Main.rand.Next(0, Main.maxTilesX);
            int y = (int)Main.worldSurface;
            tasks.Insert(genIndex + 1, new PassLegacy("Tree Bases", delegate (GenerationProgress progress)
            {
                if (Main.tile[x, y].type <= -1)
                {
                    y--;
                }
                else
                {
                    for (int i = 0; i < 100; i++)
                    {
                        WorldGen.TileRunner(x, y, 20, Main.rand.Next(5, 8), mod.TileType("Dirttile"), true, 0f, 0f, true, true);
                    }
                }
            }));
        }
    }
}