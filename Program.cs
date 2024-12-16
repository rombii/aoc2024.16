var maze = await File.ReadAllLinesAsync(Path.Join(Directory.GetCurrentDirectory(), "input.txt"));
var directions = new[] { (-1, 0), (0, 1), (1, 0), (0, -1) }; // Up, Right, Down, Left

(int X, int Y) start = (0, 0), finish = (0, 0);

for (var i = 0; i < maze.Length; i++)
{
    for (var j = 0; j < maze[i].Length; j++)
    {
        if (maze[i][j] == 'S')
            start = (i, j);
        else if (maze[i][j] == 'E')
            finish = (i, j);
    }
}

var (minCost, optimalPaths) = FindMinPathCost(maze, start, finish, directions);

Console.WriteLine($"First part: {minCost}");

var uniqueTiles = new HashSet<(int, int)>();
foreach (var path in optimalPaths)
    uniqueTiles.UnionWith(path);

Console.WriteLine($"Second part: {uniqueTiles.Count}");
return;

(long MinCost, List<HashSet<(int, int)>> OptimalPaths) FindMinPathCost(
    string[] maze,
    (int X, int Y) start,
    (int X, int Y) finish,
    (int DX, int DY)[] directions)
{
    var costs = new Dictionary<((int X, int Y) Pos, int Dir), long>
    {
        [(start, 1)] = 0
    };
    var optimalPaths = new Dictionary<((int X, int Y) Pos, int Dir), HashSet<(int, int)>>
    {
        [(start, 1)] = new() { start }
    };
    var pq = new PriorityQueue<((int X, int Y) Pos, int Dir), long>();
    pq.Enqueue((start, 1), 0);

    long minCost = long.MaxValue;

    while (pq.Count > 0)
    {
        var ((x, y), dir) = pq.Dequeue();
        var currentCost = costs[((x, y), dir)];

        if ((x, y) == finish)
        {
            minCost = Math.Min(minCost, currentCost);
            continue;
        }

        var (dx, dy) = directions[dir];
        var nx = x + dx;
        var ny = y + dy;

        if (IsValidTile(maze, nx, ny))
        {
            var newCost = currentCost + 1;
            UpdateState((nx, ny), dir, newCost, pq, costs, optimalPaths, (x, y), dir);
        }

        var leftDir = (dir - 1 + 4) % 4;
        var (lDx, lDy) = directions[leftDir];
        var lx = x + lDx;
        var ly = y + lDy;

        if (IsValidTile(maze, lx, ly))
        {
            var newCost = currentCost + 1001;
            UpdateState((lx, ly), leftDir, newCost, pq, costs, optimalPaths, (x, y), dir);
        }

        var rightDir = (dir + 1) % 4;
        var (rDx, rDy) = directions[rightDir];
        var rx = x + rDx;
        var ry = y + rDy;

        if (IsValidTile(maze, rx, ry))
        {
            var newCost = currentCost + 1001;
            UpdateState((rx, ry), rightDir, newCost, pq, costs, optimalPaths, (x, y), dir);
        }
    }

    var resultPaths = optimalPaths
        .Where(kvp => kvp.Key.Pos == finish && costs[kvp.Key] == minCost)
        .Select(kvp => kvp.Value)
        .ToList();

    return (minCost, resultPaths);
}

bool IsValidTile(string[] maze, int x, int y)
{
return x >= 0 && y >= 0 && x < maze.Length && y < maze[0].Length && maze[x][y] != '#';
}

void UpdateState(
    (int X, int Y) pos,
    int dir,
    long newCost,
    PriorityQueue<((int X, int Y) Pos, int Dir), long> pq,
    Dictionary<((int X, int Y) Pos, int Dir), long> costs,
    Dictionary<((int X, int Y) Pos, int Dir), HashSet<(int, int)>> optimalPaths,
    (int X, int Y) prevPos,
    int prevDir)
{
    if (!costs.ContainsKey((pos, dir)) || newCost < costs[(pos, dir)])
    {
        costs[(pos, dir)] = newCost;

        optimalPaths[(pos, dir)] = new HashSet<(int, int)>(optimalPaths[((prevPos), prevDir)])
        {
            pos
        };

        pq.Enqueue((pos, dir), newCost);
    }
    else if (newCost == costs[(pos, dir)])
    {
        optimalPaths[(pos, dir)].UnionWith(optimalPaths[((prevPos), prevDir)]);
    }
}


