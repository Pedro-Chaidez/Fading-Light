using UnityEngine;

public class Ghost : MonoBehaviour
{
    [SerializeField]
   // private bool isStunned = false;
   // private int damageAmount = 0;
    public void Damage()
    {

    }
    public virtual void Die()
    {

    }
    public void Stun()
    {

    }
    public virtual void movementPathing()
    {

    }
}
public class FastGhost : Ghost
{

}
public class NoCollisionGhost : Ghost
{

}
public class ResistantGhost : Ghost
{

}
public class MimicGhost : Ghost
{

}
public class InvisibleGhost : Ghost
{

}
