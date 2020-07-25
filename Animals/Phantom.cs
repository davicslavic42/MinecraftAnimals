using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinecraftAnimals.Animals
{
    public class Phantom : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantom");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {
            npc.noGravity = true;
            npc.width = 47;
            npc.height = 14;
            npc.lifeMax = 58;
            npc.damage = 25;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.aiStyle = 108;
            aiType = NPCID.DD2WyvernT1;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnCondition.OverworldNightMonster.Chance * 0.03f;
        }
        private const int Frame_Float = 0;
        private const int Frame_Float_2 = 1;
        private const int Frame_Float_3 = 2;
        private const int Frame_Float_4 = 3;
        private const int Frame_Float_5 = 4;
        private const int Frame_Float_6 = 5;
        public override void FindFrame(int frameHeight)
        {
            // This makes the sprite flip horizontally in conjunction with the npc.direction.
            npc.spriteDirection = npc.direction;
            {
                npc.frameCounter++;
                if (npc.frameCounter < 9)
                {
                    npc.frame.Y = Frame_Float * frameHeight;
                }
                else if (npc.frameCounter < 18)
                {
                    npc.frame.Y = Frame_Float_2 * frameHeight;
                }
                else if (npc.frameCounter < 27)
                {
                    npc.frame.Y = Frame_Float_3 * frameHeight;
                }
                else if (npc.frameCounter < 34)
                {
                    npc.frame.Y = Frame_Float_5 * frameHeight;
                }
                else if (npc.frameCounter < 45)
                {
                    npc.frame.Y = Frame_Float_6 * frameHeight;
                }
                else if (npc.frameCounter < 56)
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


