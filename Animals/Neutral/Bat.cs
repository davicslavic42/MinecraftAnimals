using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MinecraftAnimals.Animals.Neutral
{
    public class Bat : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bat");
            Main.npcFrameCount[npc.type] = 7;
        }

        public override void SetDefaults()
        {
            npc.noGravity = true;
            npc.width = 25;
            npc.height = 35;
            npc.lifeMax = 100;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.aiStyle = -1;
            npc.scale = 0.70f;
            npc.friendly = true;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnCondition.Cavern.Chance * 0.08f;
        }
        internal enum AIStates
        {
            Normal = 0,
            Fly = 1,
            Rest = 2,
            Death = 3
        }
        internal ref float GlobalTimer => ref npc.ai[0];
        internal ref float Phase => ref npc.ai[1];
        internal ref float ActionPhase => ref npc.ai[2];
        internal ref float AttackTimer => ref npc.ai[3];

        public override void AI()
        {
            int flyCount = 0;
            GlobalTimer++;
            float inertia = 15f;
            if (Phase == (int)AIStates.Normal)
            {
                npc.velocity.X = 1.5f * npc.direction;
                if (GlobalTimer % 75 == 0)
                {
                    flyCount += 1;
                    npc.direction = Main.rand.Next(2) == 1 ? npc.direction = 1 : npc.direction = -1;
                }
                if (GlobalTimer % 30 == 0 && GlobalTimer > 160)
                {
                    npc.velocity.Y = Main.rand.Next(2) == 1 ? npc.velocity.Y = GlobalTimer * (inertia - 1) / inertia : npc.velocity.Y = GlobalTimer * (inertia - 1) / inertia * -1f;//GlobalTimer * 0.05f / 20f
                }
                if (GlobalTimer > 300) GlobalTimer = 50;
                if (flyCount == 5)
                {
                    Phase = (int)AIStates.Fly;
                    flyCount = 0;
                }
            }

            if (Phase == (int)AIStates.Fly)// bat flies upward until it finds a celing block to rest on
            {
                int x = (int)(npc.Center.X) / 16;
                int y = (int)(npc.Center.Y - ((npc.height / 2) + 1)) / 16;
                npc.velocity.Y = -1.95f;
                npc.velocity.X = 0f * npc.direction;
                if (Main.tile[x, y].active() && Main.tile[x, y].nactive() && Main.tileSolid[Main.tile[x, y].type])
                {
                    Phase = (int)AIStates.Rest;
                    GlobalTimer = 0;
                }
            }
            if (Phase == (int)AIStates.Rest)
            {
                npc.velocity.X = 0f * npc.direction;
                npc.velocity.Y = 0f;
                if (GlobalTimer >= 450)
                {
                    npc.velocity = new Vector2(npc.direction * 1, 7f);
                    Phase = (int)AIStates.Normal;
                    GlobalTimer = 0;
                }
            }
            if (Phase == (int)AIStates.Death)
            {
                npc.noGravity = true;
                npc.damage = 0;
                npc.ai[2] += 1f; // increase our death timer.
                npc.netUpdate = true;
                npc.velocity.Y = 0;
                npc.velocity.X = 0;
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
            origin.X = (float)(npc.spriteDirection == 1 ? sourceRectangle.Width - 15 : 15);

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
        private const int Frame_Float = 0;
        private const int Frame_Float_2 = 1;
        private const int Frame_Float_3 = 2;
        private const int Frame_Float_4 = 3;
        private const int Frame_Float_5 = 4;
        private const int Frame_Float_6 = 5;
        private const int Frame_Rest = 6;
        public override void FindFrame(int frameHeight)
        {
            // This makes the sprite flip horizontally in conjunction with the npc.direction.
            npc.spriteDirection = npc.direction;
            if (Phase == (int)AIStates.Normal || (Phase == (int)AIStates.Fly))
            {
                npc.frameCounter++;
                if (++npc.frameCounter % 7 == 0)
                    npc.frame.Y = (npc.frame.Y / frameHeight + 1) % (Main.npcFrameCount[npc.type] - 1) * frameHeight;
            }
            if (Phase == (int)AIStates.Rest)
            {
                npc.frameCounter++;
                npc.frame.Y = Frame_Rest * frameHeight;
            }
            if (Phase == (int)AIStates.Death)
            {
                npc.frame.Y = Frame_Float * frameHeight;
            }
        }
    }
}

