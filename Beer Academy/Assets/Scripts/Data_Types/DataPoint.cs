namespace Data_Types
{
    public class DataPoint
    {
        // Start is called before the first frame update
        public readonly float round;
        public readonly float numberOfSips;
        public readonly float avgSip;
        public readonly float turnTime;
        public readonly float avgTime;

        private readonly float[] _temps;
        

        public DataPoint(float round, float numberOfSips,float avgSip,float turnTime, float avgTime)
        {
            this.round = round;
            this.avgSip = avgSip;
            this.numberOfSips = numberOfSips;
            this.turnTime = turnTime;
            this.avgTime = avgTime;
            
            _temps = new float[]{round,numberOfSips,avgSip,turnTime,avgTime};
        }
        
        public float this[int index]
        {
            get => _temps[index];
            set => _temps[index] = value;
        }
    }
}
