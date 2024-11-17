namespace DestinyTrail.Engine
{
    public enum ShoppingState
    {
        Init,     
        AskSelection,
        AwaitSelection,
        AskQuantity,
        AwaitQuantity,
        ConfirmPurchase,
        AwaitConfirm,
        Complete,
        Leave,
    }
}