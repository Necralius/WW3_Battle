using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class LayoutManager : MonoBehaviour
{
    [SerializeField] private List<Layout> _layouts = new List<Layout>();
    [SerializeField] private bool _debug = false;

    public void OpenPanel(string panelName)
    {
        _layouts.ForEach(e =>
        {
            if (e.Name == panelName)
            {
                if (_debug) 
                    Debug.Log(panelName); 
                e.Activate();
            }
            else e.Deactive();
        });
    }

    public void OpenPanel(Layout panel) => _layouts.ForEach(e => { if (e.Name == panel.Name) e.Activate(); else e.Deactive(); });
}

[Serializable]
public class Layout
{
    public string     Name  = "Panel";
    public GameObject Panel = null;
    public float      Speed = 1f;
    public UnityEvent OnActivate;
    public UnityEvent OnDeactivate;

    private CanvasGroup cg;

    public CanvasGroup CG
    {
        get
        {
            if (cg != null)
                return cg;

            if (Panel.GetComponent<CanvasGroup>()) 
                return cg = Panel.GetComponent<CanvasGroup>();
            else 
                return cg = Panel.AddComponent<CanvasGroup>();
        }
    }

    public void Activate()
    {
        CG.blocksRaycasts   = true;
        CG.interactable     = true;
        CG.DOFade(1f, Speed);

        OnActivate.Invoke();
    }

    public void Deactive()
    {
        CG.blocksRaycasts   = false;
        CG.interactable     = false;
        CG.DOFade(0f, Speed);

        OnDeactivate.Invoke();
    }
}