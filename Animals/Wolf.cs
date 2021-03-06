using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MinecraftAnimals.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MinecraftAnimals.Animals
{
    public class Wolf : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wolf");
            Main.npcFrameCount[npc.type] = 16;
        }
        public override void SetDefaults()
        {
            npc.width = 56;
            npc.height = 30;
            npc.lifeMax = 50;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.aiStyle = -1;
            npc.friendly = true;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnCondition.OverworldDay.Chance * 0.05f;
        }
        internal enum AIStates
        {
            Passive = 0,
            Attack = 1,
            Death = 2,
            Follow = 3
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
                npc.friendly = true;
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
                npc.dontTakeDamage = false;
                npc.TargetClosest(true);
                npc.damage = 30;
                npc.velocity.X = 1.42f * npc.direction;
                AttackTimer++;
                if (player.Distance(npc.Center) > 625f)
                {
                    npc.ai[3] = 10;
                    Phase = (int)AIStates.Passive;
                }
            }
            if (Phase == (int)AIStates.Follow)
            {
                npc.TargetClosest(true);
                npc.velocity.X = 1.25f * npc.direction;
                if (player.Distance(npc.Center) < 45f)
                {
                    npc.velocity.X = 0;
                }
                if (player.HeldItem.type != ItemType<Items.Materials.Bone>())
                {
                    Phase = (int)AIStates.Passive;
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
            if (npc.ai[3] == -10 && npc.life > npc.life * 0.05)
            {
                Phase = (int)AIStates.Attack;
                npc.friendly = false;
            }
            if (player.HeldItem.type == ItemType<Items.Materials.Bone>() && npc.ai[3] != -10/* ai[3] == -10  goes hostile*/ && Phase != (int)AIStates.Death) Phase = (int)AIStates.Follow; //if player is holding a bone and dog is not dead nor hostile start to follow

            int x = (int)(npc.Center.X + (((npc.width / 2) + 8) * npc.direction)) / 16;
            int y = (int)(npc.Center.Y + ((npc.height / 2) * npc.direction) - 1) / 16;

            if (Main.tile[x, y].active() && Main.tile[x, y].nactive() && Main.tileSolid[Main.tile[x, y].type] && GlobalTimer % 50 == 0)//autojump tile detection
            {
                int i = 1;
                if (i == 1 && npc.velocity.X != 0)
                {
                    npc.velocity = new Vector2(npc.direction * 1, -7f);
                    i = 0;
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
                    if (N.active && N.Distance(npc.Center) < 325f && N.ai[1] != (int)AIStates.Death && (N.type == NPCType<Wolf>()))
                    {
                        N.netUpdate = true;
                        N.target = npc.target;
                        N.ai[3] = -10;
                    }
                }
            }
            // Thanks Joost
        }
        // The npc starts in the asleep state, waiting for a player to enter range
        // Our texture is 32x32 with 2 pixels of padding vertically, so 34 is the vertical spacing.  These are for my benefit and the numbers could easily be used directly in the code below, but this is how I keep code organized.
        private const int Frame_Walk = 0;
        private const int Frame_Walk_2 = 1;
        private const int Frame_Walk_3 = 2;
        private const int Frame_Walk_4 = 3;
        private const int Frame_Walk_5 = 4;
        private const int Frame_Walk_6 = 5;
        private const int Frame_Walk_7 = 6;
        private const int Frame_Walk_8 = 7;
        private const int Frame_Angry = 8;
        private const int Frame_Angry_2 = 9;
        private const int Frame_Angry_3 = 10;
        private const int Frame_Angry_4 = 11;
        private const int Frame_Angry_5 = 12;
        private const int Frame_Angry_6 = 13;
        private const int Frame_Angry_7 = 14;
        private const int Frame_Angry_8 = 15;

        // Here in FindFrame, we want to set the animation frame our npc will use depending on what it is doing.
        // We set npc.frame.Y to x * frameHeight where x is the xth frame in our spritesheet, counting from 0. For convenience, I have defined some consts above.
        public override void FindFrame(int frameHeight)
        {
            // This makes the sprite flip horizontally in conjunction with the npc.direction.
            Player player = Main.player[npc.target];
            npc.spriteDirection = npc.direction;
            if (Phase == (int)AIStates.Passive)
            {
                npc.frameCounter++;
                if (GlobalTimer <= 500)
                {
                    if (++npc.frameCounter % 7 == 0)
                        npc.frame.Y = (npc.frame.Y / frameHeight + 1) % (Main.npcFrameCount[npc.type] / 2) * frameHeight;
                }
                else
                {
                    npc.frame.Y = Frame_Walk * frameHeight;
                }
            }
            if (Phase == (int)AIStates.Follow)
            {
                npc.frameCounter++;
                if (player.Distance(npc.Center) < 45f)
                {
                    npc.frame.Y = Frame_Walk * frameHeight;
                }
                else
                {
                    if (++npc.frameCounter % 7 == 0)
                        npc.frame.Y = (npc.frame.Y / frameHeight + 1) % (Main.npcFrameCount[npc.type] / 2) * frameHeight;
                }
            }
            if (Phase == (int)AIStates.Attack)
            {
                npc.frameCounter++;
                if (npc.frameCounter < 7)
                {
                    npc.frame.Y = Frame_Angry * frameHeight;
                }
                else if (npc.frameCounter < 14)
                {
                    npc.frame.Y = Frame_Angry_2 * frameHeight;
                }
                else if (npc.frameCounter < 21)
                {
                    npc.frame.Y = Frame_Angry_3 * frameHeight;
                }
                else if (npc.frameCounter < 28)
                {
                    npc.frame.Y = Frame_Angry_4 * frameHeight;
                }
                else if (npc.frameCounter < 35)
                {
                    npc.frame.Y = Frame_Angry_5 * frameHeight;
                }
                else if (npc.frameCounter < 42)
                {
                    npc.frame.Y = Frame_Angry_6 * frameHeight;
                }
                else if (npc.frameCounter < 49)
                {
                    npc.frame.Y = Frame_Angry_7 * frameHeight;
                }
                else if (npc.frameCounter < 56)
                {
                    npc.frame.Y = Frame_Angry_8 * frameHeight;
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