using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DialogBlackPanel : MonoBehaviour
{
    [SerializeField] private Button _button;

    private static readonly float _fadeTime = 0.15f;
    private static readonly float _maxAlpha = 0.90f;

    private SequenceUniTask _showAnimation;
    private SequenceUniTask _hideAnimation;

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize()
    {
        _showAnimation = new SequenceUniTask();
        _showAnimation.AddTask(ShowAnimation);
        _showAnimation.SetLink(gameObject);

        _hideAnimation = new SequenceUniTask();
        _hideAnimation.AddTask(HideAnimation);
        _hideAnimation.SetLink(gameObject);
    }

    /// <summary>
    /// 設定
    /// </summary>
    public void Setup(int siblingIndex, Action onClickBlackPanelButton)
    {
        _button.OnClickButton(onClickBlackPanelButton);
        transform.SetSiblingIndex(siblingIndex);
    }

    /// <summary>
    /// 表示
    /// </summary>
    public void Show()
    {
        _hideAnimation.Kill();
        _showAnimation.Play();
    }

    /// <summary>
    /// 非表示
    /// </summary>
    public void Hide()
    {
        _showAnimation.Kill();
        _hideAnimation.Play();
    }

    private async UniTask ShowAnimation(CancellationToken token)
    {
        _button.image.SetAlpha(0f);
        gameObject.SetActive(true);
        await _button.image.DOFade(_maxAlpha, _fadeTime)
            .SetUpdate(true)
            .ToUniTask(cancellationToken: token);
    }

    private async UniTask HideAnimation(CancellationToken token)
    {
        _button.image.SetAlpha(_maxAlpha);
        await _button.image.DOFade(0f, _fadeTime)
            .SetUpdate(true)
            .ToUniTask(cancellationToken: token);
        gameObject.SetActive(false);
    }
}