using System;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace DaiMangou.BridgedData
{
    [DisallowMultipleComponent]
    public class InteractionTriggerManager: MonoBehaviour
    {
        public Interaction InteractionComponent;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetCharacter"></param>
        public void BeginInteraction(InteractableCharacter targetCharacter)
        {

            if (InteractionComponent.CommunicatingCharacters.Contains(targetCharacter.character)) return;
            if (InteractionComponent.CommunicatingCharacters.Count != 0)
                InteractionComponent.ReturnPointUID = InteractionComponent.ActiveNodeData.UID;
            InteractionComponent.CommunicatingCharacters.Add(targetCharacter.character);
            InteractionComponent.TargetRoute = targetCharacter.RouteNumber;
            InteractionComponent.GenerateActiveDialogueSet();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetCharacter"></param>
        public void EndInteraction(InteractableCharacter targetCharacter)
        {
            InteractionComponent.ResetConditions();
            if(InteractionComponent.ActiveNodeData.CallingNodeData = targetCharacter.character)
            {
                if(InteractionComponent.CommunicatingCharacters.Count >1)
                {
                    // here we need to finda way to locate a suitable return point for the interaction to continue from
                }
            }
            InteractionComponent.CommunicatingCharacters.Remove(targetCharacter.character);
            InteractionComponent.ResetConditions();
            InteractionComponent.CleanUp();

        }

        /// <summary>
        /// Call this function to begin interactingwith a character
        /// </summary>
        /// <param name="other"></param>
        public void BeginInteraction(GameObject other)
        {
            if (other.GetComponent<CollisionInteractionReceiver>() == null) return;
            var collisionInteractionReceiver = other.GetComponent<CollisionInteractionReceiver>();
            var npcCharacter = other.transform.parent.GetComponent<InteractableCharacter>();
            if (collisionInteractionReceiver.added) return;
            if (InteractionComponent.CommunicatingCharacters.Contains(npcCharacter.character)) return;
            if (InteractionComponent.CommunicatingCharacters.Count != 0)
                InteractionComponent.ReturnPointUID = InteractionComponent.ActiveNodeData.UID;
            InteractionComponent.CommunicatingCharacters.Add(npcCharacter.character);
            InteractionComponent.TargetRoute = npcCharacter.RouteNumber;
            collisionInteractionReceiver.added = true;
            InteractionComponent.GenerateActiveDialogueSet(!npcCharacter.UsesRouteNumber);
          
           
        }

        /// <summary>
        /// Call this function to end interaction with a character
        /// </summary>
        /// <param name="other"></param>
        public void EndInteraction(GameObject other)
        {
            if (other.GetComponent<CollisionInteractionReceiver>() == null) return;
            var collisionInteractionReceiver = other.GetComponent<CollisionInteractionReceiver>();
            var npcCharacter = other.transform.parent.GetComponent<InteractableCharacter>();
            if (!collisionInteractionReceiver.added) return;
            InteractionComponent.ResetConditions();
            InteractionComponent.CommunicatingCharacters.Remove(npcCharacter.character);
            collisionInteractionReceiver.added = false;
            InteractionComponent.CleanUp();
    
           
        }

    }
}
