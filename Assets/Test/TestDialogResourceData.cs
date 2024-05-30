using UnityEngine;

[CreateAssetMenu(
    fileName = "TestDialogResourceData",
    menuName = "ScriptableObjects/SoundEditor/TestDialogResourceData")]
public class TestDialogResourceData : DialogResourceDataBase
{
    [SerializeField] private TestDialog _testDialog;
    
    public override void Bind()
    {
        DialogManager.I.AddPoolController<TestDialog, TestDialogData>(_testDialog);
    }
}