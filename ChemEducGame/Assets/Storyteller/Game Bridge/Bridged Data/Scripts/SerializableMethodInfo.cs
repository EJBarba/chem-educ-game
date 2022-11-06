//This script was created by Bunny83 

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace DaiMangou.BridgedData
{
    [Serializable]
    public class SerializableMethodInfo : ISerializationCallbackReceiver
    {
        public int flags;
        public MethodInfo methodInfo;
        public string methodName;
        public List<SerializableType> parameters;
        public SerializableType type;

        public SerializableMethodInfo(MethodInfo aMethodInfo)
        {
            methodInfo = aMethodInfo;
        }

        public void OnBeforeSerialize()
        {
            if (methodInfo == null)
                return;
            type = new SerializableType(methodInfo.DeclaringType);
            methodName = methodInfo.Name;
            if (methodInfo.IsPrivate)
                flags |= (int)BindingFlags.NonPublic;
            else
                flags |= (int)BindingFlags.Public;
            if (methodInfo.IsStatic)
                flags |= (int)BindingFlags.Static;
            else
                flags |= (int)BindingFlags.Instance;
            var p = methodInfo.GetParameters();
            if (p != null && p.Length > 0)
            {
                parameters = new List<SerializableType>(p.Length);
                foreach (var param in p)
                    parameters.Add(new SerializableType(param.ParameterType));
            }
            else
            {
                parameters = null;
            }
        }

        public void OnAfterDeserialize()
        {
            if (type == null || string.IsNullOrEmpty(methodName))
                return;

            var t = type.type;
            Type[] param = null;
            if (parameters != null && parameters.Count > 0)
            {
                param = new Type[parameters.Count];
                for (var i = 0; i < parameters.Count; i++) param[i] = parameters[i].type;
            }
          //  Debug.Log(t);
            if(t!= null)
            methodInfo = param == null ? t.GetMethod(methodName, (BindingFlags)flags) : t.GetMethod(methodName, (BindingFlags)flags, null, param, null);

        }
    }
}