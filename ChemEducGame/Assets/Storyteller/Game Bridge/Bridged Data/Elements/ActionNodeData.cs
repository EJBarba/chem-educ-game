using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DaiMangou.BridgedData
{
    /// <summary>
    /// A representation of Action node data 
    /// </summary>
    [Serializable]
    public class ActionNodeData : NodeData
    {

        /// <summary>
        /// 
        /// </summary>
        public string ActionName = ""; // not yet uese
        /// <summary>
        /// 
        /// </summary>
        public List<AudioClip> LocalizedSoundEffects = new List<AudioClip> { null };
        /// <summary>
        /// Storyboard image copied from the Action Ndoe
        /// </summary>
        public Sprite StoryboardImage = null;
        /// <summary>
        /// your string tag 
        /// </summary>
        public string Tag = "";



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
