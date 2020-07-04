using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using MinecraftAnimals.Mounts;
using Microsoft.Xna.Framework;

namespace MinecraftAnimals.Items
{
	public class ShadeShroom : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Summons a rideable mooshroom");
		}

		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 30;
			item.useTime = 20;
			item.useAnimation = 20;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.value = 30000;
			item.rare = ItemRarityID.Green;
			item.UseSound = SoundID.Item79;
			item.noMelee = true;
			item.mountType = MountType<MooshroomMount>();
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Mushroom, 10);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}