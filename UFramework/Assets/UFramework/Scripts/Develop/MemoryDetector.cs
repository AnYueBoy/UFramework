/*
 * @Author: l hy 
 * @Date: 2021-01-25 19:00:12 
 * @Description: 内存监视
 */

namespace UFramework.Develop {
    using UnityEngine.Profiling;
    using UnityEngine;

    public class MemoryDetector {
        private float byteToM = 0.000001f;

        private Rect allocMemoryRect;
        private Rect reservedMemoryRect;
        private Rect unusedReservedMemoryRect;
        private Rect monoHeapRect;
        private Rect monoUsedRect;

        private readonly int x = 80;
        private readonly int y = 45;
        private readonly int w = 210;
        private readonly int h = 20;
        public void init () {
            this.allocMemoryRect = new Rect (x, y, w, h);
            this.reservedMemoryRect = new Rect (x, y + h, w, h);
            this.unusedReservedMemoryRect = new Rect (x, y + 2 * h, w, h);
            this.monoHeapRect = new Rect (x, y + 3 * h, w, h);
            this.monoUsedRect = new Rect (x, y + 4 * h, w, h);
        }

        public void drawGUI () {
            GUI.Box (
                this.allocMemoryRect,
                string.Format ("Alloc Memory : {0}M", Mathf.Floor (Profiler.GetTotalAllocatedMemoryLong () * byteToM)));

            GUI.Box (
                this.reservedMemoryRect,
                string.Format ("Reserved Memory : {0}M", Mathf.Floor (Profiler.GetTotalReservedMemoryLong () * byteToM)));

            GUI.Box (
                this.unusedReservedMemoryRect,
                string.Format ("Unused Reserved Memory : {0}M", Mathf.Floor (Profiler.GetTotalUnusedReservedMemoryLong () * byteToM)));

            GUI.Box (
                monoHeapRect,
                string.Format ("Mono Heap : {0}M", Mathf.Floor (Profiler.GetMonoHeapSizeLong () * byteToM)));

            GUI.Box (
                monoUsedRect,
                string.Format ("Mono Used : {0}M", Mathf.Floor (Profiler.GetMonoUsedSizeLong () * byteToM)));
        }
    }
}