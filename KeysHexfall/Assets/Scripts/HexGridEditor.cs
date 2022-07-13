#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace HexagonPuzzle.Editors
{
    [CustomEditor(typeof(HexGrid))]
    public class HexGridEditor : Editor
    {
        HexGrid hexGrid;

        public override void OnInspectorGUI()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                base.OnInspectorGUI();

                if (check.changed && false) //Disabled this because it's annoying
                    hexGrid.GenerateHexGrid();
            }

            if (!Application.isPlaying && GUILayout.Button("Generate Grid"))
            {
                hexGrid.GenerateHexGrid();
                //Scene is automaticly saved because sometimes editor doesn't realize scene is modified, in which case you cannot manually save it.
                EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            }

            if (!Application.isPlaying && GUILayout.Button("Remove Grid"))
            {
                hexGrid.RemoveHexGrid();
                //Scene is automaticly saved because sometimes editor doesn't realize scene is modified, in which case you cannot manually save it.
                EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            }
        }

        private void OnEnable()
        {
            hexGrid = (HexGrid)target;
        }
    }
}
#endif