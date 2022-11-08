using System.Collections.Generic;
using System.IO;
using System.Linq;
using DaiMangou.BridgedData;
using DaiMangou.Storyteller;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Grid = DaiMangou.Storyteller.Grid;

namespace DaiMangou.GameBridgeEditor
{
    [CustomEditor(typeof(SceneData))]
    [CanEditMultipleObjects]
    public class SceneDataEditor : Editor
    {

        private SceneData targetSceneData;
        //private List<Rect> areas = new List<Rect>();
        private Rect ScreenRect = new Rect();
        private EditorWindow[] edwin;
      //  private Vector2 scrollView;

        private void OnEnable()
        {
            targetSceneData = (SceneData)target;

            edwin = Resources.FindObjectsOfTypeAll(typeof(EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow")) as EditorWindow[];
        }
        public override void OnInspectorGUI()
        {

           //    DrawDefaultInspector();

           if (edwin.Length == 0)
            {
                Repaint();
                edwin = Resources.FindObjectsOfTypeAll(typeof(EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow")) as EditorWindow[];
            }

            ScreenRect.size = new Vector2(edwin[0].position.width, edwin[0].position.height);

   

            for (int i = 0; i < targetSceneData.Characters.Count; i++)
            {
                GUILayout.Label(targetSceneData.Characters[i].Name);
                GUILayout.Space(5);
                Separator2();
                GUILayout.Space(5);
                   
            }
            

        }

        private void Separator2()
        {
            var area = GUILayoutUtility.GetLastRect().AddRect(-15, 0);

            GUI.DrawTexture(area.ToLowerLeft(Screen.width, 1), Textures.DuskLight);
        }
    }

}