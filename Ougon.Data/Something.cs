namespace Ougon.Data;

[StructLayout(LayoutKind.Explicit)]
public unsafe struct Something
{
    [FieldOffset(0x2a604)]
    public GameCharacter* character;
}
