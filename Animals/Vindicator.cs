﻿using Terraria;
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
            Main.npcFrameCount[npc.type] = 8;
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
        private const int AI_State_Slot = 0;
        private const int AI_Timer_Slot = 1;
        // Here I define some values I will use with the State slot. Using an ai slot as a means to store "state" can simplify things greatly. Think flowchart.
        private const int State_Find = 0;
        private const int State_Attack = 1;
        private const int State_Jump = 2;

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
            Collision.StepUp(ref npc.position, ref npc.velocity, npc.width, npc.height, ref npc.stepSpeed, ref npc.gfxOffY);
            if (AI_State == State_Find)
            {
                AI_Timer++;
                Player player = Main.player[npc.target];
                npc.TargetClosest(true);
                npc.velocity.X = 0;
                npc.velocity.Y += 0.5f;

                if (npc.HasValidTarget && Main.player[npc.target].Distance(npc.Center) < 550f)
                {
                    AI_State = State_Attack;
                    AI_Timer = 0;
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
                Player player = Main.player[npc.target];
                npc.TargetClosest(true);
                npc.velocity.X = 1.5f * npc.direction;
                npc.velocity.Y += 0.5f;

                if (npc.HasValidTarget && Main.player[npc.target].Distance(npc.Center) > 550f)
                {
                    AI_State = State_Find;
                    AI_Timer = 0;
                }
                if (Collision.SolidCollision(npc.position, (npc.width + 2), npc.height))
                {
                    AI_State = State_Jump;
                    AI_Timer = 0;
                }
            }
            else if (AI_State == State_Jump)
            {
                AI_Timer++;
                Player player = Main.player[npc.target];
                npc.TargetClosest(true);
                npc.velocity.X = 1.5f * npc.direction;
                npc.velocity.Y += 0.5f;
                if (AI_Timer == 1)
                {
                    // We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up. 
                    npc.velocity = new Vector2(npc.direction * 1, -10f);
                }
                else if (AI_Timer > 15)
                {
                    AI_State = State_Attack;
                    AI_Timer = 0;
                }
            }
        }
        private const int Frame_Idle = 0;
        private const int Frame_Walk = 1;
        private const int Frame_Walk_2 = 2;
        private const int Frame_Walk_3 = 3;
        private const int Frame_Walk_4 = 4;
        private const int Frame_Walk_5 = 5;
        private const int Frame_Attack = 6;
        private const int Frame_Attack_2 = 7;
        public override void FindFrame(int frameHeight)
        {
            // This makes the sprite flip horizontally in conjunction with the npc.direction.
            npc.spriteDirection = npc.direction;
            if (AI_State == State_Find)
            {
                npc.frameCounter++;
                {
                    npc.frame.Y = Frame_Idle * frameHeight;
                }
            }
            if (AI_State == State_Attack)
            {
                npc.frameCounter++;
                if (npc.frameCounter < 10)
                {
                    npc.frame.Y = Frame_Walk * frameHeight;
                }
                else if (npc.frameCounter < 20)
                {
                    npc.frame.Y = Frame_Walk_2 * frameHeight;
                }
                else if (npc.frameCounter < 30)
                {
                    npc.frame.Y = Frame_Walk_3 * frameHeight;
                }
                else if (npc.frameCounter < 40)
                {
                    npc.frame.Y = Frame_Walk_4 * frameHeight;
                }
                else if (npc.frameCounter < 50)
                {
                    npc.frame.Y = Frame_Walk_5 * frameHeight;
                }
                else
                {
                    npc.frameCounter = 0;
                }
            }
            if (npc.HasValidTarget && Main.player[npc.target].Distance(npc.Center) < 50f)
            {
                npc.frameCounter++;
                if (npc.frameCounter < 36)
                {
                    npc.frame.Y = Frame_Walk * frameHeight;
                }
                else if (npc.frameCounter < 55)
                {
                    npc.frame.Y = Frame_Attack * frameHeight;
                }
                else if (npc.frameCounter < 66)
                {
                    npc.frame.Y = Frame_Attack_2 * frameHeight;
                }
                else
                {
                    npc.frameCounter = 0;
                }
            }
            if (AI_State == State_Jump)
            {
                npc.frameCounter++;

                if (npc.frameCounter < 10)
                {
                    npc.frame.Y = Frame_Walk * frameHeight;
                }
                else if (npc.frameCounter < 20)
                {
                    npc.frame.Y = Frame_Walk_2 * frameHeight;
                }
                else if (npc.frameCounter < 30)
                {
                    npc.frame.Y = Frame_Walk_3 * frameHeight;
                }
                else if (npc.frameCounter < 40)
                {
                    npc.frame.Y = Frame_Walk_4 * frameHeight;
                }
                else if (npc.frameCounter < 50)
                {
                    npc.frame.Y = Frame_Walk_5 * frameHeight;
                }
                else
                {
                    npc.frameCounter = 0;
                }
            }
        }
    }
}