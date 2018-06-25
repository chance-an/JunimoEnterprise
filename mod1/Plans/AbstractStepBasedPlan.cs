using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JunimoIntelliBox.Plans
{
    abstract class AbstractStepBasedPlan : IPlan
    {
        private Queue<Func<IStep>> steps;
        private IStep currentStep;
        protected abstract Func<IStep>[] SetupSteps();

        public AbstractStepBasedPlan()
        {
        }

        public PlanExecutionResult Execute()
        {
            if (this.steps == null)
            {
                this.steps = new Queue<Func<IStep>>(this.SetupSteps());
            }

            if (this.currentStep == null)
            {
                if (this.steps.Count == 0)
                {
                    return PlanExecutionResult.SUCCESS;
                }
                this.currentStep = this.steps.Dequeue()();
            }

            ExecutionResult result = this.currentStep.RunTick();

            if (result != ExecutionResult.CONTINUE)
            {
                this.currentStep = null;
                if (result == ExecutionResult.FAILED)
                {
                    return PlanExecutionResult.FAILED;
                }
                return PlanExecutionResult.CONTINUE;
            }

            if (this.steps.Count == 0)
            {
                return PlanExecutionResult.SUCCESS;
            }

            return PlanExecutionResult.CONTINUE;

        }
    }
}
