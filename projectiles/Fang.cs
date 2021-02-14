using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace MinecraftAnimals.projectiles
{
    internal class Fang : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            projectile.width = 38;
            projectile.height = 38;
            projectile.friendly = false;
            projectile.hostile = false;
            projectile.hide = false;
            projectile.penetrate = 20;
            projectile.timeLeft = 500;
            projectile.tileCollide = false;

            //1: projectile.penetrate = 1; // Will hit even if npc is currently immune to player
            //2a: projectile.penetrate = -1; // Will hit and unless 3 is use, set 10 ticks of immunity
            //2b: projectile.penetrate = 3; // Same, but max 3 hits before dying
            //5: projectile.usesLocalNPCImmunity = true;
            //5a: projectile.localNPCHitCooldown = -1; // 1 hit per npc max
            //5b: projectile.localNPCHitCooldown = 20; // o
        }
        internal ref float GlobalProjectileTimer => ref projectile.ai[0];
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // Die immediately if ai[1] isn't 0 (We set this to 1 for the 5 extra explosives we spawn in Kill)
            return false;
        }
        public override void AI()
        {
            GlobalProjectileTimer++;
            if (GlobalProjectileTimer <= 20 & GlobalProjectileTimer >= 0)
            {
                projectile.frame = 0;
                projectile.velocity.Y = -1.85f;
                projectile.velocity.X = 0;
            }
            else
            {
                projectile.velocity.Y = 0;
                projectile.frameCounter++;
                if (++projectile.frameCounter >= 13)
                {
                    projectile.frameCounter = 0;
                    projectile.frame += 1;
                }
                projectile.friendly = false;
                projectile.hostile = true;
            }
            if (GlobalProjectileTimer > 60)
            {
                projectile.Kill();
            }
        }

        /*
        public override void AI()
        {
            int delay = 0;
            delay++;
            if (delay <= 21)
            {
                projectile.frame = 0;
            }
            else
            {
                projectile.hostile = true;
                projectile.frameCounter++;
                if (++projectile.frameCounter >= 14)
                {
                    projectile.frameCounter = 0;
                    projectile.frame = ++projectile.frame % Main.projFrames[projectile.type];
                }           // Kill this projectile after 1 second
            }
            if (projectile.timeLeft == 0)
            {
                projectile.Kill();
            }
            if (projectile.timeLeft >= 28)
            {
                projectile.friendly = false;
            }
        }
        */

    }
}