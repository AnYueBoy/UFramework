using System;
using UnityEngine;
using UnityEngine.UI;

namespace UFramework
{
    public class GuideMask : MonoBehaviour
    {
        private Material guideMaskMaterial;
        private readonly int maskType = Shader.PropertyToID("_MaskType");
        private readonly int maskInfo = Shader.PropertyToID("_MaskInfo");
        private readonly int maskTex = Shader.PropertyToID("_MaskTex");

        private void Awake()
        {
            var guideMaskShader = Shader.Find("Guide/GuideMask");
            guideMaskMaterial = new Material(guideMaskShader);
            var imageComp = GetComponent<Image>();
            imageComp.material = guideMaskMaterial;
        }

        public void ShowCirceMask(Vector2 worldPos, float radius)
        {
            // 设置遮罩类型
            guideMaskMaterial.SetInt(maskType, (int)GuideMaskType.Circle);

            // 设置遮罩信息
            guideMaskMaterial.SetVector(maskInfo, new Vector4(worldPos.x, worldPos.y, radius));
        }

        public void ShowRectMask(Vector2 worldPos, float rectWidth, float rectHeight)
        {
            // 设置遮罩类型
            guideMaskMaterial.SetInt(maskType, (int)GuideMaskType.Rect);

            // 设置遮罩信息
            guideMaskMaterial.SetVector(maskInfo, new Vector4(worldPos.x, worldPos.y, rectWidth, rectHeight));
        }

        public void ShowTextureMask(Vector2 worldPos, Sprite maskSprite)
        {
            // 设置遮罩类型
            guideMaskMaterial.SetInt(maskType, (int)GuideMaskType.Texture);

            // 设置遮罩信息
            guideMaskMaterial.SetVector(maskInfo, new Vector4(worldPos.x, worldPos.y));

            // 设置遮罩图
            guideMaskMaterial.SetTexture(maskTex, maskSprite.texture);
        }

        /// <summary>
        /// 遮罩类型
        /// </summary>
        public enum GuideMaskType
        {
            /// <summary>
            /// 圆形
            /// </summary>
            Circle,

            /// <summary>
            /// 矩形
            /// </summary>
            Rect,

            /// <summary>
            /// 图片
            /// </summary>
            Texture,
        }
    }
}