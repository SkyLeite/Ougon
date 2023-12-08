namespace Ougon.Data;

[StructLayout(LayoutKind.Explicit)]
public unsafe struct Sprite
{
    [FieldOffset(0x8)]
    public LZLRFile* lzlr;
}
