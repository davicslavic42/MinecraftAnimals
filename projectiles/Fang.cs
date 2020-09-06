﻿using Microsoft.Xna.Framework;
using MinecraftAnimals.Items.Weapons;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using static Terraria.ModLoader.ModContent;

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
			projectile.width = 42;
			projectile.height = 42;
			projectile.friendly = false;
			projectile.hostile = true;
			projectile.hide = false;
			projectile.penetrate = 20;
			projectile.timeLeft = 30;

			//1: projectile.penetrate = 1; // Will hit even if npc is currently immune to player
			//2a: projectile.penetrate = -1; // Will hit and unless 3 is use, set 10 ticks of immunity
			//2b: projectile.penetrate = 3; // Same, but max 3 hits before dying
			//5: projectile.usesLocalNPCImmunity = true;
			//5a: projectile.localNPCHitCooldown = -1; // 1 hit per npc max
			//5b: projectile.localNPCHitCooldown = 20; // o
		}
		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			// Die immediately if ai[1] isn't 0 (We set this to 1 for the 5 extra explosives we spawn in Kill)
			if (projectile.timeLeft != 0)
			{
				projectile.timeLeft = 30;
			}
			return false;
		}
	
		public override void AI()
		{
			projectile.frameCounter++;
			if (++projectile.frameCounter >= 5)
			{
				projectile.frameCounter = 0;
				projectile.frame = ++projectile.frame % Main.projFrames[projectile.type];
			}           // Kill this projectile after 1 second
			if (projectile.ai[0] >= 60f)
			{
				projectile.Kill();
			}
		}
	}
}