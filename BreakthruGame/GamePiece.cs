using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BreakthruGame
{
    /// <summary>
    /// Abstract class describing a game piece. An abstract game piece is identified only by its position.
    /// </summary>
    public abstract class GamePiece : INotifyPropertyChanged
    {
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

        private Point mPosition;
        /// <summary>
        /// The position of the piece. This property is unsafe in the sense that a piece 
        /// can be moved to any position no matter the rules of the game. Therefore the 
        /// property should not be changed before a rule system has verified that the new position is legal.
        /// </summary>
        public Point Position
        {
            get
            {
                return mPosition;
            }
            set
            {
                mPosition = value;
                OnPropertyChanged("Position");
            }
        }

    }
}
