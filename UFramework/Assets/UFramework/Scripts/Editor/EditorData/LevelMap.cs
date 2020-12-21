/*
 * @Author: l hy 
 * @Date: 2020-12-21 16:35:21 
 * @Description: 关卡地图
 */
namespace UFramework.Editor.EditorData {

    using System.Collections.Generic;
    using UnityEngine;

    [SerializeField]
    public class LevelMap {

        public List<LevelInfo> levels = new List<LevelInfo> ();
    }
}