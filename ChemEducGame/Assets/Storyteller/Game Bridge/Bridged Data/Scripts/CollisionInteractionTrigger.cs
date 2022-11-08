using UnityEngine;
using UnityEngine.Events;

namespace DaiMangou.BridgedData
{
    /// <summary>
    /// The player trigger is placed on a gameobject with a collider which is intended to trigger an interaction with an NPC when it enters the NPCTrigger area
    /// </summary>
    [DisallowMultipleComponent]
    public class CollisionInteractionTrigger : MonoBehaviour
    {
        public InteractionTriggerManager InteractionTriggerComponent;
        public string TargetTag = "";
        public UnityEvent OnBeginInteraction = new UnityEvent();
        public UnityEvent OnEndInteraction = new UnityEvent();
      


        /// <summary>
        ///  if we enter the collision area of the NPC trigger or a NPC enters the players collision area then we tell the character component to egin setting up an interaction for that NPC
        /// </summary>
        /// <param name="other"></param>
        public void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(TargetTag)) return;
            InteractionTriggerComponent.BeginInteraction(other.gameObject);
            var collisionInteractionReceiver = other.GetComponent<CollisionInteractionReceiver>();
            collisionInteractionReceiver.OnEnterEvent.Invoke();
            OnBeginInteraction.Invoke();
        }
        /// <summary>
        /// Ifthe character or NPC leaves the Players collisaion area or the player leaves the NPC trigger area we reset and 
        /// </summary>
        /// <param name="other"></param>
        public void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(TargetTag)) return;
            InteractionTriggerComponent.EndInteraction(other.gameObject);
            var collisionInteractionReceiver = other.GetComponent<CollisionInteractionReceiver>();
            collisionInteractionReceiver.OnExitEvent.Invoke();
            OnEndInteraction.Invoke();
        }



    }
}