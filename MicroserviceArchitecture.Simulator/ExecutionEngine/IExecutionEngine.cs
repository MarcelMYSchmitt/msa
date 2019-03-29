using System;
using System.Threading.Tasks;

namespace MicroserviceArchitecture.Simulator.ExecutionEngine
{
    public interface IExecutionEngine
    {
        Task Execute(Func<Task> action);
    }
}