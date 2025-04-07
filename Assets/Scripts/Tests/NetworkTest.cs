using UnityEngine;
using FishNet.Managing;
using UnityEngine.InputSystem;

public class NetworkTest : MonoBehaviour
{
    [SerializeField] private Key _hostKey = Key.H;
    [SerializeField] private Key _clientKey = Key.C;

    private NetworkManager _networkManager;
    private Keyboard _keyboard;

    private void Awake()
    {
        _networkManager = FindObjectOfType<NetworkManager>();
        _keyboard = Keyboard.current;
    }

    private void Update()
    {
        if (_networkManager == null || _keyboard == null) return;

        // Avvia come host (server + client)
        if (_keyboard[_hostKey].wasPressedThisFrame)
        {
            _networkManager.ServerManager.StartConnection();
            _networkManager.ClientManager.StartConnection();
            Debug.Log("Started as Host");
        }

        // Avvia come client
        if (_keyboard[_clientKey].wasPressedThisFrame)
        {
            _networkManager.ClientManager.StartConnection();
            Debug.Log("Started as Client");
        }
    }
}