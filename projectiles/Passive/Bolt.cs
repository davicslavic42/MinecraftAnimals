using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace MinecraftAnimals.projectiles.Passive
{
    // The following laser shows a channeled ability, after charging up the laser will be fired
    // Using custom drawing, dust effects, and custom collision checks for tiles
    public class Bolt : ModProjectile
    {
        // Use a different style for constant so it is very clear in code when a constant is used

        // The maximum charge value

        public int TargetWhoAmI
        {
            get => (int)projectile.ai[1];
            set => projectile.ai[1] = value;
        }
        private const int MAX_TICKS = 25;
        internal ref float Charge => ref projectile.ai[0];
        private const float MAX_CHARGE = 50f;
        internal ref float GlobalProjectileTimer => ref projectile.ai[1];
        bool Loaded = false;

        // Are we at max charge? With c#6 you can simply use => which indicates this is a get only property
        public bool IsAtMaxCharge => Charge == MAX_CHARGE;

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = 3;
            projectile.hide = false;
            projectile.damage = 5;
            projectile.tileCollide = true;
            projectile.ranged = true;
            projectile.timeLeft = 2;
        }
        #region ignore
        private void UpdatePlayer(Player player)
        {
            // Multiplayer support here, only run this code if the client running it is the owner of the projectile
            Vector2 aim = Vector2.Normalize(Main.MouseWorld - player.Center);
            if (projectile.owner == Main.myPlayer)
            {
                projectile.velocity = aim;
                projectile.direction = Main.MouseWorld.X > player.position.X ? 1 : -1;
                projectile.netUpdate = true;
            }
            int dir = projectile.direction;
            player.ChangeDir(dir); // Set player direction to where we are shooting
            player.heldProj = projectile.whoAmI; // Update player's held projectile
            player.itemTime = 2; // Set item time to 2 frames while we are used
            player.itemAnimation = 2; // Set item animation time to 2 frames while we are used
            player.itemRotation = (float)Math.Atan2(projectile.velocity.Y * dir, projectile.velocity.X * dir); // Set the item rotation to where we are shooting  aim.ToRotation();
        }

        #endregion ignore
        /*
        public void Drawarrow(SpriteBatch spriteBatch, Texture2D texture, Vector2 start, Vector2 unit, float step, int damage, float rotation = 0f, float scale = 1f, Color color = default(Color))
        {
            float r = new Vector2(projectile.velocity).ToRotation();
            // Draws the laser 'body'
            spriteBatch.Draw(texture, projectile.position, null, Color.White, r, scale, 0, 0);
        }
        */
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            return false;
        }
        private void Firebolt(Player player)
        {
            Vector2 aim = Vector2.Normalize(Main.MouseWorld - player.Center);
            projectile.velocity = aim.RotatedByRandom(0.1f) * 12f;
        }

        private void Chargeshot(Player player)
        {
            if (!player.channel && !Loaded) projectile.Kill();
            if (!IsAtMaxCharge && player.channel) Charge++;
            if (IsAtMaxCharge && player.channel) Loaded = true;
            if (player.channel && Loaded) Firebolt(player);
        }

        #region basic ai and mp

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            projectile.position = player.Center + projectile.velocity * 1.5f;
            projectile.timeLeft = 2;

            // By separating large AI into methods it becomes very easy to see the flow of the AI in a broader sense
            // First we update player variables that are needed to channel the laser
            // Then we run our charging laser logic
            // If we are fully charged, we proceed to update the laser's position
            // Finally we spawn some effects like dusts and light
            UpdatePlayer(player);
            Chargeshot(player);
            // If laser is not charged yet, stop the AI here.
            if (!Loaded) return;
            Firebolt(player);
            projectile.hide = false;
            projectile.velocity.Y += 0.1f;


        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // We can only collide if we are at max charge, which is when the laser is actually fired
            if (!IsAtMaxCharge) return false;

            Player player = Main.player[projectile.owner];
            Vector2 unit = projectile.velocity;
            float point = 0f;
            // Run an AABB versus Line check to look for collisions, look up AABB collision first to see how it works
            // It will look for collisions on the given line using AABB
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), player.Center,
                projectile.Center, 22, ref point);
        }

        private void SetLaserPosition(Player player)
        {
        }
        /*
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
        */
        #endregion
    }
}
