﻿using UnityEngine;

namespace UFramework.GameCommon
{
    public interface IView
    {
        /// <summary>
        /// 界面加载路径
        /// </summary>
        string ViewPath { get; }

        /// <summary>
        /// 界面名称
        /// </summary>
        string ViewName { get; }

        /// <summary>
        /// ui实例
        /// </summary>
        ViewUI UIInstance { get; set; }

        /// <summary>
        /// ui序偶在层级
        /// </summary>
        UILayer UILayer { get; set; }

        void OnShow(params object[] param);

        void OnClose();
    }
}