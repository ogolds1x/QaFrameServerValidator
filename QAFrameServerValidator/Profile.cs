using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QAFrameServerValidator
{
    public interface Profile
    {
        void Status(Types.Status status);
        Types.Status Status();
        bool Equal(string excludeProfileStr);
        Profile DeepCopy();
    }
}
