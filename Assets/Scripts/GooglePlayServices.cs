using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class GooglePlayServices : MonoBehaviour
{
    [HideInInspector] public bool connectedToGooglePlay = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        PlayGamesPlatform.Activate();
    }

    // Start is called before the first frame update
    void Start()
    {
        LoginToGooglePlay();
    }

    private void LoginToGooglePlay()
    {
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    public void ShowLeaderboard()
    {
        if (!connectedToGooglePlay)
        {
            LoginToGooglePlay();
        }
        Social.ShowLeaderboardUI();
    }

    private void ProcessAuthentication(SignInStatus status)
    {
        connectedToGooglePlay = status == SignInStatus.Success ? true : false;
    }
}
