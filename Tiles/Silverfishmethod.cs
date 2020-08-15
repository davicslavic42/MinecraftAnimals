using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using MinecraftAnimals.Animals;

namespace MinecraftAnimals.Tiles
{
    public class Silverfishmethod : GlobalTile
    {
        // thanks chem for the help
        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (Main.netMode != 1 && !WorldGen.noTileActions && !WorldGen.gen)
            {
                if (type == TileID.Stone && Main.rand.NextBool(2225))
                {

                    NPC.NewNPC(i * 16, j * 16, mod.NPCType("Silverfish"), 0);
                    Main.NewText("I'm here!");
                }
            }
        }
    }
}