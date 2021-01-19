using System.Linq;
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
namespace MinecraftAnimals.Animals
{
    public class Blaze : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blaze");
            Main.npcFrameCount[npc.type] = 3;
        }
        public override void SetDefaults()
        {
            npc.width = 32;
            npc.height = 44;
            npc.lifeMax = 98;
            npc.damage = 25;
            npc.lavaImmune = true;
            npc.knockBackResist = 1f;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.aiStyle = -1;
            npc.value = 35f;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnCondition.Underworld.Chance * 0.1f;
        }
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
        bool flying = false;

        public override void AI()
        {
            Collision.StepUp(ref npc.position, ref npc.velocity, npc.width, npc.height, ref npc.stepSpeed, ref npc.gfxOffY);
            GlobalTimer++;
            Player player = Main.player[npc.target];
            if (Phase == (int)AIStates.Normal)
            {
                flying = false;
                npc.velocity.Y = 0.25f;
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
                flying = true;
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
                    AttackTimer = 0;
                }
            }
            // In this state, a player has been targeted
            if (Phase == (int)AIStates.Shoot)
            {
                flying = true;
                AttackTimer++;
                npc.TargetClosest(true);
                npc.velocity.X = 0f * npc.direction;
                if (AttackTimer == 180 || AttackTimer == 200 || AttackTimer == 220) //Check three states of AI_Timer, this will result in 3 shots with a delay of 15 frames
                {
                    Player TargetPlayer = Main.player[(int)Player.FindClosest(npc.position, npc.width, npc.height)];
                    _ = npc.Distance(npc.position) - 50;
                    Vector2 PlayerDir = npc.DirectionTo(TargetPlayer.position);
                    Vector2 DirToRing = npc.DirectionTo(TargetPlayer.position + PlayerDir.RotatedBy(0.001f) * -75f);

                    npc.velocity.X += DirToRing.X;
                    npc.velocity.Y += DirToRing.Y;

                    Main.PlaySound(SoundID.Item20, npc.position); //We play a sound at the NPC's position for feedback for each shot

                    Projectile.NewProjectile(npc.Center, PlayerDir.RotatedByRandom(0.15f) * 7.5f, ProjectileType<projectiles.FireCharge>(), 15, 2, Main.LocalPlayer.whoAmI); //Multiply velocity with a larger number for more speed
                }
                if (AttackTimer == 230)
                {
                    AttackTimer = 0;
                }
                if (!npc.HasValidTarget || player.Distance(npc.Center) > 450f)
                {
                    // Out targeted player seems to have left our range, so we'll go back to sleep.
                    Phase = (int)AIStates.Attack;
                    AttackTimer = 0;
                }
                // If the targeted player is in attack range (250).
            }
            // In this state, we are in the Shoot. 
            if (Phase == (int)AIStates.Death)
            {
                flying = false;
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
            if (flying == true)
            {
                //thanks nuova prime//
                if (player.position.Y < npc.position.Y + 130)
                {
                    npc.velocity.Y -= npc.velocity.Y > 0f ? 1f : .4f;
                }
                if (player.position.Y > npc.position.Y + 130)
                {
                    npc.velocity.Y += npc.velocity.Y < 0f ? 1f : .2f;
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
            origin.X = (float)(npc.spriteDirection == 1 ? sourceRectangle.Width - 15 : 15);

            Color drawColor = npc.GetAlpha(lightColor);
            if (Phase == (int)AIStates.Death)
            {
                Main.spriteBatch.Draw(texture, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY + 20),
                sourceRectangle, Color.Red * 0.8f, npc.rotation, origin, npc.scale, spriteEffects, 0f);
            }
            else
            {
                Main.spriteBatch.Draw(texture, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY ),
                sourceRectangle, drawColor, npc.rotation, origin, npc.scale, spriteEffects, 0f);
            }
            return false;
        }
        private const int Frame_Float = 0;
        private const int Frame_Float_2 = 1;
        private const int Frame_Float_3 = 2;
        public override void FindFrame(int frameHeight)
        {
            // This makes the sprite flip horizontally in conjunction with the npc.direction.
            npc.spriteDirection = npc.direction;
            if (Phase == (int)AIStates.Death)
            {
                npc.frame.Y = Frame_Float * frameHeight;
            }
            else
            {
                npc.frameCounter++;
                if (++npc.frameCounter % 7 == 0)
                    npc.frame.Y = (npc.frame.Y / frameHeight + 1) % (Main.npcFrameCount[npc.type]) * frameHeight;
            }
        }
    }
}