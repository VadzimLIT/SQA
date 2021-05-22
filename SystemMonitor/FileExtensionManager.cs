using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossCutting
{
    public class FileExtensionManager
    {
        public bool IsValid(string dumpFileName)
        {
            /*Production code to open and process the dump file goes here. The 
business logic determines if the dump file is valid. For clarity we 
will simply simulate this logic by checking the length of the filename and the file extension to determine if the dump file is valid or not.
            */
            if (dumpFileName.Length < 20 && dumpFileName.EndsWith(".dmp"))
            {
                return true;  // Dump file is valid.
            }
            return false;  // Dump file is invalid. 
        }
        // Production code here...etc.
    }


}