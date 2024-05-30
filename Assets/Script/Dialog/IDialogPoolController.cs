public interface IDialogPoolController
{
    public int SortedOrder { get; }
    public int LatestSortedOrder { get; }
    public bool IsActiveDialog { get; }
    public bool IsActiveLatestDialog { get; }
    public void RefreshAll();
    public void ReturnAll();
    public void Discard();
}