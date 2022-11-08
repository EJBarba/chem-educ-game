using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using DaiMangou.BridgedData;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveManager : MonoBehaviour
{

    public static SaveManager saveManager;
    public string SaveName = "Saved Data";
    public SceneData TargetSceneData;
    public Interaction InteractionSystem;
  
     void Awake()
    {
       if(saveManager == null)
        {
            saveManager = this;
        }
        else if(saveManager != this)
        {
            Destroy(this);
        }
        DontDestroyOnLoad(this);
    }


    public bool Saved()
    {
        return Directory.Exists(Application.persistentDataPath +"/" + SaveName);
    }

    public void Save()
    {
        if (!Saved())
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/" + SaveName);

        }

        if (!Directory.Exists(Application.persistentDataPath + "/" + SaveName + "/InteractionData"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/" + SaveName + "/InteractionData");
        }

        BinaryFormatter bf = new BinaryFormatter();

        #region UNNECESSARY SAVE. YOU ONLY NEED TO SAVE THE ActiveIndex value...  Interaction.ActiveIndex. unless you want to save the history too
        /* 
                  FileStream fl = File.Create(Application.persistentDataPath + "/" + SaveName + "/InteractionData" + "/" + InteractionSystem.name + ".txt");

                  var js = JsonUtility.ToJson(InteractionSystem);
                  bf.Serialize(fl, js);
                  fl.Close();*/
        #endregion

        #region Save the Active Index int value. this is the valuie which tells the character or dialogue where in the story to process data
        PlayerPrefs.SetInt("lastActiveIndex", InteractionSystem.ActiveIndex);
        #endregion

        #region Save the scen data file
        FileStream file = File.Create(Application.persistentDataPath + "/" + SaveName + "/InteractionData" + "/" + TargetSceneData.name + ".txt");

        var json = JsonUtility.ToJson(TargetSceneData);
        bf.Serialize(file, json);
        file.Close();
        #endregion

        #region save the sub data in the SceneData File
        foreach (var subData in InteractionSystem.sceneData.FullCharacterDialogueSet)
        {
            FileStream subFile = File.Create(Application.persistentDataPath + "/" + SaveName + "/InteractionData" + "/" + subData.UID + ".txt");

            var subJson = JsonUtility.ToJson(subData);
            bf.Serialize(subFile, subJson);
            subFile.Close();
        }
        #endregion

    }

    public void Load()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/" + SaveName + "/InteractionData"))
            return;

        BinaryFormatter bf = new BinaryFormatter();

        #region UNNECESSARY LOAD you only need to load and set the ActiveIndex value  TargtDialoguer.ActiveIndex = LoadedIntValue

        /*  if (File.Exists(Application.persistentDataPath + "/" + SaveName + "/InteractionData" + "/" + InteractionSystem.name + ".txt"))
          {
              FileStream file = File.Open(Application.persistentDataPath + "/" + SaveName + "/InteractionData" + "/" + InteractionSystem.name + ".txt", FileMode.Open);

              JsonUtility.FromJsonOverwrite((string)bf.Deserialize(file), InteractionSystem);
              file.Close();
          }*/
        #endregion

        #region Load the sceneData
        if (File.Exists(Application.persistentDataPath + "/" + SaveName + "/InteractionData" + "/" + TargetSceneData.name + ".txt"))
        {
            FileStream file = File.Open(Application.persistentDataPath + "/" + SaveName + "/InteractionData" + "/" + TargetSceneData.name + ".txt",FileMode.Open);

            JsonUtility.FromJsonOverwrite((string)bf.Deserialize(file), TargetSceneData);
            file.Close();
        }
        #endregion

        #region Load the subData 
        foreach (var subData in InteractionSystem.sceneData.FullCharacterDialogueSet)
            if (File.Exists(Application.persistentDataPath + "/" + SaveName + "/InteractionData" + "/" + subData.UID + ".txt"))
        {
            FileStream file = File.Open(Application.persistentDataPath + "/" + SaveName + "/InteractionData" + "/" + subData.UID + ".txt", FileMode.Open);

            JsonUtility.FromJsonOverwrite((string)bf.Deserialize(file), subData);
            file.Close();
        }
        #endregion

        #region Load the ActiveIndex int value
        InteractionSystem.ActiveIndex = PlayerPrefs.GetInt("lastActiveIndex");
        #endregion



    }



}
