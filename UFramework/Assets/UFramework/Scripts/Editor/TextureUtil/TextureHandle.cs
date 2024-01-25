using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using SApplication = UnityEngine.Application;

namespace UFramework
{
    public class TextureHandle : AssetPostprocessor
    {
        [MenuItem("UFramework/ConvertTexture")]
        public static void ModifyTexture()
        {
            string[] paths = Directory.GetFiles(SApplication.dataPath, "*.*", SearchOption.AllDirectories)
                .Where(s => s.EndsWith(".png") || s.EndsWith(".jpg")).ToArray();
            foreach (string path in paths)
            {
                string progressPath = path.Replace(SApplication.dataPath, "Assets");
                TextureImporter importer = AssetImporter.GetAtPath(progressPath) as TextureImporter;
                if (importer == null)
                {
                    continue;
                }

                if (importer.filterMode == FilterMode.Bilinear)
                {
                    continue;
                }

                importer.filterMode = FilterMode.Bilinear;
                AssetDatabase.ImportAsset(progressPath);
            }
        }

        private void OnPreprocessTexture()
        {
            string path = assetImporter.assetPath;
            if (path.Contains(".png") || path.Contains(".jpg"))
            {
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null || importer.filterMode == FilterMode.Bilinear)
                {
                    return;
                }

                importer.filterMode = FilterMode.Bilinear;
                AssetDatabase.ImportAsset(path);
            }
        }
    }
}