using UnityEngine;
using UnityEngine.UI;

public class TestDialog : DialogBase<TestDialogData>
{
    [SerializeField] private Button _closeButton;
    [SerializeField] private Text _text;
    
    protected override bool IsShowBlackPanelOnStart => true;
    protected override void OnInitialize()
    {
        _closeButton.OnClickButton(Close);
    }

    protected override void OnEnter(TestDialogData data)
    {
        _text.text = data.Text;
    }

    protected override void OnExit()
    {
    }

    protected override void OnDiscarding()
    {
    }
}