using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    //This should probably be in a hierarchy with player but oh well
    //Especially now I'm making enemy control manual

    [SerializeField] public int baseMovement;
    [SerializeField] public int baseHp;
    [SerializeField] public int baseStrength;
    [SerializeField] public int baseDefence;
    [SerializeField] public int baseRange;
    public int currentHp;

    private float yVectorOffset = 0.25f;

    private GameplayController gameplayController;

    public GridSquare currentGridSpace { get; private set; }

    public CurrentEnemyState currentEnemyState = CurrentEnemyState.READY_TO_MOVE;

    private void Start() {
        Debug.Log("ENEMYCONTROLLER.Start(); How many times is this being hit");
        currentHp = baseHp;
    }

    public void moveEnemy(Vector2 newPosition) {
        gameObject.transform.position = newPosition;
    }

    public bool receiveDamage(int enemyStrength) {
        Debug.Log($"hp; {currentHp} -= ({enemyStrength} - {baseDefence})");
        currentHp -= (enemyStrength - baseDefence);
        Debug.Log($"HP is now {currentHp}");
        if (currentHp <= 0) {
            gameplayController.notifyOfGhostDeath(this);
            Destroy(gameObject);
            return true;
        }
        return false;
    }

    public void setGameplayController(GameplayController controller) {
        this.gameplayController = controller;
    }

    public void resetForEndTurn() {
        currentEnemyState = CurrentEnemyState.READY_TO_MOVE;
    }

    public void setStateForHasMoved() {
        currentEnemyState = CurrentEnemyState.READY_TO_ATTACK;
    }

    public void setStateForHasAttacked() {
        currentEnemyState = CurrentEnemyState.ALL_ACTIONS_TAKEN;
    }

    public void setNewGridSpace(GridSquare gridSquare) {
        if (currentGridSpace != null) {
            currentGridSpace.removeEnemy();
        }
        currentGridSpace = gridSquare;
        var characterPos = new Vector2(gridSquare.transform.position.x, gridSquare.transform.position.y + yVectorOffset);
        gameObject.transform.position = characterPos;
    }
}

public enum CurrentEnemyState {
    READY_TO_MOVE, READY_TO_ATTACK, ALL_ACTIONS_TAKEN
}
