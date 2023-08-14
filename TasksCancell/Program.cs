using static System.Threading.Thread;
internal class Program
{
    private static void Main(string[] args)
    {
        var failure = new CancellationTokenSource();
        var token = new CancellationTokenSource();
        var paranoid = CancellationTokenSource.CreateLinkedTokenSource(failure.Token, token.Token);
        //Normal token injected:
        Task task = new Task(() => {
            while (true)
            {
                token.Token.ThrowIfCancellationRequested();
                Console.WriteLine("OK \t");
            }
        }, token.Token);

        //Paranoid token injected:

        Task task_better = new Task(() => {
            while (true)
            {
                token.Token.ThrowIfCancellationRequested();
                Console.WriteLine("OK \t");
            }
        }, paranoid.Token);

        //Task that waits for normal token cancel:
        Task.Factory.StartNew(() =>
        {
            token.Token.WaitHandle.WaitOne();
            Console.WriteLine("Stopped by normal token");

        });

        //Task waits for any of the tokens cancel ("Paranoid"):

        Task.Factory.StartNew(() =>
        {
            paranoid.Token.WaitHandle.WaitOne();
            Console.WriteLine("Stopped by any of the cancelation tokens");

        });
        task.Start();
        Console.WriteLine("Task sucessfully started");
        Console.ReadKey();
        token.Cancel();
        
    }
}