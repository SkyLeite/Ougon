using System.Runtime.InteropServices;

namespace Ougon
{

    [StructLayout(LayoutKind.Explicit)]
    public struct GameState
    {
        [FieldOffset(0x480)]
        public double fps;

        [FieldOffset(0x4b4)]
        public bool debugDisabled;

        [FieldOffset(0x3c)]
        public bool isFullScreen;

        [FieldOffset(0x30)]
        public int isRunning;

        [FieldOffset(0x34)]
        public bool isWindowed;
    }
}
