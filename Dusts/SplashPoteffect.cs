using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using static Terraria.ModLoader.ModContent;
using System;


namespace MinecraftAnimals.Dusts
{
	public class SplashPoteffect : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			float leftOrRight = 1;
			if (Main.rand.NextBool())
			{
				leftOrRight *= -1;
			}
			dust.velocity.X = Main.rand.NextFloat(3f, 10f) * leftOrRight;
			dust.noGravity = true;
			dust.velocity.Y = -0.15f;
			dust.scale = 2f;
			dust.color = Color.HotPink;
		}

		public override bool Update(Dust dust)
		{
			dust.velocity.Y -= -0.05f;
			dust.velocity.X *= 0.9f;
			dust.position += dust.velocity;
			dust.rotation += dust.velocity.X * 0.1f;
			dust.scale -= 0.035f;
			if (dust.scale < 0.5f)
			{
				dust.active = false;
			}
			return false;
		}
	}
}
