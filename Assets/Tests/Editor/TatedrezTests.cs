
using NUnit.Framework;
using Game.Tatedrez.Model;
using Game.Tatedrez.Factory;
public class TatedrezTests
{
    [Test]
    public void Player_AddAndRemovePiece()
    {
        // Arrange
        var player = new Player(PlayerColor.White);
        var knight = new Knight(PlayerColor.White);

        // Act
        player.AddPiece(knight);

        // Assert
        Assert.AreEqual(1, player.Pieces.Count, "Player should have 1 piece after adding.");

        // Act
        bool removed = player.RemovePiece(knight);

        // Assert
        Assert.IsTrue(removed, "Removing a piece should return true.");
        Assert.AreEqual(0, player.Pieces.Count, "Player should have 0 pieces after removing.");
    }

    [Test]
    public void Piece_IsValidMove_Knight()
    {
        // Arrange
        var knight = new Knight(PlayerColor.White);
        var board = new Board(3, 3);

        // Act & Assert
        Assert.IsTrue(knight.IsValidMove(0, 0, 1, 2, board), "Knight should be able to move in an L-shape.");
        Assert.IsFalse(knight.IsValidMove(0, 0, 2, 2, board), "Knight should not move diagonally.");
    }

    [Test]
    public void Piece_IsValidMove_Rook()
    {
        // Arrange
        var rook = new Rook(PlayerColor.White);
        var board = new Board(3, 3);

        // Act & Assert
        Assert.IsTrue(rook.IsValidMove(0, 0, 0, 2, board), "Rook should move vertically.");
        Assert.IsTrue(rook.IsValidMove(0, 0, 2, 0, board), "Rook should move horizontally.");
        Assert.IsFalse(rook.IsValidMove(0, 0, 1, 1, board), "Rook should not move diagonally.");
    }

    [Test]
    public void Board_PlaceAndMovePiece()
    {
        // Arrange
        var board = new Board(3, 3);
        var rook = new Rook(PlayerColor.White);

        // Act
        bool placed = board.PlacePiece(rook, 0, 0);
        bool moved = board.MovePiece(0, 0, 0, 2);

        // Assert
        Assert.IsTrue(placed, "Piece should be placed successfully.");
        Assert.IsTrue(moved, "Piece should be moved successfully.");
        Assert.IsNull(board.GetPiece(0, 0), "Original position should be empty after move.");
        Assert.AreEqual(rook, board.GetPiece(0, 2), "New position should contain the moved piece.");
    }

    [Test]
    public void Board_CheckForTicTacToe()
    {
        // Arrange
        var board = new Board(3, 3);
        var rook1 = new Rook(PlayerColor.White);
        var rook2 = new Rook(PlayerColor.White);
        var rook3 = new Rook(PlayerColor.White);

        // Act
        board.PlacePiece(rook1, 0, 0);
        board.PlacePiece(rook2, 1, 0);
        board.PlacePiece(rook3, 2, 0);

        // Assert
        Assert.IsTrue(board.CheckForTicTacToe(PlayerColor.White), "White should have a TicTacToe.");
    }

    [Test]
    public void GameState_TransitionToDynamicPhase()
    {
        // Arrange
        var player1 = new Player(PlayerColor.White);
        var player2 = new Player(PlayerColor.Black);
        var board = new Board(3, 3);
        var gameState = new GameState(player1, player2, board);

        var rook1 = new Rook(PlayerColor.White);
        var rook2 = new Rook(PlayerColor.Black);

        // Act
        gameState.PlacePiece(rook1, 0, 0);
        gameState.PlacePiece(rook2, 1, 1);

        // Assert
        Assert.AreEqual(GameState.State.PlacementPhase, gameState.CurrentState, "Game should remain in PlacementPhase until all pieces are placed.");

        // Complete placement
        player1.AddPiece(new Rook(PlayerColor.White));
        player2.AddPiece(new Rook(PlayerColor.Black));
        gameState.PlacePiece(new Rook(PlayerColor.White), 2, 2);
        gameState.PlacePiece(new Rook(PlayerColor.Black), 0, 2);

        Assert.AreEqual(GameState.State.DynamicPhase, gameState.CurrentState, "Game should transition to DynamicPhase after placement.");
    }

    [Test]
    public void GameState_DetectWin()
    {
        // Arrange
        var player1 = new Player(PlayerColor.White);
        var player2 = new Player(PlayerColor.Black);
        var board = new Board(3, 3);
        var gameState = new GameState(player1, player2, board);

        var rook1 = new Rook(PlayerColor.White);
        var rook2 = new Rook(PlayerColor.White);
        var rook3 = new Rook(PlayerColor.White);

        // Act
        gameState.PlacePiece(rook1, 0, 0);
        gameState.PlacePiece(new Rook(PlayerColor.Black), 1, 1); // Opponent move
        gameState.PlacePiece(rook2, 1, 0);
        gameState.PlacePiece(new Rook(PlayerColor.Black), 2, 2); // Opponent move
        gameState.PlacePiece(rook3, 2, 0);

        // Assert
        Assert.AreEqual(GameState.State.Completed, gameState.CurrentState, "Game should end in Completed state.");
        Assert.IsTrue(board.CheckForTicTacToe(PlayerColor.White), "White should win with a TicTacToe.");
    }

    [Test]
    public void GameState_CheckTotalMoves()
    {
        // Arrange
        var player1 = new Player(PlayerColor.White);
        var player2 = new Player(PlayerColor.Black);
        var board = new Board(3, 3);
        var gameState = new GameState(player1, player2, board);

        var rook1 = new Rook(PlayerColor.White);
        var rook2 = new Rook(PlayerColor.White);

        // Act
        gameState.PlacePiece(rook1, 0, 0);
        gameState.PlacePiece(new Rook(PlayerColor.Black), 1, 1); // Opponent move
        gameState.PlacePiece(rook2, 1, 0);
        gameState.PlacePiece(new Rook(PlayerColor.Black), 2, 2); // Opponent move


        // Assert
        Assert.AreEqual(4,gameState.TotalMoves);
    }

    [Test]
    public void PieceFactoryTest_CheckColorAndPieceType()
    {   
        //Arrange
        IPieceFactory pieceFactory = new PieceFactory();

        //Act
        var rook = pieceFactory.CreatePiece(PieceType.Rook,PlayerColor.Black);

        // Assert
        Assert.AreEqual(rook.GetPieceType(),PieceType.Rook);
        Assert.AreEqual(rook.Owner, PlayerColor.Black);
    }

    [Test]
    public void PlayerDefaultPieceTest()
    {   
        //Arrange
        IPieceFactory pieceFactory = new PieceFactory();
        var player = new Player(PlayerColor.White);
        var player2 = new Player(PlayerColor.Black);

        //Act
        var pieces = pieceFactory.CreateDefaultPieces();
        player.InitPlayerPieces(pieces);
        player2.InitPlayerPieces(pieceFactory.CreateDefaultPieces());

        // Assert
        Assert.AreEqual(player.GetAvailablePieceCount(PieceType.Rook),1);
        Assert.AreEqual(player.GetAvailablePieceCount(PieceType.Bishop),1);
        Assert.AreEqual(player.GetAvailablePieceCount(PieceType.Knight),1);

        //Act
        player.DeductAvailablePiece(PieceType.Knight);
        Assert.AreEqual(player.GetAvailablePieceCount(PieceType.Knight),0);
        Assert.AreEqual(player2.GetAvailablePieceCount(PieceType.Knight),1);
    }
}
