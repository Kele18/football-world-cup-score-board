# football-world-cup-score-board

Library for managing a live scoreboard of Football World Cup matches.

## Techologies used

- **.NET 9** (Core Library)
- **In-Memory Data Store** via `ConcurrentDictionary`
- **Domain-Driven Design (DDD)**: `Match`, `Team`, `Score`, `GoalEvent`, etc.
- **Thread Safety**: `lock` to handle concurrency on match state
- **Extensible Design**: Interfaces like `IMatchDataSource`, `IScoreboard`, `IScoreBoardFactory`
- **Logging**: Optional logger via `ILogger<IScoreboard>` for observability
- **Unit Testing Ready**: Designed for testability using xUnit/NUnit/MSTest

## Core Functionalities

- **Schedule Match** : Add a match to the schedule
- **Start Match** : Move match to `InProgress` and track its start time  
- **Update Score** : Set the absolute score for each team
- **Undo Goal** : Undo the most recent goal for a team
- **Finish Match**: Ends the match, archives it.
- **Cancel/Abandon** : Handle special cases like cancellation or abandonment
- **GetLiveSummary** : Return `InProgress` matches otderd by total score and most recent start time

## How to use
```csharp
var scoreboard = new ScoreboardFactory().CreateDefault();

// Schedule a match
var match = scoreboard.ScheduleMatch(new Team("Argentina"), new Team("France"), DateTime.UtcNow.AddMinutes(10));

// Start it
scoreboard.StartMatch(match.Id);

// Update score
scoreboard.UpdateScore(match.Id, 2, 1);

// Finish match
scoreboard.FinishMatch(match.Id);

// Summary
var activeMatches = scoreboard.MatchesSummary();
```

### Future Improvements
- Introduce Match clock that tracks active play periods
- Support async data sources
- Undo last goal should be allowed after some perion of time?
