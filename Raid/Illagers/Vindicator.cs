using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MinecraftAnimals.Raid.Illagers
{
    public class Vindicator : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vindicator");
            Main.npcFrameCount[npc.type] = 13;
        }
        public override void SetDefaults()
        {
            npc.width = 30;
            npc.height = 40;
            npc.lifeMax = 105;
            npc.damage = 45;
            npc.knockBackResist = 1f;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.aiStyle = -1;
            npc.value = 35f;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return 0f;
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

            Vector2 TownTargets = GeneralMethods.GetAnyTownNpcTargetEntity(npc.Center, 635f);//gets target center
            Vector2 PlayerTarget = GeneralMethods.GetTargetPlayerEntity(npc.Center, 635f);//gets player center
            Vector2 newTargetCenter = npc.Distance(PlayerTarget) > npc.Distance(TownTargets) ? TownTargets : PlayerTarget;

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
                if (npc.Distance(newTargetCenter) < 625f)//npc.HasValidTarget && player.Distance(npc.Center) < 725f
                {
                    Phase = (int)AIStates.Attack;
                    GlobalTimer = 0;
                }
            }
            if (Phase == (int)AIStates.Attack)
            {
                npc.direction = npc.Center.X > newTargetCenter.X ? npc.direction = -1 : npc.direction = 1;
                npc.damage = 30;
                npc.velocity.X = 1.75f * npc.direction;
                if (npc.Distance(newTargetCenter) > 625f)
                {
                    Phase = (int)AIStates.Normal;
                }
                float stopToAttack = npc.Distance(newTargetCenter) < 20f ? npc.velocity.X = 0 * npc.direction : npc.velocity.X = 1.85f * npc.direction;
            }
            // thanks oli for the tile checks
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
            int x = (int)(npc.Center.X + (((npc.width / 2) + 8) * npc.direction)) / 16;
            int y = (int)(npc.Center.Y + ((npc.height / 2) * npc.direction) - 3) / 16;

            if (Main.tile[x, y].active() && Main.tile[x, y].nactive() && Main.tileSolid[Main.tile[x, y].type] && GlobalTimer % 50 == 0)
            {
                int i = 1;
                if (i == 1 && npc.velocity.X != 0 && GlobalTimer % 50 == 0)
                {
                    npc.velocity = new Vector2(npc.direction * 1, -7f);
                    i = 0;
                }
            }
        }
        public override void NPCLoot()
        {
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
            origin.X = (float)(npc.spriteDirection == 1 ? sourceRectangle.Width - 30 : 30);

            Color drawColor = npc.GetAlpha(lightColor);
            if (Phase == (int)AIStates.Death)
            {
                Main.spriteBatch.Draw(texture, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY + 10),
                sourceRectangle, Color.Red * 0.8f, npc.rotation, origin, npc.scale, spriteEffects, 0f);
            }
            else
            {
                Main.spriteBatch.Draw(texture, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY - 10),
                sourceRectangle, drawColor, npc.rotation, origin, npc.scale, spriteEffects, 0f);
            }
            return false;
        }
        private const int Frame_Idle = 0;
        private const int Frame_Walk = 1;
        private const int Frame_Walk_2 = 2;
        private const int Frame_Walk_3 = 3;
        private const int Frame_Walk_4 = 4;
        private const int Frame_Attack = 5;
        private const int Frame_Attack_2 = 6;
        private const int Frame_Attack_3 = 7;
        private const int Frame_Attack_4 = 8;
        private const int Frame_Attack_5 = 9;
        private const int Frame_Swing = 10;
        private const int Frame_Swing_2 = 11;
        private const int Frame_Swing_3 = 12;



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
                        npc.frame.Y = (npc.frame.Y / frameHeight + 1) % ((Main.npcFrameCount[npc.type]) - 7) * frameHeight;
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
                else if (npc.frameCounter < 28)
                {
                    npc.frame.Y = Frame_Attack_3 * frameHeight;
                }
                else if (npc.frameCounter < 35)
                {
                    npc.frame.Y = Frame_Attack_4 * frameHeight;
                }
                else if (npc.frameCounter < 42)
                {
                    npc.frame.Y = Frame_Attack_5 * frameHeight;
                }
                else
                {
                    npc.frameCounter = 0;
                }
            }
            if (npc.HasValidTarget && Main.player[npc.target].Distance(npc.Center) < 45f)
            {
                npc.frameCounter++;
                if (npc.frameCounter < 15)
                {
                    npc.frame.Y = Frame_Attack * frameHeight;
                }
                else if (npc.frameCounter < 20)
                {
                    npc.frame.Y = Frame_Swing * frameHeight;
                }
                else if (npc.frameCounter < 25)
                {
                    npc.frame.Y = Frame_Swing_2 * frameHeight;
                }
                else if (npc.frameCounter < 31)
                {
                    npc.frame.Y = Frame_Swing_3 * frameHeight;
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