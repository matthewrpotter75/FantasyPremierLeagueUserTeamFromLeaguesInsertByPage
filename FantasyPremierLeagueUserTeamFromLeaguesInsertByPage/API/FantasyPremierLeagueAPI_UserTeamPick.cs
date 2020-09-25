using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace FantasyPremierLeagueUserTeams
{
    public class FantasyPremierLeagueAPIPick
    {
        public static void GetUserTeamPickDataJson(int userTeamId, int maxGWFromPicksForUserTeamId, UserTeamPicks userTeamPicksInsert, UserTeamPickAutomaticSubs userTeamPickAutomaticSubsInsert, SqlConnection db)
        {
            //Logger.Out("GetUserTeamPickDataJson: Gameweek " + Convert.ToString(gameweekId) + " - starting");
            var urlUserTeamPicks = "";
            string userTeamPicksUrl = ConfigSettings.ReadSetting("userTeamPicksUrl");

            try
            {
                if (maxGWFromPicksForUserTeamId == 0)
                {
                    maxGWFromPicksForUserTeamId = Globals.startGameweekId;
                }

                for (int gameweekId = maxGWFromPicksForUserTeamId; gameweekId <= Globals.actualGameweek; gameweekId++)
                {
                    //Process UserTeamPick and UserTeamPickAutomaticSub
                    if (gameweekId >= maxGWFromPicksForUserTeamId)
                    {
                        UserTeamPickRepository userTeamPickRepository = new UserTeamPickRepository();

                        urlUserTeamPicks = string.Format(userTeamPicksUrl, userTeamId, gameweekId);

                        HttpClient clientUserTeamPicks = new HttpClient();
                        JsonSerializer serializerUserTeamPicks = new JsonSerializer();
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        using (Stream sUserTeamPicks = clientUserTeamPicks.GetStreamAsync(urlUserTeamPicks).Result)
                        using (StreamReader srUserTeamPicks = new StreamReader(sUserTeamPicks))
                        using (JsonReader readerUserTeamPicks = new JsonTextReader(srUserTeamPicks))
                        {
                            Globals.apiCalls += 1;
                            Globals.apiUserTeamPickCalls += 1;

                            // read the json from a stream
                            // json size doesn't matter because only a small piece is read at a time from the HTTP request
                            var userTeamPickData = serializerUserTeamPicks.Deserialize<UserTeamPickData>(readerUserTeamPicks);

                            if (userTeamPickData != null)
                            {
                                GetUserTeamPickJson(userTeamId, gameweekId, userTeamPicksInsert, userTeamPickData, db);
                                GetUserTeamPickAutomaticSubJson(userTeamId, gameweekId, userTeamPickAutomaticSubsInsert, userTeamPickData, db);
                            }
                        }
                        //Logger.Out("GetUserTeamPickDataJson: Gameweek " + Convert.ToString(gameweekId) + " - completed");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("GetUserTeamPickDataJson data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
                throw new Exception("GetUserTeamPickDataJson data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
            }
        }

        //public static void GetUserTeamPickDataJson(int userTeamId, int gameweekId, UserTeamPicks userTeamPicksInsert, UserTeamPickAutomaticSubs userTeamPickAutomaticSubsInsert, string userTeamPicksUrl, SqlConnection db)
        //{
        //    //Logger.Out("GetUserTeamPickDataJson: Gameweek " + Convert.ToString(gameweekId) + " - starting");

        //    var urlUserTeamPicks = "";

        //    try
        //    {
        //        UserTeamPickRepository userTeamPickRepository = new UserTeamPickRepository();

        //        urlUserTeamPicks = string.Format(userTeamPicksUrl, userTeamId, gameweekId);

        //        HttpClient clientUserTeamPicks = new HttpClient();
        //        JsonSerializer serializerUserTeamPicks = new JsonSerializer();
        //        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        //        //Logger.Out("Before using statements - url: " + urlUserTeamPicks);

        //        using (Stream sUserTeamPicks = clientUserTeamPicks.GetStreamAsync(urlUserTeamPicks).Result)
        //        using (StreamReader srUserTeamPicks = new StreamReader(sUserTeamPicks))
        //        using (JsonReader readerUserTeamPicks = new JsonTextReader(srUserTeamPicks))
        //        {
        //            Globals.apiCalls += 1;
        //            Globals.apiUserTeamPickCalls += 1;

        //            // read the json from a stream
        //            // json size doesn't matter because only a small piece is read at a time from the HTTP request
        //            var userTeamPickData = serializerUserTeamPicks.Deserialize<UserTeamPickData>(readerUserTeamPicks);

        //            if (userTeamPickData != null)
        //            {
        //                GetUserTeamPickJson(userTeamId, gameweekId, userTeamPicksInsert, userTeamPickData, db);
        //                GetUserTeamPickAutomaticSubJson(userTeamId, gameweekId, userTeamPickAutomaticSubsInsert, userTeamPickData, db);
        //            }
        //        }
        //        //Logger.Out("GetUserTeamPickDataJson: Gameweek " + Convert.ToString(gameweekId) + " - completed");
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error("GetUserTeamPickDataJson data exception (UserTeamId: " + userTeamId.ToString() + " GameweekId: " + gameweekId.ToString() + "): " + ex.Message);
        //        //throw new Exception("GetUserTeamPickDataJson data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
        //        GetUserTeamPickDataJson(userTeamId, gameweekId, userTeamPicksInsert, userTeamPickAutomaticSubsInsert, userTeamPicksUrl, db);
        //    }
        //}

        public static void GetUserTeamPickJson(int userTeamId, int gameweekId, UserTeamPicks userTeamPicksInsert, UserTeamPickData userTeamPickData, SqlConnection db)
        {
            try
            {
                //Logger.Out("GetUserTeamPickJson: Gameweek " + Convert.ToString(gameweekId) + " - starting");

                //Load UserTeamPick data
                UserTeamPickRepository userTeamPickRepository = new UserTeamPickRepository();

                int userTeamPickIds_userteamid = 0;
                int userTeamPickIds_gameweekid = 0;

                //UserTeamPickId userTeamPickId = new UserTeamPickId();
                //userTeamPickId.userteamid = userTeamId;
                //userTeamPickId.gameweekid = gameweekId;

                List<UserTeamPickId> userTeamPickIds = userTeamPickRepository.GetAllUserTeamPickIdsForUserTeamIdAndGameweekId(userTeamId, gameweekId, db);

                if (userTeamPickIds.Count > 0)
                {
                    userTeamPickIds_userteamid = userTeamPickIds[0].userteamid;
                    userTeamPickIds_gameweekid = userTeamPickIds[0].gameweekid;
                }

                //UserTeamPicks userTeamPicksUpdate = new UserTeamPicks();

                foreach (UserTeamPick userTeamPick in userTeamPickData.picks)
                {
                    UserTeamPickId userTeamPickId = new UserTeamPickId
                    (
                        userTeamId,
                        gameweekId,
                        userTeamPick.position
                    );

                    //needed if want to assign value from parent to add into db table
                    userTeamPick.userteamid = userTeamId;
                    userTeamPick.gameweekid = gameweekId;

                    if (!userTeamPickIds.Contains(userTeamPickId))
                    {
                        userTeamPicksInsert.Add(userTeamPick);
                    }
                    //else
                    //{
                    //    userTeamPickRepository.UpdateUserTeamPick(userTeamPick);
                    //}
                }
                //Logger.Out("GetUserTeamPickJson: Gameweek " + Convert.ToString(gameweekId) + " - completed");
            }
            catch (Exception ex)
            {
                Logger.Error("GetUserTeamPickJson data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
                //throw new Exception("GetUserTeamPickJson data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
                GetUserTeamPickJson(userTeamId, gameweekId, userTeamPicksInsert, userTeamPickData, db);
            }
        }

        public static void GetUserTeamPickAutomaticSubJson(int userTeamId, int gameweekId, UserTeamPickAutomaticSubs userTeamPickAutomaticSubsInsert, UserTeamPickData userTeamPickData, SqlConnection db)
        {
            try
            {
                //Logger.Out("GetUserTeamPickAutomaticSubJson: Gameweek " + Convert.ToString(gameweekId) + " - starting");

                //Load UserTeamPickAutomaticSub data
                UserTeamPickAutomaticSubRepository userTeamPickAutomaticSubRepository = new UserTeamPickAutomaticSubRepository();

                List<int> UserTeamPickAutomaticSubIds = userTeamPickAutomaticSubRepository.GetAllUserTeamPickAutomaticSubIdsForUserTeamIdAndGameweekId(userTeamId, gameweekId, db);

                foreach (UserTeamPickAutomaticSub userTeamPickAutomaticSub in userTeamPickData.automatic_subs)
                {
                    //needed if want to assign value from parent to add into db table
                    if (!UserTeamPickAutomaticSubIds.Contains(userTeamPickAutomaticSub.element_in))
                    {
                        userTeamPickAutomaticSubsInsert.Add(userTeamPickAutomaticSub);
                    }
                    //else
                    //{
                    //    userTeamPickAutomaticSubRepository.UpdateUserTeamPickAutomaticSub(userTeamPickAutomaticSub);
                    //}
                }
                //Logger.Out("GetUserTeamPickAutomaticSubJson: Gameweek " + Convert.ToString(gameweekId) + " - completed");
            }
            catch (Exception ex)
            {
                Logger.Error("GetUserTeamPickAutomaticSubJson data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
                //throw new Exception("GetUserTeamPickAutomaticSubJson data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
                GetUserTeamPickAutomaticSubJson(userTeamId, gameweekId, userTeamPickAutomaticSubsInsert, userTeamPickData, db);
            }
        }
    }
}