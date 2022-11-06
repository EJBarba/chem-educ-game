using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DaiMangou.BridgedData
{
    /// <summary>
    /// The History dataset store individual character interaction data
    /// </summary>
    [Serializable]
   public class History
    {
        public string Name = "";
        public CharacterNodeData CharacterNodeData_;
        public DialogueNodeData DialogueNodeData_;
        public string DialogueText;
        public string TimeOfInteraction;

        public History(CharacterNodeData characterNodeData, DialogueNodeData dialogueNodeData,string dialogueText, string time = "")
        {
            Name = characterNodeData.Name + " dialogue";
            CharacterNodeData_ = characterNodeData;
            DialogueNodeData_ = dialogueNodeData;
            DialogueText = dialogueText;
            TimeOfInteraction = time;

        }
    }
}