using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JunimoIntelliBox.Plans
{
    public interface IStep
    {
        void FirstTick(); 

        ExecutionResult Tick();

        ExecutionResult RunTick();

    }
}
