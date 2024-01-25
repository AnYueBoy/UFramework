using System.Collections;

namespace UFramework
{
    public interface ICoroutineManager
    {
        void Init(); 
        
        void LocalUpdate(float dt);

        void Terminate();

        /// <summary>
        /// 创建协程(不运行)
        /// </summary>
        ICoroutine CreateCoroutine(IEnumerator routine);

        /// <summary>
        /// 开启协程
        /// </summary>
        ICoroutine StartCoroutine(IEnumerator routine);

        /// <summary>
        /// 暂停协程
        /// </summary>
        void PauseCoroutine(ICoroutine routine);

        /// <summary>
        /// 恢复协程
        /// </summary>
        void ResumeCoroutine(ICoroutine coroutine);

        /// <summary>
        /// 关闭协程
        /// </summary>
        void StopCoroutine(ICoroutine coroutine);
    }
}