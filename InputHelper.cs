using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JunimoIntelliBox
{
    class InputHelper
    {
        public bool IsOneOfTheseKeysDown(SButton button, InputButton[] keys)
        {
            IEnumerable<InputButton> result = keys.Where(b => ((int)b.key) == (int)button);
            return result.Any();
        }
    }
}
