﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVE.Dto
{
    public class LanguageDTO
    {
        public long LanguageId { get; set; }
        public string Language { get; set; }
        public string CountryCode { get; set; }
        public string LanguageCode { get; set; }
        public bool IsDefault { get; set; }
    }
}
