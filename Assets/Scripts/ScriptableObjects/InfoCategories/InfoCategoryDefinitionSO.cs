using System.Collections.Generic;
using UnityEngine;

public enum InfoCategory
{
    IMPORT_TO_AUSTRIA,
    EXPORT_FROM_AUSTRIA,
    MIGRATION_2015_2020
}

namespace ScriptableObjects.Countries
{
    [CreateAssetMenu(fileName = "InfoCategoryDefinition", menuName = "ScriptableObjects/InfoCategoryDefinition", order = 2)]
    public class InfoCategoryDefinitionSO : ScriptableObject
    {
        public string categoryName;
        public InfoCategory category;
        public Material splineMaterial;
        public List<DataFileConfigSO> fileConfigs;
        public List<InfoCategory> connectionThicknessRelativeTo;
    }
}