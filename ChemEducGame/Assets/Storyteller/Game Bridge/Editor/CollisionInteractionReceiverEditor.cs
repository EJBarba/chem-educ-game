    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DaiMangou.BridgedData;
using DaiMangou.Storyteller;

namespace DaiMangou.GameBridgeEditor
{
    [CustomEditor(typeof(CollisionInteractionReceiver))]
    [CanEditMultipleObjects]
    public class CollisionInteractionReceiverEditor : Editor
    {

        public void OnEnable()
        {
            collisionInteractionReceiver = (CollisionInteractionReceiver)target;


          

            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("This component is placed on Non Player Characters on a gameobject with a collider which is the immedite child of the NPC gameObject ", MessageType.Info);
            DrawDefaultInspector();
        }

        public CollisionInteractionReceiver collisionInteractionReceiver;
    }
}