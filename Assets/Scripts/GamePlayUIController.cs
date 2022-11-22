using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayUIController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI hpField;
    [SerializeField] TextMeshProUGUI strengthField;
    [SerializeField] TextMeshProUGUI defenceField;
    [SerializeField] TextMeshProUGUI movementField;
    [SerializeField] TextMeshProUGUI rangeField;
    [SerializeField] TextMeshProUGUI levelField;
    [SerializeField] Image playerUIImage;
    [SerializeField] TextMeshProUGUI gameStatusField;


    //My life would be so much easier if I'd made the stats a model object I could just pass to here
    public void setHp(int current, int max) {
        hpField.text = $"{current} / {max}";
    }

    public void setStrength(int strength) {
        strengthField.text = strength.ToString();
    }

    public void setDefence(int defence) {
        defenceField.text = defence.ToString();
    }

    public void setMovement(int movement) {
        movementField.text = movement.ToString();
    }

    public void setRange(int range) {
        rangeField.text = range.ToString();
    }

    public void setLevel(int level) {
        levelField.text = level.ToString();
    }

    public void setImage(Sprite imageForUI) {
        playerUIImage.sprite = imageForUI;
    }

    public void setGameStatusText(string textToDisplay) {
        gameStatusField.text = textToDisplay;
    }
}
