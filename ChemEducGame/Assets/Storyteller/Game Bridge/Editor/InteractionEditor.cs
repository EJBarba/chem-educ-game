using System.Collections.Generic;
using System.IO;
using System.Linq;
using DaiMangou.BridgedData;
using DaiMangou.Storyteller;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.InteropServices;
using System;

namespace DaiMangou.GameBridgeEditor
{
    [CustomEditor(typeof(Interaction))]
    [CanEditMultipleObjects]
    public class InteractionEditor : Editor
    {
#if UNITY_2018_2_6OR_NEWER

#endif
        public void OnEnable()
        {
            selectedInteraction = (Interaction)target;


            edwin = Resources.FindObjectsOfTypeAll(typeof(EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow")) as EditorWindow[];


            NameText = serializedObject.FindProperty("NameText");

            DisplayedText = serializedObject.FindProperty("DisplayedText");

            moveNextButton = serializedObject.FindProperty("MoveNextButton");

            // movePreviousButton = serializedObject.FindProperty("MovePreviousButton");

            RouteButton = serializedObject.FindProperty("RouteButton");

            RouteParent = serializedObject.FindProperty("RouteParent");

            TypingSpeed = serializedObject.FindProperty("TypingSpeed");

            TypingAudioCip = serializedObject.FindProperty("TypingAudioCip");

            UsesText = serializedObject.FindProperty("UsesText");

            // UsesStoryboardImages = serializedObject.FindProperty("UsesStoryboardImages");

            UsesVoiceOver = serializedObject.FindProperty("UsesVoiceOver");

            UsesSoundffects = serializedObject.FindProperty("UsesSoundffects");

            UseKeywordFilters = serializedObject.FindProperty("UseKeywordFilters");

            FilterCount = serializedObject.FindProperty("FilterCount");

            ShowKeywordFilterFouldout = serializedObject.FindProperty("ShowKeywordFilterFouldout");

            ShowGeneralSettings = serializedObject.FindProperty("ShowGeneralSettings");

            textDisplayMode = serializedObject.FindProperty("textDisplayMode");

            typingMode = serializedObject.FindProperty("typingMode");

            //  sceneData = serializedObject.FindProperty("sceneData");

            useNameUI = serializedObject.FindProperty("UseNameUI");

            useDialogueTextUI = serializedObject.FindProperty("UseDialogueTextUI");

            useMoveNextButton = serializedObject.FindProperty("UseMoveNextButton");

            // useMovePreviousButton = serializedObject.FindProperty("UseMovePreviousButton");

            useRouteButton = serializedObject.FindProperty("UseRouteButton");

            SetActionText = serializedObject.FindProperty("SetActionText");

            RoutClearsDIalogue = serializedObject.FindProperty("RoutClearsDIalogue");

            useAutoStartVoiceClip = serializedObject.FindProperty("AutoStartVoiceClip");

            useAutoStartSoundEffectClip = serializedObject.FindProperty("AutoStartSoundEffectClip");

            AutoStartAllConditionsByDefault = serializedObject.FindProperty("AutoStartAllConditionsByDefault");

            ShowNodeSpecificConditionSettings = serializedObject.FindProperty("ShowNodeSpecificConditionSettings");

            //  ComponentUID = serializedObject.FindProperty("ComponentUID");

            //    UpdateUID = serializedObject.FindProperty("UpdateUID");

            ControllingCharacterUID = serializedObject.FindProperty("ControllingCharacterUID");

            VolumeVariation = serializedObject.FindProperty("VolumeVariation");
            //   NameColour = serializedObject.FindProperty("NameColour");

            //   TextColour = serializedObject.FindProperty("TextColour");

            //   EditorApplication.delayCall += SetupCheck;

            interactionType = serializedObject.FindProperty("interactionType");

        }

        public void OnDisable()
        {
            // EditorApplication.delayCall -= SetupCheck;
        }

        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();


            if (edwin.Length == 0)
            {
                Repaint();
                edwin = Resources.FindObjectsOfTypeAll(typeof(EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow")) as EditorWindow[];
            }

            ScreenRect.size = new Vector2(edwin[0].position.width, edwin[0].position.height);

            serializedObject.Update();



            #region tell users to assign a SceneData Asset

            if (selectedInteraction.sceneData == null)
                EditorGUILayout.HelpBox("Please assign a SceneData file in the area below", MessageType.Info);

            selectedInteraction.sceneData =
                (SceneData)EditorGUILayout.ObjectField(selectedInteraction.sceneData, typeof(SceneData), false);

            #endregion

            if (selectedInteraction.sceneData == null)
                return;

            if (selectedInteraction.sceneData.SceneID == -1)
            {
                EditorGUILayout.HelpBox("This SceneData has no story scenes in it", MessageType.Info);
                return;
            }
            if (selectedInteraction.sceneData.Characters.Count == 0)
            {
                EditorGUILayout.HelpBox("There are no characters in this SceneData", MessageType.Info);
                return;
            }

            if (showHelpMessage)
                EditorGUILayout.HelpBox("This will update the Interaction  with all the necessary data from the scene necessary for the dialoguer", MessageType.Info);


            GUILayout.Space(5);




            if (selectedInteraction.CachedSceneData == null)
                DoSetup();


            if (selectedInteraction.CachedSceneData != selectedInteraction.sceneData)
            {
                if (EditorUtility.DisplayDialog("Update SceneData ",
             "Are you sure that you want to replace this SceneData, all previous node data that are notin this current SceneData will be lost ", "Update", "Don't Update"))
                    DoSetup();

                if (selectedInteraction.sceneData != selectedInteraction.CachedSceneData)
                    selectedInteraction.sceneData = selectedInteraction.CachedSceneData;

            }



            if (!selectedInteraction.sceneData.UpdateUID.Equals(selectedInteraction.UpdateUID))
            {
                DoSetup();
                Debug.Log("Ran <b><color=#009EFF>Auto Setup</color></b>");
            }

            //TEMP . DELETE AFTER VERSION 2021.0
            /* if (selectedInteraction.VoiceAudioSources.Count != selectedInteraction.sceneData.Characters.Count)
             {
                 selectedInteraction.VoiceAudioSources.Resize(selectedInteraction.sceneData.Characters.Count);
                 selectedInteraction.SoundEffectAudioSources.Resize(selectedInteraction.sceneData.Characters.Count);



                 for (int v = 0; v < selectedInteraction.sceneData.Characters.Count; v++)
                 {
                     var targetCharacter = selectedInteraction.sceneData.Characters[v];

                     var VoiceAudioManager = new GameObject(targetCharacter.Name + "Voice");
                     VoiceAudioManager.transform.SetParent(selectedInteraction.transform.Find("Audio Manager"));
                     VoiceAudioManager.transform.localPosition = Vector3.zero;
                     VoiceAudioManager.AddComponent<AudioSource>();
                     selectedInteraction.VoiceAudioSources[v] = VoiceAudioManager.GetComponent<AudioSource>();

                     var SoundEffectsAudioManager = new GameObject(targetCharacter.Name + "Sound Effects");
                     SoundEffectsAudioManager.transform.SetParent(selectedInteraction.transform.Find("Audio Manager"));
                     SoundEffectsAudioManager.transform.localPosition = Vector3.zero;
                     SoundEffectsAudioManager.AddComponent<AudioSource>();
                     selectedInteraction.SoundEffectAudioSources[v] = SoundEffectsAudioManager.GetComponent<AudioSource>();
                 }
             }*/

            selectedInteraction.sceneData.LanguageIndex = EditorGUILayout.Popup(selectedInteraction.sceneData.LanguageIndex, selectedInteraction.sceneData.LanguageNameArray);

            GUILayout.Space(5);


            GUILayout.Label("Interaction Type");
            interactionType.enumValueIndex =
                (int)(InteractionType)EditorGUILayout.EnumPopup(
                    selectedInteraction.interactionType, GUILayout.Height(15));

            if (interactionType.enumValueIndex == (int)InteractionType.Linear)
                EditorGUILayout.HelpBox("In linear mode, call Interaction.doRefresh(); or GenerateActiveDialogueSet();  to start processing Linear SceneData at runtime", MessageType.Info);

            GUILayout.Space(10);


            /* if (GUILayout.Button("Show / Hide Hidden Data"))
             {
                 if (selectedInteraction.ReflectedDataParent != null)
                     if (selectedInteraction.ReflectedDataParent.hideFlags == HideFlags.None)
                         selectedInteraction.ReflectedDataParent.hideFlags = HideFlags.HideInHierarchy;
                     else
                         selectedInteraction.ReflectedDataParent.hideFlags = HideFlags.None;

                 if (selectedInteraction.GeneralReflectedDataParent != null)
                     if (selectedInteraction.GeneralReflectedDataParent.hideFlags == HideFlags.None)
                         selectedInteraction.GeneralReflectedDataParent.hideFlags = HideFlags.HideInHierarchy;
                     else
                         selectedInteraction.GeneralReflectedDataParent.hideFlags = HideFlags.None;

             }*/



            Separator2();
            Separator3();

            #region begin checking and setting the tempStory data reference
            if (tempStoryData == null)
            {
                if (File.Exists(Application.dataPath + "/TempStoryData.asset"))
                {
                    var data = AssetDatabase.LoadAllAssetsAtPath("Assets/TempStoryData.asset");
                    foreach (var asset in data)
                        if (asset.GetType() == typeof(Story))
                            tempStoryData = asset as Story;
                }
                else
                {
                    EditorGUILayout.HelpBox(
                        "It seems that you have no Storyteller project open. Please open your storyteller project ",
                        MessageType.Info);
                    return;
                }
            }
            #endregion

            if (selectedInteraction.sceneData.UID != tempStoryData.Scenes[selectedInteraction.sceneData.SceneID].UID)
            {
                EditorGUILayout.HelpBox("The current Story project open in the Storyteller editor does not match the Game scene. You may have selected the incorrect interaction System gameobject/ wrong story porject", MessageType.Info);
                return;
            }

            if (selectedInteraction.ReflectedDataParent == null) return;


            #region General Settings



            ShowGeneralSettings.boolValue = EditorGUILayout.Foldout(selectedInteraction.ShowGeneralSettings,
                "General Settings (for all nodes)");

            if (selectedInteraction.ShowGeneralSettings)
            {
                if (showHelpMessage)
                    EditorGUILayout.HelpBox("By turning on Use Text you are choosing to use a UI system to display dialogue", MessageType.Info);

                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                GUILayout.Label("Use Text");
                GUILayout.FlexibleSpace();
                var usesTextPowerIcon = selectedInteraction.UsesText ? ImageLibrary.PowerOnpro : ImageLibrary.PowerOffpro;
                if (GUILayout.Button(usesTextPowerIcon, GUIStyle.none, GUILayout.Width(15), GUILayout.Height(15)))
                    UsesText.boolValue = !selectedInteraction.UsesText;
                GUILayout.EndHorizontal();
                GUILayout.Space(5);

                if (selectedInteraction.UsesText)
                {
                    selectedInteraction.ShowUIAndTextSettings =
                        EditorGUILayout.Foldout(selectedInteraction.ShowUIAndTextSettings, "UI Settings");

                    if (selectedInteraction.ShowUIAndTextSettings)
                    {
                        #region sdettings for Name UI

                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button(useNameUI.boolValue ? ImageLibrary.PowerOnpro : ImageLibrary.PowerOffpro,
                            GUIStyle.none, GUILayout.Width(15), GUILayout.Height(15)))
                            useNameUI.boolValue = !useNameUI.boolValue;
                        GUILayout.Label("Use a name UI");
                        GUILayout.EndHorizontal();
                        GUILayout.Space(5);

                        EditorGUI.BeginDisabledGroup(!useNameUI.boolValue);
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Name UI");
                        GUILayout.FlexibleSpace();
                        NameText.objectReferenceValue = (TextMeshProUGUI)EditorGUILayout.ObjectField(selectedInteraction.NameText,
                            typeof(TextMeshProUGUI), true, GUILayout.Height(15), GUILayout.Width(115));
                        GUILayout.EndHorizontal();
                        EditorGUI.EndDisabledGroup();
                        GUILayout.Space(5);
                        Separator2();

                        #endregion


                        #region sdettings for Dialogue UI

                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button(
                            useDialogueTextUI.boolValue ? ImageLibrary.PowerOnpro : ImageLibrary.PowerOffpro,
                            GUIStyle.none, GUILayout.Width(15), GUILayout.Height(15)))
                            useDialogueTextUI.boolValue = !useDialogueTextUI.boolValue;
                        GUILayout.Label("Use dialogue text UI");
                        GUILayout.EndHorizontal();
                        GUILayout.Space(5);

                        EditorGUI.BeginDisabledGroup(!useDialogueTextUI.boolValue);
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Dialogue Text UI");
                        GUILayout.FlexibleSpace();

                        DisplayedText.objectReferenceValue = (TextMeshProUGUI)EditorGUILayout.ObjectField(
                                                   selectedInteraction.DisplayedText, typeof(TextMeshProUGUI), true, GUILayout.Height(15),
                                                   GUILayout.Width(115));
                        GUILayout.EndHorizontal();
                        EditorGUI.EndDisabledGroup();
                        GUILayout.Space(5);
                        Separator2();

                        #endregion

                        #region sdettings for Next Button UI

                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button(
                            useMoveNextButton.boolValue ? ImageLibrary.PowerOnpro : ImageLibrary.PowerOffpro,
                            GUIStyle.none, GUILayout.Width(15), GUILayout.Height(15)))
                            useMoveNextButton.boolValue = !useMoveNextButton.boolValue;
                        GUILayout.Label("Use next button UI");
                        GUILayout.EndHorizontal();
                        GUILayout.Space(5);

                        EditorGUI.BeginDisabledGroup(!useMoveNextButton.boolValue);
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Button Next");
                        GUILayout.FlexibleSpace();
                        moveNextButton.objectReferenceValue = (Button)EditorGUILayout.ObjectField(
                            selectedInteraction.MoveNextButton, typeof(Button), true, GUILayout.Height(15),
                            GUILayout.Width(115));
                        GUILayout.EndHorizontal();
                        EditorGUI.EndDisabledGroup();
                        GUILayout.Space(5);
                        Separator2();

                        #endregion


                        #region sdettings for Previous Button UI
                        /*
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button(
                            useMovePreviousButton.boolValue ? ImageLibrary.PowerOnpro : ImageLibrary.PowerOffpro,
                            GUIStyle.none, GUILayout.Width(15), GUILayout.Height(15)))
                            useMovePreviousButton.boolValue = !useMovePreviousButton.boolValue;
                        GUILayout.Label("Use previous button UI");
                        GUILayout.EndHorizontal();
                        GUILayout.Space(5);

                        EditorGUI.BeginDisabledGroup(!useMovePreviousButton.boolValue);
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Button Previous");
                        GUILayout.FlexibleSpace();
                        movePreviousButton.objectReferenceValue = (Button)EditorGUILayout.ObjectField(
                            selectedInteraction.MovePreviousButton, typeof(Button), true, GUILayout.Height(15),
                            GUILayout.Width(115));
                        GUILayout.EndHorizontal();
                        EditorGUI.EndDisabledGroup();
                        GUILayout.Space(5);
                        Separator2();
                        */
                        #endregion


                        #region sdettings for Route  UI

                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button(
                            useRouteButton.boolValue ? ImageLibrary.PowerOnpro : ImageLibrary.PowerOffpro,
                            GUIStyle.none, GUILayout.Width(15), GUILayout.Height(15)))
                            useRouteButton.boolValue = !useRouteButton.boolValue;
                        GUILayout.Label("Use route button UI");
                        GUILayout.EndHorizontal();
                        GUILayout.Space(5);

                        EditorGUI.BeginDisabledGroup(!useRouteButton.boolValue);
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Route Parent");
                        GUILayout.FlexibleSpace();
                        RouteParent.objectReferenceValue = (GameObject)EditorGUILayout.ObjectField(
                            selectedInteraction.RouteParent, typeof(GameObject), true, GUILayout.Height(15),
                            GUILayout.Width(115));
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Route Button");
                        GUILayout.FlexibleSpace();
                        RouteButton.objectReferenceValue = (Button)EditorGUILayout.ObjectField(
                            selectedInteraction.RouteButton, typeof(Button), true, GUILayout.Height(15),
                            GUILayout.Width(115));
                        GUILayout.EndHorizontal();
                        EditorGUI.EndDisabledGroup();
                        GUILayout.Space(5);
                        Separator2();

                        #endregion
                    }

                    if (showHelpMessage)
                        EditorGUILayout.HelpBox("In text display settings you have the option of using typed text or immediately displayed text", MessageType.Info);

                    selectedInteraction.ShowTextDisplayModeSettings =
                        EditorGUILayout.Foldout(selectedInteraction.ShowTextDisplayModeSettings, "Text Display Settings");

                    if (selectedInteraction.ShowTextDisplayModeSettings)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Text Display Mode");
                        textDisplayMode.enumValueIndex =
                            (int)(InteractionTextDisplayMode)EditorGUILayout.EnumPopup(
                                selectedInteraction.textDisplayMode, GUILayout.Height(15), GUILayout.Width(115));
                        GUILayout.EndHorizontal();

                        switch (selectedInteraction.textDisplayMode)
                        {
                            case InteractionTextDisplayMode.Instant:

                                break;
                            case InteractionTextDisplayMode.Typed:
                                GUILayout.Space(5);

                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Typing Speed");
                                GUILayout.FlexibleSpace();
                                TypingSpeed.floatValue = EditorGUILayout.FloatField(selectedInteraction.TypingSpeed,
                                    GUILayout.Height(15), GUILayout.Width(115));
                                GUILayout.EndHorizontal();


                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Typing Sound");
                                GUILayout.FlexibleSpace();
                                TypingAudioCip.objectReferenceValue =
                                    (AudioClip)EditorGUILayout.ObjectField(selectedInteraction.TypingAudioCip,
                                        typeof(AudioClip), false, GUILayout.Height(15), GUILayout.Width(115));
                                GUILayout.EndHorizontal();

                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Volume Variation");
                                VolumeVariation.floatValue = EditorGUILayout.DelayedFloatField(VolumeVariation.floatValue, GUILayout.Width(115));
                                GUILayout.EndHorizontal();
                                GUILayout.Space(5);

                                if (showHelpMessage)
                                    EditorGUILayout.HelpBox(" Typing mode allows you to make granular decissions on when you want typing to take place. if the character is the player, not the player or both", MessageType.Info);

                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Typing Mode");
                                typingMode.enumValueIndex =
                                    (int)(InteractionTypingModes)EditorGUILayout.EnumPopup(
                                        selectedInteraction.typingMode, GUILayout.Height(15), GUILayout.Width(115));
                                GUILayout.EndHorizontal();




                                break;
                                /*    case DialoguerTextDisplayMode.Custom:

                                        break;*/
                        }


                    }

                    GUILayout.Space(5);
                    Separator2();


                    if (showHelpMessage)
                        EditorGUILayout.HelpBox("This may be turned off, it is typically used  as a temporart placeholder for animations etc to represent the written action", MessageType.Info);


                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(
                        SetActionText.boolValue ? ImageLibrary.PowerOnpro : ImageLibrary.PowerOffpro,
                        GUIStyle.none, GUILayout.Width(15), GUILayout.Height(15)))
                        SetActionText.boolValue = !SetActionText.boolValue;
                    GUILayout.Label("Display Action Text");
                    GUILayout.EndHorizontal();
                    GUILayout.Space(5);

                    Separator();

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(
                        RoutClearsDIalogue.boolValue ? ImageLibrary.PowerOnpro : ImageLibrary.PowerOffpro,
                        GUIStyle.none, GUILayout.Width(15), GUILayout.Height(15)))
                        RoutClearsDIalogue.boolValue = !RoutClearsDIalogue.boolValue;
                    GUILayout.Label("Route display clears dialogue text");
                    GUILayout.EndHorizontal();
                    GUILayout.Space(5);


                    Separator();


                    if (showHelpMessage)
                        EditorGUILayout.HelpBox("With Keyword filters turned on, this Player character can replace any word in dialogue with another word." +
                            " There are four variations of text replacement settings. Replacing static text with dynamic text, dynamic text with static text, ststic text with ststic text and dynamic text with dynamic text", MessageType.Info);


                    var keywordIdentifierCount = selectedInteraction.KeywordFilters.Count;

                    GUILayout.Space(5);
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Use Keyword Filters");
                    GUILayout.FlexibleSpace();
                    var usesKeywordFiltersPowerIcon = selectedInteraction.UseKeywordFilters
                        ? ImageLibrary.PowerOnpro
                        : ImageLibrary.PowerOffpro;
                    if (GUILayout.Button(usesKeywordFiltersPowerIcon, GUIStyle.none, GUILayout.Width(15),
                        GUILayout.Height(15)))
                        UseKeywordFilters.boolValue = !selectedInteraction.UseKeywordFilters;
                    GUILayout.EndHorizontal();
                    GUILayout.Space(5);


                    if (selectedInteraction.UseKeywordFilters)
                    {
                        if (showHelpMessage)
                            EditorGUILayout.HelpBox("Keyword filters change marked words into other words",
                            MessageType.Info);


                        ShowKeywordFilterFouldout.boolValue =
                            EditorGUILayout.Foldout(selectedInteraction.ShowKeywordFilterFouldout,
                                "Show Key Word Filters");

                        if (selectedInteraction.ShowKeywordFilterFouldout)
                        {
                            GUILayout.Space(5);
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Amount Of Filters", GUILayout.Width(115));
                            GUILayout.FlexibleSpace();
                            FilterCount.intValue =
                                EditorGUILayout.DelayedIntField(selectedInteraction.FilterCount, GUILayout.Width(100));
                            if (keywordIdentifierCount != selectedInteraction.FilterCount)
                                selectedInteraction.KeywordFilters.Resize(selectedInteraction.FilterCount);
                            GUILayout.Space(15);
                            GUILayout.EndHorizontal();
                            GUILayout.Space(5);

                            for (var i = 0; i < selectedInteraction.KeywordFilters.Count; i++)
                            {
                                if (selectedInteraction.KeywordFilters[i] == null)
                                    selectedInteraction.KeywordFilters[i] = new KeywordFilter();


                                var area = GUILayoutUtility.GetLastRect().AddRect(-15, 0, 0, 10);
                                var filterAreHeight = selectedInteraction.KeywordFilters[i].StaticKeywordMethod && selectedInteraction.KeywordFilters[i].StaticReplacementStringMethod ? 105 : 170;
                                var filterArea = area.PlaceUnder(Screen.width, filterAreHeight);
                                GUI.DrawTexture(filterArea, Textures.DuskLightest);
                                var headerArea = filterArea.ToUpperLeft(0, 3, 0, 15);
                                GUI.DrawTexture(headerArea, Textures.DuskLighter);

                                #region Header Content



                                var moveKeywordFilterUpButtonArea = headerArea.ToCenterLeft(15, 8, 25, -10);
                                if (ClickEvent.Click(1, moveKeywordFilterUpButtonArea, ImageLibrary.upArrow, "Move this Keyword Filter up by one position"))
                                {
                                    if (i > 0)
                                    {
                                        var KeywordFilterAtTop = selectedInteraction.KeywordFilters[i - 1];

                                        selectedInteraction.KeywordFilters[i - 1] = null;
                                        // selectedInteraction.KeywordFilters[i] = null;

                                        selectedInteraction.KeywordFilters[i - 1] = selectedInteraction.KeywordFilters[i];
                                        selectedInteraction.KeywordFilters[i] = KeywordFilterAtTop;
                                    }
                                }


                                var moveKeywordFilterDownButtonArea = moveKeywordFilterUpButtonArea.PlaceToRight(15, 0, 20);
                                if (ClickEvent.Click(1, moveKeywordFilterDownButtonArea, ImageLibrary.downArrow, "Move this keyword Filter down by one position"))
                                {
                                    if (i != selectedInteraction.KeywordFilters.Count - 1)
                                    {
                                        var KeywordFilterAtBottom = selectedInteraction.KeywordFilters[i + 1];

                                        selectedInteraction.KeywordFilters[i + 1] = null;
                                        //selectedInteraction.KeywordFilters[i] = null;

                                        selectedInteraction.KeywordFilters[i + 1] = selectedInteraction.KeywordFilters[i];
                                        selectedInteraction.KeywordFilters[i] = KeywordFilterAtBottom;
                                    }
                                }



                                if (ClickEvent.Click(1, headerArea.ToCenterRight(15, 15, -25, -8),
    selectedInteraction.KeywordFilters[i].Disabled ? ImageLibrary.PowerOffpro : ImageLibrary.PowerOnpro, "Enable / Disable this Keyword Filter"))
                                    selectedInteraction.KeywordFilters[i].Disabled = !selectedInteraction.KeywordFilters[i].Disabled;
                                #endregion

                                EditorGUI.BeginDisabledGroup(selectedInteraction.KeywordFilters[i].Disabled);

                                GUILayout.Space(25);
                                GUILayout.BeginHorizontal();
                                GUILayout.BeginVertical();
                                GUILayout.Label("Key Word", GUILayout.Width(100));
                                selectedInteraction.KeywordFilters[i].StaticKeywordMethod = GUILayout.Toggle(selectedInteraction.KeywordFilters[i].StaticKeywordMethod, "Static Method", GUILayout.Width(100));
                                GUILayout.EndVertical();
                                GUILayout.FlexibleSpace();
                                GUILayout.BeginVertical();
                                GUILayout.Label("Replacement", GUILayout.Width(100));
                                selectedInteraction.KeywordFilters[i].StaticReplacementStringMethod = GUILayout.Toggle(selectedInteraction.KeywordFilters[i].StaticReplacementStringMethod, "Static Method", GUILayout.Width(100));
                                GUILayout.EndVertical();
                                GUILayout.Space(15);
                                GUILayout.EndHorizontal();

                                var keyWordIdentifier = selectedInteraction.KeywordFilters[i];

                                if (keyWordIdentifier == null)
                                    keyWordIdentifier = new KeywordFilter();

                                GUILayout.Space(5);
                                GUILayout.BeginHorizontal();

                                if (keyWordIdentifier.StaticKeywordMethod)
                                {
                                    keyWordIdentifier.KeyWord =
                                    EditorGUILayout.TextField(keyWordIdentifier.KeyWord, GUILayout.Width(100), GUILayout.Height(20));
                                }
                                else
                                {
                                    #region dynamic keyword string

                                    GUILayout.BeginVertical();
                                    GUILayout.BeginHorizontal();
                                    GUILayout.FlexibleSpace();

                                    selectedInteraction.KeywordFilters[i].DynamicKeyword.TargetGameObject = (GameObject)EditorGUILayout.ObjectField(selectedInteraction.KeywordFilters[i].DynamicKeyword.TargetGameObject,
                                       typeof(GameObject), true, GUILayout.Height(15));
                                    GUILayout.FlexibleSpace();
                                    GUILayout.EndHorizontal();

                                    if (selectedInteraction.KeywordFilters[i].DynamicKeyword.cachedTargetObject != selectedInteraction.KeywordFilters[i].DynamicKeyword.TargetGameObject)
                                    {
                                        selectedInteraction.KeywordFilters[i].DynamicKeyword.Components = new Component[0];
                                        selectedInteraction.KeywordFilters[i].DynamicKeyword.serializedMethods = new SerializableMethodInfo[0];

                                        selectedInteraction.KeywordFilters[i].DynamicKeyword.SetComponent(0);
                                        selectedInteraction.KeywordFilters[i].DynamicKeyword.SetMethod(0);
                                        selectedInteraction.KeywordFilters[i].DynamicKeyword.cachedTargetObject = selectedInteraction.KeywordFilters[i].DynamicKeyword.TargetGameObject;
                                    }


                                    GUILayout.BeginHorizontal();
                                    GUILayout.FlexibleSpace();
                                    GUILayout.Box(ImageLibrary.DownFlowArrow, EditorStyles.label, GUILayout.Width(15),
                                        GUILayout.Height(15));
                                    GUILayout.FlexibleSpace();
                                    GUILayout.EndHorizontal();


                                    var disabledComponents = selectedInteraction.KeywordFilters[i].DynamicKeyword.TargetGameObject == null;
                                    EditorGUI.BeginDisabledGroup(disabledComponents);

                                    if (disabledComponents &&
                                        (selectedInteraction.KeywordFilters[i].DynamicKeyword.Components.Count() != 0 || selectedInteraction.KeywordFilters[i].DynamicKeyword.serializedMethods.Count() != 0))
                                    {
                                        selectedInteraction.KeywordFilters[i].DynamicKeyword.Components = new Component[0];
                                        selectedInteraction.KeywordFilters[i].DynamicKeyword.serializedMethods = new SerializableMethodInfo[0];
                                    }

                                    GUILayout.BeginHorizontal();
                                    GUILayout.FlexibleSpace();
                                    var componentName = !selectedInteraction.KeywordFilters[i].DynamicKeyword.Components.Any()
                                        ? "None"
                                        : selectedInteraction.KeywordFilters[i].DynamicKeyword.Components[selectedInteraction.KeywordFilters[i].DynamicKeyword.ComponentIndex].GetType().Name;


                                    if (GUILayout.Button(componentName, EditorStyles.popup, GUILayout.MinWidth(100), GUILayout.Height(15)))
                                    {
                                        selectedInteraction.KeywordFilters[i].DynamicKeyword.GetGameObjectComponents();
                                        var menu = new GenericMenu();
                                        for (var u = 0; u < selectedInteraction.KeywordFilters[i].DynamicKeyword.Components.Length; u++)
                                            menu.AddItem(new GUIContent(selectedInteraction.KeywordFilters[i].DynamicKeyword.Components[u].GetType().Name),
                                                   selectedInteraction.KeywordFilters[i].DynamicKeyword.ComponentIndex.Equals(u), selectedInteraction.KeywordFilters[i].DynamicKeyword.SetComponent, u);
                                        menu.ShowAsContext();
                                    }

                                    GUILayout.FlexibleSpace();
                                    GUILayout.EndHorizontal();

                                    GUILayout.BeginHorizontal();
                                    GUILayout.FlexibleSpace();
                                    GUILayout.Box(ImageLibrary.DownFlowArrow, EditorStyles.label, GUILayout.Width(15),
                                        GUILayout.Height(15));
                                    GUILayout.FlexibleSpace();
                                    GUILayout.EndHorizontal();

                                    GUILayout.BeginHorizontal();
                                    GUILayout.FlexibleSpace();
                                    var methodName = !selectedInteraction.KeywordFilters[i].DynamicKeyword.serializedMethods.Any()
                                        ? "None"
                                        : selectedInteraction.KeywordFilters[i].DynamicKeyword.serializedMethods[selectedInteraction.KeywordFilters[i].DynamicKeyword.MethodIndex].methodName;
                                    //  var disabledMethods = condition.TargetGameObject == null;


                                    if (GUILayout.Button(methodName, EditorStyles.popup, GUILayout.MinWidth(100), GUILayout.Height(15)))
                                    {
                                        selectedInteraction.KeywordFilters[i].DynamicKeyword.GetComponentMethods();
                                        var menu = new GenericMenu();
                                        for (var u = 0; u < selectedInteraction.KeywordFilters[i].DynamicKeyword.serializedMethods.Length; u++)
                                            menu.AddItem(new GUIContent(selectedInteraction.KeywordFilters[i].DynamicKeyword.serializedMethods[u].methodInfo.Name),
                                                   selectedInteraction.KeywordFilters[i].DynamicKeyword.MethodIndex.Equals(u), selectedInteraction.KeywordFilters[i].DynamicKeyword.SetMethod, u);
                                        menu.ShowAsContext();
                                    }

                                    GUILayout.FlexibleSpace();
                                    GUILayout.EndHorizontal();



                                    EditorGUI.EndDisabledGroup();



                                    GUILayout.EndVertical();

                                    #endregion
                                }
                                GUILayout.FlexibleSpace();

                                if (keyWordIdentifier.StaticReplacementStringMethod)
                                {
                                    keyWordIdentifier.ReplacementString =
                                        EditorGUILayout.TextField(keyWordIdentifier.ReplacementString, GUILayout.Width(100), GUILayout.Height(20));
                                }
                                else
                                {
                                    #region dynamic replacement String

                                    GUILayout.BeginVertical();
                                    GUILayout.BeginHorizontal();
                                    GUILayout.FlexibleSpace();

                                    selectedInteraction.KeywordFilters[i].DynamicReplacementString.TargetGameObject = (GameObject)EditorGUILayout.ObjectField(selectedInteraction.KeywordFilters[i].DynamicReplacementString.TargetGameObject,
                                       typeof(GameObject), true, GUILayout.Height(15));
                                    GUILayout.FlexibleSpace();
                                    GUILayout.EndHorizontal();

                                    if (selectedInteraction.KeywordFilters[i].DynamicReplacementString.cachedTargetObject != selectedInteraction.KeywordFilters[i].DynamicReplacementString.TargetGameObject)
                                    {
                                        selectedInteraction.KeywordFilters[i].DynamicReplacementString.Components = new Component[0];
                                        selectedInteraction.KeywordFilters[i].DynamicReplacementString.serializedMethods = new SerializableMethodInfo[0];

                                        selectedInteraction.KeywordFilters[i].DynamicReplacementString.SetComponent(0);
                                        selectedInteraction.KeywordFilters[i].DynamicReplacementString.SetMethod(0);
                                        selectedInteraction.KeywordFilters[i].DynamicReplacementString.cachedTargetObject = selectedInteraction.KeywordFilters[i].DynamicReplacementString.TargetGameObject;
                                    }


                                    GUILayout.BeginHorizontal();
                                    GUILayout.FlexibleSpace();
                                    GUILayout.Box(ImageLibrary.DownFlowArrow, EditorStyles.label, GUILayout.Width(15),
                                        GUILayout.Height(15));
                                    GUILayout.FlexibleSpace();
                                    GUILayout.EndHorizontal();


                                    var disabledComponents = selectedInteraction.KeywordFilters[i].DynamicReplacementString.TargetGameObject == null;
                                    EditorGUI.BeginDisabledGroup(disabledComponents);

                                    if (disabledComponents &&
                                        (selectedInteraction.KeywordFilters[i].DynamicReplacementString.Components.Count() != 0 || selectedInteraction.KeywordFilters[i].DynamicReplacementString.serializedMethods.Count() != 0))
                                    {
                                        selectedInteraction.KeywordFilters[i].DynamicReplacementString.Components = new Component[0];
                                        selectedInteraction.KeywordFilters[i].DynamicReplacementString.serializedMethods = new SerializableMethodInfo[0];
                                    }

                                    GUILayout.BeginHorizontal();
                                    GUILayout.FlexibleSpace();
                                    var componentName = !selectedInteraction.KeywordFilters[i].DynamicReplacementString.Components.Any()
                                        ? "None"
                                        : selectedInteraction.KeywordFilters[i].DynamicReplacementString.Components[selectedInteraction.KeywordFilters[i].DynamicReplacementString.ComponentIndex].GetType().Name;


                                    if (GUILayout.Button(componentName, EditorStyles.popup, GUILayout.MinWidth(100), GUILayout.Height(15)))
                                    {
                                        selectedInteraction.KeywordFilters[i].DynamicReplacementString.GetGameObjectComponents();
                                        var menu = new GenericMenu();
                                        for (var u = 0; u < selectedInteraction.KeywordFilters[i].DynamicReplacementString.Components.Length; u++)
                                            menu.AddItem(new GUIContent(selectedInteraction.KeywordFilters[i].DynamicReplacementString.Components[u].GetType().Name),
                                                   selectedInteraction.KeywordFilters[i].DynamicReplacementString.ComponentIndex.Equals(u), selectedInteraction.KeywordFilters[i].DynamicReplacementString.SetComponent, u);
                                        menu.ShowAsContext();
                                    }

                                    GUILayout.FlexibleSpace();
                                    GUILayout.EndHorizontal();

                                    GUILayout.BeginHorizontal();
                                    GUILayout.FlexibleSpace();
                                    GUILayout.Box(ImageLibrary.DownFlowArrow, EditorStyles.label, GUILayout.Width(15),
                                        GUILayout.Height(15));
                                    GUILayout.FlexibleSpace();
                                    GUILayout.EndHorizontal();

                                    GUILayout.BeginHorizontal();
                                    GUILayout.FlexibleSpace();
                                    var methodName = !selectedInteraction.KeywordFilters[i].DynamicReplacementString.serializedMethods.Any()
                                        ? "None"
                                        : selectedInteraction.KeywordFilters[i].DynamicReplacementString.serializedMethods[selectedInteraction.KeywordFilters[i].DynamicReplacementString.MethodIndex].methodName;
                                    //  var disabledMethods = condition.TargetGameObject == null;


                                    if (GUILayout.Button(methodName, EditorStyles.popup, GUILayout.MinWidth(100), GUILayout.Height(15)))
                                    {
                                        selectedInteraction.KeywordFilters[i].DynamicReplacementString.GetComponentMethods();
                                        var menu = new GenericMenu();
                                        for (var u = 0; u < selectedInteraction.KeywordFilters[i].DynamicReplacementString.serializedMethods.Length; u++)
                                            menu.AddItem(new GUIContent(selectedInteraction.KeywordFilters[i].DynamicReplacementString.serializedMethods[u].methodInfo.Name),
                                                   selectedInteraction.KeywordFilters[i].DynamicReplacementString.MethodIndex.Equals(u), selectedInteraction.KeywordFilters[i].DynamicReplacementString.SetMethod, u);
                                        menu.ShowAsContext();
                                    }

                                    GUILayout.FlexibleSpace();
                                    GUILayout.EndHorizontal();



                                    EditorGUI.EndDisabledGroup();



                                    GUILayout.EndVertical();

                                    #endregion
                                }

                                EditorGUI.EndDisabledGroup();

                                GUILayout.EndHorizontal();

                                GUILayout.Space(5);

                                if (GUILayout.Button(ImageLibrary.deleteConditionIcon, GUIStyle.none,
                                    GUILayout.Width(15)))
                                {
                                    selectedInteraction.KeywordFilters.RemoveAt(i);
                                    selectedInteraction.KeywordFilters.RemoveAll(n => n == null);
                                    selectedInteraction.FilterCount = selectedInteraction.KeywordFilters.Count;
                                }
                                GUILayout.Space(15);
                            }
                        }
                    }
                }

                Separator();


                /* GUILayout.Space(5);
                 GUILayout.BeginHorizontal();
                 GUILayout.Label("Use Storyboard Images");
                 GUILayout.FlexibleSpace();
                 var usesStoryboardImagesPowerIcon = selectedInteraction.UsesStoryboardImages ? ImageLibrary.PowerOnpro : ImageLibrary.PowerOffpro;
                 if (GUILayout.Button(usesStoryboardImagesPowerIcon, GUIStyle.none, GUILayout.Width(15), GUILayout.Height(15)))
                     UsesStoryboardImages.boolValue = !selectedInteraction.UsesStoryboardImages;
                 GUILayout.EndHorizontal();
                 GUILayout.Space(5);
 
                 if (selectedInteraction.UsesStoryboardImages)
                 { }
                 Separator();*/

                if (showHelpMessage)
                    EditorGUILayout.HelpBox("The UseVoiceover toggle preps your character to do voice clip playbacks", MessageType.Info);

                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                GUILayout.Label("Use Voiceover");
                GUILayout.FlexibleSpace();
                var UsesVoiceOverPowerIcon =
                    selectedInteraction.UsesVoiceOver ? ImageLibrary.PowerOnpro : ImageLibrary.PowerOffpro;
                if (GUILayout.Button(UsesVoiceOverPowerIcon, GUIStyle.none, GUILayout.Width(15), GUILayout.Height(15)))
                    UsesVoiceOver.boolValue = !selectedInteraction.UsesVoiceOver;
                GUILayout.EndHorizontal();
                GUILayout.Space(5);

                if (selectedInteraction.UsesVoiceOver)
                {
                    if (showHelpMessage)
                        EditorGUILayout.HelpBox("With Auto Start turned on, your voice clip will play as soon as the data associated with the voice clip is processed (Using the Condition system to trigger playback is recommended)", MessageType.Info);

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(useAutoStartVoiceClip.boolValue ? ImageLibrary.PowerOnpro : ImageLibrary.PowerOffpro,
                        GUIStyle.none, GUILayout.Width(15), GUILayout.Height(15)))
                        useAutoStartVoiceClip.boolValue = !useAutoStartVoiceClip.boolValue;
                    GUILayout.Label("Auto start");
                    GUILayout.EndHorizontal();
                    GUILayout.Space(5);
                }

                Separator();

                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                GUILayout.Label("Use Sound Effects");
                GUILayout.FlexibleSpace();
                var usesSoundffectsPowerIcon = UsesSoundffects.boolValue
                    ? ImageLibrary.PowerOnpro
                    : ImageLibrary.PowerOffpro;
                if (GUILayout.Button(usesSoundffectsPowerIcon, GUIStyle.none, GUILayout.Width(15),
                    GUILayout.Height(15)))
                    UsesSoundffects.boolValue = !UsesSoundffects.boolValue;
                GUILayout.EndHorizontal();
                GUILayout.Space(2);

                if (selectedInteraction.UsesSoundffects)
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(useAutoStartSoundEffectClip.boolValue ? ImageLibrary.PowerOnpro : ImageLibrary.PowerOffpro,
                        GUIStyle.none, GUILayout.Width(15), GUILayout.Height(15)))
                        useAutoStartSoundEffectClip.boolValue = !useAutoStartSoundEffectClip.boolValue;
                    GUILayout.Label("Auto start");
                    GUILayout.EndHorizontal();
                    GUILayout.Space(5);
                }
            }


            GUILayout.Space(5);

            #endregion





            #region if no scenes are in the project

            if (tempStoryData.Scenes.Count == 0) return;

            #endregion

            if (CurrentStory.SceneOrderEdited)
            {
                var targetScene = tempStoryData.Scenes.Find(s => s.UID == selectedInteraction.sceneData.UID);
                if (targetScene != null)
                {
                    var id = tempStoryData.Scenes.IndexOf(targetScene);
                    selectedInteraction.sceneData.SceneID = id;
                }
                CurrentStory.SceneOrderEdited = false;

            }

            if (selectedInteraction.sceneData.SceneID + 1 > tempStoryData.Scenes.Count)
            {
                EditorGUILayout.HelpBox(
                    "Make sure that this Story project is the correct project. Else check to ensure that the scene was not deleted ",
                    MessageType.Info);
                // if (GUILayout.Button("Attempt Correction"))
                CurrentStory.SceneOrderEdited = true;

                return;
            }

            var scene = tempStoryData.Scenes[selectedInteraction.sceneData.SceneID];

            #region if no nodes are in the scene

            if (scene.NodeElements.Count == 0)
            {
                EditorGUILayout.HelpBox("There is nothing in this Storyteller scene", MessageType.Info);
                return;
            }

            #endregion

            var selectedNode = scene.NodeElements.Last();


            #region check if we made a selection of a diferent node

            if (UID != selectedNode.UID)
            {

                matchingSelectedNodeData =
                    selectedInteraction.sceneData.FullCharacterDialogueSet.Find(n => n.UID == selectedNode.UID);
                matchingReflectedData = selectedInteraction.ReflectedDataSet.Find(r => r.UID == selectedNode.UID);

                if (selectedInteraction.GeneralReflectedData != null && selectedNode.CallingNode != null)
                    matchingGeneralReflectedData = selectedInteraction.GeneralReflectedData;

                UID = selectedNode.UID;


                //  matchingNodeDataSerializedObject = null;
            }

            #endregion




            #region if there is no matchingSelectedNodeData

            if (matchingSelectedNodeData == null)
            {
                EditorGUILayout.HelpBox("To edit this node in the inspector, push this scene to your SceneData", MessageType.Info);
                return;
            }

            #endregion


            Separator2();
            Separator3();

            if (showHelpMessage)
                EditorGUILayout.HelpBox(
                    "General Conditions are processed whenever this character is actively engaged in interaction in which it is carrying out any action",
                    MessageType.Info);

            selectedInteraction.ShowGeneralConditionsSettings =
                EditorGUILayout.Foldout(selectedInteraction.ShowGeneralConditionsSettings,
                    "General Conditions");

            GUILayout.Space(10);


            if (selectedInteraction.ShowGeneralConditionsSettings)
                DrawGeneralConditionCreator(selectedInteraction);


            if (matchingSelectedNodeData.useTime)
            {
                GUILayout.Space(15);
                Separator2();
                Separator3();



                GUILayout.BeginHorizontal();
                GUILayout.Label(" Auto Start All Conditions By Default");
                GUILayout.FlexibleSpace();
                var AutoStartAllConditionsByDefaultIcon = AutoStartAllConditionsByDefault.boolValue
                    ? ImageLibrary.PowerOnpro
                    : ImageLibrary.PowerOffpro;
                if (GUILayout.Button(AutoStartAllConditionsByDefaultIcon, GUIStyle.none, GUILayout.Width(15),
                    GUILayout.Height(15)))
                {
                    AutoStartAllConditionsByDefault.boolValue = !AutoStartAllConditionsByDefault.boolValue;
                    foreach (var condition in matchingReflectedData.Conditions)
                    {
                        Undo.RegisterCompleteObjectUndo(condition, "Condition");
                        condition.AutoStart = AutoStartAllConditionsByDefault.boolValue;
                    }
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.Space(15);
            Separator2();
            Separator3();

            #region Draw Selected Node name

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(matchingSelectedNodeData.CharacterName, Theme.GameBridgeSkin.customStyles[5]);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(matchingSelectedNodeData.Name, EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            #endregion

            GUILayout.Space(5);


            GUILayout.Space(10);


            #region node specific data

            selectedInteraction.ShowNodeSpecificSettings =
                EditorGUILayout.Foldout(selectedInteraction.ShowNodeSpecificSettings, "Show Node Specific Settings");

            GUILayout.Space(10);

            if (selectedInteraction.ShowNodeSpecificSettings)
            {
                // we check if the matchingSelectedNodeData is a charactrNodeData, if it is , we show the option for setting the IsInControl value
                if (matchingSelectedNodeData.type == typeof(CharacterNodeData))
                {
                    var character = (CharacterNodeData)matchingSelectedNodeData;

                    GUILayout.BeginHorizontal();

                    GUILayout.FlexibleSpace();
                    var state = character.IsInControl ? "Is In Control" : "Is Not In Control";
                    if (GUILayout.Button(state, GUILayout.Height(15)))
                    {
                        ControllingCharacterUID.stringValue = character.UID;
                        character.IsInControl = !character.IsInControl;
                        foreach (var dataset in character.NodeDataInMyChain)
                            dataset.IsInControl = character.IsInControl;
                        EditorUtility.SetDirty(selectedInteraction.sceneData);

                    }

                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.Space(5);

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.Box(character.IsInControl ? ImageLibrary.CrownOn : ImageLibrary.CrownOff,
                        EditorStyles.inspectorDefaultMargins);
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }


                if (matchingSelectedNodeData.type == typeof(EnvironmentNodeData))
                {


                }

                if (matchingSelectedNodeData.type == typeof(ActionNodeData))
                {
                    Separator3();

                    var action = (ActionNodeData)matchingSelectedNodeData;
                    GUILayout.Space(5);
                    action.LocalizedText[selectedInteraction.sceneData.LanguageIndex] = EditorGUILayout.TextArea(action.LocalizedText[selectedInteraction.sceneData.LanguageIndex], Theme.GameBridgeSkin.customStyles[7], GUILayout.Width(ScreenRect.width - 40), GUILayout.Height(100));
                    GUILayout.Space(5);
                    Separator();

                    Separator3();

                    if (selectedInteraction.UsesSoundffects)
                    {
                        GUILayout.Space(5);
                        GUILayout.BeginHorizontal();
                        var overrideUseDurationLengthForSoundEffectsPowerIcon =
                            matchingReflectedData.ActionSpecificData.OverrideUseSoundEffect
                                ? ImageLibrary.PowerOnpro
                                : ImageLibrary.PowerOffRed;
                        if (GUILayout.Button(overrideUseDurationLengthForSoundEffectsPowerIcon, GUIStyle.none,
                            GUILayout.Width(15), GUILayout.Height(15)))
                            matchingReflectedData.ActionSpecificData.OverrideUseSoundEffect =
                                !matchingReflectedData.ActionSpecificData.OverrideUseSoundEffect;
                        GUILayout.FlexibleSpace();
                        GUILayout.Label("Override Sound Effect");
                        GUILayout.EndHorizontal();
                        GUILayout.Space(5);

                        if (matchingReflectedData.ActionSpecificData.OverrideUseSoundEffect)
                            EditorGUILayout.HelpBox(
                                "The only override is to not use sound effects for this node in game",
                                MessageType.Warning);


                        EditorGUI.BeginDisabledGroup(matchingReflectedData.ActionSpecificData.OverrideUseSoundEffect);

                        GUILayout.Space(5);
                        action.LocalizedSoundEffects[selectedInteraction.sceneData.LanguageIndex] = (AudioClip)EditorGUILayout.ObjectField("Sound Effect", action.LocalizedSoundEffects[selectedInteraction.sceneData.LanguageIndex],
                            typeof(AudioClip), false);
                        GUILayout.Space(5);
                        Separator();

                        /*   GUILayout.Space(5);
                           GUILayout.BeginHorizontal();
                           GUILayout.Label("Use Duration Length For Soundeffects");
                           GUILayout.FlexibleSpace();
                           var useDurationLengthForSoundEffectsPowerIcon = action.useDurationLengthForSoundEffects ? ImageLibrary.PowerOnpro : ImageLibrary.PowerOffpro;
                           if (GUILayout.Button(useDurationLengthForSoundEffectsPowerIcon, GUIStyle.none, GUILayout.Width(15), GUILayout.Height(15)))
                           {
                               action.useDurationLengthForSoundEffects = !action.useDurationLengthForSoundEffects;
                               action.useSoundEffectLength = !action.useSoundEffectLength;
                           }
                           GUILayout.EndHorizontal();
                           GUILayout.Space(5);
                           Separator2();
   
                           GUILayout.Space(5);
                           GUILayout.BeginHorizontal();
                           GUILayout.Label("Use Sound Effect Length");
                           GUILayout.FlexibleSpace();
                           var useSoundEffectLengthPowerIcon = action.useSoundEffectLength ? ImageLibrary.PowerOnpro : ImageLibrary.PowerOffpro;
                           if (GUILayout.Button(useSoundEffectLengthPowerIcon, GUIStyle.none, GUILayout.Width(15), GUILayout.Height(15)))
                           {
                               action.useSoundEffectLength = !action.useSoundEffectLength;
                               action.useDurationLengthForSoundEffects = !action.useDurationLengthForSoundEffects;
   
                           }
                           GUILayout.EndHorizontal();
                           GUILayout.Space(5);
   
                           if (action.useSoundEffectLength)
                           {
                               if (action.SoundEffect != null)
                               {
                                   Separator2();
                                   GUILayout.Space(5);
                                   GUILayout.BeginHorizontal();
                                   GUILayout.Label("Soundeffect Duration");
                                   GUILayout.FlexibleSpace();
                                   GUILayout.Label(action.SoundEffect.length.ToString(), GUILayout.Width(100));
                                   GUILayout.EndHorizontal();
                                   GUILayout.Space(5);
                               }
                               else
                               {
                                   EditorGUILayout.HelpBox("This node data does not use any voice recordings", MessageType.Info);
                               }
                           }
                           Separator();*/

                        EditorGUI.EndDisabledGroup();
                    }

                    Separator3();

                    if (showHelpMessage)
                        EditorGUILayout.HelpBox("The Start time, Duration and Delay here are a reflection of the start time, duration and delay of the selected node", MessageType.Info);
                    EditorGUILayout.LabelField("Start Time", action.StartTime.ToString());
                    EditorGUILayout.LabelField("Duration", action.Duration.ToString());
                    EditorGUILayout.LabelField("Delay", action.Delay.ToString());
                    if (showHelpMessage)
                        EditorGUILayout.HelpBox("Realtime delay is set at runtime. it is recommended that you use Realtime dealy instead of Delay", MessageType.Info);
                    EditorGUILayout.LabelField("Realtime Delay", action.RealtimeDelay.ToString());
                    EditorGUILayout.HelpBox("Realtime delay is set at runtime. it is recommended that you use Realtime dealy instead of Delay", MessageType.Info);
                    // EditorGUILayout.LabelField("Realtime Delay", action.RealtimeDelay.ToString());
                }

                if (matchingSelectedNodeData.type == typeof(DialogueNodeData))
                {
                    var dialogue = (DialogueNodeData)matchingSelectedNodeData;


                    // if (selectedInteraction.UsesText)
                    //   {
                    /*  GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                var overrideTextDisplayMethodPowerIcon = dialogue.OverrideTextDisplayMethod ? ImageLibrary.PowerOnpro : ImageLibrary.PowerOffRed;
                if (GUILayout.Button(overrideTextDisplayMethodPowerIcon, GUIStyle.none, GUILayout.Width(15), GUILayout.Height(15)))
                    dialogue.OverrideTextDisplayMethod = !dialogue.OverrideTextDisplayMethod;

                GUILayout.FlexibleSpace();
                GUILayout.Label("Override Text Setting");
                GUILayout.EndHorizontal();
                GUILayout.Space(5);

          if (dialogue.OverrideTextDisplayMethod)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Text Display Mode");
                    dialogue.OverridenCharacterTextDisplayMode = (CharacterTextDisplayMode)EditorGUILayout.EnumPopup(dialogue.OverridenCharacterTextDisplayMode, GUILayout.Height(15), GUILayout.Width(150));
                    GUILayout.EndHorizontal();

                    switch (dialogue.OverridenCharacterTextDisplayMode)
                    {
                        case CharacterTextDisplayMode.Instant:

                            break;
                        case CharacterTextDisplayMode.Typed:
                            GUILayout.Space(5);

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Typing Speed");
                            GUILayout.FlexibleSpace();
                            selectedInteraction.TypingSpeed = EditorGUILayout.IntField(selectedInteraction.TypingSpeed, GUILayout.Height(15), GUILayout.Width(150));
                            GUILayout.EndHorizontal();


                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Delay");
                            GUILayout.FlexibleSpace();
                            selectedInteraction.Delay = EditorGUILayout.FloatField(selectedInteraction.Delay, GUILayout.Height(15), GUILayout.Width(150));
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Typing Sound");
                            GUILayout.FlexibleSpace();
                            selectedInteraction.TypingAudioCip = (AudioClip)EditorGUILayout.ObjectField(selectedInteraction.TypingAudioCip, typeof(AudioClip), false, GUILayout.Height(15), GUILayout.Width(150));
                            GUILayout.EndHorizontal();

                            GUILayout.Space(5);



                            break;
                        case CharacterTextDisplayMode.Custom:

                            break;
                    }
                } */


                    //  }

                    Separator3();
                    GUILayout.Space(10);

                    GUILayout.BeginHorizontal();

                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(new GUIContent(!Theme.GameBridgeSkin.customStyles[7].richText ? "Markup Edit: <color=green>On</color>" : "Markup Edit: <color=#ff0033>Off</color>", ImageLibrary.editIcon, "Do a quick markup edit"), Theme.GameBridgeSkin.button, GUILayout.Width(110), GUILayout.Height(20)))
                    {
                        Theme.GameBridgeSkin.customStyles[7].richText = !Theme.GameBridgeSkin.customStyles[7].richText;
                    }

                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button(new GUIContent("Open Editor", ImageLibrary.MarkupIcon, "Edit in Markup Editor"), Theme.GameBridgeSkin.button, GUILayout.Width(110), GUILayout.Height(20)))
                    {
                        var markupWindow = EditorWindow.GetWindow<MarkupEditor>();
                        markupWindow.sceneData = selectedInteraction.sceneData;
                        markupWindow.TargetNodeData = dialogue;

                    }

                    GUILayout.FlexibleSpace();

                    GUILayout.EndHorizontal();


                    dialogue.LocalizedText[selectedInteraction.sceneData.LanguageIndex] =
                        EditorGUILayout.TextArea(dialogue.LocalizedText[selectedInteraction.sceneData.LanguageIndex], Theme.GameBridgeSkin.customStyles[7], GUILayout.Width(ScreenRect.width - 40), GUILayout.Height(100));
                    GUILayout.Space(5);
                    Separator();
                    Separator3();

                    if (selectedInteraction.UsesVoiceOver)
                    {
                        GUILayout.Space(5);
                        GUILayout.BeginHorizontal();
                        var overrideUseDurationLengthForVoiceOverPowerIcon =
                            matchingReflectedData.DialogueSpecificData.OverrideUseVoiceover
                                ? ImageLibrary.PowerOnpro
                                : ImageLibrary.PowerOffRed;
                        if (GUILayout.Button(overrideUseDurationLengthForVoiceOverPowerIcon, GUIStyle.none,
                            GUILayout.Width(15), GUILayout.Height(15)))
                            matchingReflectedData.DialogueSpecificData.OverrideUseVoiceover =
                                !matchingReflectedData.DialogueSpecificData.OverrideUseVoiceover;
                        GUILayout.FlexibleSpace();
                        GUILayout.Label("Override Voiceover");
                        GUILayout.EndHorizontal();
                        GUILayout.Space(5);

                        if (matchingReflectedData.DialogueSpecificData.OverrideUseVoiceover)
                            EditorGUILayout.HelpBox("The only override is to not use voice for this node in game",
                                MessageType.Warning);


                        EditorGUI.BeginDisabledGroup(matchingReflectedData.DialogueSpecificData.OverrideUseVoiceover);
                        GUILayout.Space(5);
                        dialogue.LocalizedVoiceRecordings[selectedInteraction.sceneData.LanguageIndex] = (AudioClip)EditorGUILayout.ObjectField("Voice clip",
                            dialogue.LocalizedVoiceRecordings[selectedInteraction.sceneData.LanguageIndex], typeof(AudioClip), false);
                        GUILayout.Space(5);
                        Separator();


                        /* GUILayout.Space(5);
                         GUILayout.BeginHorizontal();
                         GUILayout.Label("Use Duration Length For Voice Over");
                         GUILayout.FlexibleSpace();
                         var useDurationLengthForVoiceOverPowerIcon = dialogue.useDurationLengthForVoiceOver ? ImageLibrary.PowerOnpro : ImageLibrary.PowerOffpro;
                         if (GUILayout.Button(useDurationLengthForVoiceOverPowerIcon, GUIStyle.none, GUILayout.Width(15), GUILayout.Height(15)))
                         {
                             dialogue.useDurationLengthForVoiceOver = !dialogue.useDurationLengthForVoiceOver;
                             dialogue.useVoiceOverLength = !dialogue.useVoiceOverLength;
 
 
                         }
                         GUILayout.EndHorizontal();
                         GUILayout.Space(5);
                         Separator2();
 
 
                         GUILayout.Space(5);
                         GUILayout.BeginHorizontal();
                         GUILayout.Label("Use Voiceover Length");
                         GUILayout.FlexibleSpace();
                         var useVoiceOverLengthPowerIcon = dialogue.useVoiceOverLength ? ImageLibrary.PowerOnpro : ImageLibrary.PowerOffpro;
                         if (GUILayout.Button(useVoiceOverLengthPowerIcon, GUIStyle.none, GUILayout.Width(15), GUILayout.Height(15)))
                         {
                             dialogue.useVoiceOverLength = !dialogue.useVoiceOverLength;
                             dialogue.useDurationLengthForVoiceOver = !dialogue.useDurationLengthForVoiceOver;
 
                         }
                         GUILayout.EndHorizontal();
                         GUILayout.Space(5);
 
                         if (dialogue.useVoiceOverLength)
                         {
                             if (dialogue.VoicedDialogue != null)
                             {
                                 Separator2();
                                 GUILayout.Space(5);
                                 GUILayout.BeginHorizontal();
                                 GUILayout.Label("Voiceover Duration");
                                 GUILayout.FlexibleSpace();
                                 GUILayout.Label(dialogue.VoicedDialogue.length.ToString(), GUILayout.Width(100));
                                 GUILayout.EndHorizontal();
                                 GUILayout.Space(5);
                             }
                             else
                             {
                                 EditorGUILayout.HelpBox("This node data does not use any voice recordings", MessageType.Info);
                             }
                         }
 
                         Separator();*/

                        EditorGUI.EndDisabledGroup();
                    }


                    // GUILayout.Box(AssetPreview.GetAssetPreview(dialogue.VoicedDialogue),EditorStyles.inspectorDefaultMargins);

                    if (selectedInteraction.UsesSoundffects)
                    {
                        GUILayout.Space(5);
                        GUILayout.BeginHorizontal();
                        var overrideUseDurationLengthForSoundEffectsPowerIcon =
                            matchingReflectedData.DialogueSpecificData.OverrideUseSoundEffect
                                ? ImageLibrary.PowerOnpro
                                : ImageLibrary.PowerOffRed;
                        if (GUILayout.Button(overrideUseDurationLengthForSoundEffectsPowerIcon, GUIStyle.none,
                            GUILayout.Width(15), GUILayout.Height(15)))
                            matchingReflectedData.DialogueSpecificData.OverrideUseSoundEffect =
                                !matchingReflectedData.DialogueSpecificData.OverrideUseSoundEffect;
                        GUILayout.FlexibleSpace();
                        GUILayout.Label("Override Sound Effect");
                        GUILayout.EndHorizontal();
                        GUILayout.Space(5);

                        if (matchingReflectedData.DialogueSpecificData.OverrideUseSoundEffect)
                            EditorGUILayout.HelpBox(
                                "The only override is to not use sound effects for this node in game",
                                MessageType.Warning);

                        EditorGUI.BeginDisabledGroup(matchingReflectedData.DialogueSpecificData.OverrideUseSoundEffect);

                        GUILayout.Space(5);
                        dialogue.LocalizedSoundEffects[selectedInteraction.sceneData.LanguageIndex] = (AudioClip)EditorGUILayout.ObjectField("Sound Effect",
                            dialogue.LocalizedSoundEffects[selectedInteraction.sceneData.LanguageIndex], typeof(AudioClip), false);
                        GUILayout.Space(5);
                        Separator();

                        /* GUILayout.Space(5);
                         GUILayout.BeginHorizontal();
                         GUILayout.Label("Use Duration Length For Soundeffects");
                         GUILayout.FlexibleSpace();
                         var useDurationLengthForSoundEffectsPowerIcon = dialogue.useDurationLengthForSoundEffects ? ImageLibrary.PowerOnpro : ImageLibrary.PowerOffpro;
                         if (GUILayout.Button(useDurationLengthForSoundEffectsPowerIcon, GUIStyle.none, GUILayout.Width(15), GUILayout.Height(15)))
                         {
                             dialogue.useDurationLengthForSoundEffects = !dialogue.useDurationLengthForSoundEffects;
                             dialogue.useSoundEffectLength = !dialogue.useSoundEffectLength;
                         }
                         GUILayout.EndHorizontal();
                         GUILayout.Space(5);
                         Separator2();
 
                         GUILayout.Space(5);
                         GUILayout.BeginHorizontal();
                         GUILayout.Label("Use Sound Effect Length");
                         GUILayout.FlexibleSpace();
                         var useSoundEffectLengthPowerIcon = dialogue.useSoundEffectLength ? ImageLibrary.PowerOnpro : ImageLibrary.PowerOffpro;
                         if (GUILayout.Button(useSoundEffectLengthPowerIcon, GUIStyle.none, GUILayout.Width(15), GUILayout.Height(15)))
                         {
                             dialogue.useSoundEffectLength = !dialogue.useSoundEffectLength;
                             dialogue.useDurationLengthForSoundEffects = !dialogue.useDurationLengthForSoundEffects;
 
                         }
                         GUILayout.EndHorizontal();
                         GUILayout.Space(5);
 
                         if (dialogue.useSoundEffectLength)
                         {
                             if (dialogue.SoundEffect != null)
                             {
                                 Separator2();
                                 GUILayout.Space(5);
                                 GUILayout.BeginHorizontal();
                                 GUILayout.Label("Soundeffect Duration");
                                 GUILayout.FlexibleSpace();
                                 GUILayout.Label(dialogue.SoundEffect.length.ToString(), GUILayout.Width(100));
                                 GUILayout.EndHorizontal();
                                 GUILayout.Space(5);
                             }
                             else
                             {
                                 EditorGUILayout.HelpBox("This node data does not use any voice recordings", MessageType.Info);
                             }
                         }
                         Separator();
                         */
                        EditorGUI.EndDisabledGroup();
                    }

                    Separator3();

                    GUILayout.Space(5);
                    if (showHelpMessage)
                        EditorGUILayout.HelpBox("The Start time, Duration and Delay here are a reflection of the start time, duration and delay of the selected node", MessageType.Info);
                    EditorGUILayout.LabelField("Start Time", dialogue.StartTime.ToString());
                    EditorGUILayout.LabelField("Duration", dialogue.Duration.ToString());
                    EditorGUILayout.LabelField("Delay", dialogue.Delay.ToString());
                    EditorGUILayout.LabelField("Realtime Delay", dialogue.RealtimeDelay.ToString());
                    if (showHelpMessage)
                        EditorGUILayout.HelpBox("Realtime delay is set at runtime. it is recommended that you use Realtime dealy instead of Delay", MessageType.Info);
                    GUILayout.Space(5);
                    Separator();

                    // EditorGUILayout.LabelField("Realtime Delay", dialogue.RealtimeDelay.ToString());


                }

                if (matchingSelectedNodeData.type == typeof(RouteNodeData))
                {
                    var route = (RouteNodeData)matchingSelectedNodeData;

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Alternate Route Titles (" + selectedInteraction.sceneData.Languages[selectedInteraction.sceneData.LanguageIndex].Name + ")");
                    GUILayout.FlexibleSpace();
                    var usesRouteTitlesIcon = matchingReflectedData.RouteSpecificDataset.UseAlternativeRouteTitles
                        ? ImageLibrary.PowerOnpro
                        : ImageLibrary.PowerOffpro;
                    if (GUILayout.Button(usesRouteTitlesIcon, GUIStyle.none, GUILayout.Width(15),
                        GUILayout.Height(15)))
                        matchingReflectedData.RouteSpecificDataset.UseAlternativeRouteTitles =
                            !matchingReflectedData.RouteSpecificDataset.UseAlternativeRouteTitles;
                    GUILayout.EndHorizontal();


                    GUILayout.Space(5);


                    if (matchingReflectedData.RouteSpecificDataset.UseAlternativeRouteTitles)
                    {


                        if (matchingReflectedData.RouteSpecificDataset.LanguageSpecificData.Count != selectedInteraction.sceneData.Languages.Count)
                            matchingReflectedData.RouteSpecificDataset.LanguageSpecificData.Resize(selectedInteraction.sceneData.Languages.Count);

                        if (matchingReflectedData.RouteSpecificDataset.LanguageSpecificData[selectedInteraction.sceneData.LanguageIndex] == null)
                            matchingReflectedData.RouteSpecificDataset.LanguageSpecificData[selectedInteraction.sceneData.LanguageIndex] = new ReflectedData.LanguageSpecificDataForRouteNodeData();

                        if (matchingReflectedData.RouteSpecificDataset.LanguageSpecificData[selectedInteraction.sceneData.LanguageIndex].RouteTitles.Count !=
                            route.DataIconnectedTo.Count)
                            matchingReflectedData.RouteSpecificDataset.LanguageSpecificData[selectedInteraction.sceneData.LanguageIndex].RouteTitles.Resize(
                                route.DataIconnectedTo.Count);

                        for (var i = 0; i < route.DataIconnectedTo.Count; i++)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label(route.DataIconnectedTo[i].Name, GUILayout.Width(100));
                            GUILayout.FlexibleSpace();
                            matchingReflectedData.RouteSpecificDataset.LanguageSpecificData[selectedInteraction.sceneData.LanguageIndex].RouteTitles[i] =
                                EditorGUILayout.DelayedTextField(
                                    matchingReflectedData.RouteSpecificDataset.LanguageSpecificData[selectedInteraction.sceneData.LanguageIndex].RouteTitles[i],
                                    GUILayout.Height(15));
                            GUILayout.Space(2);
                            GUILayout.EndHorizontal();
                        }
                    }

                    GUILayout.Space(5);
                    Separator();



                }

                if (matchingSelectedNodeData.type == typeof(LinkNodeData))
                {

                }
                //   var link = (LinkNodeData)matchingSelectedNodeData;

                if (matchingSelectedNodeData.type == typeof(EndNodeData))
                {

                }
                //   var end = (EndNodeData)matchingSelectedNodeData;


                #region Draw Node Specific Conditions
                if (matchingSelectedNodeData.type != typeof(CharacterNodeData))
                {
                    Separator3();

                    GUILayout.Space(10);
                    ShowNodeSpecificConditionSettings.boolValue = EditorGUILayout.Foldout(ShowNodeSpecificConditionSettings.boolValue, "Node Spefic Conditions");
                    if (ShowNodeSpecificConditionSettings.boolValue)
                        DrawConditionCreator(selectedInteraction);
                    GUILayout.Space(5);
                    Separator();
                }

                #endregion
            }


            #endregion

            GUILayout.Space(35);



            var helpMessageButtonArea = GUILayoutUtility.GetLastRect().ToLowerLeft(150, 20);
            showHelpMessage = GUI.Toggle(helpMessageButtonArea, showHelpMessage, "Show help messages");

            serializedObject.ApplyModifiedProperties();
        }



        private void DoSetup()
        {

            selectedInteraction.transform.name = "Interaction System";

            if (!ControllingCharacterUID.stringValue.Equals(""))
            {
                #region find the character node which has the matching UID and use reset the IsInControl value
                var theCharacterNodeData = selectedInteraction.sceneData.Characters.Find(c => c.UID == ControllingCharacterUID.stringValue);

                theCharacterNodeData.IsInControl = true;
                foreach (var dataset in theCharacterNodeData.NodeDataInMyChain)
                    dataset.IsInControl = theCharacterNodeData.IsInControl;
                EditorUtility.SetDirty(selectedInteraction.sceneData);
                #endregion
            }

            #region setup General Reflected Data

            #region Generate and setup GeneralReflectedData Parent

            if (selectedInteraction.GeneralReflectedData == null)
            {
                selectedInteraction.GeneralReflectedDataParent = new GameObject("General Reflected Data");
                selectedInteraction.GeneralReflectedDataParent.transform.SetParent(selectedInteraction.transform);
                selectedInteraction.GeneralReflectedDataParent.transform.localPosition = Vector3.zero;
                selectedInteraction.GeneralReflectedDataParent.hideFlags = HideFlags.HideInHierarchy;
            }
            else
            {
                selectedInteraction.TempGeneralReflectedDataSet = selectedInteraction.GeneralReflectedData;
            }

            #endregion

            #region create a new instance of ReflectedData as a gameObject and then assign the GeneralReflectedData value at i to the reflected data ID

            var newGeneralReflectedDatagameObject = new GameObject("General Reflected");
            newGeneralReflectedDatagameObject.transform.SetParent(selectedInteraction.GeneralReflectedDataParent
                .transform);
            newGeneralReflectedDatagameObject.AddComponent<ReflectedData>();
            var theGeneralReflectedDataComponent = newGeneralReflectedDatagameObject.GetComponent<ReflectedData>();
            theGeneralReflectedDataComponent.InteractionGameObject = selectedInteraction.gameObject;
            theGeneralReflectedDataComponent.interaction = selectedInteraction;
            theGeneralReflectedDataComponent.self = newGeneralReflectedDatagameObject;
            // we already resized the ReflectedDataSet list to be the same size as the SortedList so we dont use .Add
            selectedInteraction.GeneralReflectedData = theGeneralReflectedDataComponent;
            // it is VERY important that the UIDs match.
            /// selectedInteraction.GeneralReflectedData.General = true;
            // selectedInteraction.GeneralReflectedData.UID = selectedInteraction.UID;

            #endregion


            #region Add the first general condition

            var newGeneralCondition = new GameObject(newGeneralReflectedDatagameObject.name + "Condition " +
                                                     theGeneralReflectedDataComponent.Conditions.Count);
            newGeneralCondition.AddComponent<Condition>();
            var _generalCondition = newGeneralCondition.GetComponent<Condition>();
            _generalCondition.InteractionGameObject = selectedInteraction.gameObject;
            _generalCondition.interaction = selectedInteraction;
            _generalCondition.Self = newGeneralCondition;
            newGeneralCondition.transform.SetParent(newGeneralReflectedDatagameObject.transform);
            // newCondition.hideFlags = HideFlags.HideInHierarchy;
            theGeneralReflectedDataComponent.Conditions.Add(newGeneralCondition.GetComponent<Condition>());

            #endregion

            if (selectedInteraction.TempGeneralReflectedDataSet != null)
            {
                //selectedInteraction.TempGeneralReflectedDataSet = null;
                // DestroyImmediate(theGeneralReflectedDataComponent, true);

                var data = selectedInteraction.GeneralReflectedData;
                if (selectedInteraction.TempGeneralReflectedDataSet.UID == data.UID)
                {
                    data.InteractionGameObject = selectedInteraction.TempGeneralReflectedDataSet.InteractionGameObject;
                    data.interaction = selectedInteraction.TempGeneralReflectedDataSet.interaction;
                    data.interactionComponent = selectedInteraction.TempGeneralReflectedDataSet.interactionComponent;


                    for (var c = 0; c < data.Conditions.Count; c++)
                    {
                        var conditionToDelete = data.Conditions[c];
                        DestroyImmediate(conditionToDelete.Self);
                        data.Conditions.RemoveAt(c);
                    }

                    // finally we move the condition from TempReflectedDataSet[i] conditions to the datas condition list
                    foreach (var condition in selectedInteraction.TempGeneralReflectedDataSet.Conditions)
                    {
                        condition.Self.transform.SetParent(data.self.transform);
                        data.Conditions.Add(condition);
                    }
                }

                DestroyImmediate(selectedInteraction.TempGeneralReflectedDataSet.self);
                selectedInteraction.TempGeneralReflectedDataSet = null;
            }

            #endregion

            #region Generate and setup Reflected Data parent
            if (selectedInteraction.ReflectedDataSet.Count == 0)
            {
                selectedInteraction.ReflectedDataSet.Resize(
                    selectedInteraction.sceneData.FullCharacterDialogueSet.Count);

                selectedInteraction.ReflectedDataParent = new GameObject("Reflected Data");
                selectedInteraction.ReflectedDataParent.transform.SetParent(selectedInteraction.transform);
                selectedInteraction.ReflectedDataParent.transform.localPosition = Vector3.zero;
                selectedInteraction.ReflectedDataParent.hideFlags = HideFlags.HideInHierarchy;


                var AudioManager = new GameObject("Audio Manager");
                AudioManager.transform.SetParent(selectedInteraction.transform);
                AudioManager.transform.localPosition = Vector3.zero;

                var TypingAudioManager = new GameObject("Typing");
                TypingAudioManager.transform.SetParent(AudioManager.transform);
                TypingAudioManager.transform.localPosition = Vector3.zero;
                TypingAudioManager.AddComponent<AudioSource>();
                selectedInteraction.TypingAudioSource = TypingAudioManager.GetComponent<AudioSource>();


                selectedInteraction.VoiceAudioSources.Resize(selectedInteraction.sceneData.Characters.Count);
                selectedInteraction.SoundEffectAudioSources.Resize(selectedInteraction.sceneData.Characters.Count);

                for (int v = 0; v < selectedInteraction.sceneData.Characters.Count; v++)
                {
                    var targetCharacter = selectedInteraction.sceneData.Characters[v];

                    var VoiceAudioManager = new GameObject(targetCharacter.Name + "Voice");
                    VoiceAudioManager.transform.SetParent(AudioManager.transform);
                    VoiceAudioManager.transform.localPosition = Vector3.zero;
                    VoiceAudioManager.AddComponent<AudioSource>();
                    selectedInteraction.VoiceAudioSources[v] = VoiceAudioManager.GetComponent<AudioSource>();

                    var SoundEffectsAudioManager = new GameObject(targetCharacter.Name + "Sound Effects");
                    SoundEffectsAudioManager.transform.SetParent(AudioManager.transform);
                    SoundEffectsAudioManager.transform.localPosition = Vector3.zero;
                    SoundEffectsAudioManager.AddComponent<AudioSource>();
                    selectedInteraction.SoundEffectAudioSources[v] = SoundEffectsAudioManager.GetComponent<AudioSource>();
                }

                /*  var VoiceAudioManager = new GameObject("Voice");
                  VoiceAudioManager.transform.SetParent(AudioManager.transform);
                  VoiceAudioManager.transform.localPosition = Vector3.zero;
                  VoiceAudioManager.AddComponent<AudioSource>();
                  selectedInteraction.VoiceAudioSource = VoiceAudioManager.GetComponent<AudioSource>();

                  var SoundEffectsAudioManager = new GameObject("Sound Effects");
                  SoundEffectsAudioManager.transform.SetParent(AudioManager.transform);
                  SoundEffectsAudioManager.transform.localPosition = Vector3.zero;
                  SoundEffectsAudioManager.AddComponent<AudioSource>();
                  selectedInteraction.SoundEffectAudioSource = SoundEffectsAudioManager.GetComponent<AudioSource>();*/
            }
            else
            {
                // we cache the current set of reflected data in a temporary list and then empry and resize the reflecteddataset list
                selectedInteraction.TempReflectedDataSet = new List<ReflectedData>();

                foreach (var capturedData in selectedInteraction.ReflectedDataSet)
                    selectedInteraction.TempReflectedDataSet.Add(capturedData);

                selectedInteraction.ReflectedDataSet = new List<ReflectedData>();
                selectedInteraction.ReflectedDataSet.Resize(
                    selectedInteraction.sceneData.FullCharacterDialogueSet.Count);
            }
            #endregion

            #region Setup reflected data for all individual nodes
            // loop through the sorted list
            for (var i = 0; i < selectedInteraction.sceneData.FullCharacterDialogueSet.Count; i++)
            {
                #region create a new instance of ReflectedData as a gameObject and then assign the FullCharacterDialogueSet value at i to the reflected data ID

                var newReflectedDatagameObject =
                    new GameObject(selectedInteraction.sceneData.FullCharacterDialogueSet[i].Name + "Reflected");
                newReflectedDatagameObject.transform.SetParent(selectedInteraction.ReflectedDataParent.transform);
                newReflectedDatagameObject.AddComponent<ReflectedData>();
                var theReflectedDataComponent = newReflectedDatagameObject.GetComponent<ReflectedData>();
                theReflectedDataComponent.InteractionGameObject = selectedInteraction.gameObject;
                theReflectedDataComponent.interaction = selectedInteraction;
                theReflectedDataComponent.self = newReflectedDatagameObject;

                // we already resized the ReflectedDataSet list to be the same size as the FullCharacterDialogueSet so we dont use .Add
                selectedInteraction.ReflectedDataSet[i] = theReflectedDataComponent;
                // it is VERY important that the UIDs match.
                selectedInteraction.ReflectedDataSet[i].UID =
                    selectedInteraction.sceneData.FullCharacterDialogueSet[i].UID;

                #endregion

                #region Add the first conditin

                var newCondition = new GameObject(newReflectedDatagameObject.name + "Condition " +
                                                  theReflectedDataComponent.Conditions.Count);
                newCondition.AddComponent<Condition>();
                var _condition = newCondition.GetComponent<Condition>();
                _condition.InteractionGameObject = selectedInteraction.gameObject;
                _condition.interaction = selectedInteraction;
                _condition.Self = newCondition;
                newCondition.transform.SetParent(newReflectedDatagameObject.transform);
                // newCondition.hideFlags = HideFlags.HideInHierarchy;
                theReflectedDataComponent.Conditions.Add(newCondition.GetComponent<Condition>());

                #endregion

                #region here we begin checking to see if any UID values we have for reflected data in the temp reflected data. if so , we destroy their conditions and replace the m with the conditions in the TempReflectedDataSet

                if (selectedInteraction.TempReflectedDataSet.Count != 0)
                    foreach (var tempData in selectedInteraction.TempReflectedDataSet)
                    {
                        //we can use ReflectedDataSet[i] because the sorted list count and ReflectedDataSet ount are the same
                        var data = selectedInteraction.ReflectedDataSet[i];

                        if (selectedInteraction.sceneData.FullCharacterDialogueSet[i].UID == tempData.UID)
                        {
                            data.InteractionGameObject = tempData.InteractionGameObject;
                            data.interaction = tempData.interaction;
                            data.interactionComponent = tempData.interactionComponent;
                            data.RouteSpecificDataset = tempData.RouteSpecificDataset;
                            data.ActionSpecificData = tempData.ActionSpecificData;
                            data.DialogueSpecificData = tempData.DialogueSpecificData;
                            data.LinkSpecificData = tempData.LinkSpecificData;
                            data.EndSpecificData = tempData.EndSpecificData;
                            data.EnvironmentSpecificData = tempData.EnvironmentSpecificData;

                            /* foreach (var dataset in data.dialoguer.sceneData.Characters[data.dialoguer.].NodeDataInMyChain)
                             {
                                 dataset.IsInControl  = tempData.dialoguer.sceneData.Characters[data.dialoguer.targetChararacterIndex].IsInControl;
                             }*/


                            for (var c = 0; c < data.Conditions.Count; c++)
                            {
                                var conditionToDelete = data.Conditions[c];
                                DestroyImmediate(conditionToDelete.Self);
                                data.Conditions.RemoveAt(c);
                            }

                            // finally we move the condition from TempReflectedDataSet[i] conditions to the datas condition list
                            foreach (var condition in tempData.Conditions)
                            {
                                condition.Self.transform.SetParent(data.self.transform);
                                data.Conditions.Add(condition);
                            }
                        }
                    }


                #endregion
            }



            // now destroy all the data in TempReflectedDataSet
            foreach (var item in selectedInteraction.TempReflectedDataSet) DestroyImmediate(item.self);
            selectedInteraction.TempReflectedDataSet.RemoveAll(n => n == null);
            #endregion


            selectedInteraction.CachedSceneData = selectedInteraction.sceneData;
            selectedInteraction.UpdateUID = selectedInteraction.sceneData.UpdateUID;


        }

        private void DrawGeneralConditionCreator(Interaction interaction)
        {

            if (matchingGeneralReflectedData == null) return;

            var generalConditionsCount = matchingGeneralReflectedData.Conditions.Count;
            if (GeneralConditionSpecificSpaceing.Count != generalConditionsCount)
                GeneralConditionSpecificSpaceing.Resize(generalConditionsCount);


            for (var c = 0; c < generalConditionsCount; c++)
            {
                var condition = matchingGeneralReflectedData.Conditions[c];

                var conditionSerializedObject = new SerializedObject(condition);
                conditionSerializedObject.Update();

                var area = EditorGUILayout.GetControlRect();

                // GUI.DrawTexture(area.AddRect(0,0,0,60), Textures.Gray);

                #region Background UI

                var eventCount = matchingGeneralReflectedData.Conditions[c].targetEvent.GetPersistentEventCount();
                var countSpacing = 0;
                var multiplyingValue = 0;
                var xpadding = 0;

#if UNITY_2021_3_OR_NEWER
                multiplyingValue = 49;
                xpadding = -12;
                countSpacing = eventCount < 2 ? 0 : (eventCount - 1) * multiplyingValue;
#elif UNITY_2020_2_OR_NEWER
                multiplyingValue = 47;
                xpadding = -12;
                countSpacing = eventCount < 2 ? 0 : (eventCount - 1) * multiplyingValue;


#elif UNITY_2019_3_OR_NEWER
                 multiplyingValue = 47;
                xpadding = -12;
                countSpacing = eventCount < 2 ? 0 : (eventCount - 1) * multiplyingValue;  

#elif UNITY_2019_2_OR_NEWER
                 multiplyingValue = 43;
                xpadding = 0;
                countSpacing = eventCount < 2 ? 0 : (eventCount - 1) * multiplyingValue;  

#else

                multiplyingValue = 43;
                xpadding = 0;
                countSpacing = eventCount < 2 ? 0 : (eventCount - 1) * multiplyingValue;

#endif

                /* var conditionBodyArea = area.ToUpperLeft(0, areaHeight + countSpacing);*/
                var conditionBodyArea = area.ToUpperLeft(0, 115 + GeneralConditionSpecificSpaceing[c] + countSpacing);
                GUI.Box(conditionBodyArea, "", Theme.GameBridgeSkin.customStyles[2]);
                GUI.DrawTexture(area.ToUpperLeft(0, 3, 0, 15), Textures.DuskLighter);
                var conditionBodyFooter = conditionBodyArea.PlaceUnder(0, 5);
                GUI.Box(conditionBodyFooter, "", Theme.GameBridgeSkin.customStyles[3]);
                var buttonArea = conditionBodyFooter.ToLowerRight(55, 14, xpadding, 14);
                GUI.Box(buttonArea, "", Theme.GameBridgeSkin.customStyles[4]);

                var addConditionButtonArea = buttonArea.ToCenterLeft(8, 8, 10);
                if (ClickEvent.Click(4, addConditionButtonArea, ImageLibrary.addConditionIcon, "Add a new condition below"))
                {
                    var newCondition =
                        new GameObject("General Condition " + generalConditionsCount);
                    newCondition.AddComponent<Condition>(); //.self = newCondition;
                    var _condition = newCondition.GetComponent<Condition>();
                    _condition.InteractionGameObject = interaction.gameObject;
                    _condition.interaction = interaction;
                    _condition.Self = newCondition;
                    newCondition.transform.SetParent(matchingGeneralReflectedData.transform);
                    // newCondition.hideFlags = HideFlags.HideInHierarchy;
                    //   matchingGeneralReflectedData.Conditions.Add(newCondition.GetComponent<Condition>());
                    matchingGeneralReflectedData.Conditions.Insert(c + 1, newCondition.GetComponent<Condition>());

                    GeneralConditionSpecificSpaceing.Add(0);
                }


                var deleteConditionButtonArea = buttonArea.ToCenterRight(8, 8, -10);
                if (c != 0)
                    if (ClickEvent.Click(4, deleteConditionButtonArea, ImageLibrary.deleteConditionIcon, "Delete this condition"))
                    {
                        DestroyImmediate(matchingGeneralReflectedData.Conditions[c].gameObject);
                        matchingGeneralReflectedData.Conditions.RemoveAt(c);
                        return;
                    }

                #endregion


                var moveConditionUpButtonArea = area.ToCenterLeft(15, 8, 10);
                if (ClickEvent.Click(1, moveConditionUpButtonArea, ImageLibrary.upArrow,
                    "Move this condtion up by one position"))
                {
                    if (c > 0)
                    {
                        var ConditionAtTop = matchingGeneralReflectedData.Conditions[c - 1];

                        matchingGeneralReflectedData.Conditions[c - 1] = null;
                        //  matchingGeneralReflectedData.Conditions[c] = null;

                        matchingGeneralReflectedData.Conditions[c - 1] = condition;
                        matchingGeneralReflectedData.Conditions[c] = ConditionAtTop;
                    }
                }


                var moveConditionDownButtonArea = moveConditionUpButtonArea.PlaceToRight(15, 0, 20);
                if (ClickEvent.Click(1, moveConditionDownButtonArea, ImageLibrary.downArrow,
                    "Move this condtion down by one position"))
                {
                    if (c != generalConditionsCount - 1)
                    {
                        var ConditionAtBottom = matchingGeneralReflectedData.Conditions[c + 1];

                        matchingGeneralReflectedData.Conditions[c + 1] = null;
                        // matchingGeneralReflectedData.Conditions[c] = null;

                        matchingGeneralReflectedData.Conditions[c + 1] = condition;
                        matchingGeneralReflectedData.Conditions[c] = ConditionAtBottom;
                    }
                }

                var copyAllConditionDataButtonArea = moveConditionDownButtonArea.PlaceToRight(12, 12, 30, -4);
                if (ClickEvent.Click(1, copyAllConditionDataButtonArea, ImageLibrary.CopyIcon, "Copy condition data (temporarily disabled)"))
                {
                    ConditionCopy.MakeCopy(condition, c);
                }

                var copyOnlyEventsButtonArea = copyAllConditionDataButtonArea.PlaceToRight(12, 12, 20);
                if (ClickEvent.Click(1, copyOnlyEventsButtonArea, ImageLibrary.CopyIcon,
                    "copy all event data only (temporarily disabled)"))
                {
                    /*var e = System.Delegate.CreateDelegate(typeof(UnityEngine.Events.UnityAction), condition.targetEvent.GetPersistentTarget(0),
                        condition.targetEvent.GetPersistentMethodName(0)) as UnityEngine.Events.UnityAction;
                    UnityEditor.Events.UnityEventTools.AddVoidPersistentListener(ConditionCopy.TargetEvent, e);*/
                    // ConditionCopy.MakeCopy(condition, true);/
                }


                var pasteDataButtonArea = copyOnlyEventsButtonArea.PlaceToRight(12, 12, 20);
                if (ClickEvent.Click(1, pasteDataButtonArea, ImageLibrary.PasteIcon, "Paste copied data"))
                {
                    Undo.RegisterCompleteObjectUndo(condition, "Condition");
                    ConditionCopy.Paste(condition, c);
                }

                var setRepeatArea = pasteDataButtonArea.PlaceToRight(18, 12, 20);
                if (ClickEvent.Click(1, setRepeatArea, condition.Repeat ? ImageLibrary.loopActiveIcon : ImageLibrary.loopInactiveIconWhite, "Toggle repeat"))
                {
                    Undo.RegisterCompleteObjectUndo(condition, "Condition");
                    condition.Repeat = !condition.Repeat;
                }

                if (ClickEvent.Click(1, area.ToUpperRight(15, 15, -10),
                        condition.Disabled ? ImageLibrary.PowerOffpro : ImageLibrary.PowerOnpro,
                        "Enable / Disable this condition"))
                    condition.Disabled = !condition.Disabled;

                if (!condition.AutoStart)
                {
                    condition.ConditionUpdateRate = (UpdateRate)EditorGUILayout.EnumPopup(condition.ConditionUpdateRate, GUILayout.Height(15));
                    GUILayout.Space(2);
                }

                EditorGUI.BeginDisabledGroup(condition.Disabled);

                var autoStartInfo = condition.AutoStart ? "Auto Start Is On" : "Auto Start Is Off";
                if (GUILayout.Button(autoStartInfo, GUILayout.Height(15)))
                    condition.AutoStart = !condition.AutoStart;

                // starting out 4 pixels must be added to the layout height

                #region if statement
                /*
                EditorGUI.BeginDisabledGroup(condition.AutoStart);
                // all the content inside here 2 added the y
                GUILayout.BeginHorizontal();

                GUILayout.FlexibleSpace();
                GUILayout.Label("If", GUILayout.Height(15));
                GUILayout.FlexibleSpace();

                GUILayout.EndHorizontal();


                //  condition.ComponentIndex = EditorGUILayout.Popup(condition.ComponentIndex,condition.Components, GUILayout.Height(15));


                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                condition.TargetGameObject = (GameObject)EditorGUILayout.ObjectField(condition.TargetGameObject,
                    typeof(GameObject), true, GUILayout.Height(15));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                if (condition.cachedTargetObject != condition.TargetGameObject)
                {
                    condition.Components = new Component[0];
                    condition.serializedMethods = new SerializableMethodInfo[0];

                    condition.SetComponent(0);
                    condition.SetMethod(0);
                    condition.cachedTargetObject = condition.TargetGameObject;
                }


                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Box(ImageLibrary.DownFlowArrow, EditorStyles.label, GUILayout.Width(15),
                    GUILayout.Height(15));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();


                var disabledComponents = condition.TargetGameObject == null;
                EditorGUI.BeginDisabledGroup(disabledComponents);

                if (disabledComponents &&
                    (condition.Components.Count() != 0 || condition.serializedMethods.Count() != 0))
                {
                    condition.Components = new Component[0];
                    condition.serializedMethods = new SerializableMethodInfo[0];
                }

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();


                var componentName = "";
                if (!condition.Components.Any())
                    componentName = "None";
                else
                {
                    if (condition.Components[condition.ComponentIndex] == null)
                    {
                        condition.Components = new Component[0];
                        condition.serializedMethods = new SerializableMethodInfo[0];
                        
                    }
                    else
                        componentName = condition.Components[condition.ComponentIndex].GetType().Name;
                }




                if (GUILayout.Button(componentName, EditorStyles.popup, GUILayout.MinWidth(100), GUILayout.Height(15)))
                {
                    condition.GetGameObjectComponents();
                    var menu = new GenericMenu();
                    for (var i = 0; i < condition.Components.Length; i++)
                        menu.AddItem(new GUIContent(condition.Components[i].GetType().Name),
                            condition.ComponentIndex.Equals(i), condition.SetComponent, i);
                    menu.ShowAsContext();
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Box(ImageLibrary.DownFlowArrow, EditorStyles.label, GUILayout.Width(15),
                    GUILayout.Height(15));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                var methodName = !condition.serializedMethods.Any()
                    ? "None"
                    : condition.serializedMethods[condition.MethodIndex].methodName;
                //  var disabledMethods = condition.TargetGameObject == null;


                if (GUILayout.Button(methodName, EditorStyles.popup, GUILayout.MinWidth(100), GUILayout.Height(15)))
                {
                    condition.GetComponentMethods();
                    var menu = new GenericMenu();
                    for (var i = 0; i < condition.serializedMethods.Length; i++)
                        menu.AddItem(new GUIContent(condition.serializedMethods[i].methodInfo.Name),
                            condition.MethodIndex.Equals(i), condition.SetMethod, i);
                    menu.ShowAsContext();
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();


                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Box(ImageLibrary.DownEqualSign, EditorStyles.label, GUILayout.Width(15),
                    GUILayout.Height(15));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                var buttonState = condition.ObjectiveBool ? "True" : "False";
                if (GUILayout.Button(buttonState, GUILayout.Height(15)))
                    condition.ObjectiveBool = !condition.ObjectiveBool;
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                EditorGUI.EndDisabledGroup();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Box(ImageLibrary.DownFlowArrow, EditorStyles.label, GUILayout.Width(15),
                    GUILayout.Height(15));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                EditorGUI.EndDisabledGroup();



#if UNITY_2020_2_OR_NEWER
          
                if (condition.AutoStart)
                    GeneralConditionSpecificSpaceing[c] = 184;
                else
                    GeneralConditionSpecificSpaceing[c] = 201;

#elif UNITY_2019_3_OR_NEWER
                if (condition.AutoStart)
                    GeneralConditionSpecificSpaceing[c] = 184;
                else
                    GeneralConditionSpecificSpaceing[c] = 201;

#elif UNITY_2019_2_OR_NEWER
                 if (condition.AutoStart)
                    GeneralConditionSpecificSpaceing[c] = 174;
                else
                    GeneralConditionSpecificSpaceing[c] = 191;  

#else


                if (condition.AutoStart)
                    GeneralConditionSpecificSpaceing[c] = 174;
                else
                    GeneralConditionSpecificSpaceing[c] = 191;  

#endif


                */
                #endregion



                if (condition.BoolChecks.Count == 0)
                    condition.BoolChecks.Add(new BoolCheckSystem());


                for (int m = 0; m < condition.BoolChecks.Count; m++)
                {
                    var boolCheck = condition.BoolChecks[m];

                    #region if statement
                    EditorGUI.BeginDisabledGroup(condition.AutoStart);
                    // all the content inside here 2 added the y
                    GUILayout.BeginHorizontal();

                    GUILayout.FlexibleSpace();
                    GUILayout.Label("If", GUILayout.Height(15));
                    GUILayout.FlexibleSpace();

                    GUILayout.EndHorizontal();


                    //  condition.ComponentIndex = EditorGUILayout.Popup(condition.ComponentIndex,condition.Components, GUILayout.Height(15));


                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    boolCheck.TargetGameObject = (GameObject)EditorGUILayout.ObjectField(boolCheck.TargetGameObject,
                        typeof(GameObject), true, GUILayout.Height(15));
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    if (boolCheck.cachedTargetObject != boolCheck.TargetGameObject)
                    {
                        boolCheck.Components = new Component[0];
                        boolCheck.serializedMethods = new SerializableMethodInfo[0];

                        boolCheck.SetComponent(0);
                        boolCheck.SetMethod(0);
                        boolCheck.cachedTargetObject = boolCheck.TargetGameObject;
                    }


                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.Box(ImageLibrary.DownFlowArrow, EditorStyles.label, GUILayout.Width(15),
                        GUILayout.Height(15));
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();


                    var disabledComponents = boolCheck.TargetGameObject == null;
                    EditorGUI.BeginDisabledGroup(disabledComponents);

                    if (disabledComponents &&
                        (boolCheck.Components.Count() != 0 || boolCheck.serializedMethods.Count() != 0))
                    {
                        boolCheck.Components = new Component[0];
                        boolCheck.serializedMethods = new SerializableMethodInfo[0];
                    }

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    /*  var componentName = !condition.Components.Any()
                          ? "None"
                          : condition.Components[condition.ComponentIndex].GetType().Name;*/

                    var componentName = "";
                    if (!boolCheck.Components.Any())
                        componentName = "None";
                    else
                    {
                        if (boolCheck.Components[boolCheck.ComponentIndex] == null)
                        {
                            boolCheck.Components = new Component[0];
                            boolCheck.serializedMethods = new SerializableMethodInfo[0];

                        }
                        else
                            componentName = boolCheck.Components[boolCheck.ComponentIndex].GetType().Name;
                    }

                    /*  var componentName = !condition.Components.Any()
          ? "None"
          : condition.Components[condition.ComponentIndex].GetType().ToString();*/


                    if (GUILayout.Button(componentName, EditorStyles.popup, GUILayout.MinWidth(100), GUILayout.Height(15)))
                    {
                        boolCheck.GetGameObjectComponents();
                        var menu = new GenericMenu();
                        for (var i = 0; i < boolCheck.Components.Length; i++)
                            menu.AddItem(new GUIContent(boolCheck.Components[i].GetType().Name),
                                boolCheck.ComponentIndex.Equals(i), boolCheck.SetComponent, i);
                        menu.ShowAsContext();
                    }

                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.Box(ImageLibrary.DownFlowArrow, EditorStyles.label, GUILayout.Width(15),
                        GUILayout.Height(15));
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    var methodName = !boolCheck.serializedMethods.Any()
                        ? "None"
                        : boolCheck.serializedMethods[boolCheck.MethodIndex].methodName;
                    //  var disabledMethods = condition.TargetGameObject == null;


                    if (GUILayout.Button(methodName, EditorStyles.popup, GUILayout.MinWidth(100), GUILayout.Height(15)))
                    {
                        boolCheck.GetComponentMethods();
                        var menu = new GenericMenu();
                        for (var i = 0; i < boolCheck.serializedMethods.Length; i++)
                            menu.AddItem(new GUIContent(boolCheck.serializedMethods[i].methodInfo.Name),
                                boolCheck.MethodIndex.Equals(i), boolCheck.SetMethod, i);
                        menu.ShowAsContext();
                    }

                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();


                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.Box(ImageLibrary.DownEqualSign, EditorStyles.label, GUILayout.Width(15),
                        GUILayout.Height(15));
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    var buttonState = boolCheck.ObjectiveBool ? "True" : "False";
                    if (GUILayout.Button(buttonState, GUILayout.Height(15)))
                        boolCheck.ObjectiveBool = !boolCheck.ObjectiveBool;
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    EditorGUI.EndDisabledGroup();

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.Box(ImageLibrary.DownFlowArrow, EditorStyles.label, GUILayout.Width(15),
                        GUILayout.Height(15));
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    EditorGUI.EndDisabledGroup();
                    var areaOfDownFlowArrowButton = GUILayoutUtility.GetLastRect();
                    var modifyBoolChecksUIArea = new Rect(conditionBodyArea.x, areaOfDownFlowArrowButton.y + 15, conditionBodyArea.width, 15);
                    Separator3(modifyBoolChecksUIArea, modifyBoolChecksUIArea.width, 15);

                    var theAddAndRemoveButtonArea = modifyBoolChecksUIArea.ToUpperRight(55);

                    GUI.DrawTexture(theAddAndRemoveButtonArea, Textures.Blue);

                    var addBoolCheckButtonArea = theAddAndRemoveButtonArea.ToCenterLeft(8, 8, 10);

                    if (ClickEvent.Click(4, addBoolCheckButtonArea, ImageLibrary.addConditionIcon, "Add If statement"))
                    {
                        condition.BoolChecks.Insert(m + 1, new BoolCheckSystem());
                    }

                    if (m > 0)
                    {
                        var removeBoolCheckButtonArea = theAddAndRemoveButtonArea.ToCenterRight(8, 8, -10);
                        if (ClickEvent.Click(4, removeBoolCheckButtonArea, ImageLibrary.deleteConditionIcon, "Delete the above If Statement"))
                        {
                            condition.BoolChecks.RemoveAt(m);
                        }
                    }

                    var ampersantArea = modifyBoolChecksUIArea.ToCenter(20);
                    if (m < condition.BoolChecks.Count - 1)
                    {

                        GUI.Label(ampersantArea, "&&", Theme.Skin.label);

                    }
                    else
                        GUI.Label(ampersantArea, "run", Theme.Skin.label);


                    var adedHeight = (condition.BoolChecks.Count - 1) * 170;

#if UNITY_2021_3_OR_NEWER

                    if (condition.AutoStart)
                        GeneralConditionSpecificSpaceing[c] = 184 + adedHeight;
                    else
                        GeneralConditionSpecificSpaceing[c] = 201 + adedHeight;
#elif UNITY_2020_2_OR_NEWER

                    if (condition.AutoStart)
                    GeneralConditionSpecificSpaceing[c] = 184 + adedHeight;
                else
                    GeneralConditionSpecificSpaceing[c] = 201 + adedHeight;

#elif UNITY_2019_3_OR_NEWER
                if (condition.AutoStart)
                    GeneralConditionSpecificSpaceing[c] = 184 + adedHeight;
                else
                    GeneralConditionSpecificSpaceing[c] = 201 + adedHeight;

#elif UNITY_2019_2_OR_NEWER
                 if (condition.AutoStart)
                    GeneralConditionSpecificSpaceing[c] = 174 + adedHeight;
                else
                    GeneralConditionSpecificSpaceing[c] = 191 + adedHeight;  

#else



                    if (condition.AutoStart)
                        GeneralConditionSpecificSpaceing[c] = 174 + adedHeight;// 174
                    else
                        GeneralConditionSpecificSpaceing[c] = 191 + adedHeight;//191

#endif



                    #endregion

                }



                EditorGUILayout.PropertyField(conditionSerializedObject.FindProperty("targetEvent"),
                    new GUIContent(condition.name));

                conditionSerializedObject.ApplyModifiedProperties();


                GUILayout.Space(40);

                EditorGUI.EndDisabledGroup();

            }

            // final spacing
            GUILayout.Space(5);
        }
        private void DrawConditionCreator(Interaction interaction)
        {
            if (matchingReflectedData == null) return;

            var condtionsCount = matchingReflectedData.Conditions.Count;
            if (ConditionSpecificSpaceing.Count != condtionsCount)
                ConditionSpecificSpaceing.Resize(condtionsCount);


            for (var c = 0; c < condtionsCount; c++)
            {
                var condition = matchingReflectedData.Conditions[c];

                var conditionSerializedObject = new SerializedObject(condition);
                conditionSerializedObject.Update();

                var area = EditorGUILayout.GetControlRect();
                // GUI.DrawTexture(area.AddRect(0,0,0,60), Textures.Gray);

                #region Backround UI

                var eventCount = matchingReflectedData.Conditions[c].targetEvent.GetPersistentEventCount();
                var countSpacing = 0;
                var multiplyingValue = 0;
                var xpadding = 0;

#if UNITY_2021_3_OR_NEWER


                multiplyingValue = 49;
                xpadding = -12;
                countSpacing = eventCount < 2 ? 0 : (eventCount - 1) * multiplyingValue;
#elif UNITY_2020_2_OR_NEWER


                multiplyingValue = 47;
                xpadding = -12;
                countSpacing = eventCount < 2 ? 0 : (eventCount - 1) * multiplyingValue;
#elif UNITY_2019_3_OR_NEWER
                multiplyingValue = 47;
                xpadding = -12;
                countSpacing = eventCount < 2 ? 0 : (eventCount - 1) * multiplyingValue;    

#elif UNITY_2019_2_OR_NEWER
                   multiplyingValue = 43;
                xpadding = 0;
                countSpacing = eventCount < 2 ? 0 : (eventCount - 1) * multiplyingValue;    
#else

                multiplyingValue = 43;
                xpadding = 0;
                countSpacing = eventCount < 2 ? 0 : (eventCount - 1) * multiplyingValue;

#endif



                /* var conditionBodyArea = area.ToUpperLeft(0, areaHeight + countSpacing);*/
                var conditionBodyArea = area.ToUpperLeft(0, 115 + ConditionSpecificSpaceing[c] + countSpacing);
                GUI.Box(conditionBodyArea, "", Theme.GameBridgeSkin.customStyles[2]);
                GUI.DrawTexture(area.ToUpperLeft(0, 3, 0, 15), Textures.DuskLighter);
                var conditionBodyFooter = conditionBodyArea.PlaceUnder(0, 5);
                GUI.Box(conditionBodyFooter, "", Theme.GameBridgeSkin.customStyles[3]);
                var buttonArea = conditionBodyFooter.ToLowerRight(55, 14, xpadding, 14);
                GUI.Box(buttonArea, "", Theme.GameBridgeSkin.customStyles[4]);

                var addConditionButtonArea = buttonArea.ToCenterLeft(8, 8, 10);
                if (ClickEvent.Click(4, addConditionButtonArea, ImageLibrary.addConditionIcon))
                {
                    var newCondition = new GameObject(matchingSelectedNodeData.name + "Condition " +
                                                      matchingReflectedData.Conditions.Count);
                    newCondition.AddComponent<Condition>(); //.self = newCondition;
                    var _condition = newCondition.GetComponent<Condition>();
                    _condition.InteractionGameObject = interaction.gameObject;
                    _condition.interaction = interaction;
                    _condition.Self = newCondition;
                    newCondition.transform.SetParent(matchingReflectedData.transform);
                    // newCondition.hideFlags = HideFlags.HideInHierarchy;
                    //   matchingReflectedData.Conditions.Add(newCondition.GetComponent<Condition>());
                    matchingReflectedData.Conditions.Insert(c + 1, newCondition.GetComponent<Condition>());

                    ConditionSpecificSpaceing.Add(0);
                }

                var deleteConditionButtonArea = buttonArea.ToCenterRight(8, 8, -10);
                if (c != 0)
                    if (ClickEvent.Click(4, deleteConditionButtonArea, ImageLibrary.deleteConditionIcon))
                    {
                        DestroyImmediate(matchingReflectedData.Conditions[c].gameObject);
                        matchingReflectedData.Conditions.RemoveAt(c);
                        return;
                    }

                #endregion

                var moveConditionUpButtonArea = area.ToCenterLeft(15, 8, 10);
                if (ClickEvent.Click(1, moveConditionUpButtonArea, ImageLibrary.upArrow, "Move this condtion up by one position"))
                {
                    if (c > 0)
                    {
                        var ConditionAtTop = matchingReflectedData.Conditions[c - 1];

                        matchingReflectedData.Conditions[c - 1] = null;
                        matchingReflectedData.Conditions[c] = null;

                        matchingReflectedData.Conditions[c - 1] = condition;
                        matchingReflectedData.Conditions[c] = ConditionAtTop;
                    }
                }


                var moveConditionDownButtonArea = moveConditionUpButtonArea.PlaceToRight(15, 0, 20);
                if (ClickEvent.Click(1, moveConditionDownButtonArea, ImageLibrary.downArrow, "Move this condition down by one position"))
                {
                    if (c != condtionsCount - 1)
                    {
                        var ConditionAtBottom = matchingReflectedData.Conditions[c + 1];

                        matchingReflectedData.Conditions[c + 1] = null;
                        matchingReflectedData.Conditions[c] = null;

                        matchingReflectedData.Conditions[c + 1] = condition;
                        matchingReflectedData.Conditions[c] = ConditionAtBottom;
                    }
                }

                var copyAllConditionDataButtonArea = moveConditionDownButtonArea.PlaceToRight(12, 12, 20, -4);
                if (ClickEvent.Click(1, copyAllConditionDataButtonArea, ImageLibrary.CopyIcon, "Copy condition data"))
                {
                    ConditionCopy.MakeCopy(condition, c);
                }

                var copyOnlyEventsButtonArea = copyAllConditionDataButtonArea.PlaceToRight(12, 12, 20);
                if (ClickEvent.Click(1, copyOnlyEventsButtonArea, ImageLibrary.CopyIcon, "copy all event data only (temporarily disabled)"))
                {
                    //  ConditionCopy.MakeCopy(condition, true);
                }


                var pasteDataButtonArea = copyOnlyEventsButtonArea.PlaceToRight(12, 12, 20);
                if (ClickEvent.Click(1, pasteDataButtonArea, ImageLibrary.PasteIcon, "Paste copied data"))
                {
                    Undo.RegisterCompleteObjectUndo(condition, "Condition");
                    ConditionCopy.Paste(condition, c);
                }

                var setRepeatArea = pasteDataButtonArea.PlaceToRight(18, 12, 20);
                if (ClickEvent.Click(1, setRepeatArea, condition.Repeat ? ImageLibrary.loopActiveIcon : ImageLibrary.loopInactiveIconWhite, "Toggle repeat"))
                {
                    Undo.RegisterCompleteObjectUndo(condition, "Condition");
                    condition.Repeat = !condition.Repeat;
                }

                if (ClickEvent.Click(1, area.ToUpperRight(15, 15, -10),
                    condition.Disabled ? ImageLibrary.PowerOffpro : ImageLibrary.PowerOnpro, "Disable/ Enable this condition"))
                    condition.Disabled = !condition.Disabled;

                EditorGUI.BeginDisabledGroup(condition.Disabled);

                if (!condition.AutoStart && !condition.UseTime && matchingSelectedNodeData.useTime)
                {
                    condition.ConditionUpdateRate = (UpdateRate)EditorGUILayout.EnumPopup(condition.ConditionUpdateRate, GUILayout.Height(15));
                    GUILayout.Space(2);
                }


                var autoStartInfo = condition.AutoStart ? "Auto Start Is On" : "Auto Start Is Off";
                if (GUILayout.Button(autoStartInfo, GUILayout.Height(15)))
                    condition.AutoStart = !condition.AutoStart;


                /*  if (GUILayout.Button("get condtion methods", GUILayout.Height(15)))
                  {
                      condition.processMethods();
                  }*/

                // starting out 4 pixels must be added to the layout height
                // ConditionSpecificSpaceing[c] = 19;
                #region Use time settings
                if (matchingSelectedNodeData.useTime)
                {


                    GUILayout.BeginHorizontal();
                    var useTimeInfo = condition.UseTime ? "Use Time Is On" : "Use Time Is Off";

                    if (GUILayout.Button(useTimeInfo, GUILayout.Height(15)))
                        condition.UseTime = !condition.UseTime;

                    GUILayout.FlexibleSpace();
                    EditorGUI.BeginDisabledGroup(!condition.UseTime);



                    condition.timeUseMethod = (TimeUseMethod)EditorGUILayout.EnumPopup(condition.timeUseMethod, GUILayout.Width(115), GUILayout.Height(15));



                    /*var titleofTimBeingUsed = condition.UseDelay ? "Using Delay" : "Using Duration";
                    if (GUILayout.Button(titleofTimBeingUsed, GUILayout.Width(100), GUILayout.Height(15)))
                    {
                        condition.UseDuration = !condition.UseDuration;
                        condition.UseDelay = !condition.UseDelay;
                    }
                    */


                    EditorGUI.EndDisabledGroup();
                    GUILayout.EndHorizontal();
                    // from here only 3 must be added to evey height value
                    //  ConditionSpecificSpaceing[c] = 172;

                    if (condition.timeUseMethod == TimeUseMethod.Custom)
                    {

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Custom Time : ", GUILayout.Height(15));
                        condition.CustomWaitTime = EditorGUILayout.DelayedFloatField(condition.CustomWaitTime, GUILayout.Width(115), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        // ConditionSpecificSpaceing[c] = 192;
                    }

                }
                #endregion

                #region if statement
                /*
                EditorGUI.BeginDisabledGroup(condition.AutoStart || condition.UseTime);
                // all the content inside here 2 added the y
                // all the content inside here 2 added the y
                GUILayout.BeginHorizontal();

                GUILayout.FlexibleSpace();
                GUILayout.Label("If", GUILayout.Height(15));
                GUILayout.FlexibleSpace();

                GUILayout.EndHorizontal();


                //  condition.ComponentIndex = EditorGUILayout.Popup(condition.ComponentIndex,condition.Components, GUILayout.Height(15));


                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                condition.TargetGameObject = (GameObject)EditorGUILayout.ObjectField(condition.TargetGameObject,
                    typeof(GameObject), true, GUILayout.Height(15));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                if (condition.cachedTargetObject != condition.TargetGameObject)
                {
                    condition.Components = new Component[0];
                    condition.serializedMethods = new SerializableMethodInfo[0];

                    condition.SetComponent(0);
                    condition.SetMethod(0);
                    condition.cachedTargetObject = condition.TargetGameObject;
                }


                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Box(ImageLibrary.DownFlowArrow, EditorStyles.label, GUILayout.Width(15),
                    GUILayout.Height(15));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();


                var disabledComponents = condition.TargetGameObject == null;
                EditorGUI.BeginDisabledGroup(disabledComponents);

                if (disabledComponents &&
                    (condition.Components.Count() != 0 || condition.serializedMethods.Count() != 0))
                {
                    condition.Components = new Component[0];
                    condition.serializedMethods = new SerializableMethodInfo[0];
                }

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();


                var componentName = "";
                if (!condition.Components.Any())
                    componentName = "None";
                else
                {
                    if (condition.Components[condition.ComponentIndex] == null)
                    {
                        condition.Components = new Component[0];
                        condition.serializedMethods = new SerializableMethodInfo[0];

                    }
                    else
                        componentName = condition.Components[condition.ComponentIndex].GetType().Name;
                }


                if (GUILayout.Button(componentName, EditorStyles.popup, GUILayout.MinWidth(100), GUILayout.Height(15)))
                {
                    condition.GetGameObjectComponents();
                    var menu = new GenericMenu();
                    for (var i = 0; i < condition.Components.Length; i++)
                        menu.AddItem(new GUIContent(condition.Components[i].GetType().ToString()),
                            condition.ComponentIndex.Equals(i), condition.SetComponent, i);
                    menu.ShowAsContext();
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Box(ImageLibrary.DownFlowArrow, EditorStyles.label, GUILayout.Width(15),
                    GUILayout.Height(15));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                var methodName = !condition.serializedMethods.Any()
                    ? "None"
                    : condition.serializedMethods[condition.MethodIndex].methodName;
                // var disabledMethods = condition.TargetGameObject == null;


                if (GUILayout.Button(methodName, EditorStyles.popup, GUILayout.MinWidth(100), GUILayout.Height(15)))
                {
                    condition.GetComponentMethods();
                    var menu = new GenericMenu();
                    for (var i = 0; i < condition.serializedMethods.Length; i++)
                        menu.AddItem(new GUIContent(condition.serializedMethods[i].methodInfo.Name),
                            condition.MethodIndex.Equals(i), condition.SetMethod, i);
                    menu.ShowAsContext();
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();


                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Box(ImageLibrary.DownEqualSign, EditorStyles.label, GUILayout.Width(15),
                    GUILayout.Height(15));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                var buttonState = condition.ObjectiveBool ? "True" : "False";
                if (GUILayout.Button(buttonState, GUILayout.Height(15)))
                    condition.ObjectiveBool = !condition.ObjectiveBool;
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                EditorGUI.EndDisabledGroup();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Box(ImageLibrary.DownFlowArrow, EditorStyles.label, GUILayout.Width(15),
                    GUILayout.Height(15));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                EditorGUI.EndDisabledGroup();


#if UNITY_2020_2_OR_NEWER
          
                      if (matchingSelectedNodeData.useTime)
                {
                    ConditionSpecificSpaceing[c] = 203;
                    if (condition.timeUseMethod == TimeUseMethod.Custom)
                    {
                        ConditionSpecificSpaceing[c] = 221;
                    }

                    if (!condition.UseTime && !condition.AutoStart)
                    {
                        if (condition.timeUseMethod == TimeUseMethod.Custom)
                            ConditionSpecificSpaceing[c] = 238;
                        else
                            ConditionSpecificSpaceing[c] = 221;

                    }

                }
                else
                    ConditionSpecificSpaceing[c] = 184;

#elif UNITY_2019_3_OR_NEWER
                      if (matchingSelectedNodeData.useTime)
                {
                    ConditionSpecificSpaceing[c] = 203;
                    if (condition.timeUseMethod == TimeUseMethod.Custom)
                    {
                        ConditionSpecificSpaceing[c] = 221;
                    }

                    if (!condition.UseTime && !condition.AutoStart)
                    {
                        if (condition.timeUseMethod == TimeUseMethod.Custom)
                            ConditionSpecificSpaceing[c] = 238;
                        else
                            ConditionSpecificSpaceing[c] = 221;

                    }

                }
                else
                    ConditionSpecificSpaceing[c] = 184;  
#elif UNITY_2019_2_OR_NEWER
                if (matchingSelectedNodeData.useTime)
                {
                    ConditionSpecificSpaceing[c] = 192;
                    if (condition.timeUseMethod == TimeUseMethod.Custom)
                    {
                        ConditionSpecificSpaceing[c] = 209;
                    }

                    if (!condition.UseTime && !condition.AutoStart)
                    {
                        if (condition.timeUseMethod == TimeUseMethod.Custom)
                            ConditionSpecificSpaceing[c] = 228;
                        else
                            ConditionSpecificSpaceing[c] = 211;

                    }

                }
                else
                    ConditionSpecificSpaceing[c] = 174;     
#else

                if (matchingSelectedNodeData.useTime)
                {
                    ConditionSpecificSpaceing[c] = 192;
                    if (condition.timeUseMethod == TimeUseMethod.Custom)
                    {
                        ConditionSpecificSpaceing[c] = 209;
                    }

                    if (!condition.UseTime && !condition.AutoStart)
                    {
                        if (condition.timeUseMethod == TimeUseMethod.Custom)
                            ConditionSpecificSpaceing[c] = 228;
                        else
                            ConditionSpecificSpaceing[c] = 211;

                    }

                }
                else
                    ConditionSpecificSpaceing[c] = 174;       

#endif



                */

                #endregion



                if (condition.BoolChecks.Count == 0)
                    condition.BoolChecks.Add(new BoolCheckSystem());



                for (int m = 0; m < condition.BoolChecks.Count; m++)
                {
                    var boolCheck = condition.BoolChecks[m];

                    EditorGUI.BeginDisabledGroup(condition.AutoStart || condition.UseTime);
                    // all the content inside here 2 added the y
                    // all the content inside here 2 added the y
                    GUILayout.BeginHorizontal();

                    GUILayout.FlexibleSpace();
                    GUILayout.Label("If", GUILayout.Height(15));
                    GUILayout.FlexibleSpace();

                    GUILayout.EndHorizontal();


                    //  condition.ComponentIndex = EditorGUILayout.Popup(condition.ComponentIndex,condition.Components, GUILayout.Height(15));


                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    boolCheck.TargetGameObject = (GameObject)EditorGUILayout.ObjectField(boolCheck.TargetGameObject,
                        typeof(GameObject), true, GUILayout.Height(15));
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    if (boolCheck.cachedTargetObject != boolCheck.TargetGameObject)
                    {
                        boolCheck.Components = new Component[0];
                        boolCheck.serializedMethods = new SerializableMethodInfo[0];

                        boolCheck.SetComponent(0);
                        boolCheck.SetMethod(0);
                        boolCheck.cachedTargetObject = boolCheck.TargetGameObject;
                    }


                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.Box(ImageLibrary.DownFlowArrow, EditorStyles.label, GUILayout.Width(15),
                        GUILayout.Height(15));
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();


                    var disabledComponents = boolCheck.TargetGameObject == null;
                    EditorGUI.BeginDisabledGroup(disabledComponents);

                    if (disabledComponents &&
                        (boolCheck.Components.Count() != 0 || boolCheck.serializedMethods.Count() != 0))
                    {
                        boolCheck.Components = new Component[0];
                        boolCheck.serializedMethods = new SerializableMethodInfo[0];
                    }

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();


                    var componentName = "";
                    if (!boolCheck.Components.Any())
                        componentName = "None";
                    else
                    {
                        if (boolCheck.Components[boolCheck.ComponentIndex] == null)
                        {
                            boolCheck.Components = new Component[0];
                            boolCheck.serializedMethods = new SerializableMethodInfo[0];

                        }
                        else
                            componentName = boolCheck.Components[boolCheck.ComponentIndex].GetType().Name;
                    }


                    if (GUILayout.Button(componentName, EditorStyles.popup, GUILayout.MinWidth(100), GUILayout.Height(15)))
                    {
                        boolCheck.GetGameObjectComponents();
                        var menu = new GenericMenu();
                        for (var i = 0; i < boolCheck.Components.Length; i++)
                            menu.AddItem(new GUIContent(boolCheck.Components[i].GetType().ToString()),
                                boolCheck.ComponentIndex.Equals(i), boolCheck.SetComponent, i);
                        menu.ShowAsContext();
                    }

                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.Box(ImageLibrary.DownFlowArrow, EditorStyles.label, GUILayout.Width(15),
                        GUILayout.Height(15));
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    var methodName = !boolCheck.serializedMethods.Any()
                        ? "None"
                        : boolCheck.serializedMethods[boolCheck.MethodIndex].methodName;
                    // var disabledMethods = condition.TargetGameObject == null;


                    if (GUILayout.Button(methodName, EditorStyles.popup, GUILayout.MinWidth(100), GUILayout.Height(15)))
                    {
                        boolCheck.GetComponentMethods();
                        var menu = new GenericMenu();
                        for (var i = 0; i < boolCheck.serializedMethods.Length; i++)
                            menu.AddItem(new GUIContent(boolCheck.serializedMethods[i].methodInfo.Name),
                                boolCheck.MethodIndex.Equals(i), boolCheck.SetMethod, i);
                        menu.ShowAsContext();
                    }

                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();


                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.Box(ImageLibrary.DownEqualSign, EditorStyles.label, GUILayout.Width(15),
                        GUILayout.Height(15));
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    var buttonState = boolCheck.ObjectiveBool ? "True" : "False";
                    if (GUILayout.Button(buttonState, GUILayout.Height(15)))
                        boolCheck.ObjectiveBool = !boolCheck.ObjectiveBool;
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    EditorGUI.EndDisabledGroup();

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.Box(ImageLibrary.DownFlowArrow, EditorStyles.label, GUILayout.Width(15),
                        GUILayout.Height(15));
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    EditorGUI.EndDisabledGroup();

                    var areaOfDownFlowArrowButton = GUILayoutUtility.GetLastRect();
                    var modifyBoolChecksUIArea = new Rect(conditionBodyArea.x, areaOfDownFlowArrowButton.y + 15, conditionBodyArea.width, 15);
                    Separator3(modifyBoolChecksUIArea, modifyBoolChecksUIArea.width, 15);

                    var theAddAndRemoveButtonArea = modifyBoolChecksUIArea.ToUpperRight(55);

                    GUI.DrawTexture(theAddAndRemoveButtonArea, Textures.Blue);

                    var addBoolCheckButtonArea = theAddAndRemoveButtonArea.ToCenterLeft(8, 8, 10);

                    if (ClickEvent.Click(4, addBoolCheckButtonArea, ImageLibrary.addConditionIcon, "Add If statement"))
                    {
                        condition.BoolChecks.Insert(m + 1, new BoolCheckSystem());
                    }

                    if (m > 0)
                    {
                        var removeBoolCheckButtonArea = theAddAndRemoveButtonArea.ToCenterRight(8, 8, -10);
                        if (ClickEvent.Click(4, removeBoolCheckButtonArea, ImageLibrary.deleteConditionIcon, "Delete the above If Statement"))
                        {
                            condition.BoolChecks.RemoveAt(m);
                        }
                    }
                    var ampersantArea = modifyBoolChecksUIArea.ToCenter(20);
                    if (m < condition.BoolChecks.Count - 1)
                    {

                        GUI.Label(ampersantArea, "&&", Theme.Skin.label);

                    }
                    else
                        GUI.Label(ampersantArea, "run", Theme.Skin.label);



#if UNITY_2021_3_OR_NEWER
                    var adedHeight = (condition.BoolChecks.Count - 1) * 168;
                    if (matchingSelectedNodeData.useTime)
                    {
                        ConditionSpecificSpaceing[c] = 198 + adedHeight;
                        if (condition.timeUseMethod == TimeUseMethod.Custom)
                        {
                            ConditionSpecificSpaceing[c] = 221 + adedHeight;
                        }

                        if (!condition.UseTime && !condition.AutoStart)
                        {
                            if (condition.timeUseMethod == TimeUseMethod.Custom)
                                ConditionSpecificSpaceing[c] = 238 + adedHeight;
                            else
                                ConditionSpecificSpaceing[c] = 218 + adedHeight;

                        }

                    }
                    else
                        ConditionSpecificSpaceing[c] = 184 + adedHeight;
#elif UNITY_2020_2_OR_NEWER
          var adedHeight = (condition.BoolChecks.Count - 1) * 170;
                    if (matchingSelectedNodeData.useTime)
                {
                    ConditionSpecificSpaceing[c] = 203 + adedHeight;
                    if (condition.timeUseMethod == TimeUseMethod.Custom)
                    {
                        ConditionSpecificSpaceing[c] = 221 + adedHeight;
                    }

                    if (!condition.UseTime && !condition.AutoStart)
                    {
                        if (condition.timeUseMethod == TimeUseMethod.Custom)
                            ConditionSpecificSpaceing[c] = 238 + adedHeight;
                        else
                            ConditionSpecificSpaceing[c] = 221 + adedHeight;

                    }

                }
                else
                    ConditionSpecificSpaceing[c] = 184 + adedHeight;

#elif UNITY_2019_3_OR_NEWER
          var adedHeight = (condition.BoolChecks.Count - 1) * 170;
                      if (matchingSelectedNodeData.useTime)
                {
                    ConditionSpecificSpaceing[c] = 203 + adedHeight;
                    if (condition.timeUseMethod == TimeUseMethod.Custom)
                    {
                        ConditionSpecificSpaceing[c] = 221 + adedHeight;
                    }

                    if (!condition.UseTime && !condition.AutoStart)
                    {
                        if (condition.timeUseMethod == TimeUseMethod.Custom)
                            ConditionSpecificSpaceing[c] = 238 + adedHeight;
                        else
                            ConditionSpecificSpaceing[c] = 221 + adedHeight;

                    }

                }
                else
                    ConditionSpecificSpaceing[c] = 184 + adedHeight;  
#elif UNITY_2019_2_OR_NEWER
          var adedHeight = (condition.BoolChecks.Count - 1) * 170;
                if (matchingSelectedNodeData.useTime)
                {
                    ConditionSpecificSpaceing[c] = 192 + adedHeight;
                    if (condition.timeUseMethod == TimeUseMethod.Custom)
                    {
                        ConditionSpecificSpaceing[c] = 209 + adedHeight;
                    }

                    if (!condition.UseTime && !condition.AutoStart)
                    {
                        if (condition.timeUseMethod == TimeUseMethod.Custom)
                            ConditionSpecificSpaceing[c] = 228 + adedHeight;
                        else
                            ConditionSpecificSpaceing[c] = 211 + adedHeight;

                    }

                }
                else
                    ConditionSpecificSpaceing[c] = 174 + adedHeight;     
#else
                    var adedHeight = (condition.BoolChecks.Count - 1) * 170;


                    if (matchingSelectedNodeData.useTime)
                    {
                        ConditionSpecificSpaceing[c] = 192 + adedHeight;
                        if (condition.timeUseMethod == TimeUseMethod.Custom)
                        {
                            ConditionSpecificSpaceing[c] = 209 + adedHeight;
                        }

                        if (!condition.UseTime && !condition.AutoStart)
                        {
                            if (condition.timeUseMethod == TimeUseMethod.Custom)
                                ConditionSpecificSpaceing[c] = 228 + adedHeight;
                            else
                                ConditionSpecificSpaceing[c] = 211 + adedHeight;

                        }

                    }
                    else
                        ConditionSpecificSpaceing[c] = 174 + adedHeight;

#endif



                }


                if (matchingSelectedNodeData.type != typeof(CharacterNodeData))
                {
                    EditorGUILayout.PropertyField(conditionSerializedObject.FindProperty("targetEvent"),
                        new GUIContent(condition.name));

                    conditionSerializedObject.ApplyModifiedProperties();
                }

                GUILayout.Space(40);

                EditorGUI.EndDisabledGroup();
            }

            // final spacing
            GUILayout.Space(5);
        }

        private void Separator()
        {
            var area = GUILayoutUtility.GetLastRect().AddRect(-15, 0);

            GUI.DrawTexture(area.ToLowerLeft(ScreenRect.width, 1), Textures.DuskLightest);
        }

        private void Separator2()
        {
            var area = GUILayoutUtility.GetLastRect().AddRect(-15, 0);

            GUI.DrawTexture(area.ToLowerLeft(ScreenRect.width, 1), Textures.DuskLight);
        }

        private void Separator3(Rect rect, float width, float height)
        {
            GUILayout.Space(height);

            var repeatArea = rect.ToLowerLeft(width, height);
            GUI.DrawTextureWithTexCoords(repeatArea, ImageLibrary.RepeatableStipe, new Rect(0, 0, repeatArea.width / height, 1));
        }
        private void Separator3()
        {
            var area = GUILayoutUtility.GetLastRect().AddRect(-15, 20);
            Separator3(area, ScreenRect.width, 20);

        }

        #region variables

        [SerializeField] public string UID = "";

        [SerializeField] private NodeData matchingSelectedNodeData;

        private ReflectedData matchingReflectedData;
        private ReflectedData matchingGeneralReflectedData;

        private bool iconSet;
        private Rect ScreenRect = new Rect(0, 0, 0, 0);
        private Vector2 scrollView;
        private readonly List<int> ConditionSpecificSpaceing = new List<int>();
        private readonly List<int> GeneralConditionSpecificSpaceing = new List<int>();

        private Story tempStoryData;

        private Interaction selectedInteraction;

        private SerializedProperty DisplayedText;

        private SerializedProperty FilterCount;

        private SerializedProperty moveNextButton;

        //  private SerializedProperty movePreviousButton;

        private SerializedProperty NameText;

        private SerializedProperty RouteButton;

        private SerializedProperty RouteParent;

        //   private SerializedProperty sceneData;

        private SerializedProperty ShowGeneralSettings;

        private SerializedProperty ShowKeywordFilterFouldout;

        private SerializedProperty textDisplayMode;

        private SerializedProperty typingMode;

        private SerializedProperty TypingAudioCip;

        private SerializedProperty TypingSpeed;

        private SerializedProperty useDialogueTextUI;

        private SerializedProperty UseKeywordFilters;

        private SerializedProperty useMoveNextButton;

        //  private SerializedProperty useMovePreviousButton;

        private SerializedProperty useNameUI;

        private SerializedProperty useRouteButton;

        private SerializedProperty SetActionText;

        private SerializedProperty RoutClearsDIalogue;

        private SerializedProperty UsesSoundffects;

        //  private SerializedProperty UsesStoryboardImages;

        private SerializedProperty UsesText;

        private SerializedProperty UsesVoiceOver;

        private SerializedProperty useAutoStartVoiceClip;

        private SerializedProperty useAutoStartSoundEffectClip;

        private SerializedProperty AutoStartAllConditionsByDefault;

        private SerializedProperty ShowNodeSpecificConditionSettings;

        private SerializedProperty VolumeVariation;

        //   private SerializedProperty ComponentUID;

        //private SerializedProperty UpdateUID;
        public SerializedProperty ControllingCharacterUID;
        //  private SerializedProperty NameColour;

        //   private SerializedProperty TextColour;

        public SerializedProperty interactionType;

        private int AmountOfCharactersToSet;

        bool showHelpMessage = false;

        private EditorWindow[] edwin;

        #endregion


    }
}