namespace OffenseDefense.Shared
{
    class Version
    {
        private const int major = 1;
        private const int minor = 1;
        private const int fixVersion = 0;

        public static int Major
        {
            get { return major; }
        }

        public static int Minor
        {
            get { return minor; }
        }

        public static int FixVersion
        {
            get { return fixVersion; }
        }
    }
}