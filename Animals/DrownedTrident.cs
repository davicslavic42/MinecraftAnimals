﻿using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using MinecraftAnimals.Items.Weapons;

namespace MinecraftAnimals.Animals
{
    public class DrownedTrident : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Drowned");
            Main.npcFrameCount[npc.type] = 13;
        }
        public override void SetDefaults()
        {
            npc.width = 64;
            npc.height = 80;
            npc.lifeMax = 83;
            npc.knockBackResist = 1f;
            npc.damage = 20;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.aiStyle = -1;
            npc.value = 35f;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnCondition.Ocean.Chance * 0.04f;
        }
        // These const ints are for the benefit of the programmer. Organization is key to making an AI that behaves properly without driving you crazy.
        // Here I lay out what I will use each of the 4 npc.ai slots for.
        private const int AI_State_Slot = 0;
        private const int AI_Timer_Slot = 1;
        // Here I define some values I will use with the State slot. Using an ai slot as a means to store "state" can simplify things greatly. Think flowchart.
        private const int State_Search = 0;
        private const int State_Notice = 1;
        private const int State_Throw = 2;
        private const int State_Jump = 3;

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
            Collision.StepUp(ref npc.position, ref npc.velocity, npc.width, npc.height, ref npc.stepSpeed, ref npc.gfxOffY);
            // The npc starts in the asleep state, waiting for a player to enter range
            if (AI_State == State_Search)
            {
                npc.velocity.X = 0;
                npc.velocity.Y = 0.5f;
                // TargetClosest sets npc.target to the player.whoAmI of the closest player. the faceTarget parameter means that npc.direction will automatically be 1 or -1 if the targeted player is to the right or left. This is also automatically flipped if npc.confused
                npc.TargetClosest(true);
                // Now we check the make sure the target is still valid and within our specified notice range (500)
                if (npc.HasValidTarget && player.Distance(npc.Center) < 600f)
                {
                    AI_State = State_Notice;
                    AI_Timer = 0;
                }
            }
            // In this state, a player has been targeted
            else if (AI_State == State_Notice)
            {
                npc.TargetClosest(true);
                npc.velocity.X = 1 * npc.direction;
                npc.velocity.Y += 0.5f;
                if (npc.HasValidTarget && player.Distance(npc.Center) > 600f)
                {
                    AI_State = State_Search;
                    AI_Timer = 0;
                }
                if (Collision.SolidCollision(npc.position, (npc.width + 2), npc.height))
                {
                    AI_State = State_Jump;
                    AI_Timer = 0;
                }
                // If the targeted player is in attack range (250).
                if (player.Distance(npc.Center) < 325f)
                {
                    AI_State = State_Throw;
                    AI_Timer = 0;
                }
                else
                {
                    npc.TargetClosest(true);
                    if (!npc.HasValidTarget || player.Distance(npc.Center) > 600f)
                    {
                        // Out targeted player seems to have left our range, so we'll go back to sleep.
                        AI_State = State_Search;
                        AI_Timer = 0;
                    }
                }
            }
            // In this state, we are in the throw. 
            else if (AI_State == State_Throw)
            {
                npc.velocity.X = 0;
                npc.velocity.Y += 0.5f;
                if (npc.frameCounter == 61)
                {
                    Player TargetPlayer = Main.player[(int)Player.FindClosest(npc.position, npc.width, npc.height)];
                    _ = npc.Distance(npc.position) - 50;
                    Vector2 PlayerDir = npc.DirectionTo(TargetPlayer.position);
                    Vector2 DirToRing = npc.DirectionTo(TargetPlayer.position + PlayerDir.RotatedBy(0.001f) * -75f);

                    npc.velocity.X += DirToRing.X;
                    npc.velocity.Y += DirToRing.Y;

                    Projectile.NewProjectile(npc.Center, PlayerDir.RotatedByRandom(0.15f) * 7.5f, mod.ProjectileType("FireCharge"), 15, 2, Main.LocalPlayer.whoAmI); //Multiply velocity with a larger number for more speed
                }
                else
                {
                    if (!npc.HasValidTarget || player.Distance(npc.Center) > 325f)
                    {
                        // Out targeted player seems to have left our range, so we'll go back to sleep.
                        AI_State = State_Notice;
                        AI_Timer = 0;
                    }
                }
            }
            else if (AI_State == State_Jump)
            {
                AI_Timer++;
                npc.TargetClosest(true);
                npc.velocity.X = 2f * npc.direction;
                npc.velocity.Y += 0.5f;
                if (AI_Timer == 1)
                {
                    // We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up. 
                    npc.velocity = new Vector2(npc.direction * 1, -10f);
                }
                if (AI_Timer > 15)
                {
                    AI_State = State_Search;
                    AI_Timer = 0;
                }
            }
        }
        public override void NPCLoot()
        {
            base.NPCLoot();
            if (Main.rand.NextBool(50))
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Trident"));
            }
        }
        // Our texture is 32x32 with 2 pixels of padding vertically, so 34 is the vertical spacing.  These are for my benefit and the numbers could easily be used directly in the code below, but this is how I keep code organized.
        private const int Frame_Walk = 0;
        private const int Frame_Walk_2 = 1;
        private const int Frame_Walk_3 = 2;
        private const int Frame_Walk_4 = 3;
        private const int Frame_Walk_5 = 4;
        private const int Frame_Jump = 4;
        private const int Frame_Throw = 5;
        private const int Frame_Throw_2 = 6;
        private const int Frame_Throw_3 = 7;
        private const int Frame_Throw_4 = 8;
        private const int Frame_Throw_5 = 9;
        private const int Frame_Throw_6 = 10;
        private const int Frame_Throw_7 = 11;

        // Here in FindFrame, we want to set the animation frame our npc will use depending on what it is doing.
        // We set npc.frame.Y to x * frameHeight where x is the xth frame in our spritesheet, counting from 0. For convenience, I have defined some consts above.
        public override void FindFrame(int frameHeight)
        {
            // This makes the sprite flip horizontally in conjunction with the npc.direction.
            npc.spriteDirection = npc.direction;
            if (AI_State == State_Search)
            {
                npc.frameCounter++;
                if (npc.frameCounter < 10)
                {
                    npc.frame.Y = Frame_Walk * frameHeight;
                }
                else
                {
                    npc.frameCounter = 0;
                }
            }
            else if (AI_State == State_Notice)
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
                    npc.frame.Y = Frame_Walk * frameHeight;
                }
                else if (npc.frameCounter < 50)
                {
                    npc.frame.Y = Frame_Walk_4 * frameHeight;
                }
                else if (npc.frameCounter < 60)
                {
                    npc.frame.Y = Frame_Walk_5 * frameHeight;
                }
                else
                {
                    npc.frameCounter = 0;
                }
            }
            else if (AI_State == State_Throw)
            {
                npc.frameCounter++;
                if (npc.frameCounter < 15)
                {
                    npc.frame.Y = Frame_Throw * frameHeight;
                }
                else if (npc.frameCounter < 20)
                {
                    npc.frame.Y = Frame_Throw_2 * frameHeight;
                }
                else if (npc.frameCounter < 30)
                {
                    npc.frame.Y = Frame_Throw_3 * frameHeight;
                }
                else if (npc.frameCounter < 40)
                {
                    npc.frame.Y = Frame_Throw_4 * frameHeight;
                }
                else if (npc.frameCounter < 47)
                {
                    npc.frame.Y = Frame_Throw_5 * frameHeight;
                }
                else if (npc.frameCounter < 52)
                {
                    npc.frame.Y = Frame_Throw_6 * frameHeight;
                }
                else if (npc.frameCounter < 70)
                {
                    npc.frame.Y = Frame_Throw_7 * frameHeight;
                }
                else
                {
                    npc.frameCounter = 0;
                }
            }
            else if (AI_State == State_Jump)
            {
                npc.frame.Y = Frame_Jump * frameHeight;
            }
        }
    }
}