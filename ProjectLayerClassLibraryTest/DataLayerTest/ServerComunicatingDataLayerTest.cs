using ProjectLayerClassLibrary.DataLayer;
using ProjectLayerClassLibrary.DataLayer.Implementations;
using ProjectLayerClassLibrary.DataLayer.XmlSerializationStructures;
using ProjectLayerClassLibraryTest.DataLayerTest.TestUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static ProjectLayerClassLibrary.DataLayer.ADataLayer;

namespace ProjectLayerClassLibraryTest.DataLayerTest
{
    [TestClass]
    public class ServerComunicatingDataLayerTest
    {
        [TestMethod]
        [DataRow(5050, "Jan", "Kowalski", "jk@poczta.com", "12345678")]
        public async Task ShouldCreateAccountOwnerSendCorrectComunicate(int portNo, string ownerName, string ownerSurname, string ownerEmail, string ownerPassword)
        {
            AccountOwnerCreationData accountOwnerCreationData = new AccountOwnerCreationData();
            accountOwnerCreationData.Name = ownerName;
            accountOwnerCreationData.Surname = ownerSurname;
            accountOwnerCreationData.Email = ownerEmail;
            accountOwnerCreationData.Password = ownerPassword;
            XmlSerializer serializer = new XmlSerializer(typeof(AccountOwnerCreationData));
            StringWriter writer = new StringWriter();
            serializer.Serialize(writer, accountOwnerCreationData);
            byte[] clientSendBuffer = Encoding.UTF8.GetBytes(writer.ToString());
            clientSendBuffer = Encoding.ASCII.GetBytes("_CAO").Concat(BitConverter.GetBytes(1)).Concat(BitConverter.GetBytes(clientSendBuffer.Length)).Concat(clientSendBuffer).ToArray();

            Uri uri = new Uri($"ws://localhost:{portNo}");
            TestWebSocketConnection _wserver = null;
            Task server = Task.Run(async () => await TestWebSocketServer.Server(uri.Port,
                _ws =>
                {
                    _wserver = _ws; _wserver.onMessage = (bytes, count) =>
                    {
                        CollectionAssert.AreEqual(sendBuffer, bytes);
                        _ws.
                    };
                }));


            ADataLayer.CreationAccountOwnerDataLayerFlags creationAccountOwnerFlags = ADataLayer.CreationAccountOwnerDataLayerFlags.EMPTY;
            ADataLayer dataLayer = new ServerComunicatingDataLayer(portNo);
            Thread.Sleep(2000);
            //Task task = Task.Run(() => dataLayer.CreateAccountOwner(ownerName, ownerSurname, ownerEmail, ownerPassword, out creationAccountOwnerFlags));
            AAccountOwner accountOwner = dataLayer.CreateAccountOwner(ownerName, ownerSurname, ownerEmail, ownerPassword, out creationAccountOwnerFlags);
            Assert.IsNotNull(accountOwner);
            Assert.AreEqual(ownerName, accountOwner.OwnerName);
            Assert.AreEqual(ownerSurname, accountOwner.OwnerSurname);
            Assert.AreEqual(ownerEmail, accountOwner.OwnerEmail);


        }

        [TestMethod]
        [DataRow(5050, "Jan", "Kowalski", "jk@poczta.com", "12345678")]
        public async Task ShouldCreateAccountOwnerReceiveCorrectly(int portNo, string ownerName, string ownerSurname, string ownerEmail, string ownerPassword)
        {
            AccountOwnerCreationData accountOwnerCreationData = new AccountOwnerCreationData();
            accountOwnerCreationData.Name = ownerName;
            accountOwnerCreationData.Surname = ownerSurname;
            accountOwnerCreationData.Email = ownerEmail;
            accountOwnerCreationData.Password = ownerPassword;
            XmlSerializer serializer = new XmlSerializer(typeof(AccountOwnerCreationData));
            StringWriter writer = new StringWriter();
            serializer.Serialize(writer, accountOwnerCreationData);
            byte[] sendBuffer = Encoding.UTF8.GetBytes(writer.ToString());
            byte[] header = Encoding.ASCII.GetBytes("_CAO").Concat(BitConverter.GetBytes(1)).Concat(BitConverter.GetBytes(sendBuffer.Length)).ToArray();
            sendBuffer = header.Concat(sendBuffer).ToArray();

            Uri uri = new Uri($"ws://localhost:{portNo}");
            TestWebSocketConnection _wserver = null;
            Task server = Task.Run(async () => await TestWebSocketServer.Server(uri.Port,
                _ws =>
                {
                    _wserver = _ws; _wserver.onMessage = (bytes, count) =>
                    {
                        CollectionAssert.AreEqual(sendBuffer, bytes);
                    };
                }));


            ADataLayer.CreationAccountOwnerDataLayerFlags creationAccountOwnerFlags = ADataLayer.CreationAccountOwnerDataLayerFlags.EMPTY;
            ADataLayer dataLayer = new ServerComunicatingDataLayer(portNo);
            Thread.Sleep(2000);
            AAccountOwner accountOwner = dataLayer.CreateAccountOwner(ownerName, ownerSurname, ownerEmail, ownerPassword, out creationAccountOwnerFlags);
            Assert.IsNotNull(accountOwner);
            Assert.AreEqual(ownerName, accountOwner.OwnerName);
            Assert.AreEqual(ownerSurname, accountOwner.OwnerSurname);
            Assert.AreEqual(ownerEmail, accountOwner.OwnerEmail);

        }
    }
}
