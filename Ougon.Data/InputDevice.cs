namespace Ougon.Data;

[StructLayout(LayoutKind.Explicit)]
public unsafe struct InputDevice {
    [FieldOffset(0x0)]
    public int isActive;

    [FieldOffset(0x34)]
    public byte a;

    [FieldOffset(0x35)]
    public byte b;

    [FieldOffset(0x36)]
    public byte c;

    [FieldOffset(0x37)]
    public byte d;

    [FieldOffset(0x38)]
    public byte taunt;

    [FieldOffset(0x39)]
    public byte start;

    [FieldOffset(0x3a)]
    public byte ab;

    [FieldOffset(0x3b)]
    public byte bc;

    [FieldOffset(0x3c)]
    public byte abc;

    [FieldOffset(0x40)]
    public byte direction1;

    [FieldOffset(0x41)]
    public byte direction2;

    [FieldOffset(0x42)]
    public byte direction3;

    [FieldOffset(0x43)]
    public byte direction4;

    [FieldOffset(0x44)]
    public bool useAnalogStick;
}
