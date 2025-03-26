using ProjectLayerClassLibrary;

namespace ProjectLayerClassLibraryTest
{
    [TestClass]
    public class BankAccountTest
    {
        private string OwnerName = "Jan";
        private string OwnerSurname = "Kowalski";

        [TestMethod]
        public void ConstructorThrowsExceptionsTest()
        {
            Assert.ThrowsException<ArgumentException>(() => { BankAccount bankAccount = new BankAccount("", OwnerSurname); });
            Assert.ThrowsException<ArgumentException>(() => { BankAccount bankAccount = new BankAccount(OwnerName, ""); });
        }

        [TestMethod]
        public void ConstructionTest()
        {
            BankAccount bankAccount = new BankAccount(OwnerName, OwnerSurname);

            Assert.AreEqual(bankAccount.getAccountOwnerName(), OwnerName);
            Assert.AreEqual(bankAccount.getAccountOwnerSurname(), OwnerSurname);
        }

        [TestMethod]
        public void SetAccountOwnerNameThrowExceptionTest()
        {
            BankAccount bankAccount = new BankAccount(OwnerName, OwnerSurname);

            Assert.ThrowsException<ArgumentException>(() => { bankAccount.setAccountOwnerName(""); });
        }

        [TestMethod]
        [DataRow("Adam")]
        public void SetAccountOwnerNameTest(string newOwnerName)
        {
            BankAccount bankAccount = new BankAccount(OwnerName, OwnerSurname);
            
            bankAccount.setAccountOwnerName(newOwnerName);

            Assert.AreEqual(bankAccount.getAccountOwnerName(), newOwnerName);
        }

        [TestMethod]
        public void SetAccountOwnerSurnameThrowExceptionTest()
        {
            BankAccount bankAccount = new BankAccount(OwnerName, OwnerSurname);

            Assert.ThrowsException<ArgumentException>(() => { bankAccount.setAccountOwnerSurname(""); });
        }

        [TestMethod]
        [DataRow("Nowak")]
        public void SetAccountOwnerSurnameTest(string newOwnerSurname)
        {
            BankAccount bankAccount = new BankAccount(OwnerName, OwnerSurname);

            bankAccount.setAccountOwnerSurname(newOwnerSurname);

            Assert.AreEqual(bankAccount.getAccountOwnerSurname(), newOwnerSurname);
        }
    }
}