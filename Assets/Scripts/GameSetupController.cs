using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GameSetupController : MonoBehaviour
{
    [SerializeField] GameGrid gameGrid;
    private GameObject characterToPlace;
    [SerializeField] List<GameObject> friendPrefabs;
    [SerializeField] List<GameObject> friendButtons;

    private List<FriendController> friendObjects = new List<FriendController>();
    private List<EnemyController> enemyObjects = new List<EnemyController>();
    [SerializeField] Canvas gameUI;
    [SerializeField] GameplayController gameplayController;

    [SerializeField] GameObject gameStartUIContainer;

    private void Awake() {
        instantiateFriends();
        setUpButtons();    
    }

    private void Start() {
        setGridForCharacterPlacement();
        gameplayController.setActive(false);
    }

    private void instantiateFriends() {
        foreach(GameObject friendPrefab in friendPrefabs) {
            var friendObject = Instantiate(friendPrefab, new Vector2(-20, -20), Quaternion.identity);
            var friendController = friendObject.GetComponent<FriendController>();
            friendObjects.Add(friendController);
        }
    }

    //This is truly bad code
    public void setEnemies(List<EnemyController> enemies) {
        this.enemyObjects = enemies;
    }

    private void setUpButtons() {
        for (int i = 0; i < friendButtons.Count; i++) {
            var friendObject = friendObjects[i];
            var friendButton = friendButtons[i];
            friendButton.GetComponent<PlayerButton>().setFriend(friendObject);
        }
    }

    private void setGridForCharacterPlacement() {
        var positionsforInitialPlacement = new List<Tuple<int, int>>();

        //This is a very hard-codey way of doing this
        for (int i = 0; i < 3; i++) {
            for (int j = 0; j < 6; j++) {
                positionsforInitialPlacement.Add(Tuple.Create<int, int>(j, i));
            }
        }
        
        gameGrid.displaySquaresForInitialPlacement(positionsforInitialPlacement);
    }

    public void startGame() {
        var allFriendsPlaced = true;
        foreach (FriendController friend in friendObjects) {
            if (!friend.hasBeenPlaced) {
                allFriendsPlaced = false;
            }
        }

        if (allFriendsPlaced) {
            //TODO - set up UI
            gameGrid.setAllSquaresNeutral();
            setUpGamePlayController();
            Destroy(gameObject);
        }
    }

    private void setUpGamePlayController() {
        Debug.Log("GameSetupController; Getting the gameplay controller on the scene!");
        gameplayController.setActive(true);
        gameplayController.setGameObjects(gameGrid, friendObjects, enemyObjects);
        gameStartUIContainer.SetActive(false);
        Destroy(gameObject);
    }
}
