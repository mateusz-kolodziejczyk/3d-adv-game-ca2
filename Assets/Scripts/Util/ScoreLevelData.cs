using System;

namespace Util
{
    [Serializable]
    public class ScoreLevelData
    {
        public int score;
        public int level;

        public ScoreLevelData(int score, int level)
        {
            this.score = score;
            this.level = level;
        }
    }
}