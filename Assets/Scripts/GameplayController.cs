using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayController : MonoBehaviour {
    private GameGrid gameGrid;
    private List<FriendController> friendObjects;
    private List<EnemyController> enemyObjects;

    private FriendController _selectedFriend;
    private FriendController selectedFriend {
        get => _selectedFriend;
        set {
            this._selectedFriend = value;
            if (_selectedFriend != null) {
                gamePlayUI.setHp(_selectedFriend.currentHp, _selectedFriend.baseHp);
                gamePlayUI.setDefence(_selectedFriend.baseDefence);
                gamePlayUI.setStrength(_selectedFriend.baseStrength);
                gamePlayUI.setMovement(_selectedFriend.baseMovement - 1);
                gamePlayUI.setRange(_selectedFriend.baseRange);
                gamePlayUI.setLevel(_selectedFriend.level);
                gamePlayUI.setImage(_selectedFriend.uiSprite);
            }
        }
    }

    private EnemyController _selectedEnemy;
    private EnemyController selectedEnemy {
        get { return this._selectedEnemy; }
        set {
            this._selectedEnemy = value;
            if (_selectedEnemy != null) {
                gamePlayUI.setHp(_selectedEnemy.currentHp, _selectedEnemy.baseHp);
                gamePlayUI.setDefence(_selectedEnemy.baseDefence);
                gamePlayUI.setStrength(_selectedEnemy.baseStrength);
                gamePlayUI.setMovement(_selectedEnemy.baseMovement - 1);
                gamePlayUI.setRange(_selectedEnemy.baseRange);
                gamePlayUI.setLevel(1);
                gamePlayUI.setImage(_selectedEnemy.gameObject.GetComponent<SpriteRenderer>().sprite);
            }
        }
    }

    [SerializeField] GameObject gamePlayUIContainer;
    [SerializeField] GamePlayUIController gamePlayUI;

    private GamePlayState gamePlayState = GamePlayState.PLAYER_TURN;

    public void setActive(bool isActive) {
        gameObject.SetActive(isActive);
    }

    private void Awake() {
        gamePlayUIContainer.SetActive(true);
        gamePlayUI.setGameStatusText("Friend Turn!");
    }

    public void notifyOfLevelUp(FriendController levelledUpFriend) {
        //This is truly lazy, Stephen
        selectedFriend = levelledUpFriend;
        gamePlayUI.setGameStatusText("Friend has levelled up!");
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            resolveMouseClickLeft();
        }
    }

    private void resolveMouseClickLeft() {
        var gridSquare = gameGrid.getGridSquareAtPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (gridSquare == null) return;

        if (!gridSquare.isOccupiedByFriend && !gridSquare.isOccupiedByEnemy && !gridSquare.canBeMovedTo) {
            selectedFriend = null;
            gameGrid.setAllSquaresNeutral();
            return;
        }

        if (gamePlayState == GamePlayState.PLAYER_TURN) {
            resolvePlayerTurn(gridSquare);
        } else {
            resolveEnemyTurn(gridSquare);
        }
    }

    private void resolveEnemyTurn(GridSquare gridSquare) {
        if (gridSquare.isOccupiedByEnemy) {
            if (gridSquare.enemyInSquare == selectedEnemy) {
                switch (selectedEnemy.currentEnemyState) {
                    case CurrentEnemyState.READY_TO_MOVE:
                        selectedEnemy.setStateForHasMoved();
                        gameGrid.setAllSquaresNeutral();
                        gameGrid.displayAttackFromSquare(gridSquare, selectedEnemy.baseRange, gridSquare.rowInGrid, gridSquare.columnInGrid);
                        break;
                    case CurrentEnemyState.READY_TO_ATTACK:
                        gameGrid.setAllSquaresNeutral();
                        gameGrid.displayAttackFromSquare(gridSquare, selectedEnemy.baseRange, gridSquare.rowInGrid, gridSquare.columnInGrid);
                        break;
                    case CurrentEnemyState.ALL_ACTIONS_TAKEN:
                        gameGrid.setAllSquaresNeutral();
                        break;
                }
            } else {
                gameGrid.setAllSquaresNeutral();
                selectedEnemy = gridSquare.enemyInSquare;
                switch (selectedEnemy.currentEnemyState) {
                    case CurrentEnemyState.READY_TO_MOVE:
                        gameGrid.setAllSquaresNeutral();
                        gameGrid.displayMoveAndAttackFromSquare(gridSquare, selectedEnemy.baseMovement, selectedEnemy.baseRange);
                        break;
                    case CurrentEnemyState.READY_TO_ATTACK:
                        gameGrid.setAllSquaresNeutral();
                        gameGrid.displayAttackFromSquare(gridSquare, selectedEnemy.baseRange, gridSquare.rowInGrid, gridSquare.columnInGrid);
                        break;
                    case CurrentEnemyState.ALL_ACTIONS_TAKEN:
                        gameGrid.setAllSquaresNeutral();
                        break;
                }
            }
        } else if (gridSquare.isOccupiedByFriend) {
            if (selectedEnemy != null && selectedEnemy.currentEnemyState == CurrentEnemyState.READY_TO_ATTACK && gridSquare.canBeAttacked) {
                gridSquare.friendInSquare.receiveDamage(selectedEnemy.baseStrength);
                selectedEnemy.setStateForHasAttacked();
                selectedEnemy = null;
                gameGrid.setAllSquaresNeutral();
            } else {
                if (selectedEnemy != null) {
                    selectedEnemy = null;
                    gameGrid.setAllSquaresNeutral();
                }
            }
        } else {
            if (selectedEnemy != null && gridSquare.canBeMovedTo) { // I'm relying on a mix of gridsquare and enemy state, which isn't great
                gridSquare.setEnemy(selectedEnemy);
                selectedEnemy.setStateForHasMoved();
                gameGrid.setAllSquaresNeutral();
                gameGrid.displayAttackFromSquare(gridSquare, selectedEnemy.baseRange, gridSquare.rowInGrid, gridSquare.columnInGrid);
            } else {
                selectedEnemy = null;
                gameGrid.setAllSquaresNeutral();
            }
        }
    }

    public void notifyOfGhostDeath(EnemyController spookyGhost) {
        enemyObjects.Remove(spookyGhost);
        if (enemyObjects.Count == 0) {
            gamePlayUI.setGameStatusText("Friends win!");

        }
    }

    public void notifyOfSadFriendDeath(FriendController friend) {
        friendObjects.Remove(friend);
        if (friendObjects.Count == 0) {
            gamePlayUI.setGameStatusText("Friends lose :(");

        }
    }

    private void resolvePlayerTurn(GridSquare gridSquare) {
        if (gridSquare.isOccupiedByFriend) {
            if (gridSquare.friendInSquare == selectedFriend) {
                switch (selectedFriend.playerState) {
                    case CurrentPlayerState.READY_TO_MOVE:
                        selectedFriend.setStateForHasMoved();
                        gameGrid.setAllSquaresNeutral();
                        gameGrid.displayAttackFromSquare(gridSquare, selectedFriend.baseRange, gridSquare.rowInGrid, gridSquare.columnInGrid);
                        break;
                    case CurrentPlayerState.READY_TO_ATTACK:
                        gameGrid.setAllSquaresNeutral();
                        gameGrid.displayAttackFromSquare(gridSquare, selectedFriend.baseRange, gridSquare.rowInGrid, gridSquare.columnInGrid);
                        break;
                    case CurrentPlayerState.ALL_ACTIONS_TAKEN:
                        gameGrid.setAllSquaresNeutral();
                        break;
                }
            } else {
                gameGrid.setAllSquaresNeutral();
                selectedFriend = gridSquare.friendInSquare;
                switch (selectedFriend.playerState) {
                    case CurrentPlayerState.READY_TO_MOVE:
                        gameGrid.setAllSquaresNeutral();
                        gameGrid.displayMoveAndAttackFromSquare(gridSquare, selectedFriend.baseMovement, selectedFriend.baseRange);
                        break;
                    case CurrentPlayerState.READY_TO_ATTACK:
                        gameGrid.setAllSquaresNeutral();
                        gameGrid.displayAttackFromSquare(gridSquare, selectedFriend.baseRange, gridSquare.rowInGrid, gridSquare.columnInGrid);
                        break;
                    case CurrentPlayerState.ALL_ACTIONS_TAKEN:
                        gameGrid.setAllSquaresNeutral();
                        break;
                }
            }
        } else if (gridSquare.isOccupiedByEnemy) {
            if (selectedFriend != null && selectedFriend.playerState == CurrentPlayerState.READY_TO_ATTACK && gridSquare.canBeAttacked) {
                var spookyEnemyIsDead = gridSquare.enemyInSquare.receiveDamage(selectedFriend.baseStrength);
                if (spookyEnemyIsDead) {
                    selectedFriend.levelup();
                }
                selectedFriend.setStateForHasAttacked();
                selectedFriend = null;
                gameGrid.setAllSquaresNeutral();
            } else {
                if (selectedFriend != null) {
                    selectedFriend = null;
                    gameGrid.setAllSquaresNeutral();
                }
            }
        } else {
            if (selectedFriend != null && gridSquare.canBeMovedTo) { // I'm relying on a mix of gridsquare and player state, which isn't great
                gridSquare.setFriend(selectedFriend);
                selectedFriend.setStateForHasMoved();
                gameGrid.setAllSquaresNeutral();
                gameGrid.displayAttackFromSquare(gridSquare, selectedFriend.baseRange, gridSquare.rowInGrid, gridSquare.columnInGrid);
            } else {
                selectedFriend = null;
                gameGrid.setAllSquaresNeutral();
            }
        }
    }


    private void readyFriendForMovement(GridSquare gridSquare) {
        gameGrid.displayMoveAndAttackFromSquare(gridSquare, selectedFriend.baseMovement, selectedFriend.baseRange);
    }

    public void setGameObjects(GameGrid gameGrid, List<FriendController> newFriendObjects, List<EnemyController> enemiesToAdd) {
        this.gameGrid = gameGrid;
        friendObjects = newFriendObjects;
        foreach (FriendController friend in friendObjects) {
            friend.setGameplayController(this);
        }
        this.enemyObjects = enemiesToAdd;
        foreach (EnemyController enemy in enemyObjects) {
            enemy.setGameplayController(this);
        }
    }

    public void endPlayerTurn() {
        if (gamePlayState == GamePlayState.PLAYER_TURN) {
            foreach (FriendController friend in friendObjects) {
                friend.resetForEndTurn();
            }
            gameGrid.setAllSquaresNeutral();
            gamePlayState = GamePlayState.ENEMY_TURN;
            gamePlayUI.setGameStatusText("Spooky Ghost Turn!");

        } else {
            foreach (EnemyController enemy in enemyObjects) {
                enemy.resetForEndTurn();

            }
            gameGrid.setAllSquaresNeutral();
            gamePlayState = GamePlayState.PLAYER_TURN;
            gamePlayUI.setGameStatusText("Alien Friend Turn!");
        }
    }
}

enum GamePlayState {
    PLAYER_TURN, ENEMY_TURN
}
