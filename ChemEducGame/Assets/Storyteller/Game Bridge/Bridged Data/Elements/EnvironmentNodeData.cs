using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DaiMangou.BridgedData
{
    /// <summary>
    /// representation of Environment Node Data (Not Fully Setup)
    /// </summary>
    [Serializable]
    public class EnvironmentNodeData : NodeData
    {
      //  public string EnvironmentLocation = "";

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
        }
    }
}

