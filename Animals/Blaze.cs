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
            npc.lifeMax = 250;
            npc.damage = 25;
            npc.lavaImmune = true;
            npc.knockBackResist = 0f;
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
                npc.velocity.X = 0;
                //thanks nuova prime//
                if (Main.player[npc.target].position.Y < npc.position.Y + 130)
                {
                    npc.velocity.Y -= npc.velocity.Y > 0f ? 1f : .5f;
                }
                if (Main.player[npc.target].position.Y > npc.position.Y + 130)
                {
                    npc.velocity.Y += npc.velocity.Y < 0f ? 1f : .25f;
                }
                if (npc.HasValidTarget && Main.player[npc.target].Distance(npc.Center) < 550f)
                {
                    AI_State = State_Notice;
                    AI_Timer = 0;
                }
            }
            else if (AI_State == State_Notice)
            {
                Player player = Main.player[npc.target];
                npc.TargetClosest(true);
                npc.velocity.X = 1 * npc.direction;
                //thanks nuova prime//
                if (Main.player[npc.target].position.Y < npc.position.Y + 130)
                {
                    npc.velocity.Y -= npc.velocity.Y > 0f ? 1f : .5f;
                }
                if (Main.player[npc.target].position.Y > npc.position.Y + 130)
                {
                    npc.velocity.Y += npc.velocity.Y < 0f ? 1f : .25f;
                }
                if (Main.player[npc.target].Distance(npc.Center) < 320f)
                {
                    AI_State = State_Shoot;
                    AI_Timer = 0;
                }
                if (npc.HasValidTarget && Main.player[npc.target].Distance(npc.Center) > 550f)
                {
                    AI_State = State_Search;
                    AI_Timer = 0;
                }
            }
            else if (AI_State == State_Shoot)
            {
                npc.velocity.X = 0;
                AI_Timer++;
                Player player = Main.player[npc.target];
                npc.TargetClosest(true);
                if (Main.player[npc.target].position.Y < npc.position.Y + 130)
                {
                    npc.velocity.Y -= npc.velocity.Y > 0f ? 1f : .5f;
                }
                if (Main.player[npc.target].position.Y > npc.position.Y + 130)
                {
                    npc.velocity.Y += npc.velocity.Y < 0f ? 1f : .25f;
                }

                if (AI_Timer > 200)
                {
                    Vector2 newVelocity = Vector2.Normalize(Main.player[npc.target].Center + Main.player[npc.target].velocity - npc.Center) * 9;
                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, newVelocity.X, newVelocity.Y, ModContent.ProjectileType<FireCharge>(), npc.damage / 3, 3f, Main.myPlayer, BuffID.OnFire, 600f);
                    AI_Timer = 0;
                }
                else
                {
                    if (!npc.HasValidTarget || Main.player[npc.target].Distance(npc.Center) > 350f)
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

