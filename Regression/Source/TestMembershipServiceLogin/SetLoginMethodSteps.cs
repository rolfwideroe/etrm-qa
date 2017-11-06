using System;
using System.Collections.Generic;
using ElvizTestUtils.QAAppserverServiceReference;
using NUnit.Framework;
using TechTalk.SpecFlow;
using System.Linq;
using TechTalk.SpecFlow.Assist;

namespace TestMembershipServiceLogin
{
    [Binding]
    public class SetLoginMethodSteps
    {
       
        [Given(@"String MembershipServiceURL is configured")]
        public void GivenStringMembershipServiceURLIsConfigured()
        {
            IDictionary<string, string> settings = ElvizTestUtils.ElvizRegistyConfigurationTool.GetElvizStringValuesFromRegistry("MembershipServiceURL");
            Assert.IsNotEmpty(settings["MembershipServiceURL"]);

            Console.WriteLine("MembershipServiceURL is configured to: " + settings["MembershipServiceURL"]);
        }
        
        [Given(@"UseMembershipAuthentication is set to value (.*)")]
        public static void GivenUseMembershipAuthenticationIsSetToValueOn(QaAppServerServiceElvizDWordRegValues loginMembership)
        {
            KeyValuePair<string, QaAppServerServiceElvizDWordRegValues> newSettings;
          
            newSettings = ElvizTestUtils.ElvizRegistyConfigurationTool.SetElvizDWordValueInRegistry(QaAppServerServiceElvizDWordRegKeys.UseMembershipAuthentication,
                loginMembership);

            Assert.AreEqual(QaAppServerServiceElvizDWordRegKeys.UseMembershipAuthentication.ToString(), newSettings.Key);
            Assert.AreEqual(newSettings.Value, loginMembership);
        }
        
        [Given(@"ElvizUserName is set to (.*)")]
        public static void GivenElvizUserNameIsSetTo(string username)
        {
            KeyValuePair<string, string> newSettings = new KeyValuePair<string, string>();
            newSettings = ElvizTestUtils.ElvizRegistyConfigurationTool.SetElvizStringValueInRegistry(QaAppServerServiceElvizStringRegKeys.ElvizUserName,
               username);
            Assert.AreEqual(QaAppServerServiceElvizStringRegKeys.ElvizUserName.ToString(), newSettings.Key);
            Assert.AreEqual(newSettings.Value, username);
        }
        
        [Given(@"ElvizPassword is set to (.*)")]
        public static void GivenElvizPasswordIsSetTo(string password)
        {
            KeyValuePair<string, string> newSettings = new KeyValuePair<string, string>();
            newSettings = ElvizTestUtils.ElvizRegistyConfigurationTool.SetElvizStringValueInRegistry(QaAppServerServiceElvizStringRegKeys.ElvizPassword,
               password);
            Assert.AreEqual(QaAppServerServiceElvizStringRegKeys.ElvizPassword.ToString(), newSettings.Key);
            Assert.AreEqual(newSettings.Value, password);
        }

        [Then(@"Restart ETRM services")]
        public void ThenRestartETRMServices()
        { 
            ElvizTestUtils.ElvizRegistyConfigurationTool.RestartElvizServices();
            Console.WriteLine("Restarting services...");
        }

      
        [Then(@"New user can login with membership service credentials")]
        public void ThenNewUserCanLoginWithMembershipServiceCredentials()
        {
            Console.WriteLine("new login window...");
            //test will login with wrong credentials first
            //after test will login with correct credentials
            //for the moment I didn't find way of sending parameters (username/password) to TestCompelete test item,
            //so credentials are hardcoded in test script, function RunETRMWhenMembershipLoginIsOn()
            TestExecuteWrapper.MembershipLoginWindowAppeared();
        }



        [AfterFeature]
        public static void RestoreSettings()
        {
            string defaultUserName = "Vizard";
            string defaultPswName = "elviz";

            //restore default settings
            GivenElvizUserNameIsSetTo(defaultUserName);
            GivenElvizPasswordIsSetTo(defaultPswName);

            //use login manager
            GivenUseMembershipAuthenticationIsSetToValueOn(QaAppServerServiceElvizDWordRegValues.Off);

            //restarting services
            ElvizTestUtils.ElvizRegistyConfigurationTool.RestartElvizServices();
            Console.WriteLine("Restarting services...");

        }

    }
}
