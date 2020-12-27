using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace MinecraftAnimals.Animals
{
    public class Husk : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Husk");
            Main.npcFrameCount[npc.type] = 8;
        }
        public override void SetDefaults()
        {
            npc.width = 34;
            npc.height = 60;
            npc.lifeMax = 50;
            npc.damage = 18;
            npc.knockBackResist = 0f;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.aiStyle = -1;
            npc.value = 35f;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnCondition.OverworldDayDesert.Chance * 0.1f;
        }
        // These const ints are for the benefit of the programmer. Organization is key to making an AI that behaves properly without driving you crazy.
        // Here I lay out what I will use each of the 4 npc.ai slots for.
        public enum AIStates
        {
            Normal = 0,
            Attack = 1
        }
        internal ref float GlobalTimer => ref npc.ai[0];
        internal ref float Phase => ref npc.ai[1];
        internal ref float ActionPhase => ref npc.ai[2];
        internal ref float AttackTimer => ref npc.ai[3];
        public override void AI()
        {
            Collision.StepUp(ref npc.position, ref npc.velocity, npc.width, npc.height, ref npc.stepSpeed, ref npc.gfxOffY);
            GlobalTimer++;
            Player player = Main.player[npc.target];
            if (Phase == (int)AIStates.Normal)
            {
                npc.TargetClosest(false);
                npc.velocity.X = 1 * npc.direction;
                if (GlobalTimer == 5)
                {
                    switch (Main.rand.Next(2))
                    {
                        case 0:
                            npc.direction = 1;
                            return;
                        case 1:
                            npc.direction = -1;
                            return;
                    }
                }
                float change = GlobalTimer <= 500 ? npc.velocity.X = 1 * npc.direction : npc.velocity.X = 0 * npc.direction;
                if (GlobalTimer >= 800)
                {
                    GlobalTimer = 0;
                }
                if (npc.HasValidTarget && player.Distance(npc.Center) < 575f)
                {
                    Phase = (int)AIStates.Attack;
                    GlobalTimer = 0;
                }

            }
            // thanks oli for the tile checks
            if (Phase == (int)AIStates.Attack)
            {
                npc.TargetClosest(true);
                npc.velocity.X = 0.75f * npc.direction;

                if (npc.HasValidTarget && player.Distance(npc.Center) > 575f)
                {
                    Phase = (int)AIStates.Normal;
                    GlobalTimer = 0;
                }
                _ = player.Distance(npc.Center) < 29f ? npc.velocity.X = 0 * npc.direction : npc.velocity.X = 1 * npc.direction;
            }
            if (Collision.SolidCollision(npc.position, (npc.width / 2), npc.height - 1) && AttackTimer >= 50)
            {
                for (int i = 0; i < 1; i++)
                {
                    npc.velocity = new Vector2(npc.direction * 2, -6f);
                }
                AttackTimer = 0;
            }
        }


        private const int Frame_Walk = 0;
        private const int Frame_Walk_2 = 1;
        private const int Frame_Walk_3 = 2;
        private const int Frame_Walk_4 = 3;
        private const int Frame_Walk_5 = 4;
        private const int Frame_Attack = 5;
        private const int Frame_Attack_2 = 6;
        private const int Frame_Attack_3 = 7;
        public override void FindFrame(int frameHeight)
        {
            Player player = Main.player[npc.target];
            // This makes the sprite flip horizontally in conjunction with the npc.direction.
            npc.spriteDirection = npc.direction;
            if (Phase == (int)AIStates.Normal)
            {
                npc.frameCounter++;
                if (GlobalTimer <= 500)
                {
                    if (npc.frameCounter < 7)
                    {
                        npc.frame.Y = Frame_Walk * frameHeight;
                    }
                    else if (npc.frameCounter < 14)
                    {
                        npc.frame.Y = Frame_Walk_2 * frameHeight;
                    }
                    else if (npc.frameCounter < 28)
                    {
                        npc.frame.Y = Frame_Walk_3 * frameHeight;
                    }
                    else if (npc.frameCounter < 35)
                    {
                        npc.frame.Y = Frame_Walk_4 * frameHeight;
                    }
                    else if (npc.frameCounter < 42)
                    {
                        npc.frame.Y = Frame_Walk_5 * frameHeight;
                    }
                    else
                    {
                        npc.frameCounter = 0;
                    }
                }
                else
                {
                    npc.frame.Y = Frame_Walk * frameHeight;
                }
            }
            if (Phase == (int)AIStates.Attack)
            {
                npc.frameCounter++;
                if (player.Distance(npc.Center) < 34f)
                {
                    npc.frameCounter++;
                    if (npc.frameCounter < 7)
                    {
                        npc.frame.Y = Frame_Attack * frameHeight;
                    }
                    else if (npc.frameCounter < 14)
                    {
                        npc.frame.Y = Frame_Attack_2 * frameHeight;
                    }
                    else if (npc.frameCounter < 28)
                    {
                        npc.frame.Y = Frame_Attack_3 * frameHeight;
                    }
                    else
                    {
                        npc.frameCounter = 0;
                    }

                }
                else
                {
                    if (npc.frameCounter < 7)
                    {
                        npc.frame.Y = Frame_Walk * frameHeight;
                    }
                    else if (npc.frameCounter < 14)
                    {
                        npc.frame.Y = Frame_Walk_2 * frameHeight;
                    }
                    else if (npc.frameCounter < 28)
                    {
                        npc.frame.Y = Frame_Walk_3 * frameHeight;
                    }
                    else if (npc.frameCounter < 35)
                    {
                        npc.frame.Y = Frame_Walk_4 * frameHeight;
                    }
                    else if (npc.frameCounter < 42)
                    {
                        npc.frame.Y = Frame_Walk_5 * frameHeight;
                    }
                    else
                    {
                        npc.frameCounter = 0;
                    }

                }
            }
        }
    }
}          