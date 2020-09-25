using System;
using System.Reflection;
using log4net;

namespace FantasyPremierLeagueUserTeams
{
    public static class Logger
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void Out(string LogText)
        {
            try
            {
                // do the actual work
                _log.Info(LogText);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void Error(string LogText)
        {
            try
            {
                // do the actual work
                _log.Error(LogText);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
