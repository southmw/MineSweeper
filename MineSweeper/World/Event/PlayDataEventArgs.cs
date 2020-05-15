using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper.World
{
    public delegate void PlayDataEventHandler(object sender, PlayDataEventArgs e);

    public class PlayDataEventArgs : EventArgs
    {
        public int FlagCount{ get; private set; } = 0;

        public int QuestionCount { get; private set; } = 0;

        public string PlayState { get; private set; } = string.Empty;

        public PlayDataEventArgs(string playstate, int flagcount,int questioncount)
        {
            PlayState = playstate;
            FlagCount = flagcount;
            QuestionCount = questioncount;
        }
    }
}
