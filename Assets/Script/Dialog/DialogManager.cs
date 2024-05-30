using System;
using System.Collections.Generic;
using R3;
using R3.Triggers;
using UnityEngine;

public class DialogManager : SingletonMonoBehaviour<DialogManager>
{
    [SerializeField] private DialogBlackPanel _blackPanelPrefab;
    [SerializeField] private DialogResourceDataBase _dialogResourceData;

    private Dictionary<string, IDialogPoolController> _dialogPoolControllerMap;
    private DialogBlackPanelPoolController _blackPanelPoolController;
    private Action _onDiscard;
    private int _sortedOrder;
    
    protected override bool _isDontDestroy => false;

    public void Initialize()
    {
        _dialogPoolControllerMap = new Dictionary<string, IDialogPoolController>();
        _blackPanelPoolController = new DialogBlackPanelPoolController(_blackPanelPrefab, transform);
        _onDiscard = null;
        
        _dialogResourceData.Bind();
        
        this.OnDestroyAsObservable().Subscribe(_ => Discard()).AddTo(this);
    }
    
    /// <summary>
    /// 暗幕ボタンの貸し出し
    /// </summary>
    public DialogBlackPanel RentBlackPanel(Action onClickBlackPanelButton, int siblingIndex)
    {
        return _blackPanelPoolController.Rent(onClickBlackPanelButton, siblingIndex);
    }
    
    /// <summary>
    /// 暗幕ボタンの返却
    /// </summary>
    public void ReturnBlackPanel(DialogBlackPanel button)
    {
        _blackPanelPoolController.Return(button);
    }
    
    /// <summary>
    /// ダイアログの貸し出し
    /// </summary>
    public TDialog RentDialog<TDialog, TDialogData>(TDialogData data)
        where TDialog : DialogBase<TDialogData>
        where TDialogData : IDialogData
    {
        if(RentDialog(data) is TDialog dialog)
        {
            return dialog;
        }

        Debug.LogError("ダイアログの貸し出しに失敗");
        return null;
    }
    
    /// <summary>
    /// ダイアログの貸し出し
    /// </summary>
    public DialogBase<TDialogData> RentDialog<TDialogData>(TDialogData data)
        where TDialogData : IDialogData
    {
        var factoryController = GetPoolController<TDialogData>();
        var popup = factoryController.Rent(data, _sortedOrder);
        _sortedOrder = popup.SortedOrder;
        return popup;
    }
    
    /// <summary>
    /// ダイアログの返却
    /// </summary>
    public void ReturnDialog<TDialogData>(DialogBase<TDialogData> dialog)
        where TDialogData : IDialogData
    {
        var factoryController = GetPoolController<TDialogData>();
        factoryController.Return(dialog);
    }
    
    /// <summary>
    /// すべてのダイアログをリフレッシュする
    /// </summary>
    public void RefreshAll()
    {
        foreach (var factoryController in _dialogPoolControllerMap.Values)
        {
            factoryController.RefreshAll();
        }
    }
    
    /// <summary>
    /// ダイアログの追加
    /// </summary>
    public void AddPoolController<TDialog, TDialogData>(TDialog dialog)
        where TDialog : DialogBase<TDialogData>
        where TDialogData : IDialogData
    {
        var factoryController = new DialogPoolController<TDialogData>(dialog, transform);
        _onDiscard += factoryController.Discard;
        _dialogPoolControllerMap.Add(typeof(TDialogData).Name, factoryController);
    }

    private DialogPoolController<TDialogData> GetPoolController<TDialogData>()
        where TDialogData : IDialogData
    {
        return _dialogPoolControllerMap[typeof(TDialogData).Name] as DialogPoolController<TDialogData>;
    }
    
    private void Discard()
    {
        _onDiscard?.Invoke();
    }
}