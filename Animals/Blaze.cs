using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MinecraftAnimals.projectiles;

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
        private const int AI_State_Slot = 0;
        private const int AI_Timer_Slot = 1;

        private const int State_Search = 0;
        private const int State_Notice = 1;
        private const int State_Shoot = 2;

        public float AI_Timer
        {
            get => npc.ai[AI_Timer_Slot];
            set => npc.ai[AI_Timer_Slot] = value;
        }
        public float AI_State
        {
            get => npc.ai[AI_State_Slot];
            set => npc.ai[AI_State_Slot] = value;
        }

        public override void AI()
        {
            if (AI_State == State_Search)
            {
                Player player = Main.player[npc.target];
                npc.TargetClosest(true);
                npc.velocity.X = 0 * npc.direction;
                //thanks nuova prime//
                AI_Timer++;
                if (npc.HasValidTarget && Main.player[npc.target].Distance(npc.Center) < 550f)
                {
                    AI_State = State_Notice;
                    AI_Timer = 0;
                }
                if (Main.player[npc.target].position.Y < npc.position.Y + 130)
                {
                    npc.velocity.Y -= npc.velocity.Y > 0f ? 1f : .5f;
                }
                if (Main.player[npc.target].position.Y > npc.position.Y + 130)
                {
                    npc.velocity.Y += npc.velocity.Y < 0f ? 1f : .25f;
                }

            }
            else if (AI_State == State_Notice)
            {
                Player player = Main.player[npc.target];
                npc.TargetClosest(true);
                npc.velocity.X = 1 * npc.direction;
                //thanks nuova prime//
                AI_Timer++;
                if (npc.HasValidTarget && Main.player[npc.target].Distance(npc.Center) > 550f)
                {
                    AI_State = State_Search;
                    AI_Timer = 0;
                }
                if (Main.player[npc.target].position.Y < npc.position.Y + 130)
                {
                    npc.velocity.Y -= npc.velocity.Y > 0f ? 1f : .5f;
                }
                if (Main.player[npc.target].position.Y > npc.position.Y + 130)
                {
                    npc.velocity.Y += npc.velocity.Y < 0f ? 1f : .25f;
                }
                if (!npc.HasValidTarget || Main.player[npc.target].Distance(npc.Center) < 270f)
                {
                    // Out targeted player seems to have left our range, so we'll go back to sleep.
                    AI_State = State_Shoot;
                    AI_Timer = 0;
                }
            }
            else if (AI_State == State_Shoot)
            {
                AI_Timer++;
                Player player = Main.player[npc.target];
                npc.TargetClosest(true);
                npc.velocity.X = 0 * npc.direction;
                //thanks nuova prime//
                if (Main.player[npc.target].position.Y < npc.position.Y + 130)
                {
                    npc.velocity.Y -= npc.velocity.Y > 0f ? 1f : .5f;
                }
                if (Main.player[npc.target].position.Y > npc.position.Y + 130)
                {
                    npc.velocity.Y += npc.velocity.Y < 0f ? 1f : .25f;
                }
                if (AI_Timer == 165 || AI_Timer == 180 || AI_Timer == 195) //Check three states of AI_Timer, this will result in 3 shots with a delay of 15 frames
                {
                    Player TargetPlayer = Main.player[(int)Player.FindClosest(npc.position, npc.width, npc.height)];
                    _ = npc.Distance(npc.position) - 50;
                    Vector2 PlayerDir = npc.DirectionTo(TargetPlayer.position);
                    Vector2 DirToRing = npc.DirectionTo(TargetPlayer.position + PlayerDir.RotatedBy(0.001f) * -75f);

                    npc.velocity.X += DirToRing.X;
                    npc.velocity.Y += DirToRing.Y;

                    Main.PlaySound(SoundID.Item20, npc.position); //We play a sound at the NPC's position for feedback for each shot

                    Projectile.NewProjectile(npc.Center, PlayerDir.RotatedByRandom(0.15f) * 7.5f, mod.ProjectileType("FireCharge"), 15, 2, Main.LocalPlayer.whoAmI); //Multiply velocity with a larger number for more speed
                }
                if (AI_Timer == 210)
                {
                    AI_Timer = 0;
                }
                else
                {
                    if (!npc.HasValidTarget || Main.player[npc.target].Distance(npc.Center) > 270f)
                    {
                        // Out targeted player seems to have left our range, so we'll go back to sleep.
                        AI_State = State_Notice;
                        AI_Timer = 0;
                    }
                }
            }
        }
        private const int Frame_Float = 0;
        private const int Frame_Float_2 = 1;
        private const int Frame_Float_3 = 2;
        public override void FindFrame(int frameHeight)
        {
            // This makes the sprite flip horizontally in conjunction with the npc.direction.
            npc.spriteDirection = npc.direction;
            {
                npc.frameCounter++;
                if (npc.frameCounter < 10)
                {
                    npc.frame.Y = Frame_Float * frameHeight;
                }
                else if (npc.frameCounter < 20)
                {
                    npc.frame.Y = Frame_Float_2 * frameHeight;
                }
                else if (npc.frameCounter < 30)
                {
                    npc.frame.Y = Frame_Float_3 * frameHeight;
                }
                else
                {
                    npc.frameCounter = 0;
                }
            }
        }
    }
}

