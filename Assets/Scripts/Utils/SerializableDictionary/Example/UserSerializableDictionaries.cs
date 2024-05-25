using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/*[Serializable]
public class StringStringDictionary : SerializableDictionary<string, string> {}

[Serializable]
public class ObjectColorDictionary : SerializableDictionary<UnityEngine.Object, Color> {}

[Serializable]
public class ColorArrayStorage : SerializableDictionary.Storage<Color[]> {}

[Serializable]
public class StringColorArrayDictionary : SerializableDictionary<string, Color[], ColorArrayStorage> {}*/

/*[Serializable]
public class MyClass
{
    public int i;
    public string str;
}
[Serializable]
public class QuaternionMyClassDictionary : SerializableDictionary<Quaternion, MyClass> {}*/


[Serializable]
public class GameObjectStorage : SerializableDictionary.Storage<List<GameObject>> {}

[Serializable]
public class IntGameObjectListDictionary : SerializableDictionary<int, List<GameObject>, GameObjectStorage> {}

[Serializable]
public class GameObjectFloatDictionary : SerializableDictionary<GameObject, float> {}

[Serializable]
public class InfoCategoryTextAssetDictionary : SerializableDictionary<InfoCategory, TextAsset> {}
