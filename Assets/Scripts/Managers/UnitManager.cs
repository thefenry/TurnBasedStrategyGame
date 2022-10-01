using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance { get; private set; }

    private List<Unit> _unitList;
    private List<Unit> _friendlyUnits;
    private List<Unit> _enemyUnits;

    private void Awake()
    {
        _unitList = new List<Unit>();
        _friendlyUnits = new List<Unit>();
        _enemyUnits = new List<Unit>();

        if (Instance != null)
        {
            Debug.LogError("There has been another instance of UnitManager detected");
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        Unit.OnAnySpawned += Unit_OnAnySpawned;
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;
    }

    private void Unit_OnAnySpawned(object sender, EventArgs e)
    {
        Unit unit = sender as Unit;
        Debug.Log($"{unit} spawned");

        _unitList.Add(unit);
        if (unit.IsEnemy())
        {
            _enemyUnits.Add(unit);
        }
        else
        {
            _friendlyUnits.Add(unit);
        }
    }

    private void Unit_OnAnyUnitDead(object sender, EventArgs e)
    {
        Unit unit = sender as Unit;

        Debug.Log($"{unit} died");

        _unitList.Remove(unit);
        if (unit.IsEnemy())
        {
            _enemyUnits.Remove(unit);
        }
        else
        {
            _friendlyUnits.Remove(unit);
        }
    }

    public List<Unit> GetUnitList()
    {
        return _unitList;
    }

    public List<Unit> GetEnemyUnitList()
    {
        return _enemyUnits;
    }

    public List<Unit> GetFriendlyUnitList()
    {
        return _friendlyUnits;
    }
}
