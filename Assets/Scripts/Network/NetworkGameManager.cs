using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using System.Collections.Generic;

public class NetworkGameManager : NetworkBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private GameObject _aiControllerPrefab;

    // Dizionario che mappa gli ID dei giocatori ai loro GameObject
    private Dictionary<int, GameObject> _players = new Dictionary<int, GameObject>();

    // Flag per il gioco contro IA
    private bool _playingAgainstAI = false;

    private void Start()
    {
        // Verifica se stiamo giocando contro l'IA
        _playingAgainstAI = PlayerPrefs.GetInt("PlayingAgainstAI", 0) == 1;

        // Se non siamo online e giochiamo contro l'IA, avvia il gioco offline
        if (_playingAgainstAI && NetworkBootstrap.Instance != null &&
            !NetworkBootstrap.Instance.IsServer() && !NetworkBootstrap.Instance.IsClient())
        {
            StartOfflineGame();
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("Server started. Waiting for clients...");
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log("Client started.");
    }

    // Metodo chiamato quando un client si connette al server
    public override void OnSpawnServer(NetworkConnection connection)
    {
        base.OnSpawnServer(connection);

        // Ottieni l'ID del giocatore (ClientId + 1)
        int playerId = (int)connection.ClientId + 1;

        // Determina la posizione di spawn
        int spawnIndex = playerId - 1;
        if (spawnIndex >= 0 && spawnIndex < _spawnPoints.Length)
        {
            // Crea il giocatore
            GameObject playerObj = Instantiate(_playerPrefab, _spawnPoints[spawnIndex].position, Quaternion.identity);

            // Ottieni il componente NetworkPlayer
            NetworkPlayer networkPlayer = playerObj.GetComponent<NetworkPlayer>();
            if (networkPlayer != null)
            {
                networkPlayer.SetPlayerId(playerId);
            }

            // Spawna il giocatore nel network e assegna l'ownership
            Spawn(playerObj, connection);

            // Aggiungi al dizionario
            _players[playerId] = playerObj;

            Debug.Log($"Player {playerId} spawned");
        }
    }

    private void StartOfflineGame()
    {
        Debug.Log("Starting offline game against AI");

        // Crea il giocatore locale
        GameObject playerObj = Instantiate(_playerPrefab, _spawnPoints[0].position, Quaternion.identity);
        NetworkPlayer playerComponent = playerObj.GetComponent<NetworkPlayer>();
        if (playerComponent != null)
        {
            // Impostalo manualmente come Player 1 (in modalità offline)
            playerComponent.SetPlayerId(1);
        }

        // Crea l'IA
        if (_aiControllerPrefab != null)
        {
            GameObject aiController = Instantiate(_aiControllerPrefab);

            // Se l'IA ha bisogno del riferimento al giocatore
            AIController aiComponent = aiController.GetComponent<AIController>();
            if (aiComponent != null)
            {
                aiComponent.SetPlayer(playerObj);
            }

            // Crea anche un'unità controllata dall'IA
            GameObject aiUnit = Instantiate(_playerPrefab, _spawnPoints[1].position, Quaternion.identity);
            NetworkPlayer aiUnitComponent = aiUnit.GetComponent<NetworkPlayer>();
            if (aiUnitComponent != null)
            {
                aiUnitComponent.SetPlayerId(2);
            }

            // Se l'IA ha bisogno del riferimento alla sua unità
            if (aiComponent != null)
            {
                aiComponent.SetAIUnit(aiUnit);
            }
        }
    }
}