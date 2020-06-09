namespace MvcClient.ViewModels
{
    public class CartComponentViewModel
    {
        public int ItemsInCart { get; internal set; }
        public double TotalCost { get; internal set; }
        public string Disabled => (ItemsInCart == 0) ? "is-disabled" : "";
    }
}