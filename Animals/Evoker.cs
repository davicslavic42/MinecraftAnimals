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
    public class Evoker : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Evoker");
            Main.npcFrameCount[npc.type] = 9;
        }
        public override void SetDefaults()
        {
            npc.width = 60;
            npc.height = 60;
            npc.lifeMax = 210;
            npc.knockBackResist = 1f;
            npc.damage = 20;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.aiStyle = -1;
            npc.value = 35f;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnCondition.Overworld.Chance * 0;
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
        internal ref float AttackPhase => ref npc.ai[2];
        internal ref float AttackTimer => ref npc.ai[3];
        public override void AI()
        {
            Collision.StepUp(ref npc.position, ref npc.velocity, npc.width, npc.height, ref npc.stepSpeed, ref npc.gfxOffY);
            GlobalTimer++;
            Player player = Main.player[npc.target];
            npc.TargetClosest(true);
            if (Phase == (int)AIStates.Normal)
            {
                npc.velocity.X = 0 * npc.direction;
                if (npc.HasValidTarget && player.Distance(npc.Center) < 780f)
                {
                   
                    npc.velocity.X = 1.5f * npc.direction;
                }
                if (player.Distance(npc.Center) < 325f)
                {
                    Phase = (int)AIStates.Attack;
                    GlobalTimer = 0;
                }
            }
            // In this state, a player has been targeted
            if (Phase == (int)AIStates.Attack)
            {
                int dustIndex = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y - 2), npc.width, (npc.height / 2 - 1), 31, 0f, 0f, 100, default(Color), 1f);
                Main.dust[dustIndex].scale = 0.2f + (float)Main.rand.Next(5) * 0.1f;
                npc.ai[3]++;
                npc.velocity.X = 0;

                if (npc.ai[3] == 200)
                {
                    switch (Main.rand.Next(2))
                    {
                        case 0:
                            NPC.NewNPC(Main.rand.Next((int)npc.position.X - 15, (int)npc.position.X + 15), Main.rand.Next((int)npc.position.Y - 25, (int)npc.position.Y ), mod.NPCType("Vex"), 0);
                            npc.ai[3] = 0;
                            return;
                        case 1:
                            if (player.Distance(npc.Center) < 125f)
                            {
                                for (int i = 0; i < 2; i++)
                                {
                                    Projectile.NewProjectile((npc.Center.X - 55) + (i * 125), npc.position.Y - 10, 0, 2, mod.ProjectileType("Techproj"), 0, 3, Main.myPlayer);
                                }
                            }
                            if (player.Distance(npc.Center) > 125f)
                            {
                                for (int i = 0; i < 7; i++)
                                {
                                    Projectile.NewProjectile((npc.direction) + (i * 110), npc.position.Y - 10, 0, 2, mod.ProjectileType("Techproj"), 0, 3, Main.myPlayer);
                                }
                            }
                            npc.ai[3] = 25;
                            return;
                    }
                }
                else
                {
                    if (!npc.HasValidTarget || player.Distance(npc.Center) > 325f)
                    {
                        // Out targeted player seems to have left our range, so we'll go back to sleep.
                        Phase = (int)AIStates.Normal;
                        GlobalTimer = 0;
                    }
                }
            }
            if (Collision.SolidCollision(npc.position, (npc.width / 2 + 1), npc.height))
            {
                for (int i = 0; i < 1; i++)
                {
                    npc.velocity = new Vector2(npc.direction * 2, -6f);
                }
            }
        }

        // Our texture is 32x32 with 2 pixels of padding vertically, so 34 is the vertical spacing.  These are for my benefit and the numbers could easily be used directly in the code below, but this is how I keep code organized.
        private const int Frame_Walk = 0;
        private const int Frame_Walk_2 = 1;
        private const int Frame_Walk_3 = 2;
        private const int Frame_Walk_4 = 3;
        private const int Frame_Walk_5 = 4;
        private const int Frame_Magic = 5;
        private const int Frame_Magic_2 = 6;
        private const int Frame_Magic_3 = 7;
        private const int Frame_Magic_4 = 8;

        // Here in FindFrame, we want to set the animation frame our npc will use depending on what it is doing.
        // We set npc.frame.Y to x * frameHeight where x is the xth frame in our spritesheet, counting from 0. For convenience, I have defined some consts above.
        public override void FindFrame(int frameHeight)
        {
            // This makes the sprite flip horizontally in conjunction with the npc.direction.
            npc.spriteDirection = npc.direction;
            if (Phase == (int)AIStates.Normal)
            {
                npc.frameCounter++;
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
                    else if (npc.frameCounter < 38)
                    {
                        npc.frame.Y = Frame_Walk_5 * frameHeight;
                    }
                    else
                    {
                        npc.frameCounter = 0;
                    }
                }
            }
            if (Phase == (int)AIStates.Attack)
            {
                npc.frameCounter++;
                if (npc.frameCounter < 7)
                {
                    npc.frame.Y = Frame_Magic * frameHeight;
                }
                else if (npc.frameCounter < 14)
                {
                    npc.frame.Y = Frame_Magic_2 * frameHeight;
                }
                else if (npc.frameCounter < 28)
                {
                    npc.frame.Y = Frame_Magic_3 * frameHeight;
                }
                else if (npc.frameCounter < 35)
                {
                    npc.frame.Y = Frame_Magic_4 * frameHeight;
                }
                else
                {
                    npc.frameCounter = 0;
                }
            }
        }
    }
}