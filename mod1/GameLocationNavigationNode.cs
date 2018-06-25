namespace JunimoIntelliBox
{
    using StardewValley;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GameLocationNavigationNode
    {
        public GameLocation current;
        public GameLocation next;
        public int CurrentX { get; }
        public int CurrentY { get; }
        public int TargetX { get; }
        public int TargetY { get; }
        public string TargetName { get; }

        public GameLocationNavigationNode(GameLocation current, GameLocation next, int currentX, int currentY, int targetX, int targetY, string targetName)
        {
            this.current = current;
            this.next = next;
            this.CurrentX = currentX;
            this.CurrentY = currentY;
            this.TargetX = targetX;
            this.TargetY = targetY;
            this.TargetName = targetName;
        }
    }
}
