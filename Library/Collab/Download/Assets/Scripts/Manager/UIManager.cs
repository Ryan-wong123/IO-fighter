using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using TMPro;

public class UIManager : MonoBehaviour
{
    //text for leaderboard

    public Text firstPlayer;
    public Text secondPlayer;
    public Text thirdPlayer;
    public Text fourthPlayer;
    public Text fifthPlayer;

    public Text firstScore;
    public Text secondScore;
    public Text thirdScore;
    public Text fourthScore;
    public Text fifthScore;

    public TextMeshProUGUI deathScore;

    private void LateUpdate()
    {
        UpdateLeaderboardUI();
        UpgradeSystemButtons();

        //show deathscore
        deathScore.text = "Total Kills: " + PhotonNetwork.LocalPlayer.GetScore().ToString();
    }

    private void UpdateLeaderboardUI()
    {
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        List<string> places = Leaderboard.GetPlaces();
        List<int> scorePlaces = Leaderboard.GetScorePlace();

        if (playerCount > 0)
        {
            firstPlayer.text = places[0];
            firstScore.text = scorePlaces[0].ToString();
        }
        if (playerCount > 1)
        {
            secondPlayer.text = places[1];
            secondScore.text = scorePlaces[1].ToString();
        }
        if (playerCount > 2)
        {
            thirdPlayer.text = places[2];
            thirdScore.text = scorePlaces[2].ToString();
        }
        if (playerCount > 3)
        {
            fourthPlayer.text = places[3];
            fourthScore.text = scorePlaces[3].ToString();
        }
        if (playerCount > 4)
        {
            fifthPlayer.text = places[4];
            fifthScore.text = scorePlaces[4].ToString();
        }
    }

    private void UpgradeSystemButtons()
    {
        int playerCurrentScore = PhotonNetwork.LocalPlayer.GetScore();

        switch (playerCurrentScore)
        {
            case 10:
                //choose between fire or ice element and change the button sprite
                break;

            case 20:
                //allow the player to get the next upgrade
                break;

            case 30:
                //allow the player to get the next upgrade
                break;

            case 40:
                //allow the player to get the next upgrade
                break;

            default:
                Debug.Log("not time to upgrade");
                break;
        }
    }
}