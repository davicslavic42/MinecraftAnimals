using Terraria;
using Terraria.ID;
using MinecraftAnimals.projectiles;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using System.Collections.Generic;

namespace MinecraftAnimals.Items.Weapons
{
	public class Crossbow : ModItem
	{
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
        }

		public override void SetDefaults()
		{
			// Alter any of these values as you see fit, but you should probably keep useStyle on 1, as well as the noUseGraphic and noMelee bools
			item.shootSpeed = 10f;
			item.damage = 25;
			item.knockBack = 5f;
			item.channel = true; //Channel so that you can held the weapon [Important]
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.useAnimation = 25;
			item.useTime = 25;
			item.width = 40;
			item.height = 20;
			item.maxStack = 1;
			item.rare = ItemRarityID.Pink;

			item.noUseGraphic = true;
			item.noMelee = true;
			item.autoReuse = true;
			item.UseSound = SoundID.Item1;
			item.value = Item.sellPrice(silver: 5);
			// Look at the javelin projectile for a lot of custom code
			// If you are in an editor like Visual Studio, you can hold CTRL and Click ExampleJavelinProjectile
			item.shoot = ProjectileType<projectiles.Passive.Bolt>();
		}
	}
}
