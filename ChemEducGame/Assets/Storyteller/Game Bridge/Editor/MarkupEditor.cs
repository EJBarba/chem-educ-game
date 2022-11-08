using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;
using DaiMangou.Storyteller;
using DaiMangou.BridgedData;

namespace DaiMangou.GameBridgeEditor
{
    public class MarkupEditor : EditorWindow
    {
        public SceneData sceneData;
        public NodeData TargetNodeData;
        private Rect ScreenRect = new Rect();
        private Vector2 ScrollView;
        private bool showMarkupSidebar;
        void OnEnable()
        {
            minSize = new Vector2(700, 400);
            maxSize =  new Vector2(700, 400);
            titleContent.text = "Markup Editor";
        }
        void OnGUI()
        {
            GUI.skin = Theme.GameBridgeSkin;
            GUI.DrawTexture(ScreenRect, Textures.Dusk);

            ScreenRect.size = new Vector2(position.width, position.height);


            var HeaderArea = ScreenRect.ToUpperLeft(0, 20);
            GUI.DrawTexture(HeaderArea, Textures.DuskLight);

            var QuickEditBUttonArea = HeaderArea.ToCenterLeft(150, 15,20);
            if (GUI.Button( QuickEditBUttonArea,new GUIContent(!Theme.GameBridgeSkin.customStyles[7].richText?"Markup Edit: <color=green>On</color>": "Markup Edit: <color=#ff0033>Off</color>", ImageLibrary.editIcon, "Enter Editr Mode")))
                Theme.GameBridgeSkin.customStyles[7].richText = !Theme.GameBridgeSkin.customStyles[7].richText;

            var ShowMarkupTagsArea = QuickEditBUttonArea.PlaceToRight(150, 15, 20);
            if (GUI.Button(ShowMarkupTagsArea, "Markup Tags Examples"))
                showMarkupSidebar = !showMarkupSidebar;

          


            var TextEditorArea = HeaderArea.PlaceUnder(showMarkupSidebar? ScreenRect.width- 300:0, ScreenRect.height - HeaderArea.height);

            var height = Theme.Skin.customStyles[7].CalcHeight(new GUIContent(TargetNodeData.LocalizedText[sceneData.LanguageIndex]), TextEditorArea.width);
            ScrollView = GUI.BeginScrollView(new Rect(0,0, TextEditorArea.width, TextEditorArea.height), ScrollView, new Rect(0, 0, TextEditorArea.width-20, height));

            TargetNodeData.LocalizedText[sceneData.LanguageIndex] = EditorGUI.TextArea(TextEditorArea, TargetNodeData.LocalizedText[sceneData.LanguageIndex], Theme.GameBridgeSkin.customStyles[7]);
            GUI.EndScrollView();

            if (showMarkupSidebar)
            {
                var MarkupFoldoutArea = ScreenRect.ToUpperRight(300);
                GUI.DrawTexture(MarkupFoldoutArea, Textures.DuskLight);
                GUI.DrawTexture(MarkupFoldoutArea.ToUpperLeft(2), Textures.Blue);

  var ShowMarkupTagsAreaHeader = MarkupFoldoutArea.ToUpperLeft(0, 20);
GUI.Label(ShowMarkupTagsAreaHeader, "<color=lightblue> <b>Tag Examples</b> </color>");

                var boldExampleArea = ShowMarkupTagsAreaHeader.PlaceUnder(0, 20,0,40);
                GUI.Label(boldExampleArea,"This is a <b>bold</b> ",EditorStyles.label);
                GUI.Label(boldExampleArea.PlaceUnder(), "This is a <b>bold</b> ");

                var italicExampleArea = boldExampleArea.PlaceUnder(0, 0,0, 40);
                GUI.Label(italicExampleArea, "This is  <i>italics</i>", EditorStyles.label);
                GUI.Label(italicExampleArea.PlaceUnder(), "This is  <i>italics</i>");

                var sizeExampleArea = italicExampleArea.PlaceUnder(0, 40,0, 40);
                GUI.Label(sizeExampleArea, "A larger <size=20>sized</size> text", EditorStyles.label);
                GUI.Label(sizeExampleArea.PlaceUnder(), "A larger <size=30>sized</size> text");

                var colourExampleArea = sizeExampleArea.PlaceUnder(0, 0,0, 40);
                GUI.Label(colourExampleArea, "<color=green> green</color> and <color=#00ffffff>blue</color>", EditorStyles.label);
                GUI.Label(colourExampleArea.PlaceUnder(), "<color=green> green</color> and <color=#00ffffff>blue</color>");

              //  var mixedExampleArea = colourExampleArea.PlaceUnder(0, 0,0, 40);
             //   GUI.Label(mixedExampleArea, "Try <size= 30>everything</size> , be <b>bold</b> and <i>lean</i> into change... something something <color=blue>blue</color>", EditorStyles.label);
              //  GUI.Label(mixedExampleArea.PlaceUnder(), "Try <size= 20>everything</size> , be <b>bold</b> and <i>lean</i> into change... something something <color=blue>blue</color>");

            }

        }

        void OnInspectorUpdate()
        {
            Repaint();
        }
    }
}
