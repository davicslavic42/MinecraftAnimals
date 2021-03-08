using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using MinecraftAnimals.Raid.Illagers;
using System.IO;
using Terraria.Localization;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;
using MinecraftAnimals.Raid;
using Terraria.World.Generation;
using Terraria.ObjectData;

namespace MinecraftAnimals.Tiles.TileEffects
{
    public class GlobalTileEffects : GlobalTile
    {
        // thanks chem for the help
        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            int textCount = 0;
            if (Main.netMode != NetmodeID.MultiplayerClient && !WorldGen.noTileActions && !WorldGen.gen)
            {
                if (type == TileID.Stone && Main.rand.NextBool(1500)) NPC.NewNPC(i * 16, j * 16, NPCType<Animals.Silverfish>(), 0);
                if (Main.tile[i, j].type == TileID.ShadowOrbs && !WorldGen.shadowOrbSmashed && !fail && textCount == 0)
                {
                    textCount++;
                    Main.NewText("Illagers have begun exploring the lands", Color.Red);
                } 
            }
        }
    }
}