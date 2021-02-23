using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MinecraftAnimals.Raid.Illagers
{
    public class Vex : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vex");
            Main.npcFrameCount[npc.type] = 12;
        }
        public override void SetDefaults()
        {
            npc.width = 30;
            npc.height = 60;
            npc.lifeMax = 20;
            npc.damage = 25;
            npc.knockBackResist = 0.5f;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.aiStyle = -1;
            npc.value = 35f;
            npc.scale = 0.65f;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnCondition.Overworld.Chance * 0;
        }
        internal enum AIStates
        {
            Normal = 0,
            Attack = 1,
            Charge = 2,
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
            Vector2 TownTargets = GeneralMethods.GetAnyTownNpcTargetEntity(npc.Center, 605f);//gets target center, town npcs in this case
            Vector2 PlayerTarget = GeneralMethods.GetTargetPlayerEntity(npc.Center, 605f);//gets player center
            Vector2 newTargetCenter = npc.Distance(PlayerTarget) > npc.Distance(TownTargets) ? TownTargets : PlayerTarget;
            int SpeedLim = 1;

            if (Phase == (int)AIStates.Normal)
            {
                SpeedLim = 1;
                npc.velocity.Y = -0.15f;
                npc.TargetClosest(false);
                npc.velocity.X = 1 * npc.direction;
                if (GlobalTimer == 5) npc.direction = Main.rand.Next(2) == 1 ? npc.direction = 1 : npc.direction = -1;// random direction for passive movement
                float walkOrPause = GlobalTimer <= 500 ? npc.velocity.X = 0.75f * npc.direction : npc.velocity.X = 0 * npc.direction;
                if (GlobalTimer >= 800) GlobalTimer = 0;
                if (npc.Distance(newTargetCenter) < 605f)
                {
                    Phase = (int)AIStates.Attack;
                    GlobalTimer = 0;
                }
            }
            if (Phase == (int)AIStates.Attack)
            {
                npc.direction = npc.Center.X > newTargetCenter.X ? npc.direction = -1 : npc.direction = 1;
                npc.velocity.X = 1.5f * npc.direction;
                SpeedLim = 1;
                if (npc.Distance(newTargetCenter) > 605f)
                {
                    Phase = (int)AIStates.Normal;
                    GlobalTimer = 0;
                }
                if (npc.Distance(newTargetCenter) < 250f && GlobalTimer > 205)
                {
                    Phase = (int)AIStates.Charge;
                    GlobalTimer = 0;
                }
            }
            // In this state, a player has been targeted
            if (Phase == (int)AIStates.Charge)
            {
                npc.direction = npc.Center.X > newTargetCenter.X ? npc.direction = -1 : npc.direction = 1;
                npc.velocity.X = 3.25f * npc.direction;
                SpeedLim = 2;
                AttackTimer++;
                npc.TargetClosest(true);
                if (AttackTimer > 175)
                {
                    // Out targeted player seems to have left our range, so we'll go back to sleep.
                    Phase = (int)AIStates.Attack;
                    AttackTimer = 0;
                    GlobalTimer = 0;
                }
            }
            if (Phase == (int)AIStates.Death)
            {
                SpeedLim = 2;
                npc.noGravity = true;
                npc.damage = 0;
                npc.ai[2] += 1f; // increase our death timer.
                npc.netUpdate = true;
                npc.velocity.X = 0;
                npc.velocity.Y = 0;
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
            //thanks nuova prime//
            if (newTargetCenter.Y < npc.position.Y + 25)
            {
                npc.velocity.Y -= npc.velocity.Y > 0f ? 0.75f : .4f;
            }
            if (newTargetCenter.Y > npc.position.Y + 25)
            {
                npc.velocity.Y += npc.velocity.Y < 0f ? 0.75f : .45f;
            }
            if (SpeedLim == 1)//prevents the velocity from going above inputed number
            {
                if (npc.velocity.X * npc.direction > 1.9f)
                {
                    npc.velocity.X = 1.9f * npc.direction;
                }
                if (npc.velocity.Y > 1.5f)
                {
                    npc.velocity.Y = 1.5f * npc.direction;
                }
            }
            else
            {
                if (npc.velocity.X * npc.direction > 3.5f)
                {
                    npc.velocity.X = 3.5f * npc.direction;
                }
                if (npc.velocity.Y > 3.5f)
                {
                    npc.velocity.Y = 3.5f * npc.direction;
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
            origin.X = (float)(npc.spriteDirection == 1 ? sourceRectangle.Width - 30 : 30);

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
        private const int Frame_Walk_6 = 5;
        private const int Frame_Attack = 6;
        private const int Frame_Attack_2 = 7;
        private const int Frame_Attack_3 = 8;
        private const int Frame_Attack_4 = 9;
        private const int Frame_Attack_5 = 10;
        private const int Frame_Attack_6 = 11;
        public override void FindFrame(int frameHeight)
        {
            // This makes the sprite flip horizontally in conjunction with the npc.direction.
            npc.spriteDirection = npc.direction;
            if (Phase == (int)AIStates.Normal)
            {
                if (++npc.frameCounter % 7 == 0)
                    npc.frame.Y = (npc.frame.Y / frameHeight + 1) % ((Main.npcFrameCount[npc.type]) / 2) * frameHeight;
            }
            if (Phase == (int)AIStates.Attack)
            {
                if (++npc.frameCounter % 7 == 0)
                    npc.frame.Y = (npc.frame.Y / frameHeight + 1) % ((Main.npcFrameCount[npc.type]) / 2) * frameHeight;
            }
            if (Phase == (int)AIStates.Charge)
            {
                npc.frameCounter++;
                if (npc.frameCounter < 6)
                {
                    npc.frame.Y = Frame_Attack * frameHeight;
                }
                else if (npc.frameCounter < 12)
                {
                    npc.frame.Y = Frame_Attack_2 * frameHeight;
                }
                else if (npc.frameCounter < 17)
                {
                    npc.frame.Y = Frame_Attack_3 * frameHeight;
                }
                else if (npc.frameCounter < 24)
                {
                    npc.frame.Y = Frame_Attack_4 * frameHeight;
                }
                else if (npc.frameCounter < 30)
                {
                    npc.frame.Y = Frame_Attack_5 * frameHeight;
                }
                else if (npc.frameCounter < 38)
                {
                    npc.frame.Y = Frame_Attack_6 * frameHeight;
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