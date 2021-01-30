using Terraria;
using Terraria.ModLoader;
using System.Linq;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using static Terraria.ModLoader.ModContent;
using System;
using System.Security.Cryptography.X509Certificates;
using MinecraftAnimals.BaseAI;


namespace MinecraftAnimals.Dusts
{
	public class Poof : ModDust
	{
		Vector2 angle = Vector2.UnitX.RotateRandom(Math.PI);
		public override void OnSpawn(Dust dust)
		{
			float leftOrRight = 1f;
			if (Main.rand.NextBool())
            {
				leftOrRight *= -1f;
            }
			dust.velocity.X = Main.rand.NextFloat(0f, 4f) * leftOrRight;
			dust.velocity.Y = -0.25f;
			dust.noGravity = true;
			dust.scale = 2.5f;
		}

		public override bool Update(Dust dust)
		{
			dust.velocity.Y -= 0.1f;
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
