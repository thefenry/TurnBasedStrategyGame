using System;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    private int _turnNumber = 1;
    private bool _isPlayerTurn = true;

    public static TurnSystem Instance { get; private set; }

    public event EventHandler OnTurnChanged;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"There's more than one TurnSystem. {transform} - {Instance}");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }


    public void NextTurn()
    {
        _isPlayerTurn = !_isPlayerTurn;
        
        if (_isPlayerTurn) { _turnNumber++; }

        OnTurnChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetTurnNumber()
    {
        return _turnNumber;
    }

    public bool IsPlayerTurn()
    {
        return _isPlayerTurn;
    }
}
