using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MinecraftAnimals.Animals
{
    public class Bee : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bee");
            Main.npcFrameCount[npc.type] = 8;
        }

        public override void SetDefaults()
        {
            npc.width = 34;
            npc.height = 30;
            npc.lifeMax = 100;
            npc.damage = 0;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.aiStyle = -1;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnCondition.OverworldDay.Chance * 0.09f;
        }
        internal enum AIStates
        {
            Passive = 0,
            Attack = 1,
            Death = 2,
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
                npc.friendly = false;
                npc.dontTakeDamage = false;
                npc.TargetClosest(true);
                npc.damage = 30;
                npc.velocity.X = 1.45f * npc.direction;
                if (player.Distance(npc.Center) > 625f)
                {
                    npc.ai[3] = 10;
                    Phase = (int)AIStates.Passive;
                }
            }
            if (Phase == (int)AIStates.Death)
            {
                if (npc.ai[3] != -10)
                {
                    npc.damage = 0;
                    npc.ai[2] += 1f; // increase our death timer.
                    npc.netUpdate = true;
                    npc.velocity.X = 0;
                    npc.velocity.Y += 1.5f;
                    npc.dontTakeDamage = true;
                    npc.rotation = GeneralMethods.ManualMobRotation(npc.rotation, MathHelper.ToRadians(180f), 16f);
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
            }
            if (npc.ai[3] == -10 && npc.life > npc.life * 0.05)
            {
                Phase = (int)AIStates.Attack;
            }
            int x = (int)(npc.Center.X) / 16;
            int y = (int)(npc.Center.Y + ((npc.height / 2) - 6)) / 16;

            if (Main.tile[x, y].active() && Main.tile[x, y].nactive() && Main.tileSolid[Framing.GetTileSafely(x, y).type])
            {
                npc.velocity.Y -= 0.1f;
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
            origin.X = (float)(npc.spriteDirection == 1 ? sourceRectangle.Width - 10 : 10);

            Color drawColor = npc.GetAlpha(lightColor);
            if (Phase == (int)AIStates.Death)
            {
                Main.spriteBatch.Draw(texture, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY),
                sourceRectangle, Color.Red * 0.8f, npc.rotation, origin, npc.scale, spriteEffects, 0f);
            }
            else
            {
                Main.spriteBatch.Draw(texture, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY),
                sourceRectangle, drawColor, npc.rotation, origin, npc.scale, spriteEffects, 0f);
            }
            return false;
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                npc.ai[3] = 10;
                npc.netUpdate = true;
                GlobalTimer = 0;
                npc.life = 1;
                Phase = (int)AIStates.Death;
            }
            if (Phase != (int)AIStates.Death && npc.life > npc.life * 0.1)
            {
                for (int n = 0; n < 150; n++)
                {
                    NPC N = Main.npc[n];
                    if (N.active && N.Distance(npc.Center) < 325f && N.ai[1] != (int)AIStates.Death && (N.type == NPCType<Bee>()))
                    {
                        N.netUpdate = true;
                        N.target = npc.target;
                        N.ai[3] = -10;
                    }
                }
            }
            // Thanks Joost
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            base.OnHitPlayer(target, damage, crit);
            target.AddBuff(BuffID.Poisoned, 675);
        }
        private const int Frame_Float = 0;
        private const int Frame_Float_2 = 1;
        private const int Frame_Float_3 = 2;
        private const int Frame_Float_4 = 3;
        private const int Frame_AngryFloat = 4;
        private const int Frame_AngryFloat_2 = 5;
        private const int Frame_AngryFloat_3 = 6;
        private const int Frame_AngryFloat_4 = 7;
        public override void FindFrame(int frameHeight)
        {
            int i = 1;
            // This makes the sprite flip horizontally in conjunction with the npc.direction.
            npc.spriteDirection = npc.direction;
            if (Phase == (int)AIStates.Passive)
            {
                npc.frameCounter++;
                if (++npc.frameCounter % 7 == 0)
                    npc.frame.Y = (npc.frame.Y / frameHeight + 1) % (Main.npcFrameCount[npc.type] / 2) * frameHeight;
            }
            if (Phase == (int)AIStates.Attack)
            {
                npc.frameCounter++;
                if (npc.frameCounter < 7)
                {
                    npc.frame.Y = Frame_AngryFloat * frameHeight;
                }
                else if (npc.frameCounter < 14)
                {
                    npc.frame.Y = Frame_AngryFloat_2 * frameHeight;
                }
                else if (npc.frameCounter < 21)
                {
                    npc.frame.Y = Frame_AngryFloat_3 * frameHeight;
                }
                else if (npc.frameCounter < 28)
                {
                    npc.frame.Y = Frame_AngryFloat_4 * frameHeight;
                }
                else
                {
                    npc.frameCounter = 0;
                }
            }
            if (Phase == (int)AIStates.Death)
            {
                npc.frame.Y = Frame_Float * frameHeight;
            }
        }
    }
}
