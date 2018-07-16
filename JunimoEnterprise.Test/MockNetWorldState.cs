
namespace JunimoEnterprise.Test
{
    using Netcode;
    using StardewValley;
    using StardewValley.Network;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    public class MockNetWorldState : IWorldState
    {
        public WorldDate Date => new WorldDate();

        public bool IsPaused { get => true; set => throw new NotImplementedException(); }
        public bool IsGoblinRemoved { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsSubmarineLocked { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int LowestMineLevel { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int WeatherForTomorrow { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public NetBundles Bundles => throw new NotImplementedException();

        public NetIntDictionary<bool, NetBool> BundleRewards => throw new NotImplementedException();

        public NetVector2Dictionary<int, NetInt> MuseumPieces => throw new NotImplementedException();

        public NetFields NetFields => new NetFields();

        public void UpdateFromGame1()
        {
            throw new NotImplementedException();
        }

        public void WriteToGame1()
        {
            throw new NotImplementedException();
        }
    }
}
