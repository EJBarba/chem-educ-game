using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DaiMangou.BridgedData;
using DaiMangou.Storyteller;

namespace DaiMangou.GameBridgeEditor
{
    [CustomEditor(typeof(ClickListener))]
    [CanEditMultipleObjects]
    public class ClickListenerEditor : Editor
    {

        private void OnEnable()
        {
            clickListener = (ClickListener)target;


        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("This component can be added to any gameobject which you wish to use to trigger route switching at runtime. This is assigned to Route Buttons at runtime By default", MessageType.Info);

            if (clickListener.interactionComponent == null)
                EditorGUILayout.HelpBox("Please assign a Interaction System", MessageType.Info);

            DrawDefaultInspector();

            serializedObject.ApplyModifiedProperties();
        }

        private ClickListener clickListener;
    }
}