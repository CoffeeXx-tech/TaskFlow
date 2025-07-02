using System;
using System.Threading;

public class EscListener
{
    private Thread? listenerThread;

    public void StartListening()
    {
        listenerThread = new Thread(() =>
        {
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Escape)
                    {
                        Environment.Exit(0);
                    }
                }
                Thread.Sleep(50);
            }
        });
        listenerThread.IsBackground = true;
        listenerThread.Start();
    }
}
