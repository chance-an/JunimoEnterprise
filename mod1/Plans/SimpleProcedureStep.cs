using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JunimoIntelliBox.Plans
{
    public class SimpleProcedureStep : AbstractStep
    {
        private Func<ExecutionResult> procedure;

        public SimpleProcedureStep(Func<ExecutionResult> procedure)
        {
            this.procedure = procedure;
        }
        public override void FirstTick()
        {
        }

        public override ExecutionResult Tick()
        {
            return this.procedure();
        }
    }
}
