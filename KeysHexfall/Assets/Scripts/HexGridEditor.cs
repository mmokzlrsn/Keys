#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace HexagonPuzzle.Editors
{ 
    [CustomEditor(typeof(HexGrid))]
    public class HexGridEditor : Editor //this will helps me to generate or remove hex grids
    {
        HexGrid hexGrid;

        public override void OnInspectorGUI()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                base.OnInspectorGUI();

                if (check.changed && false)  
                    hexGrid.GenerateHexGrid();
            }

            if (!Application.isPlaying && GUILayout.Button("Generate Grid"))
            {
                hexGrid.GenerateHexGrid();
                
                EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            }

            if (!Application.isPlaying && GUILayout.Button("Remove Grid"))
            {
                hexGrid.RemoveHexGrid();
                
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