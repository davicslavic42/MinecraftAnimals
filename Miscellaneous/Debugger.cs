using Terraria;
using Terraria.ID;
using MinecraftAnimals.Tiles;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

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
			item.useTime = 1;
			item.width = 30;
			item.height = 30;
			item.maxStack = 1;
			item.rare = ItemRarityID.Pink;

			item.consumable = false;
			item.autoReuse = false;

			item.UseSound = SoundID.Item1;
			item.value = Item.sellPrice(silver: 5);
		}
		public override bool UseItem(Player player)
		{

			for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-03); k++)
			{
				// The inside of this for loop corresponds to one single splotch of our Ore.
				// First, we randomly choose any coordinate in the world by choosing a random x and y value.
				int x = WorldGen.genRand.Next(2) == 0 ? WorldGen.genRand.Next(60, Main.maxTilesX / 5) : WorldGen.genRand.Next(Main.maxTilesX / 4 * 3, Main.maxTilesX - 60);
				int y = WorldGen.genRand.Next((int)WorldGen.worldSurfaceLow, (int)WorldGen.worldSurface);
				Tile tile = Framing.GetTileSafely(x, y);
				WorldGen.SquareTileFrame(x, y);
				if (tile.active() && tile.type == TileID.Grass)
				{
					WorldGen.TileRunner(x, y, WorldGen.genRand.Next(2, 3), WorldGen.genRand.Next(1, 2), TileType<Dirttile>());
				}
			}
			return base.UseItem(player);
        }
    }
}
