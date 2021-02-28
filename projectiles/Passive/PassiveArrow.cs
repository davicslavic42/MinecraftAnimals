using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace MinecraftAnimals.projectiles.Passive
{
    public class PassiveArrow : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arrow");
        }
        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 28;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = 3;
            projectile.hide = false;
            projectile.damage = 12;
            projectile.knockBack = 2.5f;
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
                const float velYmult = 0.1f; // y velocity factor, every AI update the y velocity will be be 0.35f bigger of the original speed, causing the javelin to drop to the ground
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
    }
}