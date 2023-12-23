namespace Ougon.Data;

[StructLayout(LayoutKind.Explicit)]
public unsafe struct Sequence
{
    [FieldOffset(0x0)]
    public SequenceHeader header;

    [FieldOffset(0x44)]
    public Frame frame0;

    public unsafe Frame*[] Frames()
    {
        var length = this.header.frame_count;
        Frame*[] frameArray = new Frame*[length];

        fixed (Frame* pFrames = &frame0)
        {
            for (int i = 0; i < length; i++)
            {
                frameArray[i] = pFrames + i;
            }
        }

        return frameArray;
    }

    public FrameData GetFrameData() {
        var frames = this.Frames();
        var frameData = new FrameData();

        foreach (var frame in frames) {
            if (frame->duration < 0 || frame->duration > 500) {
                continue;
            }

            var isActive = frame->attackbox.w > 0 || frame->attackbox.h > 0;

            if (frameData.active == 0 && isActive == false) {
                frameData.startup += frame->duration;
                continue;
            }

            if (isActive) {
                frameData.active += frame->duration;
                continue;
            }

            if (frameData.startup != 0 && isActive == false) {
                frameData.recovery += frame->duration;
                continue;
            }
        }

        return frameData;
    }

    public bool IsIdle() {
        return this.GetFrameData().active == 0;
    }
};
