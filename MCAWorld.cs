﻿using MinecraftAnimals.Tiles;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.World.Generation;
using static Terraria.ModLoader.ModContent;

namespace MinecraftAnimals
{
	public class MCAWorld : ModWorld
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

		private void Treebases(GenerationProgress progress)
		{
			// progress.Message is the message shown to the user while the following code is running. Try to make your message clear. You can be a little bit clever, but make sure it is descriptive enough for troubleshooting purposes. 
			progress.Message = "Tree bases";

			// "6E-05" is "scientific notation". It simply means 0.00006 but in some ways is easier to read.
			
			for (int k = 0; k < (int)(Main.maxTilesX * (WorldGen.worldSurface * 0.20) * WorldGen.worldSurface * 1E-03); k++)
			{
				// The inside of this for loop corresponds to one single splotch of our Ore.
				// First, we randomly choose any coordinate in the world by choosing a random x and y value.
				int x = WorldGen.genRand.Next(2) == 0 ? WorldGen.genRand.Next(60, Main.maxTilesX / 4) : WorldGen.genRand.Next(Main.maxTilesX / 4 * 3, Main.maxTilesX - 60);
				int y = (int)(WorldGen.worldSurface * 0.20);
				// Then, we call WorldGen.TileRunner with random "strength" and random "steps", as well as the Tile we wish to place. Feel free to experiment with strength and step to see the shape they generate.
				// Alternately, we could check the tile already present in the coordinate we are interested. Wrapping WorldGen.TileRunner in the following condition would make the ore only generate in Snow.
				Tile tile = Framing.GetTileSafely(x, y);
				if ( y != TileID.Grass)
				{
					y++;
				}
                else
                {
					WorldGen.TileRunner(x, y, WorldGen.genRand.Next(2, 3), WorldGen.genRand.Next(1, 2), TileType<Dirttile>());
				}
				if (y == WorldGen.worldSurface)
                {
					break;
                }
			}
		}
	}
}