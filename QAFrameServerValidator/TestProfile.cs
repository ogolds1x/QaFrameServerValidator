using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QAFrameServerValidator
{
    class TestProfile : Profile
    {
        #region members
        public Types.Status isToDo = Types.Status.Do;
        private string m_command;
        #endregion

        #region constructors
        public TestProfile(string values)
        {
            parse(values);
        }

        public TestProfile(string command, Types.Status status)
        {
            this.m_command = command;
            this.isToDo = status;
        }
        #endregion

        #region public methods
        void Profile.Status(Types.Status status)
        {
            this.isToDo = status;
        }

        Types.Status Profile.Status()
        {
            return this.isToDo;
        }

        bool Profile.Equal(string excludeProfileStr)
        {
            int index = excludeProfileStr.IndexOf("TestProfiles,");
            string cleanExcludeCommand = (index < 0 ? excludeProfileStr : excludeProfileStr.Substring("TestProfiles,".Length));
            return this.m_command == cleanExcludeCommand;
        }

        Profile Profile.DeepCopy()
        {
            return new TestProfile(this.m_command, this.isToDo);
        }

        public string Command
        {
            get { return this.m_command; }
        }

        public override string ToString()
        {
            return String.Format("\t\t{0} (TestProfile): {1}\n", this.isToDo, this.m_command);
        }
        #endregion

        #region private methods
        private void parse(string values)
        {
            this.m_command = values.Trim();
        }
        #endregion


    }
}
