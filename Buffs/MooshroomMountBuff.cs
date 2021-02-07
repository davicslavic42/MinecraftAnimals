using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MinecraftAnimals.Buffs
{
    public class MooshroomMountBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Mooshroom");
            Description.SetDefault("You hate to see it");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.mount.SetMount(MountType<Mounts.MooshroomMount>(), player);
            player.buffTime[buffIndex] = 10;
        }
    }
}
