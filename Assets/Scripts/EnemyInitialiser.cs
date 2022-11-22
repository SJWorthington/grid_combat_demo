using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInitialiser : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;
    private float enemyPositionYOffset = 0.25f;
    [SerializeField] GameSetupController gameSetup;

    public void initialiseEnemies(List<GridSquare> gridSquares) {

        List<EnemyController> enemyControllers = new List<EnemyController>();

        foreach (GridSquare square in gridSquares) {
            var squareToSet = square.GetComponent<GridSquare>();
            var enemyObject = Instantiate(
                enemyPrefab,
                new Vector2(square.transform.position.x, square.transform.position.y + enemyPositionYOffset),
                Quaternion.identity);
            var enemyController = enemyObject.GetComponent<EnemyController>();
            enemyControllers.Add(enemyController);
            squareToSet.setCanBePlacedOn();
            squareToSet.setEnemy(enemyController);
            squareToSet.setNeutral();
        }

        gameSetup.setEnemies(enemyControllers);
    }
}
