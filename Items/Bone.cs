using MinecraftAnimals.Raid;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinecraftAnimals.Items
{
    public class Bone : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Weapons? Remember to remove test properties before update");
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
        public override bool CanUseItem(Player player)
        {
            return true;
        }
        public override bool UseItem(Player player)
        {
            Main.NewText(RaidWorld.RaiderCounter + "enemy counter");
            Main.NewText(RaidWorld.RaidKillCount + " kills");
            Main.NewText(RaidWorld.progressPerWave + "progress per wave");
            Main.NewText(RaidWorld.RaidWaves + "current wave");
            return true;

        }
    }
}