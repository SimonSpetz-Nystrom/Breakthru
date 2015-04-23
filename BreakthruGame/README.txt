The board game this game describes is called Breakthru. The rules for the game can be found on Wikipedia: http://en.wikipedia.org/wiki/Breakthru_(board_game)

The lab 2 version of the code includes a complete rule engine which allows for a full game to be played. The class BreakthruGameplay is responsible
for playing the game, asking the players (of class Player) for moves and then verifying them. 

All games are played between two human players on the same computer, but the code is constructed in a way that easily allows for other types
of players (e.g. AI or online) if that is a request for a future version (beyond the range of the TDDD49 labs).

The game is played by selecting a piece by clicking on the piece and then on the position the piece shall be moved to. Illegal moves will be
shown by displaying a red X on the board where the user wanted to move a piece. The current player is displayed above the board. When a player has won
that text updates to write the winner instead. A new game can be started at any point by selecting "New Game".

The tests located in TestBreakthruGame cover all public methods of the game. The tests are fairly simple, but still cover >63% of the lines in breakthrugame.exe.
There are currently no tests for MainWindow.

//Simon Spetz-Nyström