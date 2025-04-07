using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassServerLibraryTest.LogicLayerTest
{
    [TestClass]
    public class MyTestClass
    {
        [TestMethod]
        void MyTest()
        {
            SharedResource resource = new SharedResource();

            // Thread waiting for condition
            Task.Run(() => resource.WaitUntilReady());
            Task.Run(() => resource.WaitUntilReady());

            // Thread setting the condition after a delay
            Task.Delay(5000).ContinueWith(_ => resource.SetReady());

            Console.ReadLine();
            Assert.IsFalse(true);
        }
    }

    internal class SharedResource
    {
        private readonly object _lock = new object();
        private bool _ready = false;

        public void WaitUntilReady()
        {
            lock (_lock)
            {
                while (!_ready)
                {
                    Console.WriteLine("[Thread] Waiting for condition...");
                    Monitor.Wait(_lock); // release lock and wait to be signaled
                }

                Console.WriteLine("[Thread] Condition met, continuing...");
            }
        }

        public void SetReady()
        {
            lock (_lock)
            {
                _ready = true;
                Monitor.PulseAll(_lock); // signal all waiting threads
                Console.WriteLine("[Main] Condition set, threads notified.");
            }
        }
    }
}
