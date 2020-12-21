/*
 * @Author: l hy 
 * @Date: 2020-12-21 16:35:54 
 * @Description: 节点信息
 */
namespace UFramework.Editor.EditorData {
    using UnityEngine;

    [SerializeField]
    public class NodeInfo {

        public string nodeName = null;

        public double x = 0;

        public double y = 0;

        public double rotation = 0;

        public double scale = 0;
    }
}