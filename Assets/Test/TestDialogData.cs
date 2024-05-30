public struct TestDialogData : IDialogData
{
    public readonly string Text;

    public TestDialogData(string text)
    {
        Text = text;
    }
}