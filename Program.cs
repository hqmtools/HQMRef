using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HockeyEditor;

namespace HQMRef
{
    class Program
    {
        static void Main(string[] args)
        {
            MemoryEditor.Init();
            Linesman linesman = new Linesman();            
            
            while(true)
            {
                linesman.CheckForOffside();
                System.Threading.Thread.Sleep(50); //approximates ping
            }
        }

        
    }
}
