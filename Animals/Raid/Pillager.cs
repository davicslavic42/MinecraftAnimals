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
using MinecraftAnimals.BaseAI;

namespace MinecraftAnimals.Animals.Raid
{
    public class Pillager : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pillager");
            Main.npcFrameCount[npc.type] = 9;
        }
        public override void SetDefaults()
        {
            npc.width = 32;
            npc.height = 50;
            npc.lifeMax = 85;
            npc.knockBackResist = 0f;
            npc.damage = 8;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.aiStyle = -1;
            npc.value = 35f;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (MCAWorld.RaidEvent)
            {
                return 45f;
            }
            return SpawnCondition.Overworld.Chance * 0.03f;
        }
        // These const ints are for the benefit of the programmer. Organization is key to making an AI that behaves properly without driving you crazy.
        // Here I lay out what I will use each of the 4 npc.ai slots for.
        internal enum AIStates
        {
            Normal = 0,
            Attack = 1,
            Shoot = 2,
            Death = 3
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
                    npc.direction = Main.rand.Next(2) == 1 ? npc.direction = 1 : npc.direction = -1;
                }
                float walkOrPause = GlobalTimer <= 500 ? npc.velocity.X = 1 * npc.direction : npc.velocity.X = 0 * npc.direction;
                if (GlobalTimer >= 800)
                {
                    GlobalTimer = 0;
                }
                if (npc.HasValidTarget && player.Distance(npc.Center) < 725f)
                {
                    Phase = (int)AIStates.Attack;
                    GlobalTimer = 0;
                }
            }
            if (Phase == (int)AIStates.Attack)
            {
                npc.TargetClosest(true);
                npc.velocity.X = 1.4f * npc.direction;
                if (npc.HasValidTarget && player.Distance(npc.Center) > 725f)
                {
                    Phase = (int)AIStates.Normal;
                    GlobalTimer = 0;
                }
                if (player.Distance(npc.Center) < 350f)
                {
                    Phase = (int)AIStates.Shoot;
                    GlobalTimer = 0;
                }
            }
            // In this state, a player has been targeted
            if (Phase == (int)AIStates.Shoot)
            {
                npc.TargetClosest(true);
                npc.velocity.X = 0f * npc.direction;
                npc.velocity.Y += 0.5f;
                npc.velocity.X = 0 * npc.direction;
                npc.velocity.Y += 0.5f;
                if (npc.frameCounter == 144)
                {
                    Player TargetPlayer = Main.player[(int)Player.FindClosest(npc.position, npc.width, npc.height)];
                    _ = npc.Distance(npc.position) - 25;
                    Vector2 PlayerDir = npc.DirectionTo(TargetPlayer.position);
                    Vector2 DirToRing = npc.DirectionTo(TargetPlayer.position + PlayerDir.RotatedBy(0.001f) * -50f);

                    npc.velocity.X += DirToRing.X;
                    npc.velocity.Y += DirToRing.Y;

                    Projectile.NewProjectile(npc.Center, PlayerDir.RotatedByRandom(0.1f) * 8f, mod.ProjectileType("Arrow"), 22, 3, Main.LocalPlayer.whoAmI);
                }
                if (!npc.HasValidTarget || Main.player[npc.target].Distance(npc.Center) > 350f)
                {
                    // Out targeted player seems to have left our range, so we'll go back to sleep.
                    Phase = (int)AIStates.Attack;
                    GlobalTimer = 0;
                }
                // If the targeted player is in attack range (250).
            }
            // In this state, we are in the Shoot. 
            if (Phase == (int)AIStates.Death)
            {
                npc.damage = 0;
                npc.ai[2] += 1f; // increase our death timer.
                npc.netUpdate = true;
                npc.velocity.X = 0;
                npc.velocity.Y += 1.5f;
                npc.dontTakeDamage = true;
                npc.rotation = GeneralMethods.ManualMobRotation(npc.rotation, MathHelper.ToRadians(90f), 8f);
                if (npc.ai[2] >= 110f)
                {
                    for (int i = 0; i < 20; i++)
                    {
                        int dustIndex = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustType<Dusts.Poof>(), 0f, 0f, 100, default(Color), 1f); //spawns ender dust
                        Main.dust[dustIndex].noGravity = true;
                    }
                    npc.life = 0;
                }
            }
            int x = (int)(npc.Center.X + ((npc.width / 2) + 8) * npc.direction) / 16;
            int y = (int)(npc.Center.Y + ((npc.height / 2) * npc.direction) - 1) / 16;

            if (Main.tile[x, y].active() && Main.tile[x, y].nactive() && Main.tileSolid[Main.tile[x, y].type])
            {
                int i = 0;
                i++;
                if (i == 1 && GlobalTimer < 500)
                {
                    npc.velocity = new Vector2(npc.direction * 1, -7f);
                    i = 0;
                }
            }
        }
        public override void NPCLoot()
        {
            if (MCAWorld.RaidEvent == true)
            {
                MCAWorld.RaidKillCount += 1;
            }
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            GlobalTimer = 0;
            if (npc.life <= 0)
            {
                npc.life = 1;
                Phase = (int)AIStates.Death;
            }
            base.HitEffect(hitDirection, damage);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (npc.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Texture2D texture = Main.npcTexture[npc.type];
            int frameHeight = Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type];
            int startY = npc.frame.Y;
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            origin.X = (float)(npc.spriteDirection == 1 ? sourceRectangle.Width - 20 : 20);

            Color drawColor = npc.GetAlpha(lightColor);
            if (Phase == (int)AIStates.Death)
            {
                Main.spriteBatch.Draw(texture, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY + 20),
                sourceRectangle, Color.Red * 0.8f, npc.rotation, origin, npc.scale, spriteEffects, 0f);
            }
            else
            {
                Main.spriteBatch.Draw(texture, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY - 10),
                sourceRectangle, drawColor, npc.rotation, origin, npc.scale, spriteEffects, 0f);
            }
            return false;
        }
        // Our texture is 32x32 with 2 pixels of padding vertically, so 34 is the vertical spacing.  These are for my benefit and the numbers could easily be used directly in the code below, but this is how I keep code organized.
        private const int Frame_Walk = 0;
        private const int Frame_Walk_2 = 1;
        private const int Frame_Walk_3 = 2;
        private const int Frame_Walk_4 = 3;
        private const int Frame_Walk_5 = 4;
        private const int Frame_Shoot = 5;
        private const int Frame_Shoot_2 = 6;
        private const int Frame_Shoot_3 = 7;
        private const int Frame_Shoot_4 = 8;
        // Here in FindFrame, we want to set the animation frame our npc will use depending on what it is doing.
        // We set npc.frame.Y to x * frameHeight where x is the xth frame in our spritesheet, counting from 0. For convenience, I have defined some consts above.
        public override void FindFrame(int frameHeight)
        {
            // This makes the sprite flip horizontally in conjunction with the npc.direction.
            npc.spriteDirection = npc.direction;
            if (Phase == (int)AIStates.Normal)
            {
                npc.frameCounter++;
                if (GlobalTimer <= 500)
                {
                    if (++npc.frameCounter % 7 == 0)
                        npc.frame.Y = (npc.frame.Y / frameHeight + 1) % ((Main.npcFrameCount[npc.type]) - 4) * frameHeight;
                }
                else
                {
                    npc.frame.Y = Frame_Walk * frameHeight;
                }
            }
            if (Phase == (int)AIStates.Attack)
            {
                npc.frameCounter++;
                if (++npc.frameCounter % 7 == 0)
                    npc.frame.Y = (npc.frame.Y / frameHeight + 1) % ((Main.npcFrameCount[npc.type]) - 4) * frameHeight;
            }
            if (Phase == (int)AIStates.Shoot)
            {
                npc.frameCounter++;
                if (npc.frameCounter < 15)
                {
                    npc.frame.Y = Frame_Shoot * frameHeight;
                }
                else if (npc.frameCounter < 25)
                {
                    npc.frame.Y = Frame_Shoot_2 * frameHeight;
                }
                else if (npc.frameCounter < 135)
                {
                    npc.frame.Y = Frame_Shoot_3 * frameHeight;
                }
                else if (npc.frameCounter < 145)
                {
                    npc.frame.Y = Frame_Shoot_4 * frameHeight;
                }
                else
                {
                    npc.frameCounter = 0;
                }
            }
            if (Phase == (int)AIStates.Death)
            {
                npc.frame.Y = Frame_Walk * frameHeight;
            }
        }
    }
}