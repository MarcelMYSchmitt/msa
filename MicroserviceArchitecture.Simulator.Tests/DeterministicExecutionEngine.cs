using MicroserviceArchitecture.Simulator.ExecutionEngine;
using System;
using System.Threading.Tasks;

namespace MicroserviceArchitecture.Simulator.Tests
{
    public class DeterministicExecutionEngine : IExecutionEngine
    {
        private readonly int _numberOfExecutions;

        public DeterministicExecutionEngine(int numberOfExecutions)
        {
            _numberOfExecutions = numberOfExecutions;
        }

        public async Task Execute(Func<Task> function)
        {
            for (var i = 0; i < _numberOfExecutions; i++)
            {
                await function();
            }
        }
    }
}