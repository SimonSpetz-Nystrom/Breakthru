using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace BreakthruGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Private variables and properties

        #region GUI

        private Image mSelectedImg = null;
        private List<Image> mImages;
        private Storyboard mStoryboard;
        private DoubleAnimation mXAnim;
        private DoubleAnimation mYAnim;
        private Dictionary<Image, Point> mOldPositions;
        private bool mLoaded;

        /// <summary>
        /// Square size used to convert between grid coordinates and board coordinates
        /// </summary>
        private double SquareSize
        {
            get { return (double)this.Resources["SquareSize"]; }
        }

        #endregion

        #region Gameplay

        private BreakthruGameBoard mBoard;
        ExternalPlayer mPlayer;
        private Semaphore mMoveSema;
        private Move mMove;
        private Thread mGameThread;

        #endregion

        #endregion

        #region Public properties

        private PlayerRole? mCurrentRole;
        /// <summary>
        /// The PlayerRole of the player who shall move.
        /// This property is only changed when the GUI is told by the rule system to perform a move.
        /// If one or both of the players are not human controlled the property will not update.
        /// The role is set to null when a player has won the game.
        /// </summary>
        public PlayerRole? CurrentRole
        {
            get { return mCurrentRole; }
            set { mCurrentRole = value; OnPropertyChanged("CurrentRole"); }
        }

        #endregion

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Initialize player with event handlers. In the current "human vs human on one computer mode", the same player is used for both players.
            mPlayer = new ExternalPlayer();
            mPlayer.MoveRequested += PlayerMoveRequested;
            mPlayer.IllegalMoveAttempted += PlayerIllegalMoveAttempted;
            mPlayer.StartingPlayerRequested += PlayerStartingPlayerRequested;

            //Initialize animation and storyboard
            mXAnim = this.Resources["xAnimation"] as DoubleAnimation;
            mYAnim = this.Resources["yAnimation"] as DoubleAnimation;
            mStoryboard = new Storyboard { Children = new TimelineCollection { mXAnim, mYAnim } };

            mMoveSema = new Semaphore(0, 1);

            this.DataContext = this;
        }

        #region Event handlers

        #region Gameplay events

        #region Called in game thread from rule system

        /// <summary>
        /// Event handler for when a (human) player is requested a move by the rule system.
        /// This method will be called in the game thread and will wait for the GUI thread to perform a move which it then returns.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Contains information on which player role shall move</param>
        /// <returns>The move performed by the user</returns>
        private Move PlayerMoveRequested(object sender, MoveRequestEventArgs e)
        {
            CurrentRole = e.Role;
            mMoveSema.WaitOne();
            return mMove;
        }

        /// <summary>
        /// Called when a player has attempted an illegal move.
        /// Will show illegal move marker in GUI and return the new move that the user selects.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Contains (among other things the previously attempted (illegal) move</param>
        /// <returns>The new move performed by the user</returns>
        private Move PlayerIllegalMoveAttempted(object sender, IllegalMoveEventArgs e)
        {
            this.Dispatcher.Invoke((Action)(
                () =>
                {
                    Grid.SetRow(IllegalMoveMarker, (int)e.Move.Destination.Y);
                    Grid.SetColumn(IllegalMoveMarker, (int)e.Move.Destination.X);
                    (this.Resources["FadeInOutAnim"] as Storyboard).Begin(this);
                }));

            mMoveSema.WaitOne();
            return mMove;
        }

        /// <summary>
        /// Event handler for when the player has received a request to select the starting player.
        /// The method assumes that this question is only asked to the gold player and opens a Yes/No dialog
        /// where the gold player can determine if he/she wants to start.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        /// <returns>The role of the starting player (Gold if Yes, otherwise Silver)</returns>
        private PlayerRole PlayerStartingPlayerRequested(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Gold player, do you want to start?", "Start decision", MessageBoxButton.YesNo, MessageBoxImage.Question);

            return result == MessageBoxResult.Yes ? PlayerRole.Gold : PlayerRole.Silver;
        }

        #endregion

        #region GUI event handlers

        /// <summary>
        /// Event handler for when an image (=piece) is clicked. 
        /// Selects the piece if it matches the current role, otherwise does nothing.
        /// If the piece clicked does not match the role the event will not be marked as handled 
        /// and can instead be handled as a click on the board (=move).
        /// </summary>
        /// <param name="sender">The image clicked</param>
        /// <param name="e">Only used to mark as handled</param>
        private void ImageClick(object sender, MouseButtonEventArgs e)
        {
            Image img = sender as Image;
            BreakthruGamePiece piece = img.DataContext as BreakthruGamePiece;

            if (mCurrentRole == PlayerRole.Gold && (piece.TypeOfPiece == BreakthruGamePieceType.GoldEscort || piece.TypeOfPiece == BreakthruGamePieceType.GoldFlagship) || mCurrentRole == PlayerRole.Silver && piece.TypeOfPiece == BreakthruGamePieceType.Silvership)
            {
                if (mSelectedImg != null)
                {
                    mSelectedImg.Opacity = 1;
                }

                mSelectedImg = img;
                img.Opacity = 0.5;

                e.Handled = true;
            }
        }

        /// <summary>
        /// Event handler for when the user has clicked on the game board.
        /// If the user has selected a piece an attempt to move to the clicked position will be made, otherwise nothing happens.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Additional event information, e.g. position of mouse</param>
        private void GameBoard_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (mSelectedImg != null)
            {
                // Calculate board coordinates from grid coordinates
                var position = e.GetPosition(GameBoard);
                int x = (int)(position.X / SquareSize);
                int y = (int)(position.Y / SquareSize);

                // Try to perform the move
                BreakthruGamePiece piece = mSelectedImg.DataContext as BreakthruGamePiece;

                // Set the move to the requested move and release semaphore. This will result in a move attempt in the game thread by PlayerMoveRequested.
                mMove = new Move(piece, new Point(x, y));
                mMoveSema.Release();

                // Reset selected image
                mSelectedImg.Opacity = 1;
                mSelectedImg = null;
            }
        }

        /// <summary>
        /// When the position of a piece has changed, this method runs an animation to display the change.
        /// </summary>
        /// <param name="sender">The image that is bound to the moved piece</param>
        /// <param name="e">Not used</param>
        private void PiecePositionUpdated(object sender, DataTransferEventArgs e)
        {
            if (mLoaded)
            {
                Image src = sender as Image;

                mXAnim.From = SquareSize * mOldPositions[src].X;
                mYAnim.From = SquareSize * mOldPositions[src].Y;

                Storyboard.SetTarget(mXAnim, src);
                Storyboard.SetTarget(mYAnim, src);

                mStoryboard.Begin(this);

                mOldPositions[src] = (src.DataContext as GamePiece).Position;
            }
        }

        /// <summary>
        /// Called when the binding for the winner block text has been changed (via bindings).
        /// This could (if the new text is not empty) mean that somebody has won and CurrentRole should be set to null.
        /// </summary>
        /// <param name="sender">The text block</param>
        /// <param name="e">Not used</param>
        private void WinnerBlockTextUpdated(object sender, DataTransferEventArgs e)
        {
            if (!String.IsNullOrEmpty((sender as TextBlock).Text))
            {
                CurrentRole = null;
            }
        }

        /// <summary>
        /// Called when the menu item for new game is selected. Starts a new game.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void NewGameClick(object sender, RoutedEventArgs e)
        {
            StartNewGame();
        }

        #endregion

        #endregion

        #region Window events

        /// <summary>
        /// Event handler for when the window has loaded. Used to start the game thread and to "activate" piece movements
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialize variables related to the actual game
            mOldPositions = new Dictionary<Image, Point>();
            mBoard = new BreakthruGameBoard();
            mImages = new List<Image>();

            foreach (var piece in mBoard.Pieces)
            {
                Image img = new Image();
                img.DataContext = piece;
                mOldPositions.Add(img, piece.Position);
                GameBoard.Children.Add(img);

                mImages.Add(img);
            }


            mLoaded = true;

            StartNewGame();
        }

        /// <summary>
        /// Event handler for when the window has closed. Aborts the game thread to ensure the program dies.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void Window_Closed(object sender, EventArgs e)
        {
            mGameThread.Abort();
        }

        #endregion

        #endregion

        #region Help methods

        /// <summary>
        /// Starts a new game. This method assumes mBoard and mPlayer are already properly initialized (i.e. not null).
        /// </summary>
        private void StartNewGame()
        {
            mBoard.ResetPieces();
            if (mGameThread != null) mGameThread.Abort();
            BreakthruGameplay gameplay = new BreakthruGameplay(mBoard, mPlayer, mPlayer);
            WinnerBlock.DataContext = gameplay;
            mGameThread = new Thread(gameplay.PlayGame);
            mGameThread.Start();
        }

        #endregion
    }
}
