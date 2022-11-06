using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization;

namespace DaiMangou.BridgedData
{
  
    //[Serializable]
    //public class EvTrigger : UnityEvent<UnityEngine.Object> { }



    /// <summary>
    ///  this is the base class data for the data of the nodes in your story.
    /// </summary>
    [Serializable]
    public class NodeData : ScriptableObject, ISerializationCallbackReceiver
    {




        /// <summary>
        ///  the name of the node data
        /// </summary>
        public string Name = "";
        /// <summary>
        /// 
        /// </summary>
        public string Text = "";
        /// <summary>
        /// a specific ID number that matches the ID number of the node it represents in the storyteller
        /// </summary>
      //  [HideInInspector]
        //public int DataID;
        public string UID = "";
        /// <summary>
        /// the  character who this node data belongs to
        /// </summary>
        //[HideInInspector]
        public string CharacterName = "";
        [UnityEngine.Serialization.FormerlySerializedAs("MyCharacter")]
        public CharacterNodeData CallingNodeData;
        /// <summary>
        /// the environment in which this node data belongs
        /// </summary>
        public string EnvironmentName ="";
        public EnvironmentNodeData Environment;
        //   [HideInInspector]
        public bool Pass;
        [SerializeField]
        public bool Runtime_Pass;
        // [HideInInspector]
        /// <summary>
        /// the data which is connected to this node data
        /// </summary>
        public List<NodeData> DataConnectedToMe = new List<NodeData>();
        // [HideInInspector]
        /// <summary>
        /// the node data which this data is connected to
        /// </summary>
        public List<NodeData> DataIconnectedTo = new List<NodeData>();
        /// <summary>
        /// The type of node data
        /// </summary>
        [HideInInspector]
        public Type type;
        /// <summary>
        /// the duration of the node "playback"
        /// </summary>
        [HideInInspector]
        public float Duration;
        /// <summary>
        /// This is the delay from the playback of the last nodeData behind this one
        /// </summary>
        [HideInInspector]
        public float Delay;
        /// <summary>
        /// The actual delay time calculated dynamically 
        /// </summary>
        [HideInInspector]
        public float RealtimeDelay;
        /// <summary>
        /// The real time in which the 
        /// </summary>
        // [HideInInspector]
        public float StartTime;
        /// <summary>
        /// 
        /// </summary>
        [HideInInspector]
        public float DurationSum;
        /// <summary>
        /// 
        /// </summary>
        [HideInInspector]
        public bool useTime;
        /// <summary>
        /// a flag to check if the Data is a descendant of a character that is designated as the player
        /// </summary>
        [FormerlySerializedAs("IsPlayer")]
        public bool IsInControl;
        /// <summary>
        /// flas to check if node data that typically does not use timing is forced to use a start time , e.g RouteNodes
        /// </summary>
        [FormerlySerializedAs("OverrideStartTime")]
        public bool OverrideTime;

        public List<string> LocalizedText = new List<string> { "" };



        public virtual void OnAfterDeserialize()
        {
    
      
        }
        public virtual void OnBeforeSerialize()
        {
        }


        public virtual void OnEnable()
        {
            Runtime_Pass = Pass;

            /* if (Conditions.Count == 0)
             {
                 var newCondition = CreateInstance(typeof(Condition)) as Condition;
                 newCondition.hideFlags = HideFlags.DontSave;
                 newCondition.name = name;
                 Conditions.Add(newCondition);
                 //     ExecutingEvent.Add(new UnityEvent());
                 //    Conditions.Last().targetEvent = ExecutingEvent.Last();
             }*/
        }

        /// <summary>
        ///  here we generate a new chain of events based on choices made in the story
        /// </summary>
        public void Aggregate()
        {
            if (this.type == typeof(LinkNodeData))
            {
                var link = (LinkNodeData)this;
                if (link.Loop)
                    return;
            }

            // look at each node data that we have connected to and tell it to aggrigate 
            for (var i = 0; i < DataIconnectedTo.Count; i++)
            {
                var dataIConnectedTo = DataIconnectedTo[i];
                // if this is a route , then 
                if (type == typeof(RouteNodeData))
                {

                    var route = (RouteNodeData)this;
                    if (route.Runtime_Pass)
                    {
                        //we assign its pass value of what we are connected to , to true of pass is true
                        dataIConnectedTo.Runtime_Pass = true;
                    }
                    else
                    {
                        // this means that the data we conneced to at i will have a pass value equal to false if the route id does not match
                         dataIConnectedTo.Runtime_Pass = i != route.RuntimeRouteID;

                    }

                    // handle some convergence node setups
                    if(route.ConvergenceMode)
                    {
                        if (route.DataConnectedToMe.All(n => n.Runtime_Pass == true))
                        {
                            route.Runtime_Pass = true;
                        }
                        else
                        {
                            route.Runtime_Pass = false;
                        }
                    }
                }
                else
                {
                    // set the pass value of what we are connected to , to be our pass value

                  //  if(dataIConnectedTo == null)
                 //   Debug.Log(Name);
                    dataIConnectedTo.Runtime_Pass = Runtime_Pass;

                }

                dataIConnectedTo.Aggregate();


            }


            
        }

      /// <summary>
      /// Overridden in inheriting classes and usd to process specidfic datasets
      /// </summary>
        public virtual void ProcessData()
        {
            // no longer necessary
            /*foreach (var data in DataIconnectedTo)
            {
                if (!data.Pass)
                {
                    if (data.type == typeof(RouteNodeData) || data.type == typeof(LinkNodeData) || data.type == typeof(EndNodeData))
                    {
                        data.ProcessData();
                    }
                }
            }*/
        }

      /*  public virtual void ProcessData(bool value)
        {

        }*/


        // No longer Used
        /* public void AddMeToThisList(List<NodeData> nodeDataList)
         {



             nodeDataList.Add(this);
             foreach (var data in DataIconnectedTo)
             {
                 if (data.Pass) continue;
                 if (data.type != typeof(DialogueNodeData) && data.type != typeof(ActionNodeData))
                 {
                     data.AddMeToThisList(nodeDataList);

                 }
             }

         }*/

    }
}
