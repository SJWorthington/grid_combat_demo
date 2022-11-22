using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerButton : MonoBehaviour {
   
    [SerializeField] CharacterPlacer characterPlacer;
    private FriendController friendController;

    public void setFriend(FriendController friend) {
        friendController = friend;
    }

    public void onClick() {
        if (!friendController.hasBeenPlaced) {
            characterPlacer.setCharacterToPlace(friendController);
        }
    }
}
