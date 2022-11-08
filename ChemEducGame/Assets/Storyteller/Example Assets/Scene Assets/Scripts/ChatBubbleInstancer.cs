using DaiMangou.BridgedData;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatBubbleInstancer : MonoBehaviour
{
    public Interaction TargetInteractionComponent;
    public GameObject RightChatBubbleGameObject;
    public GameObject LeftChatBubbleGameObject;
    public GameObject ParentGameObject;
    public Button MoveNextButtonbutton;


    public void InstanceChatBubbleAsOtherCharacter()
    {
        if (!TargetInteractionComponent.ActiveNodeData.IsInControl)
            StartCoroutine(beginInvokeProcess());
    }

    IEnumerator beginInvokeProcess()

    {
        MoveNextButtonbutton.interactable = false;
        yield return new WaitForSeconds(Random.Range(1.4f,4f)); // or just use the realtime delay or delay time (TargetDialoguer.ActiveNodeData.RealtimeDelay)... which makes WAY MORE SENSE but for this scen we are just working based on interaction position
        Instance();
    }

    public void InstanceChatBubbleAsPlayer()
    {
        if (TargetInteractionComponent.ActiveNodeData.IsInControl)
            Instance();
    }

private void Instance()
    {
        MoveNextButtonbutton.interactable = true;
        if (TargetInteractionComponent.ActiveNodeData.type != typeof(DialogueNodeData)) return;
        // we can instance that chat bubbles on the left and right by using a bubble designed to dock to the left and another designed to dock to the right 
        //  var instancedChatBubble = Instantiate(TargetDialoguer.ActiveNodeData.IsPlayer ? RightChatBubbleGameObject : LeftChatBubbleGameObject, ParentGameObject.transform);
        // instead of using the above code to instance , we will use the position of each scharacter in the character list in scene date to set the instance position.

        var indexOfCharacterInSceneDataCharacterList = TargetInteractionComponent.sceneData.Characters.IndexOf(TargetInteractionComponent.ActiveNodeData.CallingNodeData)+2;
        // if the indexOfCharacterInSceneDataCharacterList% 2 is !== 0 then we dock it to the left
        var instancedChatBubble = Instantiate(indexOfCharacterInSceneDataCharacterList%2 ==0 ? RightChatBubbleGameObject : LeftChatBubbleGameObject, ParentGameObject.transform);
        instancedChatBubble.transform.GetChild(indexOfCharacterInSceneDataCharacterList % 2 == 0 ? 0 : 1).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = TargetInteractionComponent.GetHistoryDialogueAtIndex();

    }
}
