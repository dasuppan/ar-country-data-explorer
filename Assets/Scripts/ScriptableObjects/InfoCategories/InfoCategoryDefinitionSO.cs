using UnityEngine;

public enum InfoCategory
{
    IMPORT,
    EXPORT
}

public enum CategoryType
{
    TO_PIVOT,
    FROM_PIVOT
}

namespace ScriptableObjects.Countries
{
    [CreateAssetMenu(fileName = "InfoCategoryDefinition", menuName = "ScriptableObjects/InfoCategoryDefinition", order = 2)]
    public class InfoCategoryDefinitionSO : ScriptableObject
    {
        public string categoryName;
        public InfoCategory category;
        public CategoryType type;
        public Material splineMaterial;
        public TextAsset csvFile;
    }
}