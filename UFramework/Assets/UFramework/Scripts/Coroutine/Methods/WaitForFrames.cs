namespace UFramework.Coroutine
{
    public class WaitForFrames : YieldInstruction
    {
        private int _curCont;
        private readonly int _count;

        public WaitForFrames(int count)
        {
            _curCont = 0;
            _count = count;
        }

        protected override bool IsCompleted()
        {
            _curCont++;
            return _curCont >= _count;
        }
    }
}