using ProjectLayerClassServerLibrary.DataLayer;
using ProjectLayerClassServerLibrary.DataLayer.Implementations;
using ProjectLayerClassServerLibrary.LogicLayer;
using ProjectLayerClassServerLibrary.LogicLayer.Exceptions;
using ProjectLayerClassServerLibrary.LogicLayer.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static ProjectLayerClassServerLibrary.LogicLayer.ALogicLayer;

namespace ProjectLayerClassServerLibraryTest.LogicLayerTest
{
    [TestClass]
    public class My2Test
    {
        [TestMethod]
        [DataRow("Jan", "Kowalski", "jk@poczta.com", "12345678")]
        public void shouldCreateCorrectAccount(string name, string surname, string email, string password)
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
}
