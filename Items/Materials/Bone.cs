﻿using MinecraftAnimals.Raid;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinecraftAnimals.Items.Materials
{
    public class Bone : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Weapons?");
        }

        public override void SetDefaults()
        {
            item.maxStack = 64;
            item.width = 20;
            item.height = 30;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.value = 0;
            item.noMelee = true;
        }
        public override bool CanUseItem(Player player)
        {
            return false;
        }
        public override bool UseItem(Player player)
        {
            return false;
        }
    }
}