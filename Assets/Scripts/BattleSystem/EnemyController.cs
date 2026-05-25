using UnityEngine;

public class EnemyController : BattlerController
{
    // Enemy Decisions (Temporary)
    public Rank ChooseRank()
    {
        return HandManager.Hand[UnityEngine.Random.Range(0, HandManager.Hand.Count)].Rank;
    }

    public bool DecideDoPeixinho()
    {
        if (HandManager.HasPeixinho(out var peixinhos))
        {
            return true;
        }
        if (HandManager.HasHalfPeixinho())
        {
            return Random.Range(0f, 1.0f) < 0.5f;
        }
        else return false;
    }

    public Rank DoPeixinho()
    {
        return Rank.None;
    }
}
