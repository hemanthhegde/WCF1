using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace DocumentDBDataService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IActiDataService" in both code and config file together.
    [ServiceContract]
    public interface IActiDataService
    {
        [OperationContract(Name = "CreateUserProfile")]
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "CreateUserProfile")]
        bool CreateUserProfile(Stream data);

        [OperationContract(Name = "IsUserNameAvailable")]
        [WebGet(UriTemplate = "IsUserNameAvailable/userName/{name}", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        bool IsUserNameAvailable(string name);

        [OperationContract(Name = "UpdateUserPicture")]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "UpdateUserPicture")]
        bool UpdateUserPicture(Stream data);

        [OperationContract(Name = "CreateStory")]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "CreateStory")]
        bool CreateStory(Stream data);

        [OperationContract(Name = "GetUser")]
        [WebGet(UriTemplate = "GetUser/userName/{userName}", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        DBUser GetUser(string userName);

        [OperationContract(Name = "GetStoriesForUser")]
        [WebGet(UriTemplate = "GetStoriesForUser/userName/{userName}", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        IEnumerable<DBStory> GetStoriesForUser(string userName);
    }
}
