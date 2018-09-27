namespace ReqspecModels
{
    public enum UserstorySyncActionTypeEnum
    {
        ADD =1,
        DELETE,
        UPDATE,
        MOVE
    }

    public class UserstorySyncActionType
    {
        public int Id { get; set; }
        public string Code { get; set; }
    }
}
