using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// To access the Game Bridge components that are used in game, you must use Daimangou.BridgedData
using DaiMangou.BridgedData;

/// <summary>
/// This is only for scripting documentation reference and can be live tested in scenes.
/// </summary>
public class ScriptAccessExamples : MonoBehaviour
{
    private new Collider collider;
    public Camera cam;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    public void ExampleFunction()
    {

        #region ClickListener Access
        //In this example, we will access the ClickListener and use raycasting to click on 3D gameObjects
        //Whenever a Route is being processed (character is making a choice) 
        // any gameobject with a ClickListener script attached will wait for a click input and pass its set route 
        //value to the InteractionComponent for the choice to be made and processed 

        if (Input.GetMouseButtonDown(0))
        {

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (collider.Raycast(ray, out hit, 100))
                GetComponent<ClickListener>().SwitchRoute();
        }

        #endregion

        #region CollidionInteractionReceiver

        // this comonent will likely never need to be acced via custom script .
        // you can access the OnEnterEvent and OnExitEvent of thiso script and invoke them

        // trigger an event- when called from code there can be triggered at any time 
        GetComponent<CollisionInteractionReceiver>().OnEnterEvent.Invoke();
        // trigger an event- when called from code there can be triggered at any time 
        GetComponent<CollisionInteractionReceiver>().OnExitEvent.Invoke();

        #endregion

        #region CollidionInteractionTrigger

        // this comonent will likely never need to be acced via custom script .
        // you can access the OnEnterEvent and OnExitEvent of thiso script and invoke them

        // trigger an event- when called from code there can be triggered at any time 
        GetComponent<CollisionInteractionTrigger>().OnBeginInteraction.Invoke();
        //  trigger an event- when called from code there can be triggered at any time 
        GetComponent<CollisionInteractionTrigger>().OnEndInteraction.Invoke();
        // then Interaction will be triggered if the gameobject with the CollisionInteractionTrigger tagged with TargetTag collides with the gameobject
        //with the CollisionInteractionReceiver gameobject attached to it;
        GetComponent<CollisionInteractionTrigger>().TargetTag = "SomeTag";

        #endregion

        #region Interaction

        // the Interaction script itself is already fully documented and accessing it is done in the same as accessing the previous scripts.

        // the active index value represents the index of a node in your story.
        // when creating a save system and saving you game you must save the active index value and reassign it when loading the game / level

        var someIntValue = 8;
        GetComponent<Interaction>().ActiveIndex = someIntValue;

        // you can generate a fresh chain of events starting from the current ActiveIndex value by calling 
        Interaction.doRefresh(); // static call 
        // OR 
        GetComponent<Interaction>().GenerateActiveDialogueSet();

        // accessing Lovalization specific data like sound effects, voice clips and text is done this way

        var data = GetComponent<Interaction>().ActiveNodeData;
        var theType = GetComponent<Interaction>().ActiveNodeData.GetType();
        if (theType == typeof(DialogueNodeData))
        {
            var dialogue = (DialogueNodeData)data;

            // now we access some specific data associated with the dialogue node

            var theSoundEffect = dialogue.LocalizedSoundEffects[GetComponent<Interaction>().sceneData.LanguageIndex];
            // or if you want to manually access a specific index 
            // var theSoundEffect = dialogue.LocalizedSoundEffects[2];

            var theVoiceClip = dialogue.LocalizedVoiceRecordings[GetComponent<Interaction>().sceneData.LanguageIndex];

            var theText = dialogue.LocalizedText[GetComponent<Interaction>().sceneData.LanguageIndex];

            // you can do the same for all other node types 

            //ActionNodeData
            //RouteNodeData
            //EndNodeData
            //LinkNodeData


        }


        // to access a character of any node you can do this
        var character = GetComponent<Interaction>().ActiveNodeData.CallingNodeData;

        // from there cou have full accessto all the characters data
        Debug.Log(character.CharacterName);

        character.CharacterPersonality.NegativeTraits.Angry = 0.5f;

        character.IsInControl = false;




        #endregion

    }
}