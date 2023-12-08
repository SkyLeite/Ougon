namespace Ougon.Data;

[StructLayout(LayoutKind.Explicit)]
public unsafe struct Sequence
{
    [FieldOffset(0x0)]
    public SequenceHeader header;

    [FieldOffset(0x44)]
    public Frame frame0;

    [FieldOffset(0x44 + (0x60 * 1))]
    public Frame frame1;

    [FieldOffset(0x44 + (0x60 * 2))]
    public Frame frame2;

    [FieldOffset(0x44 + (0x60 * 3))]
    public Frame frame3;

    [FieldOffset(0x44 + (0x60 * 4))]
    public Frame frame4;

    [FieldOffset(0x44 + (0x60 * 5))]
    public Frame frame5;

    [FieldOffset(0x44 + (0x60 * 6))]
    public Frame frame6;

    [FieldOffset(0x44 + (0x60 * 7))]
    public Frame frame7;

    [FieldOffset(0x44 + (0x60 * 8))]
    public Frame frame8;

    [FieldOffset(0x44 + (0x60 * 9))]
    public Frame frame9;

    [FieldOffset(0x44 + (0x60 * 10))]
    public Frame frame10;

    [FieldOffset(0x44 + (0x60 * 11))]
    public Frame frame11;

    [FieldOffset(0x44 + (0x60 * 12))]
    public Frame frame12;

    [FieldOffset(0x44 + (0x60 * 13))]
    public Frame frame13;

    [FieldOffset(0x44 + (0x60 * 14))]
    public Frame frame14;

    [FieldOffset(0x44 + (0x60 * 15))]
    public Frame frame15;

    [FieldOffset(0x44 + (0x60 * 16))]
    public Frame frame16;

    [FieldOffset(0x44 + (0x60 * 17))]
    public Frame frame17;

    [FieldOffset(0x44 + (0x60 * 18))]
    public Frame frame18;

    [FieldOffset(0x44 + (0x60 * 19))]
    public Frame frame19;

    [FieldOffset(0x44 + (0x60 * 20))]
    public Frame frame20;

    public unsafe Frame*[] frames()
    {
        var length = 21;
        Frame*[] frameArray = new Frame*[length];

        fixed (Frame* pFrame0 = &frame0)
        fixed (Frame* pFrame1 = &frame1)
        fixed (Frame* pFrame2 = &frame2)
        fixed (Frame* pFrame3 = &frame3)
        fixed (Frame* pFrame4 = &frame4)
        fixed (Frame* pFrame5 = &frame5)
        fixed (Frame* pFrame6 = &frame6)
        fixed (Frame* pFrame7 = &frame7)
        fixed (Frame* pFrame8 = &frame8)
        fixed (Frame* pFrame9 = &frame9)
        fixed (Frame* pFrame10 = &frame10)
        fixed (Frame* pFrame11 = &frame11)
        fixed (Frame* pFrame12 = &frame12)
        fixed (Frame* pFrame13 = &frame13)
        fixed (Frame* pFrame14 = &frame14)
        fixed (Frame* pFrame15 = &frame15)
        fixed (Frame* pFrame16 = &frame16)
        fixed (Frame* pFrame17 = &frame17)
        fixed (Frame* pFrame18 = &frame18)
        fixed (Frame* pFrame19 = &frame19)
        fixed (Frame* pFrame20 = &frame20)
        {
            frameArray[0] = pFrame0;
            frameArray[1] = pFrame1;
            frameArray[2] = pFrame2;
            frameArray[3] = pFrame3;
            frameArray[4] = pFrame4;
            frameArray[5] = pFrame5;
            frameArray[6] = pFrame6;
            frameArray[7] = pFrame7;
            frameArray[8] = pFrame8;
            frameArray[9] = pFrame9;
            frameArray[10] = pFrame10;
            frameArray[11] = pFrame11;
            frameArray[12] = pFrame12;
            frameArray[13] = pFrame13;
            frameArray[14] = pFrame14;
            frameArray[15] = pFrame15;
            frameArray[16] = pFrame16;
            frameArray[17] = pFrame17;
            frameArray[18] = pFrame18;
            frameArray[19] = pFrame19;
            frameArray[20] = pFrame20;
        }

        return frameArray;
    }
};
