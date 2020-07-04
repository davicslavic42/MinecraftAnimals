using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinecraftAnimals.Animals
{
    public class Bat : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bat");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.noGravity = true;
            npc.width = 68;
            npc.height = 50;
            npc.lifeMax = 100;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.aiStyle = 14;
            aiType = NPCID.CaveBat;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnCondition.Cavern.Chance * 0.02f;
        }
        private const int Frame_Float = 0;
        private const int Frame_Float_2 = 1;
        private const int Frame_Float_3 = 2;
        private const int Frame_Float_4 = 3;
        public override void FindFrame(int frameHeight)
        {
            // This makes the sprite flip horizontally in conjunction with the npc.direction.
            npc.spriteDirection = npc.direction;
            {
                npc.frameCounter++;
                if (npc.frameCounter < 8)
                {
                    npc.frame.Y = Frame_Float * frameHeight;
                }
                else if (npc.frameCounter < 16)
                {
                    npc.frame.Y = Frame_Float_2 * frameHeight;
                }
                else if (npc.frameCounter < 24)
                {
                    npc.frame.Y = Frame_Float_3 * frameHeight;
                }
                else if (npc.frameCounter < 32)
                {
                    npc.frame.Y = Frame_Float_4 * frameHeight;
                }
                else
                {
                    npc.frameCounter = 0;
                }
            }
        }
    }
}

