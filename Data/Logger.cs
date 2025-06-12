using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Data
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
            string line = $"{DateTime.Now:o};{id};{x:F2};{y:F2};{vx:F2};{vy:F2}";
            queue.Add(line);
        }

        public void LogWallCollision(Guid id, double x, double y, string wall)
        {
            string line = $"{DateTime.Now:o};WALL_COLLISION;{id};{x:F2};{y:F2};{wall}";
            queue.Add(line);
        }

        public void LogBallCollision(Guid id1, double x1, double y1, Guid id2, double x2, double y2)
        {
            string line = $"{DateTime.Now:o};BALL_COLLISION;{id1};{x1:F2};{y1:F2};{id2};{x2:F2};{y2:F2}";
            queue.Add(line);
        }

        private void WriteLoop()
        {
            var fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            using var writer = new StreamWriter(fileStream, System.Text.Encoding.ASCII);
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
