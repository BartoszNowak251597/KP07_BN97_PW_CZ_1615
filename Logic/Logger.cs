using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Logic
{
    public class BallLogger : IDisposable
    {
        private readonly BlockingCollection<string> queue = new();
        private readonly Task writerTask;
        private readonly CancellationTokenSource cts = new();
        private readonly string filePath;

        public BallLogger(string filePath)
        {
            this.filePath = filePath;
            writerTask = Task.Run(() => WriteLoop());
        }

        public void Log(Guid id, double x, double y, double vx, double vy)
        {
            string line = $"{DateTime.UtcNow:o};{id};{x:F2};{y:F2};{vx:F2};{vy:F2}";
            queue.Add(line);
        }

        private void WriteLoop()
        {
            var fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            using var writer = new StreamWriter(fileStream);
            foreach (var entry in queue.GetConsumingEnumerable(cts.Token))
            {
                writer.WriteLine(entry);
            }
        }

        public void Dispose()
        {
            queue.CompleteAdding();
            cts.Cancel();
            try { writerTask.Wait(); } catch { }
        }
    }
}
