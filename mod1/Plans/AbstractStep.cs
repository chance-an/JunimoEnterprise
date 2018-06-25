using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JunimoIntelliBox.Plans
{
    public abstract class AbstractStep : IStep
    {
        private Func<ExecutionResult> tickProc;
        public AbstractStep()
        {
            this.tickProc = delegate()
            {
                this.FirstTick();
                this.tickProc = delegate ()
                {
                    return this.Tick();
                };
                return ExecutionResult.CONTINUE;
            };
        }
        public abstract void FirstTick();
        public ExecutionResult RunTick() {

            return this.tickProc();
        }
        public abstract ExecutionResult Tick();
    }
}
