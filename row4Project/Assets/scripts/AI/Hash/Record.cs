
public class Record {
    public int hashValue;
    public int minScore;
    public int maxScore;
    public int bestMove;
    public int depth;

    public Record ()
    {
        hashValue = 0;
        minScore = 0;
        maxScore = 0;
        bestMove = 0;
        depth = 0;
    }
}
