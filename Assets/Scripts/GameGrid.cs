using System;
using System.Collections.Generic;
using UnityEngine;

public class GameGrid : MonoBehaviour {
    [SerializeField] GameObject gridSquarePrefab;
    [SerializeField] private float size = 1f;
    [SerializeField] int rowCount;
    [SerializeField] int columnCount;
    [SerializeField] EnemyInitialiser enemyInitialiser;

    private GridSquare[,] gridSquares;

    private void Awake() {
        gridSquares = new GridSquare[rowCount, columnCount];
        generateGrid();
    }

    private void generateGrid() {
        var xstart = transform.position.x;
        var yStart = transform.position.y;
        for (int row = 0; row < rowCount; row++) {
            for (int column = 0; column < columnCount; column++) {
                var gridSquareObject = Instantiate(gridSquarePrefab, new Vector2(column + xstart, row + yStart), Quaternion.identity);
                var gridSquareController = gridSquareObject.GetComponent<GridSquare>();

                gridSquares[row, column] = gridSquareController;
                gridSquareController.rowInGrid = row;
                gridSquareController.columnInGrid = column;
            }
        }
        initialiseEnemies();
    }

    public void displaySquaresForInitialPlacement(List<Tuple<int, int>> positions) {

        foreach (Tuple<int, int> position in positions) {
            var gridSquare = gridSquares[position.Item1, position.Item2];
            if (!gridSquare.isOccupiedByFriend) {
                gridSquare.setCanBePlacedOn();
            }
        }
    }

    //A couple times this has come up an NPE, must have a race condition
    public void setAllSquaresNeutral() {
        for (int row = 0; row < gridSquares.GetLength(0); row++) {
            for (int column = 0; column < gridSquares.GetLength(1); column++) {
                gridSquares[row, column].GetComponent<GridSquare>().setNeutral();
            }
        }
    }

    public GridSquare getGridSquareAtPoint(Vector2 point) {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null && hit.transform.CompareTag("GridSquare")) {
            var gridSquare = hit.transform.GetComponent<GridSquare>();
            return gridSquare;
        } else {
            return null;
        }
    }

    //This probably goes elsewhere
    private void initialiseEnemies() {
        var squaresToInitialise = new List<GridSquare>();

        for (int row = 0; row < rowCount; row++) {
            squaresToInitialise.Add(gridSquares[row, columnCount - 1]);
        }

        enemyInitialiser.initialiseEnemies(squaresToInitialise);
    }


    public void displayMoveAndAttackFromSquare(GridSquare gridSquare, int moveRemaining, int attackRange) {
        if (gridSquare == null) return;

        var gridSquareRow = gridSquare.rowInGrid;
        var gridSquareColumn = gridSquare.columnInGrid;
        if (moveRemaining > 0) {
            gridSquare.setCanBeMovedTo();
            displayMoveAndAttackFromSquare(getGridSquareOrNull(gridSquareRow + 1, gridSquareColumn), moveRemaining - 1, attackRange);
            displayMoveAndAttackFromSquare(getGridSquareOrNull(gridSquareRow, gridSquareColumn + 1), moveRemaining - 1, attackRange);
            displayMoveAndAttackFromSquare(getGridSquareOrNull(gridSquareRow - 1, gridSquareColumn), moveRemaining - 1, attackRange);
            displayMoveAndAttackFromSquare(getGridSquareOrNull(gridSquareRow, gridSquareColumn - 1), moveRemaining - 1, attackRange);
        } else if (attackRange > 0 && !gridSquare.canBeMovedTo) {
            gridSquare.setIsInAttackRange();
            displayMoveAndAttackFromSquare(getGridSquareOrNull(gridSquareRow + 1, gridSquareColumn), moveRemaining, attackRange - 1);
            displayMoveAndAttackFromSquare(getGridSquareOrNull(gridSquareRow, gridSquareColumn + 1), moveRemaining, attackRange - 1);
            displayMoveAndAttackFromSquare(getGridSquareOrNull(gridSquareRow - 1, gridSquareColumn), moveRemaining, attackRange - 1);
            displayMoveAndAttackFromSquare(getGridSquareOrNull(gridSquareRow, gridSquareColumn - 1), moveRemaining, attackRange - 1);
        }
    }

    public void displayAttackFromSquare(GridSquare gridSquare, int attackRange, int originatingRow, int originatingColumn) {
        if (gridSquare == null) return;
        if (originatingAttackSquare == null) {
            originatingAttackSquare = gridSquare;
        }
        if (attackRange < 0) {
            originatingAttackSquare = null;
            return;
        }

        var gridSquareRow = gridSquare.rowInGrid;
        var gridSquareColumn = gridSquare.columnInGrid;
        if (gridSquareRow != originatingRow || gridSquareColumn != originatingColumn) {
            gridSquare.setCanBeAttacked();
        }
        displayAttackFromSquare(getGridSquareOrNull(gridSquareRow + 1, gridSquareColumn), attackRange - 1, originatingRow, originatingColumn);
        displayAttackFromSquare(getGridSquareOrNull(gridSquareRow - 1, gridSquareColumn), attackRange - 1, originatingRow, originatingColumn);
        displayAttackFromSquare(getGridSquareOrNull(gridSquareRow, gridSquareColumn + 1), attackRange - 1, originatingRow, originatingColumn);
        displayAttackFromSquare(getGridSquareOrNull(gridSquareRow, gridSquareColumn - 1), attackRange - 1, originatingRow, originatingColumn);
    }

    GridSquare originatingAttackSquare;
    public void displayAttackFromSquare(GridSquare gridSquare, int attackRange) {
        if (gridSquare == null) return;
        if (originatingAttackSquare == null) {
            originatingAttackSquare = gridSquare;
        }
        if (attackRange < 0) {
            originatingAttackSquare = null;
            return;
        }

        var gridSquareRow = gridSquare.rowInGrid;
        var gridSquareColumn = gridSquare.columnInGrid;
        var originatingRow = originatingAttackSquare.rowInGrid;
        var originatingColumn = originatingAttackSquare.columnInGrid;
        if (gridSquareRow != originatingRow || gridSquareColumn != originatingColumn) {
            gridSquare.setCanBeAttacked();
        }
        displayAttackFromSquare(getGridSquareOrNull(gridSquareRow + 1, gridSquareColumn), attackRange - 1);
        displayAttackFromSquare(getGridSquareOrNull(gridSquareRow - 1, gridSquareColumn), attackRange - 1);
        displayAttackFromSquare(getGridSquareOrNull(gridSquareRow, gridSquareColumn + 1), attackRange - 1);
        displayAttackFromSquare(getGridSquareOrNull(gridSquareRow, gridSquareColumn - 1), attackRange - 1);
    }

    private GridSquare getGridSquareOrNull(int row, int column) {
        try {
            var gridSquare = gridSquares[row, column];
            return gridSquare;
        } catch (Exception) {
            return null;
        }
    }

    private void setAttackSquare(GridSquare gridSquare) {
        if (gridSquare != null && !gridSquare.canBeMovedTo) {
            gridSquare.setCanBeAttacked();
        }
    }
}
