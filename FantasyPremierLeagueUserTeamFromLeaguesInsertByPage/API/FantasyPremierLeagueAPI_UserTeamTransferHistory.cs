using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Data.SqlClient;

namespace FantasyPremierLeagueUserTeams
{
    public class FantasyPremierLeagueAPITransferHistory
    {
        public static void GetUserTeamTransferHistoryDataJson(int userTeamId, UserTeamTransferHistoryData userTeamTransferHistoriesInsert, SqlConnection db)
        {
            try
            {
                string userTeamTransferUrl = ConfigSettings.ReadSetting("userTeamTransfersURL");

                userTeamTransferUrl = string.Format(userTeamTransferUrl, userTeamId);

                JsonSerializer serializer = new JsonSerializer() { Formatting = Formatting.None };
                
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                using (HttpClient client = new HttpClient())
                using (Stream s = client.GetStreamAsync(userTeamTransferUrl).Result)
                using (StreamReader sr = new StreamReader(s))
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    Globals.ApiCalls += 1;
                    Globals.ApiUserTeamTransferHistoryCalls += 1;

                    // read the json from a stream
                    // json size doesn't matter because only a small piece is read at a time from the HTTP request
                    var userTeamTransferHistoryData = serializer.Deserialize<UserTeamTransferHistoryData>(reader);

                    if (userTeamTransferHistoryData != null)
                    {
                        //UserTeamTransferHistoryData userTeamTransferHistory = (UserTeamTransferHistoryData)userTeamTransferHistoryData;
                        GetUserTeamTransferHistoryJson(userTeamId, userTeamTransferHistoriesInsert, userTeamTransferHistoryData, db);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("GetUserTeamTransferHistoryDataJson data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
                throw new Exception("GetUserTeamTransferHistoryDataJson data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
            }
        }

        public static void GetUserTeamTransferHistoryJson(int userTeamId, UserTeamTransferHistoryData userTeamTransferHistoriesInsert, UserTeamTransferHistoryData userTeamTransferHistoryData, SqlConnection db)
        {
            try
            {
                UserTeamTransferHistoryRepository userTeamTransferHistoryRepository = new UserTeamTransferHistoryRepository();

                //List<long> UserTeamTransferHistoryIds = userTeamTransferHistoryRepository.GetAllUserTeamTransferHistoryIdsForUserTeamId(userTeamId, db);

                long userTeamIdForKey;
                long elementinForKey;
                long elementoutForKey;

                UserTeamTransferHistoryData userTeamTransferHistorysUpdate = new UserTeamTransferHistoryData();

                foreach (UserTeamTransferHistory userTeamTransferHistory in userTeamTransferHistoryData)
                {
                    userTeamIdForKey = Convert.ToInt64(userTeamId) * 100000000;
                    elementinForKey = userTeamTransferHistory.element_in * 10000;
                    elementoutForKey = userTeamTransferHistory.element_out;
                    userTeamTransferHistory.userteamtransferhistoryid = userTeamIdForKey + elementinForKey + elementoutForKey;

                    //if (!UserTeamTransferHistoryIds.Contains(userTeamTransferHistory.userteamtransferhistoryid) && !userTeamTransferHistoriesInsert.Contains(userTeamTransferHistory))
                    if (!userTeamTransferHistoriesInsert.Contains(userTeamTransferHistory) && userTeamTransferHistory.@event > Globals.MaxGWFromTransferHistoryForUserTeamId)
                    {
                        userTeamTransferHistoriesInsert.Add(userTeamTransferHistory);
                    }
                    //else
                    //{
                    //    userTeamTransferHistoryRepository.UpdateUserTeamTransferHistory(userTeamTransferHistory);
                    //}
                }
            }
            catch (Exception ex)
            {
                Logger.Error("GetUserTeamTransferHistoryJson data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
                throw new Exception("GetUserTeamTransferHistoryJson data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
            }
        }
    }
}