using DaiMangou.Storyteller;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace DaiMangou.BridgedData
{
    /// <summary>
    ///  This class is simply responsible for allowing the copying and pasting of condition data across conditions
    /// </summary>
    public static class ConditionCopy
    {
        static BoolCheckSystem BoolCheck = new BoolCheckSystem();

        static bool AutoStart;

        static GameObject cachedTargetObject;

        static internal MethodInfo[] cacheMethods = new MethodInfo[0];

        static int ComponentIndex;

        static internal  Component[] Components = new Component[0];

        static bool Disabled;

        static bool Invoked;

        static int MethodIndex;

        static bool ObjectiveBool;

        static GameObject Self;

        static internal SerializableMethodInfo[] serializedMethods = new SerializableMethodInfo[0];

        public static UnityEvent TargetEvent = new UnityEvent();

        static GameObject TargetGameObject;

        static TimeUseMethod timeUseMethod = TimeUseMethod.RealtimeDelay;

        static bool UseTime;

        static float CustomWaitTime = 0;

        static bool Repeat;

        static UpdateRate ConditionUpdateRate = UpdateRate.Once_Per_Frame;

        static GameObject InteractionGameObject;

        static bool EventsOnly;
        // Copy and past could have all been done i na single function but this is just the setup for testing 
        public static void MakeCopy(Condition condition, int index, bool eventsOnly = false)
        {
            EventsOnly = eventsOnly;

            if (!EventsOnly)
            {
                
                Debug.Log(condition.BoolChecks.Count);
                BoolCheck = condition.BoolChecks[index];
                AutoStart = condition.AutoStart;
                Disabled = condition.Disabled;
                Self = condition.Self;
                timeUseMethod = condition.timeUseMethod;
                UseTime = condition.UseTime;
                CustomWaitTime = condition.CustomWaitTime;
                Repeat = condition.Repeat;
                ConditionUpdateRate = condition.ConditionUpdateRate;

            }
            else
            {
                //  var e = Delegate.CreateDelegate(typeof(UnityAction), condition.targetEvent.GetPersistentTarget(0),
                //    condition.targetEvent.GetPersistentMethodName(0));


                // TargetEvent.
            }
            //  targetEvent = condition.targetEvent;




        }

        public static void Paste(Condition condition, int index)
        {
            if (!EventsOnly)
            {

                condition.BoolChecks[index] = BoolCheck;
                condition.AutoStart = AutoStart;
                condition.Disabled = Disabled;
                condition.Self = Self;
                condition.timeUseMethod = timeUseMethod;
                condition.UseTime = UseTime;
                condition.CustomWaitTime = CustomWaitTime;
                condition.Repeat = Repeat;
                condition.ConditionUpdateRate = ConditionUpdateRate;


            }
            /*  else
              {

                  condition.targetEvent = TargetEvent;
              }*/
            // 


        }
    }
}