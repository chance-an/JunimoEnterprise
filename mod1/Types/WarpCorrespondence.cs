using System;
namespace JunimoIntelliBox.Types
{
    using JunimoEnterpriseNative.JEGraphUtilities;
    using System.Xml.Serialization;

    public class WarpCorrespondence: IEdgeDescriber<WarpCorrespondence>
    {
        public Tuple<int, int> from;
        public Tuple<int, int> to;
        public WarpCorrespondence()
        {

        }
        public WarpCorrespondence(GameLocationNavigationNode node)
        {
            from = new Tuple<int, int>(node.CurrentX, node.CurrentY);
            to = new Tuple<int, int>(node.TargetX, node.TargetY);
        }

        WarpCorrespondence IEdgeDescriber<WarpCorrespondence>.Reverse()
        {
            WarpCorrespondence instance = new WarpCorrespondence();
            instance.from = this.to;
            instance.to = this.from;
            return instance;
        }

        public static implicit operator WarpCorrespondence(GameLocationNavigationNode node)
        {
            return new WarpCorrespondence(node);
        }
    }
}
