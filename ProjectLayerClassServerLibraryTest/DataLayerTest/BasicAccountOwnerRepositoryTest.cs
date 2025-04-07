using ProjectLayerClassServerLibrary.DataLayer;
using ProjectLayerClassServerLibrary.DataLayer.Implementations;
using ProjectLayerClassServerLibrary.DataLayer.Implementations.Repositories;
using ProjectLayerClassServerLibrary.DataLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassServerLibraryTest.DataLayerTest
{
    [TestClass]
    public class BasicAccountOwnerRepositoryTest
    {
        AAccountOwner accountOwner = new BasicAccountOwner(1000, "IK123456", "Jan", "Kowalski", "jk@poczta.com", "12345678");

        [TestMethod]
        void shouldCreateCorrectAndEmpty()
        {
            IAccountOwnerRepository accountOwnerRepository = new BasicAccountOwnerRepository();
            Assert.AreEqual(0, accountOwnerRepository.GetAll().Count());
        }

        [TestMethod]
        public void shouldGetByIdReturn()
        {
            IAccountOwnerRepository accountOwnerRepository = new BasicAccountOwnerRepository();
            accountOwnerRepository.Save(accountOwner);
            Assert.AreEqual(1, accountOwnerRepository.GetAll().Count());
        }

        [TestMethod]
        public void shouldGetByIdReturnEqual()
        {
            IAccountOwnerRepository accountOwnerRepository = new BasicAccountOwnerRepository();
            accountOwnerRepository.Save(accountOwner);
            AAccountOwner? returned = accountOwnerRepository.Get(accountOwner.GetId());
            Assert.IsNotNull(returned);
            Assert.AreEqual(accountOwner, returned);
        }

        [TestMethod]
        public void shouldGetByLoginReturnEqual()
        {
            IAccountOwnerRepository accountOwnerRepository = new BasicAccountOwnerRepository();
            accountOwnerRepository.Save(accountOwner);
            AAccountOwner? returned = accountOwnerRepository.GetByOwnerLogin(accountOwner.OwnerLogin);
            Assert.IsNotNull(returned);
            Assert.AreEqual(accountOwner, returned);
        }

        [TestMethod]
        public void shouldRemoveById()
        {
            IAccountOwnerRepository accountOwnerRepository = new BasicAccountOwnerRepository();
            accountOwnerRepository.Save(accountOwner);
            Assert.IsTrue(accountOwnerRepository.Remove(accountOwner.GetId()));
            Assert.AreEqual(0, accountOwnerRepository.GetAll().Count());
        }

        [TestMethod]
        public void shouldRemoveByObjectReference()
        {
            IAccountOwnerRepository accountOwnerRepository = new BasicAccountOwnerRepository();
            accountOwnerRepository.Save(accountOwner);
            Assert.IsTrue(accountOwnerRepository.Remove(accountOwner));
            Assert.AreEqual(0, accountOwnerRepository.GetAll().Count());
        }
    }
}
