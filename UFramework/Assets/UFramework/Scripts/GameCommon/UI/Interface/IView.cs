using UnityEngine;

namespace UFramework
{
    public interface IView
    {
        /// <summary>
        /// 界面加载路径
        /// </summary>
        string ViewPath { get; }

        /// <summary>
        /// ui实例
        /// </summary>
        BindUI UIInstance { get; set; }

        /// <summary>
        /// ui序偶在层级
        /// </summary>
        UILayer UILayer { get; set; }

        /// <summary>
        /// 仅在IView构建时调用一次
        /// </summary>
        void OnInit();

        void OnShow(params object[] param);

        void LocalUpdate(float dt);

        void OnClose();
    }
}