﻿using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using MinecraftAnimals.BaseAI;
using static Terraria.ModLoader.ModContent;

namespace MinecraftAnimals.Animals
{
    public class Zombie : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Zombie");
            Main.npcFrameCount[npc.type] = 8;
        }
        public override void SetDefaults()
        {
            npc.width = 30;
            npc.height = 60;
            npc.lifeMax = 100;
            npc.damage = 25;
            npc.knockBackResist = 0f;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.aiStyle = -1;
            npc.value = 35f;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnCondition.OverworldNightMonster.Chance * 0.1f;
        }
        // These const ints are for the benefit of the programmer. Organization is key to making an AI that behaves properly without driving you crazy.
        // Here I lay out what I will use each of the 4 npc.ai slots for.
        internal enum AIStates
        {
            Normal = 0,
            Attack = 1,
            Death = 2
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
                float change = GlobalTimer <= 500 ? npc.velocity.X = 1 * npc.direction : npc.velocity.X = 0 * npc.direction;
                if (GlobalTimer >= 800)
                {
                    GlobalTimer = 0;
                }
                if (npc.HasValidTarget && player.Distance(npc.Center) < 675f)
                {
                    Phase = (int)AIStates.Attack;
                    GlobalTimer = 0;
                }

            }
            // thanks oli for the tile checks
            if (Phase == (int)AIStates.Attack)
            {
                npc.TargetClosest(true);
                npc.velocity.X = 1.4f * npc.direction;
                float stopToAttack = player.Distance(npc.Center) < 27f ? npc.velocity.X = 0 * npc.direction : npc.velocity.X = 1 * npc.direction;
                if (npc.HasValidTarget && player.Distance(npc.Center) > 675f)
                {
                    Phase = (int)AIStates.Normal;
                    GlobalTimer = 0;
                }
                if (player.Distance(npc.Center) < 30f){
                    npc.velocity.X = 0 * npc.direction;
                }
            }
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

            if (Main.tile[x, y].active() && Main.tile[x, y].nactive() && Main.tileSolid[Main.tile[x, y].type]) //bit scuffed but the moment it gets close enough to a tile in front it will jump
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
                Main.spriteBatch.Draw(texture, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY),
                sourceRectangle, drawColor, npc.rotation, origin, npc.scale, spriteEffects, 0f);
            }
            return false;
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
                    if (++npc.frameCounter % 7 == 0)
                        npc.frame.Y = (npc.frame.Y / frameHeight + 1) % (Main.npcFrameCount[npc.type] - 3) * frameHeight;
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
                    if (++npc.frameCounter % 7 == 0)
                        npc.frame.Y = (npc.frame.Y / frameHeight + 1) % (Main.npcFrameCount[npc.type] - 3) * frameHeight;
                }
            }
            if (Phase == (int)AIStates.Death)
            {
                npc.frame.Y = Frame_Walk * frameHeight;
            }
        }
    }
}