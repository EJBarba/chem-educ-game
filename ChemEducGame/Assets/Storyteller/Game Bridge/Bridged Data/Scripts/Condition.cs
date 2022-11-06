using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace DaiMangou.BridgedData
{

    public enum UpdateRate
    {
        Once_Per_Frame = 0,
        Once_Per_NodeData = 1
    }

    public enum TimeUseMethod
    {

        RealtimeDelay = 0,
        Delay = 1,
        Duration,
        Custom

    }
    [Serializable]
    public class BoolCheckSystem
    {

        /// <summary>
        /// </summary>
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

        /// <summary>
        /// </summary>
        public void GetComponentMethods()
        {
           // var theNamespace = Components[ComponentIndex].GetType().Namespace == ""
             //   ? ""
             //   : Components[ComponentIndex].GetType().Namespace + ".";

            //  Debug.Log(Type.GetType(Components[ComponentIndex].GetType().Name));
          //  cacheMethods = Type.GetType(theNamespace + Components[ComponentIndex].GetType().Name, false)
               // .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            cacheMethods = Components[ComponentIndex].GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            /*  var boolMethods = new List<MethodInfo>();
              foreach (var b in cacheMethods)
                  if (b.ReturnType == typeof(bool))
                      boolMethods.Add(b);

              cacheMethods = boolMethods.ToArray();*/

            cacheMethods = cacheMethods.Where(b => b.ReturnType == typeof(bool)).ToArray();


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
        ///     once this bool is equal to a bool value you decide upon the condition system will be activated
        /// </summary>
        public bool ObjectiveBool;

        /// <summary>
        ///     here we use a speial class which allows us to Serialize methodInfo
        /// </summary>
        public SerializableMethodInfo[] serializedMethods = new SerializableMethodInfo[0];

        /// <summary>
        ///     This is the gameobject whose mono scripts we wish to analuze for public methods
        /// </summary>
        public GameObject TargetGameObject;

        /// <summary>
        ///     delegate that we will use to create a delegate method
        /// </summary>
        /// <returns></returns>
        public delegate bool delegateBool();

        /// <summary>
        ///     he delegate method
        /// </summary>
        public delegateBool theDelegate;

        public bool CanInvoke;

    }
    /// <summary>
    ///     the condition system allows foe any  accessible function to be called once a condition is met
    /// </summary>
    [Serializable]
    public class Condition : MonoBehaviour
    {


        // public void Awake()
        //  {
        //  }

        //  public void Start()
        //  {
        //   ConditionTimerCoroutine = ConditionTimer();

        //}




        /// <summary>
        /// </summary>
        public void ProcessConditionData()
        {


            if (AutoStart)
                if (!Invoked)
                {
                    if (!Repeat)
                    {
                        targetEvent.Invoke();
                        Invoked = true;
                    }
                    else
                        targetEvent.Invoke();
                    // in the next update we will let users set a invoke amount 
                }

            if (UseTime)
            {
                // setup the elapse timer and determine if we will delay by duration 
                if (!ConditionTimerStarted && !Invoked)
                {
                    if (!Repeat)
                    {
                        Invoked = ConditionTimerStarted = true;
                        coroutine = StartCoroutine(ConditionTimer());
                    }
                    else
                    {
                        if (!ConditionTimerStarted)
                        {
                            ConditionTimerStarted = true;
                            coroutine = StartCoroutine(ConditionTimer());

                        }
                    }
                }
            }

            switch (ConditionUpdateRate)
            {
                case UpdateRate.Once_Per_Frame:
                 
                    for (int m = 0; m < BoolChecks.Count; m++)
                    {
                        var boolCheck = BoolChecks[m];

                        if (boolCheck.TargetGameObject == null || UseTime || AutoStart) return;
                        var comp = boolCheck.Components[boolCheck.ComponentIndex];

                        if (boolCheck.theDelegate == null)
                            boolCheck.theDelegate =
                                (BoolCheckSystem.delegateBool)Delegate.CreateDelegate(typeof(BoolCheckSystem.delegateBool), comp, boolCheck.serializedMethods[boolCheck.MethodIndex].methodName);


                        if (boolCheck.theDelegate() != boolCheck.ObjectiveBool)
                            boolCheck.CanInvoke = false;
                        else
                            boolCheck.CanInvoke = true;

                    }
                    if (BoolChecks.Any(b => b.CanInvoke == false))
                        return;

                        if (!Invoked)
                        {
                            if (!Repeat)
                            {
                                Invoked = true;
                                targetEvent.Invoke();                               
                            }
                            else
                                targetEvent.Invoke();

                           
                        }

                    break;

                case UpdateRate.Once_Per_NodeData:

                    for (int m = 0; m < BoolChecks.Count; m++)
                    {
                        var boolCheck = BoolChecks[m];

                        if (boolCheck.TargetGameObject == null || UseTime || AutoStart) return;
                        var comp = boolCheck.Components[boolCheck.ComponentIndex];

                        if (RanConditionCheck && m!= BoolChecks.Count-1) return;
                        RanConditionCheck = true;

                        if (boolCheck.theDelegate == null)
                            boolCheck.theDelegate =
                                (BoolCheckSystem.delegateBool)Delegate.CreateDelegate(typeof(BoolCheckSystem.delegateBool), comp, boolCheck.serializedMethods[boolCheck.MethodIndex].methodName);


                        if (boolCheck.theDelegate() != boolCheck.ObjectiveBool)
                            boolCheck.CanInvoke = false;
                        else
                            boolCheck.CanInvoke = true;
                    }



                    if (BoolChecks.Any(b => b.CanInvoke == false))
                        return;

                    if (!Invoked)
                    {
                        if (!Repeat)
                        {
                            Invoked = true;
                            targetEvent.Invoke();                            
                        }
                        else
                            targetEvent.Invoke();


                    }
                    break;

                default:

                    break;


            }



        }


        /// <summary>
        /// </summary>
        /// <returns></returns>
        public IEnumerator ConditionTimer()
        {


            var waitTime = 0f;
            switch (timeUseMethod)
            {
                case TimeUseMethod.RealtimeDelay:
                    waitTime = interaction.ActiveNodeData.Delay;
                    break;
                case TimeUseMethod.Delay:
                    waitTime = interaction.ActiveNodeData.Delay;
                    break;
                case TimeUseMethod.Duration:
                    waitTime = interaction.ActiveNodeData.Duration;
                    break;
                case TimeUseMethod.Custom:
                    waitTime = CustomWaitTime;
                    break;
            }
            yield return new WaitForSeconds(waitTime);
            ConditionTimerStarted = false;
            targetEvent.Invoke();


        }

        #region if statement variables
/*
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
        ///     once this bool is equal to a bool value you decide upon the condition system will be activated
        /// </summary>
        public bool ObjectiveBool;

        /// <summary>
        ///     here we use a speial class which allows us to Serialize methodInfo
        /// </summary>
        public SerializableMethodInfo[] serializedMethods = new SerializableMethodInfo[0];

        /// <summary>
        ///     This is the gameobject whose mono scripts we wish to analuze for public methods
        /// </summary>
        public GameObject TargetGameObject;

        /// <summary>
        ///     delegate that we will use to create a delegate method
        /// </summary>
        /// <returns></returns>
        private delegate bool delegateBool();

        /// <summary>
        ///     he delegate method
        /// </summary>
        private delegateBool theDelegate;*/
        #endregion

        /// <summary>
        ///     flag to check i the target event is invoked
        /// </summary>
        // [NonSerialized]
        public bool Invoked;


        /// <summary>
        ///     a flag to check if the target event is to be invoked automatically once a condition is met or if the nodedata is
        ///     being processed
        /// </summary>
        public bool AutoStart;

        /// <summary>
        /// </summary>
        private bool ConditionTimerStarted;

        public bool Disabled;



        /// <summary>
        ///     This is the gameobject which this Condition Component is atached to
        /// </summary>
        public GameObject Self;

        /// <summary>
        ///     this unity event only act as a proxy for an unity event in the ReflectedData
        /// </summary>
        public UnityEvent targetEvent = new UnityEvent();

        public TimeUseMethod timeUseMethod = TimeUseMethod.RealtimeDelay;

        // public bool PlaySoundEffect;
        //  public bool PlayVoiceClip;
        /// <summary>
        ///     if the node data uses time then the
        /// </summary>
        public bool UseTime;

        /// <summary>
        /// 
        /// </summary>
        public float CustomWaitTime = 0;

        // public IEnumerator ConditionTimerCoroutine;
        public Coroutine coroutine;

        /// <summary>
        /// When true, repeat will tell a condition to constantly execute its function
        /// </summary>
        public bool Repeat;

        public UpdateRate ConditionUpdateRate = UpdateRate.Once_Per_Frame;

        public bool RanConditionCheck;

        public List<BoolCheckSystem> BoolChecks = new List<BoolCheckSystem>();

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

        /* public int TargetConditionIndex;
         public Condition TargetCondition;
         public bool TargetConditionObjectiveBool;*/
    }
}