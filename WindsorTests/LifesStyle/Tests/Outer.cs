namespace WindsorTests.LifesStyle.Tests
{
    class Outer : IOuter
    {
        public Outer(IA aOuter)
        {
            AOuter = aOuter;
        }

        public IA AOuter { get; }
    }
}