using UnityEngine;
using UnityEngine.Events;

namespace DaiMangou.BridgedData
{
    /// <summary>
    /// This sctipt is added to any gameobject with a collider belongingto a NPC which youur player can interact in 
    /// </summary>
    [DisallowMultipleComponent]
    public class CollisionInteractionReceiver : MonoBehaviour
    {
        [HideInInspector]
        public bool added;

        public bool InteractingwithPlayer
        {
            get
            {
                return added;
            }
        }

     //   public Interaction TargetInteraction;
        public UnityEvent OnEnterEvent = new UnityEvent();
        public UnityEvent OnExitEvent = new UnityEvent();
    }
}