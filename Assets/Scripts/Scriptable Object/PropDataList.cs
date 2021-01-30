using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPropDataList", menuName = "Tool/Create Prop Data List")]
public class PropDataList : ScriptableObject
{
    [SerializeField] private List<PropData> list = new List<PropData>();

    public PropData GetPropDataByName(string value)
    {
        return list.Find(x => x != null && x.name.Contains(value));
    }
}
