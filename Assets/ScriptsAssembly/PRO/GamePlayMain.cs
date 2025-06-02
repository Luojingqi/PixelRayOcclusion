using PRO;
using PRO.TurnBased;
using PRO;
using System.Collections.Generic;
using UnityEngine;

public class PROMain : MonoScriptBase, ITime_Start, ITime_Update
{
    public static PROMain Inst { get; private set; }

    public List<Role> roles = new List<Role>();

    public void TimeStart()
    {
        Inst = this;
        round = new RoundFSM();
        foreach (var role in roles)
            round.AddRole(role);
    }
    public RoundFSM round;

    bool p = false;

    public void TimeUpdate()
    {
        round.Update();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (p) Time.timeScale = 1;
            else Time.timeScale = 0;
            p = !p;
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
           // MCTS.Main();
        }
    }
    private Dictionary<Transform, Role> RoleDic = new Dictionary<Transform, Role>();
    public void AddRole(Role role)
    {
        RoleDic.Add(role.transform, role);
    }
    public void RemoveRole(Transform role)
    {
        RoleDic.Remove(role);
    }
    public Role GetRole(Transform role)
    {
        if (RoleDic.TryGetValue(role, out Role ret)) return ret;
        else return null;
    }
    public bool GetRole(Transform role, out Role ret)
    {
        ret = null;
        if (role == null) return false;
        if (RoleDic.TryGetValue(role, out ret)) return true;
        else return false;
    }
}