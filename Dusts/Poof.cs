using Terraria;
using Terraria.ModLoader;


namespace MinecraftAnimals.Dusts
{
	public class Poof : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			dust.velocity.X = 2.45f * Main.rand.Next(-1 ,1);
			dust.velocity.Y = 0.1f;
			dust.noGravity = true;
			dust.scale = 2f;
		}

		public override bool Update(Dust dust)
		{
			dust.velocity.X *= 0.45f;
			dust.velocity.Y -= 0.35f;
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
