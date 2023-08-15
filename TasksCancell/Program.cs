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
                SpinWait(10000000);
            }
        }, token.Token);

        //Paranoid token injected:

        Task task_better = new Task(() => {
            while (true)
            {
                paranoid.Token.ThrowIfCancellationRequested();
                Console.WriteLine("OK \t");
                SpinWait(10000000);
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

        //After click any of keys the token will be cancelled:
        Console.ReadKey();
        token.Cancel();


        //Wait all additional code with if conditional loop:
        if(task.Status is not TaskStatus.Canceled && task_better.Status is not TaskStatus.Canceled)
            Task.WaitAll(new[] { task, task_better }, 5000,token.Token);

        //Or the other way with handling exceptions:
        try
        {
            Task.WaitAll(new[] { task, task_better }, 5000, token.Token);

        }
        catch (AggregateException ex)
        {
            foreach (var exception in ex.InnerExceptions)
            {
                Console.WriteLine(exception.Source + "" + exception.Message);
            }
        }



    }

}