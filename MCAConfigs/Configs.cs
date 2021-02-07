using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;

namespace MinecraftAnimals.MCAConfigs
{
    public class Configs : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [DefaultValue(true)]
        [Label("Mob Griefing")]
        public bool MobGriefing { get; set; }
    }
}