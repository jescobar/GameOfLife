using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LifeApi.Data.Entities;

public class BoardData : IComparable<BoardData>
{
    public required List<List<bool>> Matrix { get; set; }
    public int Width { get { return Matrix.Count; } }
    public int Height { get { return Matrix[0].Count; } }
    public int ActiveCells { get; private set; }
    public int InactiveCells { get; private set; }
    public bool IsFinalState { get; set; }
    public bool IsErrorState { get; set; }

    [JsonIgnore]
    private BoardData? TempBoardData { get; set; }

    public (BoardData latestIteration, List<BoardData> allIterations) MoveTo(List<BoardData> historicBoardIterations, int maxIterations = 1, int errorAt = 1000)
    {
        var iterationsCount = 0;
        TempBoardData = new BoardData()
        {
            Matrix = Matrix.Select(row => new List<bool>(row)).ToList(),
            ActiveCells = ActiveCells,
            InactiveCells = InactiveCells
        };
        var boardIterations = new List<BoardData>();
        while (iterationsCount < maxIterations)
        {
            UpdateGrid();
            iterationsCount++;

            var allCellsAreInactive = TempBoardData.ActiveCells == 0 && TempBoardData.InactiveCells > 0;
            var repeatedPattern = boardIterations.Count > 0 && (boardIterations.LastOrDefault()!.CompareTo(TempBoardData) == 0 || boardIterations.Any(bi => bi.CompareTo(TempBoardData) == 0));
            var repeatedPatternHistoric =historicBoardIterations.Count > 0 && (historicBoardIterations.LastOrDefault()!.CompareTo(TempBoardData) == 0 || historicBoardIterations.Any(bi => bi.CompareTo(TempBoardData) == 0));

            TempBoardData.IsFinalState = repeatedPatternHistoric || repeatedPattern || allCellsAreInactive;


            if (TempBoardData.IsFinalState)
            {
                iterationsCount = maxIterations;
            }
            else if (iterationsCount >= errorAt)
            {
                TempBoardData.IsErrorState = true;
                iterationsCount = maxIterations;
            }
            boardIterations.Add(JsonSerializer.Deserialize<BoardData>(JsonSerializer.Serialize(TempBoardData!)));
        }

        return (TempBoardData!, boardIterations);
    }

    private int ComputeHash()
    {
        byte[] byteArr = Matrix.SelectMany(row => row).Select(b => b ? (byte)1 : (byte)0).ToArray();
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hash = sha256.ComputeHash(byteArr);
            return BitConverter.ToInt32(hash);
        }
    }

    private void UpdateGrid()
    {
        if (TempBoardData == null)
        {
            return;
        }
        var nextGrid = Enumerable.Range(0, Width).Select(_ => new List<bool>(new bool[Height])).ToList();
        TempBoardData.ActiveCells = 0;
        TempBoardData.InactiveCells = 0;
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                int aliveNeighbors = CountAliveNeighbors(x, y);
                nextGrid[x][y] = ApplyRules(TempBoardData!.Matrix[x][y], aliveNeighbors);

                if (nextGrid[x][y])
                {
                    TempBoardData.ActiveCells++;
                }
                else
                {
                    TempBoardData.InactiveCells++;
                }
            }
        }
        TempBoardData.Matrix = nextGrid;
    }

    private int CountAliveNeighbors(int x, int y)
    {
        if (TempBoardData == null)
        {
            return 0;
        }

        int count = 0;
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx != 0 || dy != 0)
                {
                    int neighborX = (x + dx + Width) % Width;
                    int neighborY = (y + dy + Height) % Height;
                    count += TempBoardData.Matrix[neighborX][neighborY] ? 1 : 0;
                }
            }
        }
        return count;
    }

    private bool ApplyRules(bool cellIsAlive, int aliveNeighbors)
    {
        if (cellIsAlive)
        {
            return aliveNeighbors == 2 || aliveNeighbors == 3;  // Survival rule
        }
        else
        {
            return aliveNeighbors == 3;  // Birth rule
        }
    }

    public int CompareTo(BoardData? other)
    {
        return ComputeHash() - other?.ComputeHash() ?? 0;
    }
}
