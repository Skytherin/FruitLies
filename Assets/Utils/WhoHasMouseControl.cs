namespace Assets.Utils
{
    public static class Global
    {
        public static Mouser WhoHasMouseControl = Mouser.Cutscene;
    }

    public enum Mouser
    {
        Cutscene,
        General,
        Death
    }
}