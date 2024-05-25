using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*[CustomPropertyDrawer(typeof(StringStringDictionary))]
[CustomPropertyDrawer(typeof(ObjectColorDictionary))]
[CustomPropertyDrawer(typeof(StringColorArrayDictionary))]*/
[CustomPropertyDrawer(typeof(GameObjectFloatDictionary))]
[CustomPropertyDrawer(typeof(IntGameObjectListDictionary))]
[CustomPropertyDrawer(typeof(InfoCategoryTextAssetDictionary))]
public class AnySerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer {}

/*[CustomPropertyDrawer(typeof(ColorArrayStorage))]*/
[CustomPropertyDrawer(typeof(GameObjectStorage))]
public class AnySerializableDictionaryStoragePropertyDrawer: SerializableDictionaryStoragePropertyDrawer {}

