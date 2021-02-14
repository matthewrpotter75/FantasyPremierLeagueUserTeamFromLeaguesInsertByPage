using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Data.SqlClient;

namespace FantasyPremierLeagueUserTeams
{
    public class FantasyPremierLeagueAPIPick
    {
        public static void GetUserTeamPickDataJson(int userTeamId, UserTeamPicks userTeamPicksInsert, UserTeamPickAutomaticSubs userTeamPickAutomaticSubsInsert, SqlConnection db)
        {
            int gameweekId;

            //Logger.Out("GetUserTeamPickDataJson: Gameweek " + Convert.ToString(gameweekId) + " - starting");
            //var urlUserTeamPicks = "";
            string userTeamPicksUrl = ConfigSettings.ReadSetting("userTeamPicksUrl");

            try
            {
                if (Globals.maxGWFromPicksForUserTeamId == 0 && Globals.startGameweekId > 0)
                {
                    Globals.maxGWFromPicksForUserTeamId = Globals.startGameweekId;
                }
                else
                {
                    Globals.maxGWFromPicksForUserTeamId += 1;
                }

                for (gameweekId = Globals.maxGWFromPicksForUserTeamId; gameweekId <= Globals.actualGameweek; gameweekId++)
                {
                    GetUserTeamPickJson(userTeamId, gameweekId, userTeamPicksUrl, userTeamPicksInsert, userTeamPickAutomaticSubsInsert, db);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("GetUserTeamPickDataJson data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
                //throw new Exception("GetUserTeamPickDataJson data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
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

        public static void GetUserTeamPickJson(int userTeamId, int gameweekId, string urlUserTeamPicks, UserTeamPicks userTeamPicksInsert, UserTeamPickAutomaticSubs userTeamPickAutomaticSubsInsert, SqlConnection db)
        {
            try
            {
                //Process UserTeamPick and UserTeamPickAutomaticSub
                if (gameweekId >= Globals.maxGWFromPicksForUserTeamId)
                {
                    UserTeamPickRepository userTeamPickRepository = new UserTeamPickRepository();

                    urlUserTeamPicks = string.Format(urlUserTeamPicks, userTeamId, gameweekId);

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
                            //Load UserTeamPick data
                            //int userTeamPickIds_userteamid = 0;
                            //int userTeamPickIds_gameweekid = 0;

                            //UserTeamPickId userTeamPickId = new UserTeamPickId();
                            //userTeamPickId.userteamid = userTeamId;
                            //userTeamPickId.gameweekid = gameweekId;

                            List<UserTeamPickId> userTeamPickIds = userTeamPickRepository.GetAllUserTeamPickIdsForUserTeamIdAndGameweekId(userTeamId, gameweekId, db);

                            //if (userTeamPickIds.Count > 0)
                            //{
                            //    userTeamPickIds_userteamid = userTeamPickIds[0].userteamid;
                            //    userTeamPickIds_gameweekid = userTeamPickIds[0].gameweekid;
                            //}

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

                                if (!userTeamPickIds.Contains(userTeamPickId) && !userTeamPicksInsert.Contains(userTeamPick))
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
                        GetUserTeamPickAutomaticSubJson(userTeamId, gameweekId, userTeamPickAutomaticSubsInsert, userTeamPickData, db);
                    }
                }
                //Logger.Out("GetUserTeamPickDataJson: Gameweek " + Convert.ToString(gameweekId) + " - completed");
            }
            catch (Exception ex)
            {
                Logger.Error("GetUserTeamPickJson data exception (UserTeamId:" + userTeamId.ToString() + "/GameweekId:" + gameweekId.ToString() + "): " + ex.Message);
                Logger.Error("GetUserTeamPickJson data exception (UserTeamId:" + userTeamId.ToString() + "/GameweekId:" + gameweekId.ToString() + "): skipping userteam/gameweek");
                //throw new Exception("GetUserTeamPickJson data exception (UserTeamId: " + userTeamId.ToString() + "): " + ex.Message);
                //GetUserTeamPickJson(userTeamId, gameweekId, urlUserTeamPicks, maxGWFromPicksForUserTeamId, userTeamPicksInsert, userTeamPickAutomaticSubsInsert, db);
                //if (gameweekId + 1 < Globals.actualGameweek)
                //{
                //    gameweekId++;
                //    GetUserTeamPickJson(userTeamId, gameweekId, urlUserTeamPicks, maxGWFromPicksForUserTeamId, userTeamPicksInsert, userTeamPickAutomaticSubsInsert, db);
                //}
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
                    if (!UserTeamPickAutomaticSubIds.Contains(userTeamPickAutomaticSub.element_in) && !userTeamPickAutomaticSubsInsert.Contains(userTeamPickAutomaticSub))
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
                //GetUserTeamPickAutomaticSubJson(userTeamId, gameweekId, userTeamPickAutomaticSubsInsert, userTeamPickData, db);
            }
        }
    }
}