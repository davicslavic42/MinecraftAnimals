using Microsoft.Xna.Framework;
using MinecraftAnimals.Items.Weapons;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MinecraftAnimals.projectiles
{
	public class Harmpot : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Potion of Harming");
		}
		public override void SetDefaults()
		{
			projectile.width = 16;
			projectile.height = 20;
			projectile.friendly = false;
			projectile.hostile = true;
			projectile.penetrate = 3;
			projectile.hide = false;
			projectile.damage = 5;
			projectile.timeLeft = 50;
		}
		public int TargetWhoAmI
		{
			get => (int)projectile.ai[1];
			set => projectile.ai[1] = value;
		}

		private const int MAX_TICKS = 25;

		// Change this number if you want to alter how the alpha changes
		public override void AI()
		{
			NormalAI();
		}
		private void NormalAI()
		{
			TargetWhoAmI++;

			if (TargetWhoAmI >= MAX_TICKS)
			{
				const float velXmult = 0.99f; // x velocity factor, every AI update the x velocity will be 98% of the original speed
				const float velYmult = 0.1f; // y velocity factor, every AI update the y velocity will be be 0.35f bigger of the original speed, causing the javelin to drop to the ground
				TargetWhoAmI = MAX_TICKS; // set ai1 to maxTicks continuously
				projectile.velocity.X *= velXmult;
				projectile.velocity.Y += velYmult;
			}
			if (projectile.owner == Main.myPlayer && projectile.timeLeft <= 3)
			{
				projectile.tileCollide = false;
				// Set to transparent. This projectile technically lives as  transparent for about 3 frames
				// change the hitbox size, centered about the original projectile center. This makes the projectile damage enemies during the explosion.
				projectile.position = projectile.Center;
			}

			// Make sure to set the rotation accordingly to the velocity, and add some to work around the sprite's rotation
			// Please notice the MathHelper usage, offset the rotation by 90 degrees (to radians because rotation uses radians) because the sprite's rotation is not aligned!
		}
		public override void Kill(int timeLeft)
		{
			// Play explosion sound
			Main.PlaySound(SoundID.Item15, projectile.position);
			// Smoke Dust spawn
			for (int i = 0; i < 10; i++)
			{
				int dustIndex = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, DustType<Dusts.SplashPoteffect>(), 0f, 0f, 100, default(Color), 2f);
				Main.dust[dustIndex].velocity *= 1.2f;
				Main.dust[dustIndex].color = Color.HotPink;
			}
			//Rectangle dustHitbox = new Rectangle((int)Main.dust[dustIndex].position.X, (int)Main.dust[dustIndex].position.Y, 5, 5); //atempting to create rectangles on the dust so that they can be changed to the projectilehitbox
			//projectile.Hitbox = dustHitbox;

		}
	}
}