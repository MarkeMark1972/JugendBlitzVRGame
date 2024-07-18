using UnityEngine;

namespace Prefabs.Produce.SO_Data
{
    [CreateAssetMenu(fileName = "NewProduceData", menuName = "Produce/ProduceData")]
    public class ProduceData : ScriptableObject
    {
        public string produceName;
        [Space] public TargetBinType targetBinType;
        [Space] public GameObject connectedPrefab;
        public TargetBinType connectedTargetBinType;
        [Space] public int winPoints;
    }
}

public enum TargetBinType
{
    Paper,
    Glass,
    PlasticAndMetal,
    Organic,
    Special
}