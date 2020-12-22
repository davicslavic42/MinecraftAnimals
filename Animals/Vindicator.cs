using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace MinecraftAnimals.Animals
{
    public class Vindicator : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vindicator");
            Main.npcFrameCount[npc.type] = 12;
        }
        public override void SetDefaults()
        {
            npc.width = 30;
            npc.height = 40;
            npc.lifeMax = 250;
            npc.damage = 45;
            npc.knockBackResist = 1f;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.aiStyle = -1;
            npc.value = 35f;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnCondition.Overworld.Chance * 0;
        }
        // These const ints are for the benefit of the programmer. Organization is key to making an AI that behaves properly without driving you crazy.
        // Here I lay out what I will use each of the 4 npc.ai slots for.
      internal enum AIStates
        {
            Normal = 0,
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
                    npc.velocity.X = 1.25f * npc.direction;
                }
                if (player.Distance(npc.Center) < 325f)
                {
                    Phase = (int)AIStates.Attack;
                    GlobalTimer = 0;
                }
            }
            if (Phase == (int)AIStates.Attack)
            {
                npc.TargetClosest(true);
                npc.damage = 30;
                npc.velocity.X = 1.75f * npc.direction;
                AttackTimer++;
                if (player.Distance(npc.Center) > 925f)
                {
                    Phase = (int)AIStates.Normal;
                }
            }
           // thanks oli for the tile checks
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
                    if (++npc.frameCounter % 7 == 0)
                        npc.frame.Y = (npc.frame.Y / frameHeight + 1) % ((Main.npcFrameCount[npc.type] / 2) - 1) * frameHeight;
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
            if (npc.HasValidTarget && Main.player[npc.target].Distance(npc.Center) < 50f)
            {
                npc.frameCounter++;
                if (npc.frameCounter < 15)
                {
                    npc.frame.Y = Frame_Walk * frameHeight;
                }
                else if (npc.frameCounter < 20)
                {
                    npc.frame.Y = Frame_Swing * frameHeight;
                }
                else if (npc.frameCounter < 25)
                {
                    npc.frame.Y = Frame_Swing_2 * frameHeight;
                }
                else
                {
                    npc.frameCounter = 0;
                }
            }
        }
    }
}