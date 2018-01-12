using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JunimoIntelliBox.Plans
{
    public interface IPlan
    {
        PlanExecutionResult Execute();
    }

    public enum PlanExecutionResult {
        SUCCESS,
        FAILED,
        CONTINUE
    }
}


