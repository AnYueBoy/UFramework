namespace UFramework.Promise {
    using System.Runtime.InteropServices;

    [StructLayoutAttribute (LayoutKind.Sequential)]
    public struct TimeData {

        public float elapsedTime;
        public float deltaTime;
    }
}