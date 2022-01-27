using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using log4net.Config;
using System.Linq;
using CommandLine.Utility;

namespace FantasyPremierLeagueUserTeams
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            //int pageId;
            int pageCnt;
            int leagueId = 314;

            UserTeams userTeamsUpdateInsert = new UserTeams();
            UserTeamPicks userTeamPicksInsert = new UserTeamPicks();
            UserTeamPickAutomaticSubs userTeamPickAutomaticSubsInsert = new UserTeamPickAutomaticSubs();
            UserTeamGameweekHistories userTeamGameweekHistoriesInsert = new UserTeamGameweekHistories();
            UserTeamChips userTeamChipsInsert = new UserTeamChips();
            UserTeamTransferHistoryData userTeamTransferHistoriesInsert = new UserTeamTransferHistoryData();
            UserTeamSeasons userTeamSeasonsInsert = new UserTeamSeasons();
            UserTeamClassicLeagues userTeamClassicLeaguesInsert = new UserTeamClassicLeagues();
            UserTeamH2hLeagues userTeamH2hLeaguesInsert = new UserTeamH2hLeagues();

            UserTeamCupMatches userTeamCupInsert = new UserTeamCupMatches();

            UserTeamRepository userTeamRepository = new UserTeamRepository();

            SqlConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString);
            //SqlConnection dbDW = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueDW"].ConnectionString);

            try
            {
                Logger.Out("Starting...");
                Logger.Out("");

                int pageId;
                Globals.PageId = 1;

                int insertInterval;
                Globals.InsertInterval = 1;

                bool test = true;

                //Arguments commandLine = new Arguments(args);
                //if (commandLine["-t"] != null)
                //{
                //    args[] = {"1 1" };
                //}

                List<string> allArgs = args.ToList<string>();

                if (args.Length == 0)
                {
                    //System.Console.WriteLine("No arguments passed into program");
                    Logger.Out("No arguments passed into program");
                }
                else
                {
                    test = int.TryParse(args[0], out pageId);

                    if (test == false)
                    {
                        Logger.Out("Argument[0] passed into program doesnn't exist or was not integer");
                        return;
                    }
                    else
                    {
                        Globals.PageId = pageId;
                    }

                    test = int.TryParse(args[1], out insertInterval);

                    if (test == false)
                    {
                        //System.Console.WriteLine("Arguments passed into program were not integer");
                        Logger.Out("Argument[1] InsertInterval passed into program doesnn't exist or were not integer");
                        return;
                    }
                    else
                    {
                        Globals.InsertInterval = insertInterval;
                    }
                }

                Logger.Out("Starting UserTeams data load");
                Logger.Out("");

                string userTeamUrl = ConfigSettings.ReadSetting("userTeamURL");
                string userTeamLeaguesUrl = ConfigSettings.ReadSetting("userTeamLeaguesURL");

                using (db)
                {
                    db.Open();

                    bool has_next = true;

                    Globals.UserTeamRetries = 0;
                    pageCnt = 0;

                    List<int> userTeamIds = userTeamRepository.GetAllUserTeamIds(db);

					Globals.ApiCalls = 0;
					Globals.ApiPageCalls = 0;
					Globals.ApiUserTeamCalls = 0;
					Globals.ApiUserTeamHistoryCalls = 0;
					Globals.ApiUserTeamTransferHistoryCalls = 0;
					Globals.ApiUserTeamPickCalls = 0;
					Globals.UserTeamInsertCount = 0;

                    while (has_next == true)
                    {
                        SetLatestGameweek("UserTeamGameweekHistory");
                        SetActualGameweek("UserTeamGameweekPick");

                        pageCnt += 1;

                        if (db.State == ConnectionState.Closed)
                        {
                            db.ConnectionString = ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString;
                            db.Open();
                            Logger.Error("Program Info (LeagueId: " + leagueId.ToString() + ", PageId:" + Globals.PageId.ToString() + "): Reopening closed db connection");
                        }

                        has_next = FantasyPremierLeagueLeaguesAPIClient.GetLeagueDataJson(leagueId, userTeamIds, userTeamLeaguesUrl, userTeamUrl, userTeamsUpdateInsert, userTeamCupInsert, userTeamClassicLeaguesInsert, userTeamH2hLeaguesInsert, userTeamGameweekHistoriesInsert, userTeamPicksInsert, userTeamPickAutomaticSubsInsert, userTeamChipsInsert, userTeamTransferHistoriesInsert, userTeamSeasonsInsert, db);

                        if (pageCnt >= Globals.InsertInterval)
                        {
                            WriteToDB(Globals.PageId, userTeamsUpdateInsert, userTeamCupInsert, userTeamClassicLeaguesInsert, userTeamH2hLeaguesInsert, userTeamGameweekHistoriesInsert, userTeamPicksInsert, userTeamPickAutomaticSubsInsert, userTeamChipsInsert, userTeamTransferHistoriesInsert, userTeamSeasonsInsert, db);
							
							Globals.ApiCalls = 0;
							Globals.ApiPageCalls = 0;
							Globals.ApiUserTeamCalls = 0;
							Globals.ApiUserTeamHistoryCalls = 0;
							Globals.ApiUserTeamTransferHistoryCalls = 0;
							Globals.ApiUserTeamPickCalls = 0;
							Globals.UserTeamInsertCount = 0;
							
                            pageCnt = 0;
                        }

                        Globals.PageId += 1;
                    }

                    WriteToDB(Globals.PageId, userTeamsUpdateInsert, userTeamCupInsert, userTeamClassicLeaguesInsert, userTeamH2hLeaguesInsert, userTeamGameweekHistoriesInsert, userTeamPicksInsert, userTeamPickAutomaticSubsInsert, userTeamChipsInsert, userTeamTransferHistoriesInsert, userTeamSeasonsInsert, db);

                    db.Close();

                }

                Logger.Out("UserTeams data load complete");
                Logger.Out("");

                Logger.Out("Finished!!!");

                //// Wait for user input - keep the program running
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Logger.Error("Program Exception Info (LeagueId: " + leagueId.ToString() + ", PageId:" + Globals.PageId.ToString() + "):" + ex.Message);
                //Logger.Error(userTeamName + " caused error!!!");

                if (db.State == ConnectionState.Closed)
                {
                    db.ConnectionString = ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString;
                    db.Open();
                    Logger.Error("Program Exception Info (LeagueId: " + leagueId.ToString() + ", PageId:" + Globals.PageId.ToString() + "): Reopening closed db connection");
                }

                //If error is thrown from sub program write existing records to the DB
                WriteToDB(Globals.PageId, userTeamsUpdateInsert, userTeamCupInsert, userTeamClassicLeaguesInsert, userTeamH2hLeaguesInsert, userTeamGameweekHistoriesInsert, userTeamPicksInsert, userTeamPickAutomaticSubsInsert, userTeamChipsInsert, userTeamTransferHistoriesInsert, userTeamSeasonsInsert, db);

                //db.Close();

                //throw ex;
            }
        }

        public static void WriteToDB(int pageId, UserTeams userTeamsUpdateInsert, UserTeamCupMatches userTeamCupInsert, UserTeamClassicLeagues userTeamClassicLeaguesInsert, UserTeamH2hLeagues userTeamH2hLeaguesInsert, UserTeamGameweekHistories userTeamGameweekHistoriesInsert, UserTeamPicks userTeamPicksInsert, UserTeamPickAutomaticSubs userTeamPickAutomaticSubsInsert, UserTeamChips userTeamChipsInsert, UserTeamTransferHistoryData userTeamTransferHistoriesInsert, UserTeamSeasons userTeamSeasonsInsert, SqlConnection db)
        {
            try
            {
                Globals.UserTeamUpdateRowsInserted = 0;
                Globals.UserTeamPickRowsInserted = 0;
                Globals.UserTeamPickAutomaticSubsRowsInserted = 0;
                Globals.UserTeamGameweekHistoryRowsInserted = 0;
                Globals.UserTeamChipRowsInserted = 0;
                Globals.UserTeamTransferHistoryRowsInserted = 0;
                Globals.UserTeamSeasonRowsInserted = 0;
                Globals.UserTeamClassicLeagueRowsInserted = 0;
                Globals.UserTeamH2hLeagueRowsInserted = 0;
                Globals.UserTeamCupRowsInserted = 0;

                Logger.Out("");

                WriteUserTeamToDB(Globals.PageId, userTeamsUpdateInsert, db);
                WriteUserTeamChipToDB(Globals.PageId, userTeamChipsInsert, db);
                WriteUserTeamGameweekHistoryToDB(Globals.PageId, userTeamGameweekHistoriesInsert, db);
                WriteUserTeamPicksToDB(Globals.PageId, userTeamPicksInsert, db);
                WriteUserTeamPickAutomaticSubsToDB(Globals.PageId, userTeamPickAutomaticSubsInsert, db);
                WriteUserTeamTransferHistoryToDB(Globals.PageId, userTeamTransferHistoriesInsert, db);
                WriteUserTeamSeasonToDB(Globals.PageId, userTeamSeasonsInsert, db);
                WriteUserTeamClassicLeagueToDB(Globals.PageId, userTeamClassicLeaguesInsert, db);
                WriteUserTeamH2hLeagueToDB(Globals.PageId, userTeamH2hLeaguesInsert, db);
                WriteUserTeamCupToDB(Globals.PageId, userTeamCupInsert, db);

                Logger.Out("");
                Logger.Out("UserTeam: " + Convert.ToString(Globals.UserTeamInsertCount) + " rows inserted");
                Logger.Out("UserTeamCup: " + Convert.ToString(Globals.UserTeamCupRowsInserted) + " rows inserted");
                Logger.Out("UserTeamUpdates: " + Convert.ToString(Globals.UserTeamUpdateRowsInserted) + " rows inserted");
                Logger.Out("UserTeamSeason: " + Convert.ToString(Globals.UserTeamSeasonRowsInserted) + " rows inserted");
                Logger.Out("UserTeamClassicLeague: " + Convert.ToString(Globals.UserTeamClassicLeagueRowsInserted) + " rows inserted");
                Logger.Out("UserTeamH2hLeague: " + Convert.ToString(Globals.UserTeamH2hLeagueRowsInserted) + " rows inserted");
                Logger.Out("UserTeamGameweekHistory: " + Convert.ToString(Globals.UserTeamGameweekHistoryRowsInserted) + " rows inserted");
                Logger.Out("UserTeamTransferHistory: " + Convert.ToString(Globals.UserTeamTransferHistoryRowsInserted) + " rows inserted");
                Logger.Out("UserTeamChip: " + Convert.ToString(Globals.UserTeamChipRowsInserted) + " rows inserted");
                Logger.Out("UserTeamPick: " + Convert.ToString(Globals.UserTeamPickRowsInserted) + " rows inserted");
                Logger.Out("UserTeamPickAutomaticSub: " + Convert.ToString(Globals.UserTeamPickAutomaticSubsRowsInserted) + " rows inserted");
                Logger.Out("");
                Logger.Out("API Page calls: " + Convert.ToString(Globals.ApiPageCalls));
                Logger.Out("API UserTeam calls: " + Convert.ToString(Globals.ApiUserTeamCalls));
                Logger.Out("API UserTeamHistory calls: " + Convert.ToString(Globals.ApiUserTeamHistoryCalls));
                Logger.Out("API UserTeamTransferHistory calls: " + Convert.ToString(Globals.ApiUserTeamTransferHistoryCalls));
                Logger.Out("API UserTeamPick calls: " + Convert.ToString(Globals.ApiUserTeamPickCalls));
                Logger.Out("API calls: " + Convert.ToString(Globals.ApiCalls));
                Logger.Out("");
            }
            catch (Exception ex)
            {
                Logger.Error("Program WriteToDB error: " + ex.Message);
                //WriteToDB(userTeamRowsInserted, userTeamSeasonRowsInserted, userTeamInsert, userTeamSeasonsInsert, db);
            }
        }

        public static void WriteUserTeamToDB(int pageId, UserTeams userTeamsUpdateInsert, SqlConnection db)
        {
            try
            {
                UserTeamRepository userTeamRepository = new UserTeamRepository();

                //Write UserTeamUpdate to the db
                Globals.UserTeamUpdateRowsInserted = userTeamRepository.UpdateUserTeam(userTeamsUpdateInsert, db);
                Globals.UserTeamUpdateRowsInserted = userTeamRepository.UpdateUserTeamSQL(db);
                userTeamRepository.TruncateUserTeamUpdateStaging(db);
                Logger.Out("UserTeamUpdates bulk insert complete (PageId: " + Convert.ToString(pageId) + ")");

                userTeamsUpdateInsert.Clear();
            }
            catch (Exception ex)
            {
                Logger.Error("Program WriteUserTeamToDB error: " + ex.Message);
            }
        }

        public static void WriteUserTeamChipToDB(int pageId, UserTeamChips userTeamChipsInsert, SqlConnection db)
        {
            try
            {
                UserTeamChipRepository userTeamChipRepository = new UserTeamChipRepository();

                //Write UserTeamChip to the db
                Globals.UserTeamChipRowsInserted = userTeamChipRepository.InsertUserTeamChip(userTeamChipsInsert, db);
                Logger.Out("UserTeamChip bulk insert complete (PageId: " + Convert.ToString(pageId) + ")");

                userTeamChipsInsert.Clear();
            }
            catch (Exception ex)
            {
                Logger.Error("Program WriteUserTeamChipToDB error: " + ex.Message);
            }
        }

        public static void WriteUserTeamGameweekHistoryToDB(int pageId, UserTeamGameweekHistories userTeamGameweekHistoriesInsert, SqlConnection db)
        {
            try
            {
                UserTeamGameweekHistoryRepository userTeamGameweekHistoryRepository = new UserTeamGameweekHistoryRepository();

                //Write UserTeamGameweekHistory to the db
                Globals.UserTeamGameweekHistoryRowsInserted = userTeamGameweekHistoryRepository.InsertUserTeamGameweekHistories(userTeamGameweekHistoriesInsert, db);
                Logger.Out("UserTeamGameweekHistory bulk insert complete (PageId: " + Convert.ToString(pageId) + ")");

                userTeamGameweekHistoriesInsert.Clear();
            }
            catch (Exception ex)
            {
                Logger.Error("Program WriteUserTeamGameweekHistoryToDB error: " + ex.Message);
            }
        }

        public static void WriteUserTeamPicksToDB(int pageId, UserTeamPicks userTeamPicksInsert, SqlConnection db)
        {
            try
            {
                UserTeamPickRepository userTeamPickRepository = new UserTeamPickRepository();

                //Write UserTeamPick to the db
                Globals.UserTeamPickRowsInserted = userTeamPickRepository.InsertUserTeamPick(userTeamPicksInsert, db);
                Logger.Out("UserTeamPick bulk insert complete (PageId: " + Convert.ToString(pageId) + ")");

                userTeamPicksInsert.Clear();
            }
            catch (Exception ex)
            {
                Logger.Error("Program WriteUserTeamPickToDB error: " + ex.Message);
                //WriteToDB(userTeamRowsInserted, userTeamSeasonRowsInserted, userTeamInsert, userTeamSeasonsInsert, db);
            }
        }

        public static void WriteUserTeamPickAutomaticSubsToDB(int pageId, UserTeamPickAutomaticSubs userTeamPickAutomaticSubsInsert, SqlConnection db)
        {
            try
            {
                UserTeamPickAutomaticSubRepository userTeamPickAutomaticSubRepository = new UserTeamPickAutomaticSubRepository();

                //Write UserTeamPickAutomaticSub to the db
                Globals.UserTeamPickAutomaticSubsRowsInserted = userTeamPickAutomaticSubRepository.InsertUserTeamPickAutomaticSubs(userTeamPickAutomaticSubsInsert, db);
                Logger.Out("UserTeamPickAutomaticSub bulk insert complete (PageId: " + Convert.ToString(pageId) + ")");

                userTeamPickAutomaticSubsInsert.Clear();
            }
            catch (Exception ex)
            {
                Logger.Error("Program WriteUserTeamPickAutomaticSubsToDB error: " + ex.Message);
            }
        }

        public static void WriteUserTeamTransferHistoryToDB(int pageId, UserTeamTransferHistoryData userTeamTransferHistoriesInsert, SqlConnection db)
        {
            try
            {
                UserTeamTransferHistoryRepository userTeamTransferHistoryRepository = new UserTeamTransferHistoryRepository();

                //Write UserTeamTransferHistory to the db
                Globals.UserTeamTransferHistoryRowsInserted = userTeamTransferHistoryRepository.InsertUserTeamTransferHistories(userTeamTransferHistoriesInsert, db);
                Logger.Out("UserTeamTransferHistory bulk insert complete (PageId: " + Convert.ToString(pageId) + ")");

                userTeamTransferHistoriesInsert.Clear();
            }
            catch (Exception ex)
            {
                Logger.Error("Program WriteUserTeamTransferHistoryToDB error: " + ex.Message);
            }
        }

        public static void WriteUserTeamSeasonToDB(int pageId, UserTeamSeasons userTeamSeasonsInsert, SqlConnection db)
        {
            try
            {
                UserTeamSeasonRepository userTeamSeasonRepository = new UserTeamSeasonRepository();

                //Write UserTeamSeason to the db
                Globals.UserTeamSeasonRowsInserted = userTeamSeasonRepository.InsertUserTeamSeason(userTeamSeasonsInsert, db);
                Logger.Out("UserTeamSeason bulk insert complete (PageId: " + Convert.ToString(pageId) + ")");

                userTeamSeasonsInsert.Clear();
            }
            catch (Exception ex)
            {
                Logger.Error("Program WriteUserTeamSeasonToDB error: " + ex.Message);
            }
        }

        public static void WriteUserTeamClassicLeagueToDB(int pageId, UserTeamClassicLeagues userTeamClassicLeaguesInsert, SqlConnection db)
        {
            try
            {
                UserTeamClassicLeagueRepository userTeamClassicLeagueRepository = new UserTeamClassicLeagueRepository();

                //Write UserTeamClassicLeague to the db
                Globals.UserTeamClassicLeagueRowsInserted = userTeamClassicLeagueRepository.InsertUserTeamClassicLeague(userTeamClassicLeaguesInsert, db);
                Logger.Out("UserTeamClassicLeague bulk insert complete (PageId: " + Convert.ToString(pageId) + ")");

                userTeamClassicLeaguesInsert.Clear();
            }
            catch (Exception ex)
            {
                Logger.Error("Program WriteUserTeamClassicLeagueToDB error: " + ex.Message);
            }
        }

        public static void WriteUserTeamH2hLeagueToDB(int pageId, UserTeamH2hLeagues userTeamH2hLeaguesInsert, SqlConnection db)
        {
            try
            {
                UserTeamH2hLeagueRepository userTeamH2hLeagueRepository = new UserTeamH2hLeagueRepository();

                //Write UserTeamH2hLeague to the db
                Globals.UserTeamH2hLeagueRowsInserted = userTeamH2hLeagueRepository.InsertUserTeamH2hLeague(userTeamH2hLeaguesInsert, db);
                Logger.Out("UserTeamH2hLeague bulk insert complete (PageId: " + Convert.ToString(pageId) + ")");

                userTeamH2hLeaguesInsert.Clear();
            }
            catch (Exception ex)
            {
                Logger.Error("Program WriteUserTeamH2hLeagueToDB error: " + ex.Message);
            }
        }

        public static void WriteUserTeamCupToDB(int pageId, UserTeamCupMatches userTeamCupInsert, SqlConnection db)
        {
            try
            {
                UserTeamCupRepository userTeamCupRepository = new UserTeamCupRepository();

                //Write UserTeamCup to the db
                Globals.UserTeamCupRowsInserted = userTeamCupRepository.InsertUserTeamCup(userTeamCupInsert, db);
                Logger.Out("UserTeamCup bulk insert complete (PageId: " + Convert.ToString(pageId) + ")");

                userTeamCupInsert.Clear();
            }
            catch (Exception ex)
            {
                Logger.Error("Program WriteUserTeamCupToDB error: " + ex.Message);
            }
        }

        private static void SetLatestGameweek(string entity)
        {
            int latestGameweek;
            UserTeamRepository userTeamRepository = new UserTeamRepository();

            using (SqlConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueDW"].ConnectionString))
            {
                db.Open();

                latestGameweek = userTeamRepository.GetLatestGameweekId(db, entity);

                Globals.LatestGameweek = latestGameweek;

                db.Close();
            }
        }

        private static void SetActualGameweek(string entity)
        {
            int latestGameweek;
            UserTeamRepository userTeamRepository = new UserTeamRepository();

            using (SqlConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueDW"].ConnectionString))
            {
                db.Open();

                latestGameweek = userTeamRepository.GetLatestGameweekId(db, entity);

                Globals.ActualGameweek = latestGameweek;

                db.Close();
            }
        }
    }
}