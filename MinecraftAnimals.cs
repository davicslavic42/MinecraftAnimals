using MinecraftAnimals.Animals;
using Terraria;
using Terraria.ModLoader;

namespace MinecraftAnimals
{
    public class MinecraftAnimals : Mod
    {
        static internal MinecraftAnimals instance;
        public MinecraftAnimals()
        {
            Properties = new ModProperties()
            {
                Autoload = true,
                AutoloadGores = true,
                AutoloadSounds = true,
                AutoloadBackgrounds = true
            };
        }

        public override void Load()
        {
            if (!Main.dedServ)
            instance = this;
        }
        public override void Unload()
        {
            if (!Main.dedServ) 
            {
            }
            instance = null;
        }
    }
}