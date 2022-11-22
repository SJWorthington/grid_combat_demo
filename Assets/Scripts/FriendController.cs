using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendController : MonoBehaviour {

    //TODO - learn how to make a field with private set still accessible to Unity with SerializeField
    public int baseMovement;
    public int baseHp;
    public int baseStrength;
    public int baseDefence;
    public int baseRange;
    public int level = 1;
    public Sprite uiSprite;

    [SerializeField] int movementUpgrade;
    [SerializeField] int hpUpgrade;
    [SerializeField] int strengthUpgrade;
    [SerializeField] int defenceUpgrade;
    [SerializeField] int rangeUpgrade;

    

    public int currentHp;


    public bool hasBeenPlaced = false;
    private Vector2 offScreenVector = new Vector2(-20, -20);

    private GameplayController gameplayController;

    private float yVectorOffset = 0.25f;

    public CurrentPlayerState playerState = CurrentPlayerState.READY_TO_MOVE;

    public GridSquare currentGridSpace { get; private set; }

    private void Start() {
        currentHp = baseHp;
    }

    public void setGameplayController(GameplayController controller) {
        this.gameplayController = controller;
    }

    public void levelup() {
        level++;
        baseMovement += movementUpgrade;
        baseHp += hpUpgrade;
        currentHp += hpUpgrade;
        baseDefence += defenceUpgrade;
        baseStrength += strengthUpgrade;
        baseRange += rangeUpgrade;
        gameplayController.notifyOfLevelUp(this);
    }

    public void moveOffScreen() {
        gameObject.transform.position = offScreenVector;
    }

    public void moveFriend(Vector2 newPosition) {
        gameObject.transform.position = newPosition;
    }

    public void resetForEndTurn() {
        playerState = CurrentPlayerState.READY_TO_MOVE;
    }

    public void setStateForHasMoved() {
        playerState = CurrentPlayerState.READY_TO_ATTACK;
    }

    public void setStateForHasAttacked() {
        playerState = CurrentPlayerState.ALL_ACTIONS_TAKEN;
    }

    public void receiveDamage(int enemyStrength) {
        currentHp -= enemyStrength;
        if (baseHp <= 0) {
            gameplayController.notifyOfSadFriendDeath(this);
            Destroy(gameObject);
        }
    }

    // I am well aware that having this and the grid square hold references to each other is bad
    //But it fixes the issue of not being able to update a gridSquare to not hold a reference to a character
    // And this is a throw away demo, so I'm gonna say it's ok
    public void setNewGridSpace(GridSquare gridSquare) {
        if (currentGridSpace != null) {
            currentGridSpace.removeFriend();
        }
        currentGridSpace = gridSquare;
        var characterPos = new Vector2(gridSquare.transform.position.x, gridSquare.transform.position.y + yVectorOffset);
        gameObject.transform.position = characterPos;
    }
}

public enum CurrentPlayerState {
    READY_TO_MOVE, READY_TO_ATTACK, ALL_ACTIONS_TAKEN
}