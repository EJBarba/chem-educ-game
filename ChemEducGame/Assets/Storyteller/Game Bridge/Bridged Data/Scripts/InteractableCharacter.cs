using DaiMangou.BridgedData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DaiMangou.BridgedData
{
    public class InteractableCharacter : MonoBehaviour
    {
        public SceneData sceneData;
        public CharacterNodeData character;
        public int TargetCharacterIndex;
        public int RouteNumber;
        public bool UsesRouteNumber = true;


        public void Start()
        {

            character = sceneData.Characters[TargetCharacterIndex];

        }

    }
}
