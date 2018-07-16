using System;
namespace JunimoIntelliBox.Types
{
    using JunimoEnterpriseNative;
    using JunimoEnterpriseNative.JEGraphUtilities;
    using System.Xml.Serialization;

    public class WarpCorrespondence: IEdgeDescriber<WarpCorrespondence>
    {
        public SerializableTuple<int, int> from;
        public SerializableTuple<int, int> to;
        public WarpCorrespondence()
        {

        }
        public WarpCorrespondence(GameLocationNavigationNode node)
        {
            from = new SerializableTuple<int, int>(node.CurrentX, node.CurrentY);
            to = new SerializableTuple<int, int>(node.TargetX, node.TargetY);
        }

        public WarpCorrespondence Reverse()
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
