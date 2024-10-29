using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Enum
{
    public static class Enums
    { public enum Role
        {
            Customer,
            Admin
        }
        public enum OrderStatus
        {
            Pending,
            Shipped,
            Delivered,
            Cancelled,
            Returned
        }
        public enum Condition
        {
            New,
            Like_New,
            Good,
            Fair 
        }
        public enum Size
        {
            XS,
            S,
            M,
            L,
            XL,
            twoXL,
            Other
        }
        public enum PaymentMethod
        {
            COD,
            Banking
        }
        public enum PaymentStatus
        {
            Pending,
            Completed,
            Failed
        }
        public enum Bank
        {
            VPBank,               // 12/08/1993 - 79,339.00 VND
            BIDV,                 // 26/04/1957 - 57,004.30 VND
            Vietcombank,          // 01/04/1963 - 55,980.90 VND
            VietinBank,           // 26/03/1988 - 53,699.90 VND
            MBBank,               // 04/11/1994 - 52,140.00 VND
            ACB,                  // 04/06/1993 - 38,841.00 VND
            SHB,                  // 13/11/1993 - 36,194.00 VND
            Techcombank,          // 27/09/1993 - 35,172.00 VND
            Agribank,             // 26/03/1988 - 34,446.86 VND
            HDBank,               // 04/01/1990 - 29,076.00 VND
            LienVietPostBank,     // 28/03/2008 - 25,576.00 VND
            VIB,                  // 18/09/1996 - 25,368.00 VND
            SeABank,              // 24/03/1994 - 24,537.00 VND
            VBSP,                 // 04/10/2002 - 23,960.10 VND
            TPBank,               // 05/05/2008 - 22,016.00 VND
            OCB,                  // 10/06/1996 - 20,548.00 VND
            MSB,                  // 12/07/1991 - 20,000.00 VND
            Sacombank,            // 21/12/1991 - 18,852.20 VND
            Eximbank,             // 24/05/1989 - 17,470.00 VND
            SCB,                  // 01/01/2012 - 15,231.70 VND
            VDB,                  // 19/05/2006 - 15,085.00 VND
            NamABank,             // 21/10/1992 - 10,580.00 VND
            ABBank,               // 13/05/1993 - 10,350.00 VND
            PVcomBank,            // 16/09/2013 - 9,000.00 VND
            BacABank,             // 01/09/1994 - 8,334.00 VND
            UOB,                  // 21/09/2017 - 8,000.00 VND
            Woori,                // 31/10/2016 - 7,700.00 VND
            HSBC,                 // 08/09/2008 - 7,528.00 VND
            SCBVL,                // 08/09/2008 - 6,954.90 VND
            PBVN,                 // 24/03/2016 - 6,000.00 VND
            SHBVN,                // 29/12/2008 - 5,709.90 VND
            NCB,                  // 18/09/1995 - 5,601.55 VND
            VietABank,            // 04/07/2003 - 5,399.60 VND
            VietCapitalBank,      // 25/12/1992 - 5,017.00 VND
            DongABank,            // 01/07/1992 - 5,000.00 VND
            Vietbank,             // 02/02/2007 - 4,776.80 VND
            ANZVL,                // 09/10/2008 - 4,511.90 VND
            OceanBank,            // 30/12/1993 - 4,000.10 VND
            CIMB,                 // 31/08/2016 - 3,698.20 VND
            Kienlongbank,         // 27/10/1995 - 3,653.00 VND
            IVB,                  // 21/01/1990 - 3,377.50 VND
            BAOVIETBank,          // 14/01/2009 - 3,150.00 VND
            SAIGONBANK,           // 16/10/1987 - 3,080.00 VND
            CoOpBank,             // 01/07/2013 - 3,029.63 VND
            GPBank,               // 13/11/1993 - 3,018.00 VND
            VRB,                  // 19/11/2006 - 3,008.40 VND
            CB,                   // 01/09/1989 - 3,000.00 VND
            HLBVN,                // 29/12/2008 - 3,000.00 VND
            PGBank                // 13/11/1993 - 3,000.00 VND
        }

    }
}
