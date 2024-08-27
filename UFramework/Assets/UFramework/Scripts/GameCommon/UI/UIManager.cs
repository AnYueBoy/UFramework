using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UFramework
{
    public class UIManager : IUIManager
    {
        private readonly Dictionary<Type, IView> viewDic = new Dictionary<Type, IView>();

        private HashSet<IView> currentLowerViews = new HashSet<IView>();
        private HashSet<IView> currentTopViews = new HashSet<IView>();
        private List<IView> currentTipViews = new List<IView>();

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
            GameObject viewNode = Pool.Spawn(viewPrefab);
            var iviewUI = viewNode.GetComponent<BindUI>();
            // 设置ui实例
            view.UIInstance = iviewUI;
            view.OnInit();
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
            if (uiLayer == UILayer.Lower)
            {
                currentLowerViews.Add(view);
            }
            else if (uiLayer == UILayer.Top)
            {
                currentTopViews.Add(view);
            }
            else if (uiLayer == UILayer.Tip)
            {
                currentTipViews.Add(view);
            }
            else
            {
                Debug.LogError($"添加view时产生了错误的UILayer:{uiLayer}");
            }

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
            if (view.UILayer == UILayer.Lower)
            {
                currentLowerViews.Remove(view);
            }
            else if (view.UILayer == UILayer.Top)
            {
                currentTopViews.Remove(view);
            }
            else if (view.UILayer == UILayer.Tip)
            {
                currentTipViews.Remove(view);
            }
            else
            {
                Debug.LogError($"移除view时产生了错误的UILayer:{view.UILayer}");
            }

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

        public void LocalUpdate(float dt)
        {
            foreach (var views in viewDic)
            {
                var view = views.Value;
                if (!view.UIInstance.gameObject.activeSelf)
                {
                    continue;
                }

                view.LocalUpdate(dt);
            }
        }

        public IView GetCurrentView()
        {
            if (currentTipViews.Count > 0)
            {
                return currentTipViews[0];
            }

            if (currentTopViews.Count > 0)
            {
                return currentTopViews.First();
            }

            if (currentLowerViews.Count > 0)
            {
                return currentLowerViews.First();
            }

            return null;
        }

        public IView GetCurrentViewExcludeTip()
        {
            if (currentTopViews.Count > 0)
            {
                return currentTopViews.First();
            }

            if (currentLowerViews.Count > 0)
            {
                return currentLowerViews.First();
            }

            return null;
        }

        public IView GetCurrentLowerView()
        {
            if (currentLowerViews.Count > 0)
            {
                return currentLowerViews.First();
            }

            return null;
        }

        public IView GetCurrentTopView()
        {
            if (currentTopViews.Count > 0)
            {
                return currentTopViews.First();
            }

            return null;
        }

        public IView GetCurrentTipView()
        {
            if (currentTipViews.Count > 0)
            {
                return currentTipViews[0];
            }

            return null;
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