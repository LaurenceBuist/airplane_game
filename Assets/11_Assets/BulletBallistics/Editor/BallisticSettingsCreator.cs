using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

namespace Ballistics
{
    public class BallisticSettingsCreator
    {

        [MenuItem("Ballistics/Create BallisticSettings")]
        public static void NewSettingsObject()
        {
            CreateAsset<BallisticSettings>();
        }

        static string CreateAsset<T>() where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");

            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;

            return assetPathAndName;
        }
    }
}
