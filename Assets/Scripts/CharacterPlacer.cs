using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPlacer : MonoBehaviour {

    private FriendController currentFriendObject;
    [SerializeField] GameGrid gameGrid;

    // Update is called once per frame
    void Update() {
        if (currentFriendObject != null) {
            currentFriendObject.gameObject.transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.GetMouseButtonDown(0)) {
            placeCharacter();
        }
    }

    private void placeCharacter() {
        if (currentFriendObject == null) return;
        var gridSquare = gameGrid.getGridSquareAtPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        Debug.Log("CharacterPlacer; gridSquare ID when placing is " + gridSquare.GetInstanceID());
        if (gridSquare != null && gridSquare.canBePlacedOn && gridSquare.setFriend(currentFriendObject)) {
            var friendScript = currentFriendObject.GetComponent<FriendController>();
            friendScript.hasBeenPlaced = true;
            currentFriendObject = null;
        }
    }

    public void setCharacterToPlace(FriendController newFriendObject) {
        FriendController previousFriend = currentFriendObject;
        if (previousFriend != null) {
            var previousFriendController = previousFriend.GetComponent<FriendController>();
            previousFriendController.moveOffScreen();
        }
        currentFriendObject = newFriendObject;
    }
}
