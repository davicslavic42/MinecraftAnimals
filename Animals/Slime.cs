﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MinecraftAnimals.Animals
{
    public class Slime : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slime");
            Main.npcFrameCount[npc.type] = 2;
        }

        public override void SetDefaults()
        {
            npc.width = 32;
            npc.height = 26;
            npc.lifeMax = 38;
            npc.damage = 10;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.aiStyle = -1;
        }
        internal enum AIStates
        {
            Passive = 0,
            Jump = 1,
            Crouch = 2,
            Death = 3
        }
        internal ref float GlobalTimer => ref npc.ai[0];
        internal ref float Phase => ref npc.ai[1];
        internal ref float ActionPhase => ref npc.ai[2];
        internal ref float AttackTimer => ref npc.ai[3];
        // Our AI here makes our NPC sit waiting for a player to enter range, jumps to attack, flutter mid-fall to stay afloat a little longer, then falls to the ground. Note that animation should happen in FindFrame
        public override void AI()
        {
            Collision.StepUp(ref npc.position, ref npc.velocity, npc.width, npc.height, ref npc.stepSpeed, ref npc.gfxOffY);
            Player player = Main.player[npc.target];
            npc.TargetClosest(true);
            GlobalTimer++;
            // The npc starts in the asleep state, waiting for a player to enter range
            if (Phase == (int)AIStates.Passive)
            {
                if (GlobalTimer == 5)
                {
                    _ = Main.rand.Next(2) == 1 ? npc.direction = 1 : npc.direction = -1;
                    npc.velocity = new Vector2(npc.direction * 2, -4f);
                }
                if (GlobalTimer >= 245 && Collision.SolidCollision(npc.position, (npc.width), npc.height + 1))
                {
                    GlobalTimer = 0;
                }
                if (npc.HasValidTarget && player.Distance(npc.Center) < 740f)
                {
                    // Since we have a target in range, we change to the Notice state. (and zero out the Timer for good measure)
                    Phase = (int)AIStates.Crouch;
                    GlobalTimer = 0;
                }
            }
            // In this state, a player has been targeted
            if (Phase == (int)AIStates.Crouch)
            {
                ActionPhase = 20;
                ActionPhase--;
                if (AttackTimer >= 0 && AttackTimer <= 20) npc.velocity.X = ActionPhase / 15f * npc.direction;
                else npc.velocity.X = 0 * npc.direction;
                if (player.Distance(npc.Center) < 720f)
                {
                    AttackTimer++;
                    if (AttackTimer >= 50)
                    {
                        Phase = (int)AIStates.Jump;
                        AttackTimer = 0;
                        GlobalTimer = 0;
                    }
                }
                if (!npc.HasValidTarget || player.Distance(npc.Center) > 740f)
                {
                    // Out targeted player seems to have left our range, so we'll go back to sleep.
                    Phase = (int)AIStates.Passive;
                    GlobalTimer = 0;
                }
            }
            if (Phase == (int)AIStates.Jump)
            {
                if (GlobalTimer >= 5 && GlobalTimer <= 20) npc.velocity = new Vector2(npc.direction * 2, -5f);
                if (GlobalTimer >= 5 && GlobalTimer <= 20 && player.Distance(npc.Center) < 220f) npc.velocity = new Vector2(npc.direction * 4, -3f);

                if (GlobalTimer >= 50 && Collision.SolidCollision(npc.position, (npc.width), npc.height + 1))
                {
                    npc.velocity = new Vector2(npc.direction * 3f, -2.5f);
                    Phase = (int)AIStates.Crouch;
                    GlobalTimer = 0;
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
                    for (int y = 0; y < 1; y++)
                    {
                        NPC.NewNPC(Main.rand.Next((int)npc.position.X - 95, (int)npc.position.X + 95), Main.rand.Next((int)npc.position.Y - 55, (int)npc.position.Y), NPCType<SlimePiece>(), 0);
                    }
                    npc.life = 0;
                }
            }
        }
        public override void HitEffect(int hitDirection, double damage)
        {
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
                Main.spriteBatch.Draw(texture, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY),
                sourceRectangle, Color.Red * 0.8f, npc.rotation, origin, npc.scale, spriteEffects, 0f);
            }
            else
            {
                Main.spriteBatch.Draw(texture, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY - 4),
                sourceRectangle, drawColor, npc.rotation, origin, npc.scale, spriteEffects, 0f);
            }
            return false;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnCondition.OverworldDay.Chance * 0.01f;
        }
        private const int Frame_Hop = 0;
        private const int Frame_Hop_2 = 1;
        public override void NPCLoot()
        {
        }

        public override void FindFrame(int frameHeight)
        {
            // This makes the sprite flip horizontally in conjunction with the npc.direction.
            npc.spriteDirection = npc.direction;
            {
                npc.frameCounter++;
                if (npc.frameCounter < 10)
                {
                    npc.frame.Y = Frame_Hop * frameHeight;
                }
                else if (npc.frameCounter < 20)
                {
                    npc.frame.Y = Frame_Hop_2 * frameHeight;
                }
                else
                {
                    npc.frameCounter = 0;
                }
            }
            if (Phase == (int)AIStates.Death)
            {
                npc.frame.Y = Frame_Hop * frameHeight;
            }
        }
    }
}