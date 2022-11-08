using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DaiMangou.BridgedData

{
    /// <summary>
    /// Representatation of End Node Data
    /// </summary>
    [Serializable]
    public class EndNodeData : NodeData
    {


        public override void OnEnable()
        {
            type = GetType();
            base.OnEnable();
        }

        /// <summary>
        /// executes the base ProcessData function along with this nodes ProcessData function
        /// </summary>
        public override void ProcessData()
        {
            base.ProcessData();
            // here we will simply load a scene that exists at sceneToContinueTo

        }
    }
}
