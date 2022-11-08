using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleConditions2 : MonoBehaviour {

    public bool SpokeToNPC1Already;
    public bool SpokeToNPC2Already;
    public bool CanNowAskNPC3AboutIDCard;
    public bool HasIDCard;
    public bool HaveConvincedNPC3;

    public Animator DoorRight;
    public Animator DoorLeft;

    public bool IHaveAnIDCared
    {
        get
        {
            return HasIDCard;
        }
        set
        {
            HasIDCard = value;
        }



    }

    public bool SpokeToNPC1
    {
        get
        {
            return SpokeToNPC1Already;
        }
        set
        {
            SpokeToNPC1Already = value;
        }
    }

    public bool SpokeToNPC2
    {
        get
        {
            return SpokeToNPC2Already;
        }
        set
        {
            SpokeToNPC2Already = value;
        }
    }

    public bool CanAskNPC3AboutIDCard
    {
        get
        {
            return CanNowAskNPC3AboutIDCard;
        }
        set
        {
            CanNowAskNPC3AboutIDCard = value;
        }
    }
    public bool ConvincedNPC3
    {
        get
        {
            return HaveConvincedNPC3;
        }
        set
        {
            HaveConvincedNPC3 = value;
        }
    }
    public void PlayDoorRight()
    {
        DoorRight.SetBool("OpenRightDoor", true);
    }


    public void PlayDoorleft()
    {
        DoorLeft.SetBool("OpenLeftDoor", true);
    }

}
