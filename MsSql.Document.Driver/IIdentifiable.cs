using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsSql.Document.Driver
{
    public interface IIdentifiable
    {
        string Id { get; set; }
    }
}
