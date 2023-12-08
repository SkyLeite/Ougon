namespace Ougon.Data;

[StructLayout(LayoutKind.Explicit)]
public unsafe struct GameCharacterInMatch
{
    [FieldOffset(0x41c)]
    public int health;

    // When over 1, the next knockdown stuns
    [FieldOffset(0x40c)]
    public float stun;

    [FieldOffset(0x420)]
    public float meter;

    [FieldOffset(0x3d0)]
    public float positionX;

    [FieldOffset(0x3d4)]
    public float positionY;
}
