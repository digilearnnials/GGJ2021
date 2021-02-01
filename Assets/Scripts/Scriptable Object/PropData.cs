using UnityEngine;

[CreateAssetMenu(fileName = "NewPropData", menuName = "Tool/Create Prop Data")]
public class PropData : ScriptableObject
{
    public Mesh mesh;
    public Material material;
}
