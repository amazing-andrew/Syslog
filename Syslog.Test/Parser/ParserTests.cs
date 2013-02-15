using Microsoft.VisualStudio.TestTools.UnitTesting;
using Syslog.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syslog.Test.Parser
{
    [TestClass]
    public class ParserTests
    {
        string msg2 = @"Sep 25 17:36:09 TS-XELA54 errormon[1799]: Information situation cleared! USER_DEFINITION";

        /// <summary>
        /// Parses the example message1.
        /// </summary>
        [TestMethod]
        public void ParseExampleMessage1()
        {
            string syslogString = "<11>Nov 21 17:27:53 SEVER1 MyProgram[13163]: Ouch an ERROR!";

            int currentYear = DateTimeOffset.Now.Year;
            DateTimeOffset expectedDate = TimeZoneInfo.ConvertTime(
                new DateTime(currentYear, 11, 21, 17, 27, 53), 
                TimeZoneInfo.Local);

            SyslogParser parser = new SyslogParser();
            SyslogMessage msg = parser.Parse(syslogString);

            Assert.AreEqual(SyslogFacility.User, msg.Facility);
            Assert.AreEqual(SyslogSeverity.Error, msg.Severity);
            Assert.AreEqual("SEVER1", msg.HostName);
            Assert.AreEqual(expectedDate, msg.TimeStamp);
            Assert.AreEqual("MyProgram[13163]: Ouch an ERROR!", msg.MessageText);
        }

        [TestMethod]
        public void ParseExampleMessage2()
        {
            string syslogString = "<165>1 2003-08-24T05:14:15-07:00 192.0.2.1 myproc 8710 - - %% It's time to make the do-nuts.";

            SyslogMessage msg = new SyslogParser().Parse(syslogString);

            Assert.AreEqual(SyslogFacility.Local4, msg.Facility);
            Assert.AreEqual(SyslogSeverity.Notice, msg.Severity);

            Assert.AreEqual("1", msg.Version);
            Assert.AreEqual(new DateTimeOffset(2003, 8, 24, 5, 14, 15, 0, TimeSpan.FromHours(-7)), msg.TimeStamp);
            Assert.AreEqual("192.0.2.1", msg.HostName);
            Assert.AreEqual("myproc", msg.AppName);
            Assert.AreEqual("8710", msg.ProcessID);
            Assert.AreEqual(null, msg.MessageID);
            Assert.AreEqual(0, msg.StructuredData.Count);
            Assert.AreEqual("%% It's time to make the do-nuts.", msg.MessageText);

        }

        [TestMethod]
        public void ParseExampleMessage3()
        {
            string syslogString = @"<14>1 2012-10-06T11:03:56.493 SRX100 RT_FLOW - RT_FLOW_SESSION_CLOSE [junos@2636.1.1.1.2.36 reason=""TCP FIN"" source-address=""192.168.199.207"" source-port=""59292"" destination-address=""184.73.190.157"" destination-port=""80"" service-name=""junos-http"" nat-source-address=""50.193.12.149"" nat-source-port=""19230"" nat-destination-address=""184.73.190.157"" nat-destination-port=""80"" src-nat-rule-name=""source-nat-rule"" dst-nat-rule-name=""None"" protocol-id=""6"" policy-name=""trust-to-untrust"" source-zone-name=""trust"" destination-zone-name=""untrust"" session-id-32=""9375"" packets-from-client=""9"" bytes-from-client=""4342"" packets-from-server=""7"" bytes-from-server=""1507"" elapsed-time=""1"" application=""UNKNOWN"" nested-application=""UNKNOWN"" username=""N/A"" roles=""N/A"" packet-incoming-interface=""vlan.0""]";
            SyslogMessage msg = new SyslogParser().Parse(syslogString);
        }

        [TestMethod]
        public void ParseStructuredData1()
        {
            string sd = "[origin ip=\"192.0.2.1\" ip=\"192.0.2.129\"]";
            
            SyslogParser parser = new SyslogParser();

            var resultList = parser.ParseStructuredData(sd);
            Assert.AreEqual(1, resultList.Count);

            StructuredDataElement result = resultList[0];
            Assert.AreEqual("origin", result.ID);

            Assert.AreEqual(1, result.Properties.Count);

            var expected = new string[] { "192.0.2.1", "192.0.2.129" };
            var actual = result.Properties.GetValues("ip");

            Assert.AreEqual(expected.Length, actual.Length);
            Assert.AreEqual(expected[0], actual[0]);
            Assert.AreEqual(expected[1], actual[1]);
        }

        [TestMethod]
        public void ParseStructuredData2()
        {
            string sd = "[exampleSDID@0 iut=\"3\" eventSource=\"Application\" eventID=\"1011\"]";
            var parsedList = new SyslogParser().ParseStructuredData(sd);

            Assert.AreEqual(1, parsedList.Count);

            var result = parsedList[0];

            Assert.AreEqual("exampleSDID@0", result.ID);
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("3", result["iut"]);
            Assert.AreEqual("Application", result["eventSource"]);
            Assert.AreEqual("1011", result["eventID"]);
        }

        [TestMethod]
        public void ParseMultipleStructedData()
        {
            string sd1 = "[origin ip=\"192.0.2.1\" ip=\"192.0.2.129\"]";
            string sd2 = "[exampleSDID@0 iut=\"3\" eventSource=\"Application\" eventID=\"1011\"]";
            string sdCombined = sd1 + sd2;

            var parsedList = new SyslogParser().ParseStructuredData(sdCombined);

            Assert.AreEqual(2, parsedList.Count);


            StructuredDataElement result = parsedList[0];
            var s = result.ToString();
            Assert.AreEqual("origin", result.ID);

            Assert.AreEqual(1, result.Properties.Count);

            var expected = new string[] { "192.0.2.1", "192.0.2.129" };
            var actual = result.Properties.GetValues("ip");

            Assert.AreEqual(expected.Length, actual.Length);
            Assert.AreEqual(expected[0], actual[0]);
            Assert.AreEqual(expected[1], actual[1]);


            var result2 = parsedList[1];

            Assert.AreEqual("exampleSDID@0", result2.ID);
            Assert.AreEqual(3, result2.Count);
            Assert.AreEqual("3", result2["iut"]);
            Assert.AreEqual("Application", result2["eventSource"]);
            Assert.AreEqual("1011", result2["eventID"]);
        }

        [TestMethod]
        public void ParseStructedDataWithEscpaes()
        {
            string sd = "[name value=\"this is [something\\] \\\"interesting\\\"\\\\\\\"and pleasing\\\"\"]";

            var parsedList = new SyslogParser().ParseStructuredData(sd);
            Assert.AreEqual(1, parsedList.Count);

            var result = parsedList[0];
            Assert.AreEqual("name", result.ID);
            Assert.AreEqual("this is [something] \"interesting\"\\\"and pleasing\"", result["value"]);
        }

        [TestMethod]
        public void temp()
        {
            string msg2 = "<13>Feb  5 17:32:18 10.0.0.99 Use the BFG!";
            var result2 = new SyslogParser().Parse(msg2);
        }
    }
}
