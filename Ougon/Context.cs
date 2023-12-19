using Ougon.Data;

namespace Ougon;

sealed class Context
{
    public unsafe GameState* gameState { get; set; }
    public unsafe Match* match { get; set; }
    public bool timerLocked;
    public Fight? fight;
}
