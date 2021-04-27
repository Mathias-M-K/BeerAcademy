namespace Data_Types
{
    public class DataPoint
    {
        // Start is called before the first frame update
        public readonly float round;
        public readonly float numberOfSips;
        public readonly float avgSip;

        private readonly float[] _temps;
        

        public DataPoint(float round, float numberOfSips,float avgSip)
        {
            this.round = round;
            this.avgSip = avgSip;
            this.numberOfSips = numberOfSips;
            
            _temps = new float[]{round,numberOfSips,avgSip};
        }
        
        public float this[int index]
        {
            get => _temps[index];
            set => _temps[index] = value;
        }
    }
}
