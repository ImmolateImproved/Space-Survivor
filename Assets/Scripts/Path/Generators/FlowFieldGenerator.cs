using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/FlowFieldGenerator", fileName = "FlowFieldGenerator")]
public class FlowFieldGenerator : PathGeneratorBase
{
    public override Vector3 GetNextDirection(Vector2 currentPosition)
    {
        var offset = currentPosition * scale;
        var n = noise.snoise(offset);
        
        var angle = n * 2 * Mathf.PI;

        var x = Mathf.Cos(angle);
        var z = Mathf.Sin(angle);

        var direction = new Vector3(x, 0, z);

        return direction;
    }
}