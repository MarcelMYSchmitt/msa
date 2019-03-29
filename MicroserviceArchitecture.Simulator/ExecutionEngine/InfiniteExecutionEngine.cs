using System;
using System.Threading.Tasks;

namespace MicroserviceArchitecture.Simulator.ExecutionEngine
{
    public class InfiniteRepeatExecutionEngine : IExecutionEngine
    {
        public async Task Execute(Func<Task> function)
        {
            while (true)
            {
                await function();
            }
        }
    }
}
