

namespace JunimoIntelliBox.Mocks
{
    using StardewValley;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class MockGameLocation: GameLocation
    {
        public MockGameLocation(string name) : base()
        {
            this.name.Value = name;
        }
    }
}
