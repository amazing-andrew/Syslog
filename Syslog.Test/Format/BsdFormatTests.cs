using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syslog.Test.Format
{
    [TestClass]
    public class BsdFormatTests
    {
        public string Format(SyslogMessage msg)
        {
            Syslog.Format.BsdSyslogFormat bsd = new Syslog.Format.BsdSyslogFormat();
            return bsd.GetString(msg);
        }

        [TestMethod]
        public void CanFormat1()
        {
            SyslogMessage msg = new SyslogMessage();
            msg.MessageText = "Catastrophic Database Error!";
            msg.TimeStamp = new DateTimeOffset(2012, 1, 28, 3, 27, 00, TimeSpan.Zero);
            msg.HostName = "Server1";
            msg.MessageText = "Catastrophic Database Error!";

            string formatted = Format(msg);

            Assert.AreEqual("<14>Jan 28 03:27:00 Server1 Catastrophic Database Error!", formatted);
        }
    }
}
