namespace Mw3_Fix_Online.Core
{
    public class ValueDescriptionPair
    {
        public FixMethod Key
        {
            get;
            set;
        }
        public string Description
        {
            get;
            set;
        }
    }

    public enum FixMethod
    {
        MainPlayer,
        Controller2,
        Controller3,
        Controller4
    }
}
