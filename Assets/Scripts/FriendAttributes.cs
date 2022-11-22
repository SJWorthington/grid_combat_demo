using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FriendAttributes : MonoBehaviour {

    [SerializeField] protected int baseMovement;
    [SerializeField] protected int baseHp;
    [SerializeField] protected int baseStrength;
    [SerializeField] protected int baseDefence;

    public abstract void attack();
}
