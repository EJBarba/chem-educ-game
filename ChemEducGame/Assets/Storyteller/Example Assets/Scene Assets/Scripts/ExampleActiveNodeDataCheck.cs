using DaiMangou.BridgedData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleActiveNodeDataCheck : MonoBehaviour
{
    public Interaction InteractionComponent;

    public bool IsActionDialogueOrRoute()
    {
        return (InteractionComponent.CurrentNodeIsAction() || InteractionComponent.CurrentNodeIsDialogue() || InteractionComponent.CurrentNodeIsRoute());
    }
}
