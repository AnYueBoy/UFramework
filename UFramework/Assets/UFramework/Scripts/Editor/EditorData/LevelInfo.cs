/*
 * @Author: l hy 
 * @Date: 2020-12-21 16:34:43 
 * @Description: 关卡信息
 */
namespace UFramework.Editor.EditorData {
    using System.Collections.Generic;
    public class LevelInfo {
        public int currentLevel = 0;

        public List<NodeInfo> nodeList = new List<NodeInfo> ();

    }
}