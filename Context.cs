namespace Ougon
{
    class Context
    {
        public unsafe Match* match { get; set; }
        public bool timerLocked = false;
        public Fight? fight;
    }
}
