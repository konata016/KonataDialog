using System.Collections.Generic;
using UnityEngine;

public class DialogTest : MonoBehaviour
{
    private readonly List<DialogBase<TestDialogData>> _list = new();

    private void Start()
    {
        DialogManager.I.Initialize();
    }

    private void Update()
    {
        // ダイアログを借りる
        if (Input.GetKeyDown(KeyCode.O))
        {
            var data = new TestDialogData("Hello, World!");
            var dialog = DialogManager.I.RentDialog(data);
            _list.Add(dialog);
        }
        
        // ダイアログを返す
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (_list.Count > 0)
            {
                DialogManager.I.ReturnDialog(_list[^1]);
                _list.RemoveAt(_list.Count - 1);
            }
        }
    }
}
