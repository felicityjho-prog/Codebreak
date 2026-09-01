using UnityEditor;
using UnityEngine;

namespace Ostryzhnyi.Tools.Editor
{
    public class ClearEditorPlayerPrefs : MonoBehaviour
    {
        [MenuItem("Tools/ClearEditorPlayerPrefs")]
        public static void ClearPlayerPrefs()
        {
            EditorPrefs.DeleteAll();
        }
    }
}
