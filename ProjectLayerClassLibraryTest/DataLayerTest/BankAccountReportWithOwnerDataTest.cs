using ProjectLayerClassLibrary.DataLayer;
using ProjectLayerClassLibrary.DataLayer.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ProjectLayerClassLibraryTest.DataLayerTest
{
    [TestClass]
    public class BankAccountReportWithOwnerDataTest
    {
        [TestMethod]
        [DataRow(0.0f, 100.0f, "Adam", "Nowak", "an@poczta.com")]
        public void shouldCreateCorrect(float previousAccountBalance, float currentAccountBalance, string ownerName, string ownerSurname, string ownerEmail)
        {
            DateTime prev = DateTime.UtcNow;
            ABankAccountReport bankAccountReport = new BankAccountReportWithOwnerData(previousAccountBalance, currentAccountBalance, ownerName, ownerSurname, ownerEmail);
            DateTime after = DateTime.UtcNow;

            Assert.IsTrue(bankAccountReport.TimeOfReportCreation > prev);
            Assert.IsTrue(bankAccountReport.TimeOfReportCreation < after);
            Assert.AreEqual(previousAccountBalance, bankAccountReport.PreviousAccountBalance, 0.00001);
            Assert.AreEqual(currentAccountBalance, bankAccountReport.CurrentAccountBalance, 0.00001);
            Assert.AreEqual(ownerName, bankAccountReport.OwnerName);
            Assert.AreEqual(ownerSurname, bankAccountReport.OwnerSurname);
            Assert.AreEqual(ownerEmail, bankAccountReport.OwnerEmail);
        }

        [TestMethod]
        [DataRow(0.0f, 100.0f, "Adam", "Nowak", "an@poczta.com")]
        public void shouldReturnCorrectMessage(float previousAccountBalance, float currentAccountBalance, string ownerName, string ownerSurname, string ownerEmail)
        {
            ABankAccountReport bankAccountReport = new BankAccountReportWithOwnerData(previousAccountBalance, currentAccountBalance, ownerName, ownerSurname, ownerEmail);

            Assert.AreEqual(
                $"Czas wyg.:{bankAccountReport.TimeOfReportCreation};\nImię: {ownerName} Nazwisko: {ownerSurname}\nEmail: {ownerEmail};\nPoprz. stan konta: {previousAccountBalance}\nObecny stan konta: {currentAccountBalance}\nSaldo: {currentAccountBalance - previousAccountBalance}", 
                bankAccountReport.GetReportContent());
        }
    }
}
