using LifeApi.Data.Entities;

namespace LifeApi.BusinessLogic.Test;

public class BoardDataTests
{

    [Fact]
    public void TestNextState_BlinkerPattern()
    {
        // Arrange
        var initialMatrix = new List<List<bool>>
        {
            new List<bool> { false,false, false, false, false,false,false },
            new List<bool> { false,false, false, false, false,false,false },
            new List<bool> { false,false, false, true, false,false,false },
            new List<bool> { false,false, false, true, false,false,false },
            new List<bool> { false,false, false, true, false,false,false },
            new List<bool> { false,false, false, false, false,false,false },
            new List<bool> { false,false, false, false, false,false,false },
        };
        var boardData = new BoardData { Matrix = initialMatrix };

        // Act
        var nextState = boardData.MoveTo(new List<BoardData>(), 1).latestIteration.Matrix;
        var trueCells = new List<(int, int)>() {
            { (3, 2) }, { (3, 3) }, {(3, 4)}
        };
        verifyBoard(nextState, trueCells);
    }

    private static void verifyBoard(List<List<bool>> nextState, List<(int, int)> trueCells)
    {
        for (int x = 0; x < nextState.Count; x++)
        {
            for (int y = 0; y < nextState[x].Count; y++)
            {
                if (trueCells.Contains((x, y)))
                {
                    Assert.True(nextState[x][y]);
                }
                else
                {
                    Assert.False(nextState[x][y]);
                }
            }
        }
    }

    [Fact]
    public void TestRepeatingPattern_GliderPattern()
    {
        // Arrange
        var initialMatrix = new List<List<bool>>
        {
            new List<bool> { false, false, false, false, false, false, false, false, false, false },
            new List<bool> { false, false, false, false, false, false, false, false, false, false },
            new List<bool> { false, false, false, false, false, false, false, false, false, false },
            new List<bool> { false, false, false, false, false, false, false, false, false, false },
            new List<bool> { false, false, false, false, false, false, false, false, false, false },
            new List<bool> { false, false, false, false, false, true, false, false, false, false },
            new List<bool> { false, false, false, false, false, false, true, false, false, false },
            new List<bool> { false, false, false, false, true, true, true, false, false, false },
            new List<bool> { false, false, false, false, false, false, false, false, false, false },
            new List<bool> { false, false, false, false, false, false, false, false, false, false }
        };
        var boardData = new BoardData { Matrix = initialMatrix };

        // Act
        var nextState = boardData.MoveTo(new List<BoardData>(), 4).latestIteration.Matrix;
        var trueCells = new List<(int, int)>() {
            { (6, 6) }, { (7, 7) }, {(8, 5)}, {(8, 6)}, {(8, 7)}
        };
        verifyBoard(nextState, trueCells);
    }

    [Fact]
    public void TestAllCellsDiePattern_EmptyBoard()
    {
        // Initialize a pattern where all cells die
        var boardData = new BoardData
        {
            Matrix = new List<List<bool>>
            {
                new List<bool> { false, false, false },
                new List<bool> { false, true, false },
                new List<bool> { false, false, false }
            }
        };

        var historicBoardIterations = new List<BoardData>();
        var (latestIteration, allIterations) = boardData.MoveTo(historicBoardIterations, 1);

        // Assert all cells die
        Assert.True(latestIteration.IsFinalState);
        Assert.Equal(0, latestIteration.ActiveCells);
        Assert.Equal(9, latestIteration.InactiveCells);

    }

    [Fact]
    public void TestNextState_MoveToRepeated()
    {
        // Arrange
        var initialMatrix = new List<List<bool>>
        {
            new List<bool> { false,false, false, false, false,false,false },
            new List<bool> { false,false, false, false, false,false,false },
            new List<bool> { false,false, false, true, false,false,false },
            new List<bool> { false,false, false, true, false,false,false },
            new List<bool> { false,false, false, true, false,false,false },
            new List<bool> { false,false, false, false, false,false,false },
            new List<bool> { false,false, false, false, false,false,false },
        };
        var boardData = new BoardData { Matrix = initialMatrix };

        // Act
        var result = boardData.MoveTo(new List<BoardData>(), 5);

        Assert.True(result.latestIteration.IsFinalState);
        Assert.Equal(3, result.allIterations.Count);
    }

    [Fact]
    public void TestNextState_MoveToError()
    {
        // Arrange
        var initialMatrix = new List<List<bool>>
        {
            new() {false, false, false, false, false},
            new() {false, true, true, false, false},
            new() {false, false, true, false, false},
            new() {false, false, true, false, false},
        };
        var boardData = new BoardData { Matrix = initialMatrix };

        // Act
        var result = boardData.MoveTo(new List<BoardData>(), 100, 5);

        Assert.False(result.latestIteration.IsFinalState);
        Assert.True(result.latestIteration.IsErrorState);
        Assert.Equal(5, result.allIterations.Count);
    }
}