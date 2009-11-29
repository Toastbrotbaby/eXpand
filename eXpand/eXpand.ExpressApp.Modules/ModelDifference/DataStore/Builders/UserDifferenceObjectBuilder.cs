using System;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base.Security;
using eXpand.ExpressApp.Core;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;

namespace eXpand.ExpressApp.ModelDifference.DataStore.Builders{
    public class UserDifferenceObjectBuilder 
    {
        public static void SetUp(UserModelDifferenceObject userModelDifferenceObject)
        {
            userModelDifferenceObject.Name = string.Format("AutoCreated for {0} {1}", ((IAuthenticationStandardUser)SecuritySystem.CurrentUser).UserName, DateTime.Now);
        }


        public static bool CreateDynamicMembers(XafApplication application){
            bool members = false;
            if (application != null && application.Security.UserType != null){
                members = XafTypesInfo.Instance.CreateCollection(application.Security.UserType, typeof(UserModelDifferenceObject), "UsersUserModelDiff", XafTypesInfo.XpoTypeInfoSource.XPDictionary, typeof(UserModelDifferenceObject).Name + "s") !=
                          null;
                if (members)
                    members = XafTypesInfo.Instance.CreateCollection(typeof(UserModelDifferenceObject), application.Security.UserType,
                                                                     "UsersUserModelDiff",
                                                                     XafTypesInfo.XpoTypeInfoSource.XPDictionary,
                                                                     "Users")!= null;
            }
            return members;
        }

    }
}