using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Autofac;

namespace Example.Test
{

    public interface ILogger
    {
        bool Log(string message);
        bool Log(Exception ex);
    }

    public interface IEmail
    {
        bool SendEmail(string to, string subject, string body);
    }

    public interface IBusinessObject
    {
        void Submit();
    }

    public class BusinessObject : IBusinessObject
    {

        ILogger logger;
        IEmail email;

        public BusinessObject(ILogger logger, IEmail email)
        {
            this.logger = logger;
            this.email = email;
        }

        public void Submit()
        {
            try
            {
                email.SendEmail("keith@aon.com", "Business Object Email", "Demo");
                logger.Log("Email sent to keith@aon.com");
            }
            catch (Exception ex)
            {
                logger.Log(ex);
            }
        }

    }


    [TestClass]
    public class BusinessObjectTests
    {
        public void Submit()
        {

            // Arrange
            var logger = new Mock<ILogger>(MockBehavior.Strict);
            var email = new Mock<IEmail>(MockBehavior.Strict);

            var bo = new BusinessObject(logger.Object, email.Object);

            // Act

            bo.Submit();

            // Assert

        }


        public void Submit_Autofac()
        {
            // Arrange
            var logger = new Mock<ILogger>(MockBehavior.Strict);
            var email = new Mock<IEmail>(MockBehavior.Strict);

            var builder = new ContainerBuilder();

            // register...

            var container = builder.Build();

            var bo = container.Resolve<IBusinessObject>();

            // Act

            bo.Submit();

            // Assert
        }
    }
}
