﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTML_Parser.Business.Commands
{
    public interface ICommand
    {
        void Initialize(CommandInfoBase commandInfo);
        void Execute();
    }
}
