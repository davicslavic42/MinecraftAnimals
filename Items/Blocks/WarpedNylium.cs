using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MinecraftAnimals.Items.Blocks
{
	public class WarpedNylium : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("WarpedNylium");
			Tooltip.SetDefault("Netherrack coated in a moss like substance suitable for Warped plant life");
		}

		public override void SetDefaults()
		{
			item.width = 16;
			item.height = 16;
			item.maxStack = 999;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.consumable = true;
			item.createTile = TileType<Tiles.WarpedNyliumtile>();
		}
	}
}
