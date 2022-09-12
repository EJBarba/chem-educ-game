using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class ChangeCamera : MonoBehaviour
{
    [SerializeField] GameObject playerCamera;
    [SerializeField] GameObject mapCamera;
    [SerializeField] Button cameraButton;
    private bool isPlayerFocused = true;

    void Awake() {
        cameraButton.onClick.AddListener(ToggleCamera);
    }

    void ToggleCamera()
    {
        if (isPlayerFocused)
        {
            playerCamera.SetActive(false);
            isPlayerFocused = false;       
        }
        else
        {
            playerCamera.SetActive(true);
            isPlayerFocused = true;
        }
    }

}
