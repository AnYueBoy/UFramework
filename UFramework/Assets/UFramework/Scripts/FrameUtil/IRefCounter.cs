/*
 * @Author: l hy 
 * @Date: 2021-01-19 21:42:28 
 * @Description: 引用计数器接口
 * @Last Modified by: l hy
 * @Last Modified time: 2021-01-19 22:00:20
 */
namespace UFramework.FrameUtil {
    interface IRefCounter {

        int refCount { get; }

        void addRef (object refOwner = null);

        void releaseRef (object refOwner = null);
    }
}