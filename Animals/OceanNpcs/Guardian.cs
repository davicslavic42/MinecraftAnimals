using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MinecraftAnimals.Animals.OceanNpcs
{
    public class Guardian : ModNPC
    {
        public override string Texture => "Assets/NPCTextures/Ocean/Guardian";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Guardian");
            Main.npcFrameCount[npc.type] = 6;
        }
        public override void SetDefaults()
        {
            npc.noGravity = true;
            npc.width = 48;
            npc.height = 12;
            npc.lifeMax = 10;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.aiStyle = -1;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnCondition.Ocean.Chance * 0.04f;
        }
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
            Vector2 PlayerTarget = GeneralMethods.GetTargetPlayerEntity(npc.Center, 710f);//gets player center
            npc.direction = npc.Center.X > PlayerTarget.X ? npc.direction = -1 : npc.direction = 1;
            GlobalTimer++;
            if (Phase == (int)AIStates.Normal)
            {
                npc.velocity.X = 2.25f * npc.direction;
                if (GlobalTimer % 50 == 0 && GlobalTimer > 250)
                {
                    npc.velocity.Y = Main.rand.Next(3) == 1 ? npc.velocity.Y = GlobalTimer / 150f * 0.85f * -1f : npc.velocity.Y = GlobalTimer / 150f * 0.85f;
                    npc.direction = Main.rand.Next(2) == 1 ? npc.direction = 1 : npc.direction = -1;
                    GlobalTimer = 0;
                }
                if(npc.Distance(PlayerTarget) < 700f) Phase = (int)AIStates.Attack;
            }
            if (Phase == (int)AIStates.Attack)
            {
                int flee = 0;
                Vector2 TargetDir = Vector2.Normalize(PlayerTarget - npc.Center);
                npc.velocity.X = 0f * npc.direction;
                npc.rotation = TargetDir.ToRotation();
                Projectile.NewProjectile(npc.Center, TargetDir, ProjectileType<projectiles.GuardianLaser>(), 10, 2);
            }
            if (Phase == (int)AIStates.Death)
            {
                npc.velocity.Y = 0;
                npc.velocity.X = 0;
                npc.damage = 0;
                npc.ai[2] += 1f; // increase our death timer.
                npc.netUpdate = true;
                npc.velocity.Y = 0;
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
            if (!npc.wet) npc.velocity.Y = 1.5f;
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
            Texture2D eyeTexture = ModContent.GetTexture("GuardianEye");
            Vector2 eyeposition = new Vector2(npc.Center.X + ((npc.width / 2) + 4), npc.Center.Y * 0.5f);

            int frameHeight = Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type];
            int startY = npc.frame.Y;
            int startX = texture.Width / 2;
            Rectangle sourceRectangle = new Rectangle(0, startY, startX, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            origin.X = (float)(npc.spriteDirection == 1 ? sourceRectangle.Width - 25 : 25);

            Color drawColor = npc.GetAlpha(lightColor);
            if (Phase == (int)AIStates.Death)
            {
                Main.spriteBatch.Draw(texture, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY),
                sourceRectangle, Color.Red * 0.8f, npc.rotation, origin, npc.scale, spriteEffects, 0f);
            }
            if (Phase == (int)AIStates.Normal || Phase == (int)AIStates.Attack)
            {
                Main.spriteBatch.Draw(texture, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY),
                sourceRectangle, drawColor, npc.rotation, origin, npc.scale, spriteEffects, 0f);
            }
            if (Phase == (int)AIStates.Attack && npc.direction == 1)
            {
                Main.spriteBatch.Draw(texture, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY),
                sourceRectangle, drawColor, npc.rotation, origin, npc.scale, SpriteEffects.FlipVertically, 0f);
            }
            Main.spriteBatch.Draw(eyeTexture, eyeposition - Main.screenPosition + new Vector2(0f, npc.gfxOffY),
            sourceRectangle, drawColor, npc.rotation, origin, npc.scale, SpriteEffects.FlipVertically, 0f);

            return false;
        }
        private const int Frame_Walk = 0;
        private const int Frame_Walk_2 = 1;
        private const int Frame_Walk_3 = 2;
        private const int Frame_Walk_4 = 3;
        private const int Frame_Walk_5 = 4;
        private const int Frame_Walk_6 = 5;
        public override void FindFrame(int frameHeight)
        {
            // This makes the sprite flip horizontally in conjunction with the npc.direction.
            npc.spriteDirection = npc.direction;
            if (Phase == (int)AIStates.Death)
            {
                npc.frame.Y = Frame_Walk * frameHeight;
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