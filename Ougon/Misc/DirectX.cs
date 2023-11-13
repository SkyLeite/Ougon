using System.Runtime.InteropServices;
using Ougon.Template;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Structs;
using Reloaded.Hooks.Definitions.X86;
using Reloaded.Memory.Pointers;
using SharpDX.Direct3D9;
using SharpDX.Mathematics.Interop;

namespace Ougon.Misc
{
    public class DirectX {
        public IFunction<D3DXCreateTextureFromFileInMemoryExPtr> CreateTexture;

        public DirectX(ModContext modContext) {
            var hooks = modContext.Hooks;
            var d3dx9Handle = LoadLibrary("d3dx9_43.dll");
            this.CreateTexture = hooks.CreateFunction<D3DXCreateTextureFromFileInMemoryExPtr>((long)GetProcAddress(d3dx9Handle, "D3DXCreateTextureFromFileInMemoryEx"));
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr LoadLibrary(string libraryName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [FunctionHookOptions(PreferRelativeJump = true)]
        [Function(CallingConventions.Stdcall)]
        public struct D3DXCreateTextureFromFileInMemoryExPtr
        {
            public FuncPtr<Ptr<byte>, Ptr<byte>, int, int, int, int, Usage, Format, Pool, int, int,
                RawColorBGRA, Ptr<byte>, Ptr<PaletteEntry>, Ptr<Ptr<byte>>, int> Ptr;
        }

        public static unsafe Ptr<Ptr<T>> ToPtr<T>(T** item) where T : unmanaged
        {
            return new Ptr<Ptr<T>>((Ptr<T>*)item);
        }
    }
}
