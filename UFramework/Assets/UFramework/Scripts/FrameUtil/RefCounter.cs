/*
 * @Author: l hy 
 * @Date: 2021-01-19 21:45:34 
 * @Description: 简易计数器
 * @Last Modified by: l hy
 * @Last Modified time: 2021-01-19 21:59:45
 */
namespace UFramework.FrameUtil {
    public class RefCounter : IRefCounter {
        public int refCount { get; private set; }

        public void addRef (object refOwner = null) {
            this.refCount++;
        }

        public void releaseRef (object refOwner = null) {
            this.refCount--;
            if (this.refCount <= 0) {
                this.refTrigger ();
            }
        }

        protected virtual void refTrigger () { }
    }
}