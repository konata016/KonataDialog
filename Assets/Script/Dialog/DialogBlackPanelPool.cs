using System;
using UniRx.Toolkit;
using UnityEngine;

public class DialogBlackPanelPool : ObjectPool<DialogBlackPanel>
{
    private DialogBlackPanel prefab;
    private Transform parent;

    public DialogBlackPanelPool(DialogBlackPanel prefab, Transform parent)
    {
        this.prefab = prefab;
        this.parent = parent;
    }

    public void OnBeforeRent(
        DialogBlackPanel blackPanelButton,
        int siblingIndex,
        Action onClickBlackPanelButton)
    {
        blackPanelButton.Setup(siblingIndex, onClickBlackPanelButton);
    }

    protected override DialogBlackPanel CreateInstance()
    {
        var blackPanelButton = MonoBehaviour.Instantiate(prefab, parent);
        blackPanelButton.Initialize();
        return blackPanelButton;
    }

    protected override void OnBeforeRent(DialogBlackPanel blackPanelButton)
    {
        blackPanelButton.Show();
    }

    protected override void OnBeforeReturn(DialogBlackPanel blackPanelButton)
    {
        blackPanelButton.Hide();
    }   
}