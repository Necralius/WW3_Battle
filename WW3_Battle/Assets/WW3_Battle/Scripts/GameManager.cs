using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region - Singleton Pattern -
    public static GameManager Instance { get ; private set; }

    private void Awake()
    {
        if (Instance != null) 
            Destroy(Instance.gameObject);

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    [SerializeField] private MenuSystem     _menuSystem     = null;
    [SerializeField] private LayoutManager  _layoutManager  = null;

    public MenuSystem MenuSystem
    {
        get
        {
            if (_menuSystem == null)
                return _menuSystem = FindFirstObjectByType<MenuSystem>();
            else return _menuSystem;
        }
    }

    public LayoutManager LayoutManager
    {
        get
        {
            if (_layoutManager == null)
                return _layoutManager = FindFirstObjectByType<LayoutManager>();
            else return _layoutManager;
        }
    }

    public void ClearData()
    {



    }
}