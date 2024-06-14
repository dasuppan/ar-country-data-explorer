﻿using UnityEngine;

namespace ScriptableObjects.Countries
{
    [CreateAssetMenu(fileName = "CountryDefinition", menuName = "ScriptableObjects/CountryDefinition", order = 1)]
    public class CountryDefinitionSO : ScriptableObject
    {
        public string countryName;
        public Sprite flagSprite;
    }
}