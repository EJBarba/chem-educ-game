using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using System.Reflection;

namespace DaiMangou.BridgedData
{
    /// <summary>
    /// This dataset stores individual info on keyword filters
    /// </summary>
    [Serializable]
    public class KeywordFilter
    {
        public string KeyWord = "Replace this";
        public string ReplacementString = "With this";

        public bool StaticKeywordMethod = true;
        public bool StaticReplacementStringMethod = true;

        public DynamicMethod DynamicKeyword = new DynamicMethod();
        public DynamicMethod DynamicReplacementString = new DynamicMethod();
        public bool Disabled;
        public Color NewColour = Color.white;

 
    }
}
