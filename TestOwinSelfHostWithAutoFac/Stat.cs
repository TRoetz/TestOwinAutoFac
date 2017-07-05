using System.Collections.Generic;

namespace TestOwinSelfHostWithAutoFac
{
   public class Stat : IStat
   {
      public List<string> GetStats()
      {
         var getStats = new List<string>();

         getStats.Add("TEST 1");
         getStats.Add("TEST 2");
         getStats.Add("TEST 3");
         getStats.Add("TEST 4");
         getStats.Add("TEST 5");

         return getStats;
      }
}
}