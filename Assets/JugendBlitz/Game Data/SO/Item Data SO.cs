using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace JugendBlitz.Game_Data.SO
{
    [InlineEditor, CreateAssetMenu(fileName = "ItemDataSo", menuName = "ScriptableObjects/ItemDataSo")]
    public class ItemDataSo : SerializedScriptableObject
    {
        [BoxGroup("Difficulty Settings")]
        
        [BoxGroup("Difficulty Settings/Easy Settings"), LabelWidth(200)]
        public int trashPrefabsEasyCounter;
        [BoxGroup("Difficulty Settings/Easy Settings")]
        public List<Transform> trashPrefabsEasy;

        [BoxGroup("Difficulty Settings/Medium Settings"), LabelWidth(200)]
        public int trashPrefabsMediumCounter;
        [BoxGroup("Difficulty Settings/Medium Settings")]
        public List<Transform> trashPrefabsMedium;

        [BoxGroup("Difficulty Settings/Hard Settings"), LabelWidth(200)]
        public int trashPrefabsHardCounter;
        [BoxGroup("Difficulty Settings/Hard Settings")]
        public List<Transform> trashPrefabsHard;

        [BoxGroup("Settings")]
        public Transform trashAudioSpherePrefabs;

        // [SerializeField] public List<GameObject> trashCollection;
        [BoxGroup("Settings")]
        public float spawnInterval = 4.0f;
        [BoxGroup("Settings")]
        public Transform itemSpawnPoint;

        [BoxGroup("Settings")]
        public List<Transform> audioSphereSpawnPoint;

        // [SerializeField] public TMP_Text itemsLeftReadout;
        [BoxGroup("Settings")]
        public GameObject blocker;
        [BoxGroup("Settings")]
        public Button restartHiddenImage;

        [BoxGroup("Settings")]
        public float duration = 0.2f;
    }
}