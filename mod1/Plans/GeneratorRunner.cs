using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JunimoIntelliBox.Plans
{
    public abstract class GeneratorRunner
    {
        public abstract IEnumerable<ExecutionResult> Run();

        private IEnumerator<ExecutionResult> enumerator;

        public ExecutionResult RunGenerator()
        {
            if (this.enumerator == null)
            {
                this.enumerator = this.Run().GetEnumerator();
            }

            bool hasNext = this.enumerator.MoveNext();

            if (!hasNext)
            {
                return ExecutionResult.SUCCESS;
            }
            
            return enumerator.Current;
        }

        public void ResetGenerator()
        {
            this.enumerator = null;
        }

    }
}
