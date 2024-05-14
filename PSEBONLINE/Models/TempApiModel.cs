using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.ComponentModel.DataAnnotations;

namespace PSEBONLINE.Models
{
    public class TempApiModel
	{
        public string Type { get; set; }
        public string SearchText { get; set; }
    }
}