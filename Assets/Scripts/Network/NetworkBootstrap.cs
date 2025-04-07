using UnityEngine;
using FishNet.Managing;

public class NetworkBootstrap : MonoBehaviour
{
    // Singleton
    public static NetworkBootstrap Instance { get; private set; }

    [SerializeField] private NetworkManager _networkManager;

    private void Awake()
    {
        // Implementazione del singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Trova il NetworkManager se non è assegnato
        if (_networkManager == null)
        {
            _networkManager = FindObjectOfType<NetworkManager>();
        }
    }

    public void StartHost()
    {
        if (_networkManager == null) return;

        _networkManager.ServerManager.StartConnection();
        _networkManager.ClientManager.StartConnection();

        Debug.Log("Host started!");
    }

    public void StartClient()
    {
        if (_networkManager == null) return;

        _networkManager.ClientManager.StartConnection();

        Debug.Log("Client started!");
    }

    public void StopNetwork()
    {
        if (_networkManager == null) return;

        if (_networkManager.ServerManager.Started)
            _networkManager.ServerManager.StopConnection(true);

        if (_networkManager.ClientManager.Started)
            _networkManager.ClientManager.StopConnection();

        Debug.Log("Network stopped!");
    }

    public bool IsServer()
    {
        return _networkManager != null && _networkManager.IsServer;
    }

    public bool IsClient()
    {
        return _networkManager != null && _networkManager.IsClient;
    }
}