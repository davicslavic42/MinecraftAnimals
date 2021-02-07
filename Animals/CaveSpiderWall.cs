using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MinecraftAnimals.Animals
{
    public class CaveSpiderWall : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cave Spider");
            Main.npcFrameCount[npc.type] = 4;
        }
        public override void SetDefaults()
        {
            npc.width = 40;
            npc.noGravity = true;
            npc.height = 40;
            npc.lifeMax = 58;
            npc.damage = 25;
            npc.knockBackResist = 1f;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.aiStyle = -1;
            npc.value = 35f;
            npc.scale = .75f;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnCondition.Cavern.Chance * 0.03f;
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            base.OnHitPlayer(target, damage, crit);
            target.AddBuff(BuffID.Poisoned, 180);
        }
        internal enum AIStates
        {
            Normal = 0,
            Death = 1

        }
        internal ref float GlobalTimer => ref npc.ai[0];
        internal ref float Phase => ref npc.ai[1];
        internal ref float ActionPhase => ref npc.ai[2];
        internal ref float AttackTimer => ref npc.ai[3];

        public override void AI()
        {
            if (Phase == (int)AIStates.Normal)
            {
                if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead)
                {
                    npc.TargetClosest(true);
                }
                float num548 = 2f;
                float moveSpeed = 0.16f;
                Vector2 vector70 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                float num550 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2);
                float num551 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2);
                num550 = (float)((int)(num550 / 8f) * 8);
                num551 = (float)((int)(num551 / 8f) * 8);
                vector70.X = (float)((int)(vector70.X / 8f) * 8);
                vector70.Y = (float)((int)(vector70.Y / 8f) * 8);
                num550 -= vector70.X;
                num551 -= vector70.Y;
                float num552 = (float)Math.Sqrt((double)(num550 * num550 + num551 * num551));
                if (num552 == 0f)
                {
                    num550 = npc.velocity.X;
                    num551 = npc.velocity.Y;
                }
                else
                {
                    num552 = num548 / num552;
                    num550 *= num552;
                    num551 *= num552;
                }
                if (Main.player[npc.target].dead)
                {
                    num550 = (float)npc.direction * num548 / 2f;
                    num551 = -num548 / 2f;
                }
                npc.spriteDirection = -1;
                if (!Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                {
                    npc.ai[0] += 1f;
                    if (npc.ai[0] > 0f)
                    {
                        npc.velocity.Y = npc.velocity.Y + 0.023f;
                    }
                    else
                    {
                        npc.velocity.Y = npc.velocity.Y - 0.023f;
                    }
                    if (npc.ai[0] < -100f || npc.ai[0] > 100f)
                    {
                        npc.velocity.X = npc.velocity.X + 0.023f;
                    }
                    else
                    {
                        npc.velocity.X = npc.velocity.X - 0.023f;
                    }
                    if (npc.ai[0] > 200f)
                    {
                        npc.ai[0] = -200f;
                    }
                    npc.velocity.X = npc.velocity.X + num550 * 0.007f;
                    npc.velocity.Y = npc.velocity.Y + num551 * 0.007f;
                    npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X);
                    if ((double)npc.velocity.X > 1.5)
                    {
                        npc.velocity.X = npc.velocity.X * 0.9f;
                    }
                    if ((double)npc.velocity.X < -1.5)
                    {
                        npc.velocity.X = npc.velocity.X * 0.9f;
                    }
                    if ((double)npc.velocity.Y > 1.5)
                    {
                        npc.velocity.Y = npc.velocity.Y * 0.9f;
                    }
                    if ((double)npc.velocity.Y < -1.5)
                    {
                        npc.velocity.Y = npc.velocity.Y * 0.9f;
                    }
                    if (npc.velocity.X > 3f)
                    {
                        npc.velocity.X = 3f;
                    }
                    if (npc.velocity.X < -3f)
                    {
                        npc.velocity.X = -3f;
                    }
                    if (npc.velocity.Y > 3f)
                    {
                        npc.velocity.Y = 3f;
                    }
                    if (npc.velocity.Y < -3f)
                    {
                        npc.velocity.Y = -3f;
                    }
                }
                else
                {
                    Vector2 targetVelocity = npc.DirectionTo(Main.player[npc.target].Center);
                    targetVelocity *= 6f;
                    npc.velocity = Vector2.Lerp(npc.velocity, targetVelocity, 1f / 20f);
                    npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X);
                    /*if (npc.velocity.X < num550)
					{
						npc.velocity.X = npc.velocity.X + moveSpeed;
						if (npc.velocity.X < 0f && num550 > 0f)
						{
							npc.velocity.X = npc.velocity.X + moveSpeed;
						}
					}
					else if (npc.velocity.X > num550)
					{
						npc.velocity.X = npc.velocity.X - moveSpeed;
						if (npc.velocity.X > 0f && num550 < 0f)
						{
							npc.velocity.X = npc.velocity.X - moveSpeed;
						}
					}
					if (npc.velocity.Y < num551)
					{
						npc.velocity.Y = npc.velocity.Y + moveSpeed;
						if (npc.velocity.Y < 0f && num551 > 0f)
						{
							npc.velocity.Y = npc.velocity.Y + moveSpeed;
						}
					}
					else if (npc.velocity.Y > num551)
					{
						npc.velocity.Y = npc.velocity.Y - moveSpeed;
						if (npc.velocity.Y > 0f && num551 < 0f)
						{
							npc.velocity.Y = npc.velocity.Y - moveSpeed;
						}
					}
					//npc.velocity *= 1.5f;
					npc.rotation = (float)Math.Atan2((double)num551, (double)num550);*/
                }
                float num553 = 0.5f;
                if (npc.collideX)
                {
                    npc.netUpdate = true;
                    npc.velocity.X = npc.oldVelocity.X * -num553;
                    if (npc.direction == -1 && npc.velocity.X > 0f && npc.velocity.X < 2f)
                    {
                        npc.velocity.X = 2f;
                    }
                    if (npc.direction == 1 && npc.velocity.X < 0f && npc.velocity.X > -2f)
                    {
                        npc.velocity.X = -2f;
                    }
                }
                if (npc.collideY)
                {
                    npc.netUpdate = true;
                    npc.velocity.Y = npc.oldVelocity.Y * -num553;
                    if (npc.velocity.Y > 0f && (double)npc.velocity.Y < 1.5)
                    {
                        npc.velocity.Y = 2f;
                    }
                    if (npc.velocity.Y < 0f && (double)npc.velocity.Y > -1.5)
                    {
                        npc.velocity.Y = -2f;
                    }
                }
                if (((npc.velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
                {
                    npc.netUpdate = true;
                }
                if (Main.netMode != 1)
                {
                    int num554 = (int)npc.Center.X / 16;
                    int num555 = (int)npc.Center.Y / 16;
                    bool flag36 = false;
                    int num;
                    for (int num556 = num554 - 1; num556 <= num554 + 1; num556 = num + 1)
                    {
                        for (int num557 = num555 - 1; num557 <= num555 + 1; num557 = num + 1)
                        {
                            if (Main.tile[num556, num557] == null)
                            {
                                return;
                            }
                            if (Main.tile[num556, num557].wall > 0)
                            {
                                flag36 = true;
                            }
                            num = num557;
                        }
                        num = num556;
                    }
                    if (!flag36)
                    {
                        npc.Transform(mod.NPCType("CaveSpider"));
                        return;
                    }
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
                npc.rotation = GeneralMethods.ManualMobRotation(npc.rotation, MathHelper.ToRadians(180f), 14f);
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
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                GlobalTimer = 0;
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
        private const int Frame_Crawl = 0;
        private const int Frame_Crawl_2 = 1;
        private const int Frame_Crawl_3 = 2;
        private const int Frame_Crawl_4 = 3;


        public override void FindFrame(int frameHeight)
        {
            // This makes the sprite flip horizontally in conjunction with the npc.direction.
            npc.spriteDirection = npc.direction;
            if (Phase == (int)AIStates.Normal)
            {
                npc.frameCounter++;
                if (++npc.frameCounter % 7 == 0)
                    npc.frame.Y = (npc.frame.Y / frameHeight + 1) % (Main.npcFrameCount[npc.type]) * frameHeight;
            }
            if (Phase == (int)AIStates.Death)
            {
                npc.frame.Y = Frame_Crawl * frameHeight;
            }
        }
    }
}