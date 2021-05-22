using CrossCutting;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemMonitorTestProject
{
    [TestFixture]
    public class LSystemMonitorTests
    {

        [Test]
        public void SystemMonitor_ValidDumpFile_CallsCrashLoggingServiceCorrectly()
        { // I want to test that the CrashLogger is called correctly so its an interaction test.
          //This test needs one stub and one mock. Assert against the mock to see if it's called properly. 
          // Arrange
            FileExtensionManager theFileExtensionManager = new FileExtensionManager();
            FakeCrashLoggingService mockCrashLoggingService = new FakeCrashLoggingService();
            SystemMonitor theMonitor = new SystemMonitor(theFileExtensionManager, mockCrashLoggingService, null,null); //inject the fakes
            // Act
            theMonitor.ProcessDump("ValidFile.dmp");
            // Assert
            StringAssert.Contains("Dump file is valid", mockCrashLoggingService.theMessage); // This test is about checking that the CrashLoggingService called
            // so we are 'asking' the fake 'were you called?'
        }

        [Test]
        public void SystemMonitor_InValidDumpFile_DoesNotCallCrashLoggingServiceCorrectly()
        { // I want to test that the CrashLogger is called correctly so its an interaction test.
          //This test needs one stub and one mock. Assert against the mock to see if it's called properly. 
          // Arrange
            FileExtensionManager theFileExtensionManager = new FileExtensionManager();
            FakeCrashLoggingService mockCrashLoggingService = new FakeCrashLoggingService();
            FakeCorruptFileLoggingService mockCorruptFileLoggingService = new FakeCorruptFileLoggingService();
            SystemMonitor theMonitor = new SystemMonitor(theFileExtensionManager, mockCrashLoggingService, null, mockCorruptFileLoggingService); //inject the fakes
            // Act
            theMonitor.ProcessDump("InValidFile.ddd");
            // Assert
            Assert.IsNull(mockCrashLoggingService.theMessage); // This test is about checking the CrashLoggingService is not called
        }

        [Test]
        public void SystemMonitor_ValidDumpFileCallsCrashLoggingServiceWhichThrowsException_CallsEmailService()
        { //Want to test that if the CrashLogger throws an exception then the EmailService is called correctly so its an interaction test.
          // This test needs two stubs and one mock. Assert against the mock. 
          // Arrange
            FileExtensionManager theFileExtensionManager = new FileExtensionManager();
            FakeCrashLoggingService stubCrashLoggingService = new FakeCrashLoggingService();
            stubCrashLoggingService.WillThrow = new Exception("fake exception"); // prime it with a fake exception 
            FakeEmailService mockEmailService = new FakeEmailService();
            SystemMonitor theMonitor = new SystemMonitor(theFileExtensionManager, stubCrashLoggingService, mockEmailService,null); //inject the fakes
            // Act
            theMonitor.ProcessDump("ValidFile.dmp");
            // Assert
            StringAssert.Contains("HelpDesk@lit.ie", mockEmailService.theTo);
            StringAssert.Contains("crashLoggingService Web service threw exception", mockEmailService.theSubject);
            StringAssert.Contains("fake exception", mockEmailService.theBody);
        }

        [Test]
        public void SystemMonitor_NotValidDumpFile_CallsCorruptFileLoggingServiceCorrectly() // changes done by me
        {
            FileExtensionManager theFileExtensionManager = new FileExtensionManager();// Fix this
            FakeCorruptFileLoggingService mockCorruptFileLoggingService = new FakeCorruptFileLoggingService();
            FakeCrashLoggingService mockCrashLoggingService = new FakeCrashLoggingService();
            FakeEmailService mockEmailService = new FakeEmailService();
            SystemMonitor theMonitor = new SystemMonitor(theFileExtensionManager, mockCrashLoggingService, null, mockCorruptFileLoggingService);
            theMonitor.ProcessDump("Filename.txt");
            StringAssert.Contains("Dump file is corrupt", mockCorruptFileLoggingService.theMessage);// check if Corrupt File logging service called
        }

        [Test]
        public void SystemMonitor_NotValidDumpFile_CallsCorruptFileLoggingServiceCorrectlyThrowsException()// changes done by me
        { // I want to test that the CrashLogger is called correctly so its an interaction test.
          //This test needs one stub and one mock. Assert against the mock to see if it's called properly. 
          // Arrange
            FileExtensionManager theFileExtensionManager = new FileExtensionManager();
            FakeCrashLoggingService mockCrashLoggingService = new FakeCrashLoggingService();
            FakeEmailService mockEmailService = new FakeEmailService();
            FakeCorruptFileLoggingService stubCorruptFileLoggingService = new FakeCorruptFileLoggingService();
            stubCorruptFileLoggingService.WillThrow = new Exception("fake exception"); // prime it with a fake exception 
            SystemMonitor theMonitor = new SystemMonitor(theFileExtensionManager, mockCrashLoggingService, mockEmailService, stubCorruptFileLoggingService); //inject the fakes
            // Act
            theMonitor.ProcessDump("NotValidFile.txt");
            // Assert
            StringAssert.Contains("fake exception", mockEmailService.theBody); // This test is about checking that the CrashLoggingService called
            // so we are 'asking' the fake 'were you called?'
        }

        [Test]
        public void SystemMonitor_ValidDumpFile_DoesNotCallCorruptFileLoggingService()
        { // I want to test that the CrashLogger is called correctly so its an interaction test.
          //This test needs one stub and one mock. Assert against the mock to see if it's called properly. 
          // Arrange
            FileExtensionManager theFileExtensionManager = new FileExtensionManager();
            FakeCrashLoggingService mockCrashLoggingService = new FakeCrashLoggingService();
            FakeCorruptFileLoggingService mockCorruptFileLoggingService = new FakeCorruptFileLoggingService();
            SystemMonitor theMonitor = new SystemMonitor(theFileExtensionManager, mockCrashLoggingService, null, mockCorruptFileLoggingService); //inject the fakes
            // Act
            theMonitor.ProcessDump("ValidFile.dmp");
            // Assert
            Assert.IsNull(mockCorruptFileLoggingService.theMessage); // This test is about checking the CrashLoggingService is not called
        }


    }

    internal class FakeCorruptFileLoggingService : ICorruptFileLoggingService
    { // This is a configurable fake. The test code can configure it to be either a mock or a stub, we can prime it with an exception (from the test) and it will throw it.
        public Exception WillThrow = null;
        public string theMessage = null;

        public void LogCorruptionDetails(string message)
        {
            theMessage = message;
            if (WillThrow != null)
            { throw WillThrow; }
        }
    }


    // This is part of the test infrastructure, theses are hand written fakes. Interaction frameworks provide automation.
    internal class FakeCrashLoggingService : ICrashLoggingService
    { // This is a configurable fake. The test code can configure it to be either a mock or a stub, we can prime it with an exception (from the test) and it will throw it.
        public Exception WillThrow = null;
        public string theMessage = null;

        public void LogError(string message)
        {
            theMessage = message;
            if (WillThrow != null)
            { throw WillThrow; }
        }
    }

    internal class FakeEmailService : IEmailService
    { // This fake is really only a mock as it captures the values passed to the method and its not configurable.
        public string theTo = null;
        public string theSubject = null;
        public string theBody = null;
        public void SendEmail(string to, string subject, string body)
        {
            theTo = to;
            theSubject = subject;
            theBody = body;
        }


    }
}

