using SharpDX.DirectInput;
using DearImguiSharp;
using Ougon.Data;
using System.Text;
using System.Runtime.InteropServices;

namespace Ougon.GUI;

sealed class InputConfig {
    private Context Context;
    private DirectInput? DirectInput;

    private bool IsOpen = true;
    private IList<DeviceInstance>? Devices;
    private long LastUpdatedTimestamp;

    public unsafe InputConfig(Context context) {
        this.Context = context;
    }

    public unsafe void SetDirectInput() {
        if (this.Context == null || this.IsOpen == false) return;

        if (this.DirectInput == null && this.Context.gameState != null) {
            this.DirectInput = DirectInput.FromPointer<DirectInput>(this.Context.gameState->IDirectInput8);
        }
    }

    public unsafe void Render() {
        this.SetDirectInput();

        if (this.Context == null || this.DirectInput == null) return;

        this.UpdateDevicesDebounced();

        ConfigDevice(0);
        ConfigDevice(1);
    }

    private void UpdateDevicesDebounced() {
        var now = DateTimeOffset.Now;
        var offset = DateTimeOffset.FromUnixTimeMilliseconds(this.LastUpdatedTimestamp);
        var delay = new TimeSpan(0, 0, 0, 0, 500);

        if (now - offset > delay) {
            this.Devices = this.DirectInput?.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices);
            this.LastUpdatedTimestamp = now.ToUnixTimeMilliseconds();
        }
    }

    private static string GetDeviceName(DeviceInstance instance) {
        byte[] bytes = Encoding.Unicode.GetBytes(instance.InstanceName);
        return Encoding.UTF8.GetString(bytes);
    }

    private unsafe void ConfigDevice(int index)
    {
        var (inputDevicePtr, deviceInstancePtr) = this.Context.gameState->InputDevice(index);
        var inputDevice = (InputDevice*)inputDevicePtr;
        var selectedDeviceInstance = (IDeviceInstance*)deviceInstancePtr;

        if (inputDevice == null || this.Devices == null || selectedDeviceInstance == null) return;

        var deviceInstanceGuidArr = new byte[16];
        Marshal.Copy((nint)selectedDeviceInstance->InstanceGuid, deviceInstanceGuidArr, 0, 16);
        var selectedDeviceInstanceGuid = new Guid(deviceInstanceGuidArr);

        if (ImGui.Begin($"Input Config {index}", ref IsOpen, 0)) {
            using (ImVec2 size = new ImVec2 { X = 300, Y = 5 * ImGui.GetTextLineHeightWithSpacing() })
            if (ImGui.BeginListBox($"##devices-{index}", size)) {
                foreach (var device in this.Devices) {
                    var instanceName = GetDeviceName(device);

                    using (ImVec2 selectableSize = new ImVec2 { X = 300, Y = ImGui.GetTextLineHeightWithSpacing() })
                    if (ImGui.SelectableBool(instanceName, device.InstanceGuid == selectedDeviceInstanceGuid, 0, selectableSize)) {
                        var joystick = new Joystick(this.DirectInput, device.ProductGuid);
                        joystick.Acquire();

                        this.Context.gameState->SetInputDeviceAt(index, joystick.NativePointer);
                    }
                };

                ImGui.EndListBox();
            }

            int newA = inputDevice->a;
            ImGui.InputInt("A", ref newA, 1, 1, 0);

            int newB = inputDevice->b;
            ImGui.InputInt("B", ref newB, 1, 1, 0);

            int newC = inputDevice->c;
            ImGui.InputInt("C", ref newC, 1, 1, 0);

            int newD = inputDevice->d;
            ImGui.InputInt("D", ref newD, 1, 1, 0);

            inputDevice->a = (byte)newA;
            inputDevice->b = (byte)newB;
            inputDevice->c = (byte)newC;
            inputDevice->d = (byte)newD;
        }
    }
}
