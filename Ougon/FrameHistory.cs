using Ougon.Data;

namespace Ougon;

enum FrameKind {
    Startup = 0,
    Active = 1,
    Recovery = 2
}

sealed class FrameHistory : List<IntPtr> {
    private int? firstActiveFrameIndex;

    private unsafe Frame* getFrame(int index) {
        var frame = (Frame*)this[index];

        if (frame == null)
            throw new NullReferenceException("Frame is null");

        return frame;
    }

    private List<IntPtr> Revert() {
        var newList = new List<IntPtr>(this);
        newList.Reverse();

        return newList;
    }

    // Returns true if any of the previous elements has been an active frame
    private unsafe bool HasBeenActive() {
        foreach (Frame* previousFrame in this.Revert()) {
            if (previousFrame->isActive()) {
                return true;
            }
        }

        return false;
    }

    public unsafe FrameKind Kind(int index) {
        var frame = getFrame(index);

        if (frame->isActive()) {
            if (this.firstActiveFrameIndex == null) {
                this.firstActiveFrameIndex = index;
            }

            return FrameKind.Active;
        }

        // This is the first frame in the history
        if (index == 0 && frame->isActive() == false) {
            return FrameKind.Startup;
        }

        if (this.HasBeenActive() && this.firstActiveFrameIndex != null && index > this.firstActiveFrameIndex) {
            return FrameKind.Recovery;
        } else {
            return FrameKind.Startup;
        }
    }

    public void Reset() {
        this.firstActiveFrameIndex = null;
        this.Clear();
    }
}
