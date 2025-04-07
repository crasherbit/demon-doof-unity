using UnityEngine;

public class AIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private float _decisionInterval = 1.5f;

    // Riferimenti agli oggetti di gioco
    private GameObject _playerUnit;
    private GameObject _aiUnit;

    // Timer per le decisioni dell'IA
    private float _decisionTimer = 0f;

    private void Update()
    {
        // Aggiorna il timer per le decisioni
        _decisionTimer += Time.deltaTime;

        // Prendi una decisione ad intervalli regolari
        if (_decisionTimer >= _decisionInterval)
        {
            MakeDecision();
            _decisionTimer = 0f;
        }
    }

    private void MakeDecision()
    {
        // Qui implementerai la logica dell'IA
        // Per ora, facciamo un semplice movimento casuale

        if (_aiUnit == null) return;

        // Esempio: muove l'unità dell'IA in una direzione casuale
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f).normalized;
        _aiUnit.transform.position += randomDirection * 0.5f;

        Debug.Log("AI fece una mossa");
    }

    // Metodo per impostare il riferimento all'unità del giocatore
    public void SetPlayer(GameObject playerUnit)
    {
        _playerUnit = playerUnit;
    }

    // Metodo per impostare il riferimento all'unità dell'IA
    public void SetAIUnit(GameObject aiUnit)
    {
        _aiUnit = aiUnit;
    }

    // Implementa qui le tue strategie di IA
    // Per esempio:

    private void MoveTowardsPlayer()
    {
        if (_aiUnit == null || _playerUnit == null) return;

        Vector3 direction = (_playerUnit.transform.position - _aiUnit.transform.position).normalized;
        _aiUnit.transform.position += direction * 0.3f;
    }

    private void MoveAwayFromPlayer()
    {
        if (_aiUnit == null || _playerUnit == null) return;

        Vector3 direction = (_aiUnit.transform.position - _playerUnit.transform.position).normalized;
        _aiUnit.transform.position += direction * 0.3f;
    }
}