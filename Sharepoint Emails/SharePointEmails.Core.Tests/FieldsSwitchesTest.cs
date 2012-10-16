using SharePointEmails;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections;

namespace SharePointEmails.Core.Tests
{


    /// <summary>
    ///This is a test class for FieldsSwitchesTest and is intended
    ///to contain all FieldsSwitchesTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FieldsSwitchesTest
    {
        [TestMethod()]
        public void ParseTest1()
        {
            string val = "val1:field1,field2 ;val2:field3, field4,field5";
            var actual = FieldSwitch.Parse(val, "mainField", new Dictionary<string, string> 
            {
                   {"mainField","mainFieldID"},
                {"field1","field1ID"},
                {"field2","field2ID"},
                {"field3","field3ID"},
                {"field4","field4ID"},
                {"field5","field5ID"}
            });
            Assert.AreEqual(2, actual.switches.Count);
            Assert.AreEqual("mainField", actual.field.fieldName);
            Assert.AreEqual("mainFieldID", actual.field.fieldId);
            Assert.AreEqual("val1", actual.switches[0].value);
            Assert.AreEqual("val2", actual.switches[1].value);
            Assert.AreEqual(2, actual.switches[0].fields.Count, 2);
            Assert.AreEqual(3, actual.switches[1].fields.Count, 3);
            Assert.AreEqual("field1", actual.switches[0].fields[0].fieldName);
            Assert.AreEqual("field1ID", actual.switches[0].fields[0].fieldId);
            Assert.AreEqual("field2", actual.switches[0].fields[1].fieldName);
            Assert.AreEqual("field2ID", actual.switches[0].fields[1].fieldId);
            Assert.AreEqual("field3", actual.switches[1].fields[0].fieldName);
            Assert.AreEqual("field3ID", actual.switches[1].fields[0].fieldId);
            Assert.AreEqual("field4", actual.switches[1].fields[1].fieldName);
            Assert.AreEqual("field4ID", actual.switches[1].fields[1].fieldId);
            Assert.AreEqual("field5", actual.switches[1].fields[2].fieldName);
            Assert.AreEqual("field5ID", actual.switches[1].fields[2].fieldId);
        }
        [TestMethod()]
        public void ParseTest2()
        {
            string val = " val1: field1 ,field2 ;val2:field3, field4, field5;";
            var actual = FieldSwitch.Parse(val, "mainField", new Dictionary<string, string> 
            {
                   {"mainField","mainFieldID"},
                {"field1","field1ID"},
                {"field2","field2ID"},
                {"field4","field4ID"},
                {"field5","field5ID"}
            });
            Assert.AreEqual(2, actual.switches.Count);
            Assert.AreEqual("mainField", actual.field.fieldName);
            Assert.AreEqual("mainFieldID", actual.field.fieldId);
            Assert.AreEqual("val1", actual.switches[0].value);
            Assert.AreEqual("val2", actual.switches[1].value);
            Assert.AreEqual(2, actual.switches[0].fields.Count, 2);
            Assert.AreEqual(3, actual.switches[1].fields.Count, 2);
            Assert.AreEqual("field1", actual.switches[0].fields[0].fieldName);
            Assert.AreEqual("field1ID", actual.switches[0].fields[0].fieldId);
            Assert.AreEqual("field2", actual.switches[0].fields[1].fieldName);
            Assert.AreEqual("field2ID", actual.switches[0].fields[1].fieldId);
            Assert.AreEqual("field4", actual.switches[1].fields[0].fieldName);
            Assert.AreEqual("field4ID", actual.switches[1].fields[0].fieldId);
            Assert.AreEqual("field5", actual.switches[1].fields[1].fieldName);
            Assert.AreEqual("field5ID", actual.switches[1].fields[1].fieldId);
        }

        [TestMethod]
        public void ToJsonTest()
        {
            string val = " val1: field1 ,field2 ;val2:field3, field4, field5;";
            var actual = FieldSwitch.Parse(val, "mainField", new Dictionary<string, string> 
            {
                   {"mainField","mainFieldID"},
                {"field1","field1ID"},
                {"field2","field2ID"},
                {"field4","field4ID"},
                {"field5","field5ID"}
            });

            var test = new FieldsSwitches();
            test.Add(actual);
            test.Add(actual);

            var res = test.ToJson();

            Assert.IsNotNull(res);
            Assert.IsTrue(res.Length > 0);
        }

        /// <summary>
        ///A test for Create
        ///</summary>
        [TestMethod()]
        public void CreateTest()
        {
            //string info = "[TemplateBodyUseFile{true:TemplateBody;false:TemplateBodyFile;}]"+"[TemplateSubjectUseFile{true:TemplateSubject;false:TemplateSubjectFile;}]";
            //ArrayList controls = null; // TODO: Initialize to an appropriate value
            //FieldsSwitches expected = null; // TODO: Initialize to an appropriate value
            //FieldsSwitches actual;
            //actual = FieldsSwitches.Create(info, controls);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
