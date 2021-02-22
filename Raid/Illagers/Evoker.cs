using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MinecraftAnimals.Raid.Illagers
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
            npc.lifeMax = 85;
            npc.knockBackResist = 1f;
            npc.damage = 20;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.aiStyle = -1;
            npc.value = 35f;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return 0f;
        }        // These const ints are for the benefit of the programmer. Organization is key to making an AI that behaves properly without driving you crazy.
        // Here I lay out what I will use each of the 4 npc.ai slots for.
        internal enum AIStates
        {
            Normal = 0,
            Approach = 1,
            Attack = 2,
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
            Vector2 TownTargets = GeneralMethods.GetAnyTownNpcTargetEntity(npc.Center, 635f);//gets target center
            Vector2 PlayerTarget = GeneralMethods.GetTargetPlayerEntity(npc.Center, 635f);//gets player center
            Vector2 newTargetCenter = npc.Distance(PlayerTarget) > npc.Distance(TownTargets) ? TownTargets : PlayerTarget;
            int attackType = 1;
            if (Phase == (int)AIStates.Normal)
            {
                npc.velocity.Y = 2;
                float isMoving = GlobalTimer <= 500 ? npc.velocity.X = 1 * npc.direction : npc.velocity.X = 0 * npc.direction; //basic passive movement for 500 ticks then stationary 300
                if (GlobalTimer >= 800)
                {
                    GlobalTimer = 0;
                }

                if (npc.Distance(newTargetCenter) < 630f) // passive player is within a certain range
                {
                    Phase = (int)AIStates.Approach;
                    GlobalTimer = 0;
                }
            }
            if (Phase == (int)AIStates.Approach)
            {
                npc.direction = npc.Center.X > newTargetCenter.X ? npc.direction = -1 : npc.direction = 1;
                npc.velocity.Y = 1.5f;
                npc.velocity.X = 1.25f * npc.direction;
                if (npc.Distance(newTargetCenter) > 630f) // passive player is within a certain range
                {
                    Phase = (int)AIStates.Normal;
                    GlobalTimer = 0;
                }
                if (npc.Distance(newTargetCenter) < 325f) 
                {
                    Phase = (int)AIStates.Attack;
                    GlobalTimer = 0;
                }
            }
            // In this state, a player has been targeted
            if (Phase == (int)AIStates.Attack)
            {
                npc.direction = npc.Center.X > newTargetCenter.X ? npc.direction = -1 : npc.direction = 1;
                int dustIndex = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y - 2), npc.width, (npc.height / 2 - 1), DustType<Dusts.Poteffect>(), 0f, 0f, 100, default(Color), 1f);// causes little potion effects to flaot around
                Main.dust[dustIndex].scale = 0.2f + (float)Main.rand.Next(5) * 0.1f;
                npc.ai[3]++;
                npc.velocity.X = 0;

                if (npc.ai[3] == 200)
                {
                    attackType = (Main.rand.Next(2));
                    if (attackType == 0)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            NPC.NewNPC(Main.rand.Next((int)npc.position.X - 50, (int)npc.position.X + 50), Main.rand.Next((int)npc.position.Y - 150, (int)npc.position.Y - 50), NPCType<Vex>(), 0);
                        }
                        npc.ai[3] = -200;
                    }
                    if (attackType == 1)
                    {
                        if (npc.Distance(newTargetCenter) < 125f)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                Projectile.NewProjectile((npc.Center.X - 75) + (i * 145), npc.position.Y - 10, 0, 2, ProjectileType<projectiles.Techproj>(), 0, 3, Main.myPlayer);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < 7; i++)
                            {
                                Projectile.NewProjectile(((npc.Center.X + 1) + (i * 110) * npc.direction), npc.position.Y - 10, 0, 2, ProjectileType<projectiles.Techproj>(), 0, 3, Main.myPlayer);
                            }
                        }
                        npc.ai[3] = 25;
                    }
                }
                if (npc.Distance(newTargetCenter) > 325f)
                {
                    // Out targeted player seems to have left our range, so we'll go back to sleep.
                    Phase = (int)AIStates.Approach;
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
                    npc.life = 0;
                }
            }
            int x = (int)(npc.Center.X + (((npc.width / 2) + 10) * npc.direction)) / 16;
            int y = (int)(npc.Center.Y + ((npc.height / 2) * npc.direction) - 2) / 16;

            if (Main.tile[x, y].active() && Main.tile[x, y].nactive() && Main.tileSolid[Main.tile[x, y].type] && GlobalTimer % 25 == 0)
            {
                int i = 1;
                if (i == 1 && npc.velocity.X != 0 && GlobalTimer % 50 == 0)
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
                Phase = (int)AIStates.Death;
                if (RaidWorld.RaidEvent)
                {
                    NetMessage.SendData(MessageID.WorldData);
                    RaidWorld.RaidKillCount += 1f;
                }
            }
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
            origin.X = (float)(npc.spriteDirection == 1 ? sourceRectangle.Width - 28 : 28);

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
                if (GlobalTimer <= 500)
                {
                    if (++npc.frameCounter % 7 == 0)
                        npc.frame.Y = (npc.frame.Y / frameHeight + 1) % ((Main.npcFrameCount[npc.type] / 2) + 1) * frameHeight;
                }
                else
                {
                    npc.frame.Y = Frame_Walk * frameHeight;
                }
            }
            if (Phase == (int)AIStates.Approach)
            {
                npc.frameCounter++;
                if (++npc.frameCounter % 7 == 0)
                    npc.frame.Y = (npc.frame.Y / frameHeight + 1) % ((Main.npcFrameCount[npc.type] / 2) + 1) * frameHeight;
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
            if (Phase == (int)AIStates.Death)
            {
                npc.frame.Y = Frame_Walk * frameHeight;
            }
        }
    }
}