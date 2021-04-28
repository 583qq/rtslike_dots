using Unity;
using Unity.Entities;


public struct ResourceTransactionData : IComponentData
{
    public ResourceTypes type;
    public bool isProfit;
    public uint value;
}




