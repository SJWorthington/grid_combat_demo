using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSquare : MonoBehaviour {
    [SerializeField] float characterYOffset = 0.25f;
    [SerializeField] private GameObject statusIndicatorPrefab;
    private SpriteRenderer statusSprite;

    //Region status colours
    private Color canPlaceUnitColor = new Color(0f, 200f, 0f, 0.5f);
    private Color canBeAttackedColor = new Color(200f, 0f, 0f, 0.5f);
    private Color canBeMovedToColor = new Color(0f, 0f, 200f, 0.5f);
    private Color neutralColor = new Color(0f, 0f, 0f, 0f);

    public bool canBePlacedOn { get; private set; }
    public bool canBeMovedTo { get; private set; }
    public bool canBeAttacked { get; private set; }

    //Could probably make these into a wee model class
    public int rowInGrid;
    public int columnInGrid;

    public bool isOccupiedByFriend {
        get { return friendInSquare != null; }
    }

    public bool isOccupiedByEnemy {
        get { return enemyInSquare != null; }
    }

    public FriendController friendInSquare { get; private set ; }
    public EnemyController enemyInSquare { get; private set ; }

    private void Awake() {
        statusSprite = Instantiate(statusIndicatorPrefab, gameObject.transform.position, Quaternion.identity).GetComponent<SpriteRenderer>();
        setNeutral();
    }

    public void setCanBePlacedOn() {
        canBePlacedOn = true;
        statusSprite.color = canPlaceUnitColor;
    }

    public void setCanBeAttacked() {
        canBeAttacked = true;
        statusSprite.color = canBeAttackedColor;
    }

    public void setIsInAttackRange() {
        statusSprite.color = canBeAttackedColor;
    }

    public void setCanBeMovedTo() {
        canBeMovedTo = true;
        statusSprite.color = canBeMovedToColor;
    }

    public void setNeutral() {
        canBeMovedTo = false;
        canBeAttacked = false;
        canBePlacedOn = false;
        statusSprite.color = neutralColor;
    }

    public bool setFriend(FriendController newCharacter) {
       
        if ((canBeMovedTo || canBePlacedOn ) && !isOccupiedByFriend) { // this is messy
            friendInSquare = newCharacter;
            friendInSquare.setNewGridSpace(this);
            setNeutral();
            return true;
        } else {
            return false;
        }
    }

    public void removeFriend() {
        this.friendInSquare = null;
    }

    public void removeEnemy() {
        this.enemyInSquare = null;
    }

    public void setEnemy(EnemyController enemy) {
        if (canBeMovedTo || canBePlacedOn) {
            enemyInSquare = enemy;
            enemyInSquare.setNewGridSpace(this);
            setNeutral();
        }
    }
}
