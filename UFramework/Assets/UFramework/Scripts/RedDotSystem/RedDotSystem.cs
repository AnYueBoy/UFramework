using System;
using System.Collections.Generic;
using System.Text;

namespace UFramework
{
    public class RedDotSystem : IRedDotSystem
    {
        /// <summary>
        /// 所有节点集合
        /// </summary>
        private Dictionary<string, ITreeNode> allNodes;

        /// <summary>
        /// 脏节点集合
        /// </summary>
        private HashSet<ITreeNode> dirtyNodes;

        /// <summary>
        /// 临时脏节点集合
        /// </summary>
        private List<ITreeNode> tempDirtyNodes;

        /// <summary>
        /// 节点数量改变回调
        /// </summary>
        public Action NodeNumChangeCallback { get; private set; }

        /// <summary>
        /// 节点值改变回调
        /// </summary>
        public Action<ITreeNode, int> NodeValueChangeCallback { get; private set; }

        /// <summary>
        /// 路径分隔字符
        /// </summary>
        public char SplitChar { get; private set; }

        public StringBuilder CacheStringBuilder { get; private set; }


        /// <summary>
        /// 树根节点
        /// </summary>
        public ITreeNode Root { get; private set; }

        public RedDotSystem()
        {
            SplitChar = '/';
            allNodes = new Dictionary<string, ITreeNode>();
            Root = new TreeNode("Root");
            dirtyNodes = new HashSet<ITreeNode>();
            tempDirtyNodes = new List<ITreeNode>();
            CacheStringBuilder = new StringBuilder();
        }

        public void Init()
        {
        }

        /// <summary>
        /// 添加节点值监听
        /// </summary>
        public ITreeNode AddListener(string path, Action<int> callback)
        {
            if (callback == null)
            {
                return null;
            }

            ITreeNode node = GetTreeNode(path);
            node.AddListener(callback);
            return node;
        }

        /// <summary>
        /// 移除节点值监听
        /// </summary>
        public void RemoveListener(string path, Action<int> callback)
        {
            if (callback == null)
            {
                return;
            }

            ITreeNode node = GetTreeNode(path);
            node.RemoveListener(callback);
        }

        /// <summary>
        /// 移除所有节点值监听
        /// </summary>
        public void RemoveAllListener(string path)
        {
            ITreeNode node = GetTreeNode(path);
            node.RemoveAllListener();
        }

        /// <summary>
        /// 改变节点值
        /// </summary>
        public void ChangeValue(string path, int newValue)
        {
            ITreeNode node = GetTreeNode(path);
            node.ChangedValue(newValue);
        }

        /// <summary>
        /// 获取节点值
        /// </summary>
        public int GetValue(string path)
        {
            ITreeNode node = GetTreeNode(path);
            if (node == null)
            {
                return 0;
            }

            return node.Value;
        }

        /// <summary>
        /// 获取节点
        /// </summary>
        public ITreeNode GetTreeNode(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new Exception($"路径不合法，不能为空");
            }

            if (allNodes.TryGetValue(path, out var node))
            {
                return node;
            }

            ITreeNode cur = Root;
            int length = path.Length;

            int startIndex = 0;
            for (int i = 0; i < length; i++)
            {
                if (path[i] == SplitChar)
                {
                    if (i == length - 1)
                    {
                        throw new Exception($"路径不合法，不能以路径分隔符结尾：{path}");
                    }

                    int endIndex = i - 1;
                    if (endIndex < startIndex)
                    {
                        throw new Exception($"路径不合法，不能存在连续的路径分隔符或以路径分隔符开头：{path}");
                    }

                    ITreeNode child = cur.GetOrAddChild(new RangeString(path, startIndex, endIndex));
                    startIndex = i + 1;
                    cur = child;
                }
            }

            //最后一个节点 直接用length - 1作为endIndex
            ITreeNode target = cur.GetOrAddChild(new RangeString(path, startIndex, length - 1));
            allNodes.Add(path, target);
            return target;
        }

        /// <summary>
        /// 移除节点
        /// </summary>
        public bool RemoveTreeNode(string path)
        {
            if (!allNodes.ContainsKey(path))
            {
                return false;
            }

            ITreeNode node = GetTreeNode(path);
            allNodes.Remove(path);
            return node.Parent.RemoveChild(new RangeString(node.Name, 0, node.Name.Length - 1));
        }

        public void RemoveAllTreeNode()
        {
            Root.RemoveAllChild();
            allNodes.Clear();
        }

        public void LocalUpdate()
        {
            // 刷新检测条件
            foreach (var keyValuePair in allNodes)
            {
                var node = keyValuePair.Value;
                if (node.ChildrenCount > 0)
                {
                    continue;
                }

                if (node.CheckTriggerCondition == null)
                {
                    continue;
                }

                node.ChangedValue(node.CheckTriggerCondition.Invoke());
            }


            if (dirtyNodes.Count == 0)
            {
                return;
            }

            tempDirtyNodes.Clear();
            foreach (var node in dirtyNodes)
            {
                tempDirtyNodes.Add(node);
            }

            dirtyNodes.Clear();

            for (int i = 0; i < tempDirtyNodes.Count; i++)
            {
                tempDirtyNodes[i].ChangedValue();
            }
        }

        /// <summary>
        /// 标记脏节点
        /// </summary>
        public void MakeDirtyNode(ITreeNode node)
        {
            if (node == null || node.Name == Root.Name)
            {
                return;
            }

            dirtyNodes.Add(node);
        }

        public void RegisterTrigger(params IRedDotTrigger[] triggerConditions)
        {
            for (int i = 0; i < triggerConditions.Length; i++)
            {
                var trigger = triggerConditions[i];
                var node = GetTreeNode(trigger.FullPath);
                node.CheckTriggerCondition = trigger.TriggerCondition;
            }
        }
    }
}