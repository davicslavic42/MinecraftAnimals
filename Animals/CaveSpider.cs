using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinecraftAnimals.Animals
{
    public class CaveSpider : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("CaveSpider");
            Main.npcFrameCount[npc.type] = 3;
        }
        public override void SetDefaults()
        {
            npc.width = 40;
            npc.height = 40;
            npc.lifeMax = 38;
            npc.damage = 15;
            npc.knockBackResist = 1.5f;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.aiStyle = -1;
            npc.value = 35f;
            npc.scale = .5f;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnCondition.Cavern.Chance * 0.1f;
        }
        public enum AIStates
        {
            Normal = 0,
            Attack = 1
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
            if (Phase == (int)AIStates.Normal)
            {
                npc.damage = 0;
                npc.TargetClosest(false);
                npc.velocity.X = 1 * npc.direction;
                if (GlobalTimer == 5)
                {
                    switch (Main.rand.Next(2))
                    {
                        case 0:
                            npc.direction = 1;
                            return;
                        case 1:
                            npc.direction = -1;
                            return;
                    }
                }
                float change = GlobalTimer <= 500 ? npc.velocity.X = 1 * npc.direction : npc.velocity.X = 0 * npc.direction;
                if (GlobalTimer >= 800)
                {
                    GlobalTimer = 0;
                }
            }
            // thanks oli for the tile checks
            if (Phase == (int)AIStates.Attack)
            {
                npc.damage = 38;
                npc.friendly = false;
                npc.TargetClosest(true);
                npc.velocity.X = 1 * npc.direction;
                if (npc.HasValidTarget && player.Distance(npc.Center) < 775f)
                {
                    npc.velocity.X = 1.4f * npc.direction;
                }
            }
            if (Collision.SolidCollision(npc.position, (npc.width / 16 + 1), npc.height) && AttackTimer >= 50)
            {
                for (int i = 0; i < 1; i++)
                {
                    npc.velocity = new Vector2(npc.direction * 2, -6f);
                }
                AttackTimer = 0;
            }
            Collision.StepUp(ref npc.position, ref npc.velocity, npc.width, npc.height, ref npc.stepSpeed, ref npc.gfxOffY);
            if (npc.type == mod.NPCType("CaveSpider") && Main.netMode != NetmodeID.MultiplayerClient && npc.velocity.Y > 1f)
            {
                int num99 = (int)npc.Center.X / 16;
                int num100 = (int)npc.Center.Y / 16;
                bool flag9 = false;
                for (int num101 = num99 - 1; num101 <= num99 + 1; num101++)
                {
                    for (int num102 = num100 - 1; num102 <= num100 + 1; num102++)
                    {
                        if (Main.tile[num101, num102].wall > 0)
                        {
                            flag9 = true;
                        }
                    }
                }
                if (flag9)
                {
                    npc.Transform(mod.NPCType("CaveSpiderWall"));
                }
            }
            if (Main.dayTime == false) // half brightness
            {
                Phase = (int)AIStates.Attack;
            }
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            npc.friendly = false;
            Phase = (int)AIStates.Attack;
            GlobalTimer = 0;
            base.HitEffect(hitDirection, damage);
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            base.OnHitPlayer(target, damage, crit);
            target.AddBuff(BuffID.Poisoned, 180);
        }
        private const int Frame_Walk = 0;
        private const int Frame_Walk_2 = 1;
        private const int Frame_Walk_3 = 2;

        public override void FindFrame(int frameHeight)
        {
            // This makes the sprite flip horizontally in conjunction with the npc.direction.
            npc.spriteDirection = npc.direction;
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
                else
                {
                    npc.frameCounter = 0;
                }
            }
        }
    }
}