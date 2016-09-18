﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CareTrackerV1.Models
{
    public class Visit
    {

        public Visit()
        {
            this.VisitTasks = new HashSet<VisitTask>();
        }

        public int ID { get; set; }

        [Required]
        [Display(Name = "Time of visit")]
        [DataType(DataType.Time)]
        //[DisplayFormat(DataFormatString = "{0:hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime? Time { get; set; }      //Sort out format

        [Required]
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        public int CareGiverID { get; set; }

        public int ClientID { get; set; }

        [Required]
        [Display(Name = "Details")]     //Need to decide how to read this in, eg. List with select and text box for extra details
        public string Details { get; set; }

        [Display(Name = "Alert Type")]
        public string AlertType { get; set; }   //Alert can be generated automatically if CareGiver fails to login or can be generated by CareGiver

        [Display(Name = "Alert details")]
        public string AlertDetails { get; set; }    //Could be selected from a list or details given in a text box??

        
        //Navigation Properties
        //A Visit will have 1 CareGiver only
        [ForeignKey("CareGiverID")]
        public virtual CareGiver CareGiver { get; set; }

        //A Visit will have 1 Client only
        [ForeignKey("ClientID")]
        public virtual Client Client { get; set; }

        //For CheckBox Group
        public virtual ICollection<VisitTask> VisitTasks { get; set; }
        
    }
}