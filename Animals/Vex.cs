using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;


namespace MinecraftAnimals.Animals
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
            npc.lifeMax = 80;
            npc.damage = 28;
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
        // These const ints are for the benefit of the programmer. Organization is key to making an AI that behaves properly without driving you crazy.
        // Here I lay out what I will use each of the 4 npc.ai slots for.
        private const int AI_State_Slot = 0;
        private const int AI_Timer_Slot = 1;
        // Here I define some values I will use with the State slot. Using an ai slot as a means to store "state" can simplify things greatly. Think flowchart.
        private const int State_Find = 0;
        private const int State_Attack = 1;

        // This is a property (https://msdn.microsoft.com/en-us/library/x9fsa0sw.aspx), it is very useful and helps keep out AI code clear of clutter.
        // Without it, every instance of "AI_State" in the AI code below would be "npc.ai[AI_State_Slot]". 
        // Also note that without the "AI_State_Slot" defined above, this would be "npc.ai[0]".
        // This is all to just make beautiful, manageable, and clean code.
        public float AI_State
        {
            get => npc.ai[AI_State_Slot];
            set => npc.ai[AI_State_Slot] = value;
        }

        public float AI_Timer
        {
            get => npc.ai[AI_Timer_Slot];
            set => npc.ai[AI_Timer_Slot] = value;
        }
        public override void AI()
        {
            Player player = Main.player[npc.target];
            npc.TargetClosest(true);
            Collision.StepUp(ref npc.position, ref npc.velocity, npc.width, npc.height, ref npc.stepSpeed, ref npc.gfxOffY);
            if (AI_State == State_Find)
            {
                AI_Timer++;
                npc.velocity.X = 1 * npc.direction;
                npc.velocity.Y = 1;

                if (npc.HasValidTarget && player.Distance(npc.Center) < 165f)
                {
                    AI_State = State_Attack;
                    AI_Timer = 0;
                    npc.frameCounter = 0;
                }
                if (AI_Timer == 750)
                {
                    npc.spriteDirection = -1;
                    AI_Timer = 0;
                }
            }
            // thanks oli for the tile checks
            else if (AI_State == State_Attack)
            {
                AI_Timer++;
                npc.velocity.X = 2 * npc.direction;
                npc.velocity.Y = 1;

                if (npc.HasValidTarget && player.Distance(npc.Center) > 165f)
                {
                    AI_State = State_Find;
                    AI_Timer = 0;
                    npc.frameCounter = 0;
                }
            }
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
            if (AI_State == State_Find)
            {
                npc.frameCounter++;
                if (npc.frameCounter < 5)
                {
                    npc.frame.Y = Frame_Walk * frameHeight;
                }
                else if (npc.frameCounter < 11)
                {
                    npc.frame.Y = Frame_Walk_2 * frameHeight;
                }
                else if (npc.frameCounter < 17)
                {
                    npc.frame.Y = Frame_Walk_3 * frameHeight;
                }
                else if (npc.frameCounter < 24)
                {
                    npc.frame.Y = Frame_Walk_4 * frameHeight;
                }
                else if (npc.frameCounter < 30)
                {
                    npc.frame.Y = Frame_Walk_5 * frameHeight;
                }
                else if (npc.frameCounter < 38)
                {
                    npc.frame.Y = Frame_Walk_6 * frameHeight;
                }
                else
                {
                    npc.frameCounter = 0;
                }
            }
            if (AI_State == State_Attack)
            {
                npc.frameCounter++;
                if (npc.frameCounter < 5)
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
        }
    }
}