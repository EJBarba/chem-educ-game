using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class PlayFabManager : MonoBehaviour
{
    [Header("Leaderboard")]
    public GameObject rowPrefab;
    public Transform rowsParent;
    public GameObject leaderboardTable;

    [Header("Form")]
    public GameObject formPanel;
    public TMP_Text messageText;

    [Header("Sign Up Form")]
    public TMP_InputField signUpEmailInputField;
    public TMP_InputField signUpUsernameInputField;
    public TMP_InputField signUpPasswordInputField;

    [Header("Log In Form")]
    public TMP_InputField logInEmailInputField;
    public TMP_InputField logInPasswordInputField;
  
    
    [Header("Menu")]
    public GameObject crosswordButton;
    public GameObject archeryButton;
    public GameObject loginsignupButton;
    public TMP_Text welcomeText;
    public GameObject logoutButton;
    void Start()
    {
        //Login();
        if (PlayerPrefs.GetString("email").Length > 0 && PlayerPrefs.GetString("password").Length > 0  )
        {
            logoutButton.SetActive(true);
            LoginPlayerPrefs();
        }
    }

    void Login()
    {
        var request = new LoginWithCustomIDRequest
            {
                CustomId = SystemInfo.deviceUniqueIdentifier,
                CreateAccount = true,
                InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
                {
                    GetPlayerProfile = true
                }        
            };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnError);
    }

    void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Successful login/account creation!");

        messageText.text = "";
        string name = null;
        // check if player profile exists, so it wont crash
        if (result.InfoResultPayload.PlayerProfile != null)
        {
            name = result.InfoResultPayload.PlayerProfile.DisplayName;
            welcomeText.text = "Welcome " + name + " !"; 
        }

        // check if PlayerPrefs is empty
        if (PlayerPrefs.GetString("email").Length <= 0 && PlayerPrefs.GetString("password").Length <= 0  )
        {
            PlayerPrefs.SetString("email", logInEmailInputField.text);
            PlayerPrefs.SetString("password", logInPasswordInputField.text);
            logInEmailInputField.text = "";
            logInPasswordInputField.text = "";

            formPanel.SetActive(false);

            crosswordButton.SetActive(true);
            archeryButton.SetActive(true);
            loginsignupButton.SetActive(true);
            logoutButton.SetActive(true);
        }
        
        GetLeaderboard();
        
    }

    // public void SubmitUserName()
    // {
    //     var request = new UpdateUserTitleDisplayNameRequest
    //     {
    //         DisplayName = usernameInputField.text
    //     };
    //     PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdate, OnError);
    // }

    void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("Updated display name!");
    }

    void OnError(PlayFabError error)
    {
        Debug.Log("Error while logging in/creating account");
        Debug.Log(error.GenerateErrorReport());
        messageText.text = (error.ErrorMessage);
    }

    public void SendLeaderBoard(int score)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "StudentScore",
                    Value = score
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderBoardUpdate, OnError);
    }

    void OnLeaderBoardUpdate(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Successful leaderboard score sent");
    }

    public void GetLeaderboard()
    {
        var request = new GetLeaderboardRequest
            {
                StatisticName = "StudentScore",
                StartPosition = 0,
                MaxResultsCount = 10
            };
            PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardGet, OnError);
    }

    void OnLeaderboardGet(GetLeaderboardResult result)
    {
         foreach (Transform child in leaderboardTable.transform) {
            GameObject.Destroy(child.gameObject);
        }
        foreach (var item in result.Leaderboard)
        {
            GameObject newRow = Instantiate(rowPrefab, rowsParent);
            TMP_Text[] texts = newRow.GetComponentsInChildren<TMP_Text>();
            texts[0].text = (item.Position + 1).ToString();
            texts[1].text = item.DisplayName;
            texts[2].text = item.StatValue.ToString();
            
            Debug.Log(item.Position + " " + item.PlayFabId + " " + item.StatValue);
        }
    }

    public void RegisterButton()
    {
        messageText.text = "";
        if (signUpPasswordInputField.text.Length < 6)
        {
            messageText.text = "Password too short.";
            return;
        }
        var request = new RegisterPlayFabUserRequest
        {
            Email = signUpEmailInputField.text,
            DisplayName = signUpUsernameInputField.text,
            Password = signUpPasswordInputField.text,
            RequireBothUsernameAndEmail = false
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnError);
        
    }

    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("Successfully registered and logged in!");
        messageText.text = "";
        welcomeText.text = "Welcome " + signUpUsernameInputField.text + " !"; 

        signUpEmailInputField.text = "";
        signUpUsernameInputField.text = "";
        signUpPasswordInputField.text = "";
        
        formPanel.SetActive(false);

        crosswordButton.SetActive(true);
        archeryButton.SetActive(true);
        loginsignupButton.SetActive(true);
    }

    public void LoginButton()
    {
        var request = new LoginWithEmailAddressRequest()
        {
            Email = logInEmailInputField.text,
            Password = logInPasswordInputField.text,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams { GetPlayerProfile = true }
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnError);
    }

    public void LoginPlayerPrefs()
    {
        var request = new LoginWithEmailAddressRequest()
        {
            Email = PlayerPrefs.GetString("email"),
            Password = PlayerPrefs.GetString("password"),
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams { GetPlayerProfile = true }
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnError);
    }

    public void LogoutButton()
    {
         PlayerPrefs.DeleteKey("email");
         PlayerPrefs.DeleteKey("password");
         welcomeText.text = "";
         logoutButton.SetActive(false);
    }
}
