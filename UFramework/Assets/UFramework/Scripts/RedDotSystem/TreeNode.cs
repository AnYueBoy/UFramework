using System;
using System.Collections.Generic;
using UnityEngine;

namespace UFramework
{
    /// <summary>
    /// 树节点
    /// </summary>
    public class TreeNode : ITreeNode
    {
        /// <summary>
        /// 父节点
        /// </summary>
        public ITreeNode Parent { get; private set; }

        /// <summary>
        /// 子节点
        /// </summary>
        private Dictionary<RangeString, ITreeNode> children;

        /// <summary>
        /// 节点值改变回调
        /// </summary>
        private Action<int> changedCallback;

        /// <summary>
        /// 完整路径
        /// </summary>
        private string fullPath;

        /// <summary>
        /// 节点名
        /// </summary>
        public string Name { get; private set; }

        public string FullPath
        {
            get
            {
                var redDotSystem = App.Make<IRedDotSystem>();
                if (string.IsNullOrEmpty(fullPath))
                {
                    if (Parent == null && Parent == redDotSystem.Root)
                    {
                        fullPath = Name;
                    }
                }
                else
                {
                    fullPath = Parent.FullPath + redDotSystem.SplitChar + Name;
                }

                return fullPath;
            }
        }

        /// <summary>
        /// 节点值
        /// </summary>
        public int Value { get; private set; }

        /// <summary>
        /// 子节点
        /// </summary>
        public Dictionary<RangeString, ITreeNode>.ValueCollection Children => children.Values;

        /// <summary>
        /// 子节点数量，包括子节点的子节点等等
        /// </summary>
        public int ChildrenCount
        {
            get
            {
                if (children == null)
                {
                    return 0;
                }

                int sum = children.Count;
                foreach (ITreeNode node in children.Values)
                {
                    sum += node.ChildrenCount;
                }

                return sum;
            }
        }


        public TreeNode(string name)
        {
            Name = name;
            Value = 0;
            changedCallback = null;
        }

        public TreeNode(string name, ITreeNode parent) : this(name)
        {
            Parent = parent;
        }

        /// <summary>
        /// 添加节点值监听
        /// </summary>
        public void AddListener(Action<int> callback)
        {
            changedCallback += callback;
        }

        /// <summary>
        /// 移除节点值监听
        /// </summary>
        public void RemoveListener(Action<int> callback)
        {
            changedCallback -= callback;
        }

        /// <summary>
        /// 移除所有节点值监听
        /// </summary>
        public void RemoveAllListener()
        {
            changedCallback = null;
        }

        /// <summary>
        /// 改变节点值 (使用传入的新值,只能在叶子节点上调用)
        /// </summary>
        public void ChangedValue(int newValue)
        {
            if (children != null && children.Count != 0)
            {
                throw new Exception($"不允许直接改变非叶子节点的值: {FullPath}");
            }

            InternalChangeValue(newValue);
        }

        /// <summary>
        /// 改变节点值 (根据子节点值计算新值，只对非叶子节点有效)
        /// </summary>
        public void ChangedValue()
        {
            int sum = 0;
            if (children != null && children.Count != 0)
            {
                foreach (var child in children)
                {
                    sum += child.Value.Value;
                }
            }

            InternalChangeValue(sum);
        }


        private void InternalChangeValue(int newValue)
        {
            if (Value == newValue)
            {
                return;
            }

            Value = newValue;
            changedCallback?.Invoke(newValue);
            App.Make<IRedDotSystem>().NodeValueChangeCallback?.Invoke(this, newValue);

            // 标记父节点为脏节点
            App.Make<IRedDotSystem>().MakeDirtyNode(Parent);
        }

        /// <summary>
        /// 添加子节点
        /// </summary>
        public ITreeNode AddChild(RangeString key)
        {
            if (children == null)
            {
                children = new Dictionary<RangeString, ITreeNode>();
            }
            else if (children.ContainsKey(key))
            {
                throw new Exception($"子节点添加失败，不允许重复添加: {FullPath}");
            }

            ITreeNode child = new TreeNode(key.ToString(), this);
            children.Add(key, child);
            App.Make<IRedDotSystem>().NodeNumChangeCallback?.Invoke();
            return child;
        }

        /// <summary>
        /// 移除子节点
        /// </summary>
        public bool RemoveChild(RangeString key)
        {
            if (children == null || children.Count == 0)
            {
                return false;
            }

            ITreeNode child = GetChild(key);

            if (child != null)
            {
                // 子节点被删除，需要进行一次父节点的更新
                App.Make<IRedDotSystem>().MakeDirtyNode(this);

                children.Remove(key);

                App.Make<IRedDotSystem>().NodeNumChangeCallback?.Invoke();
                return true;
            }

            return false;
        }

        /// <summary>
        /// 移除所有子节点
        /// </summary>
        public void RemoveAllChild()
        {
            if (children == null || children.Count == 0)
            {
                return;
            }

            children.Clear();
            App.Make<IRedDotSystem>().MakeDirtyNode(this);
            App.Make<IRedDotSystem>().NodeNumChangeCallback?.Invoke();
        }

        /// <summary>
        /// 获取子节点
        /// </summary>
        public ITreeNode GetChild(RangeString key)
        {
            if (children == null)
            {
                return null;
            }

            children.TryGetValue(key, out ITreeNode child);
            return child;
        }

        /// <summary>
        /// 获取子节点，如果不存在则添加
        /// </summary>
        public ITreeNode GetOrAddChild(RangeString key)
        {
            ITreeNode child = GetChild(key);
            if (child == null)
            {
                child = AddChild(key);
            }

            return child;
        }

        public Func<int> CheckTriggerCondition { get; set; }

        public override string ToString()
        {
            return FullPath;
        }
    }
}