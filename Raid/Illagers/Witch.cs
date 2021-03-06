﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MinecraftAnimals.Raid.Illagers
{
    public class Witch : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Witch");
            Main.npcFrameCount[npc.type] = 7;
        }
        public override void SetDefaults()
        {
            npc.width = 45;
            npc.height = 50;
            npc.lifeMax = 97;
            npc.knockBackResist = 0f;
            npc.damage = 3;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.aiStyle = -1;
            npc.value = 35f;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnCondition.OverworldNight.Chance * 0.1f;
        }        // These const ints are for the benefit of the programmer. Organization is key to making an AI that behaves properly without driving you crazy.
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

            Vector2 TownTargets = GeneralMethods.GetAnyTownNpcTargetEntity(npc.Center, 775f);//gets target center
            Vector2 PlayerTarget = GeneralMethods.GetTargetPlayerEntity(npc.Center, 775f);//gets player center
            Vector2 newTargetCenter = npc.Distance(PlayerTarget) > npc.Distance(TownTargets) ? TownTargets : PlayerTarget;
            if (Phase == (int)AIStates.Normal)
            {
                npc.TargetClosest(false);
                float walkOrPause = GlobalTimer <= 500 ? npc.velocity.X = 1 * npc.direction : npc.velocity.X = 0 * npc.direction;

                if (GlobalTimer == 5) npc.direction = Main.rand.Next(2) == 1 ? npc.direction = 1 : npc.direction = -1;
                if (GlobalTimer >= 800) GlobalTimer = 0;
                if (npc.Distance(newTargetCenter) < 770f)
                {
                    Phase = (int)AIStates.Attack;
                    GlobalTimer = 0;
                }
            }
            if (Phase == (int)AIStates.Attack)
            {
                npc.direction = npc.Center.X > newTargetCenter.X ? npc.direction = -1 : npc.direction = 1;
                npc.velocity.X = 1.4f * npc.direction;
                if (npc.Distance(newTargetCenter) > 770f)
                {
                    Phase = (int)AIStates.Normal;
                    GlobalTimer = 0;
                }
                if (npc.Distance(newTargetCenter) < 270f && Collision.CanHitLine(npc.Center, 1, 1, newTargetCenter, 1, 1))
                {
                    Phase = (int)AIStates.Shoot;
                    GlobalTimer = 0;
                }
            }
            // In this state, a player has been targeted
            if (Phase == (int)AIStates.Shoot)
            {
                npc.direction = npc.Center.X > newTargetCenter.X ? npc.direction = -1 : npc.direction = 1;
                AttackTimer = 0;
                npc.velocity.X = 0 * npc.direction;
                AttackTimer++;
                if (npc.frameCounter == 80)
                {
                    Vector2 TargetDir = Vector2.Normalize(newTargetCenter - npc.Center);

                    Projectile.NewProjectile(npc.Center, TargetDir.RotatedByRandom(0.1f) * 8f, ProjectileType<projectiles.Harmpot>(), 18, 3, Main.LocalPlayer.whoAmI);
                }
                if (npc.Distance(newTargetCenter) > 270f && Collision.CanHitLine(npc.Center, 1, 1, newTargetCenter, 1, 1))
                {
                    // Out targeted player seems to have left our range, so we'll go back to sleep.
                    Phase = (int)AIStates.Attack;
                    AttackTimer = 0;
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
                        int dustIndex = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustType<Dusts.Poof>(), 2.25f * Main.rand.Next(-1, 1), 0f, 100, default(Color), 1f); //spawns ender dust
                        Main.dust[dustIndex].noGravity = true;
                    }
                    npc.life = 0;
                }
            }
            int x = (int)(npc.Center.X + (((npc.width / 2) + 8) * npc.direction)) / 16;
            int y = (int)(npc.Center.Y + (npc.height / 2) - 2) / 16;

            if (Main.tile[x, y].active() && Main.tile[x, y].nactive() && Main.tileSolid[Main.tile[x, y].type] && GlobalTimer % 50 == 0)
            {
                int i = 1;
                if (i == 1 && npc.velocity.X != 0)
                {
                    npc.velocity = new Vector2(npc.direction * 1, -7f);
                    i = 0;
                }
            }
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                npc.life = 1;
                //NetMessage.SendData(MessageID.WorldData);
                RaidWorld.RaidKillCount += 1f;
                Phase = (int)AIStates.Death;
            }
        }
        public override void NPCLoot()
        {
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
                Main.spriteBatch.Draw(texture, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY - 20),
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
        private const int Frame_Walk_6 = 5;
        private const int Frame_Throw = 6;
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
                    npc.frame.Y = (npc.frame.Y / frameHeight + 1) % (Main.npcFrameCount[npc.type] - 4) * frameHeight;
            }
            if (Phase == (int)AIStates.Shoot)
            {
                npc.frameCounter++;
                if (npc.frameCounter < 80)
                {
                    npc.frame.Y = Frame_Throw * frameHeight;
                }
                else if (npc.frameCounter < 100)
                {
                    npc.frame.Y = Frame_Walk * frameHeight;
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