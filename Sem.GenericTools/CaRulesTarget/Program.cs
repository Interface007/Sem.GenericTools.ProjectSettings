namespace CaRulesTarget
{
    class Program
    {
        static void Main(string[] args)
        {
            GuardClass.EnsureNotNull(args, "args");
        }
    }
}
