using System.Collections.Generic;
using System.Linq;
using R3;
using R3.Triggers;
using UnityEngine;

public class DialogPoolController<TData> : IDialogPoolController where TData : IDialogData
{
    private readonly DialogPool<TData> _pool;
    private readonly List<DialogBase<TData>> _activeList;

    public int SortedOrder => _pool.SortedOrder;
    public int LatestSortedOrder => GetLatestDialog()?.SortedOrder ?? 0;
    public bool IsActiveDialog => _activeList.Any();
    public bool IsActiveLatestDialog => GetLatestDialog() != null;

    public DialogPoolController(DialogBase<TData> prefab, Transform parent)
    {
        _pool = Construct(prefab, parent);
        _activeList = new List<DialogBase<TData>>();

        parent.OnDestroyAsObservable().Subscribe(_ => _pool.Dispose());
    }

    /// <summary>
    /// アクティブなすべてのダイアログを更新する
    /// </summary>
    public void RefreshAll()
    {
        for (var i = 0; i < _activeList.Count; i++)
        {
            _activeList[i].Refresh();
        }
    }

    /// <summary>
    /// ダイアログを貸す
    /// ダイアログが存在していない場合は生成を行う
    /// </summary>
    public DialogBase<TData> Rent(TData data, int sortedOrder)
    {
        var dialog = _pool.Rent();
        _pool.OnBeforeRent(dialog, sortedOrder, data);
        _activeList.Add(dialog);
        return dialog;
    }

    /// <summary>
    /// ダイアログを返す
    /// 破棄はしない
    /// </summary>
    public void Return(DialogBase<TData> dialog)
    {
        _activeList.Remove(dialog);
        _pool.Return(dialog);
    }

    /// <summary>
    /// 全てのダイアログを返す
    /// </summary>
    public void ReturnAll()
    {
        for (var i = 0; i < _activeList.Count; i++)
        {
            _pool.Return(_activeList[i]);
        }

        _activeList.Clear();
    }

    /// <summary>
    /// 一番新しいダイアログを取得する
    /// </summary>
    public DialogBase<TData> GetLatestDialog()
    {
        if (_activeList.Count == 0)
        {
            Debug.Log("アクティブなダイアログが見つからない");
            return null;
        }

        return _activeList[^1];
    }

    /// <summary>
    /// 破棄
    /// </summary>
    public void Discard()
    {
        ReturnAll();
        _pool.Clear();
        _activeList.Clear();
    }

    private DialogPool<TData> Construct(DialogBase<TData> dialog, Transform parent)
    {
        return (DialogPool<TData>)typeof(DialogPool<TData>)
            .GetConstructor(new[] { typeof(DialogBase<TData>), typeof(Transform) })
            ?.Invoke(new object[] { dialog, parent });
    }
}