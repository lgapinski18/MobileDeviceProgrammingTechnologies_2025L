using ProjectLayerClassServerLibrary.DataLayer;
using ProjectLayerClassServerLibrary.DataLayer.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassServerLibraryTest.DataLayerTest
{
    [TestClass]
    public class BasicAccountOwnerTest
    {
        [TestMethod]
        [DataRow(1, "IK123456", "Jan", "Kowalski", "jk@poczta.com", "12345678")]
        public void shouldCreateCorrect(int ownerId, string ownerLogin, string ownerName, string ownerSurname, string ownerEmail, string ownerPassword)
        {
            AAccountOwner accountOwner = new BasicAccountOwner(ownerId, ownerLogin, ownerName, ownerSurname, ownerEmail, ownerPassword);
            Assert.AreEqual(ownerId, accountOwner.GetId());
            Assert.AreEqual(ownerLogin, accountOwner.OwnerLogin);
            Assert.AreEqual(ownerName, accountOwner.OwnerName);
            Assert.AreEqual(ownerSurname, accountOwner.OwnerSurname);
            Assert.AreEqual(ownerEmail, accountOwner.OwnerEmail);
            Assert.AreEqual(ownerPassword, accountOwner.OwnerPassword);
        }

        [TestMethod]
        [DataRow(1, "IK123456", "Jan", "Kowalski", "jk@poczta.com", "12345678", 2, "IK123457", "Adam", "Nowak", "an@poczta.com", "87654321")]
        public void shouldChangeEveryProperty(int ownerId, string ownerLogin, string ownerName, string ownerSurname, string ownerEmail, string ownerPassword,
                                              int ownerId2, string ownerLogin2, string ownerName2, string ownerSurname2, string ownerEmail2, string ownerPassword2)
        {
            AAccountOwner accountOwner = new BasicAccountOwner(ownerId, ownerLogin, ownerName, ownerSurname, ownerEmail, ownerPassword);
            accountOwner.SetId(ownerId2);
            accountOwner.OwnerLogin = ownerLogin2;
            accountOwner.OwnerName = ownerName2;
            accountOwner.OwnerSurname = ownerSurname2;
            accountOwner.OwnerEmail = ownerEmail2;
            accountOwner.OwnerPassword = ownerPassword2;
            Assert.AreEqual(ownerId2, accountOwner.GetId());
            Assert.AreEqual(ownerLogin2, accountOwner.OwnerLogin);
            Assert.AreEqual(ownerName2, accountOwner.OwnerName);
            Assert.AreEqual(ownerSurname2, accountOwner.OwnerSurname);
            Assert.AreEqual(ownerEmail2, accountOwner.OwnerEmail);
            Assert.AreEqual(ownerPassword2, accountOwner.OwnerPassword);
        }
    }
}
