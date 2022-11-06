using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DaiMangou.BridgedData;
using DaiMangou.Storyteller;

namespace DaiMangou.GameBridgeEditor
{
    [CustomEditor(typeof(CollisionInteractionTrigger))]
    [CanEditMultipleObjects]
    public class CollisionInteractionTriggerEditor : Editor
    {

        private void OnEnable()
        {
            collisionInteractionTrigger = (CollisionInteractionTrigger)target;


        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("This component is placed on youe 'Player' gameObject", MessageType.Info);

            if (collisionInteractionTrigger.InteractionTriggerComponent == null)
                EditorGUILayout.HelpBox("Please assign a Interaction System", MessageType.Info);

            DrawDefaultInspector();

            serializedObject.ApplyModifiedProperties();
        }

        private CollisionInteractionTrigger collisionInteractionTrigger;
    }
}