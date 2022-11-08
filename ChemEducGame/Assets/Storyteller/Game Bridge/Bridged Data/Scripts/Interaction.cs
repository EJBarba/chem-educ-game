using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using TMPro;


namespace DaiMangou.BridgedData
{
    /// <summary>
    ///     choice between typed or instant text
    /// </summary>
    public enum InteractionTextDisplayMode
    {
        Instant = 0,

        Typed = 1
    }

    public enum InteractionTypingModes
    {
        Regardless = 0,
        IfIsInControl = 1,
        IfIsNotInControl = 2
    }

    public enum InteractionType
    {
        Linear = 0,
        Non_Linear = 1

    }

   
    /// <summary>
    /// System for linear and non-linear dialogue and interaction
    /// </summary>
    [DisallowMultipleComponent]
    public class Interaction : MonoBehaviour
    {

       public void Reset()
        {
            var interactionTrigger =  gameObject.AddComponent<InteractionTriggerManager>();
            interactionTrigger.InteractionComponent = this;


        }
        private void Awake()
        {
            if (UseMoveNextButton)
            {
                if(MoveNextButton == null)
                {
                    Debug.Log("Please assign a Move Next Button");
                    return;
                }
                MoveNextButton.onClick.AddListener(MoveNext);
            }
        }

        public void OnEnable()
        {
           
            #region add the GenerateActiveDialogueSet function to the RefreshCharacterDialogue delegate function named doRefresh
            doRefresh += GenerateActiveDialogueSet;
            #endregion



        }


        public void OnDisable()
        {
            #region Make sure that only the player can modify the ActiveCharacterDialogueSet and aremoce the GenerateActiveDialogueSet from the delegate function
            doRefresh -= GenerateActiveDialogueSet;
            #endregion
        }

        IEnumerator Start()
        {

            // Create a new List of NodeData fo the ActiveCharacterDialogueSet
            sceneData.ActiveCharacterDialogueSet = new List<NodeData>();

            switch (interactionType)
            {
                case InteractionType.Linear:
                    // we generate the ActiveCharacterDialogueSet immediately REMOVE AND ALLOW USES TO DETERMINE WHEN THIS BEGINS
                  //  doRefresh();
                    break;
                case InteractionType.Non_Linear:


                    break;

                default:

                    break;
            }

            // ececute this process
            while (true)
            {
                Process();

                yield return null;
            }
        }


        /// <summary>
        ///     when two or more character interact , the ActiveCharacterDialogueSetWill be generated .DO NOT EDIT THIS 
        /// </summary>
        public void GenerateActiveDialogueSet( bool ignoreControllincCharacter)
        {

            switch (interactionType)
            {
                case InteractionType.Linear:

                    foreach (var character in sceneData.Characters) Characters.Add(character);
                    break;
                case InteractionType.Non_Linear:

                    // this line replaces the Self value
                    
                    var controllingCharacter = sceneData.Characters.Find(cc => cc.IsInControl == true) as CharacterNodeData;

                    if(controllingCharacter == null)
                    {
                        Debug.Log("<b><color=#009EFF> Please assign a controlling character</color></b>, select the character in the storyteller and canvas and ckick the 'Is Not In Control' button in the inspector . Make sure to select the Interaction system in the hierarchy");
                        return;
                    }
                    // combine the list of nodes in the communicating character node list to characters node list


                    tempNodedataCombination = ignoreControllincCharacter&& CommunicatingCharacters.Count==1? new List<NodeData>():controllingCharacter.NodeDataInMyChain.ToList();
                    // var nodedataCombination = Self.NodeDataInMyChain.ToList();

                    tempNodedataCombination = CommunicatingCharacters.Aggregate(tempNodedataCombination,
                        (current, communicatingCharacter) => current.Concat(communicatingCharacter.NodeDataInMyChain).ToList());


                    // generate the character list and add the communicating characters and this character to it
                    Characters = new List<CharacterNodeData>();
                    foreach (var character in CommunicatingCharacters)
                        Characters.Add(character);

                    // we add itself to the character list last
                    if(!ignoreControllincCharacter || CommunicatingCharacters.Count >1) //THIS CAN BE USED TO MAKE DIALOGUE MORE DYNAMIC
                    Characters.Add(controllingCharacter);
                    //  Characters.Add(Self);


                    break;

                default:

                    break;
            }



            #region here we will populate the  ActiveChracterDalogueSet at runtime, unlike the NodeDataInMyChain list, this list will have all nodes ordered correctly for execution at runtime

            // firstly make ActiveCharacterDialogueSet a new list
            sceneData.ActiveCharacterDialogueSet = new List<NodeData>();

            // add all nodedata to the active character dialogue set if it is not a character node and is not set to pass

            foreach (var data in interactionType == InteractionType.Linear ? sceneData.FullCharacterDialogueSet : tempNodedataCombination)
                if (!data.Runtime_Pass && data.type != typeof(CharacterNodeData))
                    sceneData.ActiveCharacterDialogueSet.Add(data);



            // here we will begin the process of  setting times on all other nodes. Do not break this function.
            foreach (var AllNodesInChainNotSetToPass in Characters.Select(character =>
                character.NodeDataInMyChain.Where(n => !n.Runtime_Pass).OrderBy(st => st.StartTime).ToList()))
            {
                // we will start from the end of the nodeData list and assign start times and delay times in reverse
                for (var i = AllNodesInChainNotSetToPass.Count - 1; i >= 0; i--)
                {
                    if (AllNodesInChainNotSetToPass[i].type == typeof(RouteNodeData))
                    {
                        var route = (RouteNodeData)AllNodesInChainNotSetToPass[i];

                        if (!route.OverrideTime)
                            route.StartTime = route.DataIconnectedTo[route.RuntimeRouteID].StartTime - 0.00001f;
                    }

                    if (AllNodesInChainNotSetToPass[i].type == typeof(LinkNodeData))
                    {
                        var link = (LinkNodeData)AllNodesInChainNotSetToPass[i];

                        if (!link.OverrideTime)
                        {

                            if (link.Loop)
                                link.StartTime =
                                    link.DataConnectedToMe[0].DataConnectedToMe[0].StartTime + 0.00002f;
                            else
                                link.StartTime = link.DataIconnectedTo[0].StartTime - 0.00001f;
                        }

                        // no else here because we push the overriding start time to the start time during the data push
                    }

                    if (AllNodesInChainNotSetToPass[i].type == typeof(EnvironmentNodeData))
                    {
                        var environment = (EnvironmentNodeData)AllNodesInChainNotSetToPass[i];

                        environment.StartTime = environment.DataIconnectedTo[0].StartTime - 0.00001f;
                    }

                    // end nodes are connected to nothing , so we must look at the entire list again and find the node data with the largest start time in the current chain and use that value as the 
                    // an nodes start time
                    if (AllNodesInChainNotSetToPass[i].type != typeof(EndNodeData)) continue;
                    var end = (EndNodeData)AllNodesInChainNotSetToPass[i];

                    // find all nodedata in the characters nodedatainmychain and add them ro a new list if the nodedata is not set to pass
                    if (!end.OverrideTime)
                        end.StartTime = end.DataConnectedToMe.Max(s => s.StartTime) + 0.00001f;
                }
            }

            // and now we order the list by start time ranging from smallest to larget
            sceneData.ActiveCharacterDialogueSet =
                sceneData.ActiveCharacterDialogueSet.OrderBy(t => t.StartTime).ToList();

            // create a list of only the Action and Dialogue Nodes 
            var listOfOnlyActionAndDialogue = sceneData.ActiveCharacterDialogueSet.Where(node =>
                node.type == typeof(ActionNodeData) || node.type == typeof(DialogueNodeData)).ToList();

            #region here we dynamically calculate realtime delay
            for (var i = 0; i < listOfOnlyActionAndDialogue.Count; i++)
            {

                var data = listOfOnlyActionAndDialogue[i];
                if (i == 0)
                    data.RealtimeDelay = data.Delay;


                if (i <= 0) continue;
                var previousData = listOfOnlyActionAndDialogue[i - 1];


                var calculatedDelay = data.StartTime - (previousData.StartTime + previousData.Duration);

                if (calculatedDelay > 0)
                {
                    data.RealtimeDelay = calculatedDelay;
                }
                else
                    calculatedDelay = 0;
            }
            #endregion

            // now set the new Active index based on the cachedUID
            if (!CachedUID.Equals(""))
            {
                ActiveIndex = sceneData.ActiveCharacterDialogueSet.FindIndex(i => i.UID == CachedUID);
            }

            // set ActiveIndex value based on ReturnPointUID
            if (ReturnPointUID.Equals("")) return;
            {
                ActiveIndex = sceneData.ActiveCharacterDialogueSet.FindIndex(i => i.UID == ReturnPointUID);
                ReturnPointUID = "";
            }

            #endregion

            InteractionEnded = false;

           
        }

        public void GenerateActiveDialogueSet()
        {
            GenerateActiveDialogueSet(false);
        }
        /// <summary>
        /// this is run when the player is no longer in the trigger area of another character
        /// </summary>
        public void CleanUp()
        {
            if (CommunicatingCharacters.Count == 0 && Characters.Count > 0)
            {
                Characters = new List<CharacterNodeData>();
                sceneData.ActiveCharacterDialogueSet = new List<NodeData>();
                ActiveIndex = 0; // // this resets the conversation. this must be changed
                CachedUID = "";
                ReturnPointUID = "";
               // communicatingCharacterCount = CommunicatingCharacters.Count;
               // ActiveCharacterDialogueListReset = true;
            }

            #region run if we have communicating characters and a characters

            if (CommunicatingCharacters.Count <= 0 || Characters.Count <= 0) return;
            if (CommunicatingCharacters.Contains(sceneData.ActiveCharacterDialogueSet[ActiveIndex].CallingNodeData))
            {
                CachedUID = sceneData.ActiveCharacterDialogueSet[ActiveIndex].UID;
            }
            else
            {
                sceneData.ActiveCharacterDialogueSet = new List<NodeData>();
                CommunicatingCharacters = new List<CharacterNodeData>();
                ActiveIndex = 0; // this resets the conversation. this must be changed
                CachedUID = "";
                ReturnPointUID = "";
               // ActiveCharacterDialogueListReset = true;
            }

            doRefresh();

            #endregion
        }

        /// <summary>
        ///  Run the core process. You may replace this with the Update function
        /// </summary>
        private void Process()
        {
            #region If we are not set to interact with any other characters the code does nothing, this reduces overhead and esures fast gameplay when not intracting
              if (CommunicatingCharacters.Count == 0 && interactionType == InteractionType.Non_Linear) return;
            #endregion



            #region however if there is no actual dialogue in the database then do nithing
            if (sceneData.ActiveCharacterDialogueSet.Count == 0) return;
            #endregion

            # region if we are currently processing and agrigating data we  will return
            if (BridgeData.ActiveEvents > 0) return;
            #endregion

            #region Set the ActiveNodeData, this is the current node being processed during interactions
            ActiveNodeData = sceneData.ActiveCharacterDialogueSet[ActiveIndex];
            #endregion

          /*  #region trigger the dialogue compilation if we are interacting with another character
            // if the system detects a change in the numbers of comminunicating characters i.e player interacting with another character . the process of quickly structuring a dialogue from the 
            //database will begin
            if (CommunicatingCharacters.Count != communicatingCharacterCount )
            {               
                communicatingCharacterCount = CommunicatingCharacters.Count;
                doRefresh();
            
            }
            #endregion*/

            #region Set target reflected data and set the CachedActiveIndex
            if (TargetReflectedData == null || CachedActiveIndex != ActiveIndex)
            {
                    // and then find the targetreflecteddata that has the same UID as the ActiveNodeData
                    TargetReflectedData = ReflectedDataSet.Find(r => r.UID == ActiveNodeData.UID);      
                CachedActiveIndex = ActiveIndex;
            }
            #endregion

            #region setting character name data
            if (UseNameUI)
            {
                NameText.text = ActiveNodeData.CharacterName;
            }
            #endregion


            #region Depending on which node is being processed during dialogue, we fun a function matching the node type
            if (ActiveNodeData.type == typeof(RouteNodeData))
            {
                
                var route = (RouteNodeData)ActiveNodeData;

                // here we want to jump forward in the list by adding 1 to Active index if the RouteNodeData at ActiveIndex is from a character other than a player
                // you can increase or reduce the flexibility of the system here 
                if (route.IsInControl)
                {

                    if (!route.ConvergenceMode)
                    {
                        if (UseRouteButton)
                        {
                            if (!SetupRouteButtons)
                            {
                                if (UseDialogueTextUI && RoutClearsDIalogue)
                                    DisplayedText.text = "";

                                if (route.DataIconnectedTo.Count > RouteButtons.Count)
                                {
                                    var amountToAdd = route.DataIconnectedTo.Count - RouteButtons.Count;
                                    for (var i = 0; i < amountToAdd; i++)
                                    {
                                        var instancedButton = Instantiate(RouteButton, RouteParent.transform);

                                        var clickListener = instancedButton.gameObject.AddComponent<ClickListener>();
                                        clickListener.interactionComponent = this;
                                        instancedButton.onClick.AddListener(clickListener.SwitchRoute);

                                        instancedButton.GetComponent<ClickListener>().interactionComponent = this;
                                        RouteButtons.Add(instancedButton);
                                    }
                                }

                                foreach (var routeButton in RouteButtons) routeButton.gameObject.SetActive(false);

                                for (var i = 0; i < route.DataIconnectedTo.Count; i++)
                                    RouteButtons[i].gameObject.SetActive(true);

                                for (var i = 0; i < RouteButtons.Count; i++)
                                    // set clicklistener indexInList value here
                                    if (RouteButtons[i].gameObject.activeInHierarchy)
                                    {
                                        RouteButtons[i].GetComponent<ClickListener>().RouteNumber = i;


                                        var textchild = RouteButtons[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>();


                                        textchild.text = TargetReflectedData.RouteSpecificDataset.UseAlternativeRouteTitles
                                            ? TargetReflectedData.RouteSpecificDataset.LanguageSpecificData[sceneData.LanguageIndex].RouteTitles[i]
                                            : route.DataIconnectedTo[i].LocalizedText[sceneData.LanguageIndex];
                                    }

                                SetupRouteButtons = true;
                            }
                        }

                        if (UseMoveNextButton)
                            if (MoveNextButton.gameObject.activeInHierarchy)
                            {
                                MoveNextButton.gameObject.SetActive(false);

                                /*  if (UseMovePreviousButton) // becaue move previou puttons are not so necessary
                                      MovePreviousButton.gameObject.SetActive(false);*/
                            }
                    }
                    else
                    {
                        if (route.DataIconnectedTo.Count > 0)
                        {
                            ReturnPointUID = route.DataIconnectedTo[0].UID;
                            route.RuntimeRouteID = 0;
                            CachedRoute = route;
                        }

                    }
                }




                #region run the General condtions of whichever node is being processed

               
                for( int i = 0; i < GeneralReflectedData.Conditions.Count; i++)
                {
                    var generalCondition = GeneralReflectedData.Conditions[i];
                    if (!generalCondition.Invoked && !generalCondition.Disabled)
                        generalCondition.ProcessConditionData();
                }
                #endregion

                #region Run the node specific condition for the node 

                    for (int i = 0; i < TargetReflectedData.Conditions.Count; i++)
                    {
                    var condition = TargetReflectedData.Conditions[i];
                    if (!condition.Invoked && !condition.Disabled)
                        condition.ProcessConditionData();

                }
                #endregion

                route.ProcessData();

              /*  if (route.ControllingLinkNode != null)
                {
                    var link = (LinkNodeData)route.ControllingLinkNode.DataConnectedToMe[0];
                    if (route.ControllingLinkNode.LoopValue != link.RuntimeIterationCount)
                    {
                        Debug.Log("do move next");
                       // MoveNext();
                    }
                }*/


                if (!route.IsInControl)
                    MoveNext(); // this is only called by route nodes that are not player. By this point conditions have been processes and invoked so this is fine.

                #region instead of calling move next, we do this to ensure that the right node is used ater choosng a route path.
                if (CachedRoute != null)
                {
                    foreach (var button in RouteButtons)
                        button.gameObject.SetActive(false);

                    if (BridgeData.ActiveEvents == 0)
                    {
                        BridgeData.ActiveEvents = -1;
                        doRefresh();
                    }

                    ActiveIndex = sceneData.ActiveCharacterDialogueSet.IndexOf(CachedRoute) + 1;

                    CachedRoute = null;

                    if (UseMoveNextButton)
                        MoveNextButton.gameObject.SetActive(true);

                    /* if (UseMovePreviousButton)
                         MovePreviousButton.gameObject.SetActive(true);*/

                    ResetGeneralConditions();
                    ResetConditions();

                    SetupRouteButtons = false;
                }
                #endregion
            }
            else if (ActiveNodeData.type == typeof(LinkNodeData))
            {
                var link = (LinkNodeData)ActiveNodeData;

                if (!link.Loop)
                {
                    MoveNext();
                }
                else // this will be check in the same loop scene
                {


                    link.TimeLoopDataset[link.loopRoute.RuntimeRouteID].RuntimeIterationCount += 1;


                    if (link.TimeLoopDataset[link.loopRoute.RuntimeRouteID].RuntimeIterationCount == link.TimeLoopDataset[link.loopRoute.RuntimeRouteID].LoopTimes)
                        link.loopRoute.RuntimeRouteID = link.TimeLoopDataset[link.loopRoute.RuntimeRouteID].AutoSwitchValue;
                   
                    ReturnPointUID = link.DataIconnectedTo[0].UID;
                    link.loopRoute.ProcessData();
                    SetupRouteButtons = false;
                    BridgeData.ActiveEvents = 0; // set to 0 so that we can auto refrehs later


                }

                #region run the General condtions of whichever node is being processed

                for (int i = 0; i < GeneralReflectedData.Conditions.Count; i++)
                {
                    var generalCondition = GeneralReflectedData.Conditions[i];
                    if (!generalCondition.Invoked && !generalCondition.Disabled)
                        generalCondition.ProcessConditionData();
                }
                #endregion

                #region Run the node specific condition for the node 
                for (int i = 0; i < TargetReflectedData.Conditions.Count; i++)
                {
                    var condition = TargetReflectedData.Conditions[i];
                    if (!condition.Invoked && !condition.Disabled)
                        condition.ProcessConditionData();
                }
                #endregion

                link.ProcessData();

                #region it is important to reset the ActiveEvents value after the route nodes have completed their processes
                if (BridgeData.ActiveEvents == 0)
                {
                    BridgeData.ActiveEvents = -1;
                    // and now we refesh to restructure the list of data we will use in dialogue and interaction
                    doRefresh();
                }
                #endregion


            }
            else if (ActiveNodeData.type == typeof(EndNodeData))
            {
                var end = (EndNodeData)ActiveNodeData;


                #region run the General condtions of whichever node is being processed

                for (int i = 0; i < GeneralReflectedData.Conditions.Count; i++)
                {
                    var generalCondition = GeneralReflectedData.Conditions[i];
                    if (!generalCondition.Invoked && !generalCondition.Disabled)
                        generalCondition.ProcessConditionData();
                }
                #endregion

                #region Run the node specific condition for the node 
                for (int i = 0; i < TargetReflectedData.Conditions.Count; i++)
                {
                    var condition = TargetReflectedData.Conditions[i];
                    if (!condition.Invoked && !condition.Disabled)
                        condition.ProcessConditionData();
                }
                #endregion

                end.ProcessData();

                MoveNext();

            }
            else if (ActiveNodeData.type == typeof(EnvironmentNodeData))
            {
                var environment = (EnvironmentNodeData)ActiveNodeData;

                #region run the General condtions of whichever node is being processed

                for (int i = 0; i < GeneralReflectedData.Conditions.Count; i++)
                {
                    var generalCondition = GeneralReflectedData.Conditions[i];
                    if (!generalCondition.Invoked && !generalCondition.Disabled)
                        generalCondition.ProcessConditionData();
                }
                #endregion

                #region Run the node specific condition for the node 
                for (int i = 0; i < TargetReflectedData.Conditions.Count; i++)
                {
                    var condition = TargetReflectedData.Conditions[i];
                    if (!condition.Invoked && !condition.Disabled)
                        condition.ProcessConditionData();
                }
                #endregion

                environment.ProcessData();

                MoveNext();
            }
            else if (ActiveNodeData.type == typeof(ActionNodeData))
            {

                var action = (ActionNodeData)ActiveNodeData;

                #region we also set the test for actions , however this is not necessary in game use by everyone so you may use a general condition to MoveNext()
                if (UseDialogueTextUI && UsesText && SetActionText)
                {
                    DisplayedText.maxVisibleCharacters = action.LocalizedText[sceneData.LanguageIndex].Length;
                    DisplayedText.text = action.LocalizedText[sceneData.LanguageIndex];
                }
                #endregion

                #region Play sound effect if necessary

                if (action.LocalizedSoundEffects[sceneData.LanguageIndex] != null && UsesSoundffects)
                    if (!TargetReflectedData.ActionSpecificData.OverrideUseSoundEffect)
                        if (!TargetReflectedData.ActionSpecificData.SoundEffectWasPlayed)
                            if (AutoStartSoundEffectClip)
                            {
                                var inxedOfCallingNode = sceneData.Characters.IndexOf(ActiveNodeData.CallingNodeData);
                                SoundEffectAudioSources[inxedOfCallingNode].clip = action.LocalizedSoundEffects[sceneData.LanguageIndex];
                                 SoundEffectAudioSources[inxedOfCallingNode].PlayOneShot( SoundEffectAudioSources[inxedOfCallingNode].clip);
                                TargetReflectedData.ActionSpecificData.SoundEffectWasPlayed = true;
                            }
                #endregion

                #region run the General condtions of whichever node is being processed

                for (int i = 0; i < GeneralReflectedData.Conditions.Count; i++)
                {
                    var generalCondition = GeneralReflectedData.Conditions[i];
                    if (!generalCondition.Invoked && !generalCondition.Disabled)
                        generalCondition.ProcessConditionData();
                }
                #endregion

                #region Run the node specific condition for the node 
                for (int i = 0; i < TargetReflectedData.Conditions.Count; i++)
                {
                    var condition = TargetReflectedData.Conditions[i];
                    if (!condition.Invoked && !condition.Disabled)
                        condition.ProcessConditionData();
                }
                #endregion

                action.ProcessData();

            }
            else
            {

                var dialogue = (DialogueNodeData)ActiveNodeData;

                // we push the text into a var because the text can be manipulated at runtime 
                if (!ActiveUID.Equals(ActiveNodeData.UID))
                {
                  
                    TempText = dialogue.LocalizedText[sceneData.LanguageIndex]; //we can now edit this text without destroying the text in the asset
                    ActiveUID = ActiveNodeData.UID;

                    #region Keyword filder processing 
                    if (UseKeywordFilters)
                        foreach (var keywordFilter in KeywordFilters.Where(keywordFilter => !keywordFilter.Disabled))
                        {
                            if (TempText.Contains(keywordFilter.KeyWord))
                            {

                                #region both static keyword and replacement

                                if (keywordFilter.StaticKeywordMethod &&
                                    keywordFilter.StaticReplacementStringMethod)
                                {

                                    TempText = TempText.Replace(keywordFilter.KeyWord, keywordFilter.ReplacementString);

                                }

                                #endregion

                                #region static keyword and dynaic replacement string

                                if (keywordFilter.StaticKeywordMethod &&
                                    !keywordFilter.StaticReplacementStringMethod)
                                {
                                    var compA = keywordFilter.DynamicReplacementString.Components[
                                        keywordFilter.DynamicReplacementString.ComponentIndex];

                                    var replacementStringDelegate = (DelA)Delegate.CreateDelegate(typeof(DelA),
                                        compA,
                                        keywordFilter.DynamicReplacementString
                                            .serializedMethods[keywordFilter.DynamicReplacementString.MethodIndex]
                                            .methodName);
                                    TempText = TempText.Replace(keywordFilter.KeyWord, replacementStringDelegate());
                                }

                                #endregion

                            }

                            if (keywordFilter.DynamicKeyword.TargetGameObject == null) continue;
                            {
                                #region both non static keyword and replacement

                                if (!keywordFilter.StaticKeywordMethod &&
                                    !keywordFilter.StaticReplacementStringMethod)
                                {
                                    var compA = keywordFilter.DynamicReplacementString.Components[
                                        keywordFilter.DynamicReplacementString.ComponentIndex];
                                    var replacementStringDelegate = (DelA)Delegate.CreateDelegate(typeof(DelA),
                                        compA,
                                        keywordFilter.DynamicReplacementString
                                            .serializedMethods[keywordFilter.DynamicReplacementString.MethodIndex]
                                            .methodName);

                                    var compB = keywordFilter.DynamicKeyword.Components[
                                        keywordFilter.DynamicKeyword.ComponentIndex];
                                    var keywordDelegate = (DelB)Delegate.CreateDelegate(typeof(DelB), compB,
                                        keywordFilter.DynamicKeyword
                                            .serializedMethods[keywordFilter.DynamicKeyword.MethodIndex]
                                            .methodName);


                                    TempText = TempText.Replace(keywordDelegate(), replacementStringDelegate());
                                }

                                #endregion

                                #region dynamic keyword and static replacement string

                                if (keywordFilter.StaticKeywordMethod ||
                                    !keywordFilter.StaticReplacementStringMethod) continue;
                                {
                                    var compB = keywordFilter.DynamicKeyword.Components[
                                        keywordFilter.DynamicKeyword.ComponentIndex];
                                    var keywordDelegate = (DelB)Delegate.CreateDelegate(typeof(DelB), compB,
                                        keywordFilter.DynamicKeyword
                                            .serializedMethods[keywordFilter.DynamicKeyword.MethodIndex]
                                            .methodName);

                                    TempText = TempText.Replace(keywordDelegate(), keywordFilter.ReplacementString);
                                }

                                #endregion
                            }
                        }
                    #endregion

                }

                if (UseDialogueTextUI && UsesText)
                {


                    #region text display method processing 

                    if (textDisplayMode == InteractionTextDisplayMode.Typed)
                    {
                        switch (typingMode)
                        {
                            case InteractionTypingModes.Regardless:
                                if (!typing && LastActiveNodeData != ActiveNodeData)
                                {
                                    modifiedTempText = Regex.Replace(TempText, "<.*?>", string.Empty);
                                    DisplayedText.maxVisibleCharacters = 0;
                                    DisplayedText.text = TempText;
                                    TypingCoroutine = StartCoroutine(DoType());

                                }
                                break;
                            case InteractionTypingModes.IfIsInControl:
                                if (ActiveNodeData.IsInControl)
                                {
                                    if (!typing && LastActiveNodeData != ActiveNodeData)
                                    {
                                        modifiedTempText = Regex.Replace(TempText, "<.*?>", string.Empty);
                                        DisplayedText.maxVisibleCharacters = 0;
                                        DisplayedText.text = TempText;
                                        TypingCoroutine = StartCoroutine(DoType());

                                    }
                                }
                                else
                                {
                                    DisplayedText.maxVisibleCharacters = TempText.Length;
                                    DisplayedText.text = TempText;
                                }
                                break;
                            case InteractionTypingModes.IfIsNotInControl:
                                if (!ActiveNodeData.IsInControl)
                                {
                                    if (!typing && LastActiveNodeData != ActiveNodeData)
                                    {
                                        modifiedTempText = Regex.Replace(TempText, "<.*?>", string.Empty);
                                        DisplayedText.maxVisibleCharacters = 0;
                                        DisplayedText.text = TempText;
                                        TypingCoroutine = StartCoroutine(DoType());

                                    }
                                }
                                else
                                {
                                    DisplayedText.maxVisibleCharacters = TempText.Length;
                                    DisplayedText.text = TempText;
                                }
                                break;
                            default: break;

                        }


                    }
                    else
                    {
                        DisplayedText.maxVisibleCharacters = TempText.Length;
                        DisplayedText.text = TempText;
                    }
                    #endregion




                }

                #region creating and populating dialogue history, this is a history of our dialogue and interactions, you may edit this to include text from action node processes in the aboe if statement for action node processes
                if (DialogueHistory.Count != 0)
                {
                    if (DialogueHistory.Last().DialogueNodeData_ != dialogue)
                    {
                        DialogueHistory.Add(new History(dialogue.CallingNodeData, dialogue, TempText));
                    }
                }
                else
                {
                    DialogueHistory.Add(new History(dialogue.CallingNodeData, dialogue, TempText));
                }
                #endregion

                #region process voice over and playback

                if (dialogue.LocalizedVoiceRecordings[sceneData.LanguageIndex] != null && UsesVoiceOver)
                    if (!TargetReflectedData.DialogueSpecificData.OverrideUseVoiceover)
                        if (!TargetReflectedData.DialogueSpecificData.VoiceClipWasPlayed)
                        {


                            if (AutoStartVoiceClip)
                            {
                                var inxedOfCallingNode = sceneData.Characters.IndexOf(ActiveNodeData.CallingNodeData);
                                VoiceAudioSources[inxedOfCallingNode].clip = dialogue.LocalizedVoiceRecordings[sceneData.LanguageIndex];
                                VoiceAudioSources[inxedOfCallingNode].PlayOneShot(VoiceAudioSources[inxedOfCallingNode].clip);
                                TargetReflectedData.DialogueSpecificData.VoiceClipWasPlayed = true;
                            }
                        }

                #endregion

                #region Play sound effect if necessary
                if (dialogue.LocalizedSoundEffects[sceneData.LanguageIndex] != null && UsesSoundffects)
                    if (!TargetReflectedData.DialogueSpecificData.OverrideUseSoundEffect)
                        if (!TargetReflectedData.DialogueSpecificData.SoundEffectWasPlayed)
                        {

                            if (AutoStartSoundEffectClip)
                            {
                                var inxedOfCallingNode = sceneData.Characters.IndexOf(ActiveNodeData.CallingNodeData);
                                SoundEffectAudioSources[inxedOfCallingNode].clip = dialogue.LocalizedSoundEffects[sceneData.LanguageIndex];
                                 SoundEffectAudioSources[inxedOfCallingNode].PlayOneShot( SoundEffectAudioSources[inxedOfCallingNode].clip);
                                TargetReflectedData.DialogueSpecificData.SoundEffectWasPlayed = true;
                            }

                        }
                #endregion

                #region run the General condtions of whichever node is being processed

                for (int i = 0; i < GeneralReflectedData.Conditions.Count; i++)
                {
                    var generalCondition = GeneralReflectedData.Conditions[i];
                    if (!generalCondition.Invoked && !generalCondition.Disabled)
                        generalCondition.ProcessConditionData();
                }
                #endregion

                #region Run the node specific condition for the node 
                for (int i = 0; i < TargetReflectedData.Conditions.Count; i++)
                {
                    var condition = TargetReflectedData.Conditions[i];
                    if (!condition.Invoked && !condition.Disabled)
                        condition.ProcessConditionData();
                }
                #endregion

                dialogue.ProcessData();
            }
            #endregion

        }

        /// <summary>
        ///  Play a voice clip of a dialogue node
        /// </summary>
        public void PlayVoiceClipNow()
        {

            if (ActiveNodeData.type != typeof(DialogueNodeData)) return;

            var inxedOfCallingNode = sceneData.Characters.IndexOf(ActiveNodeData.CallingNodeData);
            VoiceAudioSources[inxedOfCallingNode].clip = ((DialogueNodeData)ActiveNodeData).LocalizedVoiceRecordings[sceneData.LanguageIndex];


            if (VoiceAudioSources[inxedOfCallingNode].clip == null || !UsesVoiceOver) return;
            if (TargetReflectedData.DialogueSpecificData.OverrideUseVoiceover) return;
            if (TargetReflectedData.DialogueSpecificData.VoiceClipWasPlayed) return;
            VoiceAudioSources[inxedOfCallingNode].PlayOneShot(VoiceAudioSources[inxedOfCallingNode].clip);
            TargetReflectedData.DialogueSpecificData.VoiceClipWasPlayed = true;
        }

        /// <summary>
        ///  Play a soundeffect of a Dialogue node or action node
        /// </summary>
        public void PlaySoundEffectNow()
        {


            if (ActiveNodeData.type != typeof(ActionNodeData) &&
                ActiveNodeData.type != typeof(DialogueNodeData)) return;

            var inxedOfCallingNode = sceneData.Characters.IndexOf(ActiveNodeData.CallingNodeData);

            SoundEffectAudioSources[inxedOfCallingNode].clip = ActiveNodeData.type == typeof(DialogueNodeData)
                ? ((DialogueNodeData)ActiveNodeData).LocalizedSoundEffects[sceneData.LanguageIndex]
                : ((ActionNodeData)ActiveNodeData).LocalizedSoundEffects[sceneData.LanguageIndex];

            if ( SoundEffectAudioSources[inxedOfCallingNode].clip == null || !UsesSoundffects) return;
            if (TargetReflectedData.DialogueSpecificData.OverrideUseSoundEffect) return;
            if (TargetReflectedData.DialogueSpecificData.SoundEffectWasPlayed) return;

             SoundEffectAudioSources[inxedOfCallingNode].PlayOneShot( SoundEffectAudioSources[inxedOfCallingNode].clip);

            if (ActiveNodeData.type == typeof(DialogueNodeData))
                TargetReflectedData.DialogueSpecificData.SoundEffectWasPlayed = true;
            else
                TargetReflectedData.ActionSpecificData.SoundEffectWasPlayed = true;

        }

        /// <summary>
        ///  Trigger typing event
        /// </summary>
        /// <returns></returns>
        IEnumerator DoType()
        {

            if (TypingAudioSource.clip == null)
                TypingAudioSource.clip = TypingAudioCip;

            typing = true;
            int typedCounter = 0;

            while (typedCounter < modifiedTempText.Length)
            {

                if (modifiedTempText[typedCounter] == ' ')
                {
                    typedCounter++;
                    DisplayedText.maxVisibleCharacters++;
                }

                typedCounter++;

                DisplayedText.maxVisibleCharacters++;


                if (TypingAudioSource)
                {
                    TypingAudioSource.Play();
                    RandomiseVolume();
                }
                yield return new WaitForSeconds(TypingSpeed);



            }

            LastActiveNodeData = ActiveNodeData;
            StopCoroutine(TypingCoroutine);
            typing = false;
            TempText = "";

            yield return null;




        }

        /// <summary>
        /// manually trigger the stop and reset of typing
        /// </summary>
        public void ResetTyping()
        {
            StopCoroutine(TypingCoroutine);
            TempText = ActiveNodeData.LocalizedText[sceneData.LanguageIndex];
            LastActiveNodeData = null;
            typing = false;
        }

        /// <summary>
        /// create variations in volume of typing sound
        /// </summary>
        private void RandomiseVolume()
        {
            TypingAudioSource.volume = UnityEngine.Random.Range(1 - VolumeVariation, VolumeVariation + 1);
        }

        /// <summary>
        ///     move to next event in chain of events
        /// </summary>
        public void MoveNext()
        {

            if (InteractionEnded)
            {
                if (ActiveIndex + 1 == sceneData.ActiveCharacterDialogueSet.Count)
                    return;
                else
                    InteractionEnded = false;

            }
            // we also deactivate the buttons BEFORE 
            if(UseRouteButton)
            foreach (var button in RouteButtons)
                button.gameObject.SetActive(false);

            // set back to false
            TargetReflectedData.DialogueSpecificData.VoiceClipWasPlayed = false;
            TargetReflectedData.DialogueSpecificData.SoundEffectWasPlayed = false;
            TargetReflectedData.ActionSpecificData.SoundEffectWasPlayed = false;

            var inxedOfCallingNode = sceneData.Characters.IndexOf(ActiveNodeData.CallingNodeData);

             VoiceAudioSources[inxedOfCallingNode].Stop();
             SoundEffectAudioSources[inxedOfCallingNode].Stop();
            LastActiveNodeData = ActiveNodeData;

            if (ActiveIndex + 1 < sceneData.ActiveCharacterDialogueSet.Count)
                ActiveIndex += 1;

            else InteractionEnded = true;

            if (typing)
                StopCoroutine(TypingCoroutine);

            typing = false;
            TempText = "";
            DisplayedText.maxVisibleCharacters = DisplayedText.text.Length;
            //  DisplayedText.text = "";






            ResetGeneralConditions();
            ResetConditions();

        }

        /// <summary>
        ///     move to previous evnt in chaing of events
        /// </summary>

        [Obsolete("Move previous have been depreciated. Instead, use a time loop to move to an earlier point in your interactions")]
        public void MovePrevious() { }
        /* public void MovePrevious()
         {
             InteractionEnded = false;
             //for safety when moving next while typing text
             foreach (var button in RouteButtons)
                 button.gameObject.SetActive(false);

             // set back to false
             TargetReflectedData.DialogueSpecificData.VoiceClipWasPlayed = false;
             TargetReflectedData.DialogueSpecificData.SoundEffectWasPlayed = false;
             TargetReflectedData.ActionSpecificData.SoundEffectWasPlayed = false;
             VoiceAudioSources[inxedOfCallingNode].Stop();
              SoundEffectAudioSources[inxedOfCallingNode].Stop();
             LastActiveNodeData = ActiveNodeData;

             if (ActiveIndex > 0)
                 ActiveIndex -= 1;

             if (typing)
                 StopCoroutine(TypingCoroutine);

                 typing = false;
                 TempText = "";
             DisplayedText.maxVisibleCharacters = DisplayedText.text.Length;
             //  DisplayedText.text = "";


             ResetGeneralConditions();
             ResetConditions();
         }*/

public void SetTargetRouteIndex( int targetRouteIndex)
        {
            TargetRoute = targetRouteIndex;
        }


        /// <summary>
        ///     This requires the public TargetRoute value to be set
        /// </summary>
        public void ContinueOnRoute()
        {

            if (sceneData.ActiveCharacterDialogueSet[ActiveIndex].type != typeof(RouteNodeData)) return;

            var route = (RouteNodeData)sceneData.ActiveCharacterDialogueSet[ActiveIndex];
            route.RuntimeRouteID = TargetRoute;
            CachedRoute = route;

            ResetGeneralConditions();
            ResetConditions();

        }

        /// <summary>
        /// When processing a route node you can choose which route to go to by setting the routeID whcih is the index of the path
        /// </summary>
        /// <param name="routeID"></param>
        public void GoToRoute(int routeID)
        {
            if (sceneData.ActiveCharacterDialogueSet[ActiveIndex].type != typeof(RouteNodeData)) return;

            var route = (RouteNodeData)sceneData.ActiveCharacterDialogueSet[ActiveIndex];
            route.RuntimeRouteID = routeID;
            CachedRoute = route;

            ResetGeneralConditions();
            ResetConditions();

        }

        /// <summary>
        /// Use a condition to turn on another condition by index value
        /// </summary>
        /// <param name="conditionIndex"></param>
        public void TurnOnCondition(int conditionIndex)
        {
            TargetReflectedData.Conditions[conditionIndex].Disabled = false;
        }

        /// <summary>
        ///  Use a condition to turn on another condition by index value
        /// </summary>
        /// <param name="conditionIndex"></param>     
        public void TurnOnGeneralCondition(int conditionIndex)
        {
            GeneralReflectedData.Conditions[conditionIndex].Disabled = false;
        }

        /// <summary>
        ///  Use a condition to turn off another condition by index value
        /// </summary>
        /// <param name="conditionIndex"></param>      
        public void TurnOffCondition(int conditionIndex)
        {
            TargetReflectedData.Conditions[conditionIndex].Disabled = true;
        }

        /// <summary>
        /// Use a condition to turn off another condition by index value
        /// </summary>
        /// <param name="conditionIndex"></param>
        public void TurnOffGeneralCondition(int conditionIndex)
        {
            GeneralReflectedData.Conditions[conditionIndex].Disabled = true;
        }

        /// <summary>
        /// Calcel a currently processing condition
        /// </summary>
        /// <param name="conditionIndex"></param>
        public void CancelCondition(int conditionIndex)
        {
            TargetReflectedData.Conditions[conditionIndex]
                .StopCoroutine(TargetReflectedData.Conditions[conditionIndex].coroutine);
        }

        /// <summary>
        /// Calcel a currently processing condition
        /// </summary>
        /// <param name="conditionIndex"></param>
        public void CancelGeneralCondition(int conditionIndex)
        {
            GeneralReflectedData.Conditions[conditionIndex]
                .StopCoroutine(GeneralReflectedData.Conditions[conditionIndex].coroutine);
        }

        /// <summary>
        /// Used to reset a condition so that it may run again
        /// </summary>
        public void ResetConditions()
        {
            // make sure to set invoked back to false to false so that the events can be invoked again if we move back to it
            // TargetReflectedData.Conditions.All(inv => inv.Invoked = false);
            for (int i = 0; i < TargetReflectedData.Conditions.Count; i++)
            {
                var condition = TargetReflectedData.Conditions[i];
                condition.RanConditionCheck = condition.Invoked = false;
            }
            
        }

        /// <summary>
        /// Used to reset a condition so that it may run again
        /// </summary>
        public void ResetGeneralConditions()
        {
         //   yield return new WaitForEndOfFrame();

            for (int i = 0; i < GeneralReflectedData.Conditions.Count; i++)
            {
                var generalCondition = GeneralReflectedData.Conditions[i];
                generalCondition.RanConditionCheck = generalCondition.Invoked = false;
            }
        }

        public void SetControllingCharacter(CharacterNodeData character)
        {
            character.IsInControl = true;
            foreach (var dataset in character.NodeDataInMyChain)
                dataset.IsInControl = character.IsInControl;
        }

        public void RemoveControllingCharacter(CharacterNodeData character)
        {
            character.IsInControl = false;
            foreach (var dataset in character.NodeDataInMyChain)
                dataset.IsInControl = character.IsInControl;
        }

        /// <summary>
        /// return the dialogue string at the end of the history list - indexPositionFromEnd , as string 
        /// </summary>
        /// <returns></returns>
        public string GetHistoryDialogueAtIndex(int indexPositionFromEnd = 0)
        {
            if (DialogueHistory.Count == 0) return "";
            return DialogueHistory[(DialogueHistory.Count - 1) - indexPositionFromEnd].DialogueText;

        }

        public void SetLanguage(int languageIndex)
        {                
                sceneData.LanguageIndex = languageIndex;
        }

        /// <summary>
        /// Check if the currently processed NodeData is an Action Node
        /// </summary>
        /// <returns></returns>
        public bool CurrentNodeIsAction()
        {
            return ActiveNodeData.type == typeof(ActionNodeData);
        }

        /// <summary>
        ///  Check if the currently processed NodeData is an Dialogue Node
        /// </summary>
        /// <returns></returns>
        public bool CurrentNodeIsDialogue()
        {
            return ActiveNodeData.type == typeof(DialogueNodeData);
        }

        /// <summary>
        ///  Check if the currently processed NodeData is an Route Node
        /// </summary>
        /// <returns></returns>
        public bool CurrentNodeIsRoute()
        {
            return ActiveNodeData.type == typeof(RouteNodeData);
        }

        /// <summary>
        ///  Check if the currently processed NodeData is an Link Node
        /// </summary>
        /// <returns></returns>
        public bool CurrentNodeIsLink()
        {
            return ActiveNodeData.type == typeof(LinkNodeData);
        }

        /// <summary>
        ///  Check if the currently processed NodeData is an End Node
        /// </summary>
        /// <returns></returns>
        public bool CurrentNodeIsEnd()
        {
            return ActiveNodeData.type == typeof(EndNodeData);
        }

        /// <summary>
        ///  Check if the currently processed NodeData is an Environment Node
        /// </summary>
        /// <returns></returns>
        public bool CurrentNodeIsEnvironment()
        {
            return ActiveNodeData.type == typeof(EnvironmentNodeData);
        }

        /// <summary>
        ///  Check if the currently processed NodeData uses time , is a Action or Dialogue Node
        /// </summary>
        /// <returns></returns>
        public bool CurrentNodeUsesTime()
        {
            return ActiveNodeData.useTime;
        }

        /// <summary>
        /// checks if the current Active Node Data belongs to the player
        /// </summary>
        /// <returns></returns>
        public bool IsInControl()
        {
            return ActiveNodeData.IsInControl;
        }



        #region variables

        /// <summary>
        /// </summary>
        [HideInInspector]
        public SceneData sceneData;

        /// <summary>
        /// </summary>
        [HideInInspector]
        public delegate void RefreshCharacterDialogue();

        /// <summary>
        ///     used to re-generate the ActiveCharacterDialogueSet
        /// </summary>
        [HideInInspector] public static RefreshCharacterDialogue doRefresh;

        /// <summary>
        ///     static int flag used to check how many charactrs are speaking at once, if the number inceases the aUI for dialogue
        ///     will be added
        /// </summary>
        [HideInInspector] public static int ActiveDialogues = 0;

        /// <summary>
        ///     this is the value of the data in ActiveNodeData that is currently being processed
        /// </summary>
        [HideInInspector]
        public int ActiveIndex;

        /// <summary>
        /// this value is use to compare against the Active index value to check for change
        /// </summary>
        private int CachedActiveIndex = -1;

        // a special set of data with an ID value matching that of each NodeData in the FullCharacterDataSet
        [HideInInspector]
        public List<CharacterNodeData> Characters = new List<CharacterNodeData>();

        /// <summary>
        /// This is a list of the reflected data of this character
        /// </summary>
        [HideInInspector]
        public List<ReflectedData> ReflectedDataSet = new List<ReflectedData>();

        /// <summary>
        /// in Editor this is used as temporart storeage for reflected data, In game this ise used as temporart storage for merged reflected data between interacting characters 
        /// </summary>
        [HideInInspector, NonSerialized] 
        public List<ReflectedData> TempReflectedDataSet = new List<ReflectedData>();

        /// <summary>
        ///     Gameobject that holds all the reflected data as child objects (do not delete)
        /// </summary>
        [HideInInspector] 
        public GameObject ReflectedDataParent;

        /// <summary>
        ///  The currently used reflected data
        /// </summary>
        [HideInInspector]
        public ReflectedData TargetReflectedData;

        /// <summary>
        /// The currentlu used General reflected data
        /// </summary>
        [HideInInspector] 
        public ReflectedData GeneralReflectedData;

        /// <summary>
        /// The parent object of the Reflected data in the scene
        /// </summary>
        [HideInInspector]
        public GameObject GeneralReflectedDataParent;

        /// <summary>
        /// This is used by the Editor only as a data reference
        /// </summary>
        [HideInInspector] 
        public ReflectedData TempGeneralReflectedDataSet;

        /// <summary>
        /// This is set only during route node processes when routes are switched
        /// </summary>
        [HideInInspector] [NonSerialized]
        public RouteNodeData CachedRoute;

        /// <summary>
        /// This is literally the node data which is currently being processed by the system
        /// </summary>
        [NonSerialized]
        public NodeData ActiveNodeData;
        /// <summary>
        /// The previous Active Node data 
        /// </summary>
        [NonSerialized]
        private NodeData LastActiveNodeData;

        /// <summary>
        /// The name text UI
        /// </summary>
        [HideInInspector] 
        public TextMeshProUGUI NameText;

        /// <summary>
        /// the Dialogue text UI
        /// </summary>
        [HideInInspector] public TextMeshProUGUI DisplayedText;

        /// <summary>
        /// the UI button for moving to the next dialogue or interaction
        /// </summary>
        [HideInInspector]
        public Button MoveNextButton;

        /// <summary>
        /// the Prefab button which we wish to use as our route buttons 
        /// </summary>
        [HideInInspector] 
        public Button RouteButton;

        /// <summary>
        /// A list of our instanced route button generated at runtime
        /// </summary>
        [HideInInspector] 
        public List<Button> RouteButtons = new List<Button>();

        /// <summary>
        /// The parent gameobject under which the route buttons will be instanced
        /// </summary>
        [HideInInspector] 
        public GameObject RouteParent;

        /// <summary>
        /// Check if we will be using a name Ui display
        /// </summary>
        [HideInInspector] 
        public bool UseNameUI = true;

        /// <summary>
        /// Check if we will be usung a dialogue display UI
        /// </summary>
        [HideInInspector] 
        public bool UseDialogueTextUI = true;

        /// <summary>
        /// check if we will be using a move next button
        /// </summary>
        [HideInInspector]
        public bool UseMoveNextButton = true;

        /// <summary>
        /// Check if we will be using a move previous button
        /// </summary>
       // [HideInInspector] public bool UseMovePreviousButton = true;

        /// <summary>
        /// Check if we will be using route buttons
        /// </summary>
        [HideInInspector] 
        public bool UseRouteButton = true;
        /// <summary>
        /// allows the displaying of text from action nodes in the dialogue UI, this is not usually necessay and typically serves as a guide 
        /// </summary>
        [HideInInspector] 
        public bool SetActionText = true;

        /// <summary>
        /// this is on by default and can be turned occ the prevent the display of route options from clearing the dialogue text if text esists.
        /// </summary>
        [HideInInspector] 
        public bool RoutClearsDIalogue = true;

        /// <summary>
        /// What will the ext display move be , Typed text or instantly displayed text
        /// </summary>
        [HideInInspector] 
        public InteractionTextDisplayMode textDisplayMode = InteractionTextDisplayMode.Instant;


        /// <summary>
        /// You can specify what type of character can actually use typed text
        /// </summary>
        [HideInInspector] 
        public InteractionTypingModes typingMode = InteractionTypingModes.Regardless;


        #region typing stuff

        /// <summary>
        /// The eyping speed
        /// </summary>
        [HideInInspector]
        public float TypingSpeed = 0.05f;
        /// <summary>
        /// used internally durin the typing process
        /// </summary>
        private string TempText;
        /// <summary>
        /// used internally durin the typing process
        /// </summary>
        private string modifiedTempText;
        /// <summary>
        /// used internally durin the typing process
        /// </summary>
        private Coroutine TypingCoroutine;
        /// <summary>
        /// used internally durin the typing process
        /// </summary>
        [HideInInspector] 
        public float Timer;
        /// <summary>
        /// used internally durin the typing process (not setup)
        /// </summary>
        [HideInInspector] 
        public float startDelay = 0.5f;
        /// <summary>
        /// used internally durin the typing process
        /// </summary>
        [HideInInspector]
        public float VolumeVariation = 0.1f;
        /// <summary>
        /// used internally durin the typing process to set a Audio for typing 
        /// </summary>
        [HideInInspector] 
        public AudioSource TypingAudioSource;
        /// <summary>
        /// used internally durin the typing process
        /// </summary>
        private bool typing;
        /// <summary>
        /// 
        /// </summary>
        public bool IsTyping
        {
            get
            {
                return typing;
            }
        }

        /// <summary>
        /// used internally durin the typing process to set a Audio for typing 
        /// </summary>
        [HideInInspector] 
        public AudioClip TypingAudioCip;

        #endregion
        /// <summary>
        /// The sudio source for voice clips to be played from This is already created and set
        /// </summary>
      //  [HideInInspector] 
      //  public AudioSource VoiceAudioSource;

        /// <summary>
        /// The sudio source for sound effect clips to be played from This is already created and set
        /// </summary>
      //  [HideInInspector]
      //  public AudioSource SoundEffectAudioSource;

        /// <summary>
        /// A list of characters which are interacting 
        /// </summary>
        [HideInInspector] 
        public List<CharacterNodeData> CommunicatingCharacters = new List<CharacterNodeData>();


        /// <summary>
        /// The number of communicating characters
        /// </summary>
      //  private int communicatingCharacterCount;

        /// <summary>
        /// used to ensure that if a character exits an interaction , the correct active node is preserved after refreshing the ActiveNodeData list
        /// </summary>
        [HideInInspector] 
        public string CachedUID = "";

        /// <summary>
        /// Bool to check if we are using Text
        /// </summary>
        [HideInInspector] 
        public bool UsesText = true;

        /// <summary>
        /// Bool to check if we are using storyboard images
        /// </summary>
        [HideInInspector] 
        public bool UsesStoryboardImages;

        /// <summary>
        /// Bool to check if we are using voiceover 
        /// </summary>
        [HideInInspector] 
        public bool UsesVoiceOver = true;

        /// <summary>
        /// Bool to check if we are using auto start for voiceover clips
        /// </summary>
        [HideInInspector] 
        public bool AutoStartVoiceClip;

        /// <summary>
        /// Bool to check if we are using sound effects
        /// </summary>
        [HideInInspector] 
        public bool UsesSoundffects = true;

        /// <summary>
        /// Bool to check if we are using auto start for sound effects
        /// </summary>
        [HideInInspector]
        public bool AutoStartSoundEffectClip;

        /// <summary>
        /// A list of our Keyword filters
        /// </summary>
        [HideInInspector] 
        public List<KeywordFilter> KeywordFilters = new List<KeywordFilter>();

        /// <summary>
        /// Check if we are usng keyword filters
        /// </summary>
        [HideInInspector]
        public bool UseKeywordFilters;

        /// <summary>
        /// Set the numbers of keyword filters Editor only
        /// </summary>
        [HideInInspector] 
        public int FilterCount;

        /// <summary>
        /// Editor only
        /// </summary>
        [HideInInspector]
        public bool ShowGeneralSettings;

        /// <summary>
        /// Editor only
        /// </summary>
        [HideInInspector] 
        public bool ShowKeywordFilterFouldout;

        /// <summary>
        /// Editor only
        /// </summary>
        [HideInInspector]
        public bool ShowNodeSpecificSettings = true;

        /// <summary>
        /// Check if we have setup the route node buttons
        /// </summary>
        [HideInInspector]
        public bool SetupRouteButtons;


        /// <summary>
        /// Editor only
        /// </summary>
        [HideInInspector]
        public bool ShowUIAndTextSettings;

        /// <summary>
        /// Editor only
        /// </summary>
        [HideInInspector]
        public bool ShowTextDisplayModeSettings;

        /// <summary>
        /// Editor only
        /// </summary>
        [HideInInspector] 
        public bool ShowGeneralConditionsSettings = true;

        /// <summary>
        /// Editor only
        /// </summary>
        [HideInInspector]
        public bool ShowNodeSpecificConditionSettings = true;

        /// <summary>
        /// used by the Link node in particular to return dialogue and interactions to the past
        /// </summary>
        [NonSerialized] 
        public string ReturnPointUID = "";

        /// <summary>
        ///     delegate that we will use to create a delegate method
        /// </summary>
        /// <returns></returns>
        private delegate string DelA();

        /// <summary>
        ///   delegate that we will use to create a delegate method
        /// </summary>
        /// <returns></returns>
        private delegate string DelB();


        /// <summary>
        /// A old method of setting Text colour 
        /// </summary>
     //   [HideInInspector] public Color TextColour = Color.white;

        /// <summary>
        /// the actuail UID of the ActiveNodeData
        /// </summary>
        private string ActiveUID = "";

        /// <summary>
        /// A option to Auto start all conditions by default
        /// </summary>
        [HideInInspector]
        public bool AutoStartAllConditionsByDefault;

        /// <summary>
        /// Editor Only
        /// </summary>
        [HideInInspector]
        public string UpdateUID = "";

        /// <summary>
        /// A history list of all your past interactions
        /// </summary>
        [HideInInspector]
        public List<History> DialogueHistory = new List<History>();

        /// <summary>
        /// Checks if your interaction with a character or characters has ended
        /// </summary>
        [HideInInspector]
        public bool InteractionEnded;

        [HideInInspector]
        public SceneData CachedSceneData = null;
        [HideInInspector]
        public string ControllingCharacterUID = "";
        [HideInInspector]
        public int ControllingCharacterIndexInCharacteList;
        [HideInInspector]
        public InteractionType interactionType = InteractionType.Linear;
        [HideInInspector]
        private List<NodeData> tempNodedataCombination = new List<NodeData>();
        /// <summary>
        /// 
        /// </summary>
        [HideInInspector]
        public int TargetRoute;
        /// <summary>
        /// 
        /// </summary>
        [HideInInspector]
        public List<AudioSource> VoiceAudioSources = new List<AudioSource>();
        /// <summary>
        /// 
        /// </summary>
        [HideInInspector]
        public List<AudioSource> SoundEffectAudioSources = new List<AudioSource>();

        #endregion



    }
}