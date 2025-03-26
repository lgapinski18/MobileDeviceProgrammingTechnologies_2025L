using ProjectLayerClassLibrary;

namespace ProjectLayerClassLibraryTest
{
    [TestClass]
    public class BankAccountTest
    {
        private string OwnerName = "Jan";
        private string OwnerSurname = "Kowalski";

        [TestMethod]
        public void shouldNotBeAbleToCreateIncorrectBankAccount()
        {
            Assert.ThrowsException<ArgumentException>(() => { BankAccount bankAccount = new BankAccount("", OwnerSurname); });
            Assert.ThrowsException<ArgumentException>(() => { BankAccount bankAccount = new BankAccount(OwnerName, ""); });
        }

        [TestMethod]
        public void shouldBeAbleToCreateCorrectBankAccount()
        {
            BankAccount bankAccount = new BankAccount(OwnerName, OwnerSurname);

            Assert.AreEqual(OwnerName, bankAccount.getAccountOwnerName());
            Assert.AreEqual(OwnerSurname, bankAccount.getAccountOwnerSurname());
            Assert.AreEqual(0.0f, bankAccount.getAccountBalance(), 0.00001f);
        }

        [TestMethod]
        public void shouldNotBeAbleToSetOwnerSurnameIfOwnerNameIsEmptyString()
        {
            BankAccount bankAccount = new BankAccount(OwnerName, OwnerSurname);

            Assert.ThrowsException<ArgumentException>(() => { bankAccount.setAccountOwnerName(""); });
        }

        [TestMethod]
        [DataRow("Adam")]
        public void shouldBeAbleToSetOwnerName(string newOwnerName)
        {
            BankAccount bankAccount = new BankAccount(OwnerName, OwnerSurname);
            
            bankAccount.setAccountOwnerName(newOwnerName);

            Assert.AreEqual(newOwnerName, bankAccount.getAccountOwnerName());
        }

        [TestMethod]
        public void shouldNotBeAbleToSetOwnerSurnameIfOwnerSurnameIsEmptyString()
        {
            BankAccount bankAccount = new BankAccount(OwnerName, OwnerSurname);

            Assert.ThrowsException<ArgumentException>(() => { bankAccount.setAccountOwnerSurname(""); });
        }

        [TestMethod]
        [DataRow("Nowak")]
        public void shouldBeAbleToSetOwnerSurname(string newOwnerSurname)
        {
            BankAccount bankAccount = new BankAccount(OwnerName, OwnerSurname);

            bankAccount.setAccountOwnerSurname(newOwnerSurname);

            Assert.AreEqual(newOwnerSurname, bankAccount.getAccountOwnerSurname());
        }

        [TestMethod]
        public void shouldBeAbleToDeposit()
        {
            BankAccount bankAccount = new BankAccount(OwnerName, OwnerSurname);

            float depositSum1 = 500.0f;
            float depositSum2 = 500.0f;

            Assert.IsTrue(bankAccount.deposit(depositSum1));
            Assert.AreEqual(depositSum1, bankAccount.getAccountBalance(), 0.000001f);
            Assert.IsTrue(bankAccount.deposit(depositSum2));
            Assert.AreEqual(depositSum1 + depositSum2, bankAccount.getAccountBalance(), 0.000001f);
        }


        [TestMethod]
        public void shouldNotBeAbleToDepositIfNegativeSumDeposited()
        {
            BankAccount bankAccount = new BankAccount(OwnerName, OwnerSurname);

            float depositSum1 = -500.0f;
            float currentBalance = bankAccount.getAccountBalance();

            Assert.ThrowsException<ArgumentException>(() => bankAccount.deposit(depositSum1));
            Assert.AreEqual(currentBalance, bankAccount.getAccountBalance(), 0.000001f);
        }

        [TestMethod]
        public void shouldBeAbleToWithdraw()
        {
            BankAccount bankAccount = new BankAccount(OwnerName, OwnerSurname);

            float depositSum = 500.0f;
            float withdrawSum = 400.0f;
            bankAccount.deposit(depositSum);

            Assert.IsTrue(bankAccount.withdraw(withdrawSum));
            Assert.AreEqual(depositSum - withdrawSum, bankAccount.getAccountBalance(), 0.000001f);
        }

        [TestMethod]
        public void shouldNotBeAbleToWithdrawIfNegativeSumWithdrawed()
        {
            BankAccount bankAccount = new BankAccount(OwnerName, OwnerSurname);

            float depositSum = -500.0f;
            float currentBalance = bankAccount.getAccountBalance();

            Assert.ThrowsException<ArgumentException>(() => bankAccount.withdraw(depositSum));
            Assert.AreEqual(currentBalance, bankAccount.getAccountBalance(), 0.000001f);
        }

        [TestMethod]
        public void shouldNotBeAbleToWithdrawIfWithdrawedSumExceedsAccountBalance()
        {
            BankAccount bankAccount = new BankAccount(OwnerName, OwnerSurname);

            float depositSum = 200.0f;
            float withdrawSum = 400.0f;
            bankAccount.deposit(depositSum);

            Assert.IsFalse(bankAccount.withdraw(withdrawSum));
            Assert.AreEqual(depositSum, bankAccount.getAccountBalance(), 0.000001f);
        }
    }
}