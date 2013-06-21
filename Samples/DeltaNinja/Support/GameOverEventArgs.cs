using System;

namespace DeltaNinja
{
    public class GameOverEventArgs : EventArgs
    {
        public GameOverEventArgs(Score score)
        {
            this.Score = score;
        }

        public Score Score { get; private set; }
    }
}
