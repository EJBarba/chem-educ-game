using System.Collections.Generic;
using System.IO;
using System.Linq;
using DaiMangou.BridgedData;
using DaiMangou.Storyteller;
using UnityEditor;
using UnityEngine;


namespace DaiMangou.GameBridgeEditor
{
    [CustomEditor(typeof(InteractableCharacter))]
    [CanEditMultipleObjects]
    public class InteractableCharacterEditor : Editor
    {

        private void OnEnable()
        {
            interactableCharacter = (InteractableCharacter)target;
            TargetCharacterIndex = serializedObject.FindProperty("TargetCharacterIndex");
            RouteNumber = serializedObject.FindProperty("RouteNumber");
            UsesRouteNumber =  serializedObject.FindProperty("UsesRouteNumber");
        }

        public override void OnInspectorGUI()
        {

            EditorGUILayout.HelpBox("This component is placed on gameobject who you wish to interact with , Using the Interaction component", MessageType.Info);
            #region tell users to assign a SceneData Asset

            if (interactableCharacter.sceneData == null)
                EditorGUILayout.HelpBox("Please assign a SceneData file in the area below", MessageType.Info);

            interactableCharacter.sceneData =
                (SceneData)EditorGUILayout.ObjectField(interactableCharacter.sceneData, typeof(SceneData), false);

            #endregion

            if (interactableCharacter.sceneData == null)
                return;

            if (characternames.Count == 0)
                foreach (var character in interactableCharacter.sceneData.Characters)
                    characternames.Add(character.CharacterName);

            if (GUILayout.Button(UsesRouteNumber.boolValue ? "Uses Route Number: On" : "Uses Route Number: Off"))
                UsesRouteNumber.boolValue = !UsesRouteNumber.boolValue;

            TargetCharacterIndex.intValue = EditorGUILayout.Popup(interactableCharacter.TargetCharacterIndex, characternames.ToArray());

            interactableCharacter.character = interactableCharacter.sceneData.Characters[TargetCharacterIndex.intValue];

            if(UsesRouteNumber.boolValue)
            RouteNumber.intValue = EditorGUILayout.IntField("Route Number", RouteNumber.intValue);

            serializedObject.ApplyModifiedProperties();
        }

        private readonly List<string> characternames = new List<string>();
        private InteractableCharacter interactableCharacter;
        public SerializedProperty TargetCharacterIndex;
        public SerializedProperty RouteNumber;
        public SerializedProperty UsesRouteNumber;
    }
}
