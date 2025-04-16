using ProjectLayerClassLibrary.DataLayer;
using ProjectLayerClassLibrary.DataLayer.Implementations;
using ProjectLayerClassLibrary.DataLayer.XmlSerializationStructures;
using ProjectLayerClassLibrary.PresentationLayer.ViewLayer;
using ProjectLayerClassLibraryTest.DataLayerTest.TestUtilities;
using System.Text;
using System.Xml.Serialization;

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
            clientSendBuffer = BitConverter.GetBytes((int)ServerComunicatingDataLayer.ComunicationCodeFromClient.CREATE_ACCOUNT_OWNER_CODE).Concat(BitConverter.GetBytes(0)).Concat(BitConverter.GetBytes(clientSendBuffer.Length)).Concat(clientSendBuffer).ToArray();

            ProjectLayerClassLibrary.DataLayer.XmlSerializationStructures.CreationAccountOwnerDataLayerFlags creationAccountOwnerFlagsServer = ProjectLayerClassLibrary.DataLayer.XmlSerializationStructures.CreationAccountOwnerDataLayerFlags.SUCCESS;
            AccountOwnerDto accountOwnerDto = new AccountOwnerDto();
            accountOwnerDto.Id = id;
            accountOwnerDto.Name = ownerName;
            accountOwnerDto.Login = ownerLogin;
            accountOwnerDto.Surname = ownerSurname; 
            accountOwnerDto.Email = ownerEmail;
            CreationAccountOwnerResponse creationAccountOwnerResponse = new CreationAccountOwnerResponse();
            creationAccountOwnerResponse.AccountOwner = accountOwnerDto;
            creationAccountOwnerResponse.CreationFlags = creationAccountOwnerFlagsServer;
            serializer = new XmlSerializer(typeof(CreationAccountOwnerResponse));
            StringWriter writer2 = new StringWriter();
            serializer.Serialize(writer2, creationAccountOwnerResponse);
            byte[] serverSendBuffer = Encoding.UTF8.GetBytes(writer2.ToString());
            serverSendBuffer = BitConverter.GetBytes((int)ServerComunicatingDataLayer.ComunicationCodeFromServer.CREATE_ACCOUNT_OWNER_CODE).Concat(BitConverter.GetBytes(0)).Concat(BitConverter.GetBytes(1)).Concat(BitConverter.GetBytes(serverSendBuffer.Length)).Concat(serverSendBuffer).ToArray();

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

            await _wserver?.DisconnectAsync();
        }

        [TestMethod]
        [DataRow(5051, 1, 1, "18273645", 100.0f)]
        public async Task ShouldCreateBankAccountSendCorrectComunicateANdReceiveCorectResponse(int portNo, int ownerId, int accountId, string accountNumber, float balance)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(int));
            StringWriter writer = new StringWriter();
            serializer.Serialize(writer, ownerId);
            byte[] clientSendBuffer = Encoding.UTF8.GetBytes(writer.ToString());
            byte[] header = BitConverter.GetBytes((int)ServerComunicatingDataLayer.ComunicationCodeFromClient.CREATE_BANK_ACCOUNT_CODE).Concat(BitConverter.GetBytes(0)).Concat(BitConverter.GetBytes(clientSendBuffer.Length)).ToArray();
            clientSendBuffer = header.Concat(clientSendBuffer).ToArray();

            BankAccountDto bankAccountDto = new BankAccountDto();
            bankAccountDto.Id = accountId;
            bankAccountDto.AccountNumber = accountNumber;
            bankAccountDto.Balance = balance;
            bankAccountDto.OwnerId = ownerId;
            serializer = new XmlSerializer(typeof(BankAccountDto));
            StringWriter writer2 = new StringWriter();
            serializer.Serialize(writer2, bankAccountDto);
            byte[] serverSendBuffer = Encoding.UTF8.GetBytes(writer2.ToString());
            serverSendBuffer = BitConverter.GetBytes((int)ServerComunicatingDataLayer.ComunicationCodeFromServer.CREATE_BANK_ACCOUNT_CODE).Concat(BitConverter.GetBytes(0)).Concat(BitConverter.GetBytes(1)).Concat(BitConverter.GetBytes(serverSendBuffer.Length)).Concat(serverSendBuffer).ToArray();

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
            ABankAccount bankAccount = dataLayer.CreateBankAccount(ownerId);
            //CollectionAssert.AreEqual(clientSendBuffer, (new ArraySegment<byte>(recievedByServerBytes, 0, recievedByServerCount)).ToArray());
            CollectionAssert.AreEqual(clientSendBuffer, recievedByServerBytes.Skip(0).Take(recievedByServerCount).ToArray());
            Assert.IsNotNull(bankAccount);
            Assert.AreEqual(accountId, bankAccount.GetId());
            Assert.AreEqual(ownerId, bankAccount.AccountOwnerId);
            Assert.AreEqual(accountNumber, bankAccount.AccountNumber);
            Assert.AreEqual(balance, bankAccount.AccountBalance);

            await _wserver?.DisconnectAsync();
        }

        [TestMethod]
        [DataRow(5052, 1, "Jan", "IK123456", "Kowalski", "jk@poczta.com")]
        public async Task ShouldGetAccountOwnerSendCorrectComunicateANdReceiveCorectResponse(int portNo, int ownerId, string ownerName, string ownerLogin, string ownerSurname, string ownerEmail)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(int));
            StringWriter writer = new StringWriter();
            serializer.Serialize(writer, ownerId);
            byte[] clientSendBuffer = Encoding.UTF8.GetBytes(writer.ToString());
            clientSendBuffer = BitConverter.GetBytes((int)ServerComunicatingDataLayer.ComunicationCodeFromClient.GET_ACCOUNT_OWNER_CODE).Concat(BitConverter.GetBytes(0)).Concat(BitConverter.GetBytes(clientSendBuffer.Length)).Concat(clientSendBuffer).ToArray();

            AccountOwnerDto accountOwnerDto = new AccountOwnerDto();
            accountOwnerDto.Id = ownerId;
            accountOwnerDto.Name = ownerName;
            accountOwnerDto.Login = ownerLogin;
            accountOwnerDto.Surname = ownerSurname;
            accountOwnerDto.Email = ownerEmail;
            serializer = new XmlSerializer(typeof(AccountOwnerDto));
            StringWriter writer2 = new StringWriter();
            serializer.Serialize(writer2, accountOwnerDto);
            byte[] serverSendBuffer = Encoding.UTF8.GetBytes(writer2.ToString());
            serverSendBuffer = BitConverter.GetBytes((int)ServerComunicatingDataLayer.ComunicationCodeFromServer.GET_ACCOUNT_OWNER_CODE).Concat(BitConverter.GetBytes(0)).Concat(BitConverter.GetBytes(1)).Concat(BitConverter.GetBytes(serverSendBuffer.Length)).Concat(serverSendBuffer).ToArray();

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
            AAccountOwner? accountOwner = dataLayer.GetAccountOwner(ownerId);
            //CollectionAssert.AreEqual(clientSendBuffer, (new ArraySegment<byte>(recievedByServerBytes, 0, recievedByServerCount)).ToArray());
            CollectionAssert.AreEqual(clientSendBuffer, recievedByServerBytes.Skip(0).Take(recievedByServerCount).ToArray());
            Assert.IsNotNull(accountOwner);
            Assert.AreEqual(ownerId, accountOwner.GetId());
            Assert.AreEqual(ownerLogin, accountOwner.OwnerLogin);
            Assert.AreEqual(ownerName, accountOwner.OwnerName);
            Assert.AreEqual(ownerSurname, accountOwner.OwnerSurname);
            Assert.AreEqual(ownerEmail, accountOwner.OwnerEmail);

            await _wserver?.DisconnectAsync();
        }

        [TestMethod]
        [DataRow(5053, 1, "Jan", "IK123456", "Kowalski", "jk@poczta.com")]
        public async Task ShouldGetAccountOwnerByLoginSendCorrectComunicateANdReceiveCorectResponse(int portNo, int ownerId, string ownerName, string ownerLogin, string ownerSurname, string ownerEmail)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(string));
            StringWriter writer = new StringWriter();
            serializer.Serialize(writer, ownerLogin);
            byte[] clientSendBuffer = Encoding.UTF8.GetBytes(writer.ToString());
            clientSendBuffer = BitConverter.GetBytes((int)ServerComunicatingDataLayer.ComunicationCodeFromClient.GET_ACCOUNT_OWNER_LOGIN_CODE).Concat(BitConverter.GetBytes(0)).Concat(BitConverter.GetBytes(clientSendBuffer.Length)).Concat(clientSendBuffer).ToArray();

            AccountOwnerDto accountOwnerDto = new AccountOwnerDto();
            accountOwnerDto.Id = ownerId;
            accountOwnerDto.Name = ownerName;
            accountOwnerDto.Login = ownerLogin;
            accountOwnerDto.Surname = ownerSurname;
            accountOwnerDto.Email = ownerEmail;
            serializer = new XmlSerializer(typeof(AccountOwnerDto));
            StringWriter writer2 = new StringWriter();
            serializer.Serialize(writer2, accountOwnerDto);
            byte[] serverSendBuffer = Encoding.UTF8.GetBytes(writer2.ToString());
            serverSendBuffer = BitConverter.GetBytes((int)ServerComunicatingDataLayer.ComunicationCodeFromServer.GET_ACCOUNT_OWNER_LOGIN_CODE).Concat(BitConverter.GetBytes(0)).Concat(BitConverter.GetBytes(1)).Concat(BitConverter.GetBytes(serverSendBuffer.Length)).Concat(serverSendBuffer).ToArray();

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
            AAccountOwner? accountOwner = dataLayer.GetAccountOwner(ownerLogin);
            //CollectionAssert.AreEqual(clientSendBuffer, (new ArraySegment<byte>(recievedByServerBytes, 0, recievedByServerCount)).ToArray());
            CollectionAssert.AreEqual(clientSendBuffer, recievedByServerBytes.Skip(0).Take(recievedByServerCount).ToArray());
            Assert.IsNotNull(accountOwner);
            Assert.AreEqual(ownerId, accountOwner.GetId());
            Assert.AreEqual(ownerLogin, accountOwner.OwnerLogin);
            Assert.AreEqual(ownerName, accountOwner.OwnerName);
            Assert.AreEqual(ownerSurname, accountOwner.OwnerSurname);
            Assert.AreEqual(ownerEmail, accountOwner.OwnerEmail);

            await _wserver?.DisconnectAsync();
        }

        [TestMethod]
        [DataRow(5054)]
        public async Task ShouldGetAllAccountOwnersSendCorrectComunicateANdReceiveCorectResponse(int portNo)
        {
            byte[] clientSendBuffer = [];
            clientSendBuffer = BitConverter.GetBytes((int)ServerComunicatingDataLayer.ComunicationCodeFromClient.GET_ALL_ACCOUNT_OWNERS_CODE).Concat(BitConverter.GetBytes(0)).Concat(BitConverter.GetBytes(clientSendBuffer.Length)).Concat(clientSendBuffer).ToArray();

            List<AccountOwnerDto> accountOwnerDtos = new List<AccountOwnerDto>();
            XmlSerializer serializer = new XmlSerializer(typeof(List<AccountOwnerDto>));
            StringWriter writer2 = new StringWriter();
            serializer.Serialize(writer2, accountOwnerDtos);
            byte[] serverSendBuffer = Encoding.UTF8.GetBytes(writer2.ToString());
            serverSendBuffer = BitConverter.GetBytes((int)ServerComunicatingDataLayer.ComunicationCodeFromServer.GET_ALL_ACCOUNT_OWNERS_CODE).Concat(BitConverter.GetBytes(0)).Concat(BitConverter.GetBytes(1)).Concat(BitConverter.GetBytes(serverSendBuffer.Length)).Concat(serverSendBuffer).ToArray();

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
            ICollection<AAccountOwner> accountOwners = dataLayer.GetAllAccountOwners();
            //CollectionAssert.AreEqual(clientSendBuffer, (new ArraySegment<byte>(recievedByServerBytes, 0, recievedByServerCount)).ToArray());
            CollectionAssert.AreEqual(clientSendBuffer, recievedByServerBytes.Skip(0).Take(recievedByServerCount).ToArray());
            Assert.IsNotNull(accountOwners);
            Assert.AreEqual(0, accountOwners.Count);

            await _wserver?.DisconnectAsync();
        }

        [TestMethod]
        [DataRow(5055)]
        public async Task ShouldGetAllBankAccountsSendCorrectComunicateANdReceiveCorectResponse(int portNo)
        {
            byte[] clientSendBuffer = [];
            clientSendBuffer = BitConverter.GetBytes((int)ServerComunicatingDataLayer.ComunicationCodeFromClient.GET_ALL_BANK_ACCOUNTS_CODE).Concat(BitConverter.GetBytes(0)).Concat(BitConverter.GetBytes(clientSendBuffer.Length)).Concat(clientSendBuffer).ToArray();

            List<BankAccountDto> bankAccountDtos = new List<BankAccountDto>();
            XmlSerializer serializer = new XmlSerializer(typeof(List<BankAccountDto>));
            StringWriter writer2 = new StringWriter();
            serializer.Serialize(writer2, bankAccountDtos);
            byte[] serverSendBuffer = Encoding.UTF8.GetBytes(writer2.ToString());
            serverSendBuffer = BitConverter.GetBytes((int)ServerComunicatingDataLayer.ComunicationCodeFromServer.GET_ALL_BANK_ACCOUNTS_CODE).Concat(BitConverter.GetBytes(0)).Concat(BitConverter.GetBytes(1)).Concat(BitConverter.GetBytes(serverSendBuffer.Length)).Concat(serverSendBuffer).ToArray();

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
            ICollection<ABankAccount> bankAccounts = dataLayer.GetAllBankAccounts();
            //CollectionAssert.AreEqual(clientSendBuffer, (new ArraySegment<byte>(recievedByServerBytes, 0, recievedByServerCount)).ToArray());
            CollectionAssert.AreEqual(clientSendBuffer, recievedByServerBytes.Skip(0).Take(recievedByServerCount).ToArray());
            Assert.IsNotNull(bankAccounts);
            Assert.AreEqual(0, bankAccounts.Count);

            await _wserver?.DisconnectAsync();
        }

        [TestMethod]
        [DataRow(5056, 1, 1, "18273645", 100.0f)]
        public async Task ShouldGetBankAccountSendCorrectComunicateANdReceiveCorectResponse(int portNo, int ownerId, int accountId, string accountNumber, float balance)
        {
            XmlSerializer serializer;
            serializer = new XmlSerializer(typeof(string));
            StringWriter writer = new StringWriter();
            serializer.Serialize(writer, accountNumber);
            byte[] clientSendBuffer = Encoding.UTF8.GetBytes(writer.ToString());
            clientSendBuffer = BitConverter.GetBytes((int)ServerComunicatingDataLayer.ComunicationCodeFromClient.GET_BANK_ACCOUNT_CODE).Concat(BitConverter.GetBytes(0)).Concat(BitConverter.GetBytes(clientSendBuffer.Length)).Concat(clientSendBuffer).ToArray();

            BankAccountDto bankAccountDto = new BankAccountDto();
            bankAccountDto.Id = accountId;
            bankAccountDto.AccountNumber = accountNumber;
            bankAccountDto.Balance = balance;
            bankAccountDto.OwnerId = ownerId;
            serializer = new XmlSerializer(typeof(BankAccountDto));
            StringWriter writer2 = new StringWriter();
            serializer.Serialize(writer2, bankAccountDto);
            byte[] serverSendBuffer = Encoding.UTF8.GetBytes(writer2.ToString());
            serverSendBuffer = BitConverter.GetBytes((int)ServerComunicatingDataLayer.ComunicationCodeFromServer.GET_BANK_ACCOUNT_CODE).Concat(BitConverter.GetBytes(0)).Concat(BitConverter.GetBytes(1)).Concat(BitConverter.GetBytes(serverSendBuffer.Length)).Concat(serverSendBuffer).ToArray();

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
            ABankAccount bankAccount = dataLayer.GetBankAccount(accountNumber);
            //CollectionAssert.AreEqual(clientSendBuffer, (new ArraySegment<byte>(recievedByServerBytes, 0, recievedByServerCount)).ToArray());
            CollectionAssert.AreEqual(clientSendBuffer, recievedByServerBytes.Skip(0).Take(recievedByServerCount).ToArray());
            Assert.AreEqual(accountId, bankAccount.GetId());
            Assert.AreEqual(ownerId, bankAccount.AccountOwnerId);
            Assert.AreEqual(accountNumber, bankAccount.AccountNumber);
            Assert.AreEqual(balance, bankAccount.AccountBalance);

            await _wserver?.DisconnectAsync();
        }

        [TestMethod]
        [DataRow(5057, 1)]
        public async Task ShouldGetBankAccountsSendCorrectComunicateANdReceiveCorectResponse(int portNo, int ownerId)
        {
            XmlSerializer serializer;
            serializer = new XmlSerializer(typeof(int));
            StringWriter writer = new StringWriter();
            serializer.Serialize(writer, ownerId);
            byte[] clientSendBuffer = Encoding.UTF8.GetBytes(writer.ToString());
            clientSendBuffer = BitConverter.GetBytes((int)ServerComunicatingDataLayer.ComunicationCodeFromClient.GET_BANK_ACCOUNTS_CODE).Concat(BitConverter.GetBytes(0)).Concat(BitConverter.GetBytes(clientSendBuffer.Length)).Concat(clientSendBuffer).ToArray();

            List<BankAccountDto> bankAccountDtos = new List<BankAccountDto>();
            serializer = new XmlSerializer(typeof(List<BankAccountDto>));
            StringWriter writer2 = new StringWriter();
            serializer.Serialize(writer2, bankAccountDtos);
            byte[] serverSendBuffer = Encoding.UTF8.GetBytes(writer2.ToString());
            serverSendBuffer = BitConverter.GetBytes((int)ServerComunicatingDataLayer.ComunicationCodeFromServer.GET_BANK_ACCOUNTS_CODE).Concat(BitConverter.GetBytes(0)).Concat(BitConverter.GetBytes(1)).Concat(BitConverter.GetBytes(serverSendBuffer.Length)).Concat(serverSendBuffer).ToArray();

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
            ICollection<ABankAccount> bankAccounts = dataLayer.GetBankAccounts(ownerId);
            //CollectionAssert.AreEqual(clientSendBuffer, (new ArraySegment<byte>(recievedByServerBytes, 0, recievedByServerCount)).ToArray());
            CollectionAssert.AreEqual(clientSendBuffer, recievedByServerBytes.Skip(0).Take(recievedByServerCount).ToArray());
            Assert.IsNotNull(bankAccounts);
            Assert.AreEqual(0, bankAccounts.Count);

            await _wserver?.DisconnectAsync();
        }

        [TestMethod]
        [DataRow(5058, "IK123456", "12345678")]
        public async Task ShouldAuthenticateAccountOwnerSendCorrectComunicateANdReceiveCorectResponse(int portNo, string login, string password)
        {
            Credentials credentials = new Credentials() { Login = login, Password = password };
            XmlSerializer serializer = new XmlSerializer(typeof(Credentials));
            StringWriter writer = new StringWriter();
            serializer.Serialize(writer, credentials);
            byte[] clientSendBuffer = Encoding.UTF8.GetBytes(writer.ToString());
            clientSendBuffer = BitConverter.GetBytes((int)ServerComunicatingDataLayer.ComunicationCodeFromClient.AUTHENTICATE_ACCOUNT_OWNER_CODE).Concat(BitConverter.GetBytes(0)).Concat(BitConverter.GetBytes(clientSendBuffer.Length)).Concat(clientSendBuffer).ToArray();

            serializer = new XmlSerializer(typeof(bool));
            StringWriter writer2 = new StringWriter();
            serializer.Serialize(writer2, true);
            byte[] serverSendBuffer = Encoding.UTF8.GetBytes(writer2.ToString());
            serverSendBuffer = BitConverter.GetBytes((int)ServerComunicatingDataLayer.ComunicationCodeFromServer.AUTHENTICATE_ACCOUNT_OWNER_CODE).Concat(BitConverter.GetBytes(0)).Concat(BitConverter.GetBytes(1)).Concat(BitConverter.GetBytes(serverSendBuffer.Length)).Concat(serverSendBuffer).ToArray();

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
            bool b = dataLayer.AuthenticateAccountOwner(login, password);
            //CollectionAssert.AreEqual(clientSendBuffer, (new ArraySegment<byte>(recievedByServerBytes, 0, recievedByServerCount)).ToArray());
            CollectionAssert.AreEqual(clientSendBuffer, recievedByServerBytes.Skip(0).Take(recievedByServerCount).ToArray());
            Assert.IsTrue(b);

            await _wserver?.DisconnectAsync();
        }

        [TestMethod]
        [DataRow(5059, "87654321", "12345678", 1000.0f, "asdasfasfasf")]
        public async Task ShouldPerformTransferOwnerSendCorrectComunicate(int portNo, string ownerAccountNumber, string targetAccountNumber, float amount, string description)
        {
            TransferData transferData = new TransferData()
            {
                SourceAccountNumber = ownerAccountNumber,
                TargetAccountNumber = targetAccountNumber,
                Amount = amount,
                Description = description
            };
            XmlSerializer serializer = new XmlSerializer(typeof(TransferData));
            StringWriter writer = new StringWriter();
            serializer.Serialize(writer, transferData);
            byte[] clientSendBuffer = Encoding.UTF8.GetBytes(writer.ToString());
            clientSendBuffer = BitConverter.GetBytes((int)ServerComunicatingDataLayer.ComunicationCodeFromClient.PERFORM_TRANSFER_CODE).Concat(BitConverter.GetBytes(0)).Concat(BitConverter.GetBytes(clientSendBuffer.Length)).Concat(clientSendBuffer).ToArray();

            serializer = new XmlSerializer(typeof(ProjectLayerClassLibrary.DataLayer.XmlSerializationStructures.TransferResultCodes));
            StringWriter writer2 = new StringWriter();
            serializer.Serialize(writer2, ProjectLayerClassLibrary.DataLayer.XmlSerializationStructures.TransferResultCodes.SUCCESS);
            byte[] serverSendBuffer = Encoding.UTF8.GetBytes(writer2.ToString());
            serverSendBuffer = BitConverter.GetBytes((int)ServerComunicatingDataLayer.ComunicationCodeFromServer.PERFORM_TRANSFER_CODE).Concat(BitConverter.GetBytes(0)).Concat(BitConverter.GetBytes(1)).Concat(BitConverter.GetBytes(serverSendBuffer.Length)).Concat(serverSendBuffer).ToArray();

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
            dataLayer.PerformTransfer(ownerAccountNumber, targetAccountNumber, amount, description, (tc, oAN, tAN, a, d) => {
                Assert.AreEqual(ADataLayer.TransferResultCodes.SUCCESS, tc);
            });
            //CollectionAssert.AreEqual(clientSendBuffer, (new ArraySegment<byte>(recievedByServerBytes, 0, recievedByServerCount)).ToArray());
            CollectionAssert.AreEqual(clientSendBuffer, recievedByServerBytes.Skip(0).Take(recievedByServerCount).ToArray());

            await _wserver?.DisconnectAsync();
        }

        [TestMethod]
        [DataRow(5060, 1)]
        public async Task ShouldCheckForReportsUpdatesSendCorrectComunicateeANdReceiveCorectResponse(int portNo, int ownerId)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(int));
            StringWriter writer = new StringWriter();
            serializer.Serialize(writer, ownerId);
            byte[] clientSendBuffer = Encoding.UTF8.GetBytes(writer.ToString());
            clientSendBuffer = BitConverter.GetBytes((int)ServerComunicatingDataLayer.ComunicationCodeFromClient.CHECK_FOR_BANK_ACCOUNT_REPORTS_UPDATE_CODE).Concat(BitConverter.GetBytes(0)).Concat(BitConverter.GetBytes(clientSendBuffer.Length)).Concat(clientSendBuffer).ToArray();

            serializer = new XmlSerializer(typeof(bool));
            StringWriter writer2 = new StringWriter();
            serializer.Serialize(writer2, true);
            byte[] serverSendBuffer = Encoding.UTF8.GetBytes(writer2.ToString());
            serverSendBuffer = BitConverter.GetBytes((int)ServerComunicatingDataLayer.ComunicationCodeFromServer.CHECK_FOR_BANK_ACCOUNT_REPORTS_UPDATE_CODE).Concat(BitConverter.GetBytes(0)).Concat(BitConverter.GetBytes(1)).Concat(BitConverter.GetBytes(serverSendBuffer.Length)).Concat(serverSendBuffer).ToArray();

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
            Assert.IsTrue(dataLayer.CheckForReportsUpdates(ownerId));
            //CollectionAssert.AreEqual(clientSendBuffer, (new ArraySegment<byte>(recievedByServerBytes, 0, recievedByServerCount)).ToArray());
            CollectionAssert.AreEqual(clientSendBuffer, recievedByServerBytes.Skip(0).Take(recievedByServerCount).ToArray());

            await _wserver?.DisconnectAsync();
        }
    }
}
