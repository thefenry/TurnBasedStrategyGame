using System;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour
{
    protected bool IsActionActive;
    protected Unit Unit;

    protected Action OnActionComplete;

    protected  virtual void Awake()
    {
        Unit = GetComponent<Unit>();
    }
}
