using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DaiMangou.BridgedData;
using DaiMangou.Storyteller;
using DaiMangou.Storyteller.Elements;
using UnityEditor;
using UnityEngine;
using Grid = DaiMangou.Storyteller.Grid;
using Object = UnityEngine.Object;

namespace DaiMangou.GameBridgeEditor
{
    [Serializable]
    public class ComponentData
    {
        public string Description;
        public Texture2D Icons;
        public string Name;


        public ComponentData(Texture2D icon, string name, string description)
        {
            Icons = icon;
            Name = name;
            Description = description;
        }
    }

    [Serializable]
    public class PushSettings
    {
        public List<Rect> Areas = new List<Rect>();
        public bool PushSoundEffects = true;
        public bool PushPersonalityTraits;
        public bool PushVoiceOver = true;

        /* public List<bool> pushOptions = new List<bool>();
        public List<string> optionNames = new List<string>();

        public bool OverrideAll;
        public bool UpdateText = true;
        public bool UpdateSoundEffects = true;
        public bool UpdateVoiceover = true;
        public bool UpdateCharacter = true;
        public bool UpdateEnvironment = true;
        public bool UpdateStoryboardImages = true;


       public PushSettings()
        {

            pushOptions.AddMany(
          OverrideAll,
          UpdateText,
          UpdateSoundEffects,
          UpdateVoiceover,
          UpdateCharacter,
          UpdateEnvironment,
          UpdateStoryboardImages);

            optionNames.AddMany(
          "Override All",
          "Text",
          "Sound Effects",
          "Voiceover",
          "Character",
          "Environment",
          "Storyboard ");


        }
        */
    }

    [Serializable]
    public class GameBridgeEditorWindow : EditorWindow
    {
        /* void Create(string name, Dialoguer dialoguer)
         {
             var asset = ScriptableObject.CreateInstance<DialogueData>();
             ProjectWindowUtil.CreateAsset(asset, "Assets/" + name + ".asset");
             //  var newDialogueData = AssetDatabase.LoadAssetAtPath("Assets/" + name + ".asset", typeof(DialogueData)) as DialogueData;
             asset.name = name;

             dialoguer.dialogueData = asset;

             ProjectWindowUtil.ShowCreatedAsset(asset);

             AssetDatabase.SaveAssets();
             AssetDatabase.Refresh();
             Selection.activeObject = null;
         }*/

        private static string GetSelectedPathOrFallback()
        {
            var path = "Assets";

            foreach (var obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (string.IsNullOrEmpty(path) || !File.Exists(path)) continue;
                path = Path.GetDirectoryName(path);
                break;
            }

            return path;
        }

        [MenuItem("Assets/Create/Game Bridge/SceneData")]
        public static void CreateSceneData()
        {
            var asset = CreateInstance<SceneData>();
            const string name = "New SceneData";
            ProjectWindowUtil.CreateAsset(asset, name + ".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void CreateSceneData(string name, Interaction interaction)
        {
            if (!File.Exists(Application.dataPath + "/" + name + ".asset"))
            {
                var asset = CreateInstance<SceneData>();
                var path0 = GetSelectedPathOrFallback();
                var path = AssetDatabase.GetAssetPath(Selection.activeObject);
                var finalPath = path.Equals("") ? path0 : path;
                var i = 0;
                while (File.Exists(finalPath + "/" + name + ".asset"))
                {
                    i++;
                    name = name + i;
                }

                AssetDatabase.CreateAsset(asset, finalPath + "/" + name + ".asset");
                var newDialogueData =
                    AssetDatabase.LoadAssetAtPath(finalPath + "/" + name + ".asset", typeof(SceneData)) as SceneData;

                interaction.sceneData = newDialogueData;

                ProjectWindowUtil.ShowCreatedAsset(newDialogueData);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            else
            {
                var data = AssetDatabase.LoadAssetAtPath("Assets/" + name + ".asset", typeof(SceneData)) as SceneData;
                foreach (var d in data.FullCharacterDialogueSet) DestroyImmediate(d, true);
                data.ActiveCharacterDialogueSet = new List<NodeData>();

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }



        /// <summary>
        ///     UNUSED , would completely empty a dialogue data dataset
        /// </summary>
        /// <param name="dialoguer"></param>
        /*  private static void ClearDialogueData(Dialoguer dialoguer)
          {
              var data = AssetDatabase.LoadAssetAtPath(AssetDatabase.GetAssetPath(dialoguer.dialogueData), typeof(DialogueData)) as DialogueData;
              foreach (var d in data.FullCharacterDialogueSet)
              {
                  Object.DestroyImmediate(d, true);
              }
              data.ActiveCharacterDialogueSet = new List<NodeData>();

              AssetDatabase.SaveAssets();
              AssetDatabase.Refresh();
          }*/
        public void OnEnable()
        {
            titleContent.text = "Bridge";
            minSize = new Vector2(275, 400);
            titleContent.image = ImageLibrary.GameBridgeIconSmall;


            //  Debug.Log(typeof(GameBridgeEditor).AssemblyQualifiedName);

            _componentData = new List<ComponentData>();
            _componentData.AddMany(
                new ComponentData(ImageLibrary.Interaction, "Interaction", " "),
                new ComponentData(ImageLibrary.InteractableCharacter, "Interactable Character", " "),
                new ComponentData(ImageLibrary.CollisionTrigger, "Collision Trigger", " "),
                new ComponentData(ImageLibrary.CollisionReceiver, "Colission Receiver", " "),
                new ComponentData(ImageLibrary.ClickListener, "Click Listener", " ")
                );
        }

        private void CheckSelection()
        {
            if (CurrentStory.ActiveStory == null)
            {
                if (Selection.activeObject)
                    if (Selection.activeObject.GetType() == typeof(Story))
                        story = (Story)Selection.activeObject;
            }
            else
            {
                story = CurrentStory.ActiveStory;
            }

            if (!Selection.activeObject)
            {
                EditID = 0;
            }
            else
            {
                if (Selection.activeObject.GetType() == typeof(SceneData))
                {
                    SelectedSceneData = (SceneData)Selection.activeObject;
                    EditID = 1;
                }
                else
                {
                    EditID = 2;
                }
            }
        }

        public void OnGUI()
        {
            ScreenRect.size = position.size;

            GUI.DrawTexture(ScreenRect, Textures.DuskLighter);
            GUI.skin = Theme.GameBridgeSkin;
            CheckSelection();


            if (story == null) EditID = 0;

            var windowArea = ScreenRect;
            var headerArea = windowArea.ToUpperLeft(0, 50);

            if (EditID != 0)
            {
                GUI.DrawTexture(headerArea, Textures.DuskLight);
                GUI.DrawTexture(headerArea.ToLowerLeft(0, 2), Textures.Blue);
            }



            switch (EditID)
            {
                case 0:

                    //GUI.color = new Color(1, 1, 1,Mathf.Lerp(0,1,));
                    GUI.DrawTexture(ScreenRect, Textures.DuskLight);
                    GUI.DrawTexture(ScreenRect.ToCenter(150,150), ImageLibrary.GameBridgeIcon);

                    break;

                case 1:

                    #region Scene Data Setup interface

                    /* if (SelectedSceneData.SceneID != -1)
                     {
                       //  GUI.Label(headerArea, "Push Data To Game Scene", Theme.GameBridgeSkin.customStyles[0]);
                     }*/

                    if (SelectedSceneData.SceneID == -1)
                    {
                        #region we will prompt to uses to slect a scene , onve the scen is selected, we pass the scenes sceneid value to the dialogue scene id value.

                        GUI.Label(headerArea.ToCenter(0, 30), story.name, Theme.GameBridgeSkin.customStyles[0]);

                        var newarea = windowArea.AddRect(0, 50, 0, -10);
                        Grid.BeginDynaicGuiGrid(newarea.AddRect(0, 20), scenesAreas, 20, 25, 65, 75,
                            story.Scenes.Count);

                        scrollView = GUI.BeginScrollView(new Rect(0, 0, newarea.width, newarea.height), scrollView,
                            new Rect(0, 0, 0, Grid.AreaHeight + 40));

                        var activeClipArea = new Rect(0, scrollView.y, windowArea.width, windowArea.height);

                        #region we display all the scenes that can be uses and when the uses selects one, we pass hat scenes sceneId to the selectedDialiguers sceneId

                        foreach (var area in scenesAreas.Select((v, i) => new { value = v, index = i }))
                            if (activeClipArea.Contains(area.value.TopLeft()) ||
                                activeClipArea.Contains(area.value.BottomRight()))
                            {
                                GUI.DrawTexture(area.value, ImageLibrary.sceneIcon);


                                var textArea = area.value.PlaceUnder(0, 20);

                                GUI.Label(textArea, story.Scenes[area.index].SceneName);

                                if (ClickEvent.Click(3, area.value))
                                {
                                    SelectedSceneData.SceneID = area.index;
                                    SelectedSceneData.UID = story.Scenes[area.index].UID;
                                }
                            }

                        #endregion

                        GUI.EndScrollView();

                        Grid.EndDynaicGuiGrid();

                        #endregion
                    }
                    else
                    {

                        if (SelectedSceneData.UID != story.Scenes[SelectedSceneData.SceneID].UID)
                        {
                            EditorGUI.HelpBox(ScreenRect.ToCenter(0, 50), "The SceneData asset you have selected , does not belong to this Story scene. To edit this SceneData Asset, please open the scene in storyteller", MessageType.Warning);

                            EditorGUI.HelpBox(ScreenRect.ToCenterBottom(0, 30, 0, -50), "If you believe that this is indeed the correct SceneData asset, click the Update buton", MessageType.Info);
                            if (GUI.Button(ScreenRect.ToCenterBottom(0, 20), "Update"))
                                SelectedSceneData.UID = story.Scenes[SelectedSceneData.SceneID].UID;

                            return;
                        }

                        var activeDataPushArea = headerArea.PlaceUnder(0, ScreenRect.height - headerArea.height);


                        #region Push Data Settings



                        var generalSettingsHeaderArea = activeDataPushArea.ToUpperLeft(0, 20);
                        GUI.DrawTexture(generalSettingsHeaderArea, Textures.DuskLight);
                        GUI.Label(generalSettingsHeaderArea, "Settings");

                        var pushSoundEfectToggleArea = generalSettingsHeaderArea.PlaceUnder(0, 0, 0, 10);
                        pushSettings.PushSoundEffects = GUI.Toggle(pushSoundEfectToggleArea,
                            pushSettings.PushSoundEffects, "Sound Effects");

                        var pushVoiceOverToggleArea = pushSoundEfectToggleArea.PlaceUnder(0, 0, 0, 10);
                        pushSettings.PushVoiceOver = GUI.Toggle(pushVoiceOverToggleArea, pushSettings.PushVoiceOver,
                            "Voice Over");

                        var pushPersonalityTraitsToggleArea = pushVoiceOverToggleArea.PlaceUnder(0, 0, 0, 10);
                        pushSettings.PushPersonalityTraits = GUI.Toggle(pushPersonalityTraitsToggleArea,
                            pushSettings.PushPersonalityTraits, "Personality Traits");


                        var characterSelectionAreaHeader = activeDataPushArea.ToUpperLeft(0, 20, 0, 200);
                        GUI.DrawTexture(characterSelectionAreaHeader, Textures.DuskLight);
                        GUI.Label(characterSelectionAreaHeader, "Character Selection");
                        EditorGUI.HelpBox(characterSelectionAreaHeader.PlaceUnder(0, 40), "All characters are set to be pushed. If you push unchecked characters who have ties to checked characters then data will be missing", MessageType.Info);
                        var characterSelectionArea = characterSelectionAreaHeader.PlaceUnder(0, ScreenRect.height - 360, 0, 40);




                        var characterCount = story.Scenes[SelectedSceneData.SceneID].Characters.Count;
                        Grid.BeginDynaicGuiGrid(characterSelectionArea, characterAreas, 10, 20, 120, 60, story.Scenes[SelectedSceneData.SceneID].Characters.Count);

                        characterScrollView = GUI.BeginScrollView(new Rect(0, 0, characterSelectionArea.width, characterSelectionArea.height), characterScrollView, new Rect(0, 0, characterSelectionArea.width - 20, Grid.AreaHeight), false, false);

                        // we set the pushable value to true if we switch target SceneData
                        if (story.Scenes[SelectedSceneData.SceneID].UID != CachedUID)
                        {
                            story.Scenes[SelectedSceneData.SceneID].Characters.All(p => p.Pushable == true);
                            CachedUID = story.Scenes[SelectedSceneData.SceneID].UID;
                        }


                        for (int i = 0; i < characterCount; i++)
                        {

                            var targetCharacter = story.Scenes[SelectedSceneData.SceneID].Characters[i];
                            GUI.DrawTexture(characterAreas[i], Textures.DuskLightest);
                            GUI.DrawTexture(characterAreas[i].ToUpperLeft(40, 40), targetCharacter.CharacterBios[targetCharacter.BioID].CharacterImage);
                            GUI.Label(characterAreas[i].ToLowerLeft(0, 20), targetCharacter.CharacterBios[targetCharacter.BioID].CharacterName);
                            targetCharacter.Pushable = GUI.Toggle(characterAreas[i].ToUpperRight(15, 15, -15), targetCharacter.Pushable, "");
                        }

                        GUI.EndScrollView();
                        Grid.EndDynaicGuiGrid();


                        GUI.DrawTexture(characterSelectionArea.PlaceUnder(0, 2), Textures.DuskLightest);

                        #endregion


                        #region here we allow the uses to push their selected scenes data over into the DiaogueData asset. how this works is further broken down in this region

                        var pushDataButtonArea = activeDataPushArea.ToCenterBottom(150, 20, 0, -20);

                        #region begin pushing storyteller data over into dialogue Data Asset

                        if (GUI.Button(pushDataButtonArea, "Push To Scene Data"))
                        {
                            GetWindow<StorytellerEditor>();
                            EditorWindow.FocusWindowIfItsOpen(typeof(StorytellerEditor));  
                            //foreach (var nd in AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(SelectedSceneData)))
                            foreach (var nd in SelectedSceneData.FullCharacterDialogueSet) DestroyImmediate(nd, true);
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();

                            if (SelectedSceneData == null)
                            {
                                Debug.Log("please assign a Scene Data Object before pushing data");
                                return;
                            }

                            SelectedSceneData.UpdateUID = Guid.NewGuid().ToString();

                            #region find each character in the scene and aggregate the nodes in its chain. Rechaining 

                            // here we find all routenodes in storyteller and set their routes to 0 as we prep to push the data over into the SceneData Asset
                            /*   var routes = story.Scenes[SelectedSceneData.SceneID].NodeElements.Where(r => r.GetType() == typeof(RouteNode)).ToList();
                               foreach (var route in routes)
                               {
                                   var r = (RouteNode)route;
                                   r.RouteId = 0;
                               }

                               foreach (var ch in story.Scenes[SelectedSceneData.SceneID].Characters)
                               {
                                   // this  ensures that he data being pushed is not flawed. this is the same as refreshing the timeline
                                   ch.AggregateNodesInChain();

                               }
                               */

                            #endregion

                            #region we create a new list which we will add all nodes except Abstract and Media nodes in

                            var sortedList = new List<StoryElement>();

                            #endregion

                            #region iterating through all the nodes in the current storyteller scene

                            foreach (var el in story.Scenes[SelectedSceneData.SceneID].NodeElements)
                            {
                                #region we only want Character nodes, Link nodes, Route nodes, Dialogue nodes and Action Nodes Added to the list

                                if (el.type != typeof(MediaNode) && el.type != typeof(AbstractNode))
                                {
                                    #region prevent nodes that are not connected to a character from being added to the list

                                    if (el.CallingNode != null)
                                        if (el.CallingNode.Pushable)
                                            sortedList.Add(el);


                                    #endregion
                                }

                                #endregion
                            }

                            #endregion

                            // increate the size of the list of node dataset to be the same as the sorted list
                            SelectedSceneData.FullCharacterDialogueSet.Resize(sortedList.Count);


                            SelectedSceneData.Characters = new List<CharacterNodeData>();

                            // loop through the sorted list
                            for (var i = 0; i < sortedList.Count; i++)
                            {
                                #region here we lookat each sorted list StoryElement and we pass their relevant data over into NodeData scriptable object data which had similar properties and names

                                if (sortedList[i].type == typeof(CharacterNode))
                                {
                                    var character = (CharacterNode)sortedList[i];
                                    SelectedSceneData.FullCharacterDialogueSet[i] =
                                        CreateInstance(typeof(CharacterNodeData)) as NodeData;
                                    AssetDatabase.AddObjectToAsset(SelectedSceneData.FullCharacterDialogueSet[i],
                                        SelectedSceneData);
                                    var imagePath = Application.dataPath;
                                    // File.WriteAllBytes(imagePath+"/" + character.CharacterBios[character.BioID].CharacterName+ ".png", character.CharacterBios[character.BioID].CharacterImage.EncodeToPNG());
                                    // var image = AssetDatabase.LoadAssetAtPath("Assets/" + character.CharacterBios[character.BioID].CharacterName+".png", typeof(Texture2D));
                                    /// AssetDatabase.AddObjectToAsset(image, SelectedDialogueData);
                                    // AssetDatabase.Refresh();
                                    SelectedSceneData.FullCharacterDialogueSet[i].hideFlags = HideFlags.HideInHierarchy;
                                    var data = (CharacterNodeData)SelectedSceneData.FullCharacterDialogueSet[i];
                                    data.UID = character.UID;
                                    data.SortingID = character.SortingID;
                                    data.OverrideTime = character.OverrideTime;
                                    data.name = data.Name = character.CharacterBios[character.BioID].CharacterName;
                                    data.CharacterName = character.CharacterBios[character.BioID].CharacterName;

                                    if (pushSettings.PushPersonalityTraits)
                                    {
                                        var positiveTraitsST = character.CharacterBios[character.BioID].Personality.PositiveTraits;
                                        var positiveTraitsGB = data.CharacterPersonality.PositiveTraits;

                                        positiveTraitsGB.Accessible = positiveTraitsST.Accessible;
                                        positiveTraitsGB.Active = positiveTraitsST.Active;
                                        positiveTraitsGB.Adaptable = positiveTraitsST.Adaptable;
                                        positiveTraitsGB.Adventurous = positiveTraitsST.Adventurous;
                                        positiveTraitsGB.Alert = positiveTraitsST.Alert;
                                        positiveTraitsGB.Brilliant = positiveTraitsST.Brilliant;
                                        positiveTraitsGB.Calm = positiveTraitsST.Calm;
                                        positiveTraitsGB.Capable = positiveTraitsST.Capable;
                                        positiveTraitsGB.Challenging = positiveTraitsST.Challenging;
                                        positiveTraitsGB.Charismatic = positiveTraitsST.Charismatic;
                                        positiveTraitsGB.Clever = positiveTraitsST.Clever;
                                        positiveTraitsGB.Confident = positiveTraitsST.Confident;
                                        positiveTraitsGB.Cooperative = positiveTraitsST.Cooperative;
                                        positiveTraitsGB.Courageous = positiveTraitsST.Courageous;
                                        positiveTraitsGB.Curious = positiveTraitsST.Curious;
                                        positiveTraitsGB.Daring = positiveTraitsST.Daring;
                                        positiveTraitsGB.Decisive = positiveTraitsST.Decisive;
                                        positiveTraitsGB.Discreet = positiveTraitsST.Discreet;
                                        positiveTraitsGB.Efficient = positiveTraitsST.Efficient;
                                        positiveTraitsGB.Energetic = positiveTraitsST.Energetic;
                                        positiveTraitsGB.Focused = positiveTraitsST.Focused;
                                        positiveTraitsGB.Helpful = positiveTraitsST.Helpful;
                                        positiveTraitsGB.Independent = positiveTraitsST.Independent;
                                        positiveTraitsGB.Individualistic = positiveTraitsST.Individualistic;
                                        positiveTraitsGB.Innovative = positiveTraitsST.Innovative;
                                        positiveTraitsGB.Intelligent = positiveTraitsST.Intelligent;
                                        positiveTraitsGB.Leaderly = positiveTraitsST.Leaderly;
                                        positiveTraitsGB.Logical = positiveTraitsST.Logical;
                                        positiveTraitsGB.Loyal = positiveTraitsST.Loyal;
                                        positiveTraitsGB.Methodical = positiveTraitsST.Methodical;
                                        positiveTraitsGB.Orderly = positiveTraitsST.Orderly;
                                        positiveTraitsGB.Organized = positiveTraitsST.Organized;
                                        positiveTraitsGB.Patient = positiveTraitsST.Patient;
                                        positiveTraitsGB.Patriotic = positiveTraitsST.Patriotic;
                                        positiveTraitsGB.Peaceful = positiveTraitsST.Peaceful;
                                        positiveTraitsGB.Reliable = positiveTraitsST.Reliable;
                                        positiveTraitsGB.Resourceful = positiveTraitsST.Resourceful;
                                        positiveTraitsGB.Responsible = positiveTraitsST.Responsible;
                                        positiveTraitsGB.Responsive = positiveTraitsST.Responsive;
                                        positiveTraitsGB.Sane = positiveTraitsST.Sane;
                                        positiveTraitsGB.Selfless = positiveTraitsST.Selfless;
                                        positiveTraitsGB.Serious = positiveTraitsST.Serious;
                                        positiveTraitsGB.Skillful = positiveTraitsST.Skillful;
                                        positiveTraitsGB.Stoic = positiveTraitsST.Stoic;
                                        positiveTraitsGB.Strong = positiveTraitsST.Strong;


                                        var neutralTraitsST = character.CharacterBios[character.BioID].Personality.NeutralTraits;
                                        var neutralTraitsGB = data.CharacterPersonality.NeutralTraits;

                                        neutralTraitsGB.Aggressive = neutralTraitsST.Aggressive;
                                        neutralTraitsGB.Deceptive = neutralTraitsST.Deceptive;
                                        neutralTraitsGB.Dominating = neutralTraitsST.Dominating;
                                        neutralTraitsGB.Enigmatic = neutralTraitsST.Enigmatic;
                                        neutralTraitsGB.Experimental = neutralTraitsST.Experimental;
                                        neutralTraitsGB.Iconoclastic = neutralTraitsST.Iconoclastic;
                                        neutralTraitsGB.Idiosyncratic = neutralTraitsST.Idiosyncratic;
                                        neutralTraitsGB.Impassive = neutralTraitsST.Impassive;
                                        neutralTraitsGB.Obedient = neutralTraitsST.Obedient;
                                        neutralTraitsGB.Progressive = neutralTraitsST.Progressive;
                                        neutralTraitsGB.Questioning = neutralTraitsST.Questioning;
                                        neutralTraitsGB.Reserved = neutralTraitsST.Reserved;
                                        neutralTraitsGB.Stubborn = neutralTraitsST.Stubborn;


                                        var negativeTraitsST = character.CharacterBios[character.BioID].Personality.NegativeTraits;
                                        var negativeTraitsGB = data.CharacterPersonality.NegativeTraits;

                                        negativeTraitsGB.Angry = negativeTraitsST.Angry;
                                        negativeTraitsGB.Arrogantt = negativeTraitsST.Arrogantt;
                                        negativeTraitsGB.Careless = negativeTraitsST.Careless;
                                        negativeTraitsGB.Childish = negativeTraitsST.Childish;
                                        negativeTraitsGB.Cowardly = negativeTraitsST.Cowardly;
                                        negativeTraitsGB.Crafty = negativeTraitsST.Crafty;
                                        negativeTraitsGB.Crazy = negativeTraitsST.Crazy;
                                        negativeTraitsGB.Cruel = negativeTraitsST.Cruel;
                                        negativeTraitsGB.Dependent = negativeTraitsST.Dependent;
                                        negativeTraitsGB.Destructive = negativeTraitsST.Destructive;
                                        negativeTraitsGB.Devious = negativeTraitsST.Devious;
                                        negativeTraitsGB.Disloyal = negativeTraitsST.Disloyal;
                                        negativeTraitsGB.Disobedient = negativeTraitsST.Disobedient;
                                        negativeTraitsGB.Disorderly = negativeTraitsST.Disorderly;
                                        negativeTraitsGB.Disorganized = negativeTraitsST.Disorganized;
                                        negativeTraitsGB.Fearful = negativeTraitsST.Fearful;
                                        negativeTraitsGB.Fickle = negativeTraitsST.Fickle;
                                        negativeTraitsGB.Foolish = negativeTraitsST.Foolish;
                                        negativeTraitsGB.Forgetful = negativeTraitsST.Forgetful;
                                        negativeTraitsGB.Frightening = negativeTraitsST.Frightening;
                                        negativeTraitsGB.Frivolous = negativeTraitsST.Frivolous;
                                        negativeTraitsGB.Graceless = negativeTraitsST.Graceless;
                                        negativeTraitsGB.Gullible = negativeTraitsST.Gullible;
                                        negativeTraitsGB.Hostile = negativeTraitsST.Hostile;
                                        negativeTraitsGB.Impractical = negativeTraitsST.Impractical;
                                        negativeTraitsGB.Irritable = negativeTraitsST.Irritable;
                                        negativeTraitsGB.Lazy = negativeTraitsST.Lazy;
                                        negativeTraitsGB.Meddlesome = negativeTraitsST.Meddlesome;
                                        negativeTraitsGB.Miserable = negativeTraitsST.Miserable;
                                        negativeTraitsGB.Neglectful = negativeTraitsST.Neglectful;
                                        negativeTraitsGB.Opportunistic = negativeTraitsST.Opportunistic;
                                        negativeTraitsGB.Provocative = negativeTraitsST.Provocative;
                                        negativeTraitsGB.Slow = negativeTraitsST.Slow;
                                        negativeTraitsGB.Timid = negativeTraitsST.Timid;
                                        negativeTraitsGB.Uncooperative = negativeTraitsST.Uncooperative;
                                        negativeTraitsGB.Unreliable = negativeTraitsST.Unreliable;
                                        negativeTraitsGB.Weak = negativeTraitsST.Weak;
                                        negativeTraitsGB.WeakWilled = negativeTraitsST.WeakWilled;

                                    }






                                    SelectedSceneData.Characters.Add(data);
                                }

                                if (sortedList[i].type == typeof(EnvironmentNode))
                                {
                                    var environment = (EnvironmentNode)sortedList[i];
                                    SelectedSceneData.FullCharacterDialogueSet[i] =
                                        CreateInstance(typeof(EnvironmentNodeData)) as NodeData;
                                    AssetDatabase.AddObjectToAsset(SelectedSceneData.FullCharacterDialogueSet[i],
                                        SelectedSceneData);
                                    SelectedSceneData.FullCharacterDialogueSet[i].hideFlags = HideFlags.HideInHierarchy;
                                    var data = (EnvironmentNodeData)SelectedSceneData.FullCharacterDialogueSet[i];
                                    data.UID = environment.UID;
                                    data.Pass = environment.Pass;
                                    data.OverrideTime = environment.OverrideTime;
                                    data.name = data.Name = environment.Name;
                                    data.CharacterName = environment.CallingNode.Name;
                                    data.EnvironmentName = environment.Environment.Name;
                                }

                                if (sortedList[i].type == typeof(RouteNode))
                                {
                                    var route = (RouteNode)sortedList[i];
                                    SelectedSceneData.FullCharacterDialogueSet[i] =
                                        CreateInstance(typeof(RouteNodeData)) as NodeData;
                                    AssetDatabase.AddObjectToAsset(SelectedSceneData.FullCharacterDialogueSet[i],
                                        SelectedSceneData);
                                    SelectedSceneData.FullCharacterDialogueSet[i].hideFlags = HideFlags.HideInHierarchy;
                                    var data = (RouteNodeData)SelectedSceneData.FullCharacterDialogueSet[i];
                                    data.UID = route.UID;
                                    data.OverrideTime = route.OverrideTime;
                                    data.DurationSum = route.NodeDurationSum;
                                    data.AutoSwitchValue = route.AutoSwitchValue;
                                    data.Pass = route.Pass;
                                    data.RouteID = route.RouteId;
                                    data.name = data.Name = route.Name;
                                    data.ConvergenceMode = route.ConvergenceMode;
                                    if (route.OverrideTime)
                                        data.StartTime = route.StartingTime;

                                    data.CharacterName = route.CallingNode.CharacterBios[route.CallingNode.BioID]
                                        .CharacterName;
                                    if (route.Environment)
                                        data.EnvironmentName = route.Environment.Name;
                                }

                                if (sortedList[i].type == typeof(LinkNode))
                                {
                                    var link = (LinkNode)sortedList[i];
                                    SelectedSceneData.FullCharacterDialogueSet[i] =
                                        CreateInstance(typeof(LinkNodeData)) as NodeData;
                                    AssetDatabase.AddObjectToAsset(SelectedSceneData.FullCharacterDialogueSet[i],
                                        SelectedSceneData);
                                    SelectedSceneData.FullCharacterDialogueSet[i].hideFlags = HideFlags.HideInHierarchy;
                                    var data = (LinkNodeData)SelectedSceneData.FullCharacterDialogueSet[i];
                                    data.UID = link.UID;
                                    //  data.OverrideTime = link.OverrideTime;
                                    data.LoopValue = link.LoopValue;

                                    if(link.TimeLoopDataset.Count >0)
                                    {
                                        data.TimeLoopDataset = new List<BridgedData.TimeLoopData>();
                                        for (int d = 0; d < link.TimeLoopDataset.Count; d++)
                                        {
                                            data.TimeLoopDataset.Add(new BridgedData.TimeLoopData());
                                            data.TimeLoopDataset[d].AutoSwitchValue = link.TimeLoopDataset[d].AutoSwitchValue;
                                            data.TimeLoopDataset[d].LoopTimes = link.TimeLoopDataset[d].LoopTimes;
                                        }

                                    }

                                    data.name = data.Name = link.Name;
                                    data.Pass = link.Pass;
                                    data.Loop = link.Loop;
                                    if (link.OverrideTime)
                                        data.StartTime = link.StartingTime;

                                    data.CharacterName = link.CallingNode.CharacterBios[link.CallingNode.BioID]
                                        .CharacterName;
                                    if (link.Environment)
                                        data.EnvironmentName = link.Environment.Name;
                                }

                                if (sortedList[i].type == typeof(EndNode))
                                {
                                    var end = (EndNode)sortedList[i];
                                    SelectedSceneData.FullCharacterDialogueSet[i] =
                                        CreateInstance(typeof(EndNodeData)) as NodeData;
                                    AssetDatabase.AddObjectToAsset(SelectedSceneData.FullCharacterDialogueSet[i],
                                        SelectedSceneData);
                                    SelectedSceneData.FullCharacterDialogueSet[i].hideFlags = HideFlags.HideInHierarchy;
                                    var data = (EndNodeData)SelectedSceneData.FullCharacterDialogueSet[i];
                                    data.UID = end.UID;
                                    //   data.OverrideTime = end.OverrideTime;
                                    data.Pass = end.Pass;
                                    data.name = data.Name = end.Name;
                                    if (end.OverrideTime)
                                        data.StartTime = end.StartingTime;

                                    data.CharacterName = end.CallingNode.CharacterBios[end.CallingNode.BioID]
                                        .CharacterName;
                                    if (end.Environment)
                                        data.EnvironmentName = end.Environment.Name;
                                }

                                if (sortedList[i].type == typeof(ActionNode))
                                {
                                    var action = (ActionNode)sortedList[i];
                                    SelectedSceneData.FullCharacterDialogueSet[i] =
                                        CreateInstance(typeof(ActionNodeData)) as NodeData;
                                    AssetDatabase.AddObjectToAsset(SelectedSceneData.FullCharacterDialogueSet[i],
                                        SelectedSceneData);
                                    SelectedSceneData.FullCharacterDialogueSet[i].hideFlags = HideFlags.HideInHierarchy;
                                    var data = (ActionNodeData)SelectedSceneData.FullCharacterDialogueSet[i];
                                    data.UID = action.UID;
                                    //  data.OverrideTime = action.OverrideTime;
                                    data.Delay = action.DelayTimeInSeconds;
                                    data.Duration = action.TimeInSeconds;
                                    data.StartTime = action.StartingTime;
                                    data.DurationSum = action.NodeDurationSum;

                                    /* if (action.StoryboardImage)
                                         if (pushSettings.PushPersonalityTraits)
                                             data.StoryboardImage = Sprite.Create(action.StoryboardImage,
                                                 new Rect(0, 0, action.StoryboardImage.width,
                                                     action.StoryboardImage.height),
                                                 new Vector2(action.StoryboardImage.width / 2,
                                                     action.StoryboardImage.height / 2));*/

                                    if (pushSettings.PushSoundEffects)
                                    {
                                        //data.SoundEffect = action.SoundEffect;
                                        data.LocalizedSoundEffects.Resize(action.LocalizedSoundEffects.Count);
                                        for (int l = 0; l < action.LocalizedSoundEffects.Count; l++)
                                        {
                                            data.LocalizedSoundEffects[l] = action.LocalizedSoundEffects[l];
                                        }
                                    }

                                    data.Pass = action.Pass;
                                    data.name = data.Name = action.Name;
                                    data.Text = action.Text;
                                    data.LocalizedText.Resize(action.LocalizedText.Count);
                                    for (int lt = 0; lt < action.LocalizedText.Count; lt++)
                                    {
                                        data.LocalizedText[lt] = action.LocalizedText[lt];
                                    }
                                    data.Tag = action.Tag;
                                    data.CharacterName = action.CallingNode.CharacterBios[action.CallingNode.BioID]
                                        .CharacterName;

                                    if (action.Environment)
                                        data.EnvironmentName = action.Environment.Name;
                                    // data.EnvironmentLocation = action.Environment.EnvironmentInfo.Location;
                                }

                                if (sortedList[i].type == typeof(DialogueNode))
                                {
                                    var dialogue = (DialogueNode)sortedList[i];
                                    SelectedSceneData.FullCharacterDialogueSet[i] =
                                        CreateInstance(typeof(DialogueNodeData)) as NodeData;
                                    AssetDatabase.AddObjectToAsset(SelectedSceneData.FullCharacterDialogueSet[i],
                                        SelectedSceneData);
                                    SelectedSceneData.FullCharacterDialogueSet[i].hideFlags = HideFlags.HideInHierarchy;
                                    var data = (DialogueNodeData)SelectedSceneData.FullCharacterDialogueSet[i];
                                    data.UID = dialogue.UID;
                                    //    data.OverrideTime = dialogue.OverrideTime;
                                    data.Delay = dialogue.DelayTimeInSeconds;
                                    data.Duration = dialogue.TimeInSeconds;
                                    data.StartTime = dialogue.StartingTime;
                                    data.DurationSum = dialogue.NodeDurationSum;

                                    /*if (dialogue.StoryboardImage)
                                        if (pushSettings.PushPersonalityTraits)
                                            data.StoryboardImage = Sprite.Create(dialogue.StoryboardImage,
                                                new Rect(0, 0, dialogue.StoryboardImage.width,
                                                    dialogue.StoryboardImage.height),
                                                new Vector2(dialogue.StoryboardImage.width / 2,
                                                    dialogue.StoryboardImage.height / 2));*/

                                    if (pushSettings.PushSoundEffects)
                                    {
                                        //data.SoundEffect = dialogue.SoundEffect;
                                        data.LocalizedSoundEffects.Resize(dialogue.LocalizedSoundEffects.Count);
                                        for (int l = 0; l < dialogue.LocalizedSoundEffects.Count; l++)
                                        {
                                            data.LocalizedSoundEffects[l] = dialogue.LocalizedSoundEffects[l];
                                        }
                                    }

                                    if (pushSettings.PushVoiceOver)
                                    {
                                        // data.VoicedDialogue = dialogue.VoiceRecording;
                                        data.LocalizedVoiceRecordings.Resize(dialogue.LocalizedVoiceRecordings.Count);
                                        for (int l = 0; l < dialogue.LocalizedVoiceRecordings.Count; l++)
                                        {
                                            data.LocalizedVoiceRecordings[l] = dialogue.LocalizedVoiceRecordings[l];
                                        }
                                    }

                                    data.Pass = dialogue.Pass;
                                    data.name = data.Name = dialogue.Name;
                                    data.Text = dialogue.Text;
                                    data.LocalizedText.Resize(dialogue.LocalizedText.Count);

                                    for (int lt = 0; lt < dialogue.LocalizedText.Count; lt++)
                                    {
                                        data.LocalizedText[lt] = dialogue.LocalizedText[lt];
                                    }
                                    data.Tag = dialogue.Tag;

                                    data.CharacterName = dialogue.CallingNode.CharacterBios[dialogue.CallingNode.BioID]
                                        .CharacterName;

                                    if (dialogue.Environment)
                                        data.EnvironmentName = dialogue.Environment.Name;
                                }

                                #endregion
                            }


                            #region we now order the SelectedSceneData.Characters list by UID

                            SelectedSceneData.Characters =
                                SelectedSceneData.Characters.OrderBy(c => c.SortingID).ToList();

                            #endregion

                            /*                            // This loop is constructed to ensure that the character list order is always the same when scene data is pushed
                            for (int c = 0; c < story.Scenes[CurrentStory.ActiveScene].Characters.Count; c++)
                            {
                                for (int i = 0; i < tempCharacterList.Count; i++)
                                {
                                    if (story.Scenes[CurrentStory.ActiveScene].Characters[c].UID == tempCharacterList[i].UID)
                                    {
                                        Debug.Log(tempCharacterList[i].name +" " + tempCharacterList[i].UID+ " and " + story.Scenes[CurrentStory.ActiveScene].Characters[c].name +" " + story.Scenes[CurrentStory.ActiveScene].Characters[c].UID);
                                        SelectedSceneData.Characters.Add(tempCharacterList[i]);

                                    }
                                }
                            }*/

                            #region we do the loop yet again , this time to pass in specific data to nodes that have properties that take in node data values like character nodes Nodes in its hain, link nodes linked nodes and route nodes routed nodes

                            // the process is quite efficient a node data in the sortedDataList and fullChracterDialogueSet have data with UID values that match.
                            for (var i = 0; i < sortedList.Count; i++)
                            {
                                if (sortedList[i].type == typeof(CharacterNode))
                                {
                                    var character = (CharacterNode)sortedList[i];
                                    var data = (CharacterNodeData)SelectedSceneData.FullCharacterDialogueSet[i];
                                    // selectedDialoguer.Characters.Add(data);

                                    for (var n = 0; n < sortedList.Count; n++)
                                    {
                                        var node = sortedList[n];
                                        if (node.CallingNode == character) //&& node != character
                                            // we add to the NodeDataInMyChain  node data list, the node data in the NodeDataInMyChain that have matching UIDs to the 
                                            // nodes in the  sorted list  that have character as their calling node .
                                            // this could be done in another way.
                                            data.NodeDataInMyChain.Add(
                                                SelectedSceneData.FullCharacterDialogueSet.Find(c =>
                                                    c.UID == node.UID));
                                    }

                                    //   data.NodeDataInMyChain = data.NodeDataInMyChain.OrderBy(m => m.StartTime).ToList();
                                    data.NodeDataInMyChain.All(a => a.CallingNodeData = data);
                                }
                                if (sortedList[i].type == typeof(EnvironmentNode))
                                { }
                                if (sortedList[i].type == typeof(RouteNode))
                                {
                                    var route = (RouteNode)sortedList[i];
                                    var data = (RouteNodeData)SelectedSceneData.FullCharacterDialogueSet[i];

                                    if (route.LinkedRout != null && route.LinkedRout.CallingNode.Pushable)
                                    {
                                        var idOfLinkRoute = route.LinkedRout.UID;
                                        data.LinkedRoute =
                                            SelectedSceneData.FullCharacterDialogueSet.Find(id =>
                                                id.UID == idOfLinkRoute) as RouteNodeData;

                                        data.LinkedRoute.RoutesLinkedToMe.Add(data);
                                    }
                                }

                                if (sortedList[i].type == typeof(LinkNode))
                                {
                                    var link = (LinkNode)sortedList[i];
                                    var data = (LinkNodeData)SelectedSceneData.FullCharacterDialogueSet[i];


                                    if (link.LoopRoute != null)
                                    {
                                        var idOfLinkedLink = link.LoopRoute.UID;

                                        
                                        data.loopRoute =
                                            SelectedSceneData.FullCharacterDialogueSet.Find(id => id.UID == idOfLinkedLink)
                                                as RouteNodeData;

                                        data.loopRoute.ControllingLinkNode = data;
                                    }
                                }

                                #region Finally set their environments
                                if (sortedList[i].Environment == null) continue;
                                SelectedSceneData.FullCharacterDialogueSet[i].Environment = (EnvironmentNodeData)SelectedSceneData.FullCharacterDialogueSet.Find(e => e.UID == sortedList[i].Environment.UID);



                                #endregion

                            }

                            #endregion

                            #region now we use our matching UID's again to find out which nodes were connected in the SortedData list and we connect the nodes with matching UIDs in the FullCharacterDataList with the same UID's

                            for (var i = 0; i < SelectedSceneData.FullCharacterDialogueSet.Count; i++)
                            {
                                var data = SelectedSceneData.FullCharacterDialogueSet[i];
                                var matchingStoryElement = CurrentStory.ActiveStory.Scenes[SelectedSceneData.SceneID]
                                    .NodeElements.Find(id => id.UID == data.UID);
                                data.DataIconnectedTo.Resize(matchingStoryElement.NodesIMadeConnectionsTo.Count);
                                data.DataConnectedToMe.Resize(matchingStoryElement.NodesThatMadeAConnectionToMe.Count);

                                // assign all the node data that the fullcharacterdataset element at i connected to
                                for (var d = 0; d < data.DataIconnectedTo.Count; d++)
                                    //   var iConnectedTo = matchingStoryElement.NodesIMadeConnectionsTo[d];
                                    //   if (iConnectedTo.GetType() == typeof(DialogueNode)|| iConnectedTo.GetType() == typeof(ActionNode) || iConnectedTo.GetType() == typeof(RouteNode)|| iConnectedTo.GetType() == typeof(LinkNode))

                                    data.DataIconnectedTo[d] = SelectedSceneData.FullCharacterDialogueSet.Find(v =>
                                        v.UID == matchingStoryElement.NodesIMadeConnectionsTo[d].UID);
                                // assign all the node data that are connected to the fullcharacterdataset element at i
                                for (var d = 0; d < data.DataConnectedToMe.Count; d++)
                                    data.DataConnectedToMe[d] = SelectedSceneData.FullCharacterDialogueSet.Find(v =>
                                        v.UID == matchingStoryElement.NodesThatMadeAConnectionToMe[d].UID);
                            }

                            #endregion

                            SelectedSceneData.LanguageIndex = story.LanguageIndex;
                            SelectedSceneData.Languages.Resize(story.Languages.Count);
                            SelectedSceneData.LanguageNameArray = new string[story.Languages.Count];

                            for (int i = 0; i < story.Languages.Count; i++)
                            {
                                SelectedSceneData.Languages[i] = new BridgedData.Language();
                                SelectedSceneData.Languages[i].Name = story.Languages[i].Name;
                                SelectedSceneData.Languages[i].CountryCode = story.Languages[i].CountryCode;
                                SelectedSceneData.LanguageNameArray[i] = story.Languages[i].Name;
                            }

                            // lastly we order the list of NodeData by their startStime value, this is  necessary for when we populate the  ActiveCharacterDialogueSet
                            SelectedSceneData.FullCharacterDialogueSet = SelectedSceneData.FullCharacterDialogueSet
                                .OrderBy(r => r.StartTime).ToList();
                        }

                        #endregion

                        #endregion
                    }

                    #endregion

                    break;
                case 2:

                    #region Component addition  

                    GUI.Label(headerArea.ToCenter(0, 30), story.name, Theme.GameBridgeSkin.customStyles[0]);

                    Grid.BeginDynaicGuiGrid(headerArea.PlaceUnder(0, ScreenRect.height - headerArea.height), _componentAreas, 30, 30, 100, 100, _componentData.Count);
                    componentsScrollView =
                        GUI.BeginScrollView(new Rect(0, 0, ScreenRect.width, ScreenRect.height - headerArea.height),
                            componentsScrollView, new Rect(0, 0, ScreenRect.width-20, Grid.AreaHeight),
                            false, false);
                    for (var a = 0; a < _componentData.Count; a++)
                    {
                        var area = _componentAreas[a];

                           // GUI.DrawTexture(area, Textures.DuskLight);
                            if (InfoBlock.Click(1, area, _componentData[a].Icons, SnapPosition.TopMiddle, Color.clear,
                                InfoBlock.HoverEvent.None, Theme.GameBridgeSkin.customStyles[0],
                                Theme.GameBridgeSkin.customStyles[1], _componentData[a].Name,
                                _componentData[a].Description, 0, 40, 70, 70))
                                AttachComponen(a);
                        
                    }

                    GUI.EndScrollView();
                    Grid.EndDynaicGuiGrid();

                    #endregion

                    break;
            }
        }

        public void OnInspectorUpdate()
        {
            Repaint();
        }

        private void AttachComponen(int a)
        {
            switch (a)
            {
                case 0:
                    _Selection().AddComponent<Interaction>();
                    break;
                case 1:
                    _Selection().AddComponent<InteractableCharacter>();
                    break;
                case 2:
                    _Selection().AddComponent<CollisionInteractionTrigger>();
                    break;
                case 3:
                    _Selection().AddComponent<CollisionInteractionReceiver>();
                    break;
                case 4:
                    _Selection().AddComponent<ClickListener>();
                    break;
                default:
                    break;
            }
        }

        private int SelectionUI(int i, int Offset)
        {
            // var headerBarArea = ScreenRect()

            return i + 1;
        }

        protected void OverrideAll()
        {
            if (EditorUtility.DisplayDialog("Replace Data?",
                "Are you sure you want to override the Current Dialoguers NodeData ?", "yes", "cancel"))
            {
            }
        }

        protected void UpdateText()
        {
        }

        protected void UpdateSoundEffects()
        {
        }

        protected void UpdateVoiceover()
        {
        }

        protected void UpdateChatacter()
        {
        }

        protected void UpdateEnvironment()
        {
        }

        protected void UpdateStoryboardImage()
        {
        }

        protected void UpdateNoeData()
        {
        }


        #region variables

        public SceneData SelectedSceneData;
        private int EditID;
        public List<Rect> scenesAreas = new List<Rect>();
        private readonly List<Rect> _componentAreas = new List<Rect>();

        private readonly List<Rect> characterAreas = new List<Rect>();


        private Vector2 scrollView;
        private Vector2 componentsScrollView;

        [SerializeField] private Story story;

        [SerializeField] private List<ComponentData> _componentData = new List<ComponentData>();

        private Rect ScreenRect = new Rect();

        public GameObject _Selection()
        {
            try
            {
                return Selection.activeGameObject;
            }
            catch
            {
                return null;
            }
        }

        public PushSettings pushSettings = new PushSettings();

        private string CachedUID = "";

        private Vector2 characterScrollView = new Vector2();


        #endregion
    }
}