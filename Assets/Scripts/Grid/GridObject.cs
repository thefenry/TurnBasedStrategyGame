using System;
using System.Collections.Generic;
using System.Linq;

public class GridObject
{
    private GridSystem _gridSystem;
    private readonly GridPosition _gridPosition;
    private List<Unit> _unitList;

    public List<Unit> UnitList => _unitList;

    public GridObject(GridSystem gridSystem, GridPosition gridPosition)
    {
        _gridSystem = gridSystem;
        _gridPosition = gridPosition;
        _unitList = new List<Unit>();
    }

    public void AddUnit(Unit unit)
    {
        _unitList.Add(unit);
    }

    public void RemoveUnit(Unit unit)
    {
        _unitList.Remove(unit);
    }

    public override string ToString()
    {
        string unitString = _unitList.Aggregate(string.Empty, (current, unit) => current + (unit + "\n"));

        return $"{_gridPosition}\n{unitString}";
    }
}
