/*
 * @Author: l hy 
 * @Date: 2021-01-25 19:00:12 
 * @Description: 内存监视
 */

namespace UFrameWork.Develop {
    using UnityEngine.Profiling;
    using UnityEngine;

    public class MemoryDetector {

        private readonly static string totalAllocMemoryInfo = "Alloc Memory : {0}M";
        private readonly static string totalReservedMemoryInfo = "Reserved Memory : {0}M";
        private readonly static string totalUnusedReservedMemoryInfo = "Unused Reserved Memory : {0}M";
        private readonly static string monoHeapInfo = "Mono Heap : {0}M";
        private readonly static string monoUsedInfo = "Mono Used : {0}M";
        private float byteToM = 0.000001f;

        private Rect allocMemoryRect;
        private Rect reservedMemoryRect;
        private Rect unusedReservedMemoryRect;
        private Rect monoHeapRect;
        private Rect monoUsedRect;

        private int x = 0;
        private int y = 0;
        private int w = 0;
        private int h = 0;

        public MemoryDetector () {
            this.x = 60;
            this.y = 60;
            this.w = 200;
            this.h = 20;

            this.allocMemoryRect = new Rect (x, y, w, h);
            this.reservedMemoryRect = new Rect (x, y + h, w, h);
            this.unusedReservedMemoryRect = new Rect (x, y + 2 * h, w, h);
            this.monoHeapRect = new Rect (x, y + 3 * h, w, h);
            this.monoUsedRect = new Rect (x, y + 4 * h, w, h);

            // FIXME:
        }

        private void OnGUI () {
            GUI.Label (this.allocMemoryRect,
                string.Format (totalAllocMemoryInfo, Profiler.GetTotalAllocatedMemoryLong () * byteToM));
            GUI.Label (this.reservedMemoryRect,
                string.Format (totalReservedMemoryInfo, Profiler.GetTotalReservedMemoryLong () * byteToM));
            GUI.Label (this.unusedReservedMemoryRect,
                string.Format (totalUnusedReservedMemoryInfo, Profiler.GetTotalUnusedReservedMemoryLong () * byteToM));
            GUI.Label (monoHeapRect,
                string.Format (monoHeapInfo, Profiler.GetMonoHeapSizeLong () * byteToM));
            GUI.Label (monoUsedRect,
                string.Format (monoUsedInfo, Profiler.GetMonoUsedSizeLong () * byteToM));
        }
    }
}