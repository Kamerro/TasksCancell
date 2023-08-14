using static System.Threading.Thread;
internal class Program
{
    private static void Main(string[] args)
    {
        var token = new CancellationTokenSource();
        Task task = new Task(() => {
            while (true)
            {
            token.Token.ThrowIfCancellationRequested();

                Console.WriteLine("OK \t");
            }
        }, token.Token);

        Task.Factory.StartNew(() =>
        {
            token.Token.WaitHandle.WaitOne();
            Console.WriteLine("Stopped");

        });

        task.Start();
        Console.WriteLine("Task sucessfully started");
        Console.ReadKey();
        token.Cancel();
        
    }
}