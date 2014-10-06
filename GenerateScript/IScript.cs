using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateScript
{
    public interface IScript
    {
        void Update();
        void Rollback();
    }

    public enum Action
    {
        Update = 0,
        Rollback = 1
    }
}
