/*
 * @Author: l hy 
 * @Date: 2020-03-31 21:30:16 
 * @Description: 导出单张图片
 * @Last Modified by: l hy
 * @Last Modified time: 2020-12-21 16:46:50
 */
namespace UFramework.Editor.GenerateName {
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    public class ExportTexture {

        [MenuItem ("UFramework/ExportTextures")]
        private static void exportTextures () {
            Texture2D image = Selection.activeObject as Texture2D;
            if (Selection.activeObject == null) {
                EditorUtility.DisplayDialog ("失败", "请选择资源窗口要导出的图片", "确认");
                return;
            }
            string rootPath = Path.GetDirectoryName (AssetDatabase.GetAssetPath (image));
            string path = rootPath + "/" + image.name + ".PNG";
            TextureImporter texImp = AssetImporter.GetAtPath (path) as TextureImporter;

            string savePath = EditorUtility.SaveFolderPanel ("选择保存的文件夹", "", "");
            if (savePath == "" || savePath == null || savePath.Length <= 0) {
                EditorUtility.DisplayDialog ("失败", "路径不存在", "确认");
                return;
            }

            foreach (SpriteMetaData metaData in texImp.spritesheet) {
                Texture2D m_image = new Texture2D ((int) metaData.rect.width, (int) metaData.rect.height);
                for (int y = (int) metaData.rect.y; y < metaData.rect.y + metaData.rect.height; y++) {
                    for (int x = (int) metaData.rect.x; x < metaData.rect.x + metaData.rect.width; x++)
                        m_image.SetPixel (x - (int) metaData.rect.x, y - (int) metaData.rect.y, image.GetPixel (x, y));
                }

                //转换纹理到EncodeToPNG兼容格式
                if (m_image.format != TextureFormat.ARGB32 && m_image.format != TextureFormat.RGB24) {
                    Texture2D newTexture = new Texture2D (m_image.width, m_image.height);
                    newTexture.SetPixels (m_image.GetPixels (0), 0);
                    m_image = newTexture;
                }
                byte[] pngData = m_image.EncodeToPNG ();

                File.WriteAllBytes (savePath + "/" + metaData.name + ".png", pngData);
            }
        }
    }
}