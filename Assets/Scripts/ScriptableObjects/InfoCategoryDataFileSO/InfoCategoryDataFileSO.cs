using UnityEngine;

public enum DataInterpretationMode
{
    FROM_SPECIFIED_COUNTRY, // e.g. EXPORT FROM AUSTRIA TO OTHER COUNTRIES 
    TO_SPECIFIED_COUNTRY // e.g. IMPORT TO AUSTRIA FROM OTHER COUNTRIES
}

namespace ScriptableObjects.InfoCategoryDataFileSO
{
    [CreateAssetMenu(fileName = "InfoCategoryDataFileSO", menuName = "ScriptableObjects/InfoCategoryDataFileSO", order = 3)]
    public class InfoCategoryDataFileSO
    {
        public Country country;
        public TextAsset csvFile;
        public DataInterpretationMode interpretationMode;
    }
}