using ProjectLayerClassLibrary.DataLayer.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibraryTest.LogicLayerTest
{
    [TestClass]
    public class BasicLogicLayerBankAccountReportTest
    {
        [TestMethod]
        [DataRow(0.0f, 100.0f, "Adam", "Nowak", "an@poczta.com")]
        public void shouldCreateCorrect(float previousAccountBalance, float currentAccountBalance, string ownerName, string ownerSurname, string ownerEmail)
        {
            DateTime prev = DateTime.UtcNow;
            ProjectLayerClassLibrary.DataLayer.ABankAccountReport bankAccountReport = new ProjectLayerClassLibrary.DataLayer.Implementations.BankAccountReportWithOwnerData(previousAccountBalance, currentAccountBalance, ownerName, ownerSurname, ownerEmail);
            ProjectLayerClassLibrary.LogicLayer.ABankAccountReport logicBankAccountReport = new ProjectLayerClassLibrary.LogicLayer.Implementations.BasicLogicLayerBankAccountReport(bankAccountReport);
            DateTime after = DateTime.UtcNow;

            Assert.IsTrue(logicBankAccountReport.TimeOfReportCreation > prev);
            Assert.IsTrue(logicBankAccountReport.TimeOfReportCreation < after);
            Assert.AreEqual(previousAccountBalance, logicBankAccountReport.PreviousAccountBalance, 0.00001);
            Assert.AreEqual(currentAccountBalance, logicBankAccountReport.CurrentAccountBalance, 0.00001);
            Assert.AreEqual(ownerName, logicBankAccountReport.OwnerName);
            Assert.AreEqual(ownerSurname, logicBankAccountReport.OwnerSurname);
            Assert.AreEqual(ownerEmail, logicBankAccountReport.OwnerEmail);
        }

        [TestMethod]
        [DataRow(0.0f, 100.0f, "Adam", "Nowak", "an@poczta.com")]
        public void shouldReturnCorrectMessage(float previousAccountBalance, float currentAccountBalance, string ownerName, string ownerSurname, string ownerEmail)
        {
            ProjectLayerClassLibrary.DataLayer.ABankAccountReport bankAccountReport = new ProjectLayerClassLibrary.DataLayer.Implementations.BankAccountReportWithOwnerData(previousAccountBalance, currentAccountBalance, ownerName, ownerSurname, ownerEmail);
            ProjectLayerClassLibrary.LogicLayer.ABankAccountReport logicBankAccountReport = new ProjectLayerClassLibrary.LogicLayer.Implementations.BasicLogicLayerBankAccountReport(bankAccountReport);

            Assert.AreEqual(
                $"Czas wyg.:{logicBankAccountReport.TimeOfReportCreation}; Imię: {ownerName} Nazwisko: {ownerSurname} Email: {ownerEmail}; Poprz. stan konta: {previousAccountBalance} Obecny stan konta: {currentAccountBalance} Saldo: {currentAccountBalance - previousAccountBalance}",
                logicBankAccountReport.GetReportContent());
        }
    }
}
