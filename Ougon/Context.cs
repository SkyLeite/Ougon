using Ougon.Misc;
using Ougon.Template;
using static Ougon.Mod;

namespace Ougon
{
    class Context
    {
        public unsafe Match* match { get; set; }
        public FormatDDS? FormatDDS { get; internal set; }
        public D3DXCreateTextureFromFileInMemoryEx? CreateTexture { get; internal set; }
        public unsafe IntPtr TextureAddresses { get; internal set; }

        public bool timerLocked = false;
        public Fight? fight;
        public DirectX directX;

        public Context(ModContext context) {
            this.directX = new DirectX(context);
        }
    }
}
