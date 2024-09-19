using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace clawPDF.Signature.Service
{
    public class SignatureService
    {
        public static ISignatureService Instance;
        static SignatureService()
        {
            Instance = new BJCAService();
        }
    }
}
