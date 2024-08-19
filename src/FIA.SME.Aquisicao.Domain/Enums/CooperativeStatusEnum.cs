namespace FIA.SME.Aquisicao.Core.Enums
{
    public enum CooperativePFTypeEnum
    {
        Undefined = 0,
        IndigenousCommunity = 1,
        PNRASettlement = 2,
        QuilombolaCommunity = 3,
        DAPsFisicas = 4,
        Other = 5
    }

    public enum CooperativePJTypeEnum
    {
        Undefined = 0,
        Association = 1,
        CentralCooperative = 2,
        SingularCooperative = 3,
        Other = 4
    }

    public enum CooperativeProductionTypeEnum
    {
        Undefined = 0,
        Conventional = 1,
        Organic = 2,
        Both = 3
    }

    public enum CooperativeStatusEnum
    {
        Undefined = 0,
        AwaitingEmailConfirmation = 1,
        AwaitingToCompleteRegistration = 2,
        Registered = 3
    }
}
