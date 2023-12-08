namespace Ougon.Data;

[StructLayout(LayoutKind.Explicit)]
public unsafe struct LZLRFile
{
    [FieldOffset(0x0)]
    public char* LZLR;

    [FieldOffset(0x4)]
    public int DDSSize;

    [FieldOffset(0x8)]
    public short DDSOffset;
}
