using Ougon.Data;
using SharpDX.Mathematics.Interop;

namespace Ougon
{
    class Context
    {
        public unsafe Match* match { get; set; }
        public unsafe Game* game { get; set; }
        public bool timerLocked = false;
        public Fight? fight;
        public Mod? mod;

        public RawViewport? viewport;
    }
}
