using System;

namespace NYear.ODA.Model
{
public partial class PRM_USER_AUTHORIZE
{
   public string USER_ID {get; set;}
   public string USER_NAME {get; set;}
   public string RESOURCE_NAME {get; set;}
   public string OPERATE_NAME {get; set;}
   public string IS_FORBIDDEN {get; set;}
   public DateTime? CREATE_DATE {get; set;}
   public string CREATE_BY {get; set;}
} 
}
