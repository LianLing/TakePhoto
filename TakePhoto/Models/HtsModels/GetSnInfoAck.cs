﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakePhoto.Models.HtsModels
{
    public class GetSnInfoAck
    {
        public string result { get; set; }    // SUCCESS / :成功， other Defect Code : FAIL
        public string message { get; set; }  // 失败时表示失败信息

        public string sn { get; set; }
        public List<SnInfo> data { get; set; }
    }
}
