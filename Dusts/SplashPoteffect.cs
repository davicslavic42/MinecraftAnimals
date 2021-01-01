using Terraria;
using Terraria.ModLoader;


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
			dust.velocity.X = Main.rand.NextFloat(4f, 12f) * leftOrRight;
			dust.noGravity = true;
			dust.velocity.Y = -0.35f;
			dust.scale = 6.5f;
		}

		public override bool Update(Dust dust)
		{
			dust.velocity.Y -= 0.1f;
			dust.velocity.X *= 0.8f;
			dust.position += dust.velocity;
			dust.rotation += dust.velocity.X * 0.1f;
			dust.scale -= 0.015f;
			if (dust.scale < 0.5f)
			{
				dust.active = false;
			}
			return false;
		}
	}
}
