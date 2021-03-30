using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    //room id interger
    private int roomName;

    //input to get the room id that user inputs
    public TMP_InputField roomName_;

    //username of player
    public TMP_InputField username;

    //text to show that the username is not keyed in
    public Text UserNameFeedbackText;

    //loading screen UI
    public Image loadingScreen;

    private void Awake()
    {
        ConnectToMaster();
    }

    //method to connect to the master server
    private void ConnectToMaster()
    {
        //check if this client is connected to master server
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("already connected to server");
            //exit function
            return;
        }
        else
        {
            //connect to master server using settings
            PhotonNetwork.ConnectUsingSettings();
            Debug.Log("connecting");
        }
    }

    //when the player is connected to the master server successfully
    public override void OnConnectedToMaster()
    {
        //hide the loading screen UI
        loadingScreen.enabled = false;
        Debug.Log("connected");

        //check if player is in lobby
        if (PhotonNetwork.InLobby)
        {
            //exit function
            return;
        }
        else
        {
            //join a lobby
            PhotonNetwork.JoinLobby(TypedLobby.Default);
        }
    }

    //when the player disconnects
    public override void OnDisconnected(DisconnectCause cause)
    {
        //try to reconnect to the master server
        ConnectToMaster();
        Debug.Log("disconnected");
    }

    //if the player joins the lobby successfully
    public override void OnJoinedLobby()
    {
        Debug.Log("Joined lobby");
    }

    //when player leaves the lobby
    public override void OnLeftLobby()
    {
        Debug.Log("left lobby");
    }

    //when player joins the room successfully
    public override void OnJoinedRoom()
    {
        Debug.Log("joined room successfully");
        //load scene to the gameplay room
        PhotonNetwork.LoadLevel(1);
    }

    //if player failed to join a random room
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Joined room failed");
        //create a new room and join it
        createRoom();
    }

    //when player failed to create a new room
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("create room failed");
        //retry to create another room
        createRoom();
    }

    //method to create a new room
    public void createRoom()
    {
        Debug.Log("creating new room");
        //generate a random id for the room
        int randRoom = Random.Range(0, 500);

        //room options for creating new room
        //set the max number of players that can join the room
        //make the room open and visible to other players to join if the max capacity is not reached
        RoomOptions roomOptions = new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = 20
        };

        //create the room with the random room id and room options
        PhotonNetwork.CreateRoom("Room" + randRoom, roomOptions);
        //set the text of username to the player username in the server
        PhotonNetwork.NickName = username.text;
        Debug.Log(randRoom);
    }

    //method to join a room
    public void joinRoom()
    {
        if (username.text == "")
        {
            UserNameFeedbackText.text = "Please enter a username";
            return;
        }

        //check if nothing is key into the room id's input field
        if (roomName_.text == "")
        {
            //set the text of username to the player username in the server
            PhotonNetwork.NickName = username.text;
            //join a random room
            PhotonNetwork.JoinRandomRoom();
        }

        //else get the random room id and join that specific room
        else
        {
            //set the text of username to the player username in the server
            PhotonNetwork.NickName = username.text;
            //convert the room id"s string into integer
            roomName = int.Parse(roomName_.text);
            //join that specific room with that room id
            PhotonNetwork.JoinRoom("Room" + roomName);
            Debug.Log("joining room: " + roomName);
        }
    }

    //method to leave room when player dies
    public void leaveRoom()
    {
        //leave room
        PhotonNetwork.LeaveRoom();
        //leave lobby
        PhotonNetwork.LeaveLobby();
        //load the main menu scene
        SceneManager.LoadScene(0);
    }

    public void quitGame()
    {
        Application.Quit();
    }
}