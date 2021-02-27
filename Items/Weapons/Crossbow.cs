using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MinecraftAnimals.Items.Weapons
{
    public class Crossbow : ModItem
    {
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
        }
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("A sturdy crossbow favored among the pillagers");
        }

        public override void SetDefaults()
        {
            // Alter any of these values as you see fit, but you should probably keep useStyle on 1, as well as the noUseGraphic and noMelee bools
            item.shootSpeed = 12f;
            item.damage = 25;
            item.knockBack = 5f;
            //Channel so that you can held the weapon [Important]  item.channel = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useAnimation = 65;
            item.useTime = 65;
            item.width = 40;
            item.height = 20;
            item.maxStack = 1;
            item.rare = ItemRarityID.Pink;
            item.ranged = true;
            item.shoot = ProjectileType<projectiles.Passive.PassiveArrow>();
            item.useAmmo = AmmoID.Arrow;
            //item.channel = true;

            item.noMelee = true;
            item.autoReuse = false;
            item.UseSound = SoundID.Item1;
            item.value = Item.sellPrice(silver: 75);
            // Look at the javelin projectile for a lot of custom code
            // If you are in an editor like Visual Studio, you can hold CTRL and Click ExampleJavelinProjectile
            //item.shoot = ProjectileType<projectiles.Passive.PassiveArrow>();
        }
    }
}
