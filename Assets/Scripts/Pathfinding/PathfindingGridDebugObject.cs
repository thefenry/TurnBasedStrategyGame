using TMPro;
using UnityEngine;

public class PathfindingGridDebugObject : GridDebugObject
{
    [SerializeField] private TextMeshPro gCostText;
    [SerializeField] private TextMeshPro hCostText;
    [SerializeField] private TextMeshPro fCostText;
    [SerializeField] private SpriteRenderer isWalkableSpriteRenderer;

    private PathNode _pathNode;

    public override void SetGridObject(object gridObject)
    {
        base.SetGridObject(gridObject);
        _pathNode = (PathNode)gridObject;
    }

    protected override void Update()
    {
        base.Update();
        gCostText.text = _pathNode.GCost.ToString();
        hCostText.text = _pathNode.HCost.ToString();
        fCostText.text = _pathNode.FCost.ToString();

        if (!_pathNode.IsWalkable)
        {
            isWalkableSpriteRenderer.color = Color.red;
        }
        else
        {
            isWalkableSpriteRenderer.color = Color.green;
            isWalkableSpriteRenderer.enabled = false;
        }
    }
}
