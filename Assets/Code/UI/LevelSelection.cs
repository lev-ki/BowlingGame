using Code.DataContainers;
using System.Collections.Generic;
using UnityEngine;

namespace Code.UI
{
    public class LevelSelection : MonoBehaviour
    {
        [SerializeField] private Transform levelListParent;
        [SerializeField] private LevelSelector levelSelectorPrefab;
        
        private List<LevelSelector> selectors;

        public void Initialize()
        {
            selectors = new List<LevelSelector>();
            for (int i = 0; i < ProgressionContainer.Instance.levels.Count; i++)
            {
                LevelSelector instance = Instantiate(levelSelectorPrefab, levelListParent);
                instance.InitializeSelector(i);
                selectors.Add(instance);
            }
        }

        public void UpdateScore(int level, float score)
        {
            if (level >= 0 && level < selectors.Count)
            {
                selectors[level].SetScore(score);
            }
        }
    }
}