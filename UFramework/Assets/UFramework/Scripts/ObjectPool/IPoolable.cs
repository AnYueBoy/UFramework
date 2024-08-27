namespace UFramework
{
    public interface IPoolable
    {
        
        /// <summary>
        /// 从对象池中取出时调用
        /// </summary>
        void OnSpawn();

        /// <summary>
        /// 返回对象池时调用
        /// </summary>
        void OnDespawn();
    }
}