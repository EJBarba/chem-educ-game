using System;
using System.Collections.Generic;
using UnityEngine;

namespace DaiMangou.BridgedData
{
    /// <summary>
    ///     this is a piece of data that has an ID vlue matching a node in the storyteller scene it is uses in parallel with
    ///     the
    /// </summary>
    [Serializable]
    public class ReflectedData : MonoBehaviour
    {
        /// <summary>
        ///     we keep a list of conditions under which certain function can be triggered
        /// </summary>
        public List<Condition> Conditions = new List<Condition>();

        /// <summary>
        ///     This is the gameobject which this script is attached to
        /// </summary>
        public GameObject self;

        /// <summary>
        ///     this is the ID value used as a reference point for NodeData (Do not remove or edit)
        /// </summary>
        public string UID = "";


        private void Awake()
        {
            /*  if(Conditions.Count == 0)
              {
                  var newCondition = new GameObject(gameObject.name + "Condition " + Conditions.Count);
                  newCondition.AddComponent<Condition>();
                  var _condition = newCondition.GetComponent<Condition>();
                  _condition.DialoguerGameObject = DialoguerGameObject;
                  _condition.self = newCondition;
                  newCondition.transform.SetParent(transform);
                  // newCondition.hideFlags = HideFlags.HideInHierarchy;
                  Conditions.Add(newCondition.GetComponent<Condition>());

              }*/
            //   self = gameObject;
        }

        #region Interaction Specific 

        /// <summary>
        ///     If the refelected data is generated under a gameboject with a Character Component then the gameobjects is set here
        /// </summary>
        public GameObject InteractionGameObject;

        /// <summary>
        ///     value is set by get set
        /// </summary>
        public Interaction interaction;

        /// <summary>
        ///     If the refelected data is generated under a gameboject with a Character Component then the dialogue scriptreference
        ///     is set here
        /// </summary>
        public Interaction interactionComponent
        {
            get
            {
                if (interaction == null) interaction = InteractionGameObject.GetComponent<Interaction>();

                return interaction;
            }
            set { interaction = value; }
        }

        #endregion

        #region nodeData Specific

        [Serializable]
        public class CharacternNodeSpecificData
        {
            public bool IsTheMainPlayer;
        }

        public CharacternNodeSpecificData CharacterSpecificData = new CharacternNodeSpecificData();

        [Serializable]
        public class LanguageSpecificDataForRouteNodeData
        {
            public List<string> RouteTitles = new List<string>();
        }
        [Serializable]
        public class RouteNodeSpecificData
        {
            public List<string> AlternativeRouteTitles = new List<string>();

            public List<LanguageSpecificDataForRouteNodeData> LanguageSpecificData = new List<LanguageSpecificDataForRouteNodeData> { new LanguageSpecificDataForRouteNodeData() };
        

            public bool UseAlternativeRouteTitles;
        }

        public RouteNodeSpecificData RouteSpecificDataset = new RouteNodeSpecificData();

        [Serializable]
        public class ActionNodeSpecificData
        {
            public bool OverrideUseSoundEffect;
            public bool OverrideUseStoryboardImage;

            [NonSerialized] public bool SoundEffectWasPlayed;

            public bool useDurationLengthForSoundEffects = true;
            public bool useSoundEffectLength;
        }

        public ActionNodeSpecificData ActionSpecificData = new ActionNodeSpecificData();

        [Serializable]
        public class DialogueNodeSpecificData
        {
            public bool OverrideTextDisplayMethod;
            public bool OverrideUseSoundEffect;
            public bool OverrideUseStoryboardImage;
            public bool OverrideUseVoiceover;

            [NonSerialized] public bool SoundEffectWasPlayed;

            public bool useDurationLengthForSoundEffects = true;
            public bool useDurationLengthForVoiceOver = true;
            public bool useSoundEffectLength;
            public bool useVoiceOverLength;

            //public int VoiceClipLoopTimes = 0;
            [NonSerialized] public bool VoiceClipWasPlayed;
        }

        public DialogueNodeSpecificData DialogueSpecificData = new DialogueNodeSpecificData();

        [Serializable]
        public class LinkNodeSpecificData
        {
        }

        public LinkNodeSpecificData LinkSpecificData = new LinkNodeSpecificData();

        [Serializable]
        public class EndNodeSpecificData
        {
            public int sceneToContinueTo; // not setup
        }

        public EndNodeSpecificData EndSpecificData = new EndNodeSpecificData();

        [Serializable]
        public class EnvironmentNodeSpecificData
        {
        }

        public EnvironmentNodeSpecificData EnvironmentSpecificData = new EnvironmentNodeSpecificData();

        #endregion
    }
}