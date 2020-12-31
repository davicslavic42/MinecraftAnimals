﻿using MinecraftAnimals.Tiles;
using MinecraftAnimals.Tiles.Trees;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.World.Generation;
using static Terraria.ModLoader.ModContent;

namespace MinecraftAnimals.Worldgen.Plantgen
{
	public class Treebase : ModWorld
	{
		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
		{
			// Because world generation is like layering several images ontop of each other, we need to do some steps between the original world generation steps.

			// The first step is an Ore. Most vanilla ores are generated in a step called "Shinies", so for maximum compatibility, we will also do this.
			// First, we find out which step "Shinies" is.
			int ShiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Micro Biomes"));
			if (ShiniesIndex != -1)
			{
				// Next, we insert our step directly after the original "Shinies" step. 
				// ExampleModOres is a method seen below.
				tasks.Insert(ShiniesIndex + 1, new PassLegacy("Tree bases", Treebases));
			}
		}
		private static int FindType(int x, int y, int maxDepth = -1, params int[] types)
		{
			if (maxDepth == -1) maxDepth = (int)(WorldGen.worldSurface); //Set default
			while (true)
			{
				if (y >= maxDepth)
					break;
				if (Main.tile[x, y].active() && types.Any(i => i == Main.tile[x, y].type))
					return y; //Returns first valid tile under intitial Y pos, -1 if max depth is reached
				y++;
			}
			return -1; //fallout case
		}
		private void Treebases(GenerationProgress progress)
		{
			progress.Message = "Tree bases";
			for (int k = 0; k < (int)((Main.maxTilesX * (int)WorldGen.worldSurface) * 0.75); k++)
			{
				// The inside of this for loop corresponds to one single splotch of our Ore.
				// First, we randomly choose any coordinate in the world by choosing a random x and y value.
				int x = WorldGen.genRand.Next(2) == 0 ? WorldGen.genRand.Next(60, Main.maxTilesX / 4) : WorldGen.genRand.Next(Main.maxTilesX / 4 * 3, Main.maxTilesX - 60);
				int y = (int)(WorldGen.worldSurface * 0.35);
				y = FindType(x, y, -1, TileID.Grass);
				if (y > 1)
				{
					Tile tile = Framing.GetTileSafely(x, y);
					WorldGen.SquareTileFrame(x, y);
					if (tile.active() && tile.type == TileID.Grass)
					{
						WorldGen.TileRunner(x, y, WorldGen.genRand.Next(2, 3), WorldGen.genRand.Next(1, 2), TileType<Dirttile>(), false, 0, 0, false, true);
					}
					Main.NewText(k);
				}
			}
		}
	}
}