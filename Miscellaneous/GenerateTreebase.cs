using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.World.Generation;
using static Terraria.ModLoader.ModContent;
using static MinecraftAnimals.MCAWorld;
using  MinecraftAnimals.Tiles;

namespace MinecraftAnimals.Miscellaneous
{

    public class GenerateTreebase : ModCommand
    {
        public override CommandType Type
        {
            get { return CommandType.Chat; }
        }

        public override string Command
        {
            get { return "Generate Treebase"; }
        }

        public override string Description
        {
            get { return "generate base tiles for trees"; }
        }

        public override void Action(CommandCaller caller, string input, string[] args)
        {
			for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-05); k++)
			{
				// The inside of this for loop corresponds to one single splotch of our Ore.
				// First, we randomly choose any coordinate in the world by choosing a random x and y value.
				int x = WorldGen.genRand.Next(0, Main.maxTilesX);
				int y = WorldGen.genRand.Next((int)WorldGen.worldSurfaceLow , (int)WorldGen.worldSurface); // WorldGen.worldSurfaceLow is actually the highest surface tile. In practice you might want to use WorldGen.rockLayer or other WorldGen values.
				// Then, we call WorldGen.TileRunner with random "strength" and random "steps", as well as the Tile we wish to place. Feel free to experiment with strength and step to see the shape they generate.
				// Alternately, we could check the tile already present in the coordinate we are interested. Wrapping WorldGen.TileRunner in the following condition would make the ore only generate in Snow.
				Tile tile = Framing.GetTileSafely(x, y);
				if (tile.active() && tile.type == TileID.Dirt)
				{
				 	WorldGen.TileRunner(x, y, WorldGen.genRand.Next(2, 3), WorldGen.genRand.Next(1, 2), TileType<Dirttile>());
				}
			}
		}
	}
}