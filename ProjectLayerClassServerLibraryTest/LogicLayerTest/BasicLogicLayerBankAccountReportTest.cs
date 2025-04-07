using ProjectLayerClassServerLibrary.DataLayer.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassServerLibraryTest.LogicLayerTest
{
    [TestClass]
    public class BasicLogicLayerBankAccountReportTest
    {
        [TestMethod]
        [DataRow(0.0f, 100.0f, "Adam", "Nowak", "an@poczta.com")]
        public void shouldCreateCorrect(float previousAccountBalance, float currentAccountBalance, string ownerName, string ownerSurname, string ownerEmail)
        {
            DateTime prev = DateTime.UtcNow;
            ProjectLayerClassServerLibrary.DataLayer.ABankAccountReport bankAccountReport = new ProjectLayerClassServerLibrary.DataLayer.Implementations.BankAccountReportWithOwnerData(previousAccountBalance, currentAccountBalance, ownerName, ownerSurname, ownerEmail);
            ProjectLayerClassServerLibrary.LogicLayer.ABankAccountReport logicBankAccountReport = new ProjectLayerClassServerLibrary.LogicLayer.Implementations.BasicLogicLayerBankAccountReport(bankAccountReport);
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
            ProjectLayerClassServerLibrary.DataLayer.ABankAccountReport bankAccountReport = new ProjectLayerClassServerLibrary.DataLayer.Implementations.BankAccountReportWithOwnerData(previousAccountBalance, currentAccountBalance, ownerName, ownerSurname, ownerEmail);
            ProjectLayerClassServerLibrary.LogicLayer.ABankAccountReport logicBankAccountReport = new ProjectLayerClassServerLibrary.LogicLayer.Implementations.BasicLogicLayerBankAccountReport(bankAccountReport);

            Assert.AreEqual(
                $"Czas wyg.:{logicBankAccountReport.TimeOfReportCreation};\nImię: {ownerName} Nazwisko: {ownerSurname}\nEmail: {ownerEmail};\nPoprz. stan konta: {previousAccountBalance}\nObecny stan konta: {currentAccountBalance}\nSaldo: {currentAccountBalance - previousAccountBalance}",
                logicBankAccountReport.GetReportContent());
        }
    }
}
