using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField newRoomInputField;
    [SerializeField] TMP_Text feedbackText;
    [SerializeField] Button startGameButton;
    [SerializeField] GameObject roomPanel;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] GameObject RoomListObject;
    [SerializeField] GameObject playerListObject;
    [SerializeField] RoomItem roomItemPrefab;
    [SerializeField] PlayerItem playerItemPrefabs;

    List<RoomItem> roomItemList = new List<RoomItem>();
    List<PlayerItem> playerItemlist = new List<PlayerItem>();

    Dictionary<string, RoomInfo> roomInfoCache = new Dictionary<string, RoomInfo>();


    private void Start()
    {
        PhotonNetwork.JoinLobby();
        roomPanel.SetActive(false);
    }

    public void ClickCreateRoom()
    {
        feedbackText.text = "";
        if (newRoomInputField.text.Length < 3)
        {
            feedbackText.text = "Room name min 3 characters";
            return;
        }
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 5;
        PhotonNetwork.CreateRoom(newRoomInputField.text);
    }

    public void ClickStartGame(string levelName)
    {

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.LoadLevel(levelName);
        }
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room : " + PhotonNetwork.CurrentRoom.Name);
        feedbackText.text = "Created room : " + PhotonNetwork.CurrentRoom.Name;
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room : " + PhotonNetwork.CurrentRoom.Name);
        feedbackText.text = "Joined room : " + PhotonNetwork.CurrentRoom.Name;
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        roomPanel.SetActive(true);

        //Update player list 
        UpdatePlayerList();

        //mengatur start game button
        SetStartGameButton();
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        //Update player list 
        UpdatePlayerList();
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        //Update player list 
        UpdatePlayerList();
    }
    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        //atur start game button
        SetStartGameButton();
    }

    private void SetStartGameButton()
    {
        //tombol master hanya muncul di master client
        startGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);

        //tombol start game bisa diklik ketika player di room lebih dari
        startGameButton.interactable = PhotonNetwork.CurrentRoom.PlayerCount > 1;
    }
    private void UpdatePlayerList()
    {

        //destroy terlebih dahulu semua player item yang sudah ada
        foreach (var item in playerItemlist)
        {
            Destroy(item.gameObject);
        }
        playerItemlist.Clear();


        //bikin ulang player list
        // PhotonNetwork.CurrentRoom.Players (alternative)
        foreach (var (id, player) in PhotonNetwork.CurrentRoom.Players)
        {
            PlayerItem newPlayerItem = Instantiate(playerItemPrefabs, playerListObject.transform);

            newPlayerItem.Set(player);
            playerItemlist.Add(newPlayerItem);

            if (player == PhotonNetwork.LocalPlayer)
                newPlayerItem.transform.SetAsFirstSibling();
        }

        // PhotonNetwork.PlayerList 
        // foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList) { };

        //start game hanya bisa diklik ketika jumlah min peserta terpenuhi 
        SetStartGameButton();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (var roomInfo in roomList)
        {
            roomInfoCache[roomInfo.Name] = roomInfo;
        }

        Debug.Log("Room Updated");

        foreach (var item in this.roomItemList)
        {
            Destroy(item.gameObject);
        }

        this.roomItemList.Clear();

        var roomInfoList = new List<RoomInfo>(roomInfoCache.Count);


        //sort yang open diadd pertama
        ;
        foreach (var roomInfo in roomInfoCache.Values)
        {
            if (roomInfo.IsOpen)
                roomInfoList.Add(roomInfo);
        }


        foreach (var roomInfo in roomInfoCache.Values)
        {
            if (roomInfo.IsOpen == false)
                roomInfoList.Add(roomInfo);
        }


        foreach (var roomInfo in roomInfoList)
        {//|| roomInfo.MaxPlayers == 0 (tambahkan ini ketika masalah get max player beres)
            if (roomInfo.IsVisible == false)
                continue;

            RoomItem newRoomItem = Instantiate(roomItemPrefab, RoomListObject.transform);
            newRoomItem.Set(this, roomInfo);
            this.roomItemList.Add(newRoomItem);
        }
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
