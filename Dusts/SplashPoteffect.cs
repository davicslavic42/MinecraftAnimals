using Terraria;
using Terraria.ModLoader;


namespace MinecraftAnimals.Dusts
{
	public class SplashPoteffect : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			dust.noGravity = true;
			dust.velocity.X = 7.75f * Main.rand.Next(-1, 1);
			dust.velocity.Y = -1.25f;
			dust.scale = 7.5f;
		}

		public override bool Update(Dust dust)
		{
			dust.velocity.Y *= 1.15f;
			dust.velocity.X *= 0.45f;
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
