using PRO.Skill;
using PRO.Tool;

namespace PRO
{
    public class MCTS
    {

        //public class Node
        //{
        //    private static ObjectPool<Node> pool = new ObjectPool<Node>();
        //    public static Node TakeOut() => pool.TakeOut();
        //    public static void PutIn(Node node)
        //    {
        //        if (node.operateRecord != null)
        //        {
        //            node.operateRecord.PutIn();
        //            node.operateRecord = null;
        //        }
        //        pool.PutIn(node);
        //    }

        //    public Node parent;
        //    public PriorityQueue<Node> chiles = new PriorityQueue<Node>();

        //    public IOperateRecord operateRecord;

        //    #region 评估值
        //    private int _win;
        //    private int _lost;

        //    public int win
        //    {
        //        get { return _win; }
        //        set
        //        {
        //            if (parent != null)
        //                parent.win = parent._win - _win + value;
        //            _win = value;
        //        }
        //    }

        //    public int lost
        //    {
        //        get { return _lost; }
        //        set
        //        {
        //            if (parent != null)
        //                parent.lost = parent._lost - _lost + value;
        //            _lost = value;
        //        }
        //    }

        //    public int num
        //    {
        //        get { return _win + _lost; }
        //    }
        //    private static float c = Mathf.Sign(2);
        //    public float ucb
        //    {
        //        get
        //        {
        //            if (_lost == 0 || num == 0) return float.MinValue;
        //            return -((float)_win / _lost + c * Mathf.Sign(Mathf.Log(parent.num) / num));
        //        }
        //    }
        //    #endregion
        //    public void 还原(SceneEntity scene)
        //    {
        //        SceneManager.Inst.NowScene.Unload();

        //        Vector2Int minLightBufferBlockPos = CameraCenterBlockPos - LightResultBufferBlockSize / 2;
        //        Vector2Int minBlockBufferPos = minLightBufferBlockPos - EachBlockReceiveLightSize / 2;
        //        Vector2Int maxBlockBufferPos = minBlockBufferPos + LightResultBufferBlockSize - new Vector2Int(1, 1) + EachBlockReceiveLightSize - new Vector2Int(1, 1);

        //        for (int y = minBlockBufferPos.y; y <= maxBlockBufferPos.y; y++)
        //            for (int x = minBlockBufferPos.x; x <= maxBlockBufferPos.x; x++)
        //                scene.ThreadLoadOrCreateBlock(new Vector2Int(x, y));


        //        for (int i = 0; i < PROMain.Inst.roles.Count; i++)
        //        {
        //            var role = PROMain.Inst.roles[i];
        //            var roleDisk = roleList[i];
        //            // RoleInfo.Clone(role.)
        //        }
        //    }


        //    public List<Role_MCTS> roleList = new List<Role_MCTS>();


        //    public class Role_MCTS
        //    {
        //        public RoleInfo roleInfo = new RoleInfo();
        //        public Vector2Int globalPos;
        //        public Toward toward;
        //    }
        //}

        //static bool init = false;
        //static Node now = new Node();
        //static List<Node> nodes = new List<Node>();
        //static Skill_0_0 TimeDelay = new Skill_0_0();
        //static List<OperateRecord> operateRecordList = new List<OperateRecord>();

        //public static void Main()
        //{

        //    var round = PROMain.Inst.round;
        //    nodes.Add(now);
        //    //当前节点没有子节点
        //    if (now.chiles.Count == 0)
        //    {
        //        var turn = round.State3_Turn.NowTurn;
        //        foreach (var operate in turn.Agent.AllCanUseOperate)
        //        {
        //            if (operate.NowState.EnumName == OperateStateEnum.t0 && operate.T0.CheckUp() && operate.T1 is I mcts)
        //            {
        //                mcts.扩展节点(ref operateRecordList);
        //                foreach (var record in operateRecordList)
        //                {
        //                    record.operate = operate;
        //                    var node = new Node();
        //                    node.operateRecord = record;
        //                    now.chiles.Enqueue(node, node.ucb);
        //                }
        //                operateRecordList.Clear();
        //            }
        //        }
        //    }
        //    TimeDelay.T1.扩展节点(ref operateRecordList);
        //    foreach (var record in operateRecordList)
        //    {
        //        record.operate = TimeDelay;
        //        var node = new Node();
        //        node.operateRecord = record;
        //        now.chiles.Enqueue(node, node.ucb);
        //    }
        //    operateRecordList.Clear();
        //    if (now.chiles.Count != 0)
        //    {
        //        //随机取出了一个子节点
        //        Node next = now.chiles.Dequeue();
        //        var operate = next.operateRecord.operate;
        //        operate.T0.TrySwitchStateToT1();
        //        if (operate != TimeDelay)
        //        {
        //            operate.Turn.Agent.ForEachBuffApplyEffect(BuffTriggerType.技能释放前, operate.context, -1);
        //        }
        //        operate.T1.节点执行(next.operateRecord);
        //        operate.SwitchState(OperateStateEnum.t2);
        //        operate.T2.Enter(next.operateRecord);
        //        now = next;
        //    }
        //}
    }
}
