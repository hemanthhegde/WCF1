using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using DocumentDBDataService;

namespace DocumentDBDataService.DocumentDB
{
    public class DocumentDBConnector
    {
        private static string _databaseId = ConfigurationManager.AppSettings["DatabaseId"];
        private static string _usersCollectionId = ConfigurationManager.AppSettings["UsersCollectionId"];
        private static string _storiesCollectionId = ConfigurationManager.AppSettings["StoriesCollectionId"];
        private static string _usersStorageUrl = ConfigurationManager.AppSettings["UsersStorageUrl"];
        private static string _storiesStorageUrl = ConfigurationManager.AppSettings["StoriesStorageUrl"];
        
        //This property establishes a new connection to DocumentDB the first time it is used, 
        //and then reuses this instance for the duration of the application avoiding the
        //overhead of instantiating a new instance of DocumentClient with each request
        private static DocumentClient _client;
        private static DocumentClient Client
        {
            get
            {
                if (_client == null)
                {
                    string endpoint = ConfigurationManager.AppSettings["EndPointUrl"];
                    string authKey = ConfigurationManager.AppSettings["AuthorizationKey"];
                    Uri endpointUri = new Uri(endpoint);
                    _client = new DocumentClient(endpointUri, authKey);
                }

                return _client;
            }
        }

        //Use the Database if it exists, if not create a new Database
        private static Database ReadOrCreateDatabase()
        {
            var db = Client.CreateDatabaseQuery()
                            .Where(d => d.Id == _databaseId)
                            .AsEnumerable()
                            .FirstOrDefault();

            if (db == null)
            {
                db = Client.CreateDatabaseAsync(new Database { Id = _databaseId }).Result;
            }

            return db;
        }

        //Use the ReadOrCreateDatabase function to get a reference to the database.
        private static Database _database;
        private static Database Database
        {
            get
            {
                if (_database == null)
                {
                    _database = ReadOrCreateDatabase();
                }

                return _database;
            }
        }

        //Use the DocumentCollection if it exists, if not create a new Collection
        private static DocumentCollection ReadOrCreateCollection(string databaseLink, string collectionId)
        {
            var col = Client.CreateDocumentCollectionQuery(databaseLink)
                              .Where(c => c.Id == collectionId)
                              .AsEnumerable()
                              .FirstOrDefault();

            if (col == null)
            {
                var collectionSpec = new DocumentCollection { Id = collectionId };
                var requestOptions = new RequestOptions { OfferType = "S1" };

                col = Client.CreateDocumentCollectionAsync(databaseLink, collectionSpec, requestOptions).Result;
            }

            return col;
        }

        //Use the ReadOrCreateCollection function to get a reference to the collection.
        private static DocumentCollection _usersCollection;
        private static DocumentCollection UsersCollection
        {
            get
            {
                if (_usersCollection == null)
                {
                    _usersCollection = ReadOrCreateCollection(Database.SelfLink, _usersCollectionId);
                }

                return _usersCollection;
            }
        }

        //Use the ReadOrCreateCollection function to get a reference to the collection.
        private static DocumentCollection _storiessCollection;
        private static DocumentCollection StoriesCollection
        {
            get
            {
                if (_storiessCollection == null)
                {
                    _storiessCollection = ReadOrCreateCollection(Database.SelfLink, _storiesCollectionId);
                }

                return _storiessCollection;
            }
        }

        public static bool IsUserNameAvailable(string userName)
        {
            var userNames = Client.CreateDocumentQuery<User>(UsersCollection.DocumentsLink).Where(user => user.Email == userName).AsEnumerable();
            var retVal = userNames.Count() == 0;
            return retVal;
        }

        public static async Task<bool> AddUser(User user)
        {
            var userPic = user.ProfilePicture;
            string uniqueFileName = "";
            if (userPic != null)
            {                
                uniqueFileName = userPic.FileName + "_" + Guid.NewGuid();
                await StorageConnector.UploadUserData(uniqueFileName, userPic.ContentType, userPic.Data);
            }

            DBUser dbUser = new DBUser() { Country= user.Country, DisplayName= user.DisplayName, DOB = user.DisplayName, Email = user.Email, Password = user.Password, ZipCode = user.ZipCode, UserProfilePictureBlob = uniqueFileName };
            var createdUser = await Client.CreateDocumentAsync(UsersCollection.DocumentsLink, dbUser);

            if (createdUser == null)
                return false;

            return true;
        }

        public static async Task<bool> UpdateUserProfilePicture(UserPicture userPic)
        {
            SqlQuerySpec querySpec = new SqlQuerySpec(string.Format("SELECT * FROM root WHERE (root.Email = \"{0}\") ", userPic.UserId));
            Document userDoc = Client.CreateDocumentQuery(UsersCollection.DocumentsLink, querySpec).AsEnumerable().FirstOrDefault();

            if (userDoc == null)
                return false;

            try
            {
                // Delete the previous profile picture if its already present.
                var blobKey = userDoc.GetPropertyValue<string>(Constants.UserProfilePictureBlobKey);
                if (!string.IsNullOrEmpty(blobKey))
                    await StorageConnector.DeleteUserProfilePicture(blobKey);

                string uniqueFileName = userPic.FileName + "_" + Guid.NewGuid();
                await StorageConnector.UploadUserData(uniqueFileName, userPic.ContentType, userPic.Data);
                userDoc.SetPropertyValue(Constants.UserProfilePictureBlobKey, uniqueFileName);
                await Client.ReplaceDocumentAsync(userDoc.SelfLink, userDoc);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public static async Task<bool> CreateStoryInDB(Story story)
        {
            var storyMedia = story.StoryVisualResource;
            if (storyMedia == null || storyMedia.Data == null)
                return false;

            string uniqueFileName = storyMedia.FileName + "_" + Guid.NewGuid();
            await StorageConnector.UploadStory(uniqueFileName, storyMedia.ContentType, storyMedia.Data);

            DBStory dbStory = new DBStory()
            {
                Country = story.Country,
                ZipCode = story.ZipCode,
                Category = story.Category,
                CreatedDate = story.CreatedDate,
                Heading = story.Heading,
                IsLocal = story.IsLocal,
                KeyWords = story.KeyWords,
                Message = story.Message,
                OwnerId = story.OwnerId,
                StoryMediaResourceBlob = uniqueFileName
            };

            var createdStory = await Client.CreateDocumentAsync(StoriesCollection.DocumentsLink, dbStory);
            if (createdStory == null)
                return false;

            return true;
        }

        public static DBUser GetUser(string userId)
        {
            SqlQuerySpec querySpec = new SqlQuerySpec(string.Format("SELECT * FROM root WHERE (root.Email = \"{0}\") ", userId));
            DBUser userDoc = Client.CreateDocumentQuery<DBUser>(UsersCollection.DocumentsLink, querySpec).AsEnumerable().FirstOrDefault();

            string blobLink = userDoc.UserProfilePictureBlob;
            if (!string.IsNullOrEmpty(blobLink) && !string.IsNullOrWhiteSpace(blobLink))
                userDoc.UserProfilePictureBlob = _usersStorageUrl + blobLink;

            return userDoc;
        }

        public static IEnumerable<DBStory> GetStoriesForUser(string userId)
        {
            SqlQuerySpec querySpec = new SqlQuerySpec(string.Format("SELECT * FROM root WHERE (root.OwnerId = \"{0}\") ", userId));
            List<DBStory> stories = Client.CreateDocumentQuery<DBStory>(StoriesCollection.DocumentsLink, querySpec).AsEnumerable().ToList();

            foreach (var story in stories)
            {
                string blobLink = story.StoryMediaResourceBlob;
                if (!string.IsNullOrEmpty(blobLink) && !string.IsNullOrWhiteSpace(blobLink))
                    story.StoryMediaResourceBlob = _storiesStorageUrl + blobLink;
            }

            return stories;
        }
    }
}