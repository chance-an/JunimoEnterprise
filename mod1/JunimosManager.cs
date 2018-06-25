
namespace JunimoIntelliBox
{
    using JunimoIntelliBox.Plans;
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
        private IMonitor monitor;
        public JunimosManager(IMonitor monitor)
        {
            this.monitor = monitor;
        }
        public JunimoSlave CreateJunimo(GameLocation location, Vector2 position)
        {
            JunimoSlave junimo = new JunimoSlave(position, monitor);
            location.addCharacter(junimo);
            junimo.currentLocation = location;
            return junimo;
        }

        public JunimoSlave RequestJunimoToExecutePlan(IPlan plan)
        {
            Vector2 playerPosition = Game1.player.position;
            Vector2 newNPCPosition = playerPosition + new Vector2(0, 80);

            return this.CreateJunimo(Game1.player.currentLocation, newNPCPosition);
        }
    }
}
