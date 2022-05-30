using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace ziv
{
    public class PlayFabAuth : MonoBehaviour
    {
        //Ui Elements for first Screens
        [Header("UI")]
        public Text MessageText;
        public Text DisplayNamet;
        public InputField DisplayNamex;
        public InputField passswordInput;
        public InputField EmailInput;
        public GameObject Screen1;
        public GameObject Screen2;


        //Variables for other classes to use
        public static string displayname = "";
        public static int TimesDied = 0;
        public static string groupname = "";

    //MonoBehaviour Method
        void Start()
        {
            Screen1.SetActive(true);
            Screen2.SetActive(false);
        }


        //PlayFabScreen Methods
        public void RegisterButton()
        {
            if (passswordInput.text.Length < 6)
            {   
                MessageText.text = "Password too short!";
                return;
            }
            var request = new RegisterPlayFabUserRequest
            {
                Email = EmailInput.text,
                Password = passswordInput.text,
                RequireBothUsernameAndEmail = false
            };

            PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnError);
        }

        public void LoginButton()
        {
            var request = new LoginWithEmailAddressRequest
            {
                Email = EmailInput.text,
                Password = passswordInput.text,
                InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
                {
                    GetPlayerProfile = true
                }
            };

            PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnError);
        }

        public void ResetPasswordButton()
        {
            var request = new SendAccountRecoveryEmailRequest
            {
                Email = EmailInput.text,
                TitleId = "1F028"
            };

            PlayFabClientAPI.SendAccountRecoveryEmail(request, OnRecoverySuccess, OnError);
        }

        void OnError(PlayFabError errorr)
        {
            MessageText.text = "Error!" + errorr;
        }

        void OnRecoverySuccess(SendAccountRecoveryEmailResult result)
        {
            MessageText.text = "password reset was sent to mail";
        }

        void OnDisplaySuccess(UpdateUserTitleDisplayNameResult result)
        {
            Debug.Log("Success");
        }

        void OnRegisterSuccess(RegisterPlayFabUserResult result)
        {
            MessageText.text = "Registered and logged";
        }

        void EnterGame()
        {
            SceneManager.LoadScene(sceneBuildIndex: 1);

        }

        void OnLoginSuccess(LoginResult result)
        {
            MessageText.text = "logged in";
            Screen1.SetActive(false);
            Screen2.SetActive(true);
            

            if (result.InfoResultPayload.PlayerProfile != null)
            {
                displayname = result.InfoResultPayload.PlayerProfile.DisplayName;
                DisplayNamet.text = "Currently you are: " +displayname;
                GetTimesDied();
                GetGroup();
            }
            else
            {
                DisplayNamet.text = "You Currently dont have a display name";
            }

            
        }

        public void UpdateDisplayName()
        {
            if (DisplayNamex != null)
            {
                if (DisplayNamex.text != null)
                {
                    var request = new UpdateUserTitleDisplayNameRequest
                    {
                        DisplayName = DisplayNamex.text,
                    };
                    PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplaySuccess, OnError);
                }
            }
            EnterGame();
        }

        //static methods
        public static void OnDataSent(UpdateUserDataResult result)
        {
            Debug.Log("data changed");
        }

        public void OnTimeRecived(GetUserDataResult result)
        {
            if (result.Data != null && result.Data.ContainsKey("deathamount"))
            {
                TimesDied = int.Parse(result.Data["deathamount"].Value);
                Debug.Log(TimesDied);
            }
        }

        public void OnGroupRecived(GetUserDataResult result)
        {
            if (result.Data != null && result.Data.ContainsKey("groupname"))
            {
                groupname = result.Data["groupname"].Value;
                Debug.Log(TimesDied);
            }
            else
            {
                groupname = "pacafist";
                UpdateUserGroup();
            }
        }

        public static void OnError2(PlayFabError errorr)
        {
            Debug.Log("Error");
        }

        public static string returndisplayname()
        {
            return displayname;
        }

        public static void UpdateTimesDied()
        {
            var request = new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>
                {
                    { "deathamount", TimesDied.ToString() }

                }
            };
            PlayFabClientAPI.UpdateUserData(request, OnDataSent, OnError2);
            Debug.Log("UPDATES");
        }

        private void UpdateUserGroup()
        {
            var request = new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>
                {
                    { "groupname", groupname }

                }
            };
            PlayFabClientAPI.UpdateUserData(request, OnDataSent, OnError2);
            Debug.Log("UPDATES");
        }

        public void GetTimesDied()
        {
            PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnTimeRecived, OnError2);
        }


        public void GetGroup()
        {
            PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnGroupRecived, OnError2);
        }

    }
}
