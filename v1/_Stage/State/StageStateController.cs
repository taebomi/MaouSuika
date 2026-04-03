namespace SOSG.Stage.State
{
    public abstract class StageStateController
    {
        public abstract StageState State { get; }
        protected StageStateManager Manager { get; private set; }

        protected StageStateController()
        {
            
        }

        public StageStateController(StageStateManager manager)
        {
            Manager = manager;
        }

        public abstract void OnStateEnter();
        public abstract void OnStateExit();
    }
}