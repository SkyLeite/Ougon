namespace Ougon.Data;

using SharpDX;
using SharpDX.Direct3D9;

[StructLayout(LayoutKind.Explicit)]
public unsafe struct Game
{
    [FieldOffset(0x4)]
    public int* windowHandle;

    [FieldOffset(0x10)]
    public Int16 width;

    [FieldOffset(0x12)]
    public Int16 height;

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

    [FieldOffset(0x4c0)]
    public int PresentationParameters;

    [FieldOffset(0x4f0)]
    public int* IDirect3D9;

    [FieldOffset(0x4f4)]
    public int* DX9Device;

    public Device GetDevice() {
        return CppObject.FromPointer<Device>((IntPtr)this.DX9Device);
    }
}
