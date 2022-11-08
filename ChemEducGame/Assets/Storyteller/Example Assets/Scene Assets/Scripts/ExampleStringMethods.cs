using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleStringMethods : MonoBehaviour
{

    public string characterName ="";
    public string wordA = "";
    public string wordB = "";
    public string wordC = "";

    public string CharacterName
    {
        get
        {
            return characterName;
        }
        set
        {
            characterName = value;
        }
    }

    public string WordA
    {
        get
        {
            return wordA;
        }
        set
        {
            wordA = value;
        }
    }
    public string WordB
    {
        get
        {
            return wordB;
        }
        set
        {
            wordB = value;
        }
    }
    public string WordC
    {
        get
        {
            return wordC;
        }
        set
        {
            wordC = value;
        }
    }

}
