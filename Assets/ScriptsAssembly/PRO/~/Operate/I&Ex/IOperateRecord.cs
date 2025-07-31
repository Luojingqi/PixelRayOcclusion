using Google.Protobuf;

namespace PRO.Skill
{
    /// <summary>
    /// 操作消息使用Proto传递，生成的Proto类需要实现此接口
    /// </summary>
    public interface IOperateRecord : IMessage
    {
        public void ClearPutIn();
    }
}