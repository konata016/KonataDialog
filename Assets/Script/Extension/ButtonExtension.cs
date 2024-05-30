using System;
using UnityEngine.UI;

public static class ButtonExtension
{
    /// <summary>
    /// ボタン押下時処理
    /// (Action登録時にRemoveAllListenersが呼ばれた後に登録される)
    /// </summary>
    public static void OnClickButton(this Button button, Action onClick)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick?.Invoke());
    }
}