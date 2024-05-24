using UnityEngine;

public abstract class UnitySingleton<T> : MonoBehaviour where T : UnitySingleton<T>
{
    private static T _instance;
    public static T Instance {  get { return _instance; } 
        private set {} }

    private bool _isPersistent = false;

    public void SetPersistent(bool state)
    {
        _isPersistent = state;
        MakePersistent();
    }

    private void MakePersistent()
    {
        transform.parent = null;
        DontDestroyOnLoad(this.gameObject);
    }
    
    protected virtual void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            _instance = this as T;
        }
        
        if (_isPersistent)
        {
            MakePersistent();
        }
    }
}
