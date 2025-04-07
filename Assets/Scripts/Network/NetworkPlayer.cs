using UnityEngine;
using FishNet.Object;

public class NetworkPlayer : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer _spriteRenderer;

    [Header("Settings")]
    [SerializeField] private Color _player1Color = Color.blue;
    [SerializeField] private Color _player2Color = Color.red;

    // ID del giocatore (1 o 2)
    private int _playerId = 0;

    // Proprietà per accedere all'ID del giocatore
    public int PlayerId => _playerId;

    public override void OnStartClient()
    {
        base.OnStartClient();

        // Se siamo il server, non serve richiedere l'ID
        if (IsServer) return;

        // Se siamo un client, chiedi al server l'ID
        RequestPlayerIdServerRpc();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        // Assegna l'ID del giocatore in base all'indice del client
        if (Owner != null && Owner.IsValid)
        {
            // ClientId parte da 0, ma vogliamo Player1 e Player2
            _playerId = (int)Owner.ClientId + 1;

            // Invia l'ID a tutti i client
            SyncPlayerIdToClientsObserversRpc(_playerId);
        }
    }

    // RPC chiamata dal client per richiedere l'ID al server
    [ServerRpc]
    private void RequestPlayerIdServerRpc()
    {
        if (!IsServer) return;

        // Invia l'ID a questo client specifico
        SyncPlayerIdToClientsObserversRpc(_playerId);
    }

    // RPC per sincronizzare l'ID a tutti i client
    [ObserversRpc]
    private void SyncPlayerIdToClientsObserversRpc(int id)
    {
        // Aggiorna l'ID
        _playerId = id;

        // Aggiorna l'aspetto visivo
        UpdateVisuals();
    }

    // Metodo per impostare l'ID del giocatore (solo lato server)
    public void SetPlayerId(int id)
    {
        if (!IsServer) return;

        _playerId = id;

        // Sincronizza con tutti i client
        SyncPlayerIdToClientsObserversRpc(_playerId);
    }

    private void UpdateVisuals()
    {
        if (_spriteRenderer == null) return;

        // Imposta il colore in base all'ID del giocatore
        if (_playerId == 1)
        {
            _spriteRenderer.color = _player1Color;
        }
        else if (_playerId == 2)
        {
            _spriteRenderer.color = _player2Color;
        }

        Debug.Log($"Player ID set to {_playerId}, color updated.");
    }

    [ServerRpc]
    public void MoveServerRpc(Vector2 direction)
    {
        // Esempio di movimento controllato dal server
        if (!IsServer) return;

        // Muovi il player
        transform.position += new Vector3(direction.x, direction.y, 0) * 5f * Time.deltaTime;
    }
}