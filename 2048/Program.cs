using _2048.Services;
using _2048.Options;

var game = new GameRunner(new GameOptions { BoardRows = 4, BoardColumns = 4, NumberOfStartingCells = 2});

game.Run();
