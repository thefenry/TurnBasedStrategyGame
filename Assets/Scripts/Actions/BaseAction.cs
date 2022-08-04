using UnityEngine;

public abstract class BaseAction : MonoBehaviour
{
    protected bool IsActionActive;
    protected Unit Unit;

    protected  virtual void Awake()
    {
        Unit = GetComponent<Unit>();
    }
}
