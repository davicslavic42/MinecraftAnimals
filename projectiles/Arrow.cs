﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace MinecraftAnimals.projectiles
{
    public class Arrow : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arrow");
        }
        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.penetrate = 3;
            projectile.hide = false;
            projectile.damage = 5;
        }
        public int TargetWhoAmI
        {
            get => (int)projectile.ai[1];
            set => projectile.ai[1] = value;
        }

        private const int MAX_TICKS = 45;

        // Change this number if you want to alter how the alpha changes
        public override void AI()
        {
            NormalAI();
        }
        private void NormalAI()
        {
            TargetWhoAmI++;

            // For a little while, the javelin will travel with the same speed, but after this, the javelin drops velocity very quickly.
            if (TargetWhoAmI >= MAX_TICKS)
            {
                const float velXmult = 0.98f; // x velocity factor, every AI update the x velocity will be 98% of the original speed
                const float velYmult = 0.15f; // y velocity factor, every AI update the y velocity will be be 0.35f bigger of the original speed, causing the javelin to drop to the ground
                TargetWhoAmI = MAX_TICKS; // set ai1 to maxTicks continuously
                projectile.velocity.X *= velXmult;
                projectile.velocity.Y += velYmult;
            }

            // Make sure to set the rotation accordingly to the velocity, and add some to work around the sprite's rotation
            // Please notice the MathHelper usage, offset the rotation by 90 degrees (to radians because rotation uses radians) because the sprite's rotation is not aligned!
            projectile.rotation =
                projectile.velocity.ToRotation() +
                MathHelper.ToRadians(90f);
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (target.active && target.townNPC && target.friendly && target.aiStyle == 7 && target.chaseable && target.HasGivenName && !NPCID.Sets.TownCritter[target.type] && (!target.homeless || target.homeless)) damage *= 2;
        }
    }
}