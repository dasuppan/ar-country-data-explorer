using System.Collections.Generic;
using UnityEngine;

public abstract class DataFileConfigSO : ScriptableObject
{
    public TextAsset csvFile;

    public abstract void ProcessFile(
        List<Country> countries,
        InfoCategory targetInfoCategory
    );
}