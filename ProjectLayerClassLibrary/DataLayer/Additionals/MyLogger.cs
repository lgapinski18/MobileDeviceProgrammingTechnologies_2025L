using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.DataLayer.Additionals
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;

    internal class MyLogger : IDisposable
    {
        private readonly string _filePath;
        private readonly Queue<string> _logQueue = new Queue<string>();
        private readonly object _lock = new object();
        private readonly AutoResetEvent _logEvent = new AutoResetEvent(false);
        private readonly Thread _logWorker;
        private bool _isRunning = true;

        public MyLogger(string filePath)
        {
            _filePath = filePath;
            _logWorker = new Thread(ProcessQueue)
            {
                IsBackground = true
            };
            _logWorker.Start();
        }

        public void Log(string message)
        {
            lock (_lock)
            {
                _logQueue.Enqueue($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - {message}");
            }
            _logEvent.Set(); // Signal worker to process log
        }

        private void ProcessQueue()
        {
            while (_isRunning)
            {
                _logEvent.WaitOne();

                while (true)
                {
                    string? logEntry = null;
                    lock (_lock)
                    {
                        if (_logQueue.Count > 0)
                        {
                            logEntry = _logQueue.Dequeue();
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (logEntry != null)
                    {
                        try
                        {
                            File.AppendAllText(_filePath, logEntry + Environment.NewLine);
                        }
                        catch (Exception ex)
                        {
                            // Optionally log to a fallback or ignore
                            Console.Error.WriteLine($"Logging failed: {ex.Message}");
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            _isRunning = false;
            _logEvent.Set();  // Wake up thread to exit
            _logWorker.Join(); // Wait for thread to finish
            _logEvent.Dispose();
        }
    }
}
