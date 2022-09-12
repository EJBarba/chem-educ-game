using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class PlayFabManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject rowPrefab;
    public Transform rowsParent;
    //public GameObject playerNameForm;
    public GameObject leaderboardTable;
    //public GameObject refreshButton;
    //public TMP_InputField playerNameInput;
    //public GameObject crossWordButton;
    //public GameObject playerNamePanel;
    public TMP_Text welcomeText;

    public TMP_InputField emailInputField;
    public TMP_InputField usernameInputField;
    public TMP_InputField passwordInputField;
    public TMP_Text messageText;

    public GameObject formPanel;
    public GameObject crosswordButton;
    public GameObject archeryButton;
    public GameObject loginsignupButton;

    void Start()
    {
        //Login();
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
        string name = null;
        if (result.InfoResultPayload.PlayerProfile != null)
        {
            name = result.InfoResultPayload.PlayerProfile.DisplayName;
            welcomeText.text = "Welcome " + name + " !";   
        }
        if (name == null)
        {
            // leaderboardTable.SetActive(false);
            // refreshButton.SetActive(false);
            //playerNameForm.SetActive(true);

            //playerNamePanel.SetActive(true);
            //crossWordButton.SetActive(false);
        }
        else
        {
            GetLeaderboard();
        }
    }

    public void SubmitUserName()
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = usernameInputField.text
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdate, OnError);
    }

    void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("Updated display name!");
        // leaderboardTable.SetActive(true);
        // refreshButton.SetActive(true);
        //playerNameForm.SetActive(false);
        
        //playerNamePanel.SetActive(false);
        //crossWordButton.SetActive(true);
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
        if (passwordInputField.text.Length < 6)
        {
            messageText.text = "Password too short.";
            return;
        }
        var request = new RegisterPlayFabUserRequest
        {
            Email = emailInputField.text,
            DisplayName = usernameInputField.text,
            Password = passwordInputField.text,
            RequireBothUsernameAndEmail = false
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnError);
        
    }

    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("Successfully registered and logged in!");
        messageText.text = "";
        welcomeText.text = "Welcome " + usernameInputField.text + " !"; 

        emailInputField.text = "";
        usernameInputField.text = "";
        passwordInputField.text = "";
        
        formPanel.SetActive(false);

        crosswordButton.SetActive(true);
        archeryButton.SetActive(true);
        loginsignupButton.SetActive(true);
    }
}
