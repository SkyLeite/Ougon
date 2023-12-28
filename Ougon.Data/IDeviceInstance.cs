namespace Ougon.Data;

[StructLayout(LayoutKind.Explicit)]
public unsafe struct IDeviceInstance {
    [FieldOffset(0x48)]
    public fixed byte InstanceGuid[16];

    [FieldOffset(0x58)]
    public fixed byte ProductGuid[16];
}
