using System;
using System.Collections.Generic;
using UFramework.Core;
using UnityEngine;

namespace UFramework.GameCommon
{
    public class UIManager : IUIManager
    {
        private readonly Dictionary<Type, IView> viewDic = new Dictionary<Type, IView>();

        private RectTransform lowerRoot;
        private RectTransform topRoot;
        private RectTransform tipRoot;

        public UIManager(INodeManager nodeManager)
        {
            lowerRoot = nodeManager.LowerRoot;
            topRoot = nodeManager.TopRoot;
            tipRoot = nodeManager.TipRoot;
        }

        private T GetView<T>() where T : class, IView, new()
        {
            Type viewType = typeof(T);
            if (viewDic.TryGetValue(viewType, out var viewUI))
            {
                return viewUI as T;
            }

            T view = new T();
            // 根据加载路径加载对应的UI实例
            GameObject viewPrefab = App.Make<IAssetsManager>().GetAssetByUrlSync<GameObject>(view.ViewPath);
            // 实例化对应UI实例
            GameObject viewNode = App.Make<IObjectPool>().RequestInstance(viewPrefab);
            var iviewUI = viewNode.GetComponent<BindUI>();
            // 设置ui实例
            view.UIInstance = iviewUI;
            view.UIInstance.Init();
            viewDic.Add(viewType, view);
            return view;
        }

        public T ShowView<T>(UILayer uiLayer, params object[] args) where T : class, IView, new()
        {
            RectTransform uiRoot = GetUIRoot(uiLayer);
            T view = GetView<T>();
            view.UIInstance.RectTrans.SetParent(uiRoot);
            view.UIInstance.RectTrans.offsetMax = view.UIInstance.RectTrans.offsetMin = Vector2.zero;
            view.UIInstance.RectTrans.localScale = Vector3.one;
            view.UIInstance.gameObject.SetActive(true);
            view.UILayer = uiLayer;
            view.OnShow(args);
            return view;
        }

        public T CloseView<T>() where T : class, IView, new()
        {
            return HideView<T>();
        }

        private T HideView<T>() where T : class, IView, new()
        {
            Type viewType = typeof(T);
            if (!viewDic.TryGetValue(viewType, out var view))
            {
                Debug.LogError($"需要关闭的ui不存在 {viewType}");
                return null;
            }

            view.UIInstance.gameObject.SetActive(false);
            view.OnClose();
            return view as T;
        }

        public void HideAll()
        {
            foreach (var keyValue in viewDic)
            {
                var view = keyValue.Value;
                if (!view.UIInstance.gameObject.activeSelf)
                {
                    continue;
                }

                view.UIInstance.gameObject.SetActive(false);
            }
        }

        private RectTransform GetUIRoot(UILayer uiLayer)
        {
            if (uiLayer == UILayer.Lower)
            {
                return lowerRoot;
            }

            if (uiLayer == UILayer.Top)
            {
                return topRoot;
            }

            if (uiLayer == UILayer.Tip)
            {
                return tipRoot;
            }

            Debug.LogError($"错误的UIRoot:{uiLayer}");
            return null;
        }
    }
}