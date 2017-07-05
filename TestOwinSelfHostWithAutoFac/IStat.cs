using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestOwinSelfHostWithAutoFac
{
   public interface IStat
   {
      List<string> GetStats();
   }
}
