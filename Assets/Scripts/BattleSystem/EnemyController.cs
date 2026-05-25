using UnityEngine;
using UnityEngine.XR;

public class EnemyController : BattlerController
{
    // Enemy Decisions (Temporary)
    public Rank ChooseRank()
    {
        return HandManager.Hand[UnityEngine.Random.Range(0, HandManager.Hand.Count)].Rank;
    }

    public bool DecideDoPeixinho()
    {
        if (HandManager.HasPeixinho()) return true;
        if (HandManager.HasHalfPeixinho()) return 
    }

    public Rank DoPeixinho()
    {
        
    }
}
