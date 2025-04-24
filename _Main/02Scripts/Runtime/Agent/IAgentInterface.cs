using Main.Runtime.Agents;

namespace Main.Runtime.Core
{
    public interface IAfterInitable
    {
        public void AfterInitialize();
    }

    public interface IAgentComponent
    {
        public void Initialize(Agent agent);
    }
}