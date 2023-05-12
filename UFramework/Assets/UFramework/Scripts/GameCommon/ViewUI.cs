/*
 * @Author: l hy 
 * @Date: 2020-03-07 16:37:25 
 * @Description: 界面基类 
 * @Last Modified by: l hy
 * @Last Modified time: 2020-12-21 16:42:57
 */

using System;

namespace UFramework.GameCommon
{
    using UnityEngine;

    public abstract class ViewUI : MonoBehaviour, IView
    {
        public string ViewPath { get; }
        public RectTransform ViewRect => _viewRect;
        private RectTransform _viewRect;

        private void Awake()
        {
            _viewRect = GetComponent<RectTransform>();
        }

        public abstract void OnShow(params object[] param);

        protected ViewUI()
        {
        }
    }
}