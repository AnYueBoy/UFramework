using System;
using System.Collections.Generic;
using UFramework.Core;
using UnityEngine;

namespace UFramework.GameCommon
{
    public class UIManager : IUIManager
    {
        private Dictionary<Type, ViewUI> viewDic = new Dictionary<Type, ViewUI>();

        private ViewUI currentBoard;

        private RectTransform boardRoot;
        private RectTransform dialogRoot;

        public void Init(RectTransform boardRoot, RectTransform dialogRoot)
        {
            this.boardRoot = boardRoot;
            this.dialogRoot = dialogRoot;
        }

        private T GetView<T>() where T : class, IView, new()
        {
            Type viewType = typeof(T);
            if (viewDic.TryGetValue(viewType, out var viewUI))
            {
                return viewUI as T;
            }

            T view = new T();
            GameObject viewNode = App.Make<IAssetsManager>().GetAssetByUrlSync<GameObject>(view.ViewPath);
            view = viewNode.GetComponent<T>();
            viewDic.Add(viewType, view as ViewUI);
            return view;
        }

        public T ShowBoard<T>(params object[] args) where T : class, IView, new()
        {
            // 关闭上一个Board
            if (currentBoard != null)
            {
                currentBoard.gameObject.SetActive(false);
            }

            T board = GetView<T>();
            currentBoard = board as ViewUI;
            currentBoard.gameObject.SetActive(true);
            currentBoard.ViewRect.SetParent(boardRoot);
            currentBoard.ViewRect.sizeDelta = Vector2.zero;
            currentBoard.ViewRect.localScale = Vector3.one;
            currentBoard.OnShow(args);
            return board;
        }

        public T ShowDialog<T>(params object[] args) where T : class, IView, new()
        {
            T dialogView = GetView<T>();
            var dialog = dialogView as ViewUI;
            dialog.gameObject.SetActive(true);
            dialog.ViewRect.SetParent(dialogRoot);
            dialog.ViewRect.sizeDelta = Vector2.zero;
            dialog.ViewRect.localScale = Vector3.one;
            dialog.OnShow(args);
            return dialogView;
        }

        public T CloseView<T>() where T : class, IView, new()
        {
            return HideView<T>();
        }

        private T HideView<T>() where T : class, IView, new()
        {
            Type viewType = typeof(T);
            if (!viewDic.TryGetValue(viewType, out var viewUI))
            {
                Debug.LogError($"需要关闭的ui不存在 {viewType}");
                return null;
            }

            viewUI.gameObject.SetActive(false);
            return viewUI as T;
        }

        public void HideAll()
        {
            foreach (var keyValue in viewDic)
            {
                var view = keyValue.Value;
                if (!view.gameObject.activeSelf)
                {
                    continue;
                }

                view.gameObject.SetActive(false);
            }
        }
    }
}