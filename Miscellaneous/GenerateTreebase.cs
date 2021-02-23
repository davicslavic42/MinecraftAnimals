using MinecraftAnimals.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

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
            for (int k = 0; k < (int)(Main.maxTilesX * (WorldGen.worldSurface * 0.20) * WorldGen.worldSurface * 3); k++)
            {
                // The inside of this for loop corresponds to one single splotch of our Ore.
                // First, we randomly choose any coordinate in the world by choosing a random x and y value.
                int x = WorldGen.genRand.Next(2) == 0 ? WorldGen.genRand.Next(60, Main.maxTilesX / 4) : WorldGen.genRand.Next(Main.maxTilesX / 4 * 2, Main.maxTilesX - 60);
                int y = (int)(WorldGen.worldSurface * 0.20);
                // Then, we call WorldGen.TileRunner with random "strength" and random "steps", as well as the Tile we wish to place. Feel free to experiment with strength and step to see the shape they generate.
                // Alternately, we could check the tile already present in the coordinate we are interested. Wrapping WorldGen.TileRunner in the following condition would make the ore only generate in Snow.
                Tile tile = Framing.GetTileSafely(x, y);
                if (y != TileID.Grass)
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