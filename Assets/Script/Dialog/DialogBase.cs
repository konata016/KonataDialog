using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public abstract class DialogBase<T> : MonoBehaviour where T : IDialogData
{
    [SerializeField] private CanvasGroup _canvasGroup;

    private CancellationTokenSource _token;
    private DialogBlackPanel _blackPanel;
    private Action _onEntry;

    public int SortedOrder { get; private set; }
    public bool IsAnimationPlaying { get; private set; }
    public RectTransform Transform { get; private set; }

    /// <summary>開始時にBlackPanelを表示するか</summary>
    protected abstract bool IsShowBlackPanelOnStart { get; }

    /// <summary>初期化時処理</summary>
    protected abstract void OnInitialize();

    /// <summary>入室時処理</summary>
    protected abstract void OnEnter(T data);

    /// <summary>退出時処理</summary>
    protected abstract void OnExit();

    /// <summary>破棄時処理</summary>
    protected abstract void OnDiscarding();

    /// <summary>表示Animation（書き換え用）</summary>
    protected virtual UniTask ShowAnimation(CancellationToken token) => Transform
        .DOScale(1, 0.2f)
        .OnStart(() => Transform.localScale = Vector3.zero)
        .OnComplete(() => Transform.localScale = Vector3.one)
        .SetEase(Ease.OutBack)
        .SetUpdate(true)
        .ToUniTask(cancellationToken: token);

    /// <summary>非表示Animation（書き換え用）</summary>
    protected virtual UniTask HideAnimation(CancellationToken token) => Transform
        .DOScale(0, 0.2f)
        .OnStart(() => Transform.localScale = Vector3.one)
        .OnComplete(() => Transform.localScale = Vector3.zero)
        .SetEase(Ease.InBack)
        .SetUpdate(true)
        .ToUniTask(cancellationToken: token);

#if UNITY_EDITOR
    private void Reset()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }
#endif

    /// <summary>
    /// 設定
    /// </summary>
    public void Initialize()
    {
        Transform = transform as RectTransform;
        Transform.localScale = Vector3.zero;

        IsAnimationPlaying = false;
        OnInitialize();
    }

    /// <summary>
    /// 入室時処理
    /// </summary>
    public int OnDialogEnter(int sortedOrder, T data)
    {
        SortedOrder = sortedOrder;

        if (IsShowBlackPanelOnStart)
        {
            SortedOrder++;
            _blackPanel = DialogManager.I.RentBlackPanel(OnClickBlackPanelButton, SortedOrder);
        }

        SortedOrder++;
        transform.SetSiblingIndex(SortedOrder);
        _onEntry = () => OnEnter(data);
        _onEntry?.Invoke();
        
        _token?.Cancel();
        _token = new CancellationTokenSource();
        PlayShowAnimation(_token.Token).Forget();

        return SortedOrder;
    }

    /// <summary>
    /// 退出時処理
    /// </summary>
    public void OnDialogExit()
    {
        if (IsShowBlackPanelOnStart)
        {
            DialogManager.I.ReturnBlackPanel(_blackPanel);
        }

        OnExit();
        
        _token?.Cancel();
        _token = new CancellationTokenSource();
        PlayHideAnimation(_token.Token).Forget();
    }

    /// <summary>
    /// 表示
    /// </summary>
    public void Hide()
    {
        Transform.gameObject.SetActive(false);
    }

    /// <summary>
    /// 非表示
    /// </summary>
    public void Show()
    {
        Transform.gameObject.SetActive(true);
    }

    /// <summary>
    /// 破棄
    /// </summary>
    public void Discard()
    {
        OnDiscarding();
    }

    /// <summary>
    /// リフレッシュ
    /// </summary>
    public void Refresh()
    {
        OnExit();
        _onEntry?.Invoke();
    }

    /// <summary>
    /// 暗幕押下時処理
    /// </summary>
    protected virtual void OnClickBlackPanelButton()
    {
        Close();
    }

    /// <summary>
    /// 入室Animation完了時処理
    /// </summary>
    protected virtual void OnFinishedEnterAnimation()
    {
    }

    /// <summary>
    /// 退出Animation完了時処理
    /// </summary>
    protected virtual void OnFinishedExitAnimation()
    {
    }

    /// <summary>
    /// 閉じる
    /// </summary>
    protected void Close()
    {
        if (IsAnimationPlaying)
        {
            return;
        }

        DialogManager.I.ReturnDialog(this);
    }
    
    /// <summary>
    /// 表示Animationの再生
    /// </summary>
    private async UniTask PlayShowAnimation(CancellationToken token)
    {
        _canvasGroup.blocksRaycasts = false;
        IsAnimationPlaying = true;
        Show();
        await ShowAnimation(token);
        IsAnimationPlaying = false;
        _canvasGroup.blocksRaycasts = true;

        if (Transform == null) return;
        
        OnFinishedEnterAnimation();
    }

    /// <summary>
    /// 非表示Animation再生
    /// </summary>
    private async UniTask PlayHideAnimation(CancellationToken token)
    {
        _canvasGroup.blocksRaycasts = false;
        IsAnimationPlaying = true;
        await HideAnimation(token);
        IsAnimationPlaying = false;
        _canvasGroup.blocksRaycasts = true;

        if (Transform == null) return;

        Hide();
        OnFinishedExitAnimation();
    }
}