using Ougon.Data;

namespace Ougon
{
    class Context
    {
        public unsafe GameState* gameState { get; set; }
        public unsafe Match* match { get; set; }
        public bool timerLocked = false;
        public Fight? fight;
    }
}
