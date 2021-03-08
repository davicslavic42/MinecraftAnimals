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
            Tooltip.SetDefault("A sturdy crossbow favored among the pillagers, turns wooden arrows into charged bolts");//, turns arros into high velocity bolts
        }

        public override void SetDefaults()
        {
            // Alter any of these values as you see fit, but you should probably keep useStyle on 1, as well as the noUseGraphic and noMelee bools
            item.shootSpeed = 18f;
            item.damage = 25;
            item.knockBack = 6f;
            //Channel so that you can held the weapon [Important]  item.channel = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useAnimation = 70;
            item.useTime = 70;
            item.width = 40;
            item.height = 20;
            item.maxStack = 1;
            item.rare = ItemRarityID.Green;
            item.ranged = true;
            item.channel = true;
            item.shoot = ProjectileType<projectiles.Passive.Bolt>();
            item.useAmmo =  AmmoID.Arrow;

            item.noMelee = true;
            item.UseSound = SoundID.Item1;
            item.value = Item.sellPrice(silver: 75);

            //item.shoot = ProjectileType<projectiles.Passive.PassiveArrow>();
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (type == ProjectileID.WoodenArrowFriendly) // or ProjectileID.WoodenArrowFriendly AmmoID.Arrow
            {
                type = ProjectileType<projectiles.Passive.Bolt>(); // or ProjectileID.FireArrow;
            }
            return true; // return true to allow tmodloader to call Projectile.NewProjectile as normal
        }
        public override void HoldItem(Player player)
        {
            base.HoldItem(player);
        }
        public override bool UseItem(Player player)
        {
            return base.UseItem(player);
        }
    }
}
