namespace Ougon.Data;

[StructLayout(LayoutKind.Explicit)]
public struct SequenceHeader
{
    [FieldOffset(0x0)]
    public int movementState;

    [FieldOffset(0x4)]
    public int movementState2;

    [FieldOffset(0x8)]
    public int movementState3;

    [FieldOffset(0x40)]
    public uint frame_count;
}
