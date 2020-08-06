﻿using Microsoft.Xna.Framework;
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
			projectile.width = 14;
			projectile.height = 14;
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

			if (TargetWhoAmI >= MAX_TICKS)
			{
				const float velXmult = 0.90f; // x velocity factor, every AI update the x velocity will be 98% of the original speed
				const float velYmult = 0.45f; // y velocity factor, every AI update the y velocity will be be 0.35f bigger of the original speed, causing the javelin to drop to the ground
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