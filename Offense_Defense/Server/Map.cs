namespace OffenseDefense.Server
{
    class Map
    {
        private string name;
        private string fileName;

        private int totalCheckpoints;

        public Map(string name, string fileName, int checkpoints)
        {
            this.name = name;
            this.fileName = fileName;
            this.totalCheckpoints = checkpoints;
        }

        public int getTotalCheckpoints()
        {
            return this.totalCheckpoints;
        }

        public string getName()
        {
            return this.name;
        }

        public string getFileName()
        {
            return this.fileName;
        }
    }
}