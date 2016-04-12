using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public int minPlayerCount = 1;
    public bool ready, started;
    public int maxLevelCount = 12;
    public Color[] colors;
    List<Kitty> kitties;
    public GameObject kittyPrefab;
    public GameObject swagPrefab;
    public float minSwagXLocation, maxSwagXLocation;

    [Header("UI")]
    public GameObject infoPanel;
    public Text playersConnected;
    public Text lastWinner;

    void Awake()
    {
        if (instance != null) { Destroy(this); }
        else { instance = this; }
        ready = false;
        started = false;
        kitties = new List<Kitty>();
        lastWinner.gameObject.SetActive(false);
        AirConsole.instance.onMessage += OnMessage;
        AirConsole.instance.onConnect += OnConnect;
        AirConsole.instance.onDisconnect += OnDisconnect;
    }

    #region AirConsole
    // Start the game if 2 players are connected, game not running (activePlayers == null)
    void OnConnect(int device_id)
    {
        Debug.Log("Connect " + device_id);
        UpdatePlayerCount();
        if (AirConsole.instance.GetActivePlayerDeviceIds.Count == 0)
        {
            if (AirConsole.instance.GetControllerDeviceIds().Count >= minPlayerCount)
            {
                // Ready to play
                Debug.Log("READY TO PLAY");
                ready = true;
            }
            else
            {
                ready = false;
                Debug.Log("NOT ENOUGH PLAYERS");
            }
        }
    }

    void OnDisconnect(int device_id)
    {
        int activePlayer = AirConsole.instance.ConvertDeviceIdToPlayerNumber(device_id);
        Debug.Log("Disconnect " + activePlayer);
        if (AirConsole.instance.GetControllerDeviceIds().Count < minPlayerCount) { ready = false; }
        UpdatePlayerCount();
    }

    void OnMessage(int device_id, JToken data)
    {
        int activePlayer = AirConsole.instance.ConvertDeviceIdToPlayerNumber(device_id);
        string button = (string)data["b"];
        Debug.Log("Message from " + device_id + "(" + activePlayer + "): " + button);

        if (started)
        {
            if (activePlayer != -1)
            {
                if (button == "a") { ButtonPress("a", activePlayer); }
                else if (button == "b") { ButtonPress("b", activePlayer); }
            }
        }
        else
        {
            if (button == "s")
            {
                StartGame();
            }
        }
    }
    #endregion

    void ButtonPress(string button, int player)
    {
        Kitty selected = kitties.Find(x => x.player.Equals(player));
        selected.Button(button);
    }

    void StartGame()
    {
        started = true;
        infoPanel.SetActive(false);
        StartCoroutine(SpawnSwag());
        Debug.Log("Starting Game");
        int playerCount = AirConsole.instance.GetControllerDeviceIds().Count;
        Debug.Log("Player count: " + playerCount);
        AirConsole.instance.SetActivePlayers(playerCount);
        //Create enough kitties
        kitties.Clear();
        Kitty kittyI;
        GameObject kitty;
        for (int i = 0; i < playerCount; i++)
        {
            Debug.Log("gen kitty " + i);
            kitty = Instantiate(kittyPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            kittyI = kitty.GetComponent<Kitty>();
            kittyI.player = i;
            kittyI.color = colors[i];
            kitties.Add(kittyI);
        }
    }

    void EndGame()
    {
        started = false;
        infoPanel.SetActive(true);
        UpdatePlayerCount();
        UpdateLastWinner(1, 100);
    }

    void UpdatePlayerCount()
    {
        playersConnected.text = "Players connected: " + AirConsole.instance.GetControllerDeviceIds().Count;
    }

    void UpdateLastWinner(int player, int score)
    {
        lastWinner.gameObject.SetActive(true);
        lastWinner.text = "Last winner: Kitty " + player + ", Swag " + score;
    }

    IEnumerator SpawnSwag()
    {
        while (started)
        {
            Instantiate(swagPrefab, new Vector3(Random.Range(minSwagXLocation, maxSwagXLocation), Random.Range(0, maxLevelCount), 1), Quaternion.identity);
            yield return new WaitForSeconds(1.5f);
        }

    }
}
