using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryTellerAutoStart : MonoBehaviour
{
   private DaiMangou.BridgedData.Interaction interactionSystem;
    private void Awake() {
        interactionSystem = this.gameObject.GetComponent<DaiMangou.BridgedData.Interaction>();
    }
    IEnumerator Start() {
        yield return new WaitForSeconds(0.2f);
        interactionSystem.GenerateActiveDialogueSet();
    }
}
