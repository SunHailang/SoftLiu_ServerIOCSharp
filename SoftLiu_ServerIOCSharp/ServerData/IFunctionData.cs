using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftLiu_ServerIOCSharp.ServerData
{
    public interface IFunctionData
    {

        ErrorData ErrorData { get; set; }

        void Response();
    }
}
