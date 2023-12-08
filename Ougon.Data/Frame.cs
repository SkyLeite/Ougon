namespace Ougon.Data;

[StructLayout(LayoutKind.Sequential, Pack = 2, Size = 96)]
public struct Frame
{
    public ushort sprite_id;
    public ushort duration;
    public ushort unknown02;
    public short tex_x;
    public short tex_y;

    public ushort unknown05;
    public ushort unknown06;
    public ushort unknown07;
    public ushort unknown08;
    public ushort unknown09;
    public ushort unknown0a;
    public ushort unknown0b;
    public ushort unknown0c;
    public ushort unknown0d;

    public Hitbox attackbox;

    public Hitbox hitbox1;
    public Hitbox hitbox2;
    public Hitbox hitbox3;

    public ushort unknown1e;
    public ushort unknown1f;
    public ushort unknown20;
    public ushort unknown21;

    public struct Attack
    {
        public byte damage;
        public byte prop1;
        public ushort prop2;
        public ushort prop3;
        public ushort prop4;
    }

    public Attack attack;

    public ushort unknown26;
    public ushort unknown27;
    public ushort unknown28;
    public ushort unknown29;
    public ushort unknown2a;
    public ushort unknown2b;
    public ushort unknown2c;
    public ushort unknown2d;
    public ushort unknown2e;
    public ushort unknown2f;
}
