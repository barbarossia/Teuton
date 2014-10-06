using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mail
{
    public interface IMail
    {
        void SendMail(string subject, string message, string attachmentPath);
    }
}
