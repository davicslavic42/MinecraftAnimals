using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Microsoft.Xna.Framework;
using System;

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
            Tooltip.SetDefault("A sturdy crossbow favored among the pillagers, turns wood arrows into 3d arrows with more knocback");//, turns arros into high velocity bolts
        }

        public override void SetDefaults()
        {
            // Alter any of these values as you see fit, but you should probably keep useStyle on 1, as well as the noUseGraphic and noMelee bools
            item.shootSpeed = 18f;
            item.damage = 38;
            item.knockBack = 5f;
            //Channel so that you can held the weapon [Important]  item.channel = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useAnimation = 100;
            item.useTime = 100;
            item.width = 40;
            item.height = 20;
            item.maxStack = 1;
            item.rare = ItemRarityID.Green;
            item.ranged = true;
            //item.useAmmo = AmmoID.Arrow;
            item.channel = true;
            item.shoot = ProjectileType<projectiles.Passive.PassiveArrow>();

            item.noMelee = true;
            item.UseSound = SoundID.Item1;
            item.value = Item.sellPrice(silver: 75);

            //item.shoot = ProjectileType<projectiles.Passive.PassiveArrow>();
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (type == ProjectileID.WoodenArrowFriendly) // or ProjectileID.WoodenArrowFriendly
            {
                type = ProjectileType<projectiles.Passive.PassiveArrow>(); // or ProjectileID.FireArrow;
            }
            return true; // return true to allow tmodloader to call Projectile.NewProjectile as normal
        }
    }
}
