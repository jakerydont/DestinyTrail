namespace DestinyTrail.Engine {
    public static class OutputStub {

        public static void Clear() {
            Console.WriteLine("\n+\n+\n+\n+\n+\n+\n+\n+\n+\n+\n+\n+\n+\n+\n+\n+\n+\n+\n+\n+\n+\n+\n+\n+\n+\n+");
        }
        public static void Write(string message) {
            Console.WriteLine(message);
        }
    }
}