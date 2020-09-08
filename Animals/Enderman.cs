using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace MinecraftAnimals.Animals
{
    public class Enderman : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Enderman");
            Main.npcFrameCount[npc.type] = 12;
        }
        public override void SetDefaults()
        {
            npc.width = 40;
            npc.height = 88;
            npc.lifeMax = 150;
            npc.knockBackResist = 1f;
            npc.damage = 0;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.aiStyle = -1;
            npc.value = 35f;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnCondition.Overworld.Chance * 0.1f;
        }
        // These const ints are for the benefit of the programmer. Organization is key to making an AI that behaves properly without driving you crazy.
        // Here I lay out what I will use each of the 4 npc.ai slots for.
        public enum AIStates
        {
            Passive = 0,
            Attack = 1
        }
        internal ref float GlobalTimer => ref npc.ai[0];
        internal ref float Phase => ref npc.ai[1];
        internal ref float AttackPhase => ref npc.ai[2];
        internal ref float AttackTimer => ref npc.ai[3];

        public override void AI()
        {
            Collision.StepUp(ref npc.position, ref npc.velocity, npc.width, npc.height, ref npc.stepSpeed, ref npc.gfxOffY);
            GlobalTimer++;

            if(Phase == (int)AIStates.Passive)
            {
                if (GlobalTimer == 5)
                {
                    int x = Main.rand.Next(2) == 1 ? npc.direction = 1 : npc.direction = -1;
                }
                float change = GlobalTimer <= 500 ? npc.velocity.X = 1 * npc.direction : npc.velocity.X = 0 * npc.direction;
                if (GlobalTimer >= 800)
                {
                    GlobalTimer = 0;
                }
            }
            if (Phase == (int)AIStates.Attack)
            {
                Player player = Main.player[npc.target];
                npc.TargetClosest(true);
                npc.damage = 30;
                npc.velocity.X = 2 * npc.direction;
                AttackTimer++;

                if (AttackTimer == 600)
                {
                    npc.netUpdate = true;
                    Rectangle rect = new Rectangle((int)(player.Center.X / 16), (int)(player.Center.Y / 16) , 200, 200);
                    if (RectangeIntersectsTiles(rect) == true)
                    {
                        new Vector2(Main.rand.Next(rect.Width), rect.Height).RotatedByRandom(MathHelper.TwoPi);
                    }
                    AttackTimer = 0;
                }
            }
        }
        //Thanks oli//
        private bool RectangeIntersectsTiles(Rectangle rectangle)
        {
            bool intersects = false;
            for (int i = (int)rectangle.TopLeft().X; i < (int)rectangle.TopLeft().X + rectangle.Width; i++)
            {
                for (int j = (int)rectangle.TopLeft().Y; j < (int)rectangle.TopLeft().Y + rectangle.Height; j++)
                {
                    Tile tile = Main.tile[i / 16, j / 16];
                    if (tile != null && Main.tileSolid[tile.type])
                    {
                        intersects = true;
                    }
                }
            }
            return intersects;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            Phase = (int)AIStates.Attack;
            GlobalTimer = 0;
            base.HitEffect(hitDirection, damage);
        }
        // The npc starts in the asleep state, waiting for a player to enter range
        // Our texture is 32x32 with 2 pixels of padding vertically, so 34 is the vertical spacing.  These are for my benefit and the numbers could easily be used directly in the code below, but this is how I keep code organized.
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

        // Here in FindFrame, we want to set the animation frame our npc will use depending on what it is doing.
        // We set npc.frame.Y to x * frameHeight where x is the xth frame in our spritesheet, counting from 0. For convenience, I have defined some consts above.
        public override void FindFrame(int frameHeight)
        {
            // This makes the sprite flip horizontally in conjunction with the npc.direction.
            npc.spriteDirection = npc.direction;
            if (Phase == (int)AIStates.Passive)
            {
                npc.frameCounter++;
                if (GlobalTimer <= 500)
                {
                    if (npc.frameCounter < 7)
                    {
                        npc.frame.Y = Frame_Walk * frameHeight;
                    }
                    else if (npc.frameCounter < 14)
                    {
                        npc.frame.Y = Frame_Walk_2 * frameHeight;
                    }
                    else if (npc.frameCounter < 28)
                    {
                        npc.frame.Y = Frame_Walk_3 * frameHeight;
                    }
                    else if (npc.frameCounter < 35)
                    {
                        npc.frame.Y = Frame_Walk_4 * frameHeight;
                    }
                    else if (npc.frameCounter < 42)
                    {
                        npc.frame.Y = Frame_Walk_5 * frameHeight;
                    }
                    else if (npc.frameCounter < 50)
                    {
                        npc.frame.Y = Frame_Walk_6 * frameHeight;
                    }
                    else
                    {
                        npc.frameCounter = 0;
                    }
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
                else if (npc.frameCounter < 50)
                {
                    npc.frame.Y = Frame_Attack_6 * frameHeight;
                }
                else
                {
                    npc.frameCounter = 0;
                }
            }
        }
    }
}