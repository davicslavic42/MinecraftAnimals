using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.Achievements;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinecraftAnimals.projectiles
{
	// to investigate: Projectile.Damage, (8843)
	internal class RavagerRoar : ModProjectile
	{
		public override void SetDefaults()
		{
			// while the sprite is actually bigger than 15x15, we use 15x15 since it lets the projectile clip into tiles as it bounces. It looks better.
			projectile.width = 15;
			projectile.height = 15;
			projectile.penetrate = -1;
			projectile.friendly = false;
			projectile.hostile = true;
			projectile.hide = true;
			// 5 second fuse.
			projectile.timeLeft = 5;

			// These 2 help the projectile hitbox be centered on the projectile sprite.
			drawOffsetX = 5;
			drawOriginOffsetY = 5;
		}
		public override void AI()
		{
			if (projectile.owner == Main.myPlayer && projectile.timeLeft <= 3)
			{
				projectile.hide = true;
				projectile.tileCollide = false;
				// Set to transparent. This projectile technically lives as  transparent for about 3 frames
				projectile.alpha = 255;
				// change the hitbox size, centered about the original projectile center. This makes the projectile damage enemies during the explosion.
				projectile.position = projectile.Center;
				//projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
				//projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
				projectile.width = 550;
				projectile.height = 550;
				projectile.Center = projectile.position;
				//projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
				//projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
				projectile.damage = 1;
				projectile.knockBack = 32f;
			}
		}
		public override void Kill(int timeLeft)
		{
			// Play explosion sound
			Main.PlaySound(SoundID.Item15, projectile.position);
			// Smoke Dust spawn
			for (int i = 0; i < 5; i++)
			{
				int dustIndex = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 31, 0f, 0f, 100, default(Color), 2f);
				Main.dust[dustIndex].velocity *= 1.4f;
			}
			projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
			projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
			projectile.width = 10;
			projectile.height = 10;
			projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
			projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
		}
	}
}