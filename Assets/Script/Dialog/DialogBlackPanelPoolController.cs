using System;
using R3;
using R3.Triggers;
using UnityEngine;

public class DialogBlackPanelPoolController
{
    private readonly DialogBlackPanelPool _pool;

    public DialogBlackPanelPoolController(DialogBlackPanel prefab, Transform parent)
    {
        _pool = new DialogBlackPanelPool(prefab, parent);
        parent.OnDestroyAsObservable().Subscribe(_ => _pool.Dispose());
    }

    /// <summary>
    /// 暗幕を貸す
    /// 暗幕が存在していない場合は生成を行う
    /// </summary>
    public DialogBlackPanel Rent(Action onClickBlackPanel, int siblingIndex)
    {
        var blackPanelButton = _pool.Rent();
        _pool.OnBeforeRent(blackPanelButton, siblingIndex, onClickBlackPanel);
        return blackPanelButton;
    }

    /// <summary>
    /// 暗幕を返す
    /// 破棄はしない
    /// </summary>
    public void Return(DialogBlackPanel blackPanelButton)
    {
        _pool.Return(blackPanelButton);
    }
}