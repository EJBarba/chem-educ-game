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
    [System.Serializable]
    public class DynamicMethod
    {
        /// <summary>
        ///     helps to determine the state of the Condition editor (do not edit)
        /// </summary>
        public GameObject cachedTargetObject;

        /// <summary>
        ///     This is an array of all the public methods of a Component
        /// </summary>
        public MethodInfo[] cacheMethods = new MethodInfo[0];

        /// <summary>
        ///     this is the index value of the component in the Components array
        /// </summary>
        public int ComponentIndex;

        /// <summary>
        ///     this is an array of all the components on a TargetGameObject
        /// </summary>
        public Component[] Components = new Component[0];

        /// <summary>
        ///     this is the index value of the target Method in the serializedMethods array
        /// </summary>
        public int MethodIndex;

        /// <summary>
        ///     This is the gameobject whose mono scrits we wish to analuze for public methods
        /// </summary>
        public GameObject TargetGameObject;

        /// <summary>
        ///     here we use a speial class which allows us to Serialize methodInfo
        /// </summary>
        public SerializableMethodInfo[] serializedMethods = new SerializableMethodInfo[0];


        public void GetGameObjectComponents()
        {
            Components = TargetGameObject.GetComponents(typeof(MonoBehaviour)); // Component
        }

        /// <summary>
        /// </summary>
        /// <param name="index"></param>
        public void SetComponent(object index)
        {
            ComponentIndex = (int)index;
        }

        public void GetComponentMethods()
        {
            var theNamespace = Components[ComponentIndex].GetType().Namespace == ""
                ? ""
                : Components[ComponentIndex].GetType().Namespace + ".";

            //  Debug.Log(Type.GetType(Components[ComponentIndex].GetType().Name));
            cacheMethods = Type.GetType(theNamespace + Components[ComponentIndex].GetType().Name, false)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            cacheMethods = cacheMethods.Where(b => b.ReturnType == typeof(string)).ToArray();

            serializedMethods = new SerializableMethodInfo[cacheMethods.Length];

            for (var i = 0; i < cacheMethods.Length; i++)
                serializedMethods[i] = new SerializableMethodInfo(cacheMethods[i]);
        }

        /// <summary>
        /// </summary>
        /// <param name="index"></param>
        public void SetMethod(object index)
        {
            MethodIndex = (int)index;
        }
    }
}
