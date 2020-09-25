using System;
using System.Collections.Generic;
using System.Linq;
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

            int pageId = 0;

            UserTeamPicks userTeamPicksInsert = new UserTeamPicks();
            UserTeamPickAutomaticSubs userTeamPickAutomaticSubsInsert = new UserTeamPickAutomaticSubs();
            UserTeamGameweekHistories userTeamGameweekHistoriesInsert = new UserTeamGameweekHistories();
            UserTeamChips userTeamChipsInsert = new UserTeamChips();
            UserTeamTransferHistoryData userTeamTransferHistoriesInsert = new UserTeamTransferHistoryData();
            UserTeamSeasons userTeamSeasonsInsert = new UserTeamSeasons();
            UserTeamClassicLeagues userTeamClassicLeaguesInsert = new UserTeamClassicLeagues();
            UserTeamH2hLeagues userTeamH2hLeaguesInsert = new UserTeamH2hLeagues();

            UserTeamRepository userTeamRepository = new UserTeamRepository();
            UserTeamPickRepository userTeamPickRepository = new UserTeamPickRepository();
            UserTeamPickAutomaticSubRepository userTeamPickAutomaticSubRepository = new UserTeamPickAutomaticSubRepository();
            UserTeamGameweekHistoryRepository userTeamGameweekHistoryRepository = new UserTeamGameweekHistoryRepository();
            UserTeamChipRepository userTeamChipRepository = new UserTeamChipRepository();
            UserTeamTransferHistoryRepository userTeamTransferHistoryRepository = new UserTeamTransferHistoryRepository();
            UserTeamSeasonRepository userTeamSeasonRepository = new UserTeamSeasonRepository();
            UserTeamClassicLeagueRepository userTeamClassicLeagueRepository = new UserTeamClassicLeagueRepository();
            UserTeamH2hLeagueRepository userTeamH2hLeagueRepository = new UserTeamH2hLeagueRepository();

            SqlConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["FantasyPremierLeagueUserTeam"].ConnectionString);

            try
            {
                Logger.Out("Starting...");
                Logger.Out("");

                pageId = 1;
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
                }

                Logger.Out("Starting UserTeams data load");
                Logger.Out("");

                string userTeamUrl = ConfigSettings.ReadSetting("userTeamURL");
                string userTeamLeaguesUrl = ConfigSettings.ReadSetting("userTeamLeaguesURL");

                using (db)
                {
                    db.Open();

                    int leagueId = 314;
                    bool has_next = true;

                    int userTeamRetries = 0;

                    List<int> userTeamIds = userTeamRepository.GetAllUserTeamIds(db);

                    while (has_next == true)
                    {
                        Globals.apiCalls = 0;
                        Globals.apiPageCalls = 0;
                        Globals.apiUserTeamCalls = 0;
                        Globals.apiUserTeamHistoryCalls = 0;
                        Globals.apiUserTeamTransferHistoryCalls = 0;
                        Globals.apiUserTeamPickCalls = 0;
                        Globals.userTeamInsertCount = 0;

                        SetLatestGameweek("UserTeamGameweekHistory");
                        SetActualGameweek("UserTeamGameweekPick");

                        has_next = FantasyPremierLeagueLeaguesAPIClient.GetLeagueDataJson(leagueId, pageId, userTeamIds, userTeamLeaguesUrl, userTeamUrl, userTeamGameweekHistoriesInsert, userTeamPicksInsert, userTeamPickAutomaticSubsInsert, userTeamChipsInsert, userTeamTransferHistoriesInsert, userTeamSeasonsInsert, userTeamClassicLeaguesInsert, userTeamH2hLeaguesInsert, userTeamRetries, db);

                        WriteToDB(pageId, userTeamGameweekHistoriesInsert, userTeamPicksInsert, userTeamPickAutomaticSubsInsert, userTeamChipsInsert, userTeamTransferHistoriesInsert, userTeamSeasonsInsert, userTeamClassicLeaguesInsert, userTeamH2hLeaguesInsert, db);

                        pageId += 1;
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

                //db.Open();

                //If error is thrown from sub program write existing records to the DB
                //WriteToDB(pageId, userTeamGameweekHistoriesInsert, userTeamPicksInsert, userTeamPickAutomaticSubsInsert, userTeamChipsInsert, userTeamTransferHistoriesInsert, userTeamSeasonsInsert, userTeamClassicLeaguesInsert, userTeamH2hLeaguesInsert, db);

                //db.Close();

                throw ex;
            }
        }

        public static void WriteToDB(int pageId, UserTeamGameweekHistories userTeamGameweekHistoriesInsert, UserTeamPicks userTeamPicksInsert, UserTeamPickAutomaticSubs userTeamPickAutomaticSubsInsert, UserTeamChips userTeamChipsInsert, UserTeamTransferHistoryData userTeamTransferHistoriesInsert, UserTeamSeasons userTeamSeasonsInsert, UserTeamClassicLeagues userTeamClassicLeaguesInsert, UserTeamH2hLeagues userTeamH2hLeaguesInsert, SqlConnection db)
        {
            try
            {
                int userTeamPickRowsInserted = 0;
                int userTeamPickAutomaticSubsRowsInserted = 0;
                int userTeamGameweekHistoryRowsInserted = 0;
                int userTeamChipRowsInserted = 0;
                int userTeamTransferHistoryRowsInserted = 0;
                int userTeamSeasonRowsInserted = 0;
                int userTeamClassicLeagueRowsInserted = 0;
                int userTeamH2hLeagueRowsInserted = 0;

                UserTeamRepository userTeamRepository = new UserTeamRepository();
                UserTeamPickRepository userTeamPickRepository = new UserTeamPickRepository();
                UserTeamPickAutomaticSubRepository userTeamPickAutomaticSubRepository = new UserTeamPickAutomaticSubRepository();
                UserTeamGameweekHistoryRepository userTeamGameweekHistoryRepository = new UserTeamGameweekHistoryRepository();
                UserTeamChipRepository userTeamChipRepository = new UserTeamChipRepository();
                UserTeamTransferHistoryRepository userTeamTransferHistoryRepository = new UserTeamTransferHistoryRepository();
                UserTeamSeasonRepository userTeamSeasonRepository = new UserTeamSeasonRepository();
                UserTeamClassicLeagueRepository userTeamClassicLeagueRepository = new UserTeamClassicLeagueRepository();
                UserTeamH2hLeagueRepository userTeamH2hLeagueRepository = new UserTeamH2hLeagueRepository();

                Logger.Out("");

                //Write UserTeamChip to the db
                userTeamChipRowsInserted = userTeamChipRepository.InsertUserTeamChip(userTeamChipsInsert, db);
                Logger.Out("UserTeamChip bulk insert complete");

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

                Logger.Out("");
                Logger.Out("UserTeam: " + Convert.ToString(Globals.userTeamInsertCount) + " rows inserted");
                Logger.Out("UserTeamSeason: " + Convert.ToString(userTeamSeasonRowsInserted) + " rows inserted");
                Logger.Out("UserTeamClassicLeague: " + Convert.ToString(userTeamClassicLeagueRowsInserted) + " rows inserted");
                Logger.Out("UserTeamH2hLeague: " + Convert.ToString(userTeamH2hLeagueRowsInserted) + " rows inserted");
                Logger.Out("UserTeamGameweekHistory: " + Convert.ToString(userTeamGameweekHistoryRowsInserted) + " rows inserted");
                Logger.Out("UserTeamTransferHistory: " + Convert.ToString(userTeamTransferHistoryRowsInserted) + " rows inserted");
                Logger.Out("UserTeamChip: " + Convert.ToString(userTeamChipRowsInserted) + " rows inserted");
                Logger.Out("UserTeamPick: " + Convert.ToString(userTeamPickRowsInserted) + " rows inserted");
                Logger.Out("UserTeamPickAutomaticSub: " + Convert.ToString(userTeamPickAutomaticSubsRowsInserted) + " rows inserted");
                Logger.Out("");

                Logger.Out("API Page calls: " + Convert.ToString(Globals.apiPageCalls));
                Logger.Out("API UserTeam calls: " + Convert.ToString(Globals.apiUserTeamCalls));
                Logger.Out("API UserTeamHistory calls: " + Convert.ToString(Globals.apiUserTeamHistoryCalls));
                Logger.Out("API UserTeamTransferHistory calls: " + Convert.ToString(Globals.apiUserTeamTransferHistoryCalls));
                Logger.Out("API UserTeamPick calls: " + Convert.ToString(Globals.apiUserTeamPickCalls));
                Logger.Out("API calls: " + Convert.ToString(Globals.apiCalls));
                Logger.Out("");

                userTeamPicksInsert.Clear();
                userTeamPickAutomaticSubsInsert.Clear();
                userTeamGameweekHistoriesInsert.Clear();
                userTeamChipsInsert.Clear();
                userTeamTransferHistoriesInsert.Clear();
                userTeamSeasonsInsert.Clear();
                userTeamClassicLeaguesInsert.Clear();
                userTeamH2hLeaguesInsert.Clear();
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

                Globals.latestGameweek = latestGameweek;

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

                Globals.actualGameweek = latestGameweek;

                db.Close();
            }
        }
    }
}