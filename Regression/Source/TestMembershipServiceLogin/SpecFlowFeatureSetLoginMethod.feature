Feature: SetLoginMethod
Registry is preconfigured to support login method using membership service:
Keys DWORD UseMembershipAuthentication and String MembershipServiceURL are exist.
Test Scenario set UseMembershipAuthentication to TRUE and restart ETRM services, 
opens ETRM client using membership login


Scenario Outline: SetUp ETRM to use membership service as login method
Given String MembershipServiceURL is configured
And UseMembershipAuthentication is set to value <MembershipLogin>
And ElvizUserName is set to <ElvizUserNameValue>
And ElvizPassword is set to <ElvizPasswordValue>
Then Restart ETRM services
And New user can login with membership service credentials

Examples: 
|MembershipLogin|ElvizUserNameValue|ElvizPasswordValue|
|On|simplelogin|I-sem17|


#Scenario Outline: SetUp ETRM to use login manager as login method
#Given String MembershipServiceURL is configured
#And UseMembershipAuthentication is set to value <MembershipLogin>
#And ElvizUserName is set to <ElvizUserNameValue>
#And ElvizPassword is set to <ElvizPasswordValue>
#Then Restart ETRM services
#
#Examples: 
#|MembershipLogin|ElvizUserNameValue|ElvizPasswordValue|
#|Off|DealImport|elviz|
