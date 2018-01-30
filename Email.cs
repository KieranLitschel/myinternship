using System;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace WebApplication
{
    class Email
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/gmail-dotnet-quickstart.json
        static string[] Scopes = {GmailService.Scope.GmailReadonly};

        static string ApplicationName = "Internship Sorter";
        private static string userID = "litschel.kieran@gmail.com";

        public static void email(String userId)
        {
            UserCredential credential;

            using (var stream =
                new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/gmail-dotnet-quickstart.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            // Create Gmail API service.
            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define parameters of request.
            UsersResource.MessagesResource.ListRequest messagesListRequest =
                service.Users.Messages.List("litschel.kieran@gmail.com");

            // List labels.
            IList<Message> messageIDs = messagesListRequest.Execute().Messages;
            
            Database db = new Database("database");
            List<string> knownID = db.getEmailIds();
            foreach (Message messageID in messageIDs)
            {
                if (!knownID.Contains(messageID.Id))
                {
                    String decodedString = "";
                    UsersResource.MessagesResource.GetRequest preMessage =
                        service.Users.Messages.Get(userID, messageID.Id);
                    preMessage.Format = UsersResource.MessagesResource.GetRequest.FormatEnum.Full;
                    Message message = preMessage.Execute();
                    string base64proper = message.Payload.Parts[0].Body.Data;
                    base64proper = base64proper.Replace('-', '+');
                    base64proper = base64proper.Replace('_', '/');

                    byte[] data = Convert.FromBase64String(base64proper);
                    string rawData = Encoding.UTF8.GetString(data);
                    string posSender = "";
                    foreach (MessagePartHeader header in message.Payload.Headers)
                    {
                        if (header.Name == "From")
                        {
                            posSender = header.Value;
                            break;
                        }
                    }
                    List<string> companies = db.getCompanyNames();
                    string sender = "Unknown";

                    foreach (string company_name in companies)
                    {
                        Regex regex = new Regex(company_name.ToLower());
                        if (regex.IsMatch(posSender.ToLower()) || regex.IsMatch(rawData.ToLower()))
                        {
                            sender = company_name;
                        }
                    }

                    long? internalDate = message.InternalDate; //Put in database
                    
                    db.newEmail(messageID.Id, sender, internalDate, categoriseEmail(rawData));
                    Console.WriteLine("");
                }
            }
        }

        private static Dictionary<string, int> words = new Dictionary<string, int>
        {
            ["telephone"] = 0,
            ["video"] = 0,
            ["aptitude"] = 0,
            ["inteview"] = 0,
            ["unsuccessful"] = 0,
            ["sorry"] = 0,
            ["offer"] = 0,
            ["congratulate"] = 0,
            ["congratulations"] = 0,
            ["test"] = 0
        };

        public static string categoriseEmail(string rawEmail)
        {
            string[] rawWords = rawEmail.Split(" ");
            foreach (string word in rawWords)
            {
                if (words.Keys.Contains(word))
                {
                    words[word] = (words[word]+1);
                }
            }

            string commonest = "";
            int count = 0;
            foreach (string key in words.Keys)
            {
                if (count < words[key])
                {
                    count = words[key];
                    commonest = key;
                }
            }

            string result = "Unknown";
            switch (commonest)
            {
                case "telephone":
                    result = "Telephone Interview";
                    break;
                case "video":
                    result = "Video Interview";
                    break;
                case "interview":
                    result = "Interview";
                    break;
                case "unsuccessful":
                    result = "Rejected";
                    break;
                case "sorry":
                    result = "Rejected";
                    break;
                case "offer":
                    result = "Offer Made";
                    break;
                case "congratulations":
                    result = "Offer Made";
                    break;
                case "congratulate":
                    result = "Offer Made";
                    break;
                case "test":
                    result = "Test";
                    break;
                case "aptitude test":
                    result = "Aptitude Test";
                    break;
                default:
                    result = "Unknown";
                    break;
            }

            return result;
        }
        
    }
}