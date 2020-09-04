using Terraria;
using Terraria.ID;
using MinecraftAnimals.Tiles;
using MinecraftAnimals.Tiles.Trees;
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
			for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 0.25); k++)
			{
				int x = WorldGen.genRand.Next(2) == 0 ? WorldGen.genRand.Next(0, Main.maxTilesX / 5) : WorldGen.genRand.Next(Main.maxTilesX / 4 * 4, Main.maxTilesX);
				int y = WorldGen.genRand.Next(Main.maxTilesY - 160, Main.maxTilesY - 125);
				Tile tile = Framing.GetTileSafely(x, y);
				WorldGen.SquareTileFrame(x, y);
				if ((Main.tile[x, y - 1].active()) && !tile.active() && Main.tile[x, y - 1].type == TileType<WarpedNyliumtile>())
				{
					WorldGen.TileRunner(x, y, 1, 1, TileType<WarpedSapling>(), false, 0, 0, false, true);
				}
			}
			for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 0.25); k++)
			{
				int x = WorldGen.genRand.Next(2) == 0 ? WorldGen.genRand.Next(0, Main.maxTilesX / 5) : WorldGen.genRand.Next(Main.maxTilesX / 4 * 4, Main.maxTilesX);
				int y = WorldGen.genRand.Next(Main.maxTilesY - 160, Main.maxTilesY - 125);
				Tile tile = Framing.GetTileSafely(x, y);
				WorldGen.SquareTileFrame(x, y);
				if ((Main.tile[x, y - 1].active()) && !tile.active() && Main.tile[x, y - 1].type == TileType<WarpedNyliumtile>())
				{
					WorldGen.TileRunner(x, y, 1, 1, TileType<WarpedHerb>(), false, 0, 0, false, true);
				}
			}
			return base.UseItem(player);
        }
    }
}
