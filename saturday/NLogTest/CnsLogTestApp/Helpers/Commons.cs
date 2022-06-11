using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CnsLogTestApp
{
    class Commons
    {
        //NLog용 정적(Static) 인스턴스 생성
        public static readonly Logger LOGGER = LogManager.GetLogger("Commons");
    }
}
