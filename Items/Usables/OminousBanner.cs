using MinecraftAnimals.Raid;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria.Localization;

namespace MinecraftAnimals.Items.Usables
{
    public class OminousBanner : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("waving this around probably isn't the greatest idea");
        }

        public override void SetDefaults()
        {
            item.maxStack = 64;
            item.width = 20;
            item.height = 30;
            item.useTime = 40;
            item.useAnimation = 40;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.value = 2500;
            item.noMelee = true;
            item.consumable = true;
        }
        public override bool CanUseItem(Player player)
        {
            return true;
        }
        public override bool UseItem(Player player)
        {
            //float distanceToSpawn = Vector2.Distance(new Vector2(player.position.X, player.position.Y), new Vector2(player.SpawnX, player.SpawnY));
            player.AddBuff(BuffType<StatusEffects.Debuff.BadOmen>(), 4000);
            return true;
        }
    }
}