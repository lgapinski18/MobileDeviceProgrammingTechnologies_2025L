using ProjectLayerClassLibrary.DataLayer.Implementations;
using ProjectLayerClassLibrary.LogicLayer.Implementations;
using ProjectLayerClassLibrary.DataLayer;
using ProjectLayerClassLibrary.LogicLayer;
using ProjectLayerClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibraryTest.LogicLayerTest
{
    [TestClass]
    public class BasicLogicLayerAccountOwnerTest
    {
        [TestMethod]
        [DataRow(1, "IK123456", "Jan", "Kowalski", "jk@poczta.com", "12345678")]
        public void shouldCreateCorrect(int ownerId, string ownerLogin, string ownerName, string ownerSurname, string ownerEmail, string ownerPassword)
        {
            ProjectLayerClassLibrary.DataLayer.AAccountOwner accountOwner = new ProjectLayerClassLibrary.DataLayer.Implementations.BasicAccountOwner(ownerId, ownerLogin, ownerName, ownerSurname, ownerEmail, ownerPassword);
            ProjectLayerClassLibrary.LogicLayer.AAccountOwner? logicAccountOwner = ProjectLayerClassLibrary.LogicLayer.AAccountOwner.CreateAccountOwner(accountOwner);
            Assert.IsNotNull(logicAccountOwner);
            Assert.AreEqual(ownerId, logicAccountOwner.GetId());
            Assert.AreEqual(ownerLogin, logicAccountOwner.OwnerLogin);
            Assert.AreEqual(ownerName, logicAccountOwner.OwnerName);
            Assert.AreEqual(ownerSurname, logicAccountOwner.OwnerSurname);
            Assert.AreEqual(ownerEmail, logicAccountOwner.OwnerEmail);
            Assert.AreEqual(ownerPassword, logicAccountOwner.OwnerPassword);
        }

        [TestMethod]
        [DataRow(1, "IK123456", "Jan", "Kowalski", "jk@poczta.com", "12345678", 2, "IK123457", "Adam", "Nowak", "an@poczta.com", "87654321")]
        public void shouldChangeEveryProperty(int ownerId, string ownerLogin, string ownerName, string ownerSurname, string ownerEmail, string ownerPassword,
                                              int ownerId2, string ownerLogin2, string ownerName2, string ownerSurname2, string ownerEmail2, string ownerPassword2)
        {
            ProjectLayerClassLibrary.DataLayer.AAccountOwner accountOwner = new ProjectLayerClassLibrary.DataLayer.Implementations.BasicAccountOwner(ownerId, ownerLogin, ownerName, ownerSurname, ownerEmail, ownerPassword);
            ProjectLayerClassLibrary.LogicLayer.AAccountOwner? logicAccountOwner = ProjectLayerClassLibrary.LogicLayer.AAccountOwner.CreateAccountOwner(accountOwner);
            Assert.IsNotNull(logicAccountOwner);
            logicAccountOwner.SetId(ownerId2);
            logicAccountOwner.OwnerLogin = ownerLogin2;
            logicAccountOwner.OwnerName = ownerName2;
            logicAccountOwner.OwnerSurname = ownerSurname2;
            logicAccountOwner.OwnerEmail = ownerEmail2;
            logicAccountOwner.OwnerPassword = ownerPassword2;
            Assert.AreEqual(ownerId2, logicAccountOwner.GetId());
            Assert.AreEqual(ownerLogin2, logicAccountOwner.OwnerLogin);
            Assert.AreEqual(ownerName2, logicAccountOwner.OwnerName);
            Assert.AreEqual(ownerSurname2, logicAccountOwner.OwnerSurname);
            Assert.AreEqual(ownerEmail2, logicAccountOwner.OwnerEmail);
            Assert.AreEqual(ownerPassword2, logicAccountOwner.OwnerPassword);
        }
    }
}
