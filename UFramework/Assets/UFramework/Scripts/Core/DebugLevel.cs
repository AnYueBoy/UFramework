namespace UFramework.Core
{
    /// <summary>
    /// 框架的调试等级
    /// </summary>
    public enum DebugLevel
    {
        /// <summary>
        /// 生产环境
        /// </summary>
        Production,

        /// <summary>
        /// 介于生产环境与开发环境之间
        /// </summary>
        Staging,

        /// <summary>
        /// 开发环境
        /// </summary>
        Development,
    }
}