using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;


namespace DaiMangou.BridgedData
{
    /// <summary>
    /// representation of Dialogue Node data
    /// </summary>
    [Serializable]
    public class DialogueNodeData : NodeData
    {


        /// <summary>
        /// 
        /// </summary>
        public List<AudioClip> LocalizedVoiceRecordings = new List<AudioClip> { null };

        /// <summary>
        /// 
        /// </summary>
        public List<AudioClip> LocalizedSoundEffects = new List<AudioClip> { null };
        /// <summary>
        /// Storyboard image copied from the Dialogue Node
        /// </summary>
        public Sprite StoryboardImage = null;
        /// <summary>
        /// your string tag 
        /// </summary>
        public string Tag = "";




        //internal CharacterTextDisplayMode OverridenCharacterTextDisplayMode = CharacterTextDisplayMode.Instant;



        public override void OnEnable()
        {
            type = GetType();
            useTime = true;
            base.OnEnable();

           
        }

        /// <summary>
        /// executes the base ProcessData function along with this nodes ProcessData function
        /// </summary>
        public override void ProcessData()
        {
            base.ProcessData();
        }
    }
}