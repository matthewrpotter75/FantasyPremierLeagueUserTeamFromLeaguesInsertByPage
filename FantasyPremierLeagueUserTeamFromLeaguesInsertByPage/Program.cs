using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using log4net.Config;

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

            try
            {
                Logger.Out("Starting...");
                Logger.Out("");

                int pageId;
                Globals.PageId = 1;
                bool test = true;

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
                        //System.Console.WriteLine("Arguments passed into program were not integer");
                        Logger.Out("Arguments passed into program were not integer");
                        return;
                    }
                    else
                    {
                        Globals.PageId = pageId;
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

                        if (pageCnt >= 50)
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
                Logger.Error(ex.Message);
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

                throw ex;
            }
        }

        public static void WriteToDB(int pageId, UserTeams userTeamsUpdateInsert, UserTeamCupMatches userTeamCupInsert, UserTeamClassicLeagues userTeamClassicLeaguesInsert, UserTeamH2hLeagues userTeamH2hLeaguesInsert, UserTeamGameweekHistories userTeamGameweekHistoriesInsert, UserTeamPicks userTeamPicksInsert, UserTeamPickAutomaticSubs userTeamPickAutomaticSubsInsert, UserTeamChips userTeamChipsInsert, UserTeamTransferHistoryData userTeamTransferHistoriesInsert, UserTeamSeasons userTeamSeasonsInsert, SqlConnection db)
        {
            try
            {
                int userTeamUpdateRowsInserted = 0;
                int userTeamPickRowsInserted = 0;
                int userTeamPickAutomaticSubsRowsInserted = 0;
                int userTeamGameweekHistoryRowsInserted = 0;
                int userTeamChipRowsInserted = 0;
                int userTeamTransferHistoryRowsInserted = 0;
                int userTeamSeasonRowsInserted = 0;
                int userTeamClassicLeagueRowsInserted = 0;
                int userTeamH2hLeagueRowsInserted = 0;
                int userTeamCupRowsInserted = 0;

                UserTeamRepository userTeamRepository = new UserTeamRepository();
                UserTeamCupRepository userTeamCupRepository = new UserTeamCupRepository();
                UserTeamPickRepository userTeamPickRepository = new UserTeamPickRepository();
                UserTeamPickAutomaticSubRepository userTeamPickAutomaticSubRepository = new UserTeamPickAutomaticSubRepository();
                UserTeamGameweekHistoryRepository userTeamGameweekHistoryRepository = new UserTeamGameweekHistoryRepository();
                UserTeamChipRepository userTeamChipRepository = new UserTeamChipRepository();
                UserTeamTransferHistoryRepository userTeamTransferHistoryRepository = new UserTeamTransferHistoryRepository();
                UserTeamSeasonRepository userTeamSeasonRepository = new UserTeamSeasonRepository();
                UserTeamClassicLeagueRepository userTeamClassicLeagueRepository = new UserTeamClassicLeagueRepository();
                UserTeamH2hLeagueRepository userTeamH2hLeagueRepository = new UserTeamH2hLeagueRepository();

                Logger.Out("");

                //Write UserTeamUpdate to the db
                userTeamUpdateRowsInserted = userTeamRepository.UpdateUserTeam(userTeamsUpdateInsert, db);
                userTeamUpdateRowsInserted = userTeamRepository.UpdateUserTeamSQL(db);
                userTeamRepository.TrncateUserTeamUpdateStaging(db);
                Logger.Out("UserTeamUpdates bulk insert complete (PageId: " + Convert.ToString(pageId) + ")");

                //Write UserTeamChip to the db
                userTeamChipRowsInserted = userTeamChipRepository.InsertUserTeamChip(userTeamChipsInsert, db);
                Logger.Out("UserTeamChip bulk insert complete (PageId: " + Convert.ToString(pageId) + ")");

                //Write UserTeamGameweekHistory to the db
                userTeamGameweekHistoryRowsInserted = userTeamGameweekHistoryRepository.InsertUserTeamGameweekHistories(userTeamGameweekHistoriesInsert, db);
                Logger.Out("UserTeamGameweekHistory bulk insert complete (PageId: " + Convert.ToString(pageId) + ")");

                //Write UserTeamPick to the db
                userTeamPickRowsInserted = userTeamPickRepository.InsertUserTeamPick(userTeamPicksInsert, db);
                Logger.Out("UserTeamPick bulk insert complete (PageId: " + Convert.ToString(pageId) + ")");

                //Write UserTeamPickAutomaticSub to the db
                userTeamPickAutomaticSubsRowsInserted = userTeamPickAutomaticSubRepository.InsertUserTeamPickAutomaticSubs(userTeamPickAutomaticSubsInsert, db);
                Logger.Out("UserTeamPickAutomaticSub bulk insert complete (PageId: " + Convert.ToString(pageId) + ")");

                //Write UserTeamTransferHistory to the db
                userTeamTransferHistoryRowsInserted = userTeamTransferHistoryRepository.InsertUserTeamTransferHistories(userTeamTransferHistoriesInsert, db);
                Logger.Out("UserTeamTransferHistory bulk insert complete (PageId: " + Convert.ToString(pageId) + ")");

                //Write UserTeamSeason to the db
                userTeamSeasonRowsInserted = userTeamSeasonRepository.InsertUserTeamSeason(userTeamSeasonsInsert, db);
                Logger.Out("UserTeamSeason bulk insert complete (PageId: " + Convert.ToString(pageId) + ")");

                //Write UserTeamClassicLeague to the db
                userTeamClassicLeagueRowsInserted = userTeamClassicLeagueRepository.InsertUserTeamClassicLeague(userTeamClassicLeaguesInsert, db);
                Logger.Out("UserTeamClassicLeague bulk insert complete (PageId: " + Convert.ToString(pageId) + ")");

                //Write UserTeamH2hLeague to the db
                userTeamH2hLeagueRowsInserted = userTeamH2hLeagueRepository.InsertUserTeamH2hLeague(userTeamH2hLeaguesInsert, db);
                Logger.Out("UserTeamH2hLeague bulk insert complete (PageId: " + Convert.ToString(pageId) + ")");

                //Write UserTeamCup to the db
                userTeamCupRowsInserted = userTeamCupRepository.InsertUserTeamCup(userTeamCupInsert, db);
                Logger.Out("UserTeamCup bulk insert complete (PageId: " + Convert.ToString(pageId) + ")");

                Logger.Out("");
                Logger.Out("UserTeam: " + Convert.ToString(Globals.UserTeamInsertCount) + " rows inserted");
                Logger.Out("UserTeamCup: " + Convert.ToString(userTeamCupRowsInserted) + " rows inserted");
                Logger.Out("UserTeamUpdates: " + Convert.ToString(userTeamUpdateRowsInserted) + " rows inserted");
                Logger.Out("UserTeamSeason: " + Convert.ToString(userTeamSeasonRowsInserted) + " rows inserted");
                Logger.Out("UserTeamClassicLeague: " + Convert.ToString(userTeamClassicLeagueRowsInserted) + " rows inserted");
                Logger.Out("UserTeamH2hLeague: " + Convert.ToString(userTeamH2hLeagueRowsInserted) + " rows inserted");
                Logger.Out("UserTeamGameweekHistory: " + Convert.ToString(userTeamGameweekHistoryRowsInserted) + " rows inserted");
                Logger.Out("UserTeamTransferHistory: " + Convert.ToString(userTeamTransferHistoryRowsInserted) + " rows inserted");
                Logger.Out("UserTeamChip: " + Convert.ToString(userTeamChipRowsInserted) + " rows inserted");
                Logger.Out("UserTeamPick: " + Convert.ToString(userTeamPickRowsInserted) + " rows inserted");
                Logger.Out("UserTeamPickAutomaticSub: " + Convert.ToString(userTeamPickAutomaticSubsRowsInserted) + " rows inserted");
                Logger.Out("");

                Logger.Out("API Page calls: " + Convert.ToString(Globals.ApiPageCalls));
                Logger.Out("API UserTeam calls: " + Convert.ToString(Globals.ApiUserTeamCalls));
                Logger.Out("API UserTeamHistory calls: " + Convert.ToString(Globals.ApiUserTeamHistoryCalls));
                Logger.Out("API UserTeamTransferHistory calls: " + Convert.ToString(Globals.ApiUserTeamTransferHistoryCalls));
                Logger.Out("API UserTeamPick calls: " + Convert.ToString(Globals.ApiUserTeamPickCalls));
                Logger.Out("API calls: " + Convert.ToString(Globals.ApiCalls));
                Logger.Out("");

                userTeamsUpdateInsert.Clear();
                userTeamPicksInsert.Clear();
                userTeamPickAutomaticSubsInsert.Clear();
                userTeamGameweekHistoriesInsert.Clear();
                userTeamChipsInsert.Clear();
                userTeamTransferHistoriesInsert.Clear();
                userTeamSeasonsInsert.Clear();
                userTeamClassicLeaguesInsert.Clear();
                userTeamH2hLeaguesInsert.Clear();
                userTeamCupInsert.Clear();
            }
            catch (Exception ex)
            {
                Logger.Error("Program WriteToDB error: " + ex.Message);
                //WriteToDB(userTeamRowsInserted, userTeamSeasonRowsInserted, userTeamInsert, userTeamSeasonsInsert, db);
            }
        }

        private static void SetLatestGameweek(string entity)
        {
            int latestGameweek;
            UserTeamRepository userTeamRepository = new UserTeamRepository();

            using (SqlConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeague"].ConnectionString))
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

            using (SqlConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeague"].ConnectionString))
            {
                db.Open();

                latestGameweek = userTeamRepository.GetLatestGameweekId(db, entity);

                Globals.ActualGameweek = latestGameweek;

                db.Close();
            }
        }
    }
}