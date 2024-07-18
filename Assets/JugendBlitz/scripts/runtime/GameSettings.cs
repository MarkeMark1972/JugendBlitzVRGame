using System.Collections.Generic;
using UnityEngine;

namespace JugendBlitz.scripts.runtime
{
    public class GameSettings : MonoBehaviour
    {
        public static GameSettings instance { get; private set; }

        [Header("Game Settings")] 
        public int CorrectItemCounter;
        
        [Header("Bin Settings")] 
        public List<Color> binColours;
        public Dictionary<TargetBinType, Color> ColourDictionary;

        private void OnEnable()
        {
            instance = this;
            SetupColourDictionary();
        }

        private void SetupColourDictionary()
        {
            ColourDictionary = new Dictionary<TargetBinType, Color>();

            for (var i = 0; i < binColours.Count; i++)
            {
                ColourDictionary[(TargetBinType)i] = binColours[i];
            }
        }
    }
}