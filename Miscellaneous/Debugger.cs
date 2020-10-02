using Terraria;
using Terraria.ID;
using MinecraftAnimals.Tiles;
using MinecraftAnimals.Tiles.Trees;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using System.Linq;

namespace MinecraftAnimals.Miscellaneous
{
	public class Debugger : ModItem
	{
		public override void SetDefaults()
		{
			// Alter any of these values as you see fit, but you should probably keep useStyle on 1, as well as the noUseGraphic and noMelee bools
			item.knockBack = 5f;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.useAnimation = 1;
			item.useTime = 5;
			item.width = 30;
			item.height = 30;
			item.maxStack = 1;
			item.rare = ItemRarityID.Pink;

			item.consumable = false;
			item.autoReuse = false;

			item.UseSound = SoundID.Item1;
			item.value = Item.sellPrice(silver: 5);
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
			return 1; //fallout case
		}
		public override bool UseItem(Player player)
		{
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
				return base.UseItem(player);
        }
    }
}
