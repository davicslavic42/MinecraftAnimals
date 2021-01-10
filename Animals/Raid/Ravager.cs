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
using System.Collections.Generic;
using MinecraftAnimals.Items.Weapons;



namespace MinecraftAnimals.Animals.Raid
{
    public class Ravager : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ravager");
            Main.npcFrameCount[npc.type] = 11;
        }
        public override void SetDefaults()
        {
            npc.width = 80;
            npc.height = 60;
            npc.lifeMax = 195;
            npc.damage = 38;
            npc.knockBackResist = 0f;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.aiStyle = -1;
            npc.value = 35f;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (MCAWorld.RaidEvent)
            {
                return 10f;
            }
            return SpawnCondition.Overworld.Chance * 0f;
        }
        // These const ints are for the benefit of the programmer. Organization is key to making an AI that behaves properly without driving you crazy.
        // Here I lay out what I will use each of the 4 npc.ai slots for.
        internal enum AIStates
        {
            Normal = 0,
            Attack = 1,
            Death = 2,
            Roar = 3
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
            npc.TargetClosest(true);
            if (Phase == (int)AIStates.Normal)
            {
                float isMoving = GlobalTimer <= 500 ? npc.velocity.X = 1 * npc.direction : npc.velocity.X = 0 * npc.direction; //basic passive movement for 500 ticks then stationary 300
                if (GlobalTimer >= 800)
                {
                    GlobalTimer = 0;
                }

                if (npc.HasValidTarget && player.Distance(npc.Center) < 730f) // passive player is within a certain range
                {
                    Phase = (int)AIStates.Attack;
                    GlobalTimer = 0;
                }
            }
            if (Phase == (int)AIStates.Attack)
            {
                npc.TargetClosest(true);
                npc.velocity.X = 1.75f * npc.direction;
                AttackTimer++;
                if (player.Distance(npc.Center) > 730f){
                    Phase = (int)AIStates.Normal;
                }
                if (AttackTimer >= 200 && player.Distance(npc.Center) < 250f)
                {
                    npc.velocity.X = 0 * npc.direction;
                    npc.velocity = new Vector2(npc.direction * 10f, -3f);
                }
                else{
                    float stopToAttack = player.Distance(npc.Center) < 50f ? npc.velocity.X = 0 * npc.direction : 1.75f * npc.direction; //as the name suggests as the player gets close enough it stops moving to attack
                }
                if(AttackTimer >= 210)
                {
                    AttackTimer = 0;
                }
            }
            if (Phase == (int)AIStates.Roar)
            {
                npc.velocity.X = 0f * npc.direction;
                if(GlobalTimer >= 55)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        int dustIndex = Dust.NewDust(new Vector2(npc.position.X + (((npc.width / 2) + 32) * npc.direction), npc.position.Y), npc.width, npc.height, DustType<Dusts.Poof>(), 0f, 0f, 100, default(Color), 1f); //spawns ender dust
                        Main.dust[dustIndex].noGravity = true;
                    }
                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, npc.velocity.X, npc.velocity.Y, ProjectileType<projectiles.RavagerRoar>(), 1, 10f, Main.myPlayer, 600f);
                    Phase = (int)AIStates.Attack;
                    AttackTimer = 0;
                }
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
                npc.rotation = GeneralMethods.ManualMobRotation(npc.rotation, MathHelper.ToRadians(180f), 14f);
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
            int x = (int)(npc.Center.X + (((npc.width / 2) + 25) * npc.direction)) / 16;
            int y = (int)(npc.Center.Y + ((npc.height / 2) * npc.direction) - 1) / 16;

            if (Main.tile[x, y].active() && Main.tile[x, y].nactive() && Main.tileSolid[Main.tile[x, y].type])
            {
                int i = 1;
                if (i == 1 && npc.velocity.X != 0)
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
                MCAWorld.RaidKillCount += 2;
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
            if(Main.rand.Next(0,5) == 1 && GlobalTimer > 150)
            {
                Phase = (int)AIStates.Roar;
                GlobalTimer = 0;
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
            origin.X = (float)(npc.spriteDirection == 1 ? sourceRectangle.Width - 85 : 85);

            Color drawColor = npc.GetAlpha(lightColor);
            if (Phase == (int)AIStates.Death)
            {
                Main.spriteBatch.Draw(texture, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY - 10),
                sourceRectangle, Color.Red * 0.8f, npc.rotation, origin, npc.scale, spriteEffects, 0f);
            }
            else
            {
                Main.spriteBatch.Draw(texture, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY - 20),
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
        public override void FindFrame(int frameHeight)
        {
            int i = 1;
            // This makes the sprite flip horizontally in conjunction with the npc.direction.
            npc.spriteDirection = npc.direction;
            if (Phase == (int)AIStates.Normal)
            {
                npc.frameCounter++;
                if (GlobalTimer <= 500)
                {
                    if (++npc.frameCounter % 11 == 0)
                        npc.frame.Y = (npc.frame.Y / frameHeight + 1) % ((Main.npcFrameCount[npc.type]) - 5) * frameHeight;
                }
                else
                {
                    npc.frame.Y = Frame_Walk * frameHeight;
                }
            }
            if (Phase == (int)AIStates.Attack)
            {
                npc.frameCounter++;
                    if (++npc.frameCounter % 11 == 0)
                        npc.frame.Y = (npc.frame.Y / frameHeight + 1) % ((Main.npcFrameCount[npc.type]) - 5) * frameHeight;
            }
            if (Phase == (int)AIStates.Roar)
            {
                npc.frameCounter++;
                if (npc.frameCounter < 14)
                {
                    npc.frame.Y = Frame_Walk * frameHeight;
                }
                else if (npc.frameCounter < 28)
                {
                    npc.frame.Y = Frame_Attack * frameHeight;
                }
                else if (npc.frameCounter < 50)
                {
                    npc.frame.Y = Frame_Attack_2 * frameHeight;
                }
                else
                {
                    npc.frameCounter = 0;
                }
            }
            if ( npc.HasValidTarget && Main.player[npc.target].Distance(npc.Center) < 75f)
            {
                npc.frameCounter++;
                if (npc.frameCounter < 11)
                {
                    npc.frame.Y = Frame_Attack * frameHeight;
                }
                else if (npc.frameCounter < 22)
                {
                    npc.frame.Y = Frame_Attack_2 * frameHeight;
                }
                else if (npc.frameCounter < 33)
                {
                    npc.frame.Y = Frame_Attack_3 * frameHeight;
                }
                else if (npc.frameCounter < 44)
                {
                    npc.frame.Y = Frame_Attack_4 * frameHeight;
                }
                else if (npc.frameCounter < 55)
                {
                    npc.frame.Y = Frame_Attack_5 * frameHeight;
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