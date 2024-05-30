using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SequenceUniTask
{
    private readonly List<Func<CancellationToken, UniTask>> _taskList;

    private CancellationTokenSource _source;
    private GameObject _gameObject;
    private bool _isLoop;
    private bool _isCancel;

    private Action _onCancel;
    private Action _onComplete;

    public SequenceUniTask()
    {
        _taskList = new List<Func<CancellationToken, UniTask>>();
        _gameObject = null;
        _isLoop = false;
        _isCancel = false;
    }

    public void AddTask(Func<CancellationToken, UniTask> task)
    {
        _taskList.Add(task);
    }

    public void RegisterOnCancel(Action onCancel)
    {
        _onCancel = onCancel;
    }

    public void RegisterOnComplete(Action onComplete)
    {
        _onComplete = onComplete;
    }

    public void RemoveTaskAll()
    {
        Kill();
        _taskList.Clear();
    }

    public void Play()
    {
        _isCancel = true;
        _source?.Cancel();

        _source = new CancellationTokenSource();
        var token = _gameObject == null
            ? _source
            : CancellationTokenSource.CreateLinkedTokenSource(
                _source.Token,
                _gameObject.GetCancellationTokenOnDestroy());

        _isCancel = false;
        ProcessLoopAsync(token.Token).Forget();
    }

    public void SetLoop()
    {
        _isLoop = true;
    }

    public void Kill()
    {
        _isCancel = true;
        _source?.Cancel();
        _onCancel?.Invoke();
    }

    public void SetLink(GameObject obj)
    {
        _gameObject = obj;
    }

    private async UniTask ProcessLoopAsync(CancellationToken token)
    {
        for (;;)
        {
            await ProcessAsync(token);

            if (!_isLoop || _isCancel || token.IsCancellationRequested)
            {
                _onComplete?.Invoke();
                return;
            }
        }
    }

    private async UniTask ProcessAsync(CancellationToken token)
    {
        for (var i = 0; i < _taskList.Count; i++)
        {
            var task = _taskList[i];
            await UniTask.WhenAny(
                task.Invoke(token),
                UniTask.WaitUntil(() => _isCancel, cancellationToken: token));

            if (_isCancel)
            {
                return;
            }
        }
    }
}