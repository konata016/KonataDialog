using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx.Toolkit;
using UnityEngine;

public class DialogPool<TData> : ObjectPool<DialogBase<TData>> where TData : IDialogData
{
    public int SortedOrder { get; private set; }
    private readonly DialogBase<TData> _prefab;
    private readonly Transform _parent;
    private readonly CancellationTokenSource _token;

    public DialogPool(DialogBase<TData> prefab, Transform parent)
    {
        _prefab = prefab;
        _parent = parent;
        _token = new CancellationTokenSource();
    }

    public void OnBeforeRent(DialogBase<TData> dialog, int sortedOrder, TData data)
    {
        SortedOrder = dialog.OnDialogEnter(sortedOrder, data);
    }

    protected override DialogBase<TData> CreateInstance()
    {
        var obj = Object.Instantiate(_prefab, _parent);
        obj.Initialize();
        return obj;
    }

    protected override void OnBeforeRent(DialogBase<TData> dialog)
    {
        // memo: 既存の表示処理を消すために呼んでいる
    }

    protected override void OnBeforeReturn(DialogBase<TData> dialog)
    {
        dialog.OnDialogExit();
    }

    protected override void OnClear(DialogBase<TData> dialog)
    {
        _token?.Cancel();
        dialog.Discard();
        base.OnClear(dialog);
    }
}