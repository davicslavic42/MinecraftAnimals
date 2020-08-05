using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using MinecraftAnimals.Mounts;
using Microsoft.Xna.Framework;

namespace MinecraftAnimals.Items
{
	public class Bone : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Weapons?");
		}

		public override void SetDefaults()
		{
			item.maxStack = 99;
			item.width = 20;
			item.height = 30;
			item.useTime = 20;
			item.useAnimation = 20;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.value = 0;
			item.noMelee = true;
		}
	}
}