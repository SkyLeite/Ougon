using System.Runtime.InteropServices;
using Ougon.Data;

namespace Ougon
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct GameState
    {
        [FieldOffset(0x480)]
        public double fps;

        [FieldOffset(0x4b4)]
        public bool debugEnabled;

        [FieldOffset(0x3c)]
        public bool isFullScreen;

        [FieldOffset(0x30)]
        public int isRunning;

        [FieldOffset(0x34)]
        public bool isWindowed;

        [FieldOffset(0x4f4)]
        public IntPtr DX9Device;

        [FieldOffset(0x6f0)]
        public IntPtr IDirectInput8;

        [FieldOffset(0x440)]
        public InputDevice* inputDevice1;

        [FieldOffset(0x444)]
        public InputDevice* inputDevice2;

        [FieldOffset(0x448)]
        public InputDevice* inputDevice3;

        [FieldOffset(0x44C)]
        public InputDevice* inputDevice4;

        [FieldOffset(0x6F8)]
        public IDeviceInstance* DeviceInstance1;

        [FieldOffset(0x6FC)]
        public IDeviceInstance* DeviceInstance2;

        [FieldOffset(0x700)]
        public IDeviceInstance* DeviceInstance3;

        [FieldOffset(0x704)]
        public IDeviceInstance* DeviceInstance4;

        public (IntPtr inputDevice, IntPtr DeviceInstance) InputDevice(int index) {
            switch (index) {
                case 0:
                    return ((IntPtr)this.inputDevice1, (IntPtr)this.DeviceInstance1);

                case 1:
                    return ((IntPtr)this.inputDevice2, (IntPtr)this.DeviceInstance2);

                case 2:
                    return ((IntPtr)this.inputDevice3, (IntPtr)this.DeviceInstance3);

                case 3:
                    return ((IntPtr)this.inputDevice4, (IntPtr)this.DeviceInstance4);
            }

            throw new KeyNotFoundException();
        }

        public void SetInputDeviceAt(int index, IntPtr ptr) {
            switch (index) {
                case 0:
                    this.DeviceInstance1 = (IDeviceInstance*)ptr;
                    return;

                case 1:
                    this.DeviceInstance2 = (IDeviceInstance*)ptr;
                    return;

                case 2:
                    this.DeviceInstance3 = (IDeviceInstance*)ptr;
                    return;

                case 3:
                    this.DeviceInstance4 = (IDeviceInstance*)ptr;
                    return;
            }

            throw new KeyNotFoundException();
        }
    }
}
