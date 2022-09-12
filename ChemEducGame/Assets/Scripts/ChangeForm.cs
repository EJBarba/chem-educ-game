using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangeForm : MonoBehaviour
{

    [SerializeField] GameObject signupForm;
    [SerializeField] GameObject loginForm;
    [SerializeField] Button toggleFormButton;
    [SerializeField] TMP_Text toggleButtonText;
    private bool isPlayerSigningUp = true;

    void Awake() {
        toggleFormButton.onClick.AddListener(ToggleForm);
    }

    void ToggleForm()
    {
         if (isPlayerSigningUp)
        {
            signupForm.SetActive(false);
            loginForm.SetActive(true);
            isPlayerSigningUp = false;
            toggleButtonText.text = "Don't have an account? Sign up";
                  
        }
        else
        {
            loginForm.SetActive(false);
            signupForm.SetActive(true);
            isPlayerSigningUp = true;
            toggleButtonText.text = "Already have an account? Log in"; 
        }
    }
}
