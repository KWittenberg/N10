using System.Diagnostics;

namespace N10.Extensions;

public class BusyMonitor
{
    private readonly Stopwatch _sw;

    private BusyMonitor()
    {
        _sw = new Stopwatch();
    }

    public static BusyMonitor Start(ref bool busyFlag)
    {
        busyFlag = true; // Pali spinner
        var monitor = new BusyMonitor();
        monitor._sw.Start();

        return monitor;
    }

    public string Stop(ref bool busyFlag)
    {
        _sw.Stop();
        busyFlag = false; // Gasi spinner

        var ms = _sw.ElapsedMilliseconds;

        // Vraća "Duration" string
        return ms > 1000 ? $"{ms / 1000.0:F2} s" : $"{ms} ms";
    }
}