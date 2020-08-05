using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MinecraftAnimals.Items.Herbs
{
	public class HugeFungusSeed : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Fungi can also be grown into much larger Huge fungi as well");
			DisplayName.SetDefault("Huge Fungus Seed");
		}

		public override void SetDefaults()
		{
			item.maxStack = 99;
			item.width = 12;
			item.height = 12;
			item.maxStack = 999;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.consumable = true;
			item.createTile = TileType<Tiles.Trees.WarpedSapling>();
		}
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemType<WarpedFungus>(), 1);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}