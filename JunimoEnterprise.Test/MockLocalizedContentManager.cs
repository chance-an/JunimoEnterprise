using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JunimoEnterprise.Test
{
    public class MockLocalizedContentManager: LocalizedContentManager
    {
        public MockLocalizedContentManager(): base(new ServiceContainer(), "")
        {
        }

        public override T Load<T>(string assetName)
        {
            if (typeof(T) == typeof(Dictionary<int, string>))
            {
                var dict = new Dictionary<int, string>();

                for (int i = 0; i < 1000; i++)
                {
                    string nameValue;
                    if (assetName == "Data\\hats")
                    {
                        nameValue = "hat/hat/true/true";
                    }
                    else if (assetName == "Data\\Furniture")
                    {
                        nameValue = "name/chair/-1/-1/-1/-1";
                    }
                    else
                    {
                        nameValue = "some_name";
                    }
                    dict.Add(i, nameValue);
                }

                return (T)Convert.ChangeType(dict, typeof(T));
            } else if (typeof(T) == typeof(Texture2D))
            {
                return default(T);
            }
            return base.Load<T>(assetName);
        }
        
    }
}
