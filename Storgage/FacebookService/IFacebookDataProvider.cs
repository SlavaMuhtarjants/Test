using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weezlabs.Storgage.FacebookService
{
    public interface IFacebookDataProvider
    {
        IDictionary<String, Object> GetUserInfo(String facebookToken);
    }
}
