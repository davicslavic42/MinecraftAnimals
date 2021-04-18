using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;
using System.Collections.Generic;
using static Terraria.ModLoader.ModContent;
using Terraria.ID;
using MinecraftAnimals.Animals.OceanNpcs;

namespace MinecraftAnimals.projectiles
{
	public class GuardianLaser : ModProjectile
	{
		public override void SetDefaults()
		{
			projectile.width = 10;
			projectile.height = 10;
			projectile.friendly = false;
			projectile.hostile = true;
			projectile.penetrate = 1;
			projectile.hide = true;
			projectile.damage = 5;
			projectile.tileCollide = false;
			projectile.timeLeft = 900;
		}
		public int ParentIndex
		{
			get => (int)projectile.ai[0] - 1;
			set => projectile.ai[0] = value + 1;
		}
		public bool HasParent => ParentIndex > -1;
		internal float Distance
		{
			get => projectile.ai[1];
			set => projectile.ai[1] = value;
		}
		internal int TargetWhoAmI
		{
			get => (int)projectile.ai[2];
			set => projectile.ai[2] = value;
		}
		internal ref float Charge => ref projectile.localAI[0];

		internal const float MAX_CHARGE = 300f;//charge time before the laser deals damage
		internal const float MOVE_DISTANCE = 60f;//Will be ajustd to be the distance to player as it needs to lock on
		bool IsAtMaxCharge => Charge == MAX_CHARGE;
		internal enum AIStates
		{
			Charge = 0,
			Attack = 1,
		}
		private void UpdateGuardian(NPC npc)
		{
			
		}

		// The core function of drawing a laser

		// Change the way of collision check of the projectile

		// Set custom immunity time on hitting an NPC
		private void SetLaserPosition()
		{
			NPC parentNPC = Main.npc[ParentIndex];
			Player playerTarget = Main.player[parentNPC.target];//TODO loop through player array and check the vector position of the target(because i use a different method) and use that vecotr as a ref point for targeting
			float distanceToPlayer = Vector2.Distance(parentNPC.Center, playerTarget.Center);
			for (Distance = MOVE_DISTANCE; Distance <= distanceToPlayer; Distance += 5f)
			{
				var start = parentNPC.Center + projectile.velocity * Distance;
				if (!Collision.CanHit(parentNPC.Center, 1, 1, start, 1, 1))
				{
					Distance -= 5f;
					break;
				}
			}
		}

		// The AI of the projectile
		public override void AI()
		{
			NPC parentNPC = Main.npc[ParentIndex];
			Player playerTarget = Main.player[parentNPC.target];//TODO loop through player array and check the vector position of the target(because i use a different method) and use that vecotr as a ref point for targeting
			Vector2 aim = Vector2.Normalize(parentNPC.Center - playerTarget.Center);
			int killTime = 0;
			float distanceToPlayer = Vector2.Distance(parentNPC.Center, playerTarget.Center);
			SetLaserPosition();
			projectile.position = parentNPC.Center + projectile.velocity * MOVE_DISTANCE;
			projectile.timeLeft = 20;
			projectile.damage = 0;
			projectile.tileCollide = true;
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				projectile.velocity = aim;
				projectile.rotation = aim.ToRotation();
				projectile.direction = parentNPC.position.X > playerTarget.position.X ? 1 : -1;
				projectile.netUpdate = true;
			}
			if (parentNPC.type == NPCType<Guardian>() && parentNPC.ai[1] != (int)Guardian.AIStates.Attack && !IsAtMaxCharge)/*checks if the guardian is in attack mode or not*/ projectile.Kill();
			else Charge++;// ifthe type is right an in attack mode charge
			if (IsAtMaxCharge)
			{
				projectile.damage = 30;
				killTime++;
			}
			if (killTime == 20) projectile.Kill();
			SetLaserPosition();

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
				player.Center + unit * Distance, 22, ref point);
		}
		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.immune[projectile.owner] = 10;
		}

		/*
		 * Sets the end of the laser position based on where it collides with something
		 * float distanceToPlayer = new Vector2(player.center - npc.center)
		 */
		public void DrawLaser(SpriteBatch spriteBatch, Texture2D texture, Vector2 start, Vector2 unit, float step, int damage, float rotation = 0f, float scale = 1f, float maxDist = 2000f, Color color = default(Color), int transDist = 50)
		{
			float r = unit.ToRotation() + rotation;

			// Draws the laser 'body'
			for (float i = transDist; i <= Distance; i += step)
			{
				Color c = Color.White;
				var origin = start + i * unit;
				spriteBatch.Draw(texture, origin - Main.screenPosition,
					new Rectangle(0, 26, 28, 26), i < transDist ? Color.Transparent : c, r,
					new Vector2(28 * .5f, 26 * .5f), scale, 0, 0);
			}

			// Draws the laser 'tail'
			spriteBatch.Draw(texture, start + unit * (transDist - step) - Main.screenPosition,
				new Rectangle(0, 0, 28, 26), Color.White, r, new Vector2(28 * .5f, 26 * .5f), scale, 0, 0);

			// Draws the laser 'head'
			spriteBatch.Draw(texture, start + (Distance + step) * unit - Main.screenPosition,
				new Rectangle(0, 52, 28, 26), Color.White, r, new Vector2(28 * .5f, 26 * .5f), scale, 0, 0);
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			// We start drawing the laser if we have charged up
			if (IsAtMaxCharge)
			{
				DrawLaser(spriteBatch, Main.projectileTexture[projectile.type], Main.player[projectile.owner].Center,
					projectile.velocity, 10, projectile.damage, -1.57f, 1f, 1000f, Color.White, (int)MOVE_DISTANCE);
			}
			return false;
		}

		/*
		private void ChargeLaser(Player player)
		{
			// Kill the projectile if the player stops channeling
			if (!player.channel)
			{
				projectile.Kill();
			}
			else
			{
				// Do we still have enough mana? If not, we kill the projectile because we cannot use it anymore
				if (Main.time % 10 < 1 && !player.CheckMana(player.inventory[player.selectedItem].mana, true))
				{
					projectile.Kill();
				}
				Vector2 offset = projectile.velocity;
				offset *= MOVE_DISTANCE - 20;
				Vector2 pos = player.Center + offset - new Vector2(10, 10);
				if (Charge < MAX_CHARGE)
				{
					Charge++;
				}
				int chargeFact = (int)(Charge / 20f);
				Vector2 dustVelocity = Vector2.UnitX * 18f;
				dustVelocity = dustVelocity.RotatedBy(projectile.rotation - 1.57f);
				Vector2 spawnPos = projectile.Center + dustVelocity;
				for (int k = 0; k < chargeFact + 1; k++)
				{
					Vector2 spawn = spawnPos + ((float)Main.rand.NextDouble() * 6.28f).ToRotationVector2() * (12f - chargeFact * 2);
					Dust dust = Main.dust[Dust.NewDust(pos, 20, 20, 226, projectile.velocity.X / 2f, projectile.velocity.Y / 2f)];
					dust.velocity = Vector2.Normalize(spawnPos - spawn) * 1.5f * (10f - chargeFact * 2f) / 10f;
					dust.noGravity = true;
					dust.scale = Main.rand.Next(10, 20) * 0.05f;
				}
			}
		}
		*/
		public override bool ShouldUpdatePosition() => false;

		/*
		 */
	}
}