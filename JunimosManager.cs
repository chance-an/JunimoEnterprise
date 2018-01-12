
namespace JunimoIntelliBox
{
    using Microsoft.Xna.Framework;
    using StardewModdingAPI;
    using StardewValley;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    public class JunimosManager
    {
        public JunimoSlave CreateJunimo(GameLocation location, Vector2 position, IMonitor monitor)
        {
            JunimoSlave junimo = new JunimoSlave(position, monitor);
            location.addCharacter(junimo);
            return junimo;
        }
    }
}
