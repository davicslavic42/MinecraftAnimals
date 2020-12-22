﻿using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using static Terraria.ModLoader.ModContent;
using System;
using System.Security.Cryptography.X509Certificates;

namespace MinecraftAnimals.Animals
{
    public class Enderman : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Enderman");
            Main.npcFrameCount[npc.type] = 12;
        }
        public override void SetDefaults()
        {
            npc.width = 40;
            npc.height = 88;
            npc.lifeMax = 150;
            npc.knockBackResist = 1f;
            npc.damage = 0;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.aiStyle = -1;
            npc.value = 35f;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnCondition.Overworld.Chance * 0.1f;
        }
        // These const ints are for the benefit of the programmer. Organization is key to making an AI that behaves properly without driving you crazy.
        // Here I lay out what I will use each of the 4 npc.ai slots for.
        internal enum AIStates
        {
            Passive = 0,
            Attack = 1,
            Death = 2,
            TP = 3,
            TPFail = 4
        }
        internal ref float GlobalTimer => ref npc.ai[0];
        internal ref float Phase => ref npc.ai[1];
        internal ref float AttackPhase => ref npc.ai[2];
        internal ref float AttackTimer => ref npc.ai[3];
        int dead = 1;

        public override void AI()
        {
            Collision.StepUp(ref npc.position, ref npc.velocity, npc.width, npc.height, ref npc.stepSpeed, ref npc.gfxOffY);
            GlobalTimer++;
            Player player = Main.player[npc.target];
            if (Phase == (int)AIStates.Passive)
            {
                npc.damage = 0;
                npc.TargetClosest(false);
                if (GlobalTimer == 5)
                {
                    npc.direction = Main.rand.Next(2) == 1 ? npc.direction = 1 : npc.direction = -1;
                }

                float isMoving = GlobalTimer <= 500 ? npc.velocity.X = 1 * npc.direction : npc.velocity.X = 0 * npc.direction; //basic passive movement for 500 ticks then stationary 300
                if (GlobalTimer >= 800)
                {
                    GlobalTimer = 0;
                }
            }
            if (Phase == (int)AIStates.Attack)
            {
                npc.TargetClosest(true);
                npc.damage = 30;
                npc.velocity.X = 2 * npc.direction;
                AttackTimer++;
                if (player.Distance(npc.Center) > 925f)
                {
                    Phase = (int)AIStates.Passive;
                }
                if (AttackTimer >= 600)
                {
                    Phase = (int)AIStates.TP;
                    AttackTimer = 0;
                }
            }
            if (Phase == (int)AIStates.TP)
            {
                AttackTimer++;
                npc.velocity.X = 0;
                AttackTimer = 0;
                for (int i = 0; i < 15; i++)
                {
                    int dustIndex = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustType<Dusts.Enderpoof>(), 0f, 0f, 100, default(Color), 1f);
                    Main.dust[dustIndex].noGravity = true;
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 angle = Vector2.UnitX.RotateRandom(Math.PI * 2);
                    npc.position.X = player.Center.X + (int)(Main.rand.Next(25, 355) * angle.X); //controls the main area of the random teleport
                    npc.position.Y = player.Center.Y + (int)(Main.rand.Next(25, 155) * angle.Y);
                    npc.netUpdate = true;
                    if (Main.tile[(int)(npc.position.X / 16), (int)(npc.position.Y / 16)].active())
                    {
                        AttackTimer = 0;
                        Phase = (int)AIStates.TPFail;
                    }
                    else
                    {
                        AttackTimer = 0;
                        Phase = (int)AIStates.Attack;
                    }
                }

            }
            if (Phase == (int)AIStates.TPFail)
            {
                float tpfail = AttackTimer >= 6 ? Phase = (int)AIStates.TP : npc.alpha = 255; // while attack timer is less than 6 the alpha is maxxed making the enderman invisible while it attempts to tp again 
            }
            if (Phase == (int)AIStates.Death)
            {
                GlobalTimer = 0;
                npc.life = 1;
                npc.dontTakeDamage = true;
                npc.netUpdate = true;
                npc.velocity.X = 0;
                npc.damage = 0;
                GeneralMethods.ManualMobRotation(npc.rotation, MathHelper.ToRadians(90f), 5f);
                if (GlobalTimer >= 100)
                {
                    dead = 0;
                    CheckDead();
                }
            }
            if (npc.life < 5)
            {
                Phase = (int)AIStates.Death;
            }
        }
        public override bool CheckActive()
        {
            if (dead == 0)
            {
                return true;
            }
            return true;
        }
        public override bool CheckDead()
        {
            Phase = (int)AIStates.Death;
            if (GlobalTimer > 150)
            {
                return true;
            }
            return false;
        }        //Thanks oli//
        private bool RectangeIntersectsTiles(Rectangle rectangle)
        {
            bool intersects = false;
            for (int i = (int)rectangle.TopLeft().X; i < (int)rectangle.TopLeft().X + rectangle.Width; i++)
            {
                for (int j = (int)rectangle.TopLeft().Y; j < (int)rectangle.TopLeft().Y + rectangle.Height; j++)
                {
                    Tile tile = Main.tile[i / 16, j / 16];
                    if (tile != null && Main.tileSolid[tile.type] && tile.nactive())
                    {
                        intersects = true;
                    }
                }
            }
            return intersects;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            npc.friendly = false;
            Phase = (int)AIStates.Attack;
            GlobalTimer = 0;
            if(npc.life < 20)
            {
                damage *= 0.35f;
            }
            base.HitEffect(hitDirection, damage);
        }
        // The npc starts in the asleep state, waiting for a player to enter range
        // Our texture is 32x32 with 2 pixels of padding vertically, so 34 is the vertical spacing.  These are for my benefit and the numbers could easily be used directly in the code below, but this is how I keep code organized.
        private const int Frame_Walk = 0;
        private const int Frame_Walk_2 = 1;
        private const int Frame_Walk_3 = 2;
        private const int Frame_Walk_4 = 3;
        private const int Frame_Walk_5 = 4;
        private const int Frame_Walk_6 = 5;
        private const int Frame_Attack = 6;
        private const int Frame_Attack_2 = 7;
        private const int Frame_Attack_3 = 8;
        private const int Frame_Attack_4 = 9;
        private const int Frame_Attack_5 = 10;
        private const int Frame_Attack_6 = 11;

        // Here in FindFrame, we want to set the animation frame our npc will use depending on what it is doing.
        // We set npc.frame.Y to x * frameHeight where x is the xth frame in our spritesheet, counting from 0. For convenience, I have defined some consts above.
        public override void FindFrame(int frameHeight)
        {
            int i = 1;
            // This makes the sprite flip horizontally in conjunction with the npc.direction.
            npc.spriteDirection = npc.direction;
            if (Phase == (int)AIStates.Passive)
            {
                npc.frameCounter++;
                if (GlobalTimer <= 500)
                {
                    if (++npc.frameCounter % 7 == 0)
                        npc.frame.Y = (npc.frame.Y / frameHeight + 1) % (Main.npcFrameCount[npc.type] / 2) * frameHeight;
                }
                else
                {
                    npc.frame.Y = Frame_Walk * frameHeight;
                }
            }
            if (Phase == (int)AIStates.Attack)
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
                else if (npc.frameCounter < 21)
                {
                    npc.frame.Y = Frame_Attack_3 * frameHeight;
                }
                else if (npc.frameCounter < 28)
                {
                    npc.frame.Y = Frame_Attack_4 * frameHeight;
                }
                else if (npc.frameCounter < 35)
                {
                    npc.frame.Y = Frame_Attack_5 * frameHeight;
                }
                else if (npc.frameCounter < 42)
                {
                    npc.frame.Y = Frame_Attack_6 * frameHeight;
                }
                else
                {
                    npc.frameCounter = 0;
                }
            }
            if (Phase == (int)AIStates.TP)
            {
                npc.frame.Y = Frame_Walk * frameHeight;
            }
            if (Phase == (int)AIStates.Death)
            {
                npc.frame.Y = Frame_Walk * frameHeight;
            }
        }
    }
}
