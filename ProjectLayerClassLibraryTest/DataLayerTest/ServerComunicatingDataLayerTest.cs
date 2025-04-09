using ProjectLayerClassLibrary.DataLayer;
using ProjectLayerClassLibrary.DataLayer.Implementations;
using ProjectLayerClassLibrary.DataLayer.XmlSerializationStructures;
using ProjectLayerClassLibraryTest.DataLayerTest.TestUtilities;
using System;
using System.Collections;
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
        [DataRow(5050, 1, "Jan", "IK123456", "Kowalski", "jk@poczta.com", "12345678")]
        public async Task ShouldCreateAccountOwnerSendCorrectComunicateANdReceiveCorectResponse(int portNo, int id, string ownerName, string ownerLogin, string ownerSurname, string ownerEmail, string ownerPassword)
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
            clientSendBuffer = Encoding.ASCII.GetBytes("_CAO").Concat(BitConverter.GetBytes(0)).Concat(BitConverter.GetBytes(clientSendBuffer.Length)).Concat(clientSendBuffer).ToArray();

            AccountOwnerDto accountOwnerDto = new AccountOwnerDto();
            accountOwnerDto.Id = id;
            accountOwnerDto.Name = ownerName;
            accountOwnerDto.Login = ownerLogin;
            accountOwnerDto.Surname = ownerSurname; 
            accountOwnerDto.Email = ownerEmail;
            serializer = new XmlSerializer(typeof(AccountOwnerDto));
            StringWriter writer2 = new StringWriter();
            serializer.Serialize(writer2, accountOwnerDto);
            byte[] serverSendBuffer = Encoding.UTF8.GetBytes(writer2.ToString());
            serverSendBuffer = Encoding.ASCII.GetBytes("_CAO").Concat(BitConverter.GetBytes(0)).Concat(BitConverter.GetBytes(1)).Concat(BitConverter.GetBytes(serverSendBuffer.Length)).Concat(serverSendBuffer).ToArray();

            byte[] recievedByServerBytes = [];
            int recievedByServerCount = 0;
            Uri uri = new Uri($"ws://localhost:{portNo}");
            TestWebSocketConnection _wserver = null;
            Task server = Task.Run(async () => await TestWebSocketServer.Server(uri.Port,
                _ws =>
                {
                    _wserver = _ws; _wserver.onMessage = (bytes, count) =>
                    {
                        recievedByServerBytes = bytes;
                        recievedByServerCount = count;
                        _ws.SendAsync(serverSendBuffer);
                    };
                }));


            ADataLayer.CreationAccountOwnerDataLayerFlags creationAccountOwnerFlags = ADataLayer.CreationAccountOwnerDataLayerFlags.EMPTY;
            ADataLayer dataLayer = new ServerComunicatingDataLayer(portNo);
            Thread.Sleep(2000);
            //Task task = Task.Run(() => dataLayer.CreateAccountOwner(ownerName, ownerSurname, ownerEmail, ownerPassword, out creationAccountOwnerFlags));
            AAccountOwner accountOwner = dataLayer.CreateAccountOwner(ownerName, ownerSurname, ownerEmail, ownerPassword, out creationAccountOwnerFlags);
            //CollectionAssert.AreEqual(clientSendBuffer, (new ArraySegment<byte>(recievedByServerBytes, 0, recievedByServerCount)).ToArray());
            CollectionAssert.AreEqual(clientSendBuffer, recievedByServerBytes.Skip(0).Take(recievedByServerCount).ToArray());
            Assert.IsNotNull(accountOwner);
            Assert.AreEqual(id, accountOwner.GetId());
            Assert.AreEqual(ownerLogin, accountOwner.OwnerLogin);
            Assert.AreEqual(ownerName, accountOwner.OwnerName);
            Assert.AreEqual(ownerSurname, accountOwner.OwnerSurname);
            Assert.AreEqual(ownerEmail, accountOwner.OwnerEmail);
        }
    }
}
