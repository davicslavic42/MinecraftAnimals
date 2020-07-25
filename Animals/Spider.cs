using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinecraftAnimals.Animals
{
    public class Spider : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spider");
            Main.npcFrameCount[npc.type] = 3;
        }
        public override void SetDefaults()
        {
            npc.width = 40;
            npc.height = 40;
            npc.lifeMax = 68;
            npc.damage = 38;
            npc.knockBackResist = 1f;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.aiStyle = -1;
            npc.value = 35f;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnCondition.Overworld.Chance * 0.03f;
        }
        private const int AI_State_Slot = 0;
        private const int AI_Timer_Slot = 1;
        // Here I define some values I will use with the State slot. Using an ai slot as a means to store "state" can simplify things greatly. Think flowchart.
        private const int State_Find = 0;
        private const int State_Attack = 1;
        public float AI_State
        {
            get => npc.ai[AI_State_Slot];
            set => npc.ai[AI_State_Slot] = value;
        }

        public float AI_Timer
        {
            get => npc.ai[AI_Timer_Slot];
            set => npc.ai[AI_Timer_Slot] = value;
        }

        public override void AI()
        {

            if (AI_State == State_Find)
            {
                AI_Timer++;
                Player player = Main.player[npc.target];
                npc.TargetClosest(true);
                npc.velocity.X = 0;
                npc.velocity.Y += 0.5f;

                if (npc.HasValidTarget && Main.player[npc.target].Distance(npc.Center) < 650f)
                {
                    AI_State = State_Attack;
                    AI_Timer = 0;
                }
            }
            // thanks oli for the tile checks
            else if (AI_State == State_Attack)
            {
                Player player = Main.player[npc.target];
                npc.TargetClosest(true);
                npc.velocity.X = 1.5f * npc.direction;
                npc.velocity.Y += 0.5f;

                if (npc.HasValidTarget && Main.player[npc.target].Distance(npc.Center) > 650f)
                {
                    AI_State = State_Find;
                    AI_Timer = 0;
                }
            }
            Collision.StepUp(ref npc.position, ref npc.velocity, npc.width, npc.height, ref npc.stepSpeed, ref npc.gfxOffY);
            if (npc.type == mod.NPCType("Spider") && Main.netMode != NetmodeID.MultiplayerClient && npc.velocity.Y > 1f)
            {
                int num99 = (int)npc.Center.X / 16;
                int num100 = (int)npc.Center.Y / 16;
                bool flag9 = false;
                for (int num101 = num99 - 1; num101 <= num99 + 1; num101++)
                {
                    for (int num102 = num100 - 1; num102 <= num100 + 1; num102++)
                    {
                        if (Main.tile[num101, num102].wall > 0)
                        {
                            flag9 = true;
                        }
                    }
                }
                if (flag9)
                {
                    npc.Transform(mod.NPCType("SpiderWall"));
                }
            }
        }

        private const int Frame_Walk = 0;
        private const int Frame_Walk_2 = 1;
        private const int Frame_Walk_3 = 2;

        public override void FindFrame(int frameHeight)
        {
            // This makes the sprite flip horizontally in conjunction with the npc.direction.
            npc.spriteDirection = npc.direction;
            {
                npc.frameCounter++;
                if (npc.frameCounter < 10)
                {
                    npc.frame.Y = Frame_Walk * frameHeight;
                }
                else if (npc.frameCounter < 20)
                {
                    npc.frame.Y = Frame_Walk_2 * frameHeight;
                }
                else if (npc.frameCounter < 30)
                {
                    npc.frame.Y = Frame_Walk_3 * frameHeight;
                }
                else
                {
                    npc.frameCounter = 0;
                }
            }
        }
    }
}
